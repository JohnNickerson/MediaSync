using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileHeader
    {
        #region Constructors
        public FileHeader()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The path of the file, relative to the local base path.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// A flag to indicate that the file was deleted.
        /// </summary>
        /// <remarks>
        /// Used only in the master index. Records with this flag that do not exist in any satellite index should be removed.
        /// </remarks>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// The most recent file revision details.
        /// </summary>
        public FileRevision CurrentRevision
        {
            get
            {
                if (Revisions == null || Revisions.Count == 0)
                {
                    return new FileRevision();
                }
                else
                {
                    return Revisions.OrderBy(r => r.Revision).Last();
                }
            }
            set
            {
                if (Revisions == null)
                {
                    Revisions = new List<FileRevision>();
                }
                Revisions.Add(value);
            }
        }

        /// <summary>
        /// Revision history.
        /// </summary>
        /// <remarks>
        /// This is required for the master index, in case it encounters an old version of the file.
        /// The current revision details will match the highest revision here.
        /// </remarks>
        public List<FileRevision> Revisions { get; set; }
        #endregion
    }
}
