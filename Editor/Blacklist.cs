using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PackageManagerTools {
    [CreateAssetMenu(fileName = nameof(PackageManagerTools) +nameof(Blacklist), menuName = nameof(PackageManagerTools)+"/"+nameof(Blacklist), order = 1)]
    public class Blacklist: ScriptableObject {
        /// <summary>
        /// A list of package identifiers that should never be resolved.
        /// </summary>
        [Tooltip("A list of package identifiers that should never be resolved.")]
        public List<string> blacklist;

        [NonSerialized]
        private static Blacklist _instance = null;
        public static Blacklist Instance {
            get {
                if (_instance == null) {
                    string[] guids = AssetDatabase.FindAssets($"t:{nameof(Blacklist)}");
                    if (guids.Length > 0) {
                        _instance = AssetDatabase.LoadAssetAtPath<Blacklist>(AssetDatabase.GUIDToAssetPath(guids[0]));
                    }
                }
                return _instance;
            }
        }

    }
}
