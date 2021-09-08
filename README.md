# MediaSync
MediaSync is a portable, multi-primary, sync program designed to work from a flash drive. Using a virtual primary file index, it determines the current state of the files on a machine and copies necessary changes to and from a space-limited shared folder. In this way, it is able, for instance, to (gradually) synchronise 100GB of files using an 8GB flash drive.

# Features
- No primary copy: changes can be made on any copy of any file. All changes will be replicated everywhere.
- Space limits: reserve as much or as little space for your sync file copies as you like.
- Copies only changed files (note: renamed files are treated as a delete and create).
- Zero-footprint on synchronised machines: the index metadata is stored on the flash drive.
- Data-preserving: both file versions are retained when conflicts are detected.
- Sync empty folders in addition to files.

# Releases
* 2021-09-09: v3.0.3
	- Renamed some file command arguments to be more intuitive.
	- Updated sync process to update, not replace, the file index.
	- Added "purge" command to force removal of orphaned file records that start to cause OutOfMemoryExceptions.
	- Added "state" option to "dir" command.
* 2021-09-03: v3.0.1.1
	- Workaround: added "purge" command to force removal of orphaned file records that start to cause OutOfMemoryExceptions.
* 2021-08-19: v3.0.1
	- Fixed library size update treating MB as bytes.
	- Added warning about files too large for reserve space.
	- Changed results report to reflect actual actions taken.
* 2021-08-16: v3.0.0.11
	- Fixed log files.
* 2021-08-02: v3.0.0.10
	- Fixed delete confirmation.
	- Streamlined some logging.
* 2021-07-22: v3.0.0.9
	- Fixed new replica deleting synchronised files. 
	- Limited orphan data purging to delete operations.
* 2021-07-15: v3.0.0.8
	- Fixed exception when deleting unknown replica.
	- Fixed bug leaving orphaned replicas when deleting libraries.
* 2021-07-05: v3.0.0.7
	- Added user confirmation before deleting 10% or more of a library's files.
* 2021-06-28: v3.0.0.6
	- Updated data model to allow multiple replicas per machine.
* 2021-03-01: v2.4.3.2
	- Fixed last known drive letter detection.
* 2021-02-23: v2.4.3
	- Added automatic detection of drive letter changes.
	- Sorted action queues by file name length for nicer display.
* 2021-02-18: v2.4.2
	- Added a "change-drive" command to update the drive letter of the flash drive.
* 2020-12-10: v2.4.1.2
	- Fixed a relative path determination bug in SimpleFileManager.
* 2020-05-27: v2.4.1.1
	- Fixed calls to Debug and Trace.WriteLine to use string interpolation.
* 2020-05-21: v2.4.1
	- Restored timestamps in logs.
	- Overhauled logging system.
* 2020-03-05: v2.4.0
	- Fixed file share cleanup for files deleted in transit..
	- Updated final result reporting mechanism to match individual profile results..
	- Some refactoring.
* 2020-01-30: v2.3.3
	- Fixed share folder cleanup for files deleted in transit state.
	- Updated in-memory lists to dictionaries for faster performance.
	- Tidied up references and "using" statements.
* 2020-01-02: v2.3.2
	- Interrupt sync.
	- Undelete folders.
* 2019-08-29: v2.3.1
	- Fixed file browse button in GUI config window.
	- Show profile reserve space in verbose output.
	- Dates in log.
	- Full action count summary at end of run.
* 2019-08-08: v2.3.0
	- LiteDb storage option.
	- Flash GUI toolbar button when finished.
	- Simplified display when no changes are needed.
* 2019-06-13: v2.2.2
  - Adjust reserve space.
  - Tracing fixes.
* 2019-05-21: v2.2.1
  - Better initial primary index building.
  - Limit overall flash drive space usage to 90%, regardless of other limits.
  - Added logging.
* 2019-05-10: v2.2.0.1
  - Patched to fix file access error in hash calculation.
* 2019-03-28: v2.2.0
  - Repository data storage.
  - Fix for CLI ignoring data path setting.
  - Scrolling for GUI output window.
* 2019-03-14: v2.1.0.1
  - Basic GUI (very much a work in progress)
  - Better error trapping
  - Check for local folder existence to prevent exception
* 2018-10-04: v2.0.11
  - Shared folder cleanup now removes unindexed files.
* 2018-09-13: v2.0.10
  - Added profile name displays to table output.
  - Bug and stability fixes.