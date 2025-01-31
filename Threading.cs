using System;
using System.Threading;


namespace Threading_and_IPC_project
{

    class Threading
    {
        private static Mutex _lock1 = new Mutex();
        private static Mutex _lock2 = new Mutex();
        static void Main(string[] args)
        {
            //Testing-Development-Deployment area
            
            


        }

        public static void BasicThreadOperations() //Uses 11mb of memory on my environment & 0.12% CPU usage
        {
            BankAccount account1 = new BankAccount("David", "Crow", 3500.25);
            BankAccount account2 = new BankAccount("Alexa", "Bowler", 2600.55);
            BankAccount account3 = new BankAccount("Reina", "Gonzales", 6430.20);
            BankAccount account4 = new BankAccount("Alice", "Johnson", 5000.00);
            BankAccount account5 = new BankAccount("Michael", "Smith", 3200.75);
            BankAccount account6 = new BankAccount("Samantha", "Brown", 1500.50);
            BankAccount account7 = new BankAccount("David", "Wilson", 7800.25);
            BankAccount account8 = new BankAccount("Emily", "Anderson", 2400.00);
            BankAccount account9 = new BankAccount("Robert", "Martinez", 9100.90);
            BankAccount account10 = new BankAccount("Jessica", "Taylor", 6700.35);
            
            //Testing concurrent threads with different accounts.
            Thread thread1 = new Thread(() => WithdrawSequence(account1));
            Thread thread2 = new Thread(() => WithdrawSequence(account2));
            Thread thread3 = new Thread(() => WithdrawSequence(account3));
            Thread thread4 = new Thread(() => WithdrawSequence(account4));
            Thread thread5 = new Thread(() => WithdrawSequence(account5));
            Thread thread6 = new Thread(() => WithdrawSequence(account6));
            Thread thread7 = new Thread(() => WithdrawSequence(account7));
            Thread thread8 = new Thread(() => WithdrawSequence(account8));
            Thread thread9 = new Thread(() => WithdrawSequence(account9));
            Thread thread10 = new Thread(() => WithdrawSequence(account10));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread5.Start();
            thread6.Start();
            thread7.Start();
            thread8.Start();
            thread9.Start();
            thread10.Start();
        }

        public static void ResourceProtection() //Uses 11mb of memory & Highest CPU usage was 1%
        {
            BankAccountMutex david = new BankAccountMutex("David", "Martin", 5000.50);

            for (int i = 0; i < 10; i++)
            {
                String name = $"[Thread {i}]";
                Thread thread = new Thread(() => WithdrawSequenceMutex(david));
                thread.Name = name;
                thread.Start();
            }
        }

        public static void DeadlockCreation()
        {
            BankAccountMutex account1 = new BankAccountMutex("David", "Martin", 5000.50);
            BankAccountMutex account2 = new BankAccountMutex("Alexa", "Bowler", 2600.55);
            
            Thread thread1 = new Thread(() => TransferAToB(account1, account2));
            thread1.Name = "[Thread 1]";
            Thread thread2 = new Thread(() => TransferBToA(account2, account1));
            thread2.Name = "[Thread 2]";
            
            thread1.Start();
            thread2.Start();
            
            
            
        }

        public static void WithdrawSequence(BankAccount account) //Series of transactions that will be done on the account objects
        {
            account.Withdraw(1000.00);
            Thread.Sleep(1000);
            account.Withdraw(2000.00);
            Thread.Sleep(1000);
            account.Withdraw(500.00);
            Thread.Sleep(1000);
            account.Withdraw(750.23);
        }
        
        public static void WithdrawSequenceMutex(BankAccountMutex account)
        {
            Random rand = new Random();

            for(int i = 0; i < 10; i++) {
                account.Withdraw(Math.Round(rand.NextDouble() * 5000, 2));
                Thread.Sleep(1000); //Thread sleeping is no longer needed due to Mutex locking. However, I still employ it so I can watch the console as it happens.
            }
            
        }

        public static void TransferAToB(BankAccountMutex accountA, BankAccountMutex accountB) //Method accesses _lock1 first but is blocked from _lock2 causing a deadlock
        {
             _lock1.WaitOne();
             try
             {
                accountA.Withdraw(500.00);
                Thread.Sleep(1000); //Ensure other thread catches up. Simulating more work.
                
                _lock2.WaitOne(); //Deadlock here
                try
                {
                    accountB.Deposit(500.00); //Method is never able to reach this due to Deadlock
                }
                finally
                {
                    _lock2.ReleaseMutex();
                }

             }
             finally
             {
                 _lock1.ReleaseMutex();
             }

        }
        
        public static void TransferBToA(BankAccountMutex accountB, BankAccountMutex accountA) //Method aaccesses _lock2 first but is blocked from _lock1 causing a deadlock.
        {
            _lock2.WaitOne();
            try
            {
                accountB.Withdraw(200.00);
                Thread.Sleep(1000); //Ensure other thread catches up. Simulating more work.
                
                _lock1.WaitOne(); //Deadlock here
                try
                {
                    accountA.Deposit(200.00); //Method is never able to reach this due to Deadlock
                }
                finally
                {
                    _lock1.ReleaseMutex();
                }

            }
            finally
            {
                _lock2.ReleaseMutex();
            }

        }


    }

    class BankAccount
    {
        private String name = "";
        private String lastName = "";
        private double balance = 0.0;

        public BankAccount()
        {
            name = "Some";
            lastName = "Guy";
            balance = 5000.0;
        }

        public BankAccount(String fName,String lName, double startBalance)
        {
            name = fName;
            lastName = lName;
            balance = startBalance;
        }

        public void Deposit(double amount)
        {
            balance += amount;
            Console.WriteLine("[" + name + " " + lastName + "]" + " Balance: " + balance);
        }

        public void Withdraw(double amount)
        {
            if (balance > amount)
            {
                balance -= amount;
                Console.WriteLine("[" + name + " " + lastName + "]" + " Balance: " + balance);
            }
            else
            {
                Console.WriteLine("[" + name + " " + lastName + "]" + " Insufficient funds");
            }
            
        }
        
    }
    
    class BankAccountMutex //Bank account with the implementation of Mutex locks
    {
        private String name = "";
        private String lastName = "";
        private double balance = 0.0;
        
        private readonly Mutex mutex = new Mutex(); //readonly to prevent reassignment and maintain thread safety

        public BankAccountMutex()
        {
            name = "Some";
            lastName = "Guy";
            balance = 5000.0;
        }

        public BankAccountMutex(String fName,String lName, double startBalance)
        {
            name = fName;
            lastName = lName;
            balance = startBalance;
        }

        public void Deposit(double amount) //Mutex implemented to ensure accurate balance updates.
        {
            mutex.WaitOne();
            try
            {
                balance += amount;
                Console.WriteLine($"{Thread.CurrentThread.Name} deposited {amount}.");
            }
            finally
            {
                mutex.ReleaseMutex();
            }
            
        }

        public void Withdraw(double amount) //Mutex ensures only 1 thread can access the balance during Withdraws, preventing accidental negative balances.
        {
            
            mutex.WaitOne();
            try
            {
                if (balance > amount)
                {
                    balance -= amount;
                    Console.WriteLine($"{Thread.CurrentThread.Name} has withdrawn {amount}. Balance remaining: {balance}.");
                }
                else
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} failed to withdraw {amount}. Insufficient funds.");
                    Deposit(5000.00);
                }  
            }
            finally
            {
                mutex.ReleaseMutex();
            }
            
            
        }
        
    }


}

