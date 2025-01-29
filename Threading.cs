using System;
using System.Threading;


namespace Threading_and_IPC_project
{

    class Threading
    {
        static void Main(string[] args)
        {

            BankAccount David = new BankAccount("David", "Crow", 3500.25);
            BankAccount Alexa = new BankAccount("Alexa", "Bowler", 2600.55);
            BankAccount Reina = new BankAccount("Reina", "Gonzales", 6430.20);

            //Testing concurrent strings with different accounts.
            Thread thread1 = new Thread(() => WithdrawSequence(David));
            Thread thread2 = new Thread(() => WithdrawSequence(Alexa));
            Thread thread3 = new Thread(() => WithdrawSequence(Reina));

            thread1.Start();
            thread2.Start();
            thread3.Start();

        }

        //Series of transactions that will be down on the account objects
        public static void WithdrawSequence(BankAccount account)
        {
            account.Withdraw(1000.00);
            account.Withdraw(2000.00);
            account.Withdraw(500.00);
            account.Withdraw(750.23);
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
                Console.WriteLine("Insufficient funds");
            }
            
        }
        
    }


}

