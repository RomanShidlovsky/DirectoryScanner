using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class DirectoryScanner : IDirectoryScanner
    {
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private TaskQueue? _taskQueue;
        public bool IsRunning { get; private set; }
        
        public FileTree Start(string path, ushort maxThreadCount)
        {
            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                return new FileTree(new Node(fileInfo.FullName, fileInfo.Name, fileInfo.Length));
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentException($"Directory {path} does not exist");
            }

            if (maxThreadCount == 0)
            {
                throw new ArgumentException($"Max thread count should be greater than 0");
            }

            IsRunning = true;
            _taskQueue = new TaskQueue(maxThreadCount, _tokenSource);
            var token = _tokenSource.Token;
            var directoryInfo = new DirectoryInfo(path);
            var root = new Node(directoryInfo.FullName, directoryInfo.Name, true);
            var rootTask = new Task(() => ScanDirectory(root), token);
            _taskQueue.Enqueue(rootTask);
            _taskQueue.StartAndWaitAll();
            IsRunning = false;
            return new FileTree(root);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            IsRunning = false;
        }

        private void ScanDirectory(Node node)
        {
            node.Childs = new List<Node>();
            var directoryInfo = new DirectoryInfo(node.FullName);
            var token = _tokenSource.Token;

            DirectoryInfo[]? directories;
            try
            {
                directories = directoryInfo.GetDirectories().
                    Where(info => info.LinkTarget == null).ToArray(); ;
            }
            catch (Exception)
            {
                directories = null;
            }

            if (directories != null)
            {
                foreach (var directory in directories)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    Node childNode = new Node(directory.FullName, directory.Name, true);
                    node.Childs.Add(childNode);
                    Task task = new Task(() => ScanDirectory(childNode), token);
                    _taskQueue!.Enqueue(task);
                }
            }

            FileInfo[]? files;
            try
            {
                files = directoryInfo.GetFiles()
                    .Where(info => info.LinkTarget == null).ToArray();
            }
            catch
            {
                files = null;
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    Node childNode = new Node(file.FullName, file.Name, file.Length);
                    node.Childs.Add(childNode);
                }
            }
        }
    }
}
