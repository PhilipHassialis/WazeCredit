using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WazeCredit.Data;
using WazeCredit.Data.Repository.IRepository;
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

        private readonly ICreditValidator _creditValidator;

        //private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger _logger;

        private readonly StripeSettings _stripeOptions;
        private readonly SendGridSettings _sendGridOptions;
        private readonly TwilioSettings _twilioOptions;
        private readonly WazeForecastSettings _wazeOptions;

        [BindProperty]
        public CreditApplication CreditModel { get; set; }

        public HomeController(IMarketForecaster marketForecaster, 
            IOptions<WazeForecastSettings> wazeOptions, 
            ICreditValidator creditValidator,
            //ApplicationDbContext db,
            IUnitOfWork unitOfWork,
            ILogger<HomeController> logger)
        {
            homeVM = new HomeVM(); // for Index IActionResult
            _logger = logger;
            _wazeOptions = wazeOptions.Value;
            // for AllConfigSettings IActionResult
            _marketForecaster = marketForecaster;
            _creditValidator = creditValidator;
            //_db = db;
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index()
        {

            _logger.LogInformation("HomeController Index started");
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
            _logger.LogInformation("HomeController Index ended");
            return View(homeVM);
        }

        public IActionResult AllConfigSettings(
            [FromServices] IOptions<StripeSettings> stripeOptions,
            [FromServices] IOptions<SendGridSettings> sendGridOptions,
            [FromServices] IOptions<TwilioSettings> twilioOptions

            )
        {
            List<string> messages = new List<string>();
            messages.Add($"Waze options - Forecast Tracker:" + _wazeOptions.ForecastTrackerEnabled);
            messages.Add($"Stripe options - Publishable Key:" + stripeOptions.Value.PublishableKey);
            messages.Add($"Stripe options - Secret Key:" + stripeOptions.Value.SecretKey);
            messages.Add($"SendGrid options - SendGrid Key: " + sendGridOptions.Value.SendGridKey);
            messages.Add($"Twilio options - Account SID: " + twilioOptions.Value.AccountSid);
            messages.Add($"Twilio options - Auth Token: " + twilioOptions.Value.AuthToken);
            messages.Add($"Twilio options - Phone Number: " + twilioOptions.Value.PhoneNumber);

            return View(messages);


        }

        public IActionResult CreditApplication()
        {
            CreditModel = new CreditApplication();
            return View(CreditModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("CreditApplication")]
        public async Task<IActionResult> CreditApplicationPOST(
            [FromServices] Func<CreditApprovedEnum, ICreditApproved> _creditService
            )
        {
            if (ModelState.IsValid)
            {
                var (validationPassed, errorMessages) = await _creditValidator.PassAllValidations(CreditModel);

                CreditResult creditResult = new CreditResult()
                {
                    ErrorList = errorMessages,
                    CreditID = 0,
                    Success = validationPassed

                };
                if (validationPassed)
                {

                    CreditModel.CreditApproved = _creditService(
                        CreditModel.Salary > 50000 ? CreditApprovedEnum.High : CreditApprovedEnum.Low
                        ).GetCreditApproved(CreditModel);

                    //_db.CreditApplicationModel.Add(CreditModel);
                    //_db.SaveChanges();

                    _unitOfWork.CreditApplication.Add(CreditModel);
                    _unitOfWork.Save();

                    creditResult.CreditID = CreditModel.Id;
                    creditResult.CreditApproved = CreditModel.CreditApproved;

                    return RedirectToAction(nameof(CreditResult), creditResult);
                }
                else
                {
                    // not adding to database
                    return RedirectToAction(nameof(CreditResult), creditResult);
                }
            }
            return View(CreditModel);
        }

        public IActionResult CreditResult(CreditResult creditResult)
        {
            return View(creditResult);
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
