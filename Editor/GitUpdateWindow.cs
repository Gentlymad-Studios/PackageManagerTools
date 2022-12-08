using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using static PackageManagerTools.GitUpdateDetector;
using static PackageManagerTools.Settings;

namespace PackageManagerTools {
    public class GitUpdateWindow : EditorWindow {

        private bool stylesInitialized = false;
        private HashSet<string> selectedPackages = new HashSet<string>();
        private GUIStyle richTextLabelStyle = null;
        private Vector2 scrollPos;
        // Add menu named "My Window" to the Window menu
        [MenuItem(updateGitPackagesMenu)]
        private static void Init() {
            // Get existing open window or if none, make a new one:
            GitUpdateWindow window = (GitUpdateWindow)GetWindow(typeof(GitUpdateWindow));
            window.titleContent = new GUIContent(nameof(GitUpdateWindow));
            window.Show();
        }

        private void SetupStyles() {
            if (!stylesInitialized) {
                stylesInitialized = true;

                richTextLabelStyle = new GUIStyle(GUI.skin.label);
                richTextLabelStyle.richText = true;
            }
        }

        private void AddPackages(string[] packages) {
            Client.AddAndRemove(packages);
        }

        private void OnGUI() {
            SetupStyles();

            if (GUILayout.Button("Detect Package Updates")) {
                GitUpdateController.DetectPackageUpdates();
            }
            EditorGUILayout.BeginHorizontal();
            bool guiEnabled = GUI.enabled;

            if (selectedPackages.Count == 0) {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Update Selected")) {
                AddPackages(selectedPackages.ToArray());
            }
            GUI.enabled = guiEnabled;

            if (GitUpdateController.packagesNeedingUpdate.Count == 0) {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Update All")) {
                string[] packagesToUpdate = new string[GitUpdateController.packagesNeedingUpdate.Count];
                for (int i = 0; i < packagesToUpdate.Length; i++) {
                    packagesToUpdate[i] = GitUpdateController.packagesNeedingUpdate[i].gitUrl;
                }
                AddPackages(packagesToUpdate);
            }
            GUI.enabled = guiEnabled;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            foreach (GitUpdatePackageInfo package in GitUpdateController.packagesNeedingUpdate) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"<b>{package.displayName} ({package.internalVersion})</b> | <color=yellow>Remote: {package.remoteVersion}</color>", richTextLabelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                bool isCurrentlySelected = selectedPackages.Contains(package.gitUrl);
                bool shouldBeSelected = EditorGUILayout.ToggleLeft("", isCurrentlySelected, GUILayout.Width(15));
                if (shouldBeSelected != isCurrentlySelected) {
                    if (shouldBeSelected && !isCurrentlySelected) {
                        selectedPackages.Add(package.gitUrl);
                    } else if (!shouldBeSelected && isCurrentlySelected) {
                        selectedPackages.Remove(package.gitUrl);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField($"<color=grey>url: {package.gitUrl}</color>", richTextLabelStyle);

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        public static void ForceRepaint() {
            ((GitUpdateWindow)GetWindow(typeof(GitUpdateWindow))).Repaint();
        }
    }
}
