using System;
using assingment.entity;
using assingment.model;

namespace assingment.demo
{
    public class GiaoDichSHB : GiaoDich
    {
        private static SHBAccountModel shbAccountModel;

        public GiaoDichSHB()
        {
            shbAccountModel = new SHBAccountModel();
        }

        public void Login()
        {
            Program.currentLoggedInAccount = null;
            Console.Clear();
            Console.WriteLine("Tiến hành đăng nhập hệ thống SHB.");
            Console.WriteLine("Vui lòng nhập usename: ");
            var username = Console.ReadLine();
            Console.WriteLine("Vui lòng nhập mật khẩu: ");
            var password = Console.ReadLine();
            var shbAccount = shbAccountModel.FindByUsernameAndPassword(username, password);
            if (shbAccount == null)
            {
                Console.WriteLine("Sai thông tin tài khoản, vui lòng đăng nhập lại.");
                return;
            }
            Program.currentLoggedInAccount = shbAccount;
        }

        public void RutTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành rút tiền tại hệ thống SHB.");
                Console.WriteLine("Vui lòng nhập số tiền cần rút.");
                var amount = decimal.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.Username,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.Username,
                    Type = SHBTransaction.TransactionType.WITHDRAW,
                    Message = "Tiến hành rút tiền tại ATM với số tiền: " + amount,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE
                };
                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
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

        public void GuiTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành gửi tiền tại hệ thống SHB.");
                Console.WriteLine("Vui lòng nhập số tiền cần gửi.");
                var amount = decimal.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.Username,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.Username,
                    Type = SHBTransaction.TransactionType.DEPOSIT,
                    Message = "Tien hanh gui tien voi so tien la: " + amount,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE
                };
                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
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
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành gửi tiền tại hệ thống SHB.");
                Console.WriteLine("Vui lòng nhập số tiền cần gửi.");
                var amount = decimal.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                Console.WriteLine("Nhap username nguoi nhan");
                var receiver = Console.ReadLine();
                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.Username,
                    ReceiverAccountNumber = receiver,
                    Type = SHBTransaction.TransactionType.TRANSFER,
                    Message = "Tien hanh gui tien voi so tien la: " + amount,
                    Amount = amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE
                };
                bool result = shbAccountModel.Transfer(Program.currentLoggedInAccount, transaction);
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