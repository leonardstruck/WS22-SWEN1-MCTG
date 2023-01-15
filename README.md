# WS22/SWEN1 Semesterprojekt MCTG

[Link zum GitHub Repository](https://github.com/leonardstruck/WS22-SWEN1-MCTG.git)

## Protokoll

### 1. Einleitung

In diesem Protokoll werden die einzelnen Schritte der Entwicklung des Semesterprojekts dokumentiert. 
Dabei wird auf die einzelnen Komponenten des Projektes eingegangen und die Lösungen beschrieben. 
Die Komponenten sind in der Reihenfolge ihrer Abhängigkeiten aufgelistet.

### 2. Komponenten

#### 2.1. HTTP-Server

Der HTTP-Server wurde mit eigenen Klassen implementiert.
Diese Klassen sind in der Subsolution "HttpServer" des Hauptprojekts enthalten.

##### 2.1.1. HTTP-Context

Der HTTP-Context ist eine Klasse, die die Informationen eines HTTP-Requests, wie z.B. die URL, die HTTP-Methode, die Header und die Daten, enthält.
Neben den Informationen des Requests, enthält der Context auch die Informationen des Responses, wie z.B. die Statuscode, die Header und die Daten.
Der Context wird von den später implementierten HTTP-Controllern verwendet, um die Informationen des Requests zu verarbeiten und die Informationen des Responses zu setzen.

###### 2.1.1.1 HTTP-Request

Der HTTP-Request ist eine Klasse, die die Informationen eines HTTP-Requests, wie z.B. die URL, die HTTP-Methode, die Header und die Daten, enthält.
Die Informationen können mit den entsprechenden Getter-Methoden abgerufen werden.

Bevor die Informationen des Requests verarbeitet werden können, muss der Request mit der Methode `CreateInstance` eingelesen werden.
Dabei wird der Request aus dem übergebenen `NetworkStream` eingelesen und die Informationen des Requests werden in den entsprechenden Properties gespeichert.

Der Body des Requests kann mit der Methode `DeserializeBody<T>` in ein Objekt des Typs `T` deserialisiert werden.

###### 2.1.1.2 HTTP-Response

Der HTTP-Response ist eine Klasse, die die Informationen eines HTTP-Responses, wie z.B. den Statuscode, die Header und die Daten, enthält.
Die Informationen können mit den entsprechenden Setter-Methoden gesetzt werden.

Um die Klasse zu verwenden, muss im Konstruktor der `NetworkStream` übergeben werden.
Dabei wird der `NetworkStream` verwendet, um die Informationen des Responses zu schreiben.

Der Body des Responses kann entweder mit dem Setter `Body` gesetzt werden oder mit der Methode `Json` kann ein beliebiges Objekt in einen String deserialisiert werden.
Wenn die Methode `Json` verwendet wird, wird der Header `Content-Type` auf `application/json` gesetzt und etwaige Daten im Body werden überschrieben.

Mit der Methode `Send` wird die Response an den Client gesendet.

#### 2.1.2. HTTP-Middleware

Middleware ist eine Funktion, die zwischen dem HTTP-Server und den HTTP-Controllern eingefügt wird.
Die Middleware kann die Informationen des Requests verarbeiten und die Informationen des Responses setzen.
Besonders nützlich ist die Middleware, um die Authentifizierung und Autorisierung zu implementieren. 
Darauf wird später noch eingegangen.

Mit dem Middleware Interface können eigene Middleware-Klassen implementiert werden.
Diese Middleware-Klassen können dann in der Middleware-Kette des HTTP-Servers eingebunden werden.

Um Middleware-Klassen zu implementieren, muss die Klasse das Interface `IMiddleware` implementieren.

Die Middleware-Klassen müssen mit einem Key-Attribut annotiert werden, um sie in der Middleware-Kette identifizieren zu können.

#### 2.1.3. HTTP-Endpoint

Ein HTTP-Endpoint wird durch das Interface `IEndpointController` definiert.
Ein HTTP-Endpoint besteht aus einer URL, einer HTTP-Methode und einer Middleware-Kette.
Die Verarbeitung des Requests wird in der Methode `HandleRequest` implementiert.

Der Pfad kann mithilfe des Symbols `*` als Platzhalter für beliebige Zeichen definiert werden.
So sind z.B. die Pfade `/users/*` und `/users/123` gleich.

Die Middleware-Kette wird durch die Aneinanderreihung von Middleware-Keys beschrieben.
Dabei wird die Middleware-Kette von links nach rechts durchlaufen.

Die Attribute `Path` und `Method` müssen mit den entsprechenden Werten annotiert werden.
Falls eine Middleware verwendet werden soll, muss die Middleware-Kette ebenfalls annotiert werden.

Im folgenden Beispiel wird ein HTTP-Endpoint implementiert, der die URL `/users/*` mit der HTTP-Methode `GET` verarbeitet.
Die Middleware-Kette besteht aus dem Key `Auth` und `Auth_PathMustMatchToUsername`.

```csharp
[HttpEndpoint("/users/*", HttpMethod.GET, "Auth", "Auth_PathMustMatchToUsername")]
public class GetUser : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // ... Verarbeitung des Requests
        
        return ctx;
    }
}
```

#### 2.1.4. Resolver

Die Resolver sind Hilfsklassen, die in der Assembly des Projekts nach Klassen suchen, die das Interface `IEndpointController`, bzw. `IMiddleware` implementieren.

Die Klasse EndpointResolver sucht nach Klassen, die das Interface `IEndpointController` implementieren.
Die Klasse MiddlewareResolver sucht nach Klassen, die das Interface `IMiddleware` implementieren.

Beide Resolver funktionieren prinzipiell gleich. Die Assembly des Projekts wird durchsucht und die gefundenen Klassen werden in einer Liste gespeichert.
Danach wird für jede gefundene Klasse eine Instanz erzeugt und zurückgegeben.

Durch diese Hilfsklassen können die Instanzen der HTTP-Endpoints und Middleware-Klassen in der `Program.cs` des Projekts einfach automatisch angelegt und müssen nicht händisch erzeugt werden.

Bei einer Vielzahl an HTTP-Endpoints und Middleware-Klassen erleichtert dies die Implementierung des HTTP-Servers enorm.

#### 2.1.4. Listener

Der Listener ist eine Klasse, die auf eingehende Verbindungen wartet, diese akzeptiert, 
einen eigenen Thread für die Verbindung erstellt und den Request dann an den entsprechenden Controller weiterleitet.

Der Listener wird in der `Program.cs` des Projekts erzeugt und gestartet.
In der Listener Klasse müssen die Http-Endpoints und die Middleware-Kette registriert werden.
Dies geschieht in der Methode `RegisterEndpoints`, bzw. `RegisterMiddleware`.

Nachdem der Listener mit der Methode `Start` gestartet wurde, wartet er auf eingehende Verbindungen.
Wenn eine Verbindung eingegangen ist, wird ein neuer Thread gestartet, der den Request an den entsprechenden Controller weiterleitet.

### 2.2. Modelle und Datenbank

#### 2.2.1. Modelle

Die Modelle sind die Datenstrukturen, die in dem Projekt verwendet werden.
Die Modelle werden in dem Subprojekt `Models` definiert.

##### 2.2.1.1 Karten

Die Karten werden in dem Interface `ICard` definiert.
Man unterscheidet zwischen den Kartentypen MonsterCard, bzw. SpellCard.

Da in den gegebenen Integrationtests die Karten aber nur als String repräsentiert werden,
gibt es eine zusätzliche Hilfsklasse `GenericCard`, die den String in ein Card-Objekt umwandeln kann.

##### 2.2.1.2 Monsterkarten / Spellkarten

Für die Monsterkarten und Spellkarten gibt es jeweils eine eigene Klasse, die das Interface `ICard` implementieren.
Die Monsterkarten werden in der Klasse `MonsterCard` definiert.
Die Spellkarten werden in der Klasse `SpellCard` definiert.

Jedes Monster, bzw. jede Spellkarte hat eine eigene Klasse, die das Interface `ICard` implementiert, 
bzw. von der Klasse `SpellCard` oder `MonsterCard` erbt.

So können die Monsterkarten und Spellkarten einfach erweitert werden, und bei Bedarf neue Monsterkarten und Spellkarten hinzugefügt werden.

##### 2.2.1.3 GenericCard

Die Klasse `GenericCard` ist eine Hilfsklasse, die einen String in ein Card-Objekt umwandelt.

Wenn der Name der Karte `Spell` enthält, wird ein SpellCard-Objekt erzeugt. (Hier reicht ein einfacher String-Vergleich)

Wenn der Name der Karte `Monster` enthält, wird ein MonsterCard-Objekt erzeugt.
Dies erfolgt wieder durch die Verwendung von Reflection.

#### 2.2.2. Datenbank

Die Daten werden in einer PostgreSQL-Datenbank persistiert.
Für die Kommunikation mit der Datenbank wird die Bibliothek `Npgsql` verwendet.

Für jedes Modell, dessen Daten in der Datenbank persistiert werden sollen, gibt es eine eigene Klasse,
die die Datenbankoperationen für dieses Modell implementiert.

Um die Datenbank für die Integrationstests zu füllen, gibt es eine Klasse `SeedRepository`, die die Datenbank mit dem erforderlichen
Schema initialisiert. 
Alternativ kann auch das Skript `seed.sql` verwendet werden.

### 2.3. Spiellogik

Die Spiellogik wird in dem Subprojekt `GameLogic` implementiert.
Durch die Trennung der Spiellogik von der REST-Schnittstelle, kann die Spiellogik auch in anderen Projekten verwendet werden.
Dies erleichtert die Implementierung von z.B. einem Desktop-Client, oder
Änderungen an der REST-Schnittstelle, ohne die Spiellogik zu verändern.

### 2.4. HTTP-Controller

Die HTTP-Controller sind die Klassen, die die REST-Schnittstelle implementieren.
Die HTTP-Controller werden in dem Subprojekt `HttpController` definiert.

Besonders möchte ich hier auf die Verwendung von Middleware eingehen.
Die Middleware-Kette erlaubt es einmal die Logik für die Authentifizierung, bzw. Autorisierung an einer Stelle zu implementieren.

So kommt es nicht zu Code-Duplikation, da ohne Middleware die Authentifizierung, bzw. Autorisierung in jeder Methode der HTTP-Controller implementiert werden müsste.

Sobald beispielsweise der Key `Auth` in der Middleware-Kette registriert wurde, wird sichergestellt, dass nur
angemeldete Benutzer die HTTP-Endpoints aufrufen können.

Zusätzlich kann die Middleware-Kette auch verwendet werden, um die Logik für die Validierung der HTTP-Requests an einer Stelle zu implementieren.

Beispielsweise wird mit der Middleware `SchemaValidation_Credentials` bei der Registrierung eines neuen Benutzers sichergestellt, dass der Benutzername und das Passwort nicht leer sind.

### 2.5. Unit-Tests

Die Unit-Tests werden in dem Subprojekt `Test` mit dem Testframework `NUnit` implementiert.
Bei den Unit-Tests wurde auf die Verwendung von Mocks verzichtet, da die Integrationstests die Funktionalität der REST-Schnittstelle testen.

Die Unit-Tests testen die Funktionalität der einzelnen Klassen, bzw. Methoden.

Um zu testen, ob die Battle-Logik korrekt funktioniert, wurden die Beispielkämpfe aus der Aufgabenstellung in Unit-Tests implementiert.
Außerdem wurden Kämpfe zwischen allen "besonderen" Karten getestet um sicherzustellen, dass ihre Fähigkeiten berückstichtigt werden.

Des Weiteren wird die Umwandlung von Strings, bzw. Generic Cards in Monster- bzw. Spell-Cards getestet.

Die Berechnung der ELO-Werte wird ebenfalls getestet.
Da sich die ELO-Werte abhängig von dem eigenen und dem gegnerischen ELO-Wert berechnen, werden hier vorgegebene ELO-Werte verwendet.

Jeder Unit-Test ist meines Erachtens nach sinnvoll und ausreichend, da z.B. bei einer fehlerhaften
Berechnung der ELO-Werte, oder der Kampflogik ein unfaires Spiel entstehen würde.

### 2.6. Integration-Tests

Die Integration-Tests werden mit dem Tool `Postman` durchgeführt.
Alle Integration-Tests sind in der Datei `IntegrationTest.postman_collection.json` gespeichert.

Die Integration-Tests testen die Funktionalität der REST-Schnittstelle. 

Neben den vorgegebenen Tests aus dem Batch-Skript wurden auch einige zusätzliche Tests implementiert.

Insgesamt werden 133 Tests durchgeführt.

## 3. Unique Mandatory Feature

Als Unique Mandatory Feature habe ich mich für die Implementierung eines Daily Rewards entschieden.

Spieler haben die Möglichkeit, täglich einen Bonus zu erhalten, wenn sie einen POST-Request an den Endpoint `/dailyReward` senden.

Der Bonus wird in Form von 10 Coins gutgeschrieben.

Sobald der Bonus abgeholt wurde, kann er nicht noch einmal abgeholt werden.

## 4. Zeitaufwand

Für die Umsetzung des Projektes habe ich insgesamt 60 Stunden benötigt.

## 5. Fazit

Ich bin mit dem Ergebnis des Projektes sehr zufrieden. 

Bei einem eigenen Projekt würde ich jedoch auf
vorhandene Bibliotheken zurückgreifen, um die Implementierung der REST-Schnittstelle zu vereinfachen.

