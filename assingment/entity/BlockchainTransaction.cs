namespace assingment.entity
{
    public class BlockchainTransaction
    {
        public string TransactionId { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public decimal Amount { get; set; }
        public long CreatedAtMLS { get; set; }
        public long UpdatedAtMLS { get; set; }
        public int Status { get; set; }

        public BlockchainTransaction()
        {
        }

        public BlockchainTransaction(string senderAddress, string receiverAddress, decimal amount)
        {
            SenderAddress = senderAddress;
            ReceiverAddress = receiverAddress;
            Amount = amount;
        }
    }
}