using System;
using assingment.entity;
using assingment.entity;
using MySql.Data.MySqlClient;

namespace assingment.model
{
    public class BlockchainAddressModel
    {
        public BlockchainAddress FindByAddressAndPrivateKey(string address, string privateKey)
        {
            var cmd = new MySqlCommand(
                "select * from blockchain_accounts where address = @address And privateKey = @privateKey",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@privateKey", privateKey);
            BlockchainAddress blockchainAddress = null;
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                blockchainAddress = new BlockchainAddress()
                {
                    PrivateKey = reader.GetString("privateKey"),
                    Address = reader.GetString("address"),
                    Balance = reader.GetDecimal("balance")
                };
            }

            ConnectionHelper.CloseConnection();
            return blockchainAddress;
        }

        public bool Withdraw(BlockchainAddress currentLoggedInAccount, BlockchainTransaction transaction)
        {
            ConnectionHelper.CloseConnection();
            var tran = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("select * from blockchain_accounts where address = @address",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@address", currentLoggedInAccount.Address);
                BlockchainAddress address = null;
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

                if (currentAccountBalance < transaction.Amount)
                {
                    Console.WriteLine("Khong du tien thuc hien giao dich");
                    return false;
                }

                currentAccountBalance -= transaction.Amount;

                var updateQuery =
                    "update `blockchain_accounts` set `balance` = @balance where address = @address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAccount.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();
                var historyTransactionQuery =
                    "insert into `blockchain_transactions` (id, senderAddress, receiverAddress, amount) " +
                    "values (@id, @senderAccountNumber, @receiverAccountNumber, @amount)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@id", transaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transaction.ReceiverAddress);
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


        public bool Deposit(BlockchainAddress currentLoggedInAccount, BlockchainTransaction transaction)
        {
            ConnectionHelper.CloseConnection();
            var tran = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("select * from blockchain_accounts where address = @address",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@address", currentLoggedInAccount.Address);
                BlockchainAddress address = null;
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

                currentAccountBalance += transaction.Amount;

                var updateQuery =
                    "update `blockchain_accounts` set `balance` = @balance where address = @address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAccount.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();
                var historyTransactionQuery =
                    "insert into `blockchain_transactions` (id, senderAddress, receiverAddress, amount) " +
                    "values (@id, @senderAccountNumber, @receiverAccountNumber, @amount)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@id", transaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transaction.ReceiverAddress);
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

        public bool Transfer(BlockchainAddress currentLoggedInAccount, BlockchainTransaction transactionHistory)
        {
            ConnectionHelper.CloseConnection();
            var mySqlTransaction = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var selectBalance =
                    "select balance from `blockchain_accounts` where address = @address";
                var cmdSelect = new MySqlCommand(selectBalance, ConnectionHelper.GetConnection());
                cmdSelect.Parameters.AddWithValue("@address", currentLoggedInAccount.Address);
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
                    "update `blockchain_accounts` set `balance` = @balance where address = @address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAccount.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();
                var selectBalanceReceiver =
                    "select balance from `blockchain_accounts` where address = @address";
                var cmdSelectReceiver = new MySqlCommand(selectBalanceReceiver, ConnectionHelper.GetConnection());
                cmdSelectReceiver.Parameters.AddWithValue("@address", transactionHistory.ReceiverAddress);
                var readerReceiver = cmdSelectReceiver.ExecuteReader();
                decimal receiverBalance = 0;
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDecimal("balance");
                }

                readerReceiver.Close();
                receiverBalance += transactionHistory.Amount;
                var updateQueryReceiver =
                    "update `blockchain_accounts` set `balance` = @balance where address = @address";
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, ConnectionHelper.GetConnection());
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@address", transactionHistory.ReceiverAddress);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();
                var historyTransactionQuery =
                    "insert into `blockchain_transactions` (id, senderAddress, receiverAddress, amount) " +
                    "values (@id, @senderAccountNumber, @receiverAccountNumber, @amount)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@amount", transactionHistory.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@id", transactionHistory.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transactionHistory.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transactionHistory.ReceiverAddress);
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