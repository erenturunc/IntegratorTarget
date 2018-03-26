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
        public static CurrencyRates Rates;

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

            Config.Rates = Util.ParseOpenExchangeRateCurrencies(CurrencyRatesJsonFilePath);
        }
    }

    public enum MappingType
    {
        Size,
        Color
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
