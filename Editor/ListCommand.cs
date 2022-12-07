using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace PackageManagerTools {
    internal class ListCommand {
        private const string unitySpecificPackagePartial = "com.unity";
        private const string packageJson = "package.json";

        private ListRequest request;
        public Action<List<AdvancedPackageInfo>> OnPackageListRetrieved;
        public bool isExecuting = false;

        public ListCommand(Action<List<AdvancedPackageInfo>> OnPackageListRetrieved) {
            this.OnPackageListRetrieved = OnPackageListRetrieved;
        }

        public void Execute() {
            if (!isExecuting) {
                isExecuting = true;
                request = Client.List();
                EditorApplication.update += EditorTick;
            }
        }

        private void EditorTick() {
            if (request.IsCompleted) {
                EditorApplication.update -= EditorTick;

                if (request.Status == StatusCode.Success) {
                    if (request.Result != null) {

                        List<AdvancedPackageInfo> packageInfos = new List<AdvancedPackageInfo>();
                        foreach(PackageInfo package in request.Result) {
                            //ignore all unity based packages, as we know those won't have git dependencies
                            if (package.source != PackageSource.Git || package.name.IndexOf(unitySpecificPackagePartial) != -1) {
                                continue;
                            }

                            CustomPackageInfo extendedPackageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomPackageInfo>(File.ReadAllText(Path.Combine(package.resolvedPath, packageJson)));
                            packageInfos.Add(new AdvancedPackageInfo(package, extendedPackageInfo));
                        }

                        OnPackageListRetrieved?.Invoke(packageInfos);
                    }
                } else if (request.Status >= StatusCode.Failure) {
                    GitDependencyResolver.Log(request.Error.message);
                }
            }
        }
    }
}
