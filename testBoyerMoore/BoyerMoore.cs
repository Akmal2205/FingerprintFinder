using System;
using System.Text;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class ImageProcessor
{
    public static string ProcessImage(string imagePath)
    {
        using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
        {
            image.Mutate(x => x.Resize(90, 100));

            StringBuilder binaryStringBuilder = new StringBuilder();
            int pixelCount = 0;
            int middleRow = image.Height / 2;

            for (int x = 0; x < image.Width && pixelCount < 30; x++)
            {
                Rgba32 pixelColor = image[x, middleRow];
                int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                string binaryValue = Convert.ToString(grayValue, 2).PadLeft(8, '0'); 
                binaryStringBuilder.Append(binaryValue);
                pixelCount++;
            }
            return binaryStringBuilder.ToString();
        }
    }

    public static string BinaryStringToAscii(string binaryString)
    {
        StringBuilder asciiStringBuilder = new StringBuilder();
        int len = binaryString.Length;
        if (len % 8 != 0)
        {
            len -= len % 8;
        }

        for (int i = 0; i < len; i += 8)
        {
            string byteString = binaryString.Substring(i, 8);
            byte byteValue = Convert.ToByte(byteString, 2);
            asciiStringBuilder.Append((char)byteValue);
        }

        return asciiStringBuilder.ToString();
    }
    public static int binaryToDecimal(string n){
        string num = n;
        int dec_value = 0;
        int base1 = 1;
        int len = num.Length;
        for (int i = len -1;i>=0;i--){
            if (num[i]=='1')
                dec_value += base1;
            base1= base1 * 2;
        }
        return dec_value;
    }
    public static string setStringtoAscii(string str){
        int N = str.Length;
        if (N%8 != 0){
            return "yaha";
        }
        string res ="";
        for (int i =0;i<N;i+=8){
            int decimal_value = binaryToDecimal((str.Substring(i,8)));
            res+= (char) (decimal_value);
        }
        return res;
    }

    public static void Main(string[] args)
    {
        string queryImagePath = "dataset/Real/1__M_Left_index_finger.BMP";
        string binaryString = ProcessImage(queryImagePath);
        Console.WriteLine("Binary String: " + binaryString);
        
        string asciiString = BinaryStringToAscii(binaryString);
        Console.WriteLine("ASCII String: " + asciiString);
        Console.WriteLine("Binary String Length: " + binaryString.Length);
        Console.WriteLine(setStringtoAscii(binaryString));
        Console.WriteLine(setStringtoAscii(binaryString).Length);
        int test = ("ÿÿÿÿ«:ÅCb{Km¼/8]&             ").Length;
        Console.WriteLine(test);
        if ( asciiString.Equals("ÿÿÿÿ«:ÅCb{Km¼/8]&")){
            Console.WriteLine("test1");
        }
        if (asciiString == "ÿÿÿÿ«:ÅCb{Km¼/8]&             "){
            Console.WriteLine("test");
        }
    }
}
