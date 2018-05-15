using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratorTarget
{
    public static class Config
    {
        public static string CurrencyRatesJsonFilePath;
        public static string ProviderProductsXmlUri;
        public static int MemberID;
        public static int ProviderID;
        public static int TargetID;
        public static string MemberCode;
        public static string PriceFormula;
        public static string ProviderPrefix;
        public static string DataOutputFolder;
        public static string TargetApiKey;
        public static string TargetApiUsername;
        public static CurrencyRates Rates;
        public static Encoding DataSourceEncoding = Encoding.UTF8;

        public static void ReadConfig(string Member, string Provider, string TargetCode)
        {
            Dictionary<string, string> ConfigKeyValues = Sql.AppDataProvider.Get_Configuration(Member, Provider, TargetCode);

            MemberCode = Member;

            if (ConfigKeyValues.ContainsKey("currencyratesjsonfilepath"))
                CurrencyRatesJsonFilePath = ConfigKeyValues["currencyratesjsonfilepath"];
            if (ConfigKeyValues.ContainsKey("providerproductsxmluri"))
                ProviderProductsXmlUri = ConfigKeyValues["providerproductsxmluri"];
            if (ConfigKeyValues.ContainsKey("memberid"))
                MemberID = int.Parse(ConfigKeyValues["memberid"]);
            if (ConfigKeyValues.ContainsKey("providerid"))
                ProviderID = int.Parse(ConfigKeyValues["providerid"]);
            if (ConfigKeyValues.ContainsKey("targetid"))
                TargetID = int.Parse(ConfigKeyValues["targetid"]);
            if (ConfigKeyValues.ContainsKey("priceformula"))
                PriceFormula = ConfigKeyValues["priceformula"];
            if (ConfigKeyValues.ContainsKey("providerprefix"))
                ProviderPrefix = ConfigKeyValues["providerprefix"];
            if (ConfigKeyValues.ContainsKey("dataoutputfolder"))
                DataOutputFolder = ConfigKeyValues["dataoutputfolder"];
            if (ConfigKeyValues.ContainsKey("targetapikey"))
                TargetApiKey = ConfigKeyValues["targetapikey"];
            if (ConfigKeyValues.ContainsKey("targetapiusername"))
                TargetApiUsername = ConfigKeyValues["targetapiusername"];
            if (ConfigKeyValues.ContainsKey("datasourceencoding"))
                DataSourceEncoding = Encoding.GetEncoding(ConfigKeyValues["datasourceencoding"]);

            Config.Rates = Util.ParseOpenExchangeRateCurrencies(CurrencyRatesJsonFilePath);
        }
    }

    public enum MappingType
    {
        ProductName,
        Category,
        Attribute01,
        Attribute02,
        Attribute03,
        Attribute04,
        Attribute05,
        Attribute06,
        Attribute07,
        Attribute08,
        Attribute09,
        Attribute10,
        Attribute11,
        Attribute12,
        Attribute13,
        Attribute14,
        Attribute15,
        Attribute16,
        Attribute17,
        Attribute18,
        Attribute19,
        Attribute20
    }

    [Serializable]
    public class Product
    {
        public long ProductID;
        public int ProviderID;
        public int MemberID;
        public string Category;
        public string GlobalBarcode;
        public string SKU;
        public string Brand;
        public string ProductName;
        public string ProductGroupSKU;
        public string Attribute01;
        public string Attribute02;
        public string Attribute03;
        public string Attribute04;
        public string Attribute05;
        public string Attribute06;
        public string Attribute07;
        public string Attribute08;
        public string Attribute09;
        public string Attribute10;
        public string Attribute11;
        public string Attribute12;
        public string Attribute13;
        public string Attribute14;
        public string Attribute15;
        public string Attribute16;
        public string Attribute17;
        public string Attribute18;
        public string Attribute19;
        public string Attribute20;
        public string Description;
        public string OfferNote;
        public string Condition;
        public int Quantity;
        public double Price;
        public double SellingPrice;
        public string FreeShipping;
        public string ShippingProviders;
        public string DeliveryTime;
        public string ImageURL01;
        public string ImageURL02;
        public string ImageURL03;
        public string ImageURL04;
        public string ImageURL05;
        public string ImageURL06;
        public string ImageURL07;
        public string ImageURL08;
        public string ImageURL09;
        public string ImageURL10;

        //public bool IsActive = true;
        public List<SubProduct> SubProducts = new List<SubProduct>();
    }

    [Serializable]
    public class SubProduct : Product
    {
        public SubProduct()
        {
        }

        public SubProduct(Product p)
        {
            this.Attribute01 = p.Attribute01;
            this.Attribute02 = p.Attribute02;
            this.Attribute03 = p.Attribute03;
            this.Attribute04 = p.Attribute04;
            this.Attribute05 = p.Attribute05;
            this.Attribute06 = p.Attribute06;
            this.Attribute07 = p.Attribute07;
            this.Attribute08 = p.Attribute08;
            this.Attribute09 = p.Attribute09;
            this.Attribute10 = p.Attribute10;
            this.Attribute11 = p.Attribute11;
            this.Attribute12 = p.Attribute12;
            this.Attribute13 = p.Attribute13;
            this.Attribute14 = p.Attribute14;
            this.Attribute15 = p.Attribute15;
            this.Attribute16 = p.Attribute16;
            this.Attribute17 = p.Attribute17;
            this.Attribute18 = p.Attribute18;
            this.Attribute19 = p.Attribute19;
            this.Attribute20 = p.Attribute20;
            this.Brand = p.Brand;
            this.Category = p.Category;
            this.Condition = p.Condition;
            this.DeliveryTime = p.DeliveryTime;
            this.Description = p.Description;
            this.FreeShipping = p.FreeShipping;
            this.GlobalBarcode = p.GlobalBarcode;
            this.ImageURL01 = p.ImageURL01;
            this.ImageURL02 = p.ImageURL02;
            this.ImageURL03 = p.ImageURL03;
            this.ImageURL04 = p.ImageURL04;
            this.ImageURL05 = p.ImageURL05;
            this.ImageURL06 = p.ImageURL06;
            this.ImageURL07 = p.ImageURL07;
            this.ImageURL08 = p.ImageURL08;
            this.ImageURL09 = p.ImageURL09;
            this.ImageURL10 = p.ImageURL10;
            this.MemberID = p.MemberID;
            this.OfferNote = p.OfferNote;
            this.Price = p.Price;
            this.ProductGroupSKU = p.ProductGroupSKU;
            this.ProductID = p.ProductID;
            this.ProductName = p.ProductName;
            this.ProviderID = p.ProviderID;
            this.Quantity = p.Quantity;
            this.SellingPrice = p.SellingPrice;
            this.ShippingProviders = p.ShippingProviders;
            this.SKU = p.SKU;
        }
    }

    public class Category
    {
        public string Name;
        public int CategoryID;
        public int ParentID;
        public List<Category> Children = new List<Category>();
    }

    public class CurrencyRates
    {
        public double USD = 1;
        public double TRY = -1;
        public double IRR = -1;
        public double AED = -1;
    }

    public static class LogHelper
    {
        public static Logger LogWriter = LogManager.GetCurrentClassLogger();
    }
}
