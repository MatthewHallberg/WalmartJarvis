using System.Diagnostics;
using UnityEngine;
using System.Threading;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System;
using System.Collections;

/// <summary>
/// Looks in the Server folder and runs the server.bat file 
/// which runs a python process and node process 
/// that communicate with each other through a socket connection.
/// *This would be much easier to just run on a server but I thought this would be *CoOoOoOLEr* cause I'm fuckin stoopid.*
/// </summary>

public class Terminal : MonoBehaviour {

    public delegate void OnInitialized(bool succcess);
    public static OnInitialized initialized;

    Process node;
    Process python;

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    IntPtr currWindow;
   
    void Awake() {
        currWindow = GetActiveWindow();
    }

    public void StartTerminal() {
        Thread server = new Thread(StartServer);
        server.Start();
        //ping process so we know when brain file loads
        API.Instance.MakeChatBotRequest(Config.TEST_QUESTION, OnTestQuestionAnswered);
    }

    void StartServer() {

        //run .bat file to start both processes
        ProcessStartInfo info = new ProcessStartInfo() {
            //Can't just use 'powershell.exe' here because it defaults to 32bit?
            FileName = "c:/windows/syswow64/WindowsPowerShell/v1.0/powershell.exe",
            WorkingDirectory = Directory.GetCurrentDirectory() + "/Server",
            Arguments = "./server.bat"
        };

        Process process = Process.Start(info);

        Thread.Sleep(1000);

        //HACK: keeping track of the parent process and closing/disposing/killing does not seem to do anything at all even when I keep it running (also why there is a .bat file to just run both)
        node = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == "node");
        python = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == "python");
    }

    int pingCount = 0;
    void OnTestQuestionAnswered(BotData data) {
        bool success = data.speech == Config.TEST_ANSWER;
        //this only works when called in the same thread as GetActiveWindow (I think?)
        SwitchToThisWindow(currWindow, true);
        initialized?.Invoke(success);
    }

    void OnDataReceived(string data) {
        if (data.Length > 0) {
            UnityEngine.Debug.Log("FROM SERVER: " + data);
        }
    }

    void OnApplicationQuit() {
        node?.Kill();
        python?.Kill();
    }
}
