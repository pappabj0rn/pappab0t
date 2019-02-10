using System;
using System.Collections.Generic;
using pappab0t.Extensions;

namespace pappab0t.Models
{
    public class Phrasebook : IPhrasebook
    {
        private readonly Random _random;

        public Phrasebook(Random random)
        {
            _random = random;
        }

        public string Affirmation()
        {
            return _random.SelectOne(new[]{
                "ok",
                "råger",
                "roger",
                "roger (moore)",
                "roger (pontare)",
                "roger (federer)",
                "jag hör dig.",
                "absolut"
            });
        }

        public string ThankYou()
        {
            return _random.SelectOne(new[]{
                "Tack!",
                "Tack.",
                "tanks.",
                "tack o bock.",
                "man tackar, man tackar!",
                "ty.",
                ":thumbsup:",
                "tank you!",
                "T.Hanks"
            });
        }

        public string Exclamation()
        {
            return _random.SelectOne(new[]{
                "Fuck YEAH!",
                "Whooo!",
                "whoppi do!",
                "Wiiii!",
                "Satan va kul!",
                "Grattis!",
                "OMG",
                ":smiley:",
                ":sunglasses:",
                ":parrot:",
                ":pbjt:",
                ":tinfoil:"
            });
        }

        public string MutedExclamation()
        {
            return _random.SelectOne(new[]{
                "Kul.",
                "Jora, så atte...",
                "Inte illa.",
                "Heja.",
                ":thumbsup:",
                "grattis.",
                ":clap:",
                ":smiley:"
            });
        }

        public string AttentionResponse(string input)
        {
            var text = (input ?? "").Replace(" pb0t", "").Replace("pappab0t", "").Trim();

            var response = new List<string>();

            switch (text)
            {
                case "hej":
                case "tja":
                case "tjena":
                    response.AddRange(new[]
                    {
                        "hej",
                        "tja",
                        "hallå",
                        "yo",
                        "tjabba",
                        "tjena"
                    });
                    break;

                case "yo":
                    response.AddRange(new[]
                    {
                        "yo!",
                        "yo man",
                        "läget?"
                    });
                    break;

                case "hi":
                    response.AddRange(new[]
                    {
                        "hi hi",
                        "hi",
                        "hi som i hej? hej.",
                        "?",
                        "nåt som är kul?",
                        "har jag missat något?"
                    });
                    break;
                case "hello":
                    response.AddRange(new[]
                    {
                        "haj!",
                        "svenska plz.",
                        "jag kan svenska, men, hej!",
                        "hi",
                        "hello"
                    });
                    break;
                case "mrn":
                case "morrn":
                case "nirrb":
                    response.AddRange(new[]
                    {
                        "morrn",
                        "nirrb",
                        "mrn"
                    });
                    break;
                default:
                    response.AddRange(new[]
                    {
                        "Hej du. Vad är upp?",
                        "Hej cepe, behöver du hjälp?",
                        "Tjena tjockis! Hur kan jag stå till tjänst?",
                        "*[yawns]*. Uj uj, ledsn fö dä. Va kan ja gö fö dä?",
                        "pb0t tll edr tjnst!"
                    });
                    break;
            }

            return _random.SelectOne(response);
        }

        public string ScoreboardHype()
        {
            return _random.SelectOne(new[]{
                "Ok, så här ser det ut ATM.",
                "Ställningen är som följer:",
                "Ok, en liten uppdatering om ställningarna då.",
            });
        }

        public string YoureWelcome()
        {
            return _random.SelectOne(new[]{
                "np",
                "lugnt",
                "esch då",
                "^^",
                "Det var så lite så."
            });
        }

        public string OpenAppology()
        {
            return _random.SelectOne(new[]{
                "sry, men",
                "ursäkta mig, men",
                "Du får ursäkta mig, men",
                "du får ursäkta mig, men",
                "hmm,",
                "Hmm,"
            });
        }

        public string IDontKnowXxxNamedYyy(string xxx, string yyy)
        {
            return _random.SelectOne(new[]{
                $"jag känner inte till {xxx} som heter {yyy}",
                $"jag har inte koll på {xxx} som heter {yyy}",
                $"mina böcker nämner inte {xxx} vid namn {yyy}"
            });
        }

        public string IDidntUnderstand()
        {
            return _random.SelectOne(new[]{
                "Det fär förstod jag inte",
                "va?",
                "huh?",
                "?",
                "...?",
                "FLAGRANT SYSTEM ERROR",
                "Flamboyant System Error",
                "Oh, Child!",
                "FRAGRANT SYSTEM ERROR",
                "Monkey, hush."
            });
        }

        public string PlayInsufficientFunds(decimal reqMoney)
        {
            return _random.SelectOne(
                new[]
                {
                    $"För lite pengar i plånkan, Du behöver {reqMoney:C}.",
                    $"Det kostart {reqMoney:C} att göra det där, vilket du inte har.",
                    $"Återkom när du har minst {reqMoney:C}.",
                    $"{reqMoney:C} först, sen kan vi prata.",
                    $"Inga rabatter, {reqMoney:C} kostar det."
                });
        }

        public string PlayDidntMakeHighScore(int points)
        {
            return _random.SelectOne(new[]
            {
                $"Du fick {points}p, vilket tyvärr inte tar dig in på highscore-listan :(",
                $"{points}p! Tyvärr räcker det inte.",
                $"{points}p, bättre lycka nästa gång!"
            });
        }

        public string DidntMakeHighScoreInCount(int points, int turns)
        {
            return _random.SelectOne(new[]{
                $"Du fick som mest {points}p över {turns} spel, vilket tyvärr inte tar dig in på highscore-listan :(",
                $"{points}p ({turns} spel)! Tyvärr räcker det inte.",
                $"{points}p över {turns} spel, bättre lycka nästa gång!",
                $"Asså, det är ju inte dåligt att få {points}p på {turns} spel, det är det inte, men det räcker inte till nån highscore - om en så säg."
            });
        }

        public string MoneyTransfered(decimal amount)
        {
            return _random.SelectOne(new[]{
                $"{amount:C} överlämnade.",
                $"{amount:C} tadda o gädda.",
                $"{amount:C} transfererade.",
                $"{amount:C} borta.",
                $"Du har blivit {amount:C} fattigare.",
                $"Du är nu {amount:C} fattigare.",
                $"Minns du de där {amount:C} du hade? Inte jag heller."
            });
        }

        public string MoneyTransferInsufficientFunds()
        {
            return _random.SelectOne(new[]{
                "Du har inte så mycket pengar.",
                "Du måste ha så mycket pengar för att kunna ge bort dem.",
                "du kan'te ge mer än du har, robin.",
                "det funkar inte så.",
                "det är mer än du har..."
            });
        }

        public string ItemDescription(string typeName, string description)
        {
            return _random.SelectOne(new[]{
                $"Sak: {typeName.ToLower()}. \r\n\"Beskrivning: {description}\"",
                $"{typeName.ToLower()}; \r\n\"{description}\"",
                $"{description} (typ: {typeName.ToLower()})",
                $"Låt mig beskriva saken ({typeName.ToLower()}) så här:\r\n{description}",
                $"{description}",
                $"{description}",
                $"{description}",
                $"{description}",
                $"{description}",
                $"{description}"
            });
        }

        public string ItemTransfered(string item)
        {
            return _random.SelectOne(new[]{
                $"{item} överlämnad.",
                $"{item} har bytt ägare.",
                $"{item} är inte längre i din ägo.",
                "Din väska blev just lättare.",
                "Din börda blev just lättare.",
                "Visst är det skönt att göra sig av med saker?"
            });
        }

        public string ItemCreated(string item)
        {
            return _random.SelectOne(new[]{
                $"{item} skapad.",
                $"{item} skapades.",
                $"{item} frammanad.",
                $"{item} framtrixad."
            });
        }

        public string ItemTransferToFewItems()
        {
            return _random.SelectOne(new[]{
                "du har inte så många grejer",
                "Tips: du kan kolla vad för saker du har med i-kommandot.",
                "Du kan inte ge bort saker du inte har.",
                "Jag skulle verkligen vilja hjälpa dig, men att flytta nåt du inte har klarar jag inte.",
                "Vad har vi här? Ett litet OutOfRangeException? Hur kommer det sig, tror du?"
            });
        }

        public string Impossible()
        {
            return _random.SelectOne(new[]{
                "Så kan man inte göra.",
                "Så kan man inte göra, fast det visste du säkert.",
                "Nej",
                "Den gubben går inte!",
                "Ömpösibbl!",
                "Vissa saker går, andra går inte. Det där gick inte.",
                "Men, det är ju omöjligt!",
                "asså, ja, nej, det går inte att göra.",
                "... hur?",
                "jo tjena",
                "du vet att man inte kan göra så.",
                "Trodde du att det skulle gå?"
            });
        }

        public string CantMoveSoulboundItems()
        {
            return _random.SelectOne(new[]{
                "Överföring av själabundna saker låter sig ikke göras!",
                "Nej, den är din för evigt.",
                "Nej, den är din för evigt. Ja, om det nu inte går att koppla loss den från din själv på nå sätt, men jag vet inte ja'.",
                "Nej, den är en del av dig.",
                "Själabunden."
            });
        }

        public string PotPayout(int points, int position, decimal money, int turns)
        {
            return _random.SelectOne(new[]{
                $"Ny highscore! {points}p tar dig till plats {position} och ger dig {money:C} :D ({turns} spel)",
                $"Vinnare! {points}p => plats {position} => {money:C}. ({turns} spel)",
                $"{money:C} utdelning! {points}p (pos. {position}) ({turns} spel)"
            });
        }

        public string NewHighscore(string hsName, string player, int position)
        {
            return _random.SelectOne(new[]{
                $"Ny higscore i {hsName}! {player} tog plats {position} :D",
                $"{player} knep just plats {position} på {hsName}-listan!",
                $"Plats {position} på {hsName}-listan tillhör nu {player}"
            });
        }

        public string NoPoints()
        {
            return _random.SelectOne(new[]{
                "Tyvärr, inga poäng denna gång :(",
                "Det här var inte din gång :/",
                "Prova igen!",
                "Rackarns! Inga poäng.",
                "zéro point",
                "pas des point",
            });
        }

        public string Noted()
        {
            return _random.SelectOne(new[]{
                "Noterat.",
                "Antecknat.",
                "Sparat.",
                "Ok"
            });
        }

        public string TauntOld()
        {
            return _random.SelectOne(new[]{
                "OÄZ!",
                "oäz!",
                "old!",
                "OÄZ.",
                "oäz.",
                "Gammal potatis.",
            });
        }

        public string CreditUserBecause(string userUuid, string reason)
        {
            return _random.SelectOne(new[]{
                $"All hail, <@{userUuid}>, {reason}!",
                $"cred till <@{userUuid}>, {reason}.",
                $"<@{userUuid}>, {reason}.",
                $"{reason}, <@{userUuid}>."
            });
        }

        public string DescribeItemToFewItems()
        {
            return _random.SelectOne(new[]{
                "Du har inte så många grejer",
                "Tips: du kan kolla vad för saker du har med i-kommandot.",
                "Jag kan bara föreställa mig vad det skulle kunna vara.",
                "Ptja, det skulle kunna vara en fisk.",
                "Det är ett s.k. Out of range exception.",
                "Vad har vi här? Ett litet OutOfRangeException? Hur kommer det sig, tror du?"
            });
        }

        public string DescribeUser()
        {
            return _random.SelectOne(new[]{
                "Jag vet inte var jag ska börja.",
                "Det är en användare.",
                "Det är en användare av bästa sort.",
                "Det är en användare i sina bästa år.",
                "Hen är väl kul.",
                "Jag har inget ont att säga om den personen.",
                "Hen är helt ok."
            });
        }

        public string QuestionAction()
        {
            return _random.SelectOne(new[]{
                "Så, vad var det som fick dig att säga så?",
                "Varför säger du det till mig?",
                "Pratar du med mig?",
                "Hmm?",
                "?",
                "vad förväntar du dig av mig?",
                "jag vet inte vad du vill riktigt",
                "What's this, I don't even.. wha?"
            });
        }

        public string NoDataFound()
        {
            return _random.SelectOne(new[]{
                "Hittade inget.",
                "Inget sånt i arkivet.",
                "Där var det tomt.",
                "Ingen utdelning.",
            });
        }

        public string QuestionSimilarUrl()
        {
            return _random.SelectOne(new[]{
                "Har redan en väldigt lik url sparad, men kan inte avgöra om din är unik",
                "Snudd på samma url som redan finns reggad.",
                "Som två tvillingurlar separerade vid födseln.",
                "Urlsch!",
                "Urlaberla"
            });
        }

        public string TimedBombExpired()
        {
            return _random.SelectOne(new[]
            {
                "Poff!",
                "Thatsa one spicy ex bomb!",
                "Var det nån som fes, eller var det en bomb som small? Det var en bomb.",
                "*poff*",
                "pläpp!",
                "*sad trombone* :boom:",
                ":mindblown:",
                ":bomb:",
                ":boom:",
                "la bomba",
                ":fireworks:"
            });
        }
    }
}