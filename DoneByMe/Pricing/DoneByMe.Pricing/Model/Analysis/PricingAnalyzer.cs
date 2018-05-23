using System;

// Domain Service
namespace DoneByMe.Pricing.Model.Analysis
{
    public class PricingAnalyzer
    {
		public void AnalyzePricing(string pricedItemId, long price, string[] keywords)
		{
			// TODO: process in this Domain Service and keep PricingHistory
			foreach(var keyword in keywords) {
				Console.WriteLine(keyword);
			}

			Console.WriteLine("PricingAnalyzer#AnalyzePricing(" + pricedItemId + ", " + price + ")");
		}
	}
}
