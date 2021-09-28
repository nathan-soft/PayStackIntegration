using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayStackIntegration.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public InitializeTransactionDto TransactionDetail  { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public ActionResult OnGet()
        {
            return Page();
        }

        public ActionResult OnPostInitializePayment()
        {
            if (ModelState.IsValid)
            {
                var url = "https://api.paystack.co/transaction/initialize";
                
            }
          
            return Page();
        }
    }

    public class InitializeTransactionDto
    {
        [Required]
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string Callback_Url { get; set; }
    }
}
