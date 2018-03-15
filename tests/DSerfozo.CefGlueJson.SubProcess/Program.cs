using System;
using Xilium.CefGlue;

namespace DSerfozo.CefGlueJson.SubProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            CefRuntime.Load();

            var mainArgs = new CefMainArgs(args);

            var app = new RpcCefApp();
            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);

            Console.WriteLine("CefRuntime.ExecuteProcess() returns {0}", exitCode);
            if (exitCode != -1)
                Environment.Exit(exitCode);
        }
    }
}
