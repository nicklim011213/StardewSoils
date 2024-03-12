using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StardewSoils
{
    static internal class SoilQualityTextHandler
    {
        public static void SeedDescriptionFixer(object sender, AssetRequestedEventArgs e, IModHelper helper, IMonitor monitor)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var ObjectDict = asset.AsDictionary<int, string>();
                    for(int i = 0; i <= 930; ++i) 
                    {
                        if (CropGrowthRefrences.SeedToCrop.TryGetValue(i, out int SeedId))
                        {
                            ObjectDict.Data[SeedId] += PlantStringBuilder(i);
                        }
                    }
                    
                });
                //monitor.Log("Requested a type of seed asset", LogLevel.Warn);
                //TODO write some code to append requirments to the end of seeds
            }
        }

        public static string PlantStringBuilder(int CropIndex)
        {
            string Reqs = "";
            CropGrowthRefrences.CropLookup.TryGetValue(CropIndex, out var Requirments);

            Reqs += "\nN:";
            for (int i = Math.Abs((int)Requirments.X); i != 0; i--)
            {
                Reqs += ArrowChoice(Requirments.X);
            }

            Reqs += "\nP:";
            for (int i = Math.Abs((int)Requirments.Y); i != 0; i--)
            {
                Reqs += ArrowChoice(Requirments.Y);
            }

            Reqs += "\nK:";
            for (int i = Math.Abs((int)Requirments.Z); i != 0; i--)
            {
                Reqs += ArrowChoice(Requirments.Z);
            }

            return Reqs;
        }

        public static string ArrowChoice(float Req)
        {
            string Arrow;
            if (Req > 0)
                Arrow = "+";
            else if (Req < 0)
                Arrow = "-";
            else
                Arrow = " ";

            return Arrow;
        }
    }
}
