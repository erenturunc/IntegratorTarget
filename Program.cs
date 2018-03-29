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

            Dictionary<long, Product> SourceProductList = ProductDataProvider.GetProducts(Config.MemberID, Config.ProviderID);
            Dictionary<string, string> ParentSKUs = SourceProductList.Where(p => !string.IsNullOrWhiteSpace(p.Value.ProductGroupSKU)).Select(p => p.Value.ProductGroupSKU).Distinct().ToDictionary(a => a, b => b);
            Dictionary<string, Product> ParentProductList = SourceProductList.Where(p => ParentSKUs.ContainsKey(p.Value.SKU)).GroupBy(p => p.Value.SKU).ToDictionary(a => a.Key, b => b.First().Value);
            Dictionary<MappingType, Dictionary<string, string>> MappingTarget = AppDataProvider.Get_Mapping(Config.MemberID, Config.ProviderID, Config.TargetID);
            
            if (MappingTarget.ContainsKey(MappingType.Attribute02))
                AttributeMapping.MapAttribute02(SourceProductList, MappingTarget[MappingType.Attribute02]);
            if (MappingTarget.ContainsKey(MappingType.Attribute04))
                AttributeMapping.MapAttribute04(SourceProductList, MappingTarget[MappingType.Attribute04]);
            if (MappingTarget.ContainsKey(MappingType.Attribute06))
                AttributeMapping.MapAttribute06(SourceProductList, MappingTarget[MappingType.Attribute06]);

            AttributeMapping.MapPrice(SourceProductList, Config.PriceFormula);

            if (Target == "souq")
            {
                SourceProductList = Souq.Validation(SourceProductList);

                Souq.Output(SourceProductList, ParentProductList);
            }
            else if (Target == "myreyon")
            {
                SourceProductList = Myreyon.Validation(SourceProductList);

                Myreyon.Output(SourceProductList, ParentProductList);
            }

        }
    }
}
