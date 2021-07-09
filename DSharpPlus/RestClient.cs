// This file is part of the DSharpPlus project.
//
// Copyright (c) 2015 Mike Santiago
// Copyright (c) 2016-2021 DSharpPlus Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DSharpPlus
{
    internal class RestClient : IRequestClient
    {
        private readonly HttpClient _http;

        public RestClient(DiscordConfiguration configuration)
        {
            _http = this.ConfigureClient(configuration);
        }

        public async Task<object> SendAsync(IRequest request)
        {
            var restRequest = (RestRequest)request;
            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(restRequest.Method),
                RequestUri = restRequest.Route,
                Content = request.Data != null ? new StreamContent(request.Data) : null
            };

            var response = await _http.SendAsync(httpRequest);

            //handle response

            //if successful deserialize

            return response; //return as stream
        }

        private HttpClient ConfigureClient(DiscordConfiguration configuration)
        {
            var httphandler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                UseProxy = configuration.Proxy != null,
                Proxy = configuration.Proxy
            };

            var http = new HttpClient(httphandler)
            {
                Timeout = configuration.HttpTimeout
            };

            http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", this.ConfigureToken(configuration));

            return http;
        }

        private string ConfigureToken(DiscordConfiguration configuration)
        {
            return configuration.TokenType switch
            {
                TokenType.Bearer => $"Bearer {configuration.Token}",
                TokenType.Bot => $"Bot {configuration.Token}",
                _ => throw new ArgumentException("Invalid token type specified.", nameof(configuration.Token)),
            };
        }
    }
}
