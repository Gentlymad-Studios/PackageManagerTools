using UnityEngine;

namespace PackageManagerTools {
    /// <summary>
    /// Settings class for commonly used helper functions or data.
    /// </summary>
    public class Settings {
        public const string webRequestType = "GET";
        public const string webRequestContentType = "application/json";
        public const string versionNumberSeperator = ".";
        public const string repoExtenstion = ".git";
        public const string packageJson = "package.json";
        public const string pathPartialToPackageJson = "/blob/master/" + packageJson;
        public const string logHeader = "<b><color=#2E8B57>" + nameof(PackageManagerTools) + "</color></b> ";
        public const string debugDefine = "TOOLS_DEBUG";
        public const string updateGitPackagesMenu = "Tools/UpdateGitDependencies";
        public const string unitySpecificPackagePartial = "com.unity";

        [System.Diagnostics.Conditional(debugDefine)]
        public static void Log(string format, params object[] args) {
            Debug.LogFormat(logHeader + format, args);
        }

        public static void LogAlways(string format, params object[] args) {
            Debug.LogFormat(logHeader + format, args);
        }
    }
}
