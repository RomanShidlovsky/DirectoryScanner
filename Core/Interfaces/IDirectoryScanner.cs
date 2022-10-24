using Core.Models;

namespace Core.Interfaces
{
    public interface IDirectoryScanner
    {
        FileTree Start(string path, ushort maxThreadCount);

        void Stop();

        bool IsRunning { get; }
    }
}
