using UnityEditor.PackageManager;

namespace PackageManagerTools {
    public class AdvancedPackageInfo {
        public CustomPackageInfo custom;
        public PackageInfo main;
        public AdvancedPackageInfo(PackageInfo main, CustomPackageInfo custom = null) {
            this.custom = custom;
            this.main = main;
        }
    }
}
