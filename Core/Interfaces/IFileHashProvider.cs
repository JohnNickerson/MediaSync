namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface IFileHashProvider
    {
        string ComputeHash(string filename);
    }
}
