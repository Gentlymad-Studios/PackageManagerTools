using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static PackageManagerTools.Settings;

namespace PackageManagerTools {
    /// <summary>
    /// This class detects changes between present git packages and their remote counter parts.
    /// </summary>
    public class GitUpdateDetector {

        /// <summary>
        /// Minimal PackageInfo class that only contains what we really need.
        /// This is done to speed up deserialization auf the package.json
        /// </summary>
        public class MiniPackageInfo {
            public string version;
        }

        /// <summary>
        /// A minimal set of info that is needed to do our asynchronous task.
        /// This needs to be thread safe and not touch any unity API.
        /// </summary>
        public class GitUpdatePackageInfo {
            public string name;
            public string packageId;
            public string internalVersion;
            public string remoteVersion;
            public string packageJsonLink;
            public string gitUrl;
            public string displayName;
        }

        /// <summary>
        /// Wrapper class for the result that is transfered after our asynchronous task is done.
        /// </summary>
        public class GitChangeDetectorResult {

            public List<GitUpdatePackageInfo> packagesToUpdate = new List<GitUpdatePackageInfo>();
            public List<string> messages = new List<string>();

            public void Clear() {
                messages.Clear();
                packagesToUpdate.Clear();
            }

            public void Add(GitUpdatePackageInfo info, string message) {
                packagesToUpdate.Add(info);
                messages.Add(message);
            }
        }

        private List<GitUpdatePackageInfo> infos = new List<GitUpdatePackageInfo>();
        private GitChangeDetectorResult result = new GitChangeDetectorResult();
        private Action<GitChangeDetectorResult> OnChangeDetection;
        private bool isExecuting = false;

        public GitUpdateDetector(Action<GitChangeDetectorResult> OnChangeDetection) {
            this.OnChangeDetection = OnChangeDetection;
        }

        /// <summary>
        /// Start the execution.
        /// This will use an asynchronous task internally and report back with the "OnChangeDetection" callback.
        /// </summary>
        /// <param name="packages">The packageInfo to operate on.</param>
        public void Execute(List<AdvancedPackageInfo> packages) {
            if (!isExecuting) {
                infos.Clear();
                foreach (AdvancedPackageInfo info in packages) {
                    infos.Add(new GitUpdatePackageInfo() {
                        internalVersion = info.@base.version,
                        name = info.@base.name,
                        packageId = info.@base.packageId,
                        packageJsonLink = info.custom.packageJsonLink,
                        displayName = info.@base.displayName
                    });
                }
                ExecuteAsync();
            }
        }

        /// <summary>
        /// Execute the change detection asynchronously.
        /// </summary>
        private async void ExecuteAsync() {
            isExecuting = true;
            Task task = new Task(UpdateTask);
            task.Start();
            await task;
            isExecuting = false;
            OnChangeDetection(result);
        }

        private void UpdateTask() {
            result.Clear();
            foreach (GitUpdatePackageInfo info in infos) {

                try {

                    // retrieve the base git url from the package id
                    info.gitUrl = info.packageId.Split("@")[1];

                    // get the package.json url, if it was not found, try to resolve it automatically by the given giturl
                    if (info.packageJsonLink == null) {
                        info.packageJsonLink = ResolvePackageJsonLink(info.gitUrl);
                    }

                    if (info.packageJsonLink != null) {
                        string[] internalVersion = info.internalVersion.Split(versionNumberSeperator);
                        // retrieve package.json file from out remote location
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(info.packageJsonLink);
                        request.Method = webRequestType;
                        request.ContentType = webRequestContentType;
                        WebResponse response = request.GetResponse();
                        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                        // deserialize version info from remote response
                        MiniPackageInfo remotePackageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<MiniPackageInfo>(responseString);
                        string[] remoteVersion = remotePackageInfo.version.Split(versionNumberSeperator);
                        info.remoteVersion = remotePackageInfo.version;

                        // check for changes in all version digits
                        for (int i = 0; i < 3; i++) {
                            // lets support downgrading as well (just in case someone made a mistake with the version numbers)
                            if (int.Parse(remoteVersion[i]) != int.Parse(internalVersion[i])) {
                                result.Add(info, $"Package: [{info.name}] detected a different version on github! {info.internalVersion} != {remotePackageInfo.version}");
                                break;
                            }
                        }
                    } else {
                        result.messages.Add($"Package: [{info.name}] does not feature a valid package.json link! Please add 'custom_packageJsonLink' to the package.json file.");
                    }

                } catch (Exception e) {
                    result.messages.Add(e.Message);
                }

            }
        }
    }
}
