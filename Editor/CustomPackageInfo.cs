using System.Collections.Generic;

namespace PackageManagerTools {
    /// <summary>
    /// We highjack the flexibility of the package.json format by simply adding some of our own fields.
    /// Luckily Unity's package manager delivers the package.json as is so we can retrieve the real URLs by specifying "gitDepedencies".
    /// 
    /// Example portion of the package.json:
    /// 
    /// "gitDependencies": {
    /// 	"com.gentlymadstudios.editorui": "https://github.com/Gentlymad-Studios/EditorUI.git"
    /// },
    /// 
    /// Surely you can add additional properties in this file.
    /// </summary>
    [System.Serializable]
    public class CustomPackageInfo {
        public Dictionary<string, string> gitDependencies;
    }
}
