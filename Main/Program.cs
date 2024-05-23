using System;
using System.Text;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
public class Database
{
    public static string connectionString = "Server=localhost;Database=TubesStima3;Uid=root;Pwd=308140;";
    public static int jumlahdata =6000;
    public static string[,] RetrieveBiodataMatrix()
    {
        
        string[,] biodataMatrix = new string[jumlahdata, 11]; 

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM biodata";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                int row = 0;
                while (reader.Read() && row < jumlahdata)
                {
                    biodataMatrix[row, 0] = reader.GetString("NIK");
                    biodataMatrix[row, 1] = reader.GetString("nama");
                    biodataMatrix[row, 2] = reader.GetString("tempat_lahir");
                    biodataMatrix[row, 3] = reader.GetDateTime("tanggal_lahir").ToString("yyyy-MM-dd");
                    biodataMatrix[row, 4] = reader.GetString("jenis_kelamin");
                    biodataMatrix[row, 5] = reader.GetString("golongan_darah");
                    biodataMatrix[row, 6] = reader.GetString("alamat");
                    biodataMatrix[row, 7] = reader.GetString("agama");
                    biodataMatrix[row, 8] = reader.GetString("status_perkawinan");
                    biodataMatrix[row, 9] = reader.GetString("pekerjaan");
                    biodataMatrix[row, 10] = reader.GetString("kewarganegaraan");

                    row++;
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return biodataMatrix;
    }

    public static string[,] RetrieveSidikJariMatrix()
    {
        string[,] sidikJariMatrix = new string[jumlahdata, 2]; 

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM sidik_jari";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                int row = 0;
                while (reader.Read() && row < jumlahdata)
                {
                    sidikJariMatrix[row, 0] = reader.GetString("berkas_citra");
                    sidikJariMatrix[row, 1] = FixCorruptedName(reader.GetString("nama"));

                    row++;
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return sidikJariMatrix;
    }
    
    public static string FixCorruptedName(string corruptedName)
    {
        return FixUpperLower(FixNumber((corruptedName)));
    }

    public static string FixNumber(string corruptedName)
    {
        string pattern = "[01234567]";
        string result = Regex.Replace(corruptedName, pattern, NumberToChar);

        return result;
    }

    public static string NumberToChar(Match number)
    {
        switch (number.Value)
        {
            case "0":
                return "o";
            case "1":
                return "i";
            case "2":
                return "z";
            case "3":
                return "e";
            case "4":
                return "a";
            case "5":
                return "s";
            case "6":
                return "g";
            case "7":
                return "t";
            default:
                return number.Value;
        }
    }

    public static string FixUpperLower(string input)
    {
        string pattern = @"\w+";
        string result = Regex.Replace(input, pattern, new MatchEvaluator(CapitalizeWord));
        return result;
    }

    public static string CapitalizeWord(Match word)
    {
        string wordValue = word.Value;
        if (wordValue.Length > 1)
        {
            return char.ToUpper(wordValue[0]) + wordValue.Substring(1).ToLower();
        }
        else
        {
            return wordValue.ToUpper();
        }
    }


      public static string ShortenText(string input)
    {
        StringBuilder result = new StringBuilder();
        string[] words = input.Split(' ');
        foreach (string word in words)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (i == 0 || (i == word.Length - 1))
                {
                    result.Append(word[i]);
                }
                else if (!"aeiou".Contains(char.ToLower(word[i])))
                {
                    result.Append(word[i]);
                }
            }
            result.Append(' ');
        }
        return result.ToString().Trim();
    }
  

}


public class Algorithm
{
    public static int BMMatch(string pattern, string text)
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

    public static int[] BuildLast(string pattern)
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

    public static int KMPMatch(string pattern, string text)
    {
        int[] lsp = ComputeLspTable(pattern);
        int j = 0;
        for (int i = 0; i < text.Length; i++)
        {
            while (j > 0 && text[i] != pattern[j])
            {
                j = lsp[j - 1];
            }
            if (text[i] == pattern[j])
            {
                j++;
                if (j == pattern.Length)
                {
                    return i - (j - 1);
                }
            }
        }
        return -1;
    }

    public static int[] ComputeLspTable(string pattern)
    {
        int[] lsp = new int[pattern.Length];
        int j = 0;
        for (int i = 1; i < pattern.Length; i++)
        {
            while (j > 0 && pattern[i] != pattern[j])
            {
                j = lsp[j - 1];
            }
            if (pattern[i] == pattern[j])
            {
                j++;
            }
            lsp[i] = j;
        }
        return lsp;
    }

    public static string Get24BinaryPixels(string filePath)
    {
        using (Image<Rgba32> image = Image.Load<Rgba32>(filePath))
        {
            StringBuilder binaryStringBuilder = new StringBuilder();
            int requiredPixels = 24;
            int rows = (int)Math.Sqrt(requiredPixels); // 4 rows
            int cols = requiredPixels / rows; // 6 cols
            int gridWidth = image.Width / cols;
            int gridHeight = image.Height / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = col * gridWidth + gridWidth / 2;
                    int y = row * gridHeight + gridHeight / 2;
                    x = Math.Min(x, image.Width - 1);
                    y = Math.Min(y, image.Height - 1);
                    Rgba32 pixelColor = image[x, y];
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    string binary = Convert.ToString(grayValue, 2).PadLeft(8, '0');
                    binaryStringBuilder.Append(binary);
                }
            }

            return BinaryArrayToAscii(binaryStringBuilder.ToString());
        }
    }



}

public class Program{

    public static void Main(string[] args){
        double similarity_min = 80.0;
        string[,] biodataMatrix = Database.RetrieveBiodataMatrix();
        string[,] sidikJariMatrix = Database.RetrieveSidikJariMatrix();
        Stopwatch timer = new Stopwatch();
        while (true){
            Console.WriteLine("Masukkan Filepath dari query image (atau ketik 'exit' untuk keluar):");
            string queryImagePath = Console.ReadLine();
            if (string.IsNullOrEmpty(queryImagePath))
            {
                Console.WriteLine("Input tidak valid.");
                continue;
            }
            if (queryImagePath.ToLower() == "exit")
            {
                break;
            }
            
            string queryBinary;
            try
            {
                queryBinary = Algorithm.Get24BinaryPixels(queryImagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal membaca gambar: {ex.Message}");
                continue;
            }
            Console.WriteLine("Pilih algoritma pencocokan (1 untuk Boyer-Moore, 2 untuk Knuth-Morris-Pratt):");
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input tidak valid.");
                continue;
            }

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Input tidak valid.");
                continue;
            }

            Func<string, string, int> matchAlgorithm;
            
            if (choice == 1)
            {
                matchAlgorithm = Algorithm.BMMatch;
                Console.WriteLine("Menggunakan algoritma Boyer-Moore.");
            }
            else if (choice == 2)
            {
                matchAlgorithm = Algorithm.KMPMatch;
                Console.WriteLine("Menggunakan algoritma Knuth-Morris-Pratt.");
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
                continue;
            }
            
            List<List<string>> matchingImages = new List<List<string>>();
            timer.Start();
            for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
            {
                string datasetBinary = sidikJariMatrix[i,0];
                int exactMatchIndex = matchAlgorithm(queryBinary, datasetBinary);

                if (exactMatchIndex != -1)
                {
                    timer.Stop();
                    Console.WriteLine($"Exact match ditemukan di {sidikJariMatrix[i,1]}");
                    Console.WriteLine($"Waktu untuk matching: {timer.ElapsedMilliseconds} ms");
                    List<string> imageFound = new List<string>();
                    imageFound.Add(sidikJariMatrix[i,1]);
                    imageFound.Add("100%");
                    matchingImages.Add(imageFound);
                }
                
            }
            long exactMatchTime = timer.ElapsedMilliseconds;
            if(matchingImages.Count==0)
            {  
                Console.WriteLine("Tidak ada gambar yang exact match.\nMencari menggunakan pendekatan Levenshtein!");
                timer.Stop();
                
                
                
                timer.Restart();
                for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
                {
                    string datasetBinary = sidikJariMatrix[i,0];
                    double similarity = Algorithm.CalculateLevenshteinSimilarity(datasetBinary, queryBinary);

                    if (similarity > similarity_min)
                    {
                        List<string> imageFound = new List<string>();
                        imageFound.Add(sidikJariMatrix[i,1]);
                        imageFound.Add(similarity.ToString());
                        matchingImages.Add(imageFound);
                    }
                }
                timer.Stop();
                long levenshteinTime = timer.ElapsedMilliseconds;
                Console.WriteLine($"Waktu untuk Levenshtein similarity calculation: {levenshteinTime} ms");
            }
            if (matchingImages.Count > 0)
            {
                Console.WriteLine($"Waktu total pencarian: {exactMatchTime} ms");
                Console.WriteLine($"Gambar yang mirip dengan query (lebih dari {similarity_min}%):");
                foreach (List<string> resultList in matchingImages)
                {
                    List<string[]> similarNames = new List<string[]>();
                    for (int i = 0; i<biodataMatrix.GetLength(0);i++)
                    {
                        string biodataName = biodataMatrix[i,1];
                        int matchVal = matchAlgorithm(resultList[0],biodataName);
                        if (matchVal!=-1){
                            string[] biodataRow = new string[biodataMatrix.GetLength(1)];
                            for (int j = 0; j < biodataMatrix.GetLength(1); j++)
                            {
                                biodataRow[j] = biodataMatrix[i, j];
                            }
                            similarNames.Add(biodataRow);

                            Console.WriteLine($"NIK: {biodataMatrix[i,0]}");
                            Console.WriteLine($"Nama: {biodataMatrix[i,1]}");
                            Console.WriteLine($"Tempat Lahir: {biodataMatrix[i,2]}");
                            Console.WriteLine($"Tanggal Lahir: {biodataMatrix[i,3]}");
                            Console.WriteLine($"Jenis Kelamin: {biodataMatrix[i,4]}");
                            Console.WriteLine($"Golongan Darah: {biodataMatrix[i,5]}");
                            Console.WriteLine($"Alamat: {biodataMatrix[i,6]}");
                            Console.WriteLine($"Agama: {biodataMatrix[i,7]}");
                            Console.WriteLine($"Status Perkawinan: {biodataMatrix[i,8]}");
                            Console.WriteLine($"Pekerjaan: {biodataMatrix[i,9]}");
                            Console.WriteLine($"Kewarganegaraan: {biodataMatrix[i,10]}");
                            Console.WriteLine($"Kemiripan : {resultList[1]}");
                            break;
                        }
                        
                    }
                    Console.WriteLine($"Tidak ada nama yang exactMatch.\nMencari menggunakan levensthein.");
                    if(similarNames.Count==0)
                    {   
                        for (int i = 0; i<biodataMatrix.GetLength(0);i++)
                        {
                            string biodataName = biodataMatrix[i,1];
                            double similarity = Algorithm.CalculateLevenshteinSimilarity(resultList[0],biodataName);
                            if (similarity > similarity_min)
                            {
                                string[] biodataRow = new string[biodataMatrix.GetLength(1)];
                                for (int j = 0; j < biodataMatrix.GetLength(1); j++)
                                {
                                    biodataRow[j] = biodataMatrix[i, j];
                                }
                                similarNames.Add(biodataRow);
                                Console.WriteLine($"NIK: {biodataMatrix[i,0]}");
                                Console.WriteLine($"Nama: {biodataMatrix[i,1]}");
                                Console.WriteLine($"Tempat Lahir: {biodataMatrix[i,2]}");
                                Console.WriteLine($"Tanggal Lahir: {biodataMatrix[i,3]}");
                                Console.WriteLine($"Jenis Kelamin: {biodataMatrix[i,4]}");
                                Console.WriteLine($"Golongan Darah: {biodataMatrix[i,5]}");
                                Console.WriteLine($"Alamat: {biodataMatrix[i,6]}");
                                Console.WriteLine($"Agama: {biodataMatrix[i,7]}");
                                Console.WriteLine($"Status Perkawinan: {biodataMatrix[i,8]}");
                                Console.WriteLine($"Pekerjaan: {biodataMatrix[i,9]}");
                                Console.WriteLine($"Kewarganegaraan: {biodataMatrix[i,10]}");
                                Console.WriteLine($"Kemiripan : {resultList[1]}");
                                break;
                            }
                        }
                    }
                    
                    if (similarNames.Count == 0)
                    {
                        Console.WriteLine("Tidak ada nama yang mirip.");
                    }

                }
                Console.WriteLine($"Jumlah data yang mirip: {matchingImages.Count}");
            }
            else
            {
                Console.WriteLine($"Waktu total pencarian: {exactMatchTime} ms");
                Console.WriteLine($"Tidak ada gambar yang memiliki kemiripan lebih dari {similarity_min}% dengan query.");
            }

            

        }




    }


}

// dataset/Real/1__M_Left_index_finger.BMP
// dataset/Altered/Altered-Hard/1__M_Left_index_finger_CR.BMP
// dataset/Altered/Altered-Easy/1__M_Left_index_finger_CR.BMP
