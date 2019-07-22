using System;
using assingment.entity;
using assingment.model;

namespace assingment.demo
{
    public class GiaoDichBlockchain : GiaoDich
    {
        private static BlockchainAddressModel blockchainAccountModel = new BlockchainAddressModel();

        public void Login()
        {
            Program.currentLoggedInAddress = null;
            Console.Clear();
            Console.WriteLine("Tiến hành đăng nhập hệ thống SHB.");
            Console.WriteLine("Vui lòng nhập address: ");
            var address = Console.ReadLine();
            Console.WriteLine("Vui lòng nhập mật khẩu: ");
            var privatekey = Console.ReadLine();
            var blockchainAddress = blockchainAccountModel.FindByAddressAndPrivateKey(address, privatekey);
            if (blockchainAddress == null)
            {
                Console.WriteLine("Sai thông tin địa chỉ, vui lòng đăng nhập lại.");
                return;
            }
            
            Program.currentLoggedInAddress = blockchainAddress;
        }

        public void RutTien()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh rut tien tai blockchain.");
                Console.WriteLine("Nhap so tien can rut.");
                var amount = decimal.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("So luong khong hop le, vui long thu lai.");
                    return;
                }
                var transaction = new BlockchainTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                bool result = blockchainAccountModel.Withdraw(Program.currentLoggedInAddress, transaction);
                if (result)
                {
                    Console.WriteLine("Thanh cong");
                }
            }
            else
            {
                Console.WriteLine("Dang nhap de su dung chuc nang nay.");
            }
        }

        public void GuiTien()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh gui tien tai blockchain.");
                Console.WriteLine("Nhap so tien can gui.");
                var amount = decimal.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("So luong k hop le.");
                    return;
                }

                var transaction = new BlockchainTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                bool result = blockchainAccountModel.Deposit(Program.currentLoggedInAddress, transaction);
                if (result)
                {
                    Console.WriteLine("Thanh cong");
                }
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void ChuyenKhoan()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tien hanh chuyen tien tai blockchain");
                Console.WriteLine("Nhap so tien can chuyen.");
                var amount = decimal.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Số lưong khong hop le.");
                    return;
                }
                Console.WriteLine("Nhap address nguoi nhan");
                var receiver = Console.ReadLine();
                var transaction = new BlockchainTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = receiver,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                bool result = blockchainAccountModel.Transfer(Program.currentLoggedInAddress, transaction);
                if (result)
                {
                    Console.WriteLine("Thanh cong");
                }
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }
    }
}