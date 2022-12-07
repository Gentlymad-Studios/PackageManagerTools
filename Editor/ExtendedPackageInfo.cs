using System.Collections.Generic;

namespace PackageManagerTools {
    [System.Serializable]
    public class ExtendedPackageInfo {
        [System.NonSerialized]
        public string name, version, documentationUrl;

        public Dictionary<string, string> gitDependencies;
    }
}
