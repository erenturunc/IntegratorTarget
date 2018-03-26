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
            Dictionary<string, Product> ParentProductList = SourceProductList.Where(p => ParentSKUs.ContainsKey(p.Value.SKU)).ToDictionary(a => a.Value.SKU, b => b.Value);
            Dictionary<string, string> MappingColor = AppDataProvider.Get_Mapping(Config.MemberID, Config.ProviderID, Config.TargetID, MappingType.Color);
            Dictionary<string, string> MappingSize = AppDataProvider.Get_Mapping(Config.MemberID, Config.ProviderID, Config.TargetID, MappingType.Size);

            if (MappingColor.Count > 0)
                AttributeMapping.MapColor(SourceProductList, MappingColor);
            if(MappingSize.Count > 0)
                AttributeMapping.MapSize(SourceProductList, MappingSize);

            AttributeMapping.MapPrice(SourceProductList, Config.PriceFormula);

            if (Target == "souq")
            {
                SourceProductList = Souq.Validation(SourceProductList);

                Souq.Output(SourceProductList, ParentProductList);
                
            }

        }
    }
}
