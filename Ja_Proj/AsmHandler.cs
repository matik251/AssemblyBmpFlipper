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
    unsafe public class AsmHandler
    {

        private const string PathToAsm = @"C:\Users\Dell\source\repos\Troy\x64\Debug\Ja_ASM.dll";

        [DllImport(PathToAsm)]

        static extern int GetIntAsm();

        public AsmHandler()
        {
            //todo multithread use of asm
        }

        public void Asm()
        {
            var temp = GetIntAsm();
            MessageBox.Show("Wynik asm: " + temp);
        }

        [DllImport(PathToAsm)]

        static extern unsafe byte* GetValueAsm(byte* bmp, byte* resbmp, int length);


        unsafe public byte[] FlipArray(byte[] part, int length)
        {
            byte[] result = new byte[length];
            byte[] resbmp = new byte[2* length];
            fixed (byte* arr = &part[0], resarr = &resbmp[0])
            {

                var templen = length/3 -1;
                GetValueAsm(arr, resarr, templen);

                for (int i = 3; i < (length +3) ; i++)
                {
                    result[i - 3] = resarr[i];
                }
            }
            return result;
        }

        public byte[] FlipArray(byte[] imgArr, int width, int height)
        {
            int quattroAlign = 0;
            if (width % 4 != 0)
            {
                quattroAlign = (width % 4);
            }

            var currStart = 0;
            byte[] temp = new byte[(width + 5 ) * 3];
            byte[] result = new byte[imgArr.Length + (quattroAlign * height)];

            for (int i = 0; i < height; i++)
            {
                if (currStart + (width * 3) <= imgArr.Length)
                {
                    Array.Copy(imgArr, currStart, temp, 0, (width * 3));
                    temp = FlipArray(temp, temp.Length);
                    Array.Copy(temp, 0, result, currStart, (width * 3));
                    currStart += (width * 3);
                    currStart += quattroAlign;
                }
            }
            return result;
        }

        public byte[] AsmFlipMultithread(Image img, int width, int height, int partCount)
        {
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

            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = partCount;

            Parallel.ForEach(imgBag, options, part => {
                resultBag.Add(new Tuple<int, byte[]>(part.Item1, FlipArray(part.Item2, width, _height)));
            });

            int j = 0;
            foreach (var res in resultBag)
            {
                Array.Copy(res.Item2, 0, result, 54 + res.Item1 * segment, segment);
            }


            return result;
        }
        public byte[] AsmFlipMultithread(Image img, int width, int height)
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
