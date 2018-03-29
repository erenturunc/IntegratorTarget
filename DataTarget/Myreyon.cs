using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IntegratorTarget.DataTarget
{
    class Myreyon
    {
        public static Dictionary<long, Product> Validation(Dictionary<long, Product> sourceProducts)
        {
            // Inventory Check
            sourceProducts = sourceProducts.Where(p => p.Value.Quantity > 5).ToDictionary(a => a.Key, b => b.Value);

            // Send only variants
            //sourceProducts = sourceProducts.Where(p => !string.IsNullOrWhiteSpace(p.Value.ProductGroupSKU)).ToDictionary(a => a.Key, b => b.Value);

            // Send higher than 80 AED
            sourceProducts = sourceProducts.Where(p => p.Value.SellingPrice > 80).ToDictionary(a => a.Key, b => b.Value);

            return sourceProducts;
        }

        internal static void Output(Dictionary<long, Product> sourceProductList, Dictionary<string, Product> parentProductList)
        {
            foreach (var item in sourceProductList)
            {
                if (!string.IsNullOrWhiteSpace(item.Value.ProductGroupSKU))
                    parentProductList[item.Value.ProductGroupSKU].SubProducts.Add(item.Value);
            }


            XmlSerializer xsSubmit = new XmlSerializer(typeof(List<Product>));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, parentProductList.Values.ToList());
                    xml = sww.ToString(); // Your XML
                }
            }

        }
    }
}
