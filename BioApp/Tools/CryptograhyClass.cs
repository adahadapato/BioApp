

namespace BioApp
{
    using System;
    using System.Text;
    using System.Security.Cryptography;
    using System.IO;

    public class CryptograhyClass
    {
        // Encrypt a file into another file using a password 

        public static readonly string EncryptionPWD = "N3c055cE2018B10m3trics";

        //public static readonly string EncryptionPWD = "N3c055cE2018B10m3trics";
        public static void Encrypt(string fileIn, string fileOut, string Password)
        {
            try
            {
                // First we are going to open the file streams 

                FileStream fsIn = new FileStream(fileIn,
                    FileMode.Open, FileAccess.Read);
                FileStream fsOut = new FileStream(fileOut,
                    FileMode.OpenOrCreate, FileAccess.Write);

                // Then we are going to derive a Key and an IV from the
                // Password and create an algorithm 

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                    new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
                    0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

                Rijndael alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);

                // Now create a crypto stream through which we are going
                // to be pumping data. 
                // Our fileOut is going to be receiving the encrypted bytes. 

                CryptoStream cs = new CryptoStream(fsOut,
                    alg.CreateEncryptor(), CryptoStreamMode.Write);

                // Now will will initialize a buffer and will be processing
                // the input file in chunks. 
                // This is done to avoid reading the whole file (which can
                // be huge) into memory. 

                int bufferLen = 4096;
                byte[] buffer = new byte[bufferLen];
                int bytesRead;

                do
                {
                    // read a chunk of data from the input file 
                    bytesRead = fsIn.Read(buffer, 0, bufferLen);
                    // encrypt it 
                    cs.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);

                // close everything 
                // this will also close the unrelying fsOut stream

                cs.Close();
                fsIn.Close();

                File.Delete(fileIn);
                File.Move(fileOut, fileIn);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Encrypt");
            }
        }


        public static void FEncrypt(string fileIn, string fileOut, string Password)
        {

            // First we are going to open the file streams 

            FileStream fsIn = new FileStream(fileIn,
                FileMode.Open, FileAccess.Read);
            FileStream fsOut = new FileStream(fileOut,
                FileMode.OpenOrCreate, FileAccess.Write);

            // Then we are going to derive a Key and an IV from the
            // Password and create an algorithm 

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
                    0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            Rijndael alg = Rijndael.Create();
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            // Now create a crypto stream through which we are going
            // to be pumping data. 
            // Our fileOut is going to be receiving the encrypted bytes. 

            CryptoStream cs = new CryptoStream(fsOut,
                alg.CreateEncryptor(), CryptoStreamMode.Write);

            // Now will will initialize a buffer and will be processing
            // the input file in chunks. 
            // This is done to avoid reading the whole file (which can
            // be huge) into memory. 

            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file 
                bytesRead = fsIn.Read(buffer, 0, bufferLen);
                // encrypt it 
                cs.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);

            // close everything 
            // this will also close the unrelying fsOut stream

            cs.Close();
            fsIn.Close();

            //File.Delete(fileIn);
            //File.Move(fileOut, fileIn);
        }



        // Decrypt a file into another file using a password 
        public static string Decrypt(string fileIn, string fileOut, string Password)
        {
            try
            {
                // First we are going to open the file streams 
                FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
                FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);

                // Then we are going to derive a Key and an IV from
                // the Password and create an algorithm 

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                    new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                Rijndael alg = Rijndael.Create();

                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);

                // Now create a crypto stream through which we are going
                // to be pumping data. 
                // Our fileOut is going to be receiving the Decrypted bytes. 

                CryptoStream cs = new CryptoStream(fsOut,
                    alg.CreateDecryptor(), CryptoStreamMode.Write);

                // Now will will initialize a buffer and will be 
                // processing the input file in chunks. 
                // This is done to avoid reading the whole file (which can be
                // huge) into memory. 

                int bufferLen = 4096;
                byte[] buffer = new byte[bufferLen];
                int bytesRead;

                do
                {
                    // read a chunk of data from the input file 
                    bytesRead = fsIn.Read(buffer, 0, bufferLen);
                    // Decrypt it 
                    cs.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);

                // close everything 
                cs.Close(); // this will also close the unrelying fsOut stream 
                fsIn.Close();

                return fileOut;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Decrypt");
            }
            return string.Empty;
        }
    }
}
