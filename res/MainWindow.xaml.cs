using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Windows;
using Microsoft.Win32;

namespace AESFILE
{
    public partial class MainWindow : Window
    {
        string filePath;
        string key;
        string iv;

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
            if (pathBox.Text != "")
            {
                if (pwBox.Password != "" && ivBox.Password != "")
                {
                    filePath = pathBox.Text;
                    key = pwBox.Password;
                    iv = ivBox.Password;
                    EncryptFile(filePath, key, iv);
                }
                else
                {
                    MessageBox.Show("Please choose your key", "ERROR - missing");
                }
            }
            else
            {
                MessageBox.Show("Please specify your file path", "ERROR - missing");
            }
        }

        private void Decrypt(object sender, RoutedEventArgs e)
        {
            if (pathBox.Text != "")
            {
                if (pwBox.Password != "" && ivBox.Password != "")
                {
                    filePath = pathBox.Text;
                    key = pwBox.Password;
                    iv = ivBox.Password;
                    DecryptFile(filePath, key, iv);
                }
                else
                {
                    MessageBox.Show("Enter your key", "ERROR - missing");
                }
            }
            else
            {
                MessageBox.Show("Select your file", "ERROR - missing");
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            pathBox.Text = "";
            pwBox.Password = "";
            ivBox.Password = "";
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
        static byte[] Iv(string value)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] data = md5.ComputeHash(ascii.GetBytes(value));
                return data;
            }
        }

        static void EncryptFile(string filePath, string key, string iv)
        {
            byte[] plainContent = File.ReadAllBytes(filePath);
            using (AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider())
            {
                cryptoProvider.BlockSize = 128;
                cryptoProvider.KeySize = 256;
                cryptoProvider.IV = Iv(iv);
                cryptoProvider.Key = Pw(key);
                cryptoProvider.Mode = CipherMode.CFB;
                cryptoProvider.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, cryptoProvider.CreateEncryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(plainContent, 0, plainContent.Length);
                    cryptoStream.FlushFinalBlock();
                    File.WriteAllBytes(filePath, memStream.ToArray());
                    MessageBox.Show("File successfully encrypted!", "Done");
                }
            }
        }

        static void DecryptFile(string filePath, string key, string iv)
        {
            byte[] encrypted = File.ReadAllBytes(filePath);
            using (AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider())
            {
                cryptoProvider.BlockSize = 128;
                cryptoProvider.KeySize = 256;
                cryptoProvider.IV = Iv(iv);
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
