using DataLayer;
using HttpServer;
using Models;
using Newtonsoft.Json.Linq;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Battle;

[HttpEndpoint("/battles", HttpMethod.POST, "Auth")]
public class StartBattle : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // get user id from context
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;
        
        // check if user has 4 cards inside his deck
        var userDeck = await CardRepository.GetCardsInDeckByOwner(userId);
        if (userDeck.Length != 4)
        {
            ctx.Response.Json(new {status="error", message = "You need to have 4 cards in your deck to start a battle"});
            ctx.Response.Status = 403;
            ctx.Response.StatusMessage = "Forbidden";
            return ctx;
        }
        
        // enter lobby
        await BattleRepository.EnterLobby(userId);

        Guid opponentId;
        
        // wait for opponent
        while (true)
        {
            var temp = await BattleRepository.GetOpponent(userId);
            if (temp != Guid.Empty)
            {
                opponentId = (Guid)temp!;
                break;
            }
            await Task.Delay(1000);
        }
        
        // leave lobby
        await BattleRepository.LeaveLobby(userId);
        
        // get decks of opponent
        var opponentDeck = await CardRepository.GetCardsInDeckByOwner(opponentId);
        
        // check if battle is already handled by other request
        var battleCheck = await BattleRepository.CheckIfBattleExists(userId, opponentId);
        var battleId = battleCheck.Item1;

        // add the battle ID to the response
        ctx.Response.Json(new
        {
            status = "ok",
            data = new
            {
                battleId = battleCheck.Item1
            }
        });
        
        // if battle is already handled, return
        if (battleCheck.Item2)
            return ctx;


        // create battle
        var battle = new GameLogic.Battle(userDeck, opponentDeck);
        var winner = battle.Play();
        
        Guid? winnerId = null;
        switch (winner)
        {
            case Winner.Player1:
                winnerId = userId;
                break;
            case Winner.Player2:
                winnerId = opponentId;
                break;
        }
        
        // update card ownership and remove them from deck
        await CardRepository.UpdateDeck(userId, battle.GetDeck1().Select(x => (Guid)x.Id).ToArray());
        await CardRepository.UpdateDeck(opponentId, battle.GetDeck2().Select(x => (Guid)x.Id).ToArray());

        Console.WriteLine($"Battle between {userId} and {opponentId} finished. Winner: {winnerId}");


        
        JObject logJson = new()
        {
            ["battleId"] = battleId,
            ["winnerId"] = winnerId,
            [userId.ToString()] = JToken.FromObject(battle.Deck1Log.GetEntries()),
            [opponentId.ToString()] = JToken.FromObject(battle.Deck2Log.GetEntries())
        };


        // save battle
        await BattleRepository.ConcludeBattle(battleId, logJson);
        
        // update stats
        await BattleRepository.UpdateStats(userId, opponentId, winnerId);
       
        return ctx;
    }
}