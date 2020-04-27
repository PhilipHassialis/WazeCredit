using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WazeCredit.Models;
using WazeCredit.Models.ViewModels;
using WazeCredit.Service;

namespace WazeCredit.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM();

            MarketForecaster marketForecaster = new MarketForecaster();
            MarketResult currentMarket = marketForecaster.GetMarketPrediction();

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
