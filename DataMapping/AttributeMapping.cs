using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratorTarget.DataMapping
{
    class AttributeMapping
    {
        internal static void MapColor(Dictionary<long, Product> sourceProductList, Dictionary<string, string> mappingColor)
        {
            foreach (var item in sourceProductList)
            {
                if (string.IsNullOrWhiteSpace(item.Value.Attribute04))
                    continue;
                if (mappingColor.ContainsKey(item.Value.Attribute04))
                    item.Value.Attribute04 = mappingColor[item.Value.Attribute04];
            }
        }

        internal static void MapSize(Dictionary<long, Product> sourceProductList, Dictionary<string, string> mappingSize)
        {
            foreach (var item in sourceProductList)
            {
                if (string.IsNullOrWhiteSpace(item.Value.Attribute02))
                    continue;
                if (mappingSize.ContainsKey(item.Value.Attribute02))
                    item.Value.Attribute02 = mappingSize[item.Value.Attribute02];
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
