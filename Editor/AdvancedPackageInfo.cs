using UnityEditor.PackageManager;

namespace PackageManagerTools {
    /// <summary>
    /// A simple wrapper class to merge the standard package info by unity with our custom package info with additional fields.
    /// </summary>
    public class AdvancedPackageInfo {
        public CustomPackageInfo custom;
        public PackageInfo @base;
        public AdvancedPackageInfo(PackageInfo main, CustomPackageInfo custom = null) {
            this.custom = custom;
            this.@base = main;
        }
    }
}
