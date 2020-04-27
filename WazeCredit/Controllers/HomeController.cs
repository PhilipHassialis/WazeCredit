using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WazeCredit.Models;
using WazeCredit.Models.ViewModels;
using WazeCredit.Service;
using WazeCredit.Utility.AppSettingsClasses;

namespace WazeCredit.Controllers
{
    public class HomeController : Controller
    {
        public HomeVM homeVM { get; set; }
        private readonly IMarketForecaster _marketForecaster;
        private readonly StripeSettings _stripeOptions;
        private readonly SendGridSettings _sendGridOptions;
        private readonly TwilioSettings _twilioOptions;
        private readonly WazeForecastSettings _wazeOptions;

        public HomeController(IMarketForecaster marketForecaster,
            IOptions<StripeSettings> stripeOptions, 
            IOptions<SendGridSettings> sendGridOptions, 
            IOptions<TwilioSettings> twilioOptions, 
            IOptions<WazeForecastSettings> wazeOptions)
        {
            homeVM = new HomeVM();
            _marketForecaster = marketForecaster;
            _stripeOptions = stripeOptions.Value;
            _sendGridOptions = sendGridOptions.Value;
            _twilioOptions = twilioOptions.Value;
            _wazeOptions = wazeOptions.Value;

        }

        public IActionResult Index()
        {
            

            MarketResult currentMarket = _marketForecaster.GetMarketPrediction();

            switch (currentMarket.MarketCondition)
            {
                case MarketCondition.StableDown:
                    homeVM.MarketForecast = "Market stable going down";
                    break;
                case MarketCondition.StableUp:
                    homeVM.MarketForecast = "Market stable going up";
                    break;
                case MarketCondition.Volatile:
                    homeVM.MarketForecast = "Market is volatile";
                    break;
                default:
                    homeVM.MarketForecast = "Situation Unknown with market";
                    break;
            }
            return View(homeVM);
        }

        public IActionResult AllConfigSettings()
        {
            List<string> messages = new List<string>();
            messages.Add($"Waze options - Forecast Tracker:" + _wazeOptions.ForecastTrackerEnabled);
            messages.Add($"Stripe options - Publishable Key:" + _stripeOptions.PublishableKey);
            messages.Add($"Stripe options - Secret Key:" + _stripeOptions.SecretKey);
            messages.Add($"SendGrid options - SendGrid Key: " + _sendGridOptions.SendGridKey);
            messages.Add($"Twilio options - Account SID: " + _twilioOptions.AccountSid);
            messages.Add($"Twilio options - Auth Token: " + _twilioOptions.AuthToken);
            messages.Add($"Twilio options - Phone Number: " + _twilioOptions.PhoneNumber);

            return View(messages);


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
