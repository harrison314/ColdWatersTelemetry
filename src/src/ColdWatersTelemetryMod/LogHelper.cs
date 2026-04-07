using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ColdWatersMod
{
    internal static class LogHelper
    {
        private static ManualLogSource logSource = null;

        internal static void SetLogSource(ManualLogSource source)
        {
            logSource = source;
        }

        public static void LogPatchError(Exception ex, string patch, [CallerMemberName] string method = "")
        {
            if (logSource != null)
            {
                logSource.LogError(string.Format("Error in {0}.{1}",
                   patch,
                   method,
                   ex.Message));

                logSource.LogError(ex);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                     "Error in {0}.{1}",
                     patch,
                     method,
                     ex.Message));

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public static void LogTrace(string text)
        {
            if (logSource != null)
            {
                logSource.Log(LogLevel.Message, text);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(text);
            }
        }

        public static void LogTrace(string format, params object[] args)
        {
            if (logSource != null)
            {
                logSource.Log(LogLevel.Message, string.Format(System.Globalization.CultureInfo.InvariantCulture,
                format,
                args: args));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                  format,
                  args: args));
            }
        }

        public static void LogInfo(string text)
        {
            if (logSource != null)
            {
                logSource.Log(LogLevel.Info, text);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(text);
            }
        }

        public static void LogInfo(string format, params object[] args)
        {
            if (logSource != null)
            {
                logSource.Log(LogLevel.Info, string.Format(System.Globalization.CultureInfo.InvariantCulture,
                format,
                args: args));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                  format,
                  args: args));
            }
        }

        public static void LogError(string text)
        {
            if (logSource != null)
            {
                logSource.Log(LogLevel.Error, text);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(text);
            }
        }

        public static void LogError(string format, params object[] args)
        {
            if (logSource != null)
            {
                logSource.Log(LogLevel.Error, string.Format(System.Globalization.CultureInfo.InvariantCulture,
                format,
                args: args));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                  format,
                  args: args));
            }
        }
    }
}
