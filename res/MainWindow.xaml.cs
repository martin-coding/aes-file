using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using Microsoft.Win32;

namespace AESFILE
{
    public partial class MainWindow : Window
    {
        private string filePath;
        private string key;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                pathBox.Text = openFileDialog.FileName;
            }
        }

        private void Encrypt(object sender, RoutedEventArgs e)
        {
            if (pathBox.Text != string.Empty)
            {
                if (pwBox.Password != string.Empty)
                {
                    filePath = pathBox.Text;
                    key = pwBox.Password;
                    EncryptFile(filePath, key);
                }
                else
                {
                    MessageBox.Show("Please set your password", "ERROR - missing");
                }
            }
            else
            {
                MessageBox.Show("Please specify your file path", "ERROR - missing");
            }
        }

        private void Decrypt(object sender, RoutedEventArgs e)
        {
            if (pathBox.Text != string.Empty)
            {
                if (pwBox.Password != string.Empty)
                {
                    filePath = pathBox.Text;
                    key = pwBox.Password;
                    DecryptFile(filePath, key);
                }
                else
                {
                    MessageBox.Show("Enter your password", "ERROR - missing");
                }
            }
            else
            {
                MessageBox.Show("Choose a file", "ERROR - missing");
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            pathBox.Text = "";
            pwBox.Password = "";
        }

        static byte[] Pw(string value)
        {
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] data = sha256.ComputeHash(ascii.GetBytes(value));
                return data;
            }
        }

        static void EncryptFile(string filePath, string key)
        {
            Random rnd = new Random();

            byte[] iv = new Byte[16];
            rnd.NextBytes(iv);

            byte[] plainContent = File.ReadAllBytes(filePath);

            using (AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider())
            {
                cryptoProvider.BlockSize = 128;
                cryptoProvider.KeySize = 256;
                cryptoProvider.IV = iv;
                cryptoProvider.Key = Pw(key);
                cryptoProvider.Mode = CipherMode.CFB;
                cryptoProvider.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, cryptoProvider.CreateEncryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(plainContent, 0, plainContent.Length);
                    cryptoStream.FlushFinalBlock();
                    File.WriteAllBytes(filePath, iv.Concat(memStream.ToArray()).ToArray());
                    MessageBox.Show("File successfully encrypted!", "Done");
                }
            }
        }

        static void DecryptFile(string filePath, string key)
        {
            byte[] data = File.ReadAllBytes(filePath);
            byte[] iv = data.Take(16).ToArray();
            byte[] encrypted = data.Skip(16).ToArray();

            using (AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider())
            {
                cryptoProvider.BlockSize = 128;
                cryptoProvider.KeySize = 256;
                cryptoProvider.IV = iv;
                cryptoProvider.Key = Pw(key);
                cryptoProvider.Mode = CipherMode.CFB;
                cryptoProvider.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, cryptoProvider.CreateDecryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(encrypted, 0, encrypted.Length);
                    cryptoStream.FlushFinalBlock();
                    File.WriteAllBytes(filePath, memStream.ToArray());
                    MessageBox.Show("File successfully decrypted!", "Done");
                }
            }
        }
    }
}
