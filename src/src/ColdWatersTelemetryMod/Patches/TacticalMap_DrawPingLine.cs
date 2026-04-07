using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColdWatersMod.Patches
{

    [HarmonyPatch(typeof(TacticalMap), "DrawPingLine")]
    class TacticalMap_DrawPingLine
    {
        [HarmonyPostfix]
        public static void UpdatePostfix()
        {
            try
            {
                PatchLoader.Server.WriteMessage("{\"kid\":\"sonarPing\"}");
            }
            catch (Exception ex)
            {
                LogHelper.LogPatchError(ex, nameof(TacticalMap_DrawPingLine));
            }
        }
    }
}