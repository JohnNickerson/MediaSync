using System;
namespace AssimilationSoftware.MediaSync.Core
{
    /// <summary>
    /// An interface for gathering input from the user.
    /// </summary>
    public interface IInputView
    {

        /// <summary>
        /// Prompts to configure a numeric value (unsigned long) or accept a default.
        /// </summary>
        /// <param name="value">The default value to present.</param>
        /// <param name="prompt">The description to provide to the user.</param>
        /// <returns>The configured value, whether default or overridden.</returns>
        ulong ConfigureULong(ulong value, string prompt);

        /// <summary>
        /// Prompts to configure a path based on an existing value.
        /// </summary>
        /// <param name="path">The path as it exists. May include "{MyDocs}" as a placeholder.</param>
        /// <param name="prompt">The human-friendly name of the folder to be used as a cue.</param>
        /// <returns>The correct path as provided by the user.</returns>
        string ConfigurePath(string path, string prompt);

        /// <summary>
        /// Prompts to configure a string value, or accept a default.
        /// </summary>
        /// <param name="value">The initial default value.</param>
        /// <param name="prompt">A prompt for the user.</param>
        /// <returns>The configured value as entered or accepted by the user.</returns>
        string ConfigureString(string value, string prompt);

        /// <summary>
        /// Waits for a keypress from the user.
        /// </summary>
        void WaitForKey();
    }
}
