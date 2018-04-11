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

        public static Dictionary<string, Product> GetProducts(string ApiKey)
        {
            //Action=GetProducts&Filter=all&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("GetProducts");
            string Filter = HttpUtility.UrlEncode("all");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+01:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode("atif.unaldi@texmart.com");
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

        public static void CreateProduct(Product product, string ApiKey)
        {
            string CreateProductResponse;
            //Action=ProductCreate&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("ProductCreate");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+01:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode("atif.unaldi@texmart.com");
            string Version = HttpUtility.UrlEncode("1.0");
            string Parameters = string.Format("Action={0}&Format={1}&Timestamp={2}&UserID={3}&Version={4}", Action, Format, TimeStamp, UserID, Version);

            string Signature = Util.GetHashSha256(Parameters, ApiKey);
            string RequestURL = Api_RequestUrl + Parameters + "&Signature=" + Signature;
            string RequestBody = Product2XML(product);

            if (!string.IsNullOrEmpty(RequestBody))
                CreateProductResponse = Util.SendHttpPostRequest(RequestURL, RequestBody);


        }

        private static string Product2XML(Product product)
        {
            //Required Parameters
            //Name, Brand, Color, Material, Weight, Description, Gender, Tax Class
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.Append("<Request>");

            sb.Append("<Product>");
            sb.Append("<Brand>" + product.Brand + "</Brand>");
            sb.Append("<Description>" + product.Description + "</Description>");
            sb.Append("<Name>" + product.ProductName + "</Name>");
            sb.Append("<Price>" + product.SellingPrice + "</Price>");
            sb.Append("<PrimaryCategory>" + product.Category + "</PrimaryCategory>");
            sb.Append("<SellerSku>" + product.SKU + "</SellerSku>");
            sb.Append("<TaxClass>default</TaxClass>");
            sb.Append("<Variation>" + product.Attribute02 + "</Variation>");
            sb.Append("<Quantity>" + product.Quantity + "</Quantity>");
            sb.Append("<Available>" + product.Quantity + "</Available>");
            sb.Append("<Status>" + "active" + "</Status>");
            sb.Append("<ProductId>" + product.ProductID + "</ProductId>");
            sb.Append("<TaxClass>default</TaxClass>");

            sb.Append("<MainImage>" + product.ImageURL01 + "</MainImage>");
            sb.Append("<Images>");

            if(!string.IsNullOrWhiteSpace(product.ImageURL01))
                sb.Append("<Image>" + product.ImageURL01 + "</Image>");
            if (!string.IsNullOrWhiteSpace(product.ImageURL02))
                sb.Append("<Image>" + product.ImageURL02 + "</Image>");
            if (!string.IsNullOrWhiteSpace(product.ImageURL03))
                sb.Append("<Image>" + product.ImageURL03 + "</Image>");
            if (!string.IsNullOrWhiteSpace(product.ImageURL04))
                sb.Append("<Image>" + product.ImageURL04 + "</Image>");
            if (!string.IsNullOrWhiteSpace(product.ImageURL05))
                sb.Append("<Image>" + product.ImageURL05 + "</Image>");

            sb.Append("</Images>");

            sb.Append("<ProductData>");
            sb.Append("<Color>" + product.Attribute04 + "</Color>");
            sb.Append("<MainMaterial>" + product.Attribute05 + "</MainMaterial>");
            sb.Append("<ProductWeight>0.5</ProductWeight>");
            sb.Append("</ProductData>");

            sb.Append("</Product>");


            sb.Append("</Request>");

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
