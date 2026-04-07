using ColdWatersMod.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColdWatersMod.Patches
{
    [HarmonyPatch(typeof(PlayerFunctions), "PlayerMessage")]
    public class PlayerFunctions_PlayerMessage
    {
        [HarmonyPostfix]
        public static void UpdatePostfix(PlayerFunctions __instance, string message, UnityEngine.Color lineColor, string lookupValue, bool checkDuplicate = false)
        {
            try
            {
                JsonBuilder jsonBuilder = new JsonBuilder();
                jsonBuilder.Add("kid", "message");
                jsonBuilder.Add("value", message);
                jsonBuilder.Add("color", lineColor.ToCssColor());

                PatchLoader.Server.WriteMessage(jsonBuilder.ToJson());
            }
            catch (Exception ex)
            {
                LogHelper.LogPatchError(ex, nameof(PlayerFunctions_PlayerMessage));
            }
        }
    }
}
