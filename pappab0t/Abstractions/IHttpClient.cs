using System;
using System.Threading.Tasks;

namespace pappab0t.Abstractions
{
    public interface IHttpClient
    {
        Task<T> DeleteAsync<T>(string uri) where T : class;
        Task<T> DeleteAsync<T>(Uri uri) where T : class;
        Task<T> GetAsync<T>(string uri) where T : class;
        Task<T> GetAsync<T>(Uri uri) where T : class;
        Task<T> PostAsync<T>(string uri, object package);
        Task<T> PostAsync<T>(Uri uri, object package);
        Task<T> PutAsync<T>(string uri, object package);
        Task<T> PutAsync<T>(Uri uri, object package);
    }
}
