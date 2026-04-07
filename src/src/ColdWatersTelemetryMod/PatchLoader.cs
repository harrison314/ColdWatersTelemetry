using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ColdWatersMod.Patches;
using HarmonyLib;
using System;


namespace ColdWatersMod
{

    [BepInPlugin("CwTP.harrison314", "Cold Waters Telemetry mod", "1.0.0.0")]
    class PatchLoader : BaseUnityPlugin
    {
        private ConfigEntry<string> endpoint;
        public static SimpleSseTcpServer Server = new SimpleSseTcpServer();

        private void Awake()
        {
            LogHelper.SetLogSource(this.Logger);
            Harmony harmony = new Harmony("CwTP.harrison314");
            harmony.PatchAll();

            this.endpoint = this.Config.Bind(
               "Network",
               "Endpoint",
               "localhost:2222",
               "Endpoint in IP:PORT format where a TCP socket is opened for SSE endpoint"
           );

            LogHelper.LogInfo("Start server: {0}", this.endpoint.Value);

            try
            {
                Server.OnConnected += this.Server_OnConnected;
                Server.Start(this.endpoint.Value);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Error: {0}", ex);
            }
        }

        private void Server_OnConnected(object sender, EventArgs e)
        {
            ResetEvenets();
        }

        internal static void ResetEvenets()
        {
            MissionManager_PopulateBriefingText.sendEvents = true;
            PlayerFunctions_SetPlayerData.sendEvents = true;
        }

        private void OnDestroy()
        {
            LogHelper.LogTrace("Stop server");

            try
            {
                Server.Stop();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Error: {0}", ex);
            }
        }
    }
}
