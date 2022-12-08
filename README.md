**Package Manager Tools** to automate some Unity package manager tasks and fill in some feature gaps.
![gitupdate](https://user-images.githubusercontent.com/530629/206590096-63bd0417-e6f9-43fd-bfdc-3481455abebb.png)

**Current Features**
- *git package dependency* resolving
- *Detects git package updates* by checking the remote source
- Update checks are offloaded to another thread to be as low on performance as possible
- Update checks are done periodically

**This package makes some assumptions you should be aware of:**
- There is no compatibility check for dependencies, the system will always initiate the download of the newest git dependency
- Same is for update checks. The system will only look for the latest release and download the newest version.

**Installation**
1. Add [https://github.com/Gentlymad-Studios/PackageManagerTools.git](https://github.com/Gentlymad-Studios/PackageManagerTools.git) as a git url to your unity project using the standard package manager
2. Add git dependencies to the `package.json` you want to use. The syntax for this is like so:
```
"custom_gitDependencies": {
  "com.gentlymadstudios.editorui": "https://github.com/Gentlymad-Studios/EditorUI.git"
},
```
3. If you are not using github, specifiy a link in you package.json where the system can retrieve the `package.json` for remote version checking.
```
"custom_packageJsonLink": "https://raw.githubusercontent.com/Gentlymad-Studios/EditorHelper/master/package.json",
```
4. Add your git packages via the package manager as usual.
5. If you have valid git dependencies, these should now be automatically installed after your main package installed.
6. If you want to update git packages more conveniently, open the `GitUpdateWindow` found under `Tools`.

~ Use at you own risk. ~
