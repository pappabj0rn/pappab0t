using System;
using System.Collections.Generic;

namespace pappab0t.Models
{
    public class Phrasebook
    {
        public string GetAffirmation()
        {
            string[] affirmations =
            {
                "ok",
                "råger",
                "roger",
                "jag hör dig.",
                "absolut"
            };

            return affirmations[new Random().Next(affirmations.Length)];
        }

        public string GetExclamation()
        {
            string[] exclamations =
            {
                "Fuck YEAH!",
                "In Yo Face",
                "Whooo!",
                "whoppi do!",
                "wiiii",
                "nej va kul!",
                "OMG"
            };

            return exclamations[new Random().Next(exclamations.Length)];
        }

        public string GetQuery(string input)
        {
            var text = (input??"").Replace(" pb0t", "").Replace("pappab0t", "").Trim();

            var svar = new List<string>();

            switch (text)
            {
                case "hej":
                case "tja":
                case "tjena":
                    svar.AddRange(new[]
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
                    svar.AddRange(new[]
                    {
                        "yo!",
                        "yo man",
                        "läget?",

                    });
                    break;

                case "hi":
                    svar.AddRange(new[]
                    {
                        "hi hi",
                        "?",
                        "nåt som är kul?",
                        "har jag missat något?"
                    });
                    break;
                case "hello":
                    svar.AddRange(new[]
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
                    svar.AddRange(new[]
                    {
                        "morrn",
                        "nirrb",
                        "mrn"
                    });
                    break;
                default:
                    svar.AddRange(new[]
                    {
                        "Hej du. Vad är upp?",
                        "Hej cepe, behöver du hjälp?",
                        "Tjena tjockis! Hur kan jag stå till tjänst?",
                        "*[yawns]*. Uj uj, ledsn fö dä. Va kan ja gö fö dä?",
                        "pb0t tll edr tjnst!"
                    });
                    break;
            }

            return svar[new Random().Next(svar.Count)];
        }

        public string GetScoreboardHype()
        {
            string[] hypes =
            {
                "Ok, så här ser det ut ATM.",
                "Ställningen är som följer:",
                "Ok, en liten uppdatering om ställningarna då.",
            };

            return hypes[new Random().Next(hypes.Length)];
        }

        public string GetYoureWelcome()
        {
            string[] youreWelcomes =
            {
                "np",
                "lugnt",
                "esch då",
                "^^",
                "Det var så lite så."
            };

            return youreWelcomes[new Random().Next(youreWelcomes.Length)];
        }
    }
}