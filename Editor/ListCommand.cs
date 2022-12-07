using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace PackageManagerTools {
    internal class ListCommand {

        private ListRequest request;
        public Action<List<ExtendedPackageInfo>> OnPackageListRetrieved;
        public bool isExecuting = false;

        public ListCommand(Action<List<ExtendedPackageInfo>> OnPackageListRetrieved) {
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
                if (request.Status == StatusCode.Success) {
                    if (request.Result != null) {
                        List<ExtendedPackageInfo> extendedPackageInfos = new List<ExtendedPackageInfo>();
                        foreach(PackageInfo package in request.Result) {
                            //ignore all unity based packages, as we know those won't have git dependencies
                            if (package.name.IndexOf("com.unity") != -1) {
                                continue;
                            }
                            ExtendedPackageInfo extendedPackageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtendedPackageInfo>(File.ReadAllText(Path.Combine(package.resolvedPath, "package.json")));
                            extendedPackageInfo.name = package.name;
                            extendedPackageInfo.version = package.version;
                            extendedPackageInfo.documentationUrl = package.documentationUrl;
                            extendedPackageInfos.Add(extendedPackageInfo);
                        }

                        OnPackageListRetrieved?.Invoke(extendedPackageInfos);
                    }
                } else if (request.Status >= StatusCode.Failure) {
                    GitDependencyResolver.Log(request.Error.message);
                }

                isExecuting = false;
                EditorApplication.update -= EditorTick;
            }
        }
    }
}
