using System.Diagnostics;
using UnityEngine;
using System.Threading;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Looks in the Server folder and runs the server.bat file 
/// which runs a python process and node process 
/// that communicate with each other through a socket connection
/// </summary>
public class Terminal : MonoBehaviour {

    Process node;
    Process python;

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    IntPtr currWindow;
   
    IEnumerator Start() {
        currWindow = GetActiveWindow();
        Thread server = new Thread(StartServer);
        server.Start();

        yield return new WaitForSeconds(4f);
        SwitchToThisWindow(currWindow, true);
    }

    void StartServer() {

        //run .bat file to start both processes
        ProcessStartInfo info = new ProcessStartInfo() {
            FileName = "c:/windows/syswow64/WindowsPowerShell/v1.0/powershell.exe",
            WorkingDirectory = Directory.GetCurrentDirectory() + "/Server",
            Arguments = "./server.bat",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process process = Process.Start(info);

        process.OutputDataReceived += (sender, args) => OnDataReceived(args.Data);
        process.ErrorDataReceived += (sender, args) => OnDataReceived(args.Data);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        Thread.Sleep(1000);

        //HACK: keeping track of the parent process and closing/disposing/killing does not seem to do anything at all even when I keep it running
        node = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == "node");
        python = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == "python");
    }

    void OnDataReceived(string data) {
        if (data.Length > 0) {
            UnityEngine.Debug.Log("FROM SERVER: " + data);
        }
    }

    void OnApplicationQuit() {
        node.Kill();
        python.Kill();
    }
}
