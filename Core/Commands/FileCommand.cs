namespace AssimilationSoftware.MediaSync.Core.Commands
{
    /// <summary>
    /// Records details of a single file sync operation, such as a copy or delete.
    /// </summary>
    public abstract class FileCommand
    {
        public abstract void Replay();
    }
}
