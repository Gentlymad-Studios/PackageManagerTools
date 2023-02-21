# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.0.1] - 2022-12-07
### Added
- Initial release

## [0.0.2] - 2022-12-07
### Added
- Added ability to manually update all git packages
- Checked for package type, so we only operate on git packages

## [0.0.3] - 2022-12-08
### Added
- Internal refactoring and tidying up of the code base
- Update detection is now fully functional and offloaded to its own thread

## [0.0.4] - 2022-12-08
### Added
- Created editor window to manage git package updates
- The link for version checking can now be declared in the package.json as "custom_packageJsonLink"
- The link for git dependencies is now done via "custom_gitDependencies" in package.json
- More refactoring and tidying up

## [0.0.5] - 2023-01-13
### Fixed
- force repaint window only if it is currently opened (prevent automatic opening of window)
- force repaint is now only triggered if there are package updates

## [0.0.6] - 2023-02-21
### Fixed
- fixed a potentially too early call to the package manager API

