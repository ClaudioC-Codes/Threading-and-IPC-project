﻿using System;
using System.Threading;


namespace Threading_and_IPC_project
{

    class Threading
    {
        private static Mutex _lock1 = new Mutex();
        private static Mutex _lock2 = new Mutex();
        

        public void BasicThreadOperations() //Uses 11mb of memory on my environment & 0.12% CPU usage
        {
            
            Console.WriteLine("\n--- Initializing Basic Thread Operations ---\n");
            
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

            //These .Joins are here to prevent the main method from continuing until the threads are done processing.
            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();
            thread5.Join();
            thread6.Join();
            thread7.Join();
            thread8.Join();
            thread9.Join();
            thread10.Join();
            
            Console.WriteLine("\n--- Basic Thread Operations Complete ---\n");
            
        }

        public void ResourceProtection() //Uses 11mb of memory & Highest CPU usage was 1%
        {
            
            Console.WriteLine("\n--- Initializing Resource Protection ---\n");
            
            BankAccountMutex david = new BankAccountMutex("David", "Martin", 5000.50);
            List<Thread> threads = new List<Thread>(); //Will store the threads.

            for (int i = 0; i < 10; i++)
            {
                String name = $"[Thread {i}]";
                Thread thread = new Thread(() => WithdrawSequenceMutex(david));
                thread.Name = name;
                thread.Start();
                threads.Add(thread);
            }

            //Calls .Join on each thread in the list. This allows all the threads to continue running whilst forcing main to wait for all of them to finished.
            //This maintains thread concurrency.
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            
            Console.WriteLine("\n--- Resource Protection Complete ---\n");
            
        }

        public void DeadlockCreation()
        {
            
            Console.WriteLine("\n--- Initializing Deadlock Creation ---\n");
            
            BankAccountMutex account1 = new BankAccountMutex("David", "Martin", 5000.50);
            BankAccountMutex account2 = new BankAccountMutex("Alexa", "Bowler", 2600.55);
            
            Thread thread1 = new Thread(() => TransferAToB(account1, account2));
            thread1.Name = "[Thread 1]";
            Thread thread2 = new Thread(() => TransferBToA(account2, account1));
            thread2.Name = "[Thread 2]";
            
            thread1.Start();
            thread2.Start();
            
            thread1.Join(3000);
            thread2.Join(3000);
            
            Console.WriteLine("\n--- Deadlock Creation Complete ---\n");
            
            //Comment lines out when testing the method. Used to end the program and threads when called from the menu.
            Console.WriteLine("Terminating program to end all threads and ensure safety.");
            Environment.Exit(0);   
            
        }

        public void DeadlockResolution()
        {
            
            Console.WriteLine("\n--- Initializing Deadlock Resolution ---\n");
            
            BankAccountMutex account1 = new BankAccountMutex("David", "Martin", 5000.50);
            BankAccountMutex account2 = new BankAccountMutex("Alexa", "Bowler", 2600.55);

            Thread thread1 = new Thread(() => DetectDeadlockA(account1, account2));
            thread1.Name = "[Thread 1]";
            Thread thread2 = new Thread(() => DetectDeadlockB(account2, account1));
            thread2.Name = "[Thread 2]";
            
            thread1.Start();
            thread2.Start();
            
            thread1.Join();
            thread2.Join();
            
            Console.WriteLine("\n--- Deadlock Resolution Complete ---\n");

        }

        public void WithdrawSequence(BankAccount account) //Series of transactions that will be done on the account objects
        {
            account.Withdraw(1000.00);
            Thread.Sleep(1000);
            account.Withdraw(2000.00);
            Thread.Sleep(1000);
            account.Withdraw(500.00);
            Thread.Sleep(1000);
            account.Withdraw(750.23);
        }
        
        public void WithdrawSequenceMutex(BankAccountMutex account)
        {
            Random rand = new Random();

            for(int i = 0; i < 10; i++) {
                account.Withdraw(Math.Round(rand.NextDouble() * 5000, 2));
                Thread.Sleep(1000); //Thread sleeping is no longer needed due to Mutex locking. However, I still employ it so I can watch the console as it happens.
            }
            
        }

        //Transfer methods used in DeadlockCreation()
        public void TransferAToB(BankAccountMutex accountA, BankAccountMutex accountB) //Method accesses _lock1 first but is blocked from _lock2 causing a deadlock
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
        
        public void TransferBToA(BankAccountMutex accountB, BankAccountMutex accountA) //Method accesses _lock2 first but is blocked from _lock1 causing a deadlock.
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

        //Transfer methods used in DeadlockResolution(). These methods will detect the deadlock and cancel transactions returning funds.
        public void DetectDeadlockA(BankAccountMutex accountA, BankAccountMutex accountB) //Method accesses _lock1 first but is blocked from _lock2 causing a deadlock
        {

            if (_lock1.WaitOne(TimeSpan.FromSeconds(1))) //Applying a timeout to the lock acquisition
            {
                
                try
                {
                    
                    accountA.Withdraw(500.00);
                    Thread.Sleep(1000); //Ensure other thread catches up. Simulating more work.
                    
                    if (_lock2.WaitOne(TimeSpan.FromSeconds(1))) //Deadlock here
                    {
                        
                        try
                        {
                            accountB.Deposit(500.00); //Method is never able to reach this due to Deadlock
                        }
                        finally
                        {
                            _lock2.ReleaseMutex();
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name}A deadlock was detected at method A, lock 2. Transaction cancelled, returning funds.");
                        accountA.Deposit(500.00);
                    }

                }
                finally
                {
                    _lock1.ReleaseMutex();
                }
                
                
            }
            else
            {
                Console.WriteLine("A deadlock was detected at method A, lock 1.");
            }


        }
        
        public void DetectDeadlockB(BankAccountMutex accountB, BankAccountMutex accountA) //Method accesses _lock2 first but is blocked from _lock1 causing a deadlock.
        {
            if (_lock2.WaitOne(TimeSpan.FromSeconds(1))) //Applying a timeout to the lock acquisition
            {
                
                try
                {
                    
                    accountB.Withdraw(500.00);
                    Thread.Sleep(1000); //Ensure other thread catches up. Simulating more work.
                    
                    if (_lock1.WaitOne(TimeSpan.FromSeconds(1))) //Deadlock here
                    {
                        
                        try
                        {
                            accountA.Deposit(500.00); //Method is never able to reach this due to Deadlock
                        }
                        finally
                        {
                            _lock1.ReleaseMutex();
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name}A deadlock was detected at method B, lock 1. Transaction cancelled, returning funds.");
                        accountB.Deposit(500.00);
                    }

                }
                finally
                {
                    _lock2.ReleaseMutex();
                }
                
                
            }
            else
            {
                Console.WriteLine("A deadlock was detected at method B, lock 2.");
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
                Console.WriteLine($"{Thread.CurrentThread.Name} deposited {amount}, to {name} {lastName}.");
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
                    Console.WriteLine($"{Thread.CurrentThread.Name} has withdrawn {amount}, from {name} {lastName}. Balance remaining: {balance}.");
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

