using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace PackageManagerTools {
    [InitializeOnLoad]
    internal static class GitDependencyResolver {
        private const string logHeader = "<b><color=#2E8B57>"+nameof(GitDependencyResolver) +"</color></b> ";

        [System.Diagnostics.Conditional("TOOLS_DEBUG")]
        public static void Log(string format, params object[] args) {
            UnityEngine.Debug.LogFormat(logHeader + format, args);
        }

        static GitDependencyResolver() {
            Log("Init");

            Events.registeredPackages -= RegisteredPackagesEventHandler;
            Events.registeredPackages += RegisteredPackagesEventHandler;
            Events.registeringPackages -= RegisteringPackagesEventHandler;
            Events.registeringPackages += RegisteringPackagesEventHandler;

        }

        [MenuItem("Tools/UpdateGitDependencies")]
        private static void UpdateGitDependencies() {
            (new ListCommand(UpdateGitDependenciesForPackages)).Execute();
        }

        private static void UpdateGitDependenciesForPackages(List<ExtendedPackageInfo> packages) {
            List<string> packagesToAdd = new List<string>();
            foreach (ExtendedPackageInfo package in packages) {
                string[] internalVersion = package.version.Split(".");

                string packageJsonUrl = package.documentationUrl.Replace("README.md", "package.json");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(packageJsonUrl);
                request.Method = "GET";
                request.ContentType = "application/json";
                WebResponse response = request.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                ExtendedPackageInfo remotePackageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtendedPackageInfo>(responseString);

                string[] remoteVersion = remotePackageInfo.version.Split(".");

                for(int i=0; i < 3; i++) {
                    if (int.Parse(remoteVersion[i]) > int.Parse(internalVersion[i])) {
                        packagesToAdd.Add(package.documentationUrl.Replace("/blob/master/README.md",".git"));
                        break;
                    }
                }

            }
            if (packagesToAdd.Count > 0) {
                Client.AddAndRemove(packagesToAdd.ToArray());
            }
        }

        private static void AddDependenciesForPackages(List<ExtendedPackageInfo> packages) {
            List<string> packagesToAdd = new List<string>();
            
            foreach (ExtendedPackageInfo package in packages) {
                if (package.gitDependencies != null) {
                    foreach (KeyValuePair<string, string> gitDependency in package.gitDependencies) {
                        bool packageAlreadyPresent = packages.Any(x => x.name == gitDependency.Key);
                        if (!packageAlreadyPresent && !packagesToAdd.Any(x => x == gitDependency.Value)) {
                            packagesToAdd.Add(gitDependency.Value);
                        }
                    }
                } else {
                    Log("no git dependencies provided");
                }
            }

            if (packagesToAdd.Count > 0) {
                Client.AddAndRemove(packagesToAdd.ToArray());
            }
        }

        // The method is expected to receive a PackageRegistrationEventArgs event argument.
        private static void RegisteringPackagesEventHandler(PackageRegistrationEventArgs packageRegistrationEventArgs) {
            Log("The list of registered packages is about to change!");

            foreach (var addedPackage in packageRegistrationEventArgs.added) {
                Log($"Adding {addedPackage.displayName}");
            }

            foreach (var removedPackage in packageRegistrationEventArgs.removed) {
                Log($"Removing {removedPackage.displayName}");
            }

        }

        private static void RegisteredPackagesEventHandler(PackageRegistrationEventArgs packageRegistrationEventArgs) {
            // Code executed here can safely assume that the Editor has finished compiling the new list of packages
            Log("The list of registered packages has changed!");

            if(packageRegistrationEventArgs.added != null && packageRegistrationEventArgs.added.ToArray().Length > 0) {
                (new ListCommand(AddDependenciesForPackages)).Execute();
            }
        }

    }
}
