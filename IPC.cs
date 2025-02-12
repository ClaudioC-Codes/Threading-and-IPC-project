using System;
using System.Diagnostics;
using System.IO;


namespace Threading_and_IPC_project
{

    public class IPC
    {

        public void PassCommand()
        {
            
            Console.WriteLine("\n--- Initializing IPC Implementation ---\n");
            
            //This is the string holding our Bash command which calls multiple text processes that communicate through pipes.
            //First it calls cat which is used to open the .txt file, this file is then passed to sort through a pipe.
            //Second in sort, the contents are sorted alphabetically by the first letter in each line. Information is then passed to tr.
            //Thirdly tr is capable of translating and deleting characters. Here it replaces all the underscores with spaces and then passes the information to sed.
            //Finaly sed is a stream editor. Here it is removing the first 3 characters on each line.
            string command = "cat TransactionHistory.txt | sort | tr '_' ' ' | sed 's/^...//' ";
            
            Console.WriteLine(BashCommand(command)); //This is where the command is passed to the method and its contents are printed to console.
            
            Console.WriteLine("\n--- IPC Implementation Complete ---\n");

        }

        public string BashCommand(string command)
        {
            
            //This code is based off a reddit users approach.
            //https://www.reddit.com/r/csharp/comments/kg6us8/is_there_is_a_way_to_execute_bash_commands_from_c/
            // 
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