using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ja_Proj
{
    public class CHandler
    {

        private const string PathToC = @"C:\Users\Dell\source\repos\Troy\x64\Debug\Ja_C.dll";

        [DllImport(PathToC)]
        static extern int GetValueC();

        [DllImport(PathToC)]
        static extern unsafe byte* FlipArrayC(byte* arr, int length);

        public CHandler()
        {
            //todo mulitthread use of c


        }

        public void C()
        {
            int temp = 0;
            temp = GetValueC();

            MessageBox.Show("Wynik C: " + temp);
        }

        unsafe public void CFLipTest()
        {
            string str = string.Empty;
            byte[] array = { 128, 128, 128, 128, 255, 255, 255, 255, };
            byte[] result = new byte[8];
            var length = array.Length;
            fixed (byte* arr = &array[0])
            {
                var res = FlipArrayC(arr, length);

                for (int i = 0; i < length; i++)
                {
                    result[i] = res[i];
                }
            }
            var testResult = result;
        }

        public void CFLipTest2()
        {
            var utf8 = new UTF8Encoding();
            var input = "siszarp";
            byte[] pass = utf8.GetBytes(input);
            var result = FlipArray(pass, pass.Length);
            MessageBox.Show($"{input} <-> {utf8.GetString(result)}");
        }

        unsafe public byte[] FlipArray(byte[] part, int length)
        {
            byte[] result = new byte[length];
            fixed (byte* arr = &part[0])
            {
                var res = FlipArrayC(arr, length);

                for (int i = 3; i < length + 3; i++)
                {
                    result[i - 3] = res[i];
                }
            }
            return result;
        }

        public string CFlip(Image img, int width, int height)
        {
            int quattroAlign = 0;
            if (width % 4 != 0)
            {
                quattroAlign = (width % 4);
            }

            var imgArr = ImageHandler.ToByteArray(img, ImageFormat.Bmp);
            byte[] result = new byte[imgArr.Length + (quattroAlign * height)];
            byte[] temp = new byte[width * 3];
            var imgRawData = new byte[imgArr.Length];


            Array.Copy(imgArr, 0, result, 0, 54);
            Array.Copy(imgArr, 54, imgRawData, 0, imgArr.Length - 54);

            var currStart = 54;

            for (int i = 0; i < height; i++)
            {
                if (currStart + (width * 3) <= imgArr.Length)
                {
                    Array.Copy(imgArr, currStart, temp, 0, (width * 3));
                    temp = FlipArray(temp, (width * 3));
                    Array.Copy(temp, 0, result, currStart, (width * 3));
                    currStart += (width * 3);
                    currStart += quattroAlign;
                }
            }

            var name = ImageHandler.ByteArrayToBitmap(result);
            return name;
        }

        public byte[] FlipArray(byte[] imgArr, int width, int height)
        {
            int quattroAlign = 0;
            if (width % 4 != 0)
            {
                quattroAlign = (width % 4);
            }

            var currStart = 0;
            byte[] temp = new byte[width * 3];
            byte[] result = new byte[imgArr.Length + (quattroAlign * height)];

            for (int i = 0; i < height; i++)
            {
                if (currStart + (width * 3) <= imgArr.Length)
                {
                    Array.Copy(imgArr, currStart, temp, 0, (width * 3));
                    temp = FlipArray(temp, (width * 3));
                    Array.Copy(temp, 0, result, currStart, (width * 3));
                    currStart += (width * 3);
                    currStart += quattroAlign;
                }
            }
            return result;
        }

        public byte[] CFlipMultithread(Image img, int width, int height, int partCount)
        {
            var imgArr = ImageHandler.ToByteArray(img, ImageFormat.Bmp);
            var imgBag = new ConcurrentBag<Tuple<int,byte[]>>();

            int quattroAlign = 0;
            if (width % 4 != 0)
            {
                quattroAlign = (width % 4);
            }
            byte[] result = new byte[imgArr.Length + (quattroAlign * height)];
            Array.Copy(imgArr, 0, result, 0, 54);

            //if ((imgArr.Length - 54) % partCount == 0)

            var segment = ((width * 3) + quattroAlign); //(imgArr.Length -54) / partCount;

            for (int i = 0; i < height; i++)
            {
                var copyTemp = new byte[segment];
                Array.Copy(imgArr, i * segment, copyTemp, 0, segment);
                imgBag.Add(new Tuple<int, byte[]>(i,copyTemp));
            }
            
            var _height = height / partCount;
            var resultBag = new ConcurrentBag<Tuple<int, byte[]>>();

            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = partCount;

            Parallel.ForEach(imgBag, options, part =>{
                resultBag.Add(new Tuple<int, byte[]>(part.Item1, FlipArray(part.Item2, width, _height)));
            });

            int j = 0;
            foreach (var res in resultBag)
            {
                Array.Copy(res.Item2, 0, result, 54 +  res.Item1*segment, segment);
            }


            return result;
        }
        public byte[] CFlipMultithread(Image img, int width, int height)
        {
            var partCount = 10;
            var imgArr = ImageHandler.ToByteArray(img, ImageFormat.Bmp);
            var imgBag = new ConcurrentBag<Tuple<int, byte[]>>();

            int quattroAlign = 0;
            if (width % 4 != 0)
            {
                quattroAlign = (width % 4);
            }
            byte[] result = new byte[imgArr.Length + (quattroAlign * height)];
            Array.Copy(imgArr, 0, result, 0, 54);

            //if ((imgArr.Length - 54) % partCount == 0)

            var segment = ((width * 3) + quattroAlign); //(imgArr.Length -54) / partCount;

            for (int i = 0; i < height; i++)
            {
                var copyTemp = new byte[segment];
                Array.Copy(imgArr, i * segment, copyTemp, 0, segment);
                imgBag.Add(new Tuple<int, byte[]>(i, copyTemp));
            }

            var _height = height / partCount;
            var resultBag = new ConcurrentBag<Tuple<int, byte[]>>();
            Parallel.ForEach(imgBag, part => {
                resultBag.Add(new Tuple<int, byte[]>(part.Item1, FlipArray(part.Item2, width, _height)));
            });

            int j = 0;
            foreach (var res in resultBag)
            {
                Array.Copy(res.Item2, 0, result, 54 + res.Item1 * segment, segment);
            }

            return result;
        }
    }
}
