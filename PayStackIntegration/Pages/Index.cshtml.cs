using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PayStackIntegration.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public InitializeTransactionDto TransactionDetail { get; set; } = new InitializeTransactionDto();

        public string Message { get; set; } = "";

        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public ActionResult OnGet()
        {
            return Page();
        }

        public async Task<ActionResult> OnPostInitializePaymentAsync()
        {
            if (ModelState.IsValid)
            {
                var url = "https://api.paystack.co/transaction/initialize";
                var testSecretKey = "sk_test_669b5c218e32337b8da931a60315be5114dd113b";
                //TransactionDetail.Callback_Url = $"{HttpContext.Request.Host.Value}/PaymentComplete";

                var client = _clientFactory.CreateClient();

                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testSecretKey);
                    //set callback url
                    TransactionDetail.callback_url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/PaymentComplete";

                    //serialize data
                    var stringContent = new StringContent(JsonConvert.SerializeObject(TransactionDetail), Encoding.UTF8, "application/json");
                    //send request.
                    var httpResponse = await client.PostAsync(url, stringContent);

                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    //deserialize to json
                    var jsonResponse = JsonConvert.DeserializeObject<PaystackResponseDto<PaystackTransactionInitializationResponseDto>>(jsonString);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        return Redirect(jsonResponse.data.authorization_url);
                    }
                    else
                    {
                        Message = jsonResponse.message;
                    }
                }
                catch (Exception ex)
                {
                    Message = $"An error occurred: {ex.Message}";
                }
            }
          
            return Page();
        }
    }

    public class InitializeTransactionDto
    {
        [Required, EmailAddress]
        public string email { get; set; } = "nathan@idevworks.com";
        public int amount { get; set; } = 1000;
        public string callback_url { get; set; }
    }

    public class PaystackResponseDto<T> where T : class
    {
        public bool status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }

    public class PaystackTransactionInitializationResponseDto
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
}
