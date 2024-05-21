using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class BoyerMooreAlgorithm
{
    public static int BMMatch(string text, string pattern)
    {
        int[] last = BuildLast(pattern);
        int n = text.Length;
        int m = pattern.Length;
        int i = m - 1;
        if (i > n - 1)
            return -1;

        int j = m - 1;
        do
        {
            if (pattern[j] == text[i])
            {
                if (j == 0)
                    return i;
                else
                {
                    i--;
                    j--;
                }
            }
            else
            {
                int lo = last[text[i]];
                i = i + m - Math.Min(j, 1 + lo);
                j = m - 1;
            }
        } while (i <= n - 1);
        return -1;
    }

    private static int[] BuildLast(string pattern)
    {
        int[] last = new int[256];
        for (int i = 0; i < 256; i++)
        {
            last[i] = -1;
        }
        for (int i = 0; i < pattern.Length; i++)
        {
            last[pattern[i]] = i;
        }
        return last;
    }

    public static double CalculateLevenshteinSimilarity(string str1, string str2)
    {
        int[,] dp = new int[str1.Length + 1, str2.Length + 1];

        for (int i = 0; i <= str1.Length; i++)
        {
            for (int j = 0; j <= str2.Length; j++)
            {
                if (i == 0)
                {
                    dp[i, j] = j;
                }
                else if (j == 0)
                {
                    dp[i, j] = i;
                }
                else if (str1[i - 1] == str2[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1];
                }
                else
                {
                    dp[i, j] = 1 + Math.Min(dp[i - 1, j], Math.Min(dp[i, j - 1], dp[i - 1, j - 1]));
                }
            }
        }

        int maxLength = Math.Max(str1.Length, str2.Length);
        return 100.0 * (1.0 - (double)dp[str1.Length, str2.Length] / maxLength);
    }

    public static string BinaryArrayToAscii(string binaryArray)
    {
        if (binaryArray.Length % 8 != 0)
        {
            throw new ArgumentException("Bukan Kelipatan 8");
        }

        StringBuilder asciiBuilder = new StringBuilder();

        for (int i = 0; i < binaryArray.Length; i += 8)
        {
            int asciiValue = Convert.ToInt32(binaryArray.Substring(i, 8), 2);
            asciiBuilder.Append((char)asciiValue);
        }

        return asciiBuilder.ToString();
    }

    public static string ReadFingerprintAndMatch(string imagePath)
    {
        using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
        {
            StringBuilder binaryStringBuilder = new StringBuilder();

            // maksimal pixel adalah 30
            // dari dataset yang dikasih rata rata ukurannya 96 * 103
            // buletin jadi (15)*6p * (20)*5p  
            for (int y = 0; y < image.Height; y+=20)
            {
                for (int x = 0; x < image.Width; x+=15)
                {
                    Rgba32 pixelColor = image[x, y];
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    string binary = Convert.ToString(grayValue, 2).PadLeft(8, '0');
                    binaryStringBuilder.Append(binary);
                }
            }
            return BinaryArrayToAscii(binaryStringBuilder.ToString());
        }
    }

    public static void Main(string[] args)
    {
        string datasetPath = "dataset/Real";
        string queryImagePath = "dataset/Real/100__M_Left_index_finger.BMP";
        // ini atur minimal similarity
        double similarity_min = 85.0;
        string queryBinary = ReadFingerprintAndMatch(queryImagePath);
        List<string> matchingImages = new List<string>();

        foreach (string imagePath in Directory.GetFiles(datasetPath, "*.BMP"))
        {
            string datasetBinary = ReadFingerprintAndMatch(imagePath);
            double similarity = CalculateLevenshteinSimilarity(datasetBinary, queryBinary);

            if (similarity > similarity_min)
            {
                matchingImages.Add($"{imagePath} - Similarity: {similarity}%");
            }
        }

        if (matchingImages.Count > 0)
        {
            Console.WriteLine("Gambar yang mirip dengan query (lebih dari {0}%):",similarity_min);
            foreach (string result in matchingImages)
            {
                Console.WriteLine(result);
            }
            Console.WriteLine("Jumlah data yang mirip {0}",matchingImages.Count);
        }
        else
        {
            Console.WriteLine("Tidak ada gambar yang memiliki kemiripan lebih dari 80% dengan query.");
        }
    }
}
