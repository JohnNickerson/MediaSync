using System;
namespace AssimilationSoftware.MediaSync.Core
{
    /// <summary>
    /// An interface for gathering input from the user.
    /// </summary>
    public interface IInputView
    {
        /// <summary>
        /// Waits for a keypress from the user.
        /// </summary>
        void WaitForKey();
    }
}
