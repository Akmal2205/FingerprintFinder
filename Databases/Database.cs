// using System;
// using System.Text;
// using System.Collections.Generic;
// using MySql.Data.MySqlClient;
// using System.Text.RegularExpressions;

// public class Program
// {
//     private static string connectionString = "Server=localhost;Database=TubesStima3;Uid=root;Pwd=308140;";
    
//     public static string[,] RetrieveBiodataMatrix()
//     {
//         string[,] biodataMatrix = new string[5, 11]; // Maksimal 5 baris dan 11 kolom

//         try
//         {
//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 connection.Open();

//                 string query = "SELECT * FROM biodata LIMIT 5";

//                 MySqlCommand command = new MySqlCommand(query, connection);
//                 MySqlDataReader reader = command.ExecuteReader();

//                 int row = 0;
//                 while (reader.Read() && row < 5)
//                 {
//                     biodataMatrix[row, 0] = reader.GetString("NIK");
//                     biodataMatrix[row, 1] = ShortenText(reader.GetString("nama"));
//                     biodataMatrix[row, 2] = reader.GetString("tempat_lahir");
//                     biodataMatrix[row, 3] = reader.GetDateTime("tanggal_lahir").ToString("yyyy-MM-dd");
//                     biodataMatrix[row, 4] = reader.GetString("jenis_kelamin");
//                     biodataMatrix[row, 5] = reader.GetString("golongan_darah");
//                     biodataMatrix[row, 6] = reader.GetString("alamat");
//                     biodataMatrix[row, 7] = reader.GetString("agama");
//                     biodataMatrix[row, 8] = reader.GetString("status_perkawinan");
//                     biodataMatrix[row, 9] = reader.GetString("pekerjaan");
//                     biodataMatrix[row, 10] = reader.GetString("kewarganegaraan");

//                     row++;
//                 }

//                 reader.Close();
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error: {ex.Message}");
//         }

//         return biodataMatrix;
//     }

//     public static string[,] RetrieveSidikJariMatrix()
//     {
//         string[,] sidikJariMatrix = new string[20, 2]; // Maksimal 5 baris dan 2 kolom

//         try
//         {
//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 connection.Open();

//                 string query = "SELECT * FROM sidik_jari LIMIT 20";

//                 MySqlCommand command = new MySqlCommand(query, connection);
//                 MySqlDataReader reader = command.ExecuteReader();

//                 int row = 0;
//                 while (reader.Read() && row < 20)
//                 {
//                     sidikJariMatrix[row, 0] = reader.GetString("berkas_citra");
//                     sidikJariMatrix[row, 1] = FixCorruptedName(reader.GetString("nama"));

//                     row++;
//                 }

//                 reader.Close();
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error: {ex.Message}");
//         }

//         return sidikJariMatrix;
//     }
//     public static int[] FindIndex(string[,] matrix, string value)
//     {
//         int[] index = {-1, -1}; // Inisialisasi dengan nilai -1 jika nilai tidak ditemukan
        
//         // Loop melalui setiap elemen matriks
//         for (int i = 0; i < matrix.GetLength(0); i++)
//         {
//             for (int j = 0; j < matrix.GetLength(1); j++)
//             {
//                 // Jika nilai pada elemen matriks sama dengan nilai yang dicari
//                 if (matrix[i, j] == value)
//                 {
//                     index[0] = i; // Set indeks baris
//                     index[1] = j; // Set indeks kolom
//                     return index; // Kembalikan indeks
//                 }
//             }
//         }
        
//         return index; // Kembalikan -1 jika nilai tidak ditemukan
//     }
//     public static string FixCorruptedName(string corruptedName)
//     {
//         return FixUpperLower(FixNumber((corruptedName)));
//     }

//     private static string FixNumber(string corruptedName)
//     {
//         // Regex pattern to match number
//         string pattern = "[01234567]";

//         // Transform each match
//         string result = Regex.Replace(corruptedName, pattern, NumberToChar);

//         return result;
//     }

//     private static string NumberToChar(Match number)
//     {
//         switch (number.Value)
//         {
//             case "0":
//                 return "o";
//             case "1":
//                 return "i";
//             case "2":
//                 return "z";
//             case "3":
//                 return "e";
//             case "4":
//                 return "a";
//             case "5":
//                 return "s";
//             case "6":
//                 return "g";
//             case "7":
//                 return "t";
//             default:
//                 return number.Value;
//         }
//     }

//     private static string FixUpperLower(string input)
//     {
//         // Regex pattern to match each word
//         string pattern = @"\w+";

//         // Transform each match
//         string result = Regex.Replace(input, pattern, new MatchEvaluator(CapitalizeWord));

//         return result;
//     }

//     private static string CapitalizeWord(Match word)
//     {
//         string wordValue = word.Value;
//         if (wordValue.Length > 1)
//         {
//             return char.ToUpper(wordValue[0]) + wordValue.Substring(1).ToLower();
//         }
//         else
//         {
//             return wordValue.ToUpper();
//         }
//     }


//       private static string ShortenText(string input)
//     {
//         StringBuilder result = new StringBuilder();
//         string[] words = input.Split(' ');
//         foreach (string word in words)
//         {
//             for (int i = 0; i < word.Length; i++)
//             {
//                 if (i == 0 || (i == word.Length - 1))
//                 {
//                     result.Append(word[i]);
//                 }
//                 else if (!"aeiou".Contains(char.ToLower(word[i])))
//                 {
//                     result.Append(word[i]);
//                 }
//             }
//             result.Append(' ');
//         }
//         return result.ToString().Trim();
//     }
//     public static void FindMatchingNames(string[,] biodataMatrix, string[,] sidikJariMatrix)
// {
//     HashSet<string> biodataNames = new HashSet<string>();
//     HashSet<string> sidikJariNames = new HashSet<string>();

//     // Ambil nama dari biodataMatrix
//     for (int i = 0; i < biodataMatrix.GetLength(0); i++)
//     {
//         biodataNames.Add(biodataMatrix[i, 1]); // Kolom 1 berisi nama
//     }

//     // Ambil nama dari sidikJariMatrix
//     for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
//     {
//         sidikJariNames.Add(sidikJariMatrix[i, 1]); // Kolom 1 berisi nama
//     }

//     // Cari nama yang sama di antara kedua HashSet
//     HashSet<string> matchingNames = new HashSet<string>(biodataNames);
//     matchingNames.IntersectWith(sidikJariNames);

//     // Jika ada nama yang sama, tampilkan
//     if (matchingNames.Count > 0)
//     {
//         Console.WriteLine("Nama yang sama ditemukan:");
//         foreach (string name in matchingNames)
//         {
//             Console.WriteLine(name);
//         }
//     }
//     else
//     {
//         Console.WriteLine("Tidak ada nama yang sama.");
//     }
// }


//     public static void Main(string[] args)
//     {
//         // Contoh penggunaan
//         string[,] biodataMatrix = RetrieveBiodataMatrix();
//         string[,] sidikJariMatrix = RetrieveSidikJariMatrix();

//         // Tampilkan hasil biodata
//         Console.WriteLine("Biodata:");
//         for (int i = 0; i < biodataMatrix.GetLength(0); i++)
//         {
//             for (int j = 0; j < biodataMatrix.GetLength(1); j++)
//             {
//                 Console.Write(biodataMatrix[i, j] + "\t");
//             }
//             Console.WriteLine();
//         }

//         // Tampilkan hasil sidik jari
//         Console.WriteLine("\nSidik Jari:");
//         for (int i = 0; i < sidikJariMatrix.GetLength(0); i++)
//         {
//             for (int j = 0; j < sidikJariMatrix.GetLength(1); j++)
//             {
//                 Console.Write(sidikJariMatrix[i, j] + "\t");
//             }
//             Console.WriteLine();
//         }
//         string nilaiDicari = "Batam";
//         int[] index = FindIndex(biodataMatrix, nilaiDicari);

//         // Cek apakah nilai ditemukan
//         if (index[0] != -1 && index[1] != -1)
//         {
//             Console.WriteLine($"Nilai '{nilaiDicari}' ditemukan pada indeks [{index[0]}, {index[1]}]");
//             Console.WriteLine(biodataMatrix[index[0],1]);
//         }
//         else
//         {
//             Console.WriteLine($"Nilai '{nilaiDicari}' tidak ditemukan dalam matriks");
//         }
//         FindMatchingNames(biodataMatrix, sidikJariMatrix);
  



//     }


// }
