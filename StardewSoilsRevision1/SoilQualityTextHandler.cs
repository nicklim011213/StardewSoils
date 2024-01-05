using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
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
           if (e.DataType == typeof(Seeds))
           {
                //TODO write some code to append requirments to the end of seeds
           }
        }
    }
}
