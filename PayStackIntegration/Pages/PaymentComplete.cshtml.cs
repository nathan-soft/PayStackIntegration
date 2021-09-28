using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace PayStackIntegration.Pages
{
    public class PaymentCompleteModel : PageModel
    {
        public PaystackTransactionVerificationResponseDto ResponseData { get; set; }
        public string ErrorMessage { get; set; }


        private readonly IHttpClientFactory _clientFactory;

        public PaymentCompleteModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<PageResult> OnGetAsync(string reference)
        {
            if (!string.IsNullOrWhiteSpace(reference))
            {
                var url = $"https://api.paystack.co/transaction/verify/{reference}";
                var testSecretKey = "sk_test_669b5c218e32337b8da931a60315be5114dd113b";

                var client = _clientFactory.CreateClient();

                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testSecretKey);

                    //send request.
                    var httpResponse = await client.GetAsync(url);

                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    //deserialize to json
                    var jsonResponse = JsonConvert.DeserializeObject<PaystackResponseDto<PaystackTransactionVerificationResponseDto>>(jsonString);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        ResponseData = jsonResponse.data;
                    }
                    else
                    {
                        ErrorMessage = jsonResponse.message;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }

            return Page();
        }
    }

    public class PaystackTransactionVerificationResponseDto
    {
        public decimal amount { get; set; }
        public string reference { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public decimal requested_amount { get; set; }
        public Customer customer { get; set; }
}

    public class Customer
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
    }
}
