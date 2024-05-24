// using System;
// using System.Text;
// using System.IO;
// using System.Collections.Generic;
// // using SixLabors.ImageSharp;
// // using SixLabors.ImageSharp.PixelFormats;
// using System.Diagnostics;
// using OpenCvSharp;
// using OpenCvSharp.Features2D;
// using System.Drawing;

// public class BoyerMooreAlgorithm
// {
//     public static int BMMatch(string pattern, string text)
//     {
//         int[] last = BuildLast(pattern);
//         int n = text.Length;
//         int m = pattern.Length;
//         int i = m - 1;
//         if (i > n - 1)
//             return -1;

//         int j = m - 1;
//         do
//         {
//             if (pattern[j] == text[i])
//             {
//                 if (j == 0)
//                     return i;
//                 else
//                 {
//                     i--;
//                     j--;
//                 }
//             }
//             else
//             {
//                 int lo = last[text[i]];
//                 i = i + m - Math.Min(j, 1 + lo);
//                 j = m - 1;
//             }
//         } while (i <= n - 1);
//         return -1;
//     }

//     private static int[] BuildLast(string pattern)
//     {
//         int[] last = new int[256];
//         for (int i = 0; i < 256; i++)
//         {
//             last[i] = -1;
//         }
//         for (int i = 0; i < pattern.Length; i++)
//         {
//             last[pattern[i]] = i;
//         }
//         return last;
//     }

//     public static double CalculateLevenshteinSimilarity(string str1, string str2)
//     {
//         int[,] dp = new int[str1.Length + 1, str2.Length + 1];

//         for (int i = 0; i <= str1.Length; i++)
//         {
//             for (int j = 0; j <= str2.Length; j++)
//             {
//                 if (i == 0)
//                 {
//                     dp[i, j] = j;
//                 }
//                 else if (j == 0)
//                 {
//                     dp[i, j] = i;
//                 }
//                 else if (str1[i - 1] == str2[j - 1])
//                 {
//                     dp[i, j] = dp[i - 1, j - 1];
//                 }
//                 else
//                 {
//                     dp[i, j] = 1 + Math.Min(dp[i - 1, j], Math.Min(dp[i, j - 1], dp[i - 1, j - 1]));
//                 }
//             }
//         }

//         int maxLength = Math.Max(str1.Length, str2.Length);
//         return 100.0 * (1.0 - (double)dp[str1.Length, str2.Length] / maxLength);
//     }

//     public static string BinaryArrayToAscii(string binaryArray)
//     {
//         if (binaryArray.Length % 8 != 0)
//         {
//             throw new ArgumentException("Bukan Kelipatan 8");
//         }

//         StringBuilder asciiBuilder = new StringBuilder();

//         for (int i = 0; i < binaryArray.Length; i += 8)
//         {
//             int asciiValue = Convert.ToInt32(binaryArray.Substring(i, 8), 2);
//             asciiBuilder.Append((char)asciiValue);
//         }

//         return asciiBuilder.ToString();
//     }

//     public static int KMPMatch(string pattern, string text)
//     {
//         int[] lsp = ComputeLspTable(pattern);
//         int j = 0;
//         for (int i = 0; i < text.Length; i++)
//         {
//             while (j > 0 && text[i] != pattern[j])
//             {
//                 j = lsp[j - 1];
//             }
//             if (text[i] == pattern[j])
//             {
//                 j++;
//                 if (j == pattern.Length)
//                 {
//                     return i - (j - 1);
//                 }
//             }
//         }
//         return -1;
//     }

//     private static int[] ComputeLspTable(string pattern)
//     {
//         int[] lsp = new int[pattern.Length];
//         int j = 0;
//         for (int i = 1; i < pattern.Length; i++)
//         {
//             while (j > 0 && pattern[i] != pattern[j])
//             {
//                 j = lsp[j - 1];
//             }
//             if (pattern[i] == pattern[j])
//             {
//                 j++;
//             }
//             lsp[i] = j;
//         }
//         return lsp;
//     }

//     // private static string Get24BinaryPixels(string filePath)
//     // {
//     //     using (Image<Rgba32> image = Image.Load<Rgba32>(filePath))
//     //     {
//     //         StringBuilder binaryStringBuilder = new StringBuilder();
//     //         int requiredPixels = 24;
//     //         int rows = (int)Math.Sqrt(requiredPixels); // 4 rows
//     //         int cols = requiredPixels / rows; // 6 cols
//     //         int gridWidth = image.Width / cols;
//     //         int gridHeight = image.Height / rows;

//     //         for (int row = 0; row < rows; row++)
//     //         {
//     //             for (int col = 0; col < cols; col++)
//     //             {
//     //                 int x = col * gridWidth + gridWidth / 2;
//     //                 int y = row * gridHeight + gridHeight / 2;
//     //                 x = Math.Min(x, image.Width - 1);
//     //                 y = Math.Min(y, image.Height - 1);
//     //                 Rgba32 pixelColor = image[x, y];
//     //                 int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
//     //                 string binary = Convert.ToString(grayValue, 2).PadLeft(8, '0');
//     //                 binaryStringBuilder.Append(binary);
//     //             }
//     //         }

//     //         return BinaryArrayToAscii(binaryStringBuilder.ToString());
//     //     }
//     // }
//     private static string Get24BinaryPixels(Mat image)
//     {
//         Bitmap bitmap;
//         using (var ms = new MemoryStream(image.ToBytes(".bmp")))
//         {
//             bitmap = new Bitmap(ms);
//         }

//         StringBuilder binaryStringBuilder = new StringBuilder();
//         int requiredPixels = 24;
//         int rows = (int)Math.Sqrt(requiredPixels); // 4 rows
//         int cols = requiredPixels / rows; // 6 cols
//         int gridWidth = bitmap.Width / cols;
//         int gridHeight = bitmap.Height / rows;

//         for (int row = 0; row < rows; row++)
//         {
//             for (int col = 0; col < cols; col++)
//             {
//                 int x = col * gridWidth + gridWidth / 2;
//                 int y = row * gridHeight + gridHeight / 2;
//                 x = Math.Min(x, bitmap.Width - 1);
//                 y = Math.Min(y, bitmap.Height - 1);
//                 Color pixelColor = bitmap.GetPixel(x, y);
//                 int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
//                 string binary = Convert.ToString(grayValue, 2).PadLeft(8, '0');
//                 binaryStringBuilder.Append(binary);
//             }
//         }

//         return BinaryArrayToAscii(binaryStringBuilder.ToString());
//     }
//     public static void ProcessImage(string queryImagePath, string originalImagePath)
//     {
//         // Load and process query image
//         Mat queryImage = Cv2.ImRead(queryImagePath);
//         Mat contrastedQueryImage = IncreaseContrast(queryImage);
//         Mat resizedQueryImage = ResizeImage(contrastedQueryImage, 90, 100);
//         KeyPoint[] queryKeypoints;
//         Mat queryDescriptors = DetectSIFTFeatures(resizedQueryImage, out queryKeypoints);
//         string queryPattern = Get24BinaryPixels(resizedQueryImage);

//         // Load and process original image
//         Mat originalImage = Cv2.ImRead(originalImagePath);
//         Mat contrastedOriginalImage = IncreaseContrast(originalImage);
//         Mat resizedOriginalImage = ResizeImage(contrastedOriginalImage, 90, 100);
//         KeyPoint[] originalKeypoints;
//         Mat originalDescriptors = DetectSIFTFeatures(resizedOriginalImage, out originalKeypoints);
//         string originalText = Get24BinaryPixels(resizedOriginalImage);

//         // Implement pattern matching using KMP and Boyer-Moore with queryPattern and originalText
//         // Implement pattern matching using keypoints and descriptors
//     }


//     public static void Main(string[] args)
//     {
//         while (true){
            
//         Console.WriteLine("Pilih algoritma pencocokan (1 untuk Boyer-Moore, 2 untuk Knuth-Morris-Pratt):");
//         string input = Console.ReadLine();

//         if (string.IsNullOrEmpty(input))
//         {
//             Console.WriteLine("Input tidak valid.");
//             return;
//         }

//         if (!int.TryParse(input, out int choice))
//         {
//             Console.WriteLine("Input tidak valid.");
//             return;
//         }

//         Func<string, string, int> matchAlgorithm;
//         string datasetPath = "dataset/Real";
//         string queryImagePath = "dataset/Altered/Altered-Easy/1__M_Left_index_finger_CR.BMP";
//         double similarity_min = 80.0;
//         string queryBinary = Get24BinaryPixels(queryImagePath);
//         List<string> matchingImages = new List<string>();

//         if (choice == 1)
//         {
//             matchAlgorithm = BMMatch;
//             Console.WriteLine("Menggunakan algoritma Boyer-Moore.");
//         }
//         else if (choice == 2)
//         {
//             matchAlgorithm = KMPMatch;
//             Console.WriteLine("Menggunakan algoritma Knuth-Morris-Pratt.");
//         }
//         else
//         {
//             Console.WriteLine("Pilihan tidak valid.");
//             return;
//         }

//         Stopwatch timer = new Stopwatch();
//         timer.Start();
//         foreach (string imagePath in Directory.GetFiles(datasetPath, "*.BMP"))
//         {
//             string datasetBinary = Get24BinaryPixels(imagePath);
//             int exactMatchIndex = matchAlgorithm(queryBinary, datasetBinary);

//             if (exactMatchIndex != -1)
//             {
//                 timer.Stop();
//                 Console.WriteLine($"Exact match found at {imagePath}");
//                 Console.WriteLine($"Time taken for matching: {timer.ElapsedMilliseconds} ms");
//                 return;
//             }
//         }

//         Console.WriteLine("Tidak ada gambar yang exact match.\nMencari menggunakan pendekatan Levenshtein!");

//         timer.Stop();
//         long exactMatchTime = timer.ElapsedMilliseconds;

//         timer.Restart();
//         foreach (string imagePath in Directory.GetFiles(datasetPath, "*.BMP"))
//         {
//             string datasetBinary = Get24BinaryPixels(imagePath);
//             double similarity = CalculateLevenshteinSimilarity(datasetBinary, queryBinary);

//             if (similarity > similarity_min)
//             {
//                 matchingImages.Add($"{imagePath} - Similarity: {similarity}%");
//             }
//         }
//         timer.Stop();
//         long levenshteinTime = timer.ElapsedMilliseconds;

//         if (matchingImages.Count > 0)
//         {
//             Console.WriteLine($"Gambar yang mirip dengan query (lebih dari {similarity_min}%):");
//             foreach (string result in matchingImages)
//             {
//                 Console.WriteLine(result);
//             }
//             Console.WriteLine($"Jumlah data yang mirip: {matchingImages.Count}");
//         }
//         else
//         {
//             Console.WriteLine($"Tidak ada gambar yang memiliki kemiripan lebih dari {similarity_min}% dengan query.");
//         }

//         Console.WriteLine($"Time taken for exact match search: {exactMatchTime} ms");
//         Console.WriteLine($"Time taken for Levenshtein similarity calculation: {levenshteinTime} ms");

//         }
//     }
// }
