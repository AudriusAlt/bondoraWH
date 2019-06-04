using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BondoraWH.Models;
using BondoraWH.Models.OAuth;
using System.Diagnostics;

namespace BondoraWH
{
    public class ApiClient
    {
        public string BaseUri { get; private set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? TokenValidUntilUtc { get; set; }

        public ApiClient(string baseUri)
        {
            BaseUri = baseUri;
        }


        #region AccountBalance

        public async Task<ApiResultAccountBalance> GetAccountBalance()
        {
            using (var client = InitializeHttpClientWithAccessToken(AccessToken))
            {
                var uri = "api/v1/account/balance";

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {

                    return await response.Content.ReadAsAsync<ApiResultAccountBalance>();
                }
                else if ((int)response.StatusCode == 429 && response.Headers.Contains("Retry-After"))
                {
                    var retryAfter = response.Headers.GetValues("Retry-After").First();
                    int seconds;
                    if (int.TryParse(retryAfter, out seconds))
                    {
                        // Wait for N seconds efore trying again
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(seconds));
                        return await GetAccountBalance();
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            return null;
        }

        #endregion


        #region LoanPartDetails
        public async Task<GetLoansPartDetailResponse> GetLoanPartDetails(List<Guid> loanPartIds)
        {
            using (var client = InitializeHttpClientWithAccessToken(AccessToken))
            {
                var uri = "api/v1/loanpart/list";
                var request = new GetLoansPartDetailRequest();
                request.ItemIds = loanPartIds;
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, request);
                if (response.IsSuccessStatusCode)
                {
                    
                    return await response.Content.ReadAsAsync<GetLoansPartDetailResponse>();
                }
                else if ((int)response.StatusCode == 429 && response.Headers.Contains("Retry-After"))
                {
                    var retryAfter = response.Headers.GetValues("Retry-After").First();
                    int seconds;
                    if (int.TryParse(retryAfter, out seconds))
                    {
                        // Wait for N seconds efore trying again
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(seconds));
                        return await GetLoanPartDetails(loanPartIds);
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                   
                }
            }
            return null;
        }
        #endregion

        #region SecondaryMarket
        public async Task<ApiResultSecondMarket> GetSecondMarketItems(int pageNr = 1, int pageSize = 10,
            decimal? desiredDiscountRateMax = null, List<string> countries = null, string lastPaymentDateFrom = null, decimal? priceMin = null, decimal? priceMax = null,
            string nextPaymentDateFrom = null, string nextPaymentDateTo = null, int? statusCode = null)
        {
           
            using (var client = InitializeHttpClientWithAccessToken(AccessToken))
            {
                //string uri1 = "api/v1/secondarymarket?PageNr=1&PageSize=100";
                var uri = string.Format("api/v1/secondarymarket?PageNr={0}&PageSize={1}", pageNr, pageSize);

                if (desiredDiscountRateMax != null)
                    uri = string.Format("{0}&DesiredDiscountRateMax={1}", uri, desiredDiscountRateMax);
                if (lastPaymentDateFrom != null)
                    uri = string.Format("{0}&LastPaymentDateFrom={1}", uri, lastPaymentDateFrom);
                if (priceMin != null)
                    uri = string.Format("{0}&PriceMin={1}", uri, priceMin);
                if (priceMax != null)
                    uri = string.Format("{0}&PriceMax={1}", uri, priceMax);
                if (nextPaymentDateFrom != null)
                    uri = string.Format("{0}&NextPaymentDateFrom={1}", uri, nextPaymentDateFrom);
                if (nextPaymentDateTo != null)
                    uri = string.Format("{0}&NextPaymentDateTo={1}", uri, nextPaymentDateTo);
                if (statusCode != null)
                    uri = string.Format("{0}&LoanStatusCode={1}", uri, statusCode);

                if (countries != null && countries.Count > 0)
                {
                    var getParams = new NameValueCollection();
                    countries.ForEach(country => getParams.Add("Countries", country));

                    uri = string.Format("{0}&Countries={1}", uri, GetQueryString(getParams));
                }

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    //Logger.LogInfo(string.Format("Response = {0}\n\n\n", await response.Content.ReadAsStringAsync()));
                    return await response.Content.ReadAsAsync<ApiResultSecondMarket>();
                }
                else if ((int)response.StatusCode == 429 && response.Headers.Contains("Retry-After"))
                {

                    /*   //debug
                       var retryAfter = response.Headers.GetValues("Retry-After").First();
                       using (System.IO.StreamWriter file = new System.IO.StreamWriter("debug.txt", true))
                       {
                            file.WriteLine(response.StatusCode + "  -  " + retryAfter);
                       }

                       //enddebug


                          var retryAfter = response.Headers.GetValues("Retry-After").First();
                          int seconds;
                          if (int.TryParse(retryAfter, out seconds))
                          {
                              // Wait for N seconds efore trying again
                              System.Threading.Thread.Sleep(TimeSpan.FromSeconds(seconds));
                           return await GetSecondMarketItems(pageNr, pageSize, desiredDiscountRateMax, countries, lastPaymentDateFrom, priceMin, priceMax,
                             nextPaymentDateFrom, nextPaymentDateTo, statusCode);
                          }
                          else
                          {

                          }
                           */

                }
                else
                {
                    
                }
            }
            return null;
        }
        #endregion

        #region Investments
        public async Task<ApiResultInvestments> GetInvestments(int pageNr = 1, int pageSize = 10000, string nextPaymentDateFrom = null, string nextPaymentDateTo = null, string lastPaymentDateFrom = null, string lastPaymentDateTo = null)
        {
            using (var client = InitializeHttpClientWithAccessToken(AccessToken))
            {
                var uri = string.Format("api/v1/account/investments?PageNr={0}&PageSize={1}", pageNr, pageSize);


                if (nextPaymentDateFrom != null)
                    uri = string.Format("{0}&NextPaymentDateFrom={1}", uri, nextPaymentDateFrom);
                if (nextPaymentDateTo != null)
                    uri = string.Format("{0}&NextPaymentDateTo={1}", uri, nextPaymentDateTo);
                if (lastPaymentDateFrom != null)
                    uri = string.Format("{0}&LastPaymentDateFrom={1}", uri, lastPaymentDateFrom);
                if (lastPaymentDateTo != null)
                    uri = string.Format("{0}&LastPaymentDateTo={1}", uri, lastPaymentDateTo);

                HttpResponseMessage auctionListResponse = await client.GetAsync(uri);
                if (auctionListResponse.IsSuccessStatusCode)
                {
                    return await auctionListResponse.Content.ReadAsAsync<ApiResultInvestments>();
                }
                else
                {
                    var retryAfter = auctionListResponse.Headers.GetValues("Retry-After").First();
                    int seconds;
                    if (int.TryParse(retryAfter, out seconds))
                    {
                        // Wait for N seconds efore trying again
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(seconds));
                        return await GetInvestments(pageNr, pageSize, nextPaymentDateFrom, nextPaymentDateTo, lastPaymentDateFrom, lastPaymentDateTo);
                    }

                }
            }
            return null;
        }
        #endregion


        #region InvestmentsSell

        public async Task<bool> SellInvestments(List<SecondMarketSell> sellItems)
        {
            var request = new SellSecondMarketItemRequest
            {
                Items = sellItems,
                CancelItemOnPaymentReceived = false,
                CancelItemOnReschedule = false
            };

            using (var client = InitializeHttpClientWithAccessToken(AccessToken))
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("api/v1/secondarymarket/sell", request);
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //return false;
            }
        }

        #endregion


        #region SecondaryMarketBuy

        public async Task<int> BuySecondaryMarketItem(List<Guid> id)
        {
            var request = new BuySecondMarketItemRequest
            {
                ItemIds = id
            };

            using (var client = InitializeHttpClientWithAccessToken(AccessToken))
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("api/v1/secondarymarket/buy", request);
                return (int)response.StatusCode;
            
                //return false;
            }
        }

        #endregion


        #region OAuth Access Token
        public async Task<AccessTokenResult> GetAccessTokenByCode(string code, string clientId, string clientSecret, string redirectUri)
        {
            var request = new AccessTokenCodeRequest
            {
                code = code,
                client_id = clientId,
                client_secret = clientSecret,
                redirect_uri = redirectUri
            };

            using (var client = InitializeHttpClientWithBaseUri())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("oauth/access_token", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<AccessTokenResult>();
                }
                else
                {
                    
                }
                return null;
            }
        }

        public async Task<RefreshTokenResult> GetAccessTokenByRefreshToken(string refreshToken, string clientId, string clientSecret, string redirectUri)
        {
            var request = new AccessTokenRefreshTokenRequest
            {
                refresh_token = refreshToken,
                client_id = clientId,
                client_secret = clientSecret,
                redirect_uri = redirectUri
            };

            using (var client = InitializeHttpClientWithBaseUri())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("oauth/access_token", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<RefreshTokenResult>();
                }
                else
                {
                   
                }
                return null;
            }
        }

        public async Task<bool> RevokeAccessToken()
        {
            using (var client = InitializeHttpClientWithAccessToken(AccessToken))
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("oauth/access_token/revoke", new object());
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Accepted)
                {
                    var result = await response.Content.ReadAsAsync<ApiResult>();
                    return result != null && result.Success;
                }
                else
                {
                   
                }
                return false;
            }
        }
        #endregion

        #region Initialization

        private static HttpClient InitializeHttpClientWithBaseUri()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(ApiConfig.ApiBaseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static HttpClient InitializeHttpClientWithAccessToken(string accessToken)
        {
            var client = InitializeHttpClientWithBaseUri();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }

        #endregion

        #region Utils

        public static string GetQueryString(NameValueCollection source)
        {
            if (source.Count == 0)
                return string.Empty;

            return String.Join("&", source.AllKeys
                .SelectMany(key => source.GetValues(key)
                    .Select(value => String.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value))))
                .ToArray());
        }

        #endregion
    }
}
