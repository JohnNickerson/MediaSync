using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Model
{
    /// <summary>
    /// Records details of a single file sync operation, such as a copy or delete.
    /// </summary>
    public class SyncOperation
    {
        #region Fields
        /// <summary>
        /// The source file for a copy operation.
        /// </summary>
        public string SourceFile;
        /// <summary>
        /// The target file, eg the path to which the source is copied.
        /// </summary>
        public string TargetFile;
        /// <summary>
        /// The action taken on the file.
        /// </summary>
        public SyncAction Action;
        #endregion

        #region Types
        /// <summary>
        /// Specifies kinds of synchronisation actions.
        /// </summary>
        public enum SyncAction
        {
            Copy,
            Delete,
            Move
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new SyncOperation recording a single synchronisation action.
        /// </summary>
        /// <param name="source">The source file (eg copy from)</param>
        /// <param name="target">The target file (eg copy to)</param>
        /// <param name="action">The action taken</param>
        public SyncOperation(string source, string target, SyncAction action)
        {
            SourceFile = source;
            TargetFile = target;
            Action = action;
        }
        /// <summary>
        /// Constructs a new sync operation recording a delete action.
        /// </summary>
        /// <param name="deletetarget">The file to delete.</param>
        public SyncOperation(string deletetarget) : this(deletetarget, deletetarget, SyncAction.Delete)
        {
        }
        #endregion
    }
}
