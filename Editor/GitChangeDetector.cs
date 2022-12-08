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
    public class GitChangeDetector {

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
        public class ThreadSafePackageInfo {
            public string name;
            public string packageId;
            public string internalVersion;
        }

        /// <summary>
        /// Wrapper class for the result that is transfered after our asynchronous task is done.
        /// </summary>
        public class GitChangeDetectorResult {
            public List<string> packagesToAdd = new List<string>();
            public List<string> messages = new List<string>();

            public void Clear() {
                messages.Clear();
                packagesToAdd.Clear();
            }

            public void Add(string url, string message) {
                packagesToAdd.Add(url);
                messages.Add(message);
            }
        }

        private List<ThreadSafePackageInfo> infos = new List<ThreadSafePackageInfo>();
        private GitChangeDetectorResult result = new GitChangeDetectorResult();
        private Action<GitChangeDetectorResult> OnChangeDetection;
        private bool isExecuting = false;

        public GitChangeDetector(Action<GitChangeDetectorResult> OnChangeDetection) {
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
                    infos.Add(new ThreadSafePackageInfo() {
                        internalVersion = info.@base.version,
                        name = info.@base.name,
                        packageId = info.@base.packageId,
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

            Task task = new Task(() => {
                result.Clear();
                foreach (ThreadSafePackageInfo info in infos) {

                    try {
                        // retrieve the base git url from the package id
                        string gitUrl = info.packageId.Split("@")[1];
                        gitUrl = gitUrl.Substring(0, gitUrl.Length - 4);

                        // get the package.json url
                        string packageJsonUrl = gitUrl.Replace("github.com", "raw.githubusercontent.com");
                        packageJsonUrl += pathPartialToPackageJson.Replace("blob/", "");
                        string[] internalVersion = info.internalVersion.Split(versionNumberSeperator);

                        // retrieve package.json file from out remote location
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(packageJsonUrl);
                        request.Method = webRequestType;
                        request.ContentType = webRequestContentType;
                        WebResponse response = request.GetResponse();
                        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        MiniPackageInfo remotePackageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<MiniPackageInfo>(responseString);
                        string[] remoteVersion = remotePackageInfo.version.Split(versionNumberSeperator);

                        // check for changes in all version digits
                        for (int i = 0; i < 3; i++) {
                            // lets support downgrading as well (just in case someone made a mistake with the version numbers)
                            if (int.Parse(remoteVersion[i]) != int.Parse(internalVersion[i])) {
                                result.Add(gitUrl + repoExtenstion, $"Package: [{info.name}] detected a different version on github! {info.internalVersion} != {remotePackageInfo.version}");
                                break;
                            }
                        }
                    } catch (Exception e) {
                        result.messages.Add(e.Message);
                    }

                }
            });
            task.Start();
            await task;
            isExecuting = false;
            OnChangeDetection(result);
        }
    }
}
