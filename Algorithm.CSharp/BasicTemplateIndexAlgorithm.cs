/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using System.Collections.Generic;
using System.Linq;
using QuantConnect.Data;
using QuantConnect.Indicators;
using QuantConnect.Interfaces;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// This example demonstrates how to add index asset types.
    /// </summary>
    /// <meta name="tag" content="using data" />
    /// <meta name="tag" content="benchmarks" />
    /// <meta name="tag" content="indexes" />
    public class BasicTemplateIndexAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private Symbol _spx;
        private Symbol _spy;
        private ExponentialMovingAverage _emaSlow;
        private ExponentialMovingAverage _emaFast;

        /// <summary>
        /// Initialize your algorithm and add desired assets.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2021, 2, 25);
            SetEndDate(2021, 3, 3);
            SetCash(1000000);

            // Use indicator for signal
            _spx = AddIndex("SPX", Resolution.Minute).Symbol;
            _emaSlow = EMA(_spx, 80);
            _emaFast = EMA(_spx, 200);

            // Trade on SPY
            _spy = AddEquity("SPY", Resolution.Minute).Symbol;
        }

        /// <summary>
        /// Index EMA Cross trading underlying.
        /// </summary>
        public override void OnData(Slice slice)
        {
            
            if (!slice.Bars.ContainsKey(_spx) || !slice.Bars.ContainsKey(_spy))
            {
                return;
            }

            Debug($"\r\n{slice.Bars[_spy].Time}: {slice.Bars[_spy]}");
            Debug($"{slice.Bars[_spx].Time}: {slice.Bars[_spx]}");
            Debug($"===============================================");

            // Warm up indicators
            if (!_emaSlow.IsReady)
            {
                return;
            }

            if (_emaFast > _emaSlow)
            {
                SetHoldings(_spy, 1);
            }
            else
            {
                Liquidate();
            }
        }

        public override void OnEndOfAlgorithm()
        {
            if (Portfolio[_spx].TotalSaleVolume > 0)
            {
                throw new Exception("Index is not tradable.");
            }
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp, Language.Python };

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "8220"},
            {"Average Win", "0.00%"},
            {"Average Loss", "0.00%"},
            {"Compounding Annual Return", "-100.000%"},
            {"Drawdown", "13.500%"},
            {"Expectancy", "-0.818"},
            {"Net Profit", "-13.517%"},
            {"Sharpe Ratio", "-2.678"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "89%"},
            {"Win Rate", "11%"},
            {"Profit-Loss Ratio", "0.69"},
            {"Alpha", "4.398"},
            {"Beta", "-0.989"},
            {"Annual Standard Deviation", "0.373"},
            {"Annual Variance", "0.139"},
            {"Information Ratio", "-12.816"},
            {"Tracking Error", "0.504"},
            {"Treynor Ratio", "1.011"},
            {"Total Fees", "$15207.00"},
            {"Estimated Strategy Capacity", "$8800000.00"},
            {"Fitness Score", "0.033"},
            {"Kelly Criterion Estimate", "0"},
            {"Kelly Criterion Probability Value", "0"},
            {"Sortino Ratio", "-8.62"},
            {"Return Over Maximum Drawdown", "-7.81"},
            {"Portfolio Turnover", "302.321"},
            {"Total Insights Generated", "0"},
            {"Total Insights Closed", "0"},
            {"Total Insights Analysis Completed", "0"},
            {"Long Insight Count", "0"},
            {"Short Insight Count", "0"},
            {"Long/Short Ratio", "100%"},
            {"Estimated Monthly Alpha Value", "$0"},
            {"Total Accumulated Estimated Alpha Value", "$0"},
            {"Mean Population Estimated Insight Value", "$0"},
            {"Mean Population Direction", "0%"},
            {"Mean Population Magnitude", "0%"},
            {"Rolling Averaged Population Direction", "0%"},
            {"Rolling Averaged Population Magnitude", "0%"},
            {"OrderListHash", "35b3f4b7a225468d42ca085386a2383e"}
        };
    }
}