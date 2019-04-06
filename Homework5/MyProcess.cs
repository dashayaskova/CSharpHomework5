using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Homework5
{
    class MyProcess : INotifyPropertyChanged
    {
        private Process _process;
        List<MyModule> _modules;
        List<MyThread> _threads;
        PerformanceCounter _processorCounter;
        PerformanceCounter _theMemCounter;
        private float _cpu;
        private float _ram;
        private string _name;
        private int _id;
        private bool _isActive;
        private string _date;
        private string _path;
        private double _total;

        private void Counter()
        {
            _isActive = _process.Responding;
          
            try
            {
                _cpu = _processorCounter.NextValue() / Environment.ProcessorCount;
                _ram = _theMemCounter.NextValue();

 
            }
            catch (Exception e)
            {
               
            }
        }

        public void Update()
        {
            Counter();
            OnPropertyChanged("CPUUsage");
            OnPropertyChanged("RAMUsage");
            OnPropertyChanged("NumOfThreads");
            OnPropertyChanged("ListOfModules");
            OnPropertyChanged("ListOfThreads");
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void Kill()
        {
            try
            {
                _process.Kill();
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public MyProcess(Process process)
        {
            _process = process;
            _name = _process.ProcessName;
            _id = _process.Id;
            _processorCounter = new PerformanceCounter("Process", "% Processor Time", Name);
            _theMemCounter = new PerformanceCounter("Process", "Working Set", Name);
            _total = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            _isActive = _process.Responding;
            Counter();
            try
            {
                _date = _process.StartTime.ToString();
                _path = _process.MainModule.FileName;
            }
            catch (Exception e)
            {
                
            }
        }

        public List<MyModule> ListOfModules
        {
            get
            {
              
                    try
                    {
                
                        _modules = new List<MyModule>();

                        ProcessModuleCollection arr = _process.Modules;
                        foreach (ProcessModule p in arr)
                        {
                            MyModule m = new MyModule(p);
                            _modules.Add(m);
                        }
                    }
                
                catch (Exception e)
                    {

                    }
              
                
                return _modules;
            }
            set
            {
                _modules = value;
            }
        }

        public List<MyThread> ListOfThreads
        {
            get
            {

                try
                {
                    _threads = new List<MyThread>();

                    ProcessThreadCollection arr = _process.Threads;
                    foreach (ProcessThread p in arr)
                    {
                        MyThread m = new MyThread(p);
                        _threads.Add(m);
                    }

                }
                catch (Exception e)
                {

                }


                return _threads;
            }
            set
            {
                _threads = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public bool IsActice
        {
            get
            {
                return _isActive;
            }
        }

        public float CPUUsage
        {
            get
            {
                return (float)Math.Round(_cpu * 100f) / 100f;
            }
        }

        public int NumOfThreads
        {
            get
            {
                return _process.Threads.Count;
            }
        }

         public string Path{
            get
            {
                return _path;
            }
           }

      
     
        public string RAMUsage
        {
            get
            {
           
                return ((_ram/_total)*100).ToString("0.00") + ", "+(_ram/(1024*1024)).ToString("0.00");
            }
        }


        public string Date
        {
            get
            {
              
                return _date;
            }

        }


    }
}
