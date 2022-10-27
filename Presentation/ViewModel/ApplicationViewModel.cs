using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Services;
using Core.Models;
using Presentation.Command;
using Presentation.Model; 

namespace Presentation.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly DirectoryScanner _scanner = new DirectoryScanner();

        public RelayCommand SetDirectoryCommand { get; }
        public RelayCommand StartScanningCommand { get; }
        public RelayCommand StopScanningCommand { get; }

        public ApplicationViewModel()
        {
            SetDirectoryCommand = new RelayCommand(_ =>
            {
                using var folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    DirectoryPath = folderBrowserDialog.SelectedPath;
                }
            });

            StartScanningCommand = new RelayCommand(_ =>
            {
                Task.Run(() =>
                {
                    Core.Models.FileTree result = _scanner.Start(DirectoryPath, MaxThreadCount);
                    Tree = new Model.FileTree(result);
                });
            }, _ => _directoryPath != null && !_scanner.IsRunning);

            StopScanningCommand = new RelayCommand(_ =>
            {
                _scanner.Stop();
            }, _ => _scanner.IsRunning);
        }

        private string _directoryPath;
        public string DirectoryPath
        {
            get { return _directoryPath; }
            set
            {
                _directoryPath = value;
                OnPropertyChanged("DirectoryPath");
            }
        }

        private ushort _maxThreadCount = 100;
        public ushort MaxThreadCount
        {
            get { return _maxThreadCount; }
            set
            {
                _maxThreadCount = value;
                OnPropertyChanged("MaxThreadCount");
            }
        }

        private Model.FileTree _tree;
        public Model.FileTree Tree
        {
            get { return _tree; }
            private set
            {
                _tree = value;
                OnPropertyChanged("Tree");
            }

        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
