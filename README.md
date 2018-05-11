# MediaSync
MediaSync is a multi-master sync program designed to work from a flash drive. Using a virtual master file index, it determines the current state of the files on a machine and copies necessary changes to and from a space-limited shared folder. In this way, it is able, for instance, to (gradually) synchronise 100GB of files using an 8GB flash drive.

# Features
- Space limits: reserve as much or as little space for your sync file copies as you like.
- Copies only changed files.
- Zero-footprint on synchronised machines: the index metadata is stored on the flash drive.
- Data-preserving: both file versions are retained when conflicts are detected.
- Sync empty folders in addition to files.