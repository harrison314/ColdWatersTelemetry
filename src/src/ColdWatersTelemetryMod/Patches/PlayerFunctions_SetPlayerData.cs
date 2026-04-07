using ColdWatersMod.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColdWatersMod.Patches
{
    [HarmonyPatch(typeof(PlayerFunctions), "SetPlayerData")]
    public class PlayerFunctions_SetPlayerData
    {
        internal static bool sendEvents = false;

        private static int playerDepthInFeet_plugin = int.MinValue;
        private static int curse = -500000;
        private static float playerSpeed = -500000.0f;
        private static bool depthUnderKeelWarned = false;
        private static bool iceWarned = false;
        private static bool mineWarned = false;
        private static float rudderAngle = -500000.0f;
        private static float diveAngle = -500000.0f;
        private static float balast = -500000.0f;

        [HarmonyPostfix]
        public static void UpdatePostfix(PlayerFunctions __instance)
        {
            bool localSendEvents = sendEvents;

            if (localSendEvents || depthUnderKeelWarned != __instance.depthUnderKeelWarned)
            {
                depthUnderKeelWarned = __instance.depthUnderKeelWarned;
                try
                {
                    PatchLoader.Server.WriteMessage(string.Concat("{\"kid\":\"depthUnderKeelWarned\",\"value\":", depthUnderKeelWarned, "}"));
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(PlayerFunctions_SetPlayerData));
                }
            }

            if (localSendEvents || iceWarned != __instance.iceWarned)
            {
                iceWarned = __instance.iceWarned;
                try
                {
                    PatchLoader.Server.WriteMessage(string.Concat("{\"kid\":\"iceWarned\",\"value\":", iceWarned, "}"));
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(PlayerFunctions_SetPlayerData));
                }
            }

            if (localSendEvents || mineWarned != __instance.mineWarned)
            {
                mineWarned = __instance.mineWarned;
                try
                {
                    PatchLoader.Server.WriteMessage(string.Concat("{\"kid\":\"mineWarned\",\"value\":", mineWarned, "}"));
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(PlayerFunctions_SetPlayerData));
                }
            }

            if (localSendEvents || __instance.playerVessel.vesselmovement.shipSpeed.z != playerSpeed)
            {
                playerSpeed = __instance.playerVessel.vesselmovement.shipSpeed.z;

                try
                {
                    PatchLoader.Server.WriteMessage(string.Concat("{\"kid\":\"speed\"", "" +
                        ", \"value\":", __instance.playerVessel.vesselmovement.shipSpeed.z * 10f,
                        "}"));
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(PlayerFunctions_SetPlayerData));
                }
            }

            int localCurse = (int)Math.Round((double)__instance.playerVessel.transform.eulerAngles.y);
            if (localSendEvents || localCurse != curse)
            {
                if (localCurse == 360)
                {
                    localCurse = 0;
                }

                try
                {
                    PatchLoader.Server.WriteMessage(string.Concat("{\"kid\":\"curse\",\"value\":", localCurse, "}"));
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(PlayerFunctions_SetPlayerData));
                }

                curse = localCurse;
            }

            if (localSendEvents || __instance.playerDepthInFeet != playerDepthInFeet_plugin)
            {
                try
                {
                    PatchLoader.Server.WriteMessage(string.Concat("{\"kid\":\"depth\",\"value\":", __instance.playerDepthInFeet, "}"));
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(PlayerFunctions_SetPlayerData));
                }

                playerDepthInFeet_plugin = __instance.playerDepthInFeet;
            }

            if (localSendEvents || __instance.playerVessel.vesselmovement.rudderAngle.x != rudderAngle
                || __instance.playerVessel.vesselmovement.diveAngle.x != diveAngle)
            {
                rudderAngle = __instance.playerVessel.vesselmovement.rudderAngle.x;
                diveAngle = __instance.playerVessel.vesselmovement.diveAngle.x;

                try
                {
                    PatchLoader.Server.WriteMessage(string.Concat("{\"kid\":\"rudder\"", "" +
                        ", \"rudderAngle\":", rudderAngle * 10f,
                        ", \"diveAngle\":", diveAngle * 10f,
                        "}"));
                }
                catch (Exception ex)
                {
                    LogHelper.LogPatchError(ex, nameof(PlayerFunctions_SetPlayerData));
                }
            }

            if (__instance.ballastRechargeTimer < 0.0f)
            {
                float actualBallast = __instance.playerVessel.vesselmovement.ballastAngle.x * 10f;

                if (localSendEvents || actualBallast != balast)
                {
                    try
                    {
                        JsonBuilder jsonBuilder = new JsonBuilder();
                        jsonBuilder.Add("kid", "ballast");
                        jsonBuilder.Add("angle", actualBallast);
                        jsonBuilder.AddNull("time");

                        PatchLoader.Server.WriteMessage(jsonBuilder.ToJson());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogPatchError(ex, nameof(MissionManager_PopulateBriefingText));
                    }
                }
            }
            else
            {
                float num = __instance.ballastRechargeTimer / __instance.ballastRechargeTime;
                float actualBallast = num * 60f;

                if (localSendEvents || actualBallast != balast)
                {
                    try
                    {
                        JsonBuilder jsonBuilder = new JsonBuilder();
                        jsonBuilder.Add("kid", "ballast");
                        jsonBuilder.AddNull("angle");
                        jsonBuilder.Add("time", actualBallast);

                        PatchLoader.Server.WriteMessage(jsonBuilder.ToJson());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogPatchError(ex, nameof(MissionManager_PopulateBriefingText));
                    }
                }
            }

            if (localSendEvents)
            {
                sendEvents = false;
            }
        }
    }
}
