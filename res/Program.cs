using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace AESFILE
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://www.youtube.com/watch?v=Xna-5XBXck8

            string filePath;
            string input;
            string key;

            Console.WriteLine(" __  __            _   _             ______                             _             ");
            Console.WriteLine("|  \\/  |          | | (_)           |  ____|                           | |            ");
            Console.WriteLine("| \\  / | __ _ _ __| |_ _ _ __  ___  | |__   _ __   ___ _ __ _   _ _ __ | |_ ___  _ __ ");
            Console.WriteLine("| |\\/| |/ _` | '__| __| | '_ \\/ __| |  __| | '_ \\ / __| '__| | | | '_ \\| __/ _ \\| '__|");
            Console.WriteLine("| |  | | (_| | |  | |_| | | | \\__ \\ | |____| | | | (__| |  | |_| | |_) | || (_) | |   ");
            Console.WriteLine("|_|  |_|\\__,_|_|   \\__|_|_| |_|___/ |______|_| |_|\\___|_|   \\__, | .__/ \\__\\___/|_|   ");
            Console.WriteLine("                                                             __/ | |                  ");
            Console.WriteLine("                                                            |___/|_|                  ");
            Console.WriteLine(" ");
            Console.WriteLine("Info: version 1.0.0");
            Console.WriteLine("Info: Do not abuse this software!");
            Console.WriteLine("Info: Use good passwords (you can use my password generator software)");
            Console.WriteLine("Info: For more Information check out: http://github.com/martin-coding/aes-file");
            Console.WriteLine(" ");

            while (true)
            {
                Console.WriteLine("a) Encrypt");
                Console.WriteLine("b) Decrypt");
                Console.WriteLine("c) Exit");

                input = Console.ReadLine();
                if (input == "c")
                    break;
                else
                {
                    if (input == "a")
                    {
                        Console.Write("Enter file path: ");
                        filePath = Console.ReadLine();
                        Console.Write("Choose password: ");
                        key = Console.ReadLine();
                        EncryptFile(filePath, key);
                    }
                    else if (input == "b")
                    {
                        Console.Write("Enter file path: ");
                        filePath = Console.ReadLine();
                        Console.Write("Enter password: ");
                        key = Console.ReadLine();
                        DecryptFile(filePath, key);
                    }
                        
                }
            }

            static byte[] pw(string value)
            {
                using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
                {
                    ASCIIEncoding ascii = new ASCIIEncoding();
                    byte[] data = sha256.ComputeHash(ascii.GetBytes(value));
                    return data;
                }
            }
            static byte[] iv(string value)
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    ASCIIEncoding ascii = new ASCIIEncoding();
                    byte[] data = md5.ComputeHash(ascii.GetBytes(value));
                    return data;
                }
            }

            static void EncryptFile(string filePath, string key)
            {
                byte[] plainContent = File.ReadAllBytes(filePath);
                using (AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider())
                {
                    cryptoProvider.BlockSize = 128;
                    cryptoProvider.KeySize = 256;
                    cryptoProvider.IV = iv(key);
                    cryptoProvider.Key = pw(key);
                    cryptoProvider.Mode = CipherMode.CFB;
                    cryptoProvider.Padding = PaddingMode.PKCS7;

                    using (var memStream = new MemoryStream())
                    {
                        CryptoStream cryptoStream = new CryptoStream(memStream, cryptoProvider.CreateEncryptor(),
                            CryptoStreamMode.Write);

                        cryptoStream.Write(plainContent, 0, plainContent.Length);
                        cryptoStream.FlushFinalBlock();
                        File.WriteAllBytes(filePath, memStream.ToArray());
                        Console.WriteLine("Encrypted successfully: " + filePath);
                    }
                }
            }

            static void DecryptFile(string filePath, string key)
            {
                byte[] encrypted = File.ReadAllBytes(filePath);
                using (AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider())
                {
                    cryptoProvider.BlockSize = 128;
                    cryptoProvider.KeySize = 256;
                    cryptoProvider.IV = iv(key);
                    cryptoProvider.Key = pw(key);
                    cryptoProvider.Mode = CipherMode.CFB;
                    cryptoProvider.Padding = PaddingMode.PKCS7;

                    using (var memStream = new MemoryStream())
                    {
                        CryptoStream cryptoStream = new CryptoStream(memStream, cryptoProvider.CreateDecryptor(),
                            CryptoStreamMode.Write);

                        cryptoStream.Write(encrypted, 0, encrypted.Length);
                        cryptoStream.FlushFinalBlock();
                        File.WriteAllBytes(filePath, memStream.ToArray());
                        Console.WriteLine("Decrypted successfully: " + filePath);
                    }
                }
            }
        }
    }
}
