using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Models;

namespace WazeCredit.Service
{
    public class MarketForecaster
    {
        public MarketResult GetMarketPrediction()
        {
            return new MarketResult() { MarketCondition = MarketCondition.StableUp };
        }
    }

    public class MarketResult
    {
        public MarketCondition MarketCondition { get; set; }
    }
}
