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

        public string DidntMakeHighScoreFormat()
        {
            return new[]{
                "Du fick {0}p, vilket tyvärr inte tar dig in på highscore-listan :(",
                "{0}p! Tyvärr räcker det inte.",
                "{0}p, bättre lycka nästa gång!"
            }.Random();
        }

        public string DidntMakeHighScoreInCountFormat()
        {
            return new[]{
                "Du fick som mest {0}p över {1} spel, vilket tyvärr inte tar dig in på highscore-listan :(",
                "{0}p ({1} spel)! Tyvärr räcker det inte.",
                "{0}p över {1} spel, bättre lycka nästa gång!"
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

        public string NewHighscoreFormat()
        {
            return new[]{
                "Ny higscore i {0}! {1} tog plats {2} :D",
                "{1} knep just plats {2} på {0}-listan!",
                "Plats {2} på {0}-listan tillhör nu {1}"
            }.Random();
        }

        public string NoPoints()
        {
            return new[]{
                "Tyvärr, inga poäng denna gång :(",
                "Det här var inte din gång :/",
                "Prova igen!",
                "Rackarns! Inga poäng."
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