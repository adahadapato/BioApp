using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace BioApp.Tools
{
    public class GZipClassLayer
    {
        public static string DeCompress(string compressedString)
        {
            StringBuilder uncompressedString = new StringBuilder();
            int totalLength = 0;
            byte[] bytInput = Convert.FromBase64String(compressedString); 
            byte[] writeData = new byte[4096];
            Stream s2 = new InflaterInputStream(new MemoryStream(bytInput));
            while (true)
            {
                int size = s2.Read(writeData, 0, writeData.Length);
                if (size > 0)
                {
                    totalLength += size;
                    uncompressedString.Append(Encoding.UTF8.GetString(writeData, 0, size));
                }
                else
                {
                    break;
                }
            }
            s2.Close();
            return uncompressedString.ToString();
        }

        public static string Compress(string uncompressedString)
        {
            byte[] bytData = System.Text.Encoding.UTF8.GetBytes(uncompressedString);
            MemoryStream ms = new MemoryStream();
            Stream s = new DeflaterOutputStream(ms);
            s.Write(bytData, 0, bytData.Length);
            s.Close();
            byte[] compressedData = (byte[])ms.ToArray();
            return System.Convert.ToBase64String(compressedData, 0, compressedData.Length);
        }

        public static string DecompressToBase64(string gzipInputString)
        {
            try
            {
                byte[] compressed = Convert.FromBase64String(gzipInputString);

                MemoryStream inputStream = new MemoryStream(compressed);
                MemoryStream outputStream = new MemoryStream(10024);
                using (var csStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inputStream))
                {
                    byte[] buffer = new byte[10024];
                    int nRead;
                    while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, nRead);
                    }

                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured: " + e.Message);
                return null;
            }
        }


        public static string CompressByteToGzipBase64(byte[] input)
        {
            try
            {
                var outputStream = new MemoryStream();
                var csStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(outputStream);
                csStream.Write(input, 0, input.Length);
                csStream.Close();
                string base64Image = Convert.ToBase64String(outputStream.ToArray());
                outputStream.Close();
                csStream = null;
                outputStream = null;

                return base64Image;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured: " + e.Message);
                return null;
            }
        }


        public static string CompressByteToGzipBase64(String fromBase64Image)
        {
            try
            {
                var outputStream = new MemoryStream();
                byte[] input = Convert.FromBase64String(fromBase64Image);

                var csStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(outputStream);
                csStream.Write(input, 0, input.Length);
                csStream.Close();
                string base64Image = Convert.ToBase64String(outputStream.ToArray());

                outputStream.Close();
                csStream = null;
                outputStream = null;

                return base64Image;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return string.Empty;
        }
    }
}
