using System;
using System.Diagnostics;

namespace Homework5
{
    class MyModule
    {
        private ProcessModule _module;

        public MyModule(ProcessModule module)
        {
            _module = module;
        }

      
        public string Name
        {
            get
            {
                string name = "";
                try
                {
                    name = _module.ModuleName;
                }
                catch (Exception e)
                {

                }
                return name;
            }
        }

        public string Path
        {
            get
            {
                string name = "";
                try
                {
                    name = _module.FileName;
                }
                catch (Exception e)
                {
                    name = e.Message;
                }
                return name;
            }
        }

    }
}
