using System;
using System.Collections.Generic;
using pappab0t.Extensions;

namespace pappab0t.Models
{
    public class Phrasebook : IPhrasebook
    {
        public string Affirmation()
        {
            return new[]{
                "ok",
                "råger",
                "roger",
                "jag hör dig.",
                "absolut"
            }.Random();
        }

        public string ThankYou()
        {
            return new[]{
                "Tack!",
                "Tack.",
                "tanks.",
                "tack o bock.",
                "man tackar, man tackar!",
                "ty.",
                ":thumbsup:",
                "tank you!"
            }.Random();
        }

        public string Exclamation()
        {
            return new[]{
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
            }.Random();
        }

        public string MutedExclamation()
        {
            return new[]{
                "Kul.",
                "Jora, så atte...",
                "Inte illa.",
                "Heja.",
                ":thumbsup:",
                "grattis.",
                ":clap:",
                ":smiley:"
            }.Random();
        }

        public string AttentionResponse(string input)
        {
            var text = (input??"").Replace(" pb0t", "").Replace("pappab0t", "").Trim();

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

            return response.Random();
        }

        public string ScoreboardHype()
        {
            return new[]{
                "Ok, så här ser det ut ATM.",
                "Ställningen är som följer:",
                "Ok, en liten uppdatering om ställningarna då.",
            }.Random();
        }

        public string YoureWelcome()
        {
            return new[]{
                "np",
                "lugnt",
                "esch då",
                "^^",
                "Det var så lite så."
            }.Random();
        }

        public string OpenAppology()
        {
            return new[]{
                "sry, men",
                "ursäkta mig, men",
                "Du får ursäkta mig, men",
                "du får ursäkta mig, men",
                "hmm,",
                "Hmm,"
            }.Random();
        }

        public string IDontKnowXxxNamedYyyFormat()
        {
            return new[]{
                "jag känner inte till {0} som heter {1}",
                "jag har inte koll på {0} som heter {1}",
                "mina böcker nämner inte {0} vid namn {1}"
            }.Random();
        }

        public string IDontKnowXxxNamedYyy(string xxx, string yyy)
        {
            return new[]{
                $"jag känner inte till {xxx} som heter {yyy}",
                $"jag har inte koll på {xxx} som heter {yyy}",
                $"mina böcker nämner inte {xxx} vid namn {yyy}"
            }.Random();
        }

        public string IDidntUnderstand()
        {
            return new[]{
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
            }.Random();
        }

        public string InsufficientFundsFormat()
        {
            return new[]{
                "För lite pengar i plånkan, Du behöver {0}kr.",
                "Det kostart {0}kr att göra det där, vilket du inte har.",
                "Återkom när du har minst {0}kr.",
                "{0}kr först, sen kan vi prata.",
                "Inga rabatter, {0}kr kostar det."
            }.Random();
        }

        public string PlayInsufficientFunds(decimal required)
        {
            throw new NotImplementedException();
        }

        public string DidntMakeHighScoreFormat()
        {
            return new[]{
                "Du fick {0}p, vilket tyvärr inte tar dig in på highscore-listan :(",
                "{0}p! Tyvärr räcker det inte.",
                "{0}p, bättre lycka nästa gång!"
            }.Random();
        }

        public string PlayDidntMakeHighScore(int points)
        {
            throw new NotImplementedException();
        }

        public string DidntMakeHighScoreInCountFormat()
        {
            return new[]{
                "Du fick som mest {0}p över {1} spel, vilket tyvärr inte tar dig in på highscore-listan :(",
                "{0}p ({1} spel)! Tyvärr räcker det inte.",
                "{0}p över {1} spel, bättre lycka nästa gång!"
            }.Random();
        }

        public string DidntMakeHighScoreInCount(int points, int turns)
        {
            throw new NotImplementedException();
        }

        public string MoneyTransfered(decimal amount)
        {
            return new[]{
                $"{amount:C} överlämnade.",
                $"{amount:C} tadda o gädda.",
                $"{amount:C} transfererade.",
                $"{amount:C} borta.",
                $"Du har blivit {amount:C} fattigare.",
                $"Du är nu {amount:C} fattigare.",
                $"Minns du de där {amount:C} du hade? Inte jag heller."
            }.Random();
        }

        public string MoneyTransferInsufficientFunds()
        {
            return new[]{
                "Du har inte så mycket pengar.",
                "Du måste ha så mycket pengar för att kunna ge bort dem.",
                "du kan'te ge mer än du har, robin.",
                "det funkar inte så.",
                "det är mer än du har..."
            }.Random();
        }

        public string ItemDescription(string typeName, string description)
        {
            return new[]{
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
            }.Random();
        }

        public string ItemTransfered(string item)
        {
            return new[]{
                $"{item} överlämnad.",
                $"{item} har bytt ägare.",
                $"{item} är inte längre i din ägo.",
                "Din väska blev just lättare.",
                "Din börda blev just lättare.",
                "Visst är det skönt att göra sig av med saker?"
            }.Random();
        }

        public string ItemCreated(string item)
        {
            return new[]{
                $"{item} skapad.",
                $"{item} skapades.",
                $"{item} frammanad.",
                $"{item} framtrixad."
            }.Random();
        }

        public string ItemTransferToFewItems()
        {
            return new[]{
                "du har inte så många grejer",
                "Tips: du kan kolla vad för saker du har med i-kommandot.",
                "Du kan inte ge bort saker du inte har.",
                "Jag skulle verkligen vilja hjälpa dig, men att flytta nåt du inte har klarar jag inte.",
                "Vad har vi här? Ett litet OutOfRangeException? Hur kommer det sig, tror du?"
            }.Random();
        }

        public string Impossible()
        {
            return new[]{
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
            }.Random();
        }

        public string CantMoveSoulboundItems()
        {
            return new[]{
                "Överföring av själabundna saker låter sig ikke göras!",
                "Nej, den är din för evigt.",
                "Nej, den är din för evigt. Ja, om det nu inte går att koppla loss den från din själv på nå sätt, men jag vet inte ja'.",
                "Nej, den är en del av dig.",
                "Själabunden."
            }.Random();
        }

        public string PotPayoutFormat()
        {
            return new[]{
                "Ny highscore! {0}p tar dig till plats {1} och ger dig {2}kr :D ({3} spel)",
                "Vinnare! {0}p => plats {1} => {2}kr. ({3} spel)",
                "{2}kr utdelning! {0}p (pos. {1}) ({3} spel)"
            }.Random();
        }

        public string PotPayout(decimal money)
        {
            throw new NotImplementedException();
        }

        public string NewHighscoreFormat()
        {
            return new[]{
                "Ny higscore i {0}! {1} tog plats {2} :D",
                "{1} knep just plats {2} på {0}-listan!",
                "Plats {2} på {0}-listan tillhör nu {1}"
            }.Random();
        }

        public string NewHighscore(string hsName, string player, int position)
        {
            throw new NotImplementedException();
        }

        public string NoPoints()
        {
            return new[]{
                "Tyvärr, inga poäng denna gång :(",
                "Det här var inte din gång :/",
                "Prova igen!",
                "Rackarns! Inga poäng.",
                "zéro point",
                "pas des point",
            }.Random();
        }

        public string Noted()
        {
            return new[]{
                "Noterat.",
                "Antecknat.",
                "Sparat.",
                "Ok"
            }.Random();
        }

        public string TauntOld()
        {
            return new[]{
                "OÄZ!",
                "oäz!",
                "old!",
                "OÄZ.",
                "oäz.",
                "Gammal potatis.",
            }.Random();
        }

        public string CreditUserBecauseFormat()
        {
            return new[]{
                "All hail, <@{0}>, {1}!",
                "cred till <@{0}>, {1}.",
                "<@{0}>, {1}.",
                "{1}, <@{0}>."
            }.Random();
        }

        public string CreditUserBecause(string userUuid, string reason)
        {
            return new[]{
                "All hail, <@{0}>, {1}!",
                "cred till <@{0}>, {1}.",
                "<@{0}>, {1}.",
                "{1}, <@{0}>."
            }.Random();
        }

        public string DesribeItemToFewItems()
        {
            return new[]{
                "Du har inte så många grejer",
                "Tips: du kan kolla vad för saker du har med i-kommandot.",
                "Jag kan bara föreställa mig vad det skulle kunna vara.",
                "Ptja, det skulle kunna vara en fisk.",
                "Det är ett s.k. Out of range exception.",
                "Vad har vi här? Ett litet OutOfRangeException? Hur kommer det sig, tror du?"
            }.Random();
        }

        public string DescribeUser()
        {
            return new[]{
                "Jag vet inte var jag ska börja.",
                "Det är en användare.",
                "Det är en användare av bästa sort.",
                "Det är en användare i sina bästa år.",
                "Hen är väl kul.",
                "Jag har inget ont att säga om den personen.",
                "Hen är helt ok."
            }.Random();
        }

        public string QuestionAction()
        {
            return new[]{
                "Så, vad var det som fick dig att säga så?",
                "Varför säger du det till mig?",
                "Pratar du med mig?",
                "Hmm?",
                "?",
                "vad förväntar du dig av mig?",
                "jag vet inte vad du vill riktigt",
                "What's this, I don't even.. wha?"
            }.Random();
        }

        public string NoDataFound()
        {
            return new[]{
                "Hittade inget.",
                "Inget sånt i arkivet.",
                "Där var det tomt.",
                "Ingen utdelning.",
            }.Random();
        }

        public string QuestionSimilarUrl()
        {
            return new[]{
                "Har redan en väldigt lik url sparad, men kan inte avgöra om din är unik",
                "Snudd på samma url som redan finns reggad.",
                "Som två tvillingurlar separerade vid födseln.",
                "Urlsch!",
                "Urlaberla"
            }.Random();
        }
    }
}