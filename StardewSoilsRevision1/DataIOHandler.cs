using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Text.Json;

namespace StardewSoils
{

    internal static class SaveHandler
    {
        public static string SAVEFILE = "";

        public static void WriteDataToFile(IModHelper helper)
        {
            helper.Data.WriteJsonFile("SoilData_" + Constants.SaveFolderName.ToString() + ".json", TileList.AllRegisteredTiles);
            SAVEFILE = "SoilData_" + Constants.SaveFolderName.ToString() + ".json";
        }

        public static void ClearData()
        {
            SAVEFILE = null;
        }

        public static void LoadData(IModHelper helper)
        {
            var model = helper.Data.ReadJsonFile<Dictionary<TilePosAndLoc, LinkedTileSoilStats>>("SoilData_" + Constants.SaveFolderName.ToString() + ".json"); //If anyone get figure out a way to use typeof(TileList.AllRegisteredTiles) in the type please do
            if (model == null)
            {
                helper.Data.WriteJsonFile("SoilData_" + Constants.SaveFolderName.ToString() + ".json", TileList.AllRegisteredTiles);
                SAVEFILE = "SoilData_" + Constants.SaveFolderName.ToString() + ".json";
            }
            else
            {
                SAVEFILE = "SoilData_" + Constants.SaveFolderName.ToString() + ".json";
                TileList.AllRegisteredTiles = model;
            }
        }
    }
}
