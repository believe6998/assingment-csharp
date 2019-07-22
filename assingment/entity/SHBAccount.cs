using System;

namespace assingment.entity
{
    public class SHBAccount
    {
        public string AccountNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public SHBAccount()
        {
            GenerateAccountNumber();
        }

        public SHBAccount(string accountNumber, string username, string password, decimal balance)
        {
            GenerateAccountNumber();
            AccountNumber = accountNumber;
            Username = username;
            Password = password;
            Balance = balance;
        }

        private void GenerateAccountNumber()
        {
            AccountNumber = Guid.NewGuid().ToString();
        }
    }
}