using System.Collections.Generic;
using pappab0t.Extensions;

namespace pappab0t.Models
{
    public class Phrasebook
    {
        public string GetAffirmation()
        {
            string[] response =
            {
                "ok",
                "råger",
                "roger",
                "jag hör dig.",
                "absolut"
            };

            return response.Random();
        }

        public string GetExclamation()
        {
            string[] response =
            {
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
            };

            return response.Random();
        }

        public string GetMutedExclamation()
        {
            string[] response =
            {
                "Kul.",
                "Jora, så atte...",
                "Inte illa.",
                "Heja.",
                ":thumbsup:",
                "grattis.",
                ":clap:",
                ":smiley:"
            };

            return response.Random();
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
            string[] response =
            {
                "Ok, så här ser det ut ATM.",
                "Ställningen är som följer:",
                "Ok, en liten uppdatering om ställningarna då.",
            };

            return response.Random();
        }

        public string GetYoureWelcome()
        {
            string[] response =
            {
                "np",
                "lugnt",
                "esch då",
                "^^",
                "Det var så lite så."
            };

            return response.Random();
        }

        public string GetOpenAppology()
        {
            string[] response =
            {
                "sry, men",
                "ursäkta mig, men",
                "Du får ursäkta mig, men",
                "du får ursäkta mig, men",
                "hmm,",
                "Hmm,"
            };

            return response.Random();
        }

        public string GetIDontKnowXxxNamedYyy()
        {
            string[] response =
            {
                "jag känner inte till {0} som heter {1}",
                "jag har inte koll på {0} som heter {1}",
            };

            return response.Random();
        }
    }
}