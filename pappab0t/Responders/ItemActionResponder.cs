using System;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class ItemActionResponder : ResponderBase//, IExposedCapability
    {
        public override bool CanRespond(ResponseContext context)
        {
            return false;
            throw new NotImplementedException();
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage();
            throw new NotImplementedException();
        }

        public ExposedInformation Info { get; }
        /*
         public ExposedInformation Info => new ExposedInformation
        {
            Usage = "pbot [verb] [item_id] (on) (item_target_id)",
            Explatation = "Visar din nuvarande inventarieförteckning."
        };

        Öppna X
        Öppna X med Y
        Stäng X
        Lås X
        Lås X med Y
        Titta på X
        Använd X
        Använd X på Y
        Drick X
        Dela X RATIO
        Ge X till USER
        */
    }
}
