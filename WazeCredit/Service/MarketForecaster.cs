using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Models;

namespace WazeCredit.Service
{
    public class MarketForecaster : IMarketForecaster
    {
        public MarketResult GetMarketPrediction()
        {
            // hard coding result from API / calculations etc
            return new MarketResult() { MarketCondition = MarketCondition.StableUp };
        }
    }


}
