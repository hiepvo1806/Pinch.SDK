using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pinch.SDK.WebSample.Helpers;
using Pinch.SDK.WebSample.Models;

namespace Pinch.SDK.WebSample.Controllers
{
    public class WebhooksController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public WebhooksController(IOptions<PinchSettings> settings, ApplicationDbContext context) : base(settings)
        {
            _context = context;
        }

        public async Task<IActionResult> ReceiveWebhook()
        {
            using (var sr = new StreamReader(Request.Body))
            {                
                var body = await sr.ReadToEndAsync();
                var headers = Request.Headers.ToDictionary(x => x.Key, x => x.Value);

                var isValid = GetApi().Webhook.VerifyWebhook("whsec_MucZPgWo3vkNorvRzNTaQQsHkOMyqiYy", body, headers);

                var delivery = new WebhookDelivery()
                {
                    Json = body
                };
                _context.WebhookDeliveries.Add(delivery);
                await _context.SaveChangesAsync();

                // A response message is optional, but will be saved.
                return Ok($"Webhook recieved. IsValid: {isValid}");
            }
        }
    }
}