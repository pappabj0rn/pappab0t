using System;
using System.Net.Http;
using System.Threading.Tasks;
using pappab0t.Abstractions;

namespace pappab0t
{
    /// <summary>
    /// HTTP Client adaptor wraps a <see cref="System.Net.Http.HttpClient"/> 
    /// that contains a reference to <see cref="ConfigurableMessageHandler"/>
    /// </summary>
    public sealed class HttpClientAdapter : IHttpClient
    {
        HttpClient _httpClient;

        public HttpClientAdapter(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        //...other code

        ///// <summary>
        /////  Send a GET request to the specified Uri as an asynchronous operation.
        ///// </summary>
        ///// <typeparam name="T">Response type</typeparam>
        ///// <param name="uri">The Uri the request is sent to</param>
        ///// <returns></returns>
        //public async System.Threading.Tasks.Task<T> GetAsync<T>(Uri uri) where T : class
        //{
        //    var result = default(T);
        //    //Try to get content as T
        //    try
        //    {
        //        //send request and get the response
        //        var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
        //        //if there is content in response to deserialize
        //        if (response.Content.Headers.ContentLength.GetValueOrDefault() > 0)
        //        {
        //            //get the content
        //            string responseBodyAsText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //            //desrialize it
        //            result = deserializeJsonToObject<T>(responseBodyAsText);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //    }
        //    return result;
        //}

        ////...other code
        public Task<T> DeleteAsync<T>(string uri) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> DeleteAsync<T>(Uri uri) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string uri) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(Uri uri) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> PostAsync<T>(string uri, object package)
        {
            throw new NotImplementedException();
        }

        public Task<T> PostAsync<T>(Uri uri, object package)
        {
            throw new NotImplementedException();
        }

        public Task<T> PutAsync<T>(string uri, object package)
        {
            throw new NotImplementedException();
        }

        public Task<T> PutAsync<T>(Uri uri, object package)
        {
            throw new NotImplementedException();
        }
    }
}
