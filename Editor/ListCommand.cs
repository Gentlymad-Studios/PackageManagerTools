using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using static PackageManagerTools.Settings;

namespace PackageManagerTools {
    /// <summary>
    /// Wrapper class to handle the package managers list command and only operate on git specific packages.
    /// With this we decouple ourselves to interfere with ane packages or depedency resolvement, except git.
    /// This makes sense as the package manager is otherwise very capable of resolving any other depedencies except git stuff...
    /// </summary>
    internal class ListCommand {

        private ListRequest request;
        public Action<List<AdvancedPackageInfo>> OnPackageListRetrieved;
        public bool isExecuting = false;

        public ListCommand(Action<List<AdvancedPackageInfo>> OnPackageListRetrieved) {
            this.OnPackageListRetrieved = OnPackageListRetrieved;
        }

        public void Execute() {
            if (!isExecuting) {
                //UnityEngine.Debug.Log("firing");
                isExecuting = true;
                request = Client.List();
                EditorApplication.update += EditorTick;
            }
        }

        private void EditorTick() {
            if (request.IsCompleted) {
                // unsubscribe from the editor tick early on so we won't fire to eternity of there is an error.
                EditorApplication.update -= EditorTick;
                isExecuting = false;

                if (request.Status == StatusCode.Success) {
                    if (request.Result != null) {

                        List<AdvancedPackageInfo> packageInfos = new List<AdvancedPackageInfo>();
                        foreach(PackageInfo package in request.Result) {
                            //ignore all unity based packages, as we know those won't have git dependencies
                            if (package.source != PackageSource.Git && package.source != PackageSource.Local|| package.name.IndexOf(unitySpecificPackagePartial) != -1) {
                                continue;
                            }

                            CustomPackageInfo extendedPackageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomPackageInfo>(File.ReadAllText(Path.Combine(package.resolvedPath, packageJson)));
                            packageInfos.Add(new AdvancedPackageInfo(package, extendedPackageInfo));
                        }

                        OnPackageListRetrieved?.Invoke(packageInfos);
                    }
                } else if (request.Status >= StatusCode.Failure) {
                    LogAlways(request.Error.message);

                }
            }
        }
    }
}
