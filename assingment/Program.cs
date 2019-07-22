using System;
using System.Collections.Generic;
using assingment.demo;
using assingment.entity;
using assingment.model;
using MySql.Data.MySqlClient;

namespace assingment
{
    class Program
    {
        public static SHBAccount currentLoggedInAccount;
        public static BlockchainAddress currentLoggedInAddress;

        static void Main(string[] args)
        {
            while (true)
            {
                GiaoDich giaoDich = null;
                Console.WriteLine("Vui lòng lựa chọn phương thức giao dịch: ");
                Console.WriteLine("============================================");
                Console.WriteLine("1. Giao dịch trên ngân hàng SHB - Spring Hero Bank.");
                Console.WriteLine("2. Giao dịch blockchain.");
                Console.WriteLine("============================================");
                Console.WriteLine("Nhập lựa chọn của bạn: ");
                var choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        giaoDich = new GiaoDichSHB();
                        break;
                    case 2:
                        giaoDich = new GiaoDichBlockchain();
                        break;
                    default:
                        Console.WriteLine("Sai phương thức đăng nhập.");
                        break;
                }
                
                giaoDich.Login();
                if (currentLoggedInAccount != null)
                {
                    Console.WriteLine("Đăng nhập thành công với tài khoản.");
                    Console.WriteLine($"Tài khoản: {currentLoggedInAccount.Username}");
                    Console.WriteLine($"Số dư: {currentLoggedInAccount.Balance}");
                    GenerateTransactionMenu(giaoDich);
                }
                else if (currentLoggedInAddress != null)
                {
                    Console.WriteLine("Dang nhap thanh cong voi address.");
                    Console.WriteLine($"Tài khoản: {currentLoggedInAddress.Address}");
                    Console.WriteLine($"Số dư: {currentLoggedInAddress.Balance}");
                    GenerateTransactionMenu(giaoDich);
                }
            }
        }

        private static void GenerateTransactionMenu(GiaoDich giaoDich)
        {
            while (true)
            {
                
                Console.WriteLine("Vui lòng lựa chọn kiểu giao dịch: ");
                Console.WriteLine("============================================");
                Console.WriteLine("1. Rút tiền.");
                Console.WriteLine("2. Gửi tiền.");
                Console.WriteLine("3. Chuyển khoản.");
                Console.WriteLine("4. Thoát.");
                Console.WriteLine("============================================");
                Console.WriteLine("Nhập lựa chọn của bạn: ");
                var choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        giaoDich.RutTien();
                        break;
                    case 2:
                        giaoDich.GuiTien();
                        break;
                    case 3:
                        giaoDich.ChuyenKhoan();
                        break;
                    case 4:
                        Console.WriteLine("Thoát giao diện giao dịch.");
                        break;
                    default:
                        Console.WriteLine("Lựa chọn sai.");
                        break;
                }

                if (choice == 4)
                {
                    break;
                }
            }
        }
    }

    internal interface GiaoDich
    {
        void RutTien();
        void GuiTien();
        void ChuyenKhoan();
        void Login();
    }
}