using System;

namespace assingment.entity
{
    public class SHBTransaction
    {
        public string TransactionId { get; set; }
        public TransactionType Type { get; set; } // 1. withdraw | 2. deposit | 3. transfer
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public long CreatedAtMLS { get; set; }
        public long UpdatedAtMLS { get; set; }
        public TransactionStatus Status { get; set; }
        public enum TransactionType
        {
            WITHDRAW = 1,
            DEPOSIT = 2, 
            TRANSFER = 3
        }
        public enum TransactionStatus
        {
            DONE = 1,
            PROTECTED = 2,
            DELETED = 3
        }
        public SHBTransaction()
        {
        }

        public SHBTransaction(string transactionId, TransactionType type, string senderAccountNumber, string receiverAccountNumber, decimal amount, string message, long createdAtMls, long updatedAtMls, TransactionStatus status)
        {
            TransactionId = Guid.NewGuid().ToString();
            Type = type;
            SenderAccountNumber = senderAccountNumber;
            ReceiverAccountNumber = receiverAccountNumber;
            Amount = amount;
            Message = message;
            CreatedAtMLS = createdAtMls;
            UpdatedAtMLS = updatedAtMls;
            Status = status;
        }
    }
}