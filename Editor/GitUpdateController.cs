using System.Collections.Generic;
using UnityEditor;
using static PackageManagerTools.GitUpdateDetector;
using static PackageManagerTools.Settings;

namespace PackageManagerTools {

    [InitializeOnLoad]
    internal static class GitUpdateController {
        private static GitUpdateDetector changeDetector = new GitUpdateDetector(OnChangeDetected);
        private static ListCommand listCommand = new ListCommand(UpdateGitDependenciesForPackages);
        private static double nextAllowedTick;

        public static List<GitUpdatePackageInfo> packagesNeedingUpdate = new List<GitUpdatePackageInfo>();

        static GitUpdateController() {
            DetectPackageUpdates();

            EditorApplication.update -= EditorTick;
            EditorApplication.update += EditorTick;
        }


        private static void EditorTick() {
            if (EditorApplication.timeSinceStartup > nextAllowedTick) {
                DetectPackageUpdates();
            }
        }

        public static void DetectPackageUpdates() {
            nextAllowedTick = EditorApplication.timeSinceStartup + updateIntervalInSeconds;
            listCommand.Execute();
        }

        private static void UpdateGitDependenciesForPackages(List<AdvancedPackageInfo> packages) {
            changeDetector.Execute(packages);
        }

        private static void OnChangeDetected(GitChangeDetectorResult changeDetectorResult) {
            // Update all packages that actually need an update
            if (changeDetectorResult.packagesToUpdate.Count > 0) {
                packagesNeedingUpdate = changeDetectorResult.packagesToUpdate;
                //Client.AddAndRemove(changeDetectorResult.packagesToUpdate.ToArray());
            }

            // display all messages
            if (logPackageUpdates && changeDetectorResult.messages.Count > 0) {
                for (int i = 0; i < changeDetectorResult.messages.Count; i++) {
                    LogAlways(changeDetectorResult.messages[i]);
                }
            }

            if (changeDetectorResult.packagesToUpdate.Count > 0) {
                // Make sure to repaint the window
                GitUpdateWindow.ForceRepaint();
            }
        }
    }
}
