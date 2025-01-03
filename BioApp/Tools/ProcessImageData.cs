using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace BioApp.Tools
{
    /// <summary>
    /// FxCop requires all Marshalled functions to be in a class called NativeMethods.
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);
    }
    public static class ProcessImageData
    {

        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(System.Drawing.Image source)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(source);
            var bitSrc = bitmap.ToBitmapSource();
            bitmap.Dispose();
            bitmap = null;
            return bitSrc;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="source">The source bitmap.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;
            var hBitmap = source.GetHbitmap();
            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        public static String byteToString(byte[] imageToEmbed)//byteToString
        {
            // create a new binary formatter
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // create a memory stream.  This is where the data will be stored
            MemoryStream retrievedStream = new MemoryStream();
            // seralize the data from the image to the memory stream 
            binaryFormatter.Serialize(retrievedStream, imageToEmbed);
            // return a string in base64 from the memorystream
            return Convert.ToBase64String(retrievedStream.ToArray());
        }

        public static byte[] StringToImage(String inputString)
        {
            // create an memorystream derived from base64 stream to byte array
            MemoryStream ourImageData = new MemoryStream(Convert.FromBase64String(inputString));
            // create a binary formatter to deserialze the memorystream
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // create an image object to return on deserialized memory stream
            byte[] ourRecoveredImage = (byte[])binaryFormatter.Deserialize(ourImageData);
            // return image
            return ourRecoveredImage;
        }
        public static System.Drawing.Image stringToImage(String inputString)
        {
            // create an memorystream derived from base64 stream to byte array
            MemoryStream ourImageData = new MemoryStream(Convert.FromBase64String(inputString));
            // create a binary formatter to deserialze the memorystream
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // create an image object to return on deserialized memory stream
            System.Drawing.Image ourRecoveredImage = (System.Drawing.Image)binaryFormatter.Deserialize(ourImageData);
            // return image
            return ourRecoveredImage;
        }

        public static BitmapImage ConvertByteArrayToBitmapImage(Byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static byte[] ConvertBitmapSourceToByteArray(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        public static byte[] ConvertBitmapSourceToByteArray(BitmapSource image)
        {
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static byte[] ConvertBitmapSourceToByteArray(Uri uri)
        {
            var image = new BitmapImage(uri);
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
        public static byte[] ConvertBitmapSourceToByteArray(string filepath)
        {
            var image = new BitmapImage(new Uri(filepath));
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static byte[] ConvertBitmapSourceToByteArray(ImageSource imageSource)
        {
            var image = imageSource as BitmapSource;
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static byte[] StringToByte(string inputString)
        {
            byte[] ourImageBytes = (byte[])Convert.FromBase64String(inputString);
            // return byte[]
            return ourImageBytes;
        }


        // ImageConverter object used to convert byte arrays containing JPEG or PNG file images into 
        //  Bitmap objects. This is static and only gets instantiated once.
        private static readonly System.Drawing.ImageConverter _imageConverter = new System.Drawing.ImageConverter();

        /// <summary>
        /// Method that uses the ImageConverter object in .Net Framework to convert a byte array, 
        /// presumably containing a JPEG or PNG file image, into a Bitmap object, which can also be 
        /// used as an Image object.
        /// </summary>
        /// <param name="byteArray">byte array containing JPEG or PNG file image or similar</param>
        /// <returns>Bitmap object if it works, else exception is thrown</returns>
        public static System.Drawing.Image GetImageFromByteArray(byte[] byteArray)
        {
            System.Drawing.Bitmap bm = (System.Drawing.Bitmap)_imageConverter.ConvertFrom(byteArray);

            if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution ||
                               bm.VerticalResolution != (int)bm.VerticalResolution))
            {

                bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                                 (int)(bm.VerticalResolution + 0.5f));
            }
            bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                                (int)(bm.VerticalResolution + 0.5f));
            return bm;
        }

        /// <summary>
        /// Method to "convert" an Image object into a byte array, formatted in PNG file format, which 
        /// provides lossless compression. This can be used together with the GetImageFromByteArray() 
        /// method to provide a kind of serialization / deserialization. 
        /// </summary>
        /// <param name="theImage">Image object, must be convertable to PNG format</param>
        /// <returns>byte array image of a PNG file containing the image</returns>
        public static byte[] CopyImageToByteArray(System.Drawing.Image theImage)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    theImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        public static byte[] ConvertFingerPrintsToByte(ref System.Windows.Forms.PictureBox pictBox)
        {
            MemoryStream ms = new MemoryStream();
            pictBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] photo_aray = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(photo_aray, 0, photo_aray.Length);
            return photo_aray;
        }


        public static String imageToString(System.Drawing.Image imageToEmbed)
        {
            // create a new binary formatter
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // create a memory stream.  This is where the data will be stored
            MemoryStream retrievedStream = new MemoryStream();
            // seralize the data from the image to the memory stream 
            binaryFormatter.Serialize(retrievedStream, imageToEmbed);
            // return a string in base64 from the memorystream
            return Convert.ToBase64String(retrievedStream.ToArray());
        }


        public static BitmapImage ToWpfImage(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage ix = new BitmapImage();
            try
            {

                ix.BeginInit();
                ix.CacheOption = BitmapCacheOption.OnLoad;
                ix.StreamSource = ms;
                ix.EndInit();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                var methodName = new StackTrace(ex).GetFrame(0).GetMethod().Name;
               // Errorlog.Errorlog.WriteToErrorLog(ex.Message, ex.StackTrace, "Error", methodName);
            }
            return ix;

        }
    }
    /*
.Net Solutions

Here Find All .NET Related Samples
Wednesday, 20 July 2016
Convert Winforms Image to WPF
  
Posted by govindu sai at 09:58 Email ThisBlogThis!Share to TwitterShare to FacebookShare to Pinterest
No comments:

Post a Comment

Newer Post Older Post Home
Subscribe to: Post Comments (Atom)
*/

   
}
