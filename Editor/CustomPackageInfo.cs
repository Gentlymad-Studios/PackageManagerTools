using Newtonsoft.Json;
using System.Collections.Generic;

namespace PackageManagerTools {
    /// <summary>
    /// We highjack the flexibility of the package.json format by simply adding some of our own fields.
    /// Luckily Unity's package manager delivers the package.json as is so we can retrieve the real URLs by specifying "gitDepedencies".
    /// 
    /// Example portion of the package.json:
    /// 
    /// List of git dependencies:
    /// 
    /// "gitDependencies": {
    /// 	"com.gentlymadstudios.editorui": "https://github.com/Gentlymad-Studios/EditorUI.git"
    /// },
    /// 
    /// The link/url where we can retrieve the package.json of the hosted git repository for verison checking:
    /// 
    /// "custom_packageJsonLink": "https://raw.githubusercontent.com/Gentlymad-Studios/EditorUI/master/package.json",
    /// Surely you can add additional properties in this file.
    /// </summary>
    [System.Serializable]
    public class CustomPackageInfo {
        [JsonProperty("custom_gitDependencies")]
        public Dictionary<string, string> gitDependencies;
        [JsonProperty("custom_packageJsonLink")]
        public string packageJsonLink;
    }
}
