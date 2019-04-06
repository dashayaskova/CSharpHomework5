using System;
using System.Diagnostics;

namespace Homework5
{
    class MyThread
    {
        private ProcessThread _thread;

        public MyThread(ProcessThread thread)
        {
            _thread = thread;
        }

        public string Id
        {
            get
            {
                string id = "";
                try
                {
                    id = _thread.Id.ToString();
                }
                catch (Exception e)
                {
                   id = e.Message;
                }
                return id;
            }
        }

        public string State
        {
            get
            {
                string state = "";
                try
                {
                    state = _thread.ThreadState.ToString();
                }
                catch (Exception e)
                {
                    state = e.Message;
                }
                return state;
            }
        }

        public string Date
        {
            get
            {
                string date = "";
                try
                {
                    date = _thread.StartTime.ToString();
                }
                catch (Exception e)
                {
                    date = e.Message;
                }
                return date;
            }
        }

    }
}
