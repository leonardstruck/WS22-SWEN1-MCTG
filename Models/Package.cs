using Models.Card;

namespace Models;

public class Package
{
    public Guid? Id { get; set; }
    public GenericCard[] Cards { get; set; }
    
    public Package(GenericCard[] cards)
    {
        Cards = cards;
    }
}