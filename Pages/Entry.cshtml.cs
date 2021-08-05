using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxTables.Pages
{
    public class Entry : PageModel
    {
        [BindProperty, Required]
        public string Name { get; set; }

        [BindProperty, Required]
        public int Age { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            // see the loading spinner (remove for production)
            await Task.Delay(TimeSpan.FromSeconds(1));
            // handle Htmx request
            return Request.IsHtmx()
                ? Partial("_Form", this)
                : Page();
        }
    }
}