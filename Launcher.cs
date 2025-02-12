

namespace Threading_and_IPC_project
{
    
    public class Launcher
    {

        static void Main(string[] args)
        {
            
            Threading p1 = new Threading();
            IPC p2 = new IPC();
            string choice = "";
            bool repeat = true;
            bool validExit = true;
            
            
            do
            {

                do
                {
                    Console.WriteLine("Which function would you like to showcase.\nA: Basic Thread Operation\nB: Resource Protection\nC: Deadlock Creation - Warning: The program will stop remaining in the deadlock. Use this last or prepare to restart.\nD: Deadlock Resolution\nE: IPC implementation");
                    choice = Console.ReadLine().ToUpper();
                    
                    switch (choice)
                    {
                        case "A": p1.BasicThreadOperations(); validExit = true;
                            break;
                        case "B": p1.ResourceProtection(); validExit = true;
                            break;
                        case "C": p1.DeadlockCreation(); validExit = true;
                            break;
                        case "D": p1.DeadlockResolution(); validExit = true;
                            break;
                        case "E": p2.PassCommand(); validExit = true;
                            break;
                        default: Console.WriteLine("Invalid choice, make sure to only input the letter of your choice. This is not case sensitive."); validExit = false;
                            break;  
                    }
                    
                } while (!validExit);

                do
                {
                    Console.WriteLine("Would you like to test another one? (y/n)");
                    choice = Console.ReadLine().ToUpper();
                
                    switch (choice)
                    {
                        case "Y": repeat = true; validExit = true;
                            break;
                        case "N": repeat = false; validExit = true;
                            break;
                        default: Console.WriteLine("Input incorrect, make sure to only input 'y' or 'n'."); validExit = false;
                            break;
                    }
                    
                } while (!validExit);

            } while(repeat);


        }



    }
    
    
}

