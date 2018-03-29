using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntegratorTarget.DataMapping
{
    class AttributeMapping
    {
        internal static void MapAttribute04(Dictionary<long, Product> sourceProductList, Dictionary<string, string> mappingAttribute04)
        {
            foreach (var item in sourceProductList)
            {
                if (string.IsNullOrWhiteSpace(item.Value.Attribute04))
                    continue;
                if (mappingAttribute04.ContainsKey(item.Value.Attribute04))
                    item.Value.Attribute04 = mappingAttribute04[item.Value.Attribute04];
            }
        }

        internal static void MapAttribute02(Dictionary<long, Product> sourceProductList, Dictionary<string, string> mappingAttribute02)
        {
            foreach (var item in sourceProductList)
            {
                if (string.IsNullOrWhiteSpace(item.Value.Attribute02))
                    continue;
                if (mappingAttribute02.ContainsKey(item.Value.Attribute02))
                    item.Value.Attribute02 = mappingAttribute02[item.Value.Attribute02];
            }
        }

        internal static void MapAttribute06(Dictionary<long, Product> sourceProductList, Dictionary<string, string> mappingAttribute06)
        {
            string SearchType = "Direct";
            if (mappingAttribute06.Keys.Where(a => a.Contains("*")).Count() > 0)
                SearchType = "Wildcard";

            foreach (var item in sourceProductList)
            {
                if (string.IsNullOrWhiteSpace(item.Value.Attribute06))
                    continue;

                if (SearchType == "Wildcard")
                {
                    foreach (var map in mappingAttribute06)
                    {
                        string regexPattern = "^" + Regex.Escape(map.Key).Replace("\\?", ".").Replace("\\*", ".*") + "$";
                        if (Regex.IsMatch(map.Key, regexPattern))
                            item.Value.Attribute06 = map.Value;
                    }

                }
                else if (SearchType == "Direct")
                {
                    if (mappingAttribute06.ContainsKey(item.Value.Attribute02))
                        item.Value.Attribute06 = mappingAttribute06[item.Value.Attribute06];
                }
            }


        }

        internal static void MapPrice(Dictionary<long, Product> sourceProductList, string priceFormula)
        {
            priceFormula = priceFormula.Replace("[try]", Config.Rates.TRY.ToString());
            priceFormula = priceFormula.Replace("[usd]", Config.Rates.USD.ToString());
            priceFormula = priceFormula.Replace("[aed]", Config.Rates.AED.ToString());
            priceFormula = priceFormula.Replace("[irr]", Config.Rates.IRR.ToString());

            foreach (var item in sourceProductList)
            {
                string productPriceFormula = priceFormula.Replace("[sellingprice]", item.Value.SellingPrice.ToString());
                productPriceFormula = productPriceFormula.Replace("[price]", item.Value.Price.ToString());

                var result = double.Parse(new DataTable().Compute(productPriceFormula, null).ToString());
                result = Math.Round(result);
                if (result.ToString().Length > 2)
                    result = Math.Round(result / (double)10) * 10;

                item.Value.Price = result;
                item.Value.SellingPrice = result;
            }
        }
    }
}
