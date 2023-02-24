# Package Manager Tools

![Version](https://img.shields.io/github/package-json/v/Gentlymad-Studios/PackageManagerTools)
![GitHub last commit](https://img.shields.io/github/last-commit/Gentlymad-Studios/PackageManagerTools)
![GitHub](https://img.shields.io/github/license/Gentlymad-Studios/PackageManagerTools)
![GitHub issues](https://img.shields.io/github/issues-raw/Gentlymad-Studios/PackageManagerTools)

This package helps to automate some Unity package manager tasks and fill in some feature gaps.

![gitupdate](https://user-images.githubusercontent.com/530629/206590096-63bd0417-e6f9-43fd-bfdc-3481455abebb.png)

*Picture of the git update window that lists any outstanding git package updates.*

## Current Features
- *git package dependency* resolving
- *Detects git package updates* by checking the remote source
- Update checks are offloaded to another thread to be as low on performance as possible
- Update checks are done periodically
- Creating an optional blacklist where you can avoid to resolve certain dependency based on their unique identifier.

**This package makes some assumptions you should be aware of:**
- There is no compatibility check for dependencies, the system will always initiate the download of the newest git dependency
- Same for update checks. The system will only look for the latest release and download the newest version.

## Installation
1. Add [https://github.com/Gentlymad-Studios/PackageManagerTools.git](https://github.com/Gentlymad-Studios/PackageManagerTools.git) as a git url to your unity project using the standard package manager
2. Add git dependencies to the `package.json` you want to use. The syntax for this is like so:
```json
"custom_gitDependencies": {
  "com.gentlymadstudios.editorui": "https://github.com/Gentlymad-Studios/EditorUI.git"
},
```
3. If you are not using github, specifiy a link in you package.json where the system can retrieve the `package.json` for remote version checking.
```json
"custom_packageJsonLink": "https://raw.githubusercontent.com/Gentlymad-Studios/EditorHelper/master/package.json",
```
4. Add your git packages via the package manager as usual.
5. If you have valid git dependencies, these should now be automatically installed after your main package installed.
6. If you want to update git packages more conveniently, open the `GitUpdateWindow` found under `Tools`.

## Optional: Add a blacklist
![image](https://user-images.githubusercontent.com/530629/220437463-27a35f97-aee2-4d3e-8b6c-74fcecfe812e.png)

If you need to prevent or excempt certain depedencies to automatically resolve you can add a blacklist.
This is useful in cases where a dependency might already be available in your project (e.g. as a plugin or embedded in your project view).

**To create a blacklist you need to:**
1. Right click in your project view in Unity and select: `Create > PackageManagerTools > Blacklist`
2. Create a new entry by clicking on the '+' icon of the list.
3. Add the unique identifier of a package (example: com.gentlymad.odinserializer) to exclude when dependencies should be resolved.

~ Use at you own risk. ~
