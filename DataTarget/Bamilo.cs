using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace IntegratorTarget.DataTarget
{
    class Bamilo
    {

        public static string Api_RequestUrl = "https://sellercenter-api.bamilo.com/?";
        //public static int BatchSize = 2;
        public static string ProductWarranty = @"<p><strong>آیا می توانم محصول را بازگشت دهم؟</strong></p> <p><span style='font-weight: 400;'>شما می توانید تا </span><strong>٣</strong><span style='font-weight: 400;'>۰ روز پس از دریافت سفارش خود، آن را مرجوع نمایید. دلایل مرجوعی کالا می تواند شامل یکی از موارد زیر باشد: </span></p> <ul> <li style='font-weight: 400;'><span style='font-weight: 400;'>کالای دریافت شده اشتباه بوده و یا اقلام سفارش به صورت کامل تحویل داده نشده‌ باشند.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>محصول دارای عیب و نقص باشد.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>سایز محصول مناسب خریدار نباشد.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>کیفیت محصول مشتری را راضی نکند.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>اطلاعات ارائه شده درباره محصول در وب‌سایت اشتباه درج شده باشد.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>مشتری نظر خود را درباره خرید محصول عوض کرده باشد.</span></li> </ul> <p><strong>شرایط پذیرش کالای برگشتی چیست؟</strong></p> <ul> <li style='font-weight: 400;'><span style='font-weight: 400;'>تگ‌های (برچسب‌ها) متصل به محصول کنده نشده باشند.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>محصول مطابق شرایط اولیه (هنگام تحویل کالا) بوده و مورد استفاده قرار نگرفته باشد.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>محصول آسیب ندیده باشد.</span></li> </ul> <p><strong>بازگشت مبلغ پرداخت شده به چه صورت است؟</strong></p> <ul> <li style='font-weight: 400;'><span style='font-weight: 400;'>بامیلو تمام تلاش خود را به کار می‌گیرد تا مبلغ مورد نظر را تا ۲۴ ساعت کاری پس از دریافت کالای برگشتی به حساب مشتری واریز نماید.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>در صورت خرید از طریق درگاه پرداخت الکترونیکی بانک سامان، مبلغ برگشتی به صورت خودکار به حساب مورد استفاده هنگام پرداخت آنلاین، واریز می‌شود.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>در صورت خرید از طریق درگاه پرداخت الکترونیکی بانک پارسیان یا پرداخت در محل توسط مشتری، مبلغ به محض دریافت اطلاعات حساب از مشتری، پرداخت خواهد شد.</span></li> </ul> <p><strong>مراحل بازگشت کالا چیست؟</strong></p> <ul> <li style='font-weight: 400;'><span style='font-weight: 400;'>ابتدا با امور مشتریان بامیلو با شماره </span><span style='font-weight: 400;'>۰۲۱-۴۱۶۸۷۰۰۰</span><span style='font-weight: 400;'>تماس گرفته و یا مشکل خود را از طریق آدرس پست الکترونیکی Support@bamilo.com با ما درمیان بگذارید.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>پس از دریافت درخواست بازگشت کالا کارشناسان بامیلو با شما تماس گرفته و برای دریافت کالا از درب منزل (سفارشات شهر تهران) هماهنگی‌های لازم را انجام داده و یا راه‌های ارسال کالا به بامیلو توسط مشتری (سفارشات دیگر شهرها) را به شما معرفی خواهند کرد.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>کارشناسان بامیلو پس از دریافت کالا شروع به بررسی دلایل بازگشت کالا (بیان شده توسط مشتری) خواهند کرد.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>اگر دلایل بازگشت کالا مورد قبول باشد اقدامات مورد نیاز برای بازگشت مبلغ پرداختی، طی ۲۴ ساعت کاری انجام خواهد شد.</span></li> <li style='font-weight: 400;'><span style='font-weight: 400;'>اما اگر دلایل بازگشت کالا مورد قبول واقع نشده و یا با محصول بازگشتی مغایرت داشته باشد محصول به مشتری پس داده می‌شود.</span></li> </ul>";
        public static Dictionary<string, Dictionary<int, int>> ImageSizeDic = new Dictionary<string, Dictionary<int, int>>();


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

        internal static string Output(Dictionary<long, Product> sourceProductList)
        {
            //Image Validation - START
            LoadImageSizeDic();
            sourceProductList = sourceProductList.Where(a => !string.IsNullOrEmpty(a.Value.ImageURL01) && ImageSizeValidation(a.Value.ImageURL01)).ToDictionary(a => a.Key, b => b.Value);
            UpdateImageSizeDic();
            //Image Validation - END

            //IndexValidation - START
            var Indexes = Sql.AppDataProvider.Get_Index(Config.MemberID, Config.TargetID);
            sourceProductList = Util.FilterProductsByIndexes(Indexes, sourceProductList, true);
            //IndexValidation - END

            //Dictionary<string, string> ParentSKUs = sourceProductList.Where(p => !string.IsNullOrWhiteSpace(p.Value.ProductGroupSKU)).Select(p => p.Value.ProductGroupSKU).Distinct().ToDictionary(a => a, b => b);
            //Dictionary<string, Product> parentProductList = sourceProductList.Where(p => ParentSKUs.ContainsKey(p.Value.SKU)).GroupBy(p => p.Value.SKU).ToDictionary(a => a.Key, b => b.First().Value);
            //sourceProductList = sourceProductList.Where(a => string.IsNullOrEmpty(a.Value.ProductGroupSKU) || parentProductList.ContainsKey(a.Value.ProductGroupSKU)).ToDictionary(a => a.Key, b => b.Value);

            //foreach (var item in sourceProductList)
            //{
            //    if (!string.IsNullOrWhiteSpace(item.Value.ProductGroupSKU))
            //        parentProductList[item.Value.ProductGroupSKU].SubProducts.Add(new SubProduct(item.Value));
            //}
            //parentProductList = parentProductList.Where(x => x.Value.SubProducts.Count > 0).ToDictionary(a => a.Key, b => b.Value);

            //Creating bamilo specific product names
            //foreach (var item in parentProductList)
            //{
            //    item.Value.ProductName = item.Value.Attribute06 + " " + item.Value.Attribute11 + " " + item.Value.Brand + " " + item.Value.Attribute11;
            //    foreach (var sProduct in item.Value.SubProducts)
            //        sProduct.ProductName = item.Value.Attribute06 + " " + item.Value.Attribute11 + " " + item.Value.Brand + " " + item.Value.Attribute11;
            //}

            //foreach (var item in parentProductList)
            //{
            //    int category;
            //    bool IsInt = int.TryParse(item.Value.Category, out category);
            //    if (!IsInt)
            //        item.Value.Category = string.Empty;
            //}
            //parentProductList = parentProductList.Where(a => !string.IsNullOrEmpty(a.Value.Category)).ToDictionary(a => a.Key, b => b.Value);

            var MainProducts = sourceProductList.Where(p => string.IsNullOrEmpty(p.Value.ProductGroupSKU)).ToDictionary(a => a.Value.SKU, b => b.Value);
            var VariantProducts = sourceProductList.Where(p => !string.IsNullOrEmpty(p.Value.ProductGroupSKU)).ToDictionary(a => a.Value.SKU, b => b.Value);

            foreach (var item in VariantProducts)
            {
                if (MainProducts.ContainsKey(item.Value.ProductGroupSKU))
                {
                    var ParentProduct = MainProducts[item.Value.ProductGroupSKU];
                    item.Value.Category = ParentProduct.Category;
                    item.Value.Attribute13 = ParentProduct.Attribute13;
                    item.Value.ProductName = ParentProduct.Attribute06 + " " + ParentProduct.Attribute11 + " " + ParentProduct.Brand + " " + ParentProduct.Attribute11;
                    ParentProduct.ProductName = item.Value.ProductName;
                    ParentProduct.Attribute02 = "---";
                }
            }

            //Category Filter
            int dummy;
            MainProducts = MainProducts.Where(p => int.TryParse(p.Value.Category, out dummy)).ToDictionary(a => a.Key, b => b.Value);
            VariantProducts = VariantProducts.Where(p => int.TryParse(p.Value.Category, out dummy)).ToDictionary(a => a.Key, b => b.Value);

            Dictionary<string, string> MainProductSKUs = VariantProducts.Select(p => p.Value.ProductGroupSKU).Distinct().ToDictionary(a => a, b => b);
            MainProducts = MainProducts.Where(p => MainProductSKUs.ContainsKey(p.Value.SKU)).ToDictionary(a => a.Key, b => b.Value);
            VariantProducts = VariantProducts.Where(p => MainProducts.ContainsKey(p.Value.ProductGroupSKU)).ToDictionary(a => a.Key, b => b.Value);


            Dictionary<string, Product> BamiloProducts = GetProducts(Config.TargetApiKey, Config.TargetApiUsername);
            BamiloProducts = BamiloProducts.Where(x => x.Key.StartsWith(Config.ProviderPrefix)).ToDictionary(a => a.Key, b => b.Value);


            var DeleteProductList = BamiloProducts.Where(a => !MainProducts.ContainsKey(a.Key) && !VariantProducts.ContainsKey(a.Key)).ToDictionary(a => a.Key, b => b.Value);
            var CreateProductList_Parent = MainProducts.Where(a => !BamiloProducts.ContainsKey(a.Key)).ToDictionary(a => a.Key, b => b.Value);
            var CreateProductList_Variant = VariantProducts.Where(a => !BamiloProducts.ContainsKey(a.Key)).ToDictionary(a => a.Key, b => b.Value);
            var UpdateProductList_Parent = MainProducts.Where(a => BamiloProducts.ContainsKey(a.Key)).ToDictionary(a => a.Key, b => b.Value);
            var UpdateProductList_Variant = VariantProducts.Where(a => BamiloProducts.ContainsKey(a.Key)).ToDictionary(a => a.Key, b => b.Value);

            if (DeleteProductList.Count > 0)
            {
                RemoveProduct(DeleteProductList, Config.TargetApiKey, Config.TargetApiUsername);
                System.Threading.Thread.Sleep(1 * 60 * 1000);
            }

            if (UpdateProductList_Parent.Count > 0)
            {
                var result = Util.SplitDictionary(UpdateProductList_Parent, 3);
                foreach (var item in result)
                {
                    UpdateProduct(item, Config.TargetApiKey, Config.TargetApiUsername);
                    System.Threading.Thread.Sleep(1 * 60 * 1000); // sleep 10 mins
                }
            }

            if (UpdateProductList_Variant.Count > 0)
            {
                var result = Util.SplitDictionary(UpdateProductList_Variant, 3);
                foreach (var item in result)
                {
                    UpdateProduct(item, Config.TargetApiKey, Config.TargetApiUsername);
                    System.Threading.Thread.Sleep(1 * 60 * 1000); // sleep 10 mins
                }
            }


            if (CreateProductList_Parent.Count > 0)
            {
                var result = Util.SplitDictionary(CreateProductList_Parent, 3);
                foreach (var item in result)
                {
                    CreateProduct(item, Config.TargetApiKey, Config.TargetApiUsername);
                    System.Threading.Thread.Sleep(2 * 60 * 1000); // sleep 30 mins
                    CreateImages(item, Config.TargetApiKey, Config.TargetApiUsername);
                }
            }

            if (CreateProductList_Variant.Count > 0)
            {
                var result = Util.SplitDictionary(CreateProductList_Variant, 5);
                foreach (var item in result)
                {
                    CreateProduct(item, Config.TargetApiKey, Config.TargetApiUsername);
                    System.Threading.Thread.Sleep(2 * 60 * 1000); // sleep 30 mins
                    CreateImages(item, Config.TargetApiKey, Config.TargetApiUsername);
                }
            }

            return string.Empty;
        }

        public static Dictionary<string, Product> GetProducts(string ApiKey, string ApiUsername)
        {
            //Action=GetProducts&Filter=all&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("GetProducts");
            string Filter = HttpUtility.UrlEncode("all");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00").ToUpperInvariant();
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

        public static void CreateProduct(Dictionary<string, Product> products, string ApiKey, string ApiUsername)
        {
            string CreateProductResponse;
            //Action=ProductCreate&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("ProductCreate");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode(ApiUsername);
            string Version = HttpUtility.UrlEncode("1.0");
            string Parameters = string.Format("Action={0}&Format={1}&Timestamp={2}&UserID={3}&Version={4}", Action, Format, TimeStamp, UserID, Version);

            string Signature = Util.GetHashSha256(Parameters, ApiKey);
            string RequestURL = Api_RequestUrl + Parameters + "&Signature=" + Signature;
            string RequestBody = Products2XML(products);

            if (!string.IsNullOrEmpty(RequestBody))
                CreateProductResponse = Util.SendHttpPostRequest(RequestURL, RequestBody);

        }

        public static void UpdateProduct(Dictionary<string, Product> products, string ApiKey, string ApiUsername)
        {
            string UpdateProductResponse;
            //Action=ProductUpdate&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("ProductUpdate");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode(ApiUsername);
            string Version = HttpUtility.UrlEncode("1.0");
            string Parameters = string.Format("Action={0}&Format={1}&Timestamp={2}&UserID={3}&Version={4}", Action, Format, TimeStamp, UserID, Version);

            string Signature = Util.GetHashSha256(Parameters, ApiKey);
            string RequestURL = Api_RequestUrl + Parameters + "&Signature=" + Signature;
            string RequestBody = Products2XML_Update(products);

            if (!string.IsNullOrEmpty(RequestBody))
                UpdateProductResponse = Util.SendHttpPostRequest(RequestURL, RequestBody);

        }

        public static void RemoveProduct(Dictionary<string, Product> products, string ApiKey, string ApiUsername)
        {
            string RemoveProductResponse;
            //Action=ProductRemove&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("ProductRemove");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode(ApiUsername);
            string Version = HttpUtility.UrlEncode("1.0");
            string Parameters = string.Format("Action={0}&Format={1}&Timestamp={2}&UserID={3}&Version={4}", Action, Format, TimeStamp, UserID, Version);

            string Signature = Util.GetHashSha256(Parameters, ApiKey);
            string RequestURL = Api_RequestUrl + Parameters + "&Signature=" + Signature;
            string RequestBody = Products2XML_Delete(products);

            if (!string.IsNullOrEmpty(RequestBody))
                RemoveProductResponse = Util.SendHttpPostRequest(RequestURL, RequestBody);

        }

        public static void CreateImages(Dictionary<string, Product> products, string ApiKey, string ApiUsername)
        {
            string CreateImageResponse;
            //Action=ProductCreate&Format=XML&Timestamp=##Timestamp##&UserID=atif.unaldi%40texmart.com&Version=1.0
            string Action = HttpUtility.UrlEncode("Image");
            string Format = HttpUtility.UrlEncode("XML");
            string TimeStamp = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00").ToUpperInvariant();
            string UserID = HttpUtility.UrlEncode(ApiUsername);
            string Version = HttpUtility.UrlEncode("1.0");
            string Parameters = string.Format("Action={0}&Format={1}&Timestamp={2}&UserID={3}&Version={4}", Action, Format, TimeStamp, UserID, Version);

            string Signature = Util.GetHashSha256(Parameters, ApiKey);
            string RequestURL = Api_RequestUrl + Parameters + "&Signature=" + Signature;
            string RequestBody = Images2XML(products);

            if (!string.IsNullOrEmpty(RequestBody))
                CreateImageResponse = Util.SendHttpPostRequest(RequestURL, RequestBody);

        }

        private static string Products2XML(Dictionary<string, Product> products)
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
                sb.Append("\t<Brand>" + product.Brand + "</Brand>\n");
                sb.Append("\t<Description>" + product.ProductName + "</Description>\n");
                sb.Append("\t<Name>" + product.ProductName + "</Name>\n");
                sb.Append("\t<Price>" + product.SellingPrice + "</Price>\n");
                sb.Append("\t<PrimaryCategory>" + product.Category + "</PrimaryCategory>\n");
                sb.Append("\t<SellerSku>" + product.SKU + "</SellerSku>\n");
                sb.Append("\t<TaxClass>default</TaxClass>\n");
                sb.Append("\t<Variation>" + product.Attribute02 + "</Variation>\n");
                sb.Append("\t<Quantity>" + product.Quantity + "</Quantity>\n");
                //sb.Append("\t<Available>" + product.Quantity + "</Available>\n");
                sb.Append("\t<Status>" + "active" + "</Status>\n");
                sb.Append("\t<ProductId>" + product.ProductID + "</ProductId>\n");

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
                sb.Append("\t\t<ColorFamily>" + product.Attribute12 + "</ColorFamily>\n");
                sb.Append("\t\t<MainMaterial>" + product.Attribute13 + "</MainMaterial>\n");
                sb.Append("\t\t<ClothMaterial>" + product.Attribute13 + "</ClothMaterial>\n");
                sb.Append("\t\t<ProductWeight>0.5</ProductWeight>\n");
                sb.Append("\t\t<Gender>" + product.Attribute06 + "</Gender>\n");
                sb.Append("\t\t<ProductWarranty><![CDATA[" + ProductWarranty + "]]></ProductWarranty>\n");
                sb.Append("\t</ProductData>\n");

                sb.Append("\t<SalePrice>" + Math.Round((product.SellingPrice / 1.42857) / (double)1000) * 1000 + "</SalePrice>\n");
                sb.Append("\t<SaleStartDate>" + "2018-01-01 00:00:00" + "</SaleStartDate>\n");
                sb.Append("\t<SaleEndDate>" + "2018-12-01 00:00:00" + "</SaleEndDate>\n");

                sb.Append("</Product>\n");


            }

            sb.Append("</Request>\n");

            return sb.ToString();
        }

        private static string Products2XML_Delete(Dictionary<string, Product> products)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.Append("<Request>\n");

            foreach (var item in products)
            {
                var product = item.Value;

                sb.Append("<Product>\n");
                sb.Append("\t<SellerSku>" + product.SKU + "</SellerSku>\n");
                sb.Append("</Product>\n");
            }

            sb.Append("</Request>\n");

            return sb.ToString();
        }

        private static string Products2XML_Update(Dictionary<string, Product> products)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.Append("<Request>\n");

            foreach (var item in products)
            {
                var product = item.Value;

                sb.Append("<Product>\n");
                sb.Append("\t<SellerSku>" + product.SKU + "</SellerSku>\n");
                sb.Append("\t<Quantity>" + product.Quantity + "</Quantity>\n");
                sb.Append("\t<Price>" + product.SellingPrice + "</Price>\n");
                sb.Append("\t<SalePrice>" + Math.Round((product.SellingPrice / 1.42857) / (double)1000) * 1000 + "</SalePrice>\n");
                sb.Append("\t<SaleStartDate>" + "2018-01-01 00:00:00" + "</SaleStartDate>\n");
                sb.Append("\t<SaleEndDate>" + "2018-12-01 00:00:00" + "</SaleEndDate>\n");
                sb.Append("</Product>\n");
            }

            sb.Append("</Request>\n");

            return sb.ToString();
        }

        private static string Images2XML(Dictionary<string, Product> products)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.Append("<Request>\n");

            foreach (var item in products)
            {
                var product = item.Value;

                sb.Append("<ProductImage>\n");
                sb.Append("\t<SellerSku>" + product.SKU + "</SellerSku>\n");

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
                sb.Append("</ProductImage>\n");
            }

            sb.Append("</Request>\n");

            return sb.ToString();
        }

        private static string Products2CSV(Dictionary<string, Product> products)
        {
            //Required Parameters
            //Name, Brand, Color, Material, Weight, Description, Gender, Tax Class
            StringBuilder sb = new StringBuilder();

            sb.Append("Name;");
            sb.Append("AlternateName;");
            sb.Append("OriginalName;");
            sb.Append("Model;");
            sb.Append("PrimaryCategory;");
            sb.Append("AdditionalCategory1;");
            sb.Append("AdditionalCategory2;");
            sb.Append("AdditionalCategory3;");
            sb.Append("BrowseNodes;");
            sb.Append("Brand;");
            sb.Append("ProductId;");
            sb.Append("ParentSku;");
            sb.Append("Price Rials;");
            sb.Append("Quantity;");
            sb.Append("Variation;");
            sb.Append("SellerSku;");
            sb.Append("CCode;");
            sb.Append("ColorFamily;");
            sb.Append("Color;");
            sb.Append("ProductLine;");
            sb.Append("MainMaterial;");
            sb.Append("ProductWeight;");
            sb.Append("ProductWarranty;");
            sb.Append("PrBodymaterial;");
            sb.Append("Description;");
            sb.Append("ShortDescription;");
            sb.Append("CareLabel;");
            sb.Append("PackageContent;");
            sb.Append("ClothMaterial;");
            sb.Append("Pattern;");
            sb.Append("SleevesType;");
            sb.Append("FasteningType;");
            sb.Append("CollarType;");
            sb.Append("Gender;");
            sb.Append("HeelSize;");
            sb.Append("SleeveLength;");
            sb.Append("SunglassesType;");
            sb.Append("TrousersFit;");
            sb.Append("DisplayIfOutOfStock;");
            sb.Append("ComplementaryProducts;");
            sb.Append("ContentSpecialistName;");
            sb.Append("TaxClass;");
            sb.Append("CstmPrice;");
            sb.Append("HsSku;");
            sb.Append("PrDiameter;");
            sb.Append("PrHeight;");
            sb.Append("PrLength;");
            sb.Append("PrWidth;");
            sb.Append("MainImage;");
            sb.Append("Image2;");
            sb.Append("Image3;");
            sb.Append("Image4;");
            sb.Append("Image5;");
            sb.Append("Image6;");
            sb.Append("Image7;");
            sb.Append("Image8\n");

            foreach (var item in products)
            {
                var ParentProduct = item.Value;


                //if (!string.IsNullOrEmpty(product.ProductGroupSKU))
                //    sb.Append("\t<ParentSku>" + product.ProductGroupSKU + "</ParentSku>\n");
                //else
                //    continue;
                foreach (var product in ParentProduct.SubProducts)
                {
                    sb.Append(product.ProductName + ";");
                    sb.Append(product.ProductName + ";");
                    sb.Append(product.ProductName + ";");
                    sb.Append(";");
                    sb.Append(ParentProduct.Category + ";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(product.Brand + ";");
                    sb.Append(product.SKU + ";");
                    sb.Append(product.ProductGroupSKU + ";");
                    sb.Append(product.SellingPrice + ";");
                    sb.Append(product.Quantity + ";");
                    sb.Append(product.Attribute02 + ";");
                    sb.Append(product.SKU + ";");
                    sb.Append(";");
                    sb.Append(product.Attribute04 + ";");
                    sb.Append(product.Attribute04 + ";");
                    sb.Append(";");
                    sb.Append(product.Attribute05 + ";");
                    sb.Append("0.5;");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(product.ProductName + ";");
                    sb.Append(product.ProductName + ";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(product.Attribute06 + ";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append("default;");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(";");
                    sb.Append(product.ImageURL01 + ";");
                    sb.Append(product.ImageURL02 + ";");
                    sb.Append(product.ImageURL03 + ";");
                    sb.Append(product.ImageURL04 + ";");
                    sb.Append(product.ImageURL05 + ";");
                    sb.Append(product.ImageURL06 + ";");
                    sb.Append(product.ImageURL07 + ";");
                    sb.Append(product.ImageURL08 + ";");
                    sb.Append(product.ImageURL09 + "\n");

                }

            }

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

        private static bool ImageSizeValidation(string ImgLink)
        {
            int w;
            int h;

            if (ImageSizeDic.ContainsKey(ImgLink))
            {
                w = ImageSizeDic[ImgLink].First().Key;
                h = ImageSizeDic[ImgLink].First().Value;
            }
            else
            {
                GetSizeOfImage(ImgLink, out w, out h);
                Dictionary<int, int> sizes = new Dictionary<int, int>();
                sizes.Add(w, h);
                ImageSizeDic.Add(ImgLink, sizes);
            }

            if (w < 500 || w > 2000 || h < 500 || h > 2000)
                return false;
            else
                return true;


        }

        public static void LoadImageSizeDic()
        {
            string ImageSizeDB = string.Format("{0}/{1}", Config.DataOutputFolder, "BamiloImagesDB.csv");
            if (File.Exists(ImageSizeDB))
            {
                StreamReader reader = new StreamReader(ImageSizeDB);
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parameters = line.Split(';');
                    if (!ImageSizeDic.ContainsKey(parameters[0]))
                    {
                        Dictionary<int, int> sizes = new Dictionary<int, int>();
                        sizes.Add(int.Parse(parameters[1]), int.Parse(parameters[2]));
                        ImageSizeDic.Add(parameters[0], sizes);
                    }
                }
                reader.Close();
            }
        }

        public static void UpdateImageSizeDic()
        {
            string ImageSizeDB = string.Format("{0}/{1}", Config.DataOutputFolder, "BamiloImagesDB.csv");

            StreamWriter writer = new StreamWriter(ImageSizeDB);

            foreach (var item in ImageSizeDic)
            {
                writer.WriteLine(item.Key + ";" + item.Value.First().Key + ";" + item.Value.First().Value);
            }

            writer.Close();

        }

        public static void GetSizeOfImage(string link, out int width, out int height)
        {
            MemoryStream imgStream;
            Image img = null;
            byte[] imageData;
            width = 0;
            height = 0;


            int retries = 3;
            while (true)
            {
                try
                {
                    imageData = new WebClient().DownloadData(link);
                    imgStream = new MemoryStream(imageData);
                    img = Image.FromStream(imgStream);
                    break; // success!
                }
                catch (Exception ex)
                {
                    if (--retries == 0)
                    { width = 0; height = 0; break; }
                    else Thread.Sleep(2000);
                }
            }

            if (img != null)
            {
                width = img.Width;
                height = img.Height;
            }

        }

    }
}
