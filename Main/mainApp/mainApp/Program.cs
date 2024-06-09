using System;
using System.Text;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Linq;

public class Encryption{
    public static string RSADecryption(string plaintext, bool isEncrypting)
    {
        int private_key = 39653; //1019; //d
        int public_key = 65537; //79; //e
        int p =  211; //47; //bebas berapa //rahasia
        int q = 223; //71; //bebas berapa //rahasia
        int n = p * q; //3337 //public
        int m = (p - 1) * (q - 1); //46620 //3220 //rahasia
        if (isEncrypting)
        {
            List<string> encrypted = StringToASCIIHex(plaintext);
            List<string> result = new List<string>();
            foreach(string hex in encrypted)
            {
                int num = Convert.ToInt32(hex, 16);
                long encryptedNum = ModularExponentiation(num, public_key, n);
                string encryptedHex = encryptedNum.ToString("X");
                result.Add(encryptedHex);
            }
            
            result = HexToASCII(result);
            string ans = string.Join("", result);
            return ans;
        }
        else
        {
            List<string> encrypted = StringToList(ASCIIToHex(plaintext));
            List<string> result = new List<string>();
            foreach(string hex in encrypted)
            {               
                int num = Convert.ToInt32(hex, 16);
                long encryptedNum = ModularExponentiation(num, private_key, n);
                string encryptedHex = encryptedNum.ToString("X");
                result.Add(encryptedHex);
            }
            result = HexToASCII(result);
            string ans = string.Join("", result);
            return ans;
        }
    }
    public static List<string> StringToList(string input)
    {
        string[] wordsArray = input.Split(' ');
        List<string> wordsList = new List<string>(wordsArray);
        return wordsList;
    }
    public static List<string> StringToASCIIHex(string plaintext)
    {
        List<string> hexValues = new List<string>();
        foreach (char c in plaintext)
        {
            int asciiVal = (int)c;
            string hexVal = asciiVal.ToString("X2");
            hexValues.Add(hexVal);
        }
        return hexValues;
    }
    public static string ASCIIToHex(string plaintext)
    {
        StringBuilder hexValues = new StringBuilder();
        foreach (char c in plaintext)
        {
            int asciiVal = (int)c;
            string hexVal = asciiVal.ToString("X2");
            hexValues.Append(hexVal);
            hexValues.Append(" ");
        }
        hexValues.Remove(hexValues.Length - 1, 1);
        return hexValues.ToString();
    }

    public static List<string> HexToASCII(List<string> input)
    {
        List<string> ASCIIVal = new List<string>();
        foreach (string hex in input)
        {
            int num = Convert.ToInt32(hex, 16);
            char character = (char)num; 
            ASCIIVal.Add(character.ToString());
        }
        return ASCIIVal;
    }


    public static long ModularExponentiation(long baseValue, long exponent, long modulus)
    {
        long result = 1;
        baseValue = baseValue % modulus;
        while (exponent > 0)
        {
            if ((exponent % 2) == 1)
                result = (result * baseValue) % modulus;
            exponent = exponent >> 1;
            baseValue = (baseValue * baseValue) % modulus;
        }
        return result;
    }
}
public class Database
{
    public static string connectionString = "Server=localhost;Database=TubesStima3;Uid=root;Pwd=308140;";
    public static int jumlahdata =6000;
    public static string[,] RetrieveBiodataMatrix()
    {
        
        string[,] biodataMatrix = new string[600, 11]; 

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
                    biodataMatrix[row, 1] = Encryption.RSADecryption(reader.GetString("nama"), isEncrypting: false);
                    biodataMatrix[row, 2] = Encryption.RSADecryption(reader.GetString("tempat_lahir"), isEncrypting: false);
                    biodataMatrix[row, 3] = reader.GetDateTime("tanggal_lahir").ToString("yyyy-MM-dd");
                    biodataMatrix[row, 4] = reader.GetString("jenis_kelamin");
                    biodataMatrix[row, 5] = Encryption.RSADecryption(reader.GetString("golongan_darah"), isEncrypting: false);
                    biodataMatrix[row, 6] =Encryption.RSADecryption(reader.GetString("alamat"), isEncrypting: false) ;
                    biodataMatrix[row, 7] = Encryption.RSADecryption(reader.GetString("agama"), isEncrypting: false);
                    biodataMatrix[row, 8] = reader.GetString("status_perkawinan");
                    biodataMatrix[row, 9] = Encryption.RSADecryption(reader.GetString("pekerjaan"), isEncrypting: false);
                    biodataMatrix[row, 10] = Encryption.RSADecryption(reader.GetString("kewarganegaraan"), isEncrypting: false);

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
        string[,] sidikJariMatrix = new string[jumlahdata, 3]; 

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
                    sidikJariMatrix[row, 1] = (reader.GetString("nama"));
                    sidikJariMatrix[row, 2] = reader.GetString("path_image");

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
        string pattern = "[012345678]";
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
            case "8":
                return "b";
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

    public static string RSADecryption(string nama, bool isEncrypting)
    {
        int private_key = 1019; //d
        int public_key = 79; //e
        int p = 47; //bebas berapa
        int q = 71; //bebas berapa
        int n = p * q; //3337
        int m = (p - 1) * (q - 1); //3220

        StringBuilder stringBuilder = new StringBuilder();

        if (isEncrypting)
        {
            string encrypted = StringToASCIIDecimal(nama);
            for (int i = 0; i < encrypted.Length; i += 3)
            {
                string enc = encrypted.Substring(i, 3);
                int num = int.Parse(enc);
                num = (int)ModularExponentiation(num, public_key, n);
                stringBuilder.Append(num);
            }
            return stringBuilder.ToString();
        }
        else
        {
            for (int i = 0; i < nama.Length; i += 3)
            {
                string enc = nama.Substring(i, Math.Min(3, nama.Length - i)); // Memastikan tidak melebihi panjang string
                int num = int.Parse(enc);
                num = (int)ModularExponentiation(num, private_key, n);
                stringBuilder.Append(num);
            }
            return ASCIIDecimalToString(stringBuilder.ToString());
        }
    }

    public static string StringToASCIIDecimal(string nama)
    {
        StringBuilder asciiDecimal = new StringBuilder();
        foreach (char c in nama)
        {
            int asciiVal = (int)c;
            asciiDecimal.Append(asciiVal.ToString("D3")); // Menambahkan nol di depan jika kurang dari tiga digit
        }
        return asciiDecimal.ToString();
    }

    public static string ASCIIDecimalToString(string asciiDecimalString)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < asciiDecimalString.Length; i += 3)
        {
            string trio = asciiDecimalString.Substring(i, 3);
            int asciiValue = int.Parse(trio);
            while (asciiValue > 128)
            {
                asciiValue = asciiValue / 10;
            }
            char character = (char)asciiValue;
            sb.Append(character);
        }
        return sb.ToString();
    }

    public static long ModularExponentiation(long baseNum, long exponent, long modulus)
    {
        if (modulus == 1)
            return 0;

        long result = 1;
        baseNum = baseNum % modulus;

        while (exponent > 0)
        {
            if (exponent % 2 == 1)
                result = (result * baseNum) % modulus;

            exponent = exponent >> 1;
            baseNum = (baseNum * baseNum) % modulus;
        }

        return result;
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

    
   public static string ProcessImage(string imagePath)
    {
        using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
        {
            StringBuilder binaryStringBuilder = new StringBuilder();
            image.Mutate(x => x.Resize(90, 100));
            for (int y = 0; y<image.Height ; y++){
                for (int x = 0; x < image.Width; x++)
                {
                    Rgba32 pixelColor = image[x, y];
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    binaryStringBuilder.Append(grayValue >= 128 ? '1' : '0');
                }
            }
            return BinaryStringToAscii(binaryStringBuilder.ToString());
        }
    }
    public static string ProcessImage1(string imagePath)
{
    using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
    {
        image.Mutate(x => x.Resize(90, 100)); 
        StringBuilder binaryStringBuilder = new StringBuilder();
        int pixelCount = 0;
        for (int y = 100-image.Height/5; y<image.Height && pixelCount<80;y++)
        {
            for (int x = 0; x < image.Width && pixelCount < 80; x++)
            {
                Rgba32 pixelColor = image[x, y];
                int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                binaryStringBuilder.Append(grayValue >= 128 ? '1' : '0');
                pixelCount++;
            }
        }
        return BinaryStringToAscii(binaryStringBuilder.ToString());
    }
}

    public static string BinaryStringToAscii(string binaryString)
    {
        StringBuilder asciiBuilder = new StringBuilder();
        int len = binaryString.Length;
        if (len % 8 !=0)
            len = len - len%8;
        for (int i = 0; i < len; i += 8)
        {
            string binaryByte = binaryString.Substring(i, 8);
            int decimalValue = Convert.ToInt32(binaryByte, 2);
            char asciiChar = Convert.ToChar(decimalValue);
            asciiBuilder.Append(asciiChar);
        }

        return asciiBuilder.ToString();
    }
}



public class Program{
    public long timeNeeded; //buat bagian waktunya
    public string[,] solutionsValid; //buat placehoder hasilnya
    public string setImageQuery; //buat data image yang di querry
    public string algoChoose; //buat masukin pilihan algo yang mana
    public double min_similarity;
    public int matches;

    public void mainProgram(string algo, double min_similar, string imagePathQuery)
    {
        this.algoChoose = algo;
        this.min_similarity = min_similar;
        this.setImageQuery = imagePathQuery;
        Stopwatch timer = new Stopwatch();

        string[,] biodataMatrix = Database.RetrieveBiodataMatrix();
        string[,] sidikJariMatrix = Database.RetrieveSidikJariMatrix();

        string queryBinary;
        queryBinary = Algorithm.ProcessImage1(imagePathQuery);

        Func<string, string, int> matchAlgorithm;

        if (algo.Equals("KMP"))
        {
            matchAlgorithm = Algorithm.KMPMatch;
            Debug.WriteLine("Menggunakan Algoritma KMP==========");
        }
        else //BM Algorithm
        {
            matchAlgorithm = Algorithm.BMMatch;
            Debug.WriteLine("Menggunakan Algoritma BM==========");
        }


        List<List<string>> matchingImages = new List<List<string>>();
        timer.Start();
        for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
        {
            string datasetBinary = (sidikJariMatrix[i, 0]);
            int exactMatchIndex = matchAlgorithm(queryBinary, datasetBinary);

            if (exactMatchIndex != -1)
            {
                // timer.Stop();
                // Console.WriteLine($"Exact match ditemukan di {sidikJariMatrix[i,1]}");
                // Console.WriteLine($"Waktu untuk matching: {timer.ElapsedMilliseconds} ms");
                List<string> imageFound = new List<string>();
                imageFound.Add(sidikJariMatrix[i, 1]);
                imageFound.Add("100%");
                imageFound.Add(sidikJariMatrix[i, 2]);
                matchingImages.Add(imageFound);
                break;
            }

        }

        if (matchingImages.Count == 0)
        {
            Debug.WriteLine("==============================================================");
            Debug.WriteLine("Tidak ada gambar yang exact match.\nMencari gambar menggunakan pendekatan Levenshtein!");
            // timer.Stop();



            // timer.Restart();
            for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
            {
                string datasetBinary = sidikJariMatrix[i, 0];
                double similarity = Algorithm.CalculateLevenshteinSimilarity(datasetBinary, queryBinary);

                if (similarity > min_similar)
                {
                    // Console.WriteLine($"Kemiripan ditemukan di {sidikJariMatrix[i,1]}");
                    List<string> imageFound = new List<string>();
                    imageFound.Add(sidikJariMatrix[i, 1]);
                    imageFound.Add(similarity.ToString("F2") + "%");
                    imageFound.Add(sidikJariMatrix[i, 2]);
                    matchingImages.Add(imageFound);
                }
            }
            timer.Stop();
            matchingImages.Sort((a, b) =>
                {
                    double similarityA = double.Parse(a[1].TrimEnd('%'));
                    double similarityB = double.Parse(b[1].TrimEnd('%'));
                    return similarityB.CompareTo(similarityA);
                });
            long levenshteinTime = timer.ElapsedMilliseconds;
            // Console.WriteLine($"Waktu untuk Levenshtein similarity calculation: {levenshteinTime} ms");
        }

        Debug.WriteLine($"Jumlah data yang mirip: {matchingImages.Count}");
        this.matches = matchingImages.Count;

        if (matchingImages.Count > 0)
        {
            int co = 0;
            this.solutionsValid = new string[1000, 13]; 
            // Console.WriteLine($"Waktu total pencarian: {exactMatchTime} ms");
            Debug.WriteLine($"Gambar yang mirip dengan query (lebih dari {min_similar}%):");
            Debug.WriteLine("==============================================================");
            foreach (List<string> resultList in matchingImages)
            {
                List<string[]> similarNames = new List<string[]>();
                for (int i = 0; i < biodataMatrix.GetLength(0) && co<0; i++)
                {
                    string biodataName = biodataMatrix[i, 1];
                    // Console.WriteLine(resultList[0]);
                    // Console.WriteLine(biodataName);
                    int matchVal;
                    if (biodataName.Length > resultList[0].Length){
                        matchVal = matchAlgorithm(Database.FixCorruptedName(biodataName.Replace(" ","")),(resultList[0]).Replace(" ",""));
                    }
                    else{
                        matchVal = matchAlgorithm((resultList[0]).Replace(" ",""), Database.FixCorruptedName(biodataName.Replace(" ","")));
                    }
                    if (matchVal != -1)
                    {
                        Debug.WriteLine("==============================================================");
                        string[] biodataRow = new string[biodataMatrix.GetLength(1)];
                        for (int j = 0; j < biodataMatrix.GetLength(1); j++)
                        {
                            biodataRow[j] = biodataMatrix[i, j];
                        }
                        similarNames.Add(biodataRow);

                        Debug.WriteLine($"NIK: {biodataMatrix[i, 0]}");
                        Debug.WriteLine($"Nama: {resultList[0]}");
                        Debug.WriteLine($"Tempat Lahir: {biodataMatrix[i, 2]}");
                        Debug.WriteLine($"Tanggal Lahir: {biodataMatrix[i, 3]}");
                        Debug.WriteLine($"Jenis Kelamin: {biodataMatrix[i, 4]}");
                        Debug.WriteLine($"Golongan Darah: {biodataMatrix[i, 5]}");
                        Debug.WriteLine($"Alamat: {biodataMatrix[i, 6]}");
                        Debug.WriteLine($"Agama: {biodataMatrix[i, 7]}");
                        Debug.WriteLine($"Status Perkawinan: {biodataMatrix[i, 8]}");
                        Debug.WriteLine($"Pekerjaan: {biodataMatrix[i, 9]}");
                        Debug.WriteLine($"Kewarganegaraan: {biodataMatrix[i, 10]}");
                        Debug.WriteLine($"Kemiripan : {resultList[1]}");

                        Debug.WriteLine($"Path image : {resultList[2]}");

                        this.solutionsValid[co, 0] = biodataMatrix[i, 0];
                        this.solutionsValid[co, 1] = resultList[0];
                        this.solutionsValid[co, 2] = biodataMatrix[i, 2];
                        this.solutionsValid[co, 3] = biodataMatrix[i, 3];
                        this.solutionsValid[co, 4] = biodataMatrix[i, 4];
                        this.solutionsValid[co, 5] = biodataMatrix[i, 5];
                        this.solutionsValid[co, 6] = biodataMatrix[i, 6];
                        this.solutionsValid[co, 7] = biodataMatrix[i, 7];
                        this.solutionsValid[co, 8] = biodataMatrix[i, 8];
                        this.solutionsValid[co, 9] = biodataMatrix[i, 9];
                        this.solutionsValid[co, 10] = biodataMatrix[i, 10];
                        this.solutionsValid[co, 11] = resultList[1];
                        this.solutionsValid[co, 12] = resultList[2];
                        co++;
                        break;
                    }

                }
                Debug.WriteLine(similarNames.Count);
                if (similarNames.Count == 0)
                {
                    co = 0;
                    List<double> similarityArray= new List<double>();
                    Debug.WriteLine($"Tidak ada nama yang exactMatch.\nMencari nama menggunakan levensthein.");
                    for (int i = 0; i < biodataMatrix.GetLength(0); i++)
                    {
                        string biodataName = biodataMatrix[i, 1];
                        double similarity = Algorithm.CalculateLevenshteinSimilarity(Database.FixCorruptedName(biodataName.Replace(" ","")),(resultList[0]).Replace(" ",""));
                        similarityArray.Add(similarity);
                        
                       // Debug.WriteLine(min_similar);
                        if (biodataName == "Bambang Bayu Rina") { Debug.WriteLine(biodataName);
                            Debug.WriteLine(similarity);
                            Debug.WriteLine(similarityArray.Max());
                        }
                        if ((similarity > 50) && (similarity >= similarityArray.Max()))
                        {
                            Debug.WriteLine(similarity>min_similar);
                            Debug.WriteLine("==============================================================");
                            string[] biodataRow = new string[biodataMatrix.GetLength(1)];
                            for (int j = 0; j < biodataMatrix.GetLength(1); j++)
                            {
                                biodataRow[j] = biodataMatrix[i, j];
                            }
                            similarNames.Add(biodataRow);
                            Debug.WriteLine($"NIK: {biodataMatrix[i, 0]}");
                            Debug.WriteLine($"Nama: {resultList[0]}");
                            Debug.WriteLine($"Tempat Lahir: {biodataMatrix[i, 2]}");
                            Debug.WriteLine($"Tanggal Lahir: {biodataMatrix[i, 3]}");
                            Debug.WriteLine($"Jenis Kelamin: {biodataMatrix[i, 4]}");
                            Debug.WriteLine($"Golongan Darah: {biodataMatrix[i, 5]}");
                            Debug.WriteLine($"Alamat: {biodataMatrix[i, 6]}");
                            Debug.WriteLine($"Agama: {biodataMatrix[i, 7]}");
                            Debug.WriteLine($"Status Perkawinan: {biodataMatrix[i, 8]}");
                            Debug.WriteLine($"Pekerjaan: {biodataMatrix[i, 9]}");
                            Debug.WriteLine($"Kewarganegaraan: {biodataMatrix[i, 10]}");
                            Debug.WriteLine($"Kemiripan Fingerprint: {resultList[1]}");
                            Debug.WriteLine($"Kemiripan Nama: {similarity} %");
                            Debug.WriteLine($"Path image : {resultList[2]}");
                            Debug.WriteLine(this.solutionsValid.ToString());
                            this.solutionsValid[co, 0] = biodataMatrix[i, 0];
                            this.solutionsValid[co, 1] = resultList[0];
                            this.solutionsValid[co, 2] = biodataMatrix[i, 2];
                            this.solutionsValid[co, 3] = biodataMatrix[i, 3];
                            this.solutionsValid[co, 4] = biodataMatrix[i, 4];
                            this.solutionsValid[co, 5] = biodataMatrix[i, 5];
                            this.solutionsValid[co, 6] = biodataMatrix[i, 6];
                            this.solutionsValid[co, 7] = biodataMatrix[i, 7];
                            this.solutionsValid[co, 8] = biodataMatrix[i, 8];
                            this.solutionsValid[co, 9] = biodataMatrix[i, 9];
                            this.solutionsValid[co, 10] = biodataMatrix[i, 10];
                            this.solutionsValid[co, 11] = resultList[1];
                            this.solutionsValid[co, 12] = resultList[2];
                            co++;
                           
                        }
                    }
                    if (co > 0){
                        this.solutionsValid[0, 0] = this.solutionsValid[co-1, 0] ;
                        this.solutionsValid[0, 1] = this.solutionsValid[co-1, 1] ;
                        this.solutionsValid[0, 2] = this.solutionsValid[co-1, 2] ;
                        this.solutionsValid[0, 3] = this.solutionsValid[co-1, 3] ;
                        this.solutionsValid[0, 4] = this.solutionsValid[co-1, 4] ;
                        this.solutionsValid[0, 5] = this.solutionsValid[co-1, 5] ;
                        this.solutionsValid[0, 6] = this.solutionsValid[co-1, 6] ;
                        this.solutionsValid[0, 7] = this.solutionsValid[co-1, 7] ;
                        this.solutionsValid[0, 8] = this.solutionsValid[co-1, 8] ;
                        this.solutionsValid[0, 9] = this.solutionsValid[co-1, 9] ;
                        this.solutionsValid[0, 10] =this.solutionsValid[co-1, 10];
                        this.solutionsValid[0, 11] =this.solutionsValid[co-1, 11];
                        this.solutionsValid[0, 12] =this.solutionsValid[co-1, 12];
                    }

               
                }
                timer.Stop();
                if (similarNames.Count == 0)
                {
                    Debug.WriteLine("Tidak ada nama yang mirip.");
                }
                Debug.WriteLine("==============================================================");

            }
            long exactMatchTime = timer.ElapsedMilliseconds;
            timeNeeded = exactMatchTime;
        }

    }

    //public static void Main(string[] args){
    //    // Membuat instance dari kelas Program
    //    Program programInstance = new Program();
    //    double similarity_min = 80.0;

    //    string[,] biodataMatrix = Database.RetrieveBiodataMatrix();
    //    string[,] sidikJariMatrix = Database.RetrieveSidikJariMatrix();

    //    Stopwatch timer = new Stopwatch();
    //    while (true){
    //        programInstance.solutionsValid=null;
    //        programInstance.algoChoose=null;
    //        programInstance.setImageQuerry = null;
    //        programInstance.timeNeeded=0;

    //        Console.WriteLine("Masukkan Filepath dari query image (atau ketik 'exit' untuk keluar):");
    //        string queryImagePath = Console.ReadLine();
    //        if (string.IsNullOrEmpty(queryImagePath))
    //        {
    //            Console.WriteLine("Input tidak valid.");
    //            continue;
    //        }
    //        if (queryImagePath.ToLower() == "exit")
    //        {
    //            break;
    //        }


    //        //
    //        programInstance.setImageQuerry= queryImagePath;
    //        //


    //        string queryBinary;
    //        try
    //        {
    //            queryBinary = Algorithm.ProcessImage1(queryImagePath);
    //        }

    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Gagal membaca gambar: {ex.Message}");
    //            continue;
    //        }
    //        Console.WriteLine("Pilih algoritma pencocokan (1 untuk Boyer-Moore, 2 untuk Knuth-Morris-Pratt):");
    //        string input = Console.ReadLine();


    //        //
    //        programInstance.algoChoose=input;
    //        //


    //        if (string.IsNullOrEmpty(input))
    //        {
    //            Console.WriteLine("Input tidak valid.");
    //            continue;
    //        }

    //        if (!int.TryParse(input, out int choice))
    //        {
    //            Console.WriteLine("Input tidak valid.");
    //            continue;
    //        }

    //        Func<string, string, int> matchAlgorithm;

    //        if (choice == 1)
    //        {
    //            matchAlgorithm = Algorithm.BMMatch;
    //            Console.WriteLine("Menggunakan algoritma Boyer-Moore.");
    //            Console.WriteLine("==============================================================");
    //        }
    //        else if (choice == 2)
    //        {
    //            matchAlgorithm = Algorithm.KMPMatch;
    //            Console.WriteLine("Menggunakan algoritma Knuth-Morris-Pratt.");
    //            Console.WriteLine("==============================================================");
    //        }
    //        else
    //        {
    //            Console.WriteLine("Pilihan tidak valid.");
    //            continue;
    //        }

    //        List<List<string>> matchingImages = new List<List<string>>();
    //        timer.Start();
    //        for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
    //        {
    //            string datasetBinary = (sidikJariMatrix[i,0]);

    //            int exactMatchIndex = matchAlgorithm(queryBinary, datasetBinary);

    //            if (exactMatchIndex != -1)
    //            {
    //                // timer.Stop();
    //                // Console.WriteLine($"Exact match ditemukan di {sidikJariMatrix[i,1]}");
    //                // Console.WriteLine($"Waktu untuk matching: {timer.ElapsedMilliseconds} ms");
    //                List<string> imageFound = new List<string>();
    //                imageFound.Add(sidikJariMatrix[i,1]);
    //                imageFound.Add("100%");
    //                imageFound.Add(sidikJariMatrix[i,2]);
    //                matchingImages.Add(imageFound);
    //            }

    //        }

    //        if(matchingImages.Count==0)
    //        {
    //         Console.WriteLine("==============================================================");
    //        Console.WriteLine("Tidak ada gambar yang exact match.\nMencari gambar menggunakan pendekatan Levenshtein!");
    //        // timer.Stop();



    //            // timer.Restart();
    //            for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
    //            {
    //                string datasetBinary = sidikJariMatrix[i,0];
    //                double similarity = Algorithm.CalculateLevenshteinSimilarity(datasetBinary, queryBinary);

    //                if (similarity > similarity_min)
    //                {
    //                    // Console.WriteLine($"Kemiripan ditemukan di {sidikJariMatrix[i,1]}");
    //                    List<string> imageFound = new List<string>();
    //                    imageFound.Add(sidikJariMatrix[i,1]);
    //                    imageFound.Add(similarity.ToString()+"%");
    //                    imageFound.Add(sidikJariMatrix[i,2]);
    //                    matchingImages.Add(imageFound);
    //                }
    //            }
    //            // timer.Stop();
    //            long levenshteinTime = timer.ElapsedMilliseconds;
    //            Console.WriteLine($"Waktu untuk Levenshtein similarity calculation: {levenshteinTime} ms");
    //        }

    //        //
    //        Console.WriteLine($"Jumlah data yang mirip: {matchingImages.Count}");
    //        programInstance.solutionsValid = new string[matchingImages.Count,13];
    //        //

    //        if (matchingImages.Count > 0)
    //        {
    //            int co=0;
    //            // Console.WriteLine($"Waktu total pencarian: {exactMatchTime} ms");
    //            Console.WriteLine($"Gambar yang mirip dengan query (lebih dari {similarity_min}%):");
    //             Console.WriteLine("==============================================================");
    //            foreach (List<string> resultList in matchingImages)
    //            {
    //                List<string[]> similarNames = new List<string[]>();
    //                for (int i = 0; i<biodataMatrix.GetLength(0);i++)
    //                {
    //                    string biodataName = biodataMatrix[i,1];
    //                    // Console.WriteLine(resultList[0]);
    //                    // Console.WriteLine(biodataName);
    //                    int matchVal = matchAlgorithm(Database.FixCorruptedName(resultList[0]),biodataName);
    //                    if (matchVal!=-1){
    //                        Console.WriteLine("==============================================================");
    //                        string[] biodataRow = new string[biodataMatrix.GetLength(1)];
    //                        for (int j = 0; j < biodataMatrix.GetLength(1); j++)
    //                        {
    //                            biodataRow[j] = biodataMatrix[i, j];
    //                        }
    //                        similarNames.Add(biodataRow);

    //                        Console.WriteLine($"NIK: {biodataMatrix[i,0]}");
    //                        Console.WriteLine($"Nama: {biodataMatrix[i,1]}");
    //                        Console.WriteLine($"Tempat Lahir: {biodataMatrix[i,2]}");
    //                        Console.WriteLine($"Tanggal Lahir: {biodataMatrix[i,3]}");
    //                        Console.WriteLine($"Jenis Kelamin: {biodataMatrix[i,4]}");
    //                        Console.WriteLine($"Golongan Darah: {biodataMatrix[i,5]}");
    //                        Console.WriteLine($"Alamat: {biodataMatrix[i,6]}");
    //                        Console.WriteLine($"Agama: {biodataMatrix[i,7]}");
    //                        Console.WriteLine($"Status Perkawinan: {biodataMatrix[i,8]}");
    //                        Console.WriteLine($"Pekerjaan: {biodataMatrix[i,9]}");
    //                        Console.WriteLine($"Kewarganegaraan: {biodataMatrix[i,10]}");
    //                        Console.WriteLine($"Kemiripan : {resultList[1]}");

    //                        Console.WriteLine($"Path image : {resultList[2]}");

    //                        programInstance.solutionsValid[co,0]=biodataMatrix[i,0];
    //                        programInstance.solutionsValid[co,1]=biodataMatrix[i,1];
    //                        programInstance.solutionsValid[co,2]=biodataMatrix[i,2];
    //                        programInstance.solutionsValid[co,3]=biodataMatrix[i,3];
    //                        programInstance.solutionsValid[co,4]=biodataMatrix[i,4];
    //                        programInstance.solutionsValid[co,5]=biodataMatrix[i,5];
    //                        programInstance.solutionsValid[co,6]=biodataMatrix[i,6];
    //                        programInstance.solutionsValid[co,7]=biodataMatrix[i,7];
    //                        programInstance.solutionsValid[co,8]=biodataMatrix[i,8];
    //                        programInstance.solutionsValid[co,9]=biodataMatrix[i,9];
    //                        programInstance.solutionsValid[co,10]=biodataMatrix[i,10];
    //                        programInstance.solutionsValid[co,11]=resultList[1];
    //                        programInstance.solutionsValid[co,12]=resultList[2];
    //                        co++;
    //                        break;
    //                    }

    //                }

    //                if(similarNames.Count==0)
    //                {
    //                Console.WriteLine($"Tidak ada nama yang exactMatch.\nMencari nama menggunakan levensthein.");
    //                    for (int i = 0; i<biodataMatrix.GetLength(0);i++)
    //                    {
    //                        string biodataName = biodataMatrix[i,1];
    //                        double similarity = Algorithm.CalculateLevenshteinSimilarity(biodataName,Database.FixCorruptedName(resultList[0]));
    //                        if (similarity > similarity_min)
    //                        {
    //                            Console.WriteLine("==============================================================");
    //                            string[] biodataRow = new string[biodataMatrix.GetLength(1)];
    //                            for (int j = 0; j < biodataMatrix.GetLength(1); j++)
    //                            {
    //                                biodataRow[j] = biodataMatrix[i, j];
    //                            }
    //                            similarNames.Add(biodataRow);
    //                            Console.WriteLine($"NIK: {biodataMatrix[i,0]}");
    //                            Console.WriteLine($"Nama: {biodataMatrix[i,1]}");
    //                            Console.WriteLine($"Tempat Lahir: {biodataMatrix[i,2]}");
    //                            Console.WriteLine($"Tanggal Lahir: {biodataMatrix[i,3]}");
    //                            Console.WriteLine($"Jenis Kelamin: {biodataMatrix[i,4]}");
    //                            Console.WriteLine($"Golongan Darah: {biodataMatrix[i,5]}");
    //                            Console.WriteLine($"Alamat: {biodataMatrix[i,6]}");
    //                            Console.WriteLine($"Agama: {biodataMatrix[i,7]}");
    //                            Console.WriteLine($"Status Perkawinan: {biodataMatrix[i,8]}");
    //                            Console.WriteLine($"Pekerjaan: {biodataMatrix[i,9]}");
    //                            Console.WriteLine($"Kewarganegaraan: {biodataMatrix[i,10]}");
    //                            Console.WriteLine($"Kemiripan Fingerprint: {resultList[1]}");
    //                            Console.WriteLine($"Kemiripan Nama: {similarity} %");
    //                            Console.WriteLine($"Path image : {resultList[2]}");
    //                            programInstance.solutionsValid[co,0]=biodataMatrix[i,0];
    //                            programInstance.solutionsValid[co,1]=biodataMatrix[i,1];
    //                            programInstance.solutionsValid[co,2]=biodataMatrix[i,2];
    //                            programInstance.solutionsValid[co,3]=biodataMatrix[i,3];
    //                            programInstance.solutionsValid[co,4]=biodataMatrix[i,4];
    //                            programInstance.solutionsValid[co,5]=biodataMatrix[i,5];
    //                            programInstance.solutionsValid[co,6]=biodataMatrix[i,6];
    //                            programInstance.solutionsValid[co,7]=biodataMatrix[i,7];
    //                            programInstance.solutionsValid[co,8]=biodataMatrix[i,8];
    //                            programInstance.solutionsValid[co,9]=biodataMatrix[i,9];
    //                            programInstance.solutionsValid[co,10]=biodataMatrix[i,10];
    //                            programInstance.solutionsValid[co,11]=resultList[1];
    //                            programInstance.solutionsValid[co,12]=resultList[2];
    //                            co++;
    //                            break;
    //                        }
    //                    }


    //                }
    //                timer.Stop();
    //                if (similarNames.Count == 0)
    //                {
    //                    Console.WriteLine("Tidak ada nama yang mirip.");
    //                }
    //                Console.WriteLine("==============================================================");

    //            }

    //        }
    //    long exactMatchTime = timer.ElapsedMilliseconds;
    //    programInstance.timeNeeded = exactMatchTime;
    //    Console.WriteLine($"Waktu total pencarian: {exactMatchTime} ms");




    //    }




    //}


}

// dataset/Real/1__M_Left_index_finger.BMP
// dataset/Altered/Altered-Hard/1__M_Left_index_finger_CR.BMP
// dataset/Altered/Altered-Easy/1__M_Left_index_finger_CR.BMP
// dataset/Real/81__F_Left_middle_finger.BMP
// Main/dataset/Altered/Altered-Hard/14__M_Right_middle_finger_Zcut.BMP
// Main/dataset/Real/600__M_Right_index_finger.BMP
// dataset/Real/28__M_Right_index_finger.BMP