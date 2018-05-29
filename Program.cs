using IntegratorTarget.DataMapping;
using IntegratorTarget.DataTarget;
using IntegratorTarget.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratorTarget
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                LogHelper.LogWriter.Error("Invalid call. Usage: Engine.exe [Member] [Provider] [Target]. Params={0}", string.Join(",", args));
                return;
            }

            string Member = args[0].ToLowerInvariant();
            string Provider = args[1].ToLowerInvariant();
            string Target = args[2].ToLowerInvariant();

            LogHelper.LogWriter.Info("Targeting has been started : {0} {1}", Member, Provider, Target);
            Config.ReadConfig(Member, Provider, Target);

            AppDataProvider.Apply_Custom_Changes();

            #region Product List Preparation
            Dictionary<long, Product> SourceProductList = ProductDataProvider.GetProducts(Config.MemberID, Config.ProviderID);
            //Add Provider Prefix to SKUs
            foreach (var item in SourceProductList)
            {
                item.Value.SKU = Config.ProviderPrefix + item.Value.SKU;
                if (!string.IsNullOrWhiteSpace(item.Value.ProductGroupSKU))
                    item.Value.ProductGroupSKU = Config.ProviderPrefix + item.Value.ProductGroupSKU;
            }
            Dictionary<string, string> ParentSKUs = SourceProductList.Where(p => !string.IsNullOrWhiteSpace(p.Value.ProductGroupSKU)).Select(p => p.Value.ProductGroupSKU).Distinct().ToDictionary(a => a, b => b);
            Dictionary<string, Product> ParentProductList = SourceProductList.Where(p => ParentSKUs.ContainsKey(p.Value.SKU)).GroupBy(p => p.Value.SKU).ToDictionary(a => a.Key, b => b.First().Value);
            #endregion

            Dictionary<MappingType, Dictionary<string, string>> MappingTarget = AppDataProvider.Get_Mapping(Config.MemberID, Config.ProviderID, Config.TargetID);

            if (MappingTarget.ContainsKey(MappingType.Attribute01))
                AttributeMapping.MapAttribute01(SourceProductList, MappingTarget[MappingType.Attribute01]);
            if (MappingTarget.ContainsKey(MappingType.Attribute02))
                AttributeMapping.MapAttribute02(SourceProductList, MappingTarget[MappingType.Attribute02]);
            if (MappingTarget.ContainsKey(MappingType.Attribute04))
                AttributeMapping.MapAttribute04(SourceProductList, MappingTarget[MappingType.Attribute04]);
            if (MappingTarget.ContainsKey(MappingType.Attribute06))
                AttributeMapping.MapAttribute06(SourceProductList, MappingTarget[MappingType.Attribute06]);
            if (MappingTarget.ContainsKey(MappingType.Attribute12))
                AttributeMapping.MapAttribute12(SourceProductList, MappingTarget[MappingType.Attribute12]);
            if (MappingTarget.ContainsKey(MappingType.ProductName))
                AttributeMapping.MapProductName(SourceProductList, MappingTarget[MappingType.ProductName]);
            if (MappingTarget.ContainsKey(MappingType.Category))
                AttributeMapping.MapCategory(SourceProductList, MappingTarget[MappingType.Category]);

            AttributeMapping.MapPrice(SourceProductList, Config.PriceFormula);


            if (Target == "souq")
            {
                SourceProductList = Souq.Validation(SourceProductList);

                string Output = Souq.Output(SourceProductList, ParentProductList);
                Util.WriteOutputFile(Member, Provider, Target, ".csv", Output);
            }
            else if (Target == "myreyon")
            {
                SourceProductList = Myreyon.Validation(SourceProductList);
                
                string Output = Myreyon.Output(SourceProductList, ParentProductList);
                Util.WriteOutputFile(Member, Provider, Target, ".xml", Output);
            }
            else if (Target == "bamilo")
            {
                SourceProductList = Bamilo.Validation(SourceProductList);
                
                string Output = Bamilo.Output(SourceProductList);
                Util.WriteOutputFile(Member, Provider, Target, ".xml", Output);
            }

        }
    }
}
