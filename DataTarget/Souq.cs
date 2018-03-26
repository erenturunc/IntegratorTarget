using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratorTarget.DataTarget
{
    class Souq
    {
        public static Dictionary<long, Product> Validation(Dictionary<long, Product> sourceProducts)
        {
            // Inventory Check
            sourceProducts = sourceProducts.Where(p => p.Value.Quantity > 5).ToDictionary(a => a.Key, b => b.Value);

            // Send only variants
            sourceProducts = sourceProducts.Where(p => !string.IsNullOrWhiteSpace(p.Value.ProductGroupSKU)).ToDictionary(a => a.Key, b => b.Value);

            // Send higher than 80 AED
            sourceProducts = sourceProducts.Where(p => p.Value.SellingPrice > 80).ToDictionary(a => a.Key, b => b.Value);

            return sourceProducts;
        }

        internal static void Output(Dictionary<long, Product> sourceProductList, Dictionary<string,Product> parentProductList)
        {
            StreamWriter sw = new StreamWriter("Souq.csv");

            foreach (var item in sourceProductList)
            {
                string line = "";
                line += Config.MemberCode + ";";
                line += "" + ";"; // GTIN
                line += Config.ProviderPrefix + item.Value.SKU + ";"; //SKU
                line += "" + ";"; // SOUQ EAN
                line += item.Value.Brand + ";"; // Brand Name
                line += item.Value.ProductName + ";"; // Item Name
                line += parentProductList[item.Value.ProductGroupSKU].ProductID + ";"; // Item Connection
                line += "Underwear" + ";"; // Attribute01
                line += item.Value.Attribute02 + ";"; // Attribute02
                line += "EU" + ";"; // Attribute03
                line += item.Value.Attribute04 + ";"; // Attribute04
                line += item.Value.Attribute05 + ";"; // Attribute05
                line += item.Value.Attribute06 + ";"; // Attribute06
                line += item.Value.Attribute07 + ";"; // Attribute07
                line += item.Value.Attribute08 + ";"; // Attribute08
                line += "Wash in warm water" + ";"; // Attribute09
                line += "Turkey" + ";"; // Attribute10
                line += "" + ";"; // Free text desc
                line += "" + ";"; // Offer Note
                line += "new" + ";"; // Offer Condition
                line += item.Value.Quantity + ";"; // Quantity available to sell
                line += item.Value.Price + ";"; // Retail Price
                line += item.Value.SellingPrice + ";"; // Selling Price
                line += "N" + ";"; // Free shipping
                line += "QPS" + ";"; // Shipping Providers
                line += "" + ";"; // Delivery time
                line += "" + ";"; // EPayment
                line += item.Value.ImageURL01 + ";"; // Images
                line += item.Value.ImageURL02 + ";"; // Images
                line += item.Value.ImageURL03 + ";"; // Images
                line += item.Value.ImageURL04 + ";"; // Images
                line += item.Value.ImageURL05 + ";"; // Images
                line += item.Value.ImageURL06 + ";"; // Images
                sw.WriteLine(line);
            }

            sw.Close();
        }
    }
}
