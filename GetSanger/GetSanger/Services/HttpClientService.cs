using GetSanger.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GetSanger.Services
{
    static class HttpClientService
    {
        private static readonly HttpClient s_HttpClient;

        static HttpClientService()
        {
            s_HttpClient = new HttpClient();
        }

        public static async Task<HttpResponseMessage> SendHttpRequest(string i_Uri, string i_Json, HttpMethod i_Method, string i_IdToken = null)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage(i_Method, i_Uri);
                httpRequest.Content = new StringContent(i_Json);
                if (i_IdToken != null)
                {
                    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", i_IdToken);
                }

                HttpResponseMessage response = await s_HttpClient.SendAsync(httpRequest);
                return response;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }

        }
    }
}
