using System;
using System.Collections.Generic;
using System.Diagnostics;
using AssimilationSoftware.MediaSync.Core.Model;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.FileManagement
{
    public class SimpleFileManager : IFileManager
    {
        private readonly IFileHashProvider _fileHasher;

        public SimpleFileManager(IFileHashProvider fileHasher)
        {
            _fileHasher = fileHasher;
            Errors = new List<Exception>();
        }

        public int Count => 0;

        public List<Exception> Errors { get; }

        public string ComputeHash(string localFile)
        {
            return _fileHasher.ComputeHash(localFile);
        }

        public FileCommandResult CopyFile(string source, string target)
        {
            try
            {
                // ensure the target folder exists.
                EnsureFolder(new FileInfo(target).DirectoryName);
                File.Copy(source, target, true);
                if (File.Exists(target))
                {
                    return FileCommandResult.Success;
                }
                else
                {
                    return FileCommandResult.Failure;
                }
            }
            catch (Exception e)
            {
                Errors.Add(e);
                return FileCommandResult.Failure;
            }
        }

        public FileCommandResult CopyFile(string localPath, string relativePath, string sharedPath)
        {
            return CopyFile(Path.Combine(localPath, relativePath), Path.Combine(sharedPath, relativePath));
        }

        public FileHeader CreateFileHeader(string localPath, string relativePath)
        {
            var fullpath = Path.Combine(localPath, relativePath);
            if (DirectoryExists(fullpath))
            {
                var dinfo = new DirectoryInfo(fullpath);
                return new FileHeader
                {
                    BasePath = localPath,
                    ContentsHash = string.Empty,
                    IsDeleted = false,
                    LastModified = dinfo.LastWriteTime,
                    RelativePath = relativePath,
                    IsFolder = DirectoryExists(fullpath)
                };
            }
            else
            {
                var finfo = new FileInfo(fullpath);
                return new FileHeader(_fileHasher)
                {
                    BasePath = localPath,
                    IsDeleted = false,
                    LastModified = finfo.LastWriteTime,
                    RelativePath = relativePath,
                    Size = finfo.Length,
                    IsFolder = DirectoryExists(fullpath)
                };
            }
        }
				
		/// <summary>
		/// Attempts to create a file header record for a given local file, if present.
		/// </summary>
		/// <returns>A FileHeader instance if possible, or null if the file does not exist.</returns>
		public FileHeader TryCreateFileHeader(string localPath, string relativePath)
		{
			try
			{
				return CreateFileHeader(localPath, relativePath);
			}
			catch
			{
				return null;
			}
		}

        public FileIndex CreateIndex(string path, params string[] searchpatterns)
        {
            FileIndex index = new FileIndex
            {
                LocalPath = path,
                TimeStamp = DateTime.Now
            };

            foreach (string file in ListLocalFiles(path, searchpatterns))
            {
                try
                {
                    var f = CreateFileHeader(index.LocalPath, file);
                    index.UpdateFile(f);
                }
                catch (Exception e)
                {
                    Errors.Add(e);
                }
            }
            return index;
        }

        public FileCommandResult Delete(string dir)
        {
            try
            {
                if (DirectoryExists(dir))
                {
                    Directory.Delete(dir);
                }
                else
                {
                    // Make sure no file attributes stand in our way.
                    File.SetAttributes(dir, FileAttributes.Normal);
                    File.Delete(dir);
                }
                return FileCommandResult.Success;
            }
            catch
            {
                return FileCommandResult.Failure;
            }
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void EnsureFolder(string targetdir)
        {
            if (!DirectoryExists(targetdir))
            {
                Directory.CreateDirectory(targetdir);
            }
        }

        public bool FileExists(string file)
        {
            return File.Exists(file);
        }

        public bool FileExists(string localPath, string relativePath)
        {
            return FileExists(Path.Combine(localPath, relativePath));
        }

        public bool FilesMatch(FileHeader masterfile, FileHeader localIndexFile)
        {
            return masterfile.Size == localIndexFile.Size
                && (masterfile.ContentsHash == null 
                || localIndexFile.ContentsHash == null 
                || masterfile.ContentsHash == localIndexFile.ContentsHash);
        }

        public bool FilesMatch(string literalFilePath, FileHeader indexFile)
        {
            if (FileExists(literalFilePath))
            {
                var finfo = new FileInfo(literalFilePath);
                return indexFile != null && indexFile.Size == finfo.Length && (indexFile.ContentsHash == null || indexFile.ContentsHash == ComputeHash(literalFilePath));
            }
            else
            {
                return false;
            }
        }

        public string GetConflictFileName(string localFile, string machineId, DateTime now)
        {
            var fileInfo = new FileInfo(localFile);
            var justname = Path.GetFileNameWithoutExtension(localFile);
            Debug.Assert(fileInfo.DirectoryName != null, "fileInfo.DirectoryName != null");
            var newname = Path.Combine(fileInfo.DirectoryName, string.Format("{0} ({1}-s conflicted copy {2:yyyy-MM-dd}){3}", justname, machineId, now, fileInfo.Extension));
            int ver = 0;
            while (File.Exists(newname))
            {
                ver++;
                newname = Path.Combine(fileInfo.DirectoryName, string.Format("{0} ({1}-s conflicted copy {2:yyyy-MM-dd}) ({3}){4}", justname, machineId, now, ver, fileInfo.Extension));
            }
            return newname;
        }

        public string[] GetDirectories(string parentFolder)
        {
            return Directory.GetDirectories(parentFolder, "*", SearchOption.AllDirectories);
        }

        public string GetRelativePath(string absolutePath, string basePath)
        {
            if (absolutePath.StartsWith(basePath))
            {
                if (basePath.EndsWith("\\"))
                {
                    return absolutePath.Remove(0, basePath.Length);
                }
                else
                {
                    return absolutePath.Remove(0, basePath.Length + 1);
                }
            }
            else
            {
                return absolutePath;
            }
        }

        public string[] ListLocalFiles(string path, params string[] searchPatterns)
        {
            List<string> result = new List<string>();
            Queue<string> queue = new Queue<string>();
            if (searchPatterns == null || searchPatterns.Length == 0)
            {
                searchPatterns = new[] { "*.*" };
            }
            queue.Enqueue(path);
            // While the queue is not empty,
            while (queue.Count > 0)
            {
                // Dequeue a folder to process.
                string folder = queue.Dequeue();
                // Enqueue subfolders.
                foreach (string subfolder in Directory.GetDirectories(folder))
                {
                    queue.Enqueue(subfolder);
                    result.Add(subfolder.Remove(0, path.Length + 1).Replace("/", "\\"));
                }
                // Add all image files to the index.
                foreach (string search in searchPatterns)
                {
                    foreach (string file in Directory.GetFiles(folder, search))
                    {
                        // Remove the base path.
                        string truncFile = file.Remove(0, path.Length + 1).Replace("/", "\\");
                        result.Add(truncFile);
                    }
                }
            }
            return result.ToArray();
        }

        public FileCommandResult MoveFile(string source, string target, bool overwrite)
        {
            if (!FileExists(target) || overwrite)
            {
                try
                {
                    EnsureFolder(new FileInfo(target).DirectoryName);
                    File.Move(source, target);
                }
                catch (Exception e)
                {
                    Errors.Add(e);
                    return FileCommandResult.Failure;
                }
            }
            return FileCommandResult.Success;
        }

        public void SetNormalAttributes(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
        }

        public ulong SharedPathSize(string path)
        {
            // Calculate the actual size of the shared path.
            ulong total = 0;
            // Search for all files, not just matching ones.
            // If some other files get mixed in, it could overrun the reserve space.
            foreach (string filename in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    total += (ulong)new FileInfo(filename).Length;
                }
                catch (FileNotFoundException) { }
            }
            return total;
        }

        public bool ShouldCopy(string filename)
        {
            return true;
        }
    }
}
