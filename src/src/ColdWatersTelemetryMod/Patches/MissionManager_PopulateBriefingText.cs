using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColdWatersMod.Patches
{
    [HarmonyPatch(typeof(MissionManager), "PopulateBriefingText")]
    class MissionManager_PopulateBriefingText
    {
        private static string breefing = null;
        internal static bool sendEvents = false;

        [HarmonyPostfix]
        public static void UpdatePostfix(MissionManager __instance)
        {
            bool localSendEvents = sendEvents;

            if (localSendEvents || !string.Equals(UIFunctions.globaluifunctions.mainColumn.text, breefing))
            {
                breefing = UIFunctions.globaluifunctions.mainColumn.text;

                try
                {
                    JsonBuilder jsonBuilder = new JsonBuilder();
                    jsonBuilder.Add("kid", "breefing");
                    jsonBuilder.Add("value", breefing);

                    PatchLoader.Server.WriteMessage(jsonBuilder.ToJson());
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(MissionManager_PopulateBriefingText));
                }
            }

            if (localSendEvents)
            {
                sendEvents = false;
            }
        }
    }
}
