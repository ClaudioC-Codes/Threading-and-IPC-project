

namespace Threading_and_IPC_project
{
    
    public class Launcher
    {

        static void Main(string[] args)
        {
            
            Threading p1 = new Threading();
            IPC p2 = new IPC();
            
            //p1.DeadlockResolution();
            p2.PassCommand();
            
            
            
            
        }



    }
    
    
}

