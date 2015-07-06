using System.Collections.Generic;
using pappab0t.Extensions;

namespace pappab0t.Models
{
    public class Phrasebook
    {
        public string GetAffirmation()
        {
            return new[]{
                "ok",
                "råger",
                "roger",
                "jag hör dig.",
                "absolut"
            }.Random();
        }

        public string GetThankYou()
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

        public string GetExclamation()
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

        public string GetMutedExclamation()
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

        public string GetQuery(string input)
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
                        "läget?",

                    });
                    break;

                case "hi":
                    response.AddRange(new[]
                    {
                        "hi hi",
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

        public string GetScoreboardHype()
        {
            return new[]{
                "Ok, så här ser det ut ATM.",
                "Ställningen är som följer:",
                "Ok, en liten uppdatering om ställningarna då.",
            }.Random();
        }

        public string GetYoureWelcome()
        {
            return new[]{
                "np",
                "lugnt",
                "esch då",
                "^^",
                "Det var så lite så."
            }.Random();
        }

        public string GetOpenAppology()
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

        public string GetIDontKnowXxxNamedYyy()
        {
            return new[]{
                "jag känner inte till {0} som heter {1}",
                "jag har inte koll på {0} som heter {1}",
                "mina böcker nämner inte {0} vid namn {1}"
            }.Random();
        }

        public string GetIDidntUnderstand()
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

        public string GetInsufficientFundsFormat()
        {
            return new[]{
                "För lite pengar i plånkan, Du behöver {0}kr.",
                "Det kostart {0}kr att göra det där, vilket du inte har.",
                "Återkom när du har minst {0}kr.",
                "{0}kr först, sen kan vi prata.",
                "Inga rabatter, {0}kr kostar det."
            }.Random();
        }

        public string GetDidntMakeHighScoreFormat()
        {
            return new[]{
                "Du fick {0}p, vilket tyvärr inte tar dig in på highscore-listan :(",
                "{0}p! Tyvärr räcker det inte.",
                "{0}p, bättre lycka nästa gång!"
            }.Random();
        }

        public string GetPotPayoutFormat()
        {
            return new[]{
                "Ny highscore! {0}p tar dig till plats {1} och ger dig {2}kr :D",
                "Vinnare! {0}p => plats {1} => {2}kr.",
                "{2}kr utdelning! {0}p (pos. {1})"
            }.Random();
        }
    }
}