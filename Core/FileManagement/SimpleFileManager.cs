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

        public List<Exception> Errors { get; }

        public FileCommandResult CopyFile(string source, string target)
        {
            try
            {
                // ensure the target folder exists.
                EnsureFolder(new FileInfo(target).DirectoryName);
                File.Copy(source, target, true);
                Trace.WriteLine($"Copied  {source}  to  {target}");
                return File.Exists(target) ? FileCommandResult.Success : FileCommandResult.Failure;
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
                    Trace.WriteLine($"Deleted directory  {dir}");
                }
                else
                {
                    // Make sure no file attributes stand in our way.
                    File.SetAttributes(dir, FileAttributes.Normal);
                    File.Delete(dir);
                    Trace.WriteLine($"Deleted file  {dir}");
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
                Trace.WriteLine($"Created directory  {targetdir}");
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

        public string GetRelativePath(string absolutePath, string basePath)
        {
            if (absolutePath.StartsWith(basePath))
            {
                return absolutePath.Remove(0, basePath.Length + (basePath.EndsWith("\\") ? 0 : 1));
            }
            else
            {
                return absolutePath;
            }
        }

        public string[] ListLocalFiles(string path, params string[] searchPatterns)
        {
            var result = new List<string>();
            var queue = new Queue<string>();
            if (searchPatterns == null || searchPatterns.Length == 0)
            {
                searchPatterns = new[] { "*.*" };
            }
            queue.Enqueue(path);
            // While the queue is not empty,
            while (queue.Count > 0)
            {
                // Dequeue a folder to process.
                var folder = queue.Dequeue();
                // Enqueue subfolders.
                try
                {
                    foreach (var subfolder in Directory.GetDirectories(folder))
                    {
                        queue.Enqueue(subfolder);
                        result.Add(subfolder.Remove(0, path.Length + 1).Replace("/", "\\"));
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Could not process subfolders of " + folder);
                    Trace.WriteLine(e.Message);
                }
                // Add all image files to the index.
                foreach (var search in searchPatterns)
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(folder, search))
                        {
                            // Remove the base path.
                            var truncFile = GetRelativePath(file, path).Replace("/", "\\");
                            result.Add(truncFile);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Could not process files in " + folder);
                        Trace.WriteLine(e.Message);
                    }
                }
            }
            return result.ToArray();
        }

        public FileCommandResult MoveFile(string source, string target, bool overwrite)
        {
            if (FileExists(target) && !overwrite) return FileCommandResult.Success;
            try
            {
                EnsureFolder(new FileInfo(target).DirectoryName);
                File.Move(source, target);
                Trace.WriteLine($"Moved file  {source}  to  {target}");
            }
            catch (Exception e)
            {
                Errors.Add(e);
                return FileCommandResult.Failure;
            }
            return FileCommandResult.Success;
        }

        public ulong SharedPathSize(string path)
        {
            // Calculate the actual size of the shared path.
            ulong total = 0;
            // Search for all files, not just matching ones.
            // If some other files get mixed in, it could overrun the reserve space.
            foreach (var filename in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    total += (ulong)new FileInfo(filename).Length;
                }
                catch (FileNotFoundException) { }
            }
            return total;
        }
    }
}
