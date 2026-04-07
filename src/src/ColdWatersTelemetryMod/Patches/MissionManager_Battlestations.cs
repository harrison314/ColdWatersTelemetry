using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColdWatersMod.Patches
{
    [HarmonyPatch(typeof(MissionManager), "Battlestations")]
    class MissionManager_Battlestations
    {
        [HarmonyPostfix]
        public static void UpdatePostfix(MissionManager __instance)
        {
            try
            {
                PatchLoader.Server.WriteMessage("{\"kid\":\"toBattlestations\"}");

                PatchLoader.ResetEvenets();
            }
            catch (Exception ex)
            {
                LogHelper.LogPatchError(ex, nameof(MissionManager_Battlestations));
            }
        }
    }
}
