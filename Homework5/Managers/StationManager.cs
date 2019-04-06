using System;


namespace Homework5.Managers
{
    internal static class StationManager
    {
        public static event Action StopThreads;


        internal static void CloseApp()
        {
            StopThreads?.Invoke();
            Environment.Exit(1);
        }
    }
}