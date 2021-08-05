using System.Collections.Generic;
using HtmxTables.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxTables.Pages
{
    public class Chat : PageModel
    {
        public List<ChatController.Message> Messages => ChatController.Messages;
    }
}