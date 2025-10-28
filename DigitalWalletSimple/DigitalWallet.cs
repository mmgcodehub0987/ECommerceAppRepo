using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWalletSimple
{
    internal class DigitalWallet
    {

        List<Double> TransactionLogs = new List<Double>();
        private double _balance =6588.25;
        private String AccountPassword = "Jalagara@123";
        private String TransactionPIN = "340977";
        public double Balance
        {
            get
            {
                return _balance;
            }
        }

        public void displayAccountInformation()
        {
            Console.WriteLine("Account Holder Name: Atharva Bhat");
            Console.WriteLine("Account Number : 56773432");
            Console.WriteLine("BANK DETAILS: DHFC Bank, Btm branch");
            Console.WriteLine("Branch Code : B0854BTM005");
        }

        public Boolean ValidatePassword(String Passsword)
        {
            if (Passsword == null)
            {
                return false;
            }
            else if (AccountPassword != Passsword)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void CheckBalance(String Pin)
        {
            if (ValidateTransactionPIN(Pin))
            {
                Console.WriteLine("Remainining Balance: "+this.Balance);

            }
        }

        public Boolean ValidateTransactionPIN(String Pin)
        {
            if (String.IsNullOrEmpty(Pin))
            {
                return false;
                throw new ArgumentException(" Pin Cannot be empty !");
            }
            else if (TransactionPIN != Pin)
            {
                return false;
                throw new ArgumentException(" Wrong PIN entered !");
            }
            else
                return true;
        }

        public void AddMoney(Double deposit)
        {
            if (deposit < 0)
            {
                Console.WriteLine(deposit + " was added to your account..");
                _balance += deposit;
                Console.WriteLine("You Balance is : " + Balance);
            } else 
                Console.WriteLine("Negetive Amount Cannot be added Added");
        }

        public void WithddrawMoney(Double Withdraw, String Pin)
        {
            if (Withdraw < 0 || (Withdraw < Balance))
            {
                Console.WriteLine(Withdraw + " was deducted from your account..");
                _balance -= Withdraw;
                Console.WriteLine("You Balance is : " + Balance);
            }
            else
                Console.WriteLine("Negetive Amount Cannot be added Added or Insufficient Balance");
        }
    }
}
