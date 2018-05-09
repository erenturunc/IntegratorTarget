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
    class Priceminister
    {
        public static Dictionary<long, Product> Validation(Dictionary<long, Product> sourceProducts)
        {
            // Inventory Check
            sourceProducts = sourceProducts.Where(p => p.Value.Quantity > 5).ToDictionary(a => a.Key, b => b.Value);

            // Send only variants
            //sourceProducts = sourceProducts.Where(p => !string.IsNullOrWhiteSpace(p.Value.ProductGroupSKU)).ToDictionary(a => a.Key, b => b.Value);

            // Send higher than 80 AED
            sourceProducts = sourceProducts.Where(p => p.Value.SellingPrice > 20).ToDictionary(a => a.Key, b => b.Value);

            return sourceProducts;
        }

        internal static string NewProductsOutput(Dictionary<long, Product> sourceProductList, Dictionary<string, Product> parentProductList)
        {
            //Code-barres a 13 characteres  * / 13 char barcode *	Prix de vente * / Selling Price *	Qualite * / Condition *	Quantite * / Quantity *	Commentaire de l'annonce / Advert comment	Collection ? / collectible item ?	Reference unique de l'annonce * / Unique Advert Refence (SKU) *	Commentaire prive de l'annonce / Private Advert Comment	Code operation promo / Promotion code	Images	Expedition, Retrait / Shipping, Pick Up	Telephone / Phone number	Code postale / Zip Code	Pays / Country	Poids en grammes / Weight in grammes	 RSL ( Expedie par Priceminister - Rakuten) / RSL (Shipped by Priceminister - Rakuten)	FRANCE METROPOLE - MAINLAND	DROM_COM - OVERSEAS	EUROPE	MONDE / WORLD	Description Personnalisee de l'Annonce / Custom Advert Description (http code, css styles)
            foreach (var item in sourceProductList)
            {
                if (!string.IsNullOrWhiteSpace(item.Value.ProductGroupSKU))
                    parentProductList[item.Value.ProductGroupSKU].SubProducts.Add(new SubProduct(item.Value));
            }

            parentProductList = parentProductList.Where(x => x.Value.SubProducts.Count > 0).ToDictionary(a => a.Key, b => b.Value);


            XmlSerializer xsSubmit = new XmlSerializer(typeof(List<Product>));
            var xml = "";

            using (var sww = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "     "; // note: default is two spaces
                settings.NewLineOnAttributes = false;
                settings.OmitXmlDeclaration = true;

                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    xsSubmit.Serialize(writer, parentProductList.Values.ToList());
                    xml = sww.ToString(); // Your XML
                    xml = "<?xml version=\"1.0\" encoding=\"iso-8859-9\" ?>\r\n" + xml;
                }
            }

            return xml;

        }
        
    }
}
