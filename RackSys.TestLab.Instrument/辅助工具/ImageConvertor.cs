using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// Image格式化器，将字节序列的数据转变为Image对象
    /// </summary>
    public class ImageBytesConvertor
    {
        public static Image ConvertByteToImg(byte[] imgData)
        {
            MemoryStream ms = new MemoryStream(imgData);

            return Image.FromStream(ms);

            //ImageConverter converter = new ImageConverter();
            //byte[] imgArray = (byte[])converter.ConvertTo(imageIn, typeof(byte[]));
        }

        public static byte[] ConvertImgToByte(Image inImage)
        {
            ImageConverter tmpImageConverter = new ImageConverter();
            byte[] imgArray = (byte[])tmpImageConverter.ConvertTo(inImage, typeof(byte[]));
            return imgArray;

        }
    }
}
