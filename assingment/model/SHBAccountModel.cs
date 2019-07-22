using System;
using System.Transactions;
using assingment.entity;
using MySql.Data.MySqlClient;

namespace assingment.model
{
    public class SHBAccountModel
    {
        public SHBAccount FindByUsernameAndPassword(string username, string password)
        {
            var cmd = new MySqlCommand("select * from accounts where username = @Username And password = @Password",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);
            SHBAccount shbAccount = null;
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                shbAccount = new SHBAccount
                {
                    Username = reader.GetString("username"),
                    Password = reader.GetString("password"),
                    Balance = reader.GetDecimal("balance")
                };
            }
            
            ConnectionHelper.CloseConnection();
            return shbAccount;
        }

        public bool UpdateBalance(SHBAccount currentLoggedInAccount, SHBTransaction transaction)
        {
            ConnectionHelper.CloseConnection();
            var tran = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("select * from accounts where username = @Username",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@Username", currentLoggedInAccount.Username);
                SHBAccount shbAccount = null;
                var reader = cmd.ExecuteReader();
                decimal currentAccountBalance = 0;

                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDecimal("balance");
                }

                reader.Close();
                if (currentAccountBalance < 0)
                {
                    Console.WriteLine("Không đủ tiền trong tài khoản.");
                    return false;
                }

                if (transaction.Type == SHBTransaction.TransactionType.WITHDRAW)
                {
                    if (currentAccountBalance < transaction.Amount)
                    {
                        Console.WriteLine("Khong du tien thuc hien giao dich");
                        return false;
                    }

                    currentAccountBalance -= transaction.Amount;
                }
                else if (transaction.Type == SHBTransaction.TransactionType.DEPOSIT)
                {
                    currentAccountBalance += transaction.Amount;
                }

                var updateQuery =
                    "update `accounts` set `balance` = @balance where username = @username";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@username", currentLoggedInAccount.Username);
                var updateResult = sqlCmd.ExecuteNonQuery();
                var historyTransactionQuery =
                    "insert into `transactions` (id, type, senderId, receiverId, amount, message) " +
                    "values (@id, @type, @senderAccountNumber, @receiverAccountNumber, @amount, @message)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@id", transaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", transaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", transaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transaction.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transaction.ReceiverAccountNumber);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                tran.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                tran.Rollback();
                return false;
            }

            ConnectionHelper.CloseConnection();
            return true;
        }

        public bool Transfer(SHBAccount currentLoggedInAccount, SHBTransaction transactionHistory)
        {
            ConnectionHelper.CloseConnection();
            var mySqlTransaction = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var selectBalance =
                    "select balance from `accounts` where username = @username";
                var cmdSelect = new MySqlCommand(selectBalance, ConnectionHelper.GetConnection());
                cmdSelect.Parameters.AddWithValue("@username", currentLoggedInAccount.Username);
                var reader = cmdSelect.ExecuteReader();
                decimal currentAccountBalance = 0;
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDecimal("balance");
                }

                reader.Close();
                if (currentAccountBalance < transactionHistory.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");
                }

                currentAccountBalance -= transactionHistory.Amount;
                var updateQuery =
                    "update `accounts` set `balance` = @balance where username = @username";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@username", currentLoggedInAccount.Username);
                var updateResult = sqlCmd.ExecuteNonQuery();
                var selectBalanceReceiver =
                    "select balance from `accounts` where username = @username";
                var cmdSelectReceiver = new MySqlCommand(selectBalanceReceiver, ConnectionHelper.GetConnection());
                cmdSelectReceiver.Parameters.AddWithValue("@username", transactionHistory.ReceiverAccountNumber);
                var readerReceiver = cmdSelectReceiver.ExecuteReader();
                decimal receiverBalance = 0;
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDecimal("balance");
                }

                readerReceiver.Close();
                receiverBalance += transactionHistory.Amount;
                var updateQueryReceiver =
                    "update `accounts` set `balance` = @balance where username = @username";
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, ConnectionHelper.GetConnection());
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@username", transactionHistory.ReceiverAccountNumber);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();
                var historyTransactionQuery =
                    "insert into `transactions` (id, type, senderId, receiverId, amount, message) " +
                    "values (@id, @type, @senderAccountNumber, @receiverAccountNumber, @amount, @message)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@id", transactionHistory.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transactionHistory.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", transactionHistory.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", transactionHistory.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transactionHistory.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transactionHistory.ReceiverAccountNumber);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1 || updateResultReceiver != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                mySqlTransaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                mySqlTransaction.Rollback();
                return false;
            }
            finally
            {
                ConnectionHelper.CloseConnection();
            }
        }
    }
}