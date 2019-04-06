using Homework5.Managers;
using Homework5.ViewModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Data;


namespace Homework5
{
    class ListViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private ConcurrentDictionary<long, MyProcess> _processes;
        private HashSet<long> _ids;
        private List<MyModule> _modules;
        private List<MyThread> _threads;
        private RelayCommand<object> _openCommand;
        private RelayCommand<object> _killCommand;
        private KeyValuePair<long,MyProcess> _selectedProc;
        private Thread _workingThread;
        private Thread _metaDatesThread;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private CollectionViewSource _collection;



        public CollectionViewSource ViewSource
        {
            get {
               if( _collection.View !=null) _collection.View.Refresh();
                return _collection;
            }

            set { _collection = value; OnPropertyChanged(); }
        }

        public RelayCommand<object> Kill
        {
            get
            {

                return _killCommand ?? (_killCommand = new RelayCommand<object>(
                     o => KillImplementation()));
            }
        }

        public RelayCommand<object> Open
        {
            get
            {

                return _openCommand ?? (_openCommand = new RelayCommand<object>(
                     o => OpenImplementation(), o => CanExecuteCommand()));
            }
        }

        private void OpenImplementation()
        {
            string argument = "/select, \"" + Item.Value.Path + "\"";
            System.Diagnostics.Process.Start("explorer.exe", @argument);
        }

        private void KillImplementation()
        {
          
                Item.Value.Kill();
                Thread.Sleep(1000);
                Sync();
                OnPropertyChanged("ViewSource");
                Item = default(KeyValuePair<long, MyProcess>);
        }




        private void Sync()
        {   
            _ids = new HashSet<long>(_processes.Keys);
            Process[] array = Process.GetProcesses();
            foreach (Process p in array)
            {
                MyProcess proc = new MyProcess(p);

                _processes.GetOrAdd(proc.Id, proc);
                _ids.Remove(proc.Id);

            }

            foreach (long id in _ids)
            {

                MyProcess ignored;
                _processes.TryRemove(id, out ignored);
            }

        }

        public ConcurrentDictionary<long, MyProcess> Processes
        {
            get
            {
                Sync();
                return _processes;
            }
            set
            {
       
                _processes = value;
                OnPropertyChanged();
            }
        }

        public List<MyModule> Modules
        {
            set
            {
                _modules = value;
                OnPropertyChanged();
            }
            get
            {
                return _modules;
            }
        }

        public List<MyThread> Threads
        {
            set
            {
                _threads = value;
                OnPropertyChanged();
            }
            get
            {
                return _threads;
            }
        }





        public KeyValuePair<long, MyProcess> Item
        {
            set
            {
                _selectedProc = value;
                
                if (!_selectedProc.Equals(default(KeyValuePair<long,MyProcess>)))
                {
                    Modules = _selectedProc.Value.ListOfModules;
                    Threads = _selectedProc.Value.ListOfThreads;
                }
                else
                {
                    Modules = null;
                    Threads = null;
                    
                }
               
                OnPropertyChanged();
            }
            get
            {
                return _selectedProc;
            }
        }


        private void StartWorkingThread()
        {
            _workingThread = new Thread(WorkingThreadProcess);
            _workingThread.Start();
            _metaDatesThread = new Thread(UpdateMetaDates);
            _metaDatesThread.Start();
        }

        private void WorkingThreadProcess()
        {
            int i = 0;
            while (!_token.IsCancellationRequested)
            {
                KeyValuePair<long, MyProcess> temp = Item;
                Sync();
                OnPropertyChanged("ViewSource");
                Item = temp;

                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(500);

                    if (_token.IsCancellationRequested)
                        break;
                }   
                i++;
            }
        }


        private void UpdateMetaDates()
        {
            int i = 0;
            while (!_token.IsCancellationRequested)
            {

                foreach (KeyValuePair<long, MyProcess> p in _processes)
                {
                    p.Value.Update();
                }

                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(200);

                    if (_token.IsCancellationRequested)
                        break;
                }
                i++;
            }
        }


        internal void StopWorkingThread()
        {
            _tokenSource.Cancel();
            _workingThread.Join(2000);
            _workingThread.Abort();
            _workingThread = null;
            _metaDatesThread.Join(2000);
            _metaDatesThread.Abort();
            _metaDatesThread = null;
        }

        public ListViewModel()
        {
            Processes = new ConcurrentDictionary<long, MyProcess>();
            _collection = new CollectionViewSource();
            ViewSource.Source = Processes;

            Process[] array = Process.GetProcesses();
            foreach (Process p in array)
            {
                MyProcess proc = new MyProcess(p);
                _processes.GetOrAdd(proc.Id, proc);
            }
            _ids = new HashSet<long>(_processes.Keys);

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartWorkingThread();
            StationManager.StopThreads += StopWorkingThread;
        }


        private bool CanExecuteCommand()
        {

            return (Item.Value?.Path != null);
        }
    }
}
