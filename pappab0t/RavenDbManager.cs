using System.Linq;
using Raven.Client;

namespace pappab0t
{
    public abstract class RavenDbManager<T>
    {
        public bool WaitForNonStaleResults { get; set; }

        protected void WaitForNonStaleResultsIfEnabled(IDocumentSession session)
        {
            if (WaitForNonStaleResults)
            {
                session
                    .Query<T>()
                    .Customize(x => x.WaitForNonStaleResults())
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    .ToList();
            }
        }
    }
}