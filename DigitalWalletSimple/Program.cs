using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWalletSimple
{
    class Program
    {
        private String AccountPassword = "";
        public String input = "";

        public static void Main(String[] args)
        {
            Program program = new Program();
            program.Operation();
        }
        public void Operation()
        {
            int i = 0;
            DigitalWallet digitalWallet = new DigitalWallet();
            Console.WriteLine("Welcome to PayBud :-)");
            Label2:
            
            Console.WriteLine("Enter your acccount Password : ");
            this.AccountPassword = Console.ReadLine().ToString();
            if (String.IsNullOrEmpty(AccountPassword))
            {
                Console.WriteLine("You haven't entered any value. Enter the correct password");
                goto Label2; 
            }      

            if (digitalWallet.ValidatePassword(AccountPassword))
            {
                digitalWallet.displayAccountInformation();
                Lable1:
                Console.WriteLine("Choose any option from below: ");
                Console.WriteLine("1. Check Balance    2. Add Money     3. Send money");
                input = Console.ReadLine();

                if (input == "1" | input == "2" | input == "3")
                {
                    switch (input)
                    {
                        case "1": //check balance
                            Console.WriteLine("Enter the account pin. ");
                            string pin = Console.ReadLine();
                            digitalWallet.CheckBalance(pin);
                            break;

                        case "2": //Add money
                                Console.WriteLine("Enter the account pin. ");
                                string pin2= Console.ReadLine();
                                if (digitalWallet.ValidateTransactionPIN(pin2))
                                {
                                    Console.WriteLine("Enter the amount to be added..");
                                    String deposit = Console.ReadLine();
                                    double Dep= double.Parse(deposit);
                                    digitalWallet.AddMoney(Dep);
                                }
                                break;

                        case "3": // Withdraw
                            Console.WriteLine("Enter account pin.");
                            string pin3 = Console.ReadLine();
                            if (digitalWallet.ValidateTransactionPIN(pin3))
                            {
                                Console.WriteLine("Enter the amount to be withdrwan..");
                                String withdrwan = Console.ReadLine().ToString();
                                double Wd = double.Parse(withdrwan);
                                digitalWallet.AddMoney(Wd);
                            }
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Wrong input entered");
                    goto Lable1;

                }

            }
            else
            {
                Console.WriteLine("Wrong password entered !");
                i += 1;
                if (i > 2)
                {
                    Console.WriteLine("You have entered wrong password 3 times ! Logging you out ...");
                }
                else
                    goto Label2;

            }

        }
    }
}
