using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace IntegratorTarget.DataTarget
{
    class Bamilo
    {

        public static string Api_RequestUrl = "https://sellercenter-api.bamilo.com/?";
        public static int BatchSize = 2;

        public static Dictionary<long, Product> Validation(Dictionary<long, Product> sourceProducts)
        {
            // Inventory Check
            sourceProducts = sourceProducts.Where(p => p.Value.Quantity > 2).ToDictionary(a => a.Key, b => b.Value);

            // Send only variants
            //sourceProducts = sourceProducts.Where(p => !string.IsNullOrWhiteSpace(p.Value.ProductGroupSKU)).ToDictionary(a => a.Key, b => b.Value);

            // Send higher than 1000 IRR
            sourceProducts = sourceProducts.Where(p => p.Value.SellingPrice > 1000).ToDictionary(a => a.Key, b => b.Value);

            return sourceProducts;
        }


        internal static string Output(Dictionary<long, Product> sourceProductList, Dictionary<string, Product> parentProductList)
        {

            Dictionary<string, Product> BamiloProducst = GetProducts(Config.TargetApiKey, Config.TargetApiUsername);

            //TEMP
            sourceProductList = sourceProductList.Where(x => x.Value.SKU == "br_124984" || x.Value.ProductGroupSKU == "br_124984").ToDictionary(a => a.Key, b => b.Value);

            string Result = Products2XML(sourceProductList);

            return Result;

        }

        public static Dictionary<string, Product> GetProducts(string ApiKey, string ApiUsername)
        {
            //Action=GetProducts&Filter=all&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("GetProducts");
            string Filter = HttpUtility.UrlEncode("all");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+01:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode(ApiUsername);
            string Version = HttpUtility.UrlEncode("1.0");
            string Parameters = string.Format("Action={0}&Filter={1}&Format={2}&Timestamp={3}&UserID={4}&Version={5}", Action, Filter, Format, TimeStamp, UserID, Version);

            string Signature = Util.GetHashSha256(Parameters, ApiKey);
            string RequestURL = Api_RequestUrl + Parameters + "&Signature=" + Signature;
            Dictionary<string, Product> Result = new Dictionary<string, Product>();
            string ProductsString = Util.SendHttpGetRequest(RequestURL);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ProductsString);

            XmlNodeList productNodes = doc.SelectNodes("//Product");

            foreach (XmlNode pNode in productNodes)
            {
                Product p = new Product();
                XmlDocument productXml = new XmlDocument();
                productXml.LoadXml("<Product>" + pNode.InnerXml + "</Product>");
                p.SKU = productXml.SelectSingleNode("//SellerSku").InnerText.Trim();

                if (!Result.ContainsKey(p.SKU))
                    Result.Add(p.SKU, p);

            }

            return Result;
        }

        public static void CreateProduct(Product product, string ApiKey, string ApiUsername)
        {
            string CreateProductResponse;
            //Action=ProductCreate&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("ProductCreate");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+01:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode(ApiUsername);
            string Version = HttpUtility.UrlEncode("1.0");
            string Parameters = string.Format("Action={0}&Format={1}&Timestamp={2}&UserID={3}&Version={4}", Action, Format, TimeStamp, UserID, Version);

            string Signature = Util.GetHashSha256(Parameters, ApiKey);
            string RequestURL = Api_RequestUrl + Parameters + "&Signature=" + Signature;
            string RequestBody = Product2XML(product);

            if (!string.IsNullOrEmpty(RequestBody))
                CreateProductResponse = Util.SendHttpPostRequest(RequestURL, RequestBody);


        }

        private static string Products2XML(Dictionary<long, Product> products)
        {
            //Required Parameters
            //Name, Brand, Color, Material, Weight, Description, Gender, Tax Class
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.Append("<Request>\n");

            foreach (var item in products)
            {
                var product = item.Value;
                sb.Append("<Product>\n");

                if (!string.IsNullOrEmpty(product.ProductGroupSKU))
                    sb.Append("\t<ParentSku>" + product.ProductGroupSKU + "</ParentSku>\n");
                else
                    continue;
                //TEMP

                sb.Append("\t<Brand>" + product.Brand + "</Brand>\n");
                sb.Append("\t<Description>" + product.ProductName + "</Description>\n");
                sb.Append("\t<Name>" + product.ProductName + "</Name>\n");
                sb.Append("\t<Price>" + product.SellingPrice + "</Price>\n");
                sb.Append("\t<PrimaryCategory>" + product.Category + "</PrimaryCategory>\n");
                sb.Append("\t<SellerSku>" + product.SKU + "</SellerSku>\n");
                sb.Append("\t<TaxClass>default</TaxClass>\n");
                sb.Append("\t<Variation>" + product.Attribute02 + "</Variation>\n");
                sb.Append("\t<Quantity>" + product.Quantity + "</Quantity>\n");
                sb.Append("\t<Available>" + product.Quantity + "</Available>\n");
                sb.Append("\t<Status>" + "active" + "</Status>\n");
                sb.Append("\t<ProductId>" + product.ProductID + "</ProductId>\n");
                sb.Append("\t<TaxClass>default</TaxClass>\n");

                sb.Append("\t<MainImage>" + product.ImageURL01 + "</MainImage>\n");
                sb.Append("\t<Images>\n");

                if (!string.IsNullOrWhiteSpace(product.ImageURL01))
                    sb.Append("\t\t<Image>" + product.ImageURL01 + "</Image>\n");
                if (!string.IsNullOrWhiteSpace(product.ImageURL02))
                    sb.Append("\t\t<Image>" + product.ImageURL02 + "</Image>\n");
                if (!string.IsNullOrWhiteSpace(product.ImageURL03))
                    sb.Append("\t\t<Image>" + product.ImageURL03 + "</Image>\n");
                if (!string.IsNullOrWhiteSpace(product.ImageURL04))
                    sb.Append("\t\t<Image>" + product.ImageURL04 + "</Image>\n");
                if (!string.IsNullOrWhiteSpace(product.ImageURL05))
                    sb.Append("\t\t<Image>" + product.ImageURL05 + "</Image>\n");

                sb.Append("\t</Images>\n");

                sb.Append("\t<ProductData>\n");
                sb.Append("\t\t<Color>" + product.Attribute04 + "</Color>\n");
                sb.Append("\t\t<MainMaterial>" + product.Attribute05 + "</MainMaterial>\n");
                sb.Append("\t\t<ProductWeight>0.5</ProductWeight>\n");
                sb.Append("\t</ProductData>\n");

                sb.Append("</Product>\n");
            }

            sb.Append("</Request>\n");

            return sb.ToString();
        }

        public static List<Category> ParseCategoryTree(string XmlCategoryTreePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlCategoryTreePath);
            XmlNode CategoriesNode = doc.SelectSingleNode("//SuccessResponse/Body/Categories");

            return LoadCategoryTree(CategoriesNode.ChildNodes, -1);
        }

        private static List<Category> LoadCategoryTree(XmlNodeList Nodes, int ParentID)
        {
            List<Category> Result = new List<Category>();

            foreach (XmlNode Node in Nodes)
            {
                Category cat = new Category();
                cat.Name = Node.SelectSingleNode("Name").InnerText;
                cat.CategoryID = int.Parse(Node.SelectSingleNode("CategoryId").InnerText);
                cat.ParentID = ParentID;
                cat.Children = LoadCategoryTree(Node.SelectSingleNode("Children").ChildNodes, cat.CategoryID);
                Result.Add(cat);
            }

            return Result;

        }

    }
}
