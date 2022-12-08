using UnityEngine;

namespace PackageManagerTools {
    /// <summary>
    /// Settings class for commonly used helper functions or data.
    /// </summary>
    public class Settings {
        public const string webRequestType = "GET";
        public const string webRequestContentType = "application/json";
        public const string versionNumberSeperator = ".";
        public const string packageJson = "package.json";
        public const string logHeader = "<b><color=#2E8B57>" + nameof(PackageManagerTools) + "</color></b> ";
        public const string debugDefine = "TOOLS_DEBUG";
        public const string updateGitPackagesMenu = "Tools/GitUpdate";
        public const string unitySpecificPackagePartial = "com.unity";
        public const string githubBaseDomain = "github.com";
        public const string githubRawContentDomain = "raw.githubusercontent.com";
        public const string githubPathPartialToPackageJson = "/master/" + packageJson;
        public const bool logPackageUpdates = false;
        public const float updateIntervalInSeconds = 600;

        [System.Diagnostics.Conditional(debugDefine)]
        public static void Log(string format, params object[] args) {
            Debug.LogFormat(logHeader + format, args);
        }

        public static void LogAlways(string format, params object[] args) {
            Debug.LogFormat(logHeader + format, args);
        }

        public static string ResolvePackageJsonLink(string gitUrl) {
            gitUrl = gitUrl.Substring(0, gitUrl.Length - 4);

            if (gitUrl.Contains(githubBaseDomain)) {
                string packageJsonUrl = gitUrl.Replace(githubBaseDomain, githubRawContentDomain);
                packageJsonUrl += githubPathPartialToPackageJson;
                return packageJsonUrl;
            }
            return null;
        }


    }
}
