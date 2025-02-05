using System;
using System.Diagnostics;
using System.IO;


namespace Threading_and_IPC_project
{

    public class IPC
    {

        static void Main(string[] args)
        {
            //Testing basic cat command to read file. Going to add pipes "|" later.
            string command = "cat TransactionHistory.txt";
            Console.WriteLine(BashCommand(command));
            
            


        }

        static string BashCommand(string command)
        {
            var psi = new ProcessStartInfo(); //Creating a process
            psi.FileName = "/bin/bash"; //This is what calls bash shell to do commands
            psi.Arguments = $"-c \"{command}\""; //This argument holds the command passed through the method
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            
            using var process = Process.Start(psi);

            process.WaitForExit();
            
            var output = process.StandardOutput.ReadToEnd();
            
            return output;
        }
        

    }
}