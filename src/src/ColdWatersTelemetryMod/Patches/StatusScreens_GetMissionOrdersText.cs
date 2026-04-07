using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColdWatersMod.Patches
{
    [HarmonyPatch(typeof(StatusScreens), "GetMissionOrdersText")]
    class StatusScreens_GetMissionOrdersText
    {
        [HarmonyPostfix]
        public static void UpdatePostfix(StatusScreens __instance, ref string __result)
        {
            try
            {
                JsonBuilder jsonBuilder = new JsonBuilder();
                jsonBuilder.Add("kid", "missionOrders");
                jsonBuilder.Add("value", __result);

                PatchLoader.Server.WriteMessage(jsonBuilder.ToJson());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error: {0}", ex));
                LogHelper.LogPatchError(ex, nameof(StatusScreens_GetMissionOrdersText));
            }
        }
    }
}
