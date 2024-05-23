// using System;
// using System.Data.SqlClient;
// using MySql.Data.MySqlClient;

// using System.IO;
// using System;
// using System.Text;
// using System.IO;
// using System.Collections.Generic;
// using SixLabors.ImageSharp;
// using SixLabors.ImageSharp.PixelFormats;
// using System.Diagnostics;
// using System.Text.RegularExpressions;

// public class Program
// {
//     private static string connectionString = "Server=localhost;Database=TubesStima3;User ID=root;Password=308140;";
//     private static Random random = new Random();
//     public static void Main(string[] args)
//     {
//         string datasetPath = "dataset/Real";
//         List<string> sidikJari = new List<string>();
//         string[] names = GenerateNamesArray();
//         foreach (string imagePath in Directory.GetFiles(datasetPath, "*.BMP"))
//         {
//             string datasetBinary = ReadFingerprintAndMatch(imagePath);
//             sidikJari.Add(datasetBinary);
//         }
//         try
//         {
//             int i =0;
//             foreach (string line in sidikJari)
//             {
//                 int methodIndex = random.Next(4);
//                 if (!string.IsNullOrWhiteSpace(line))
//                 {
//                     string randomName="";
//                     switch (methodIndex)
//                     {
//                         case 0:
//                             randomName = GenerateRandomCase(names[i]);
//                             break;
//                         case 1:
//                             randomName = ReplaceWithNumbers(names[i]);
//                             break;
//                         case 2:
//                             randomName = ShortenText(names[i]);
//                             break;
//                         case 3:
//                             randomName = CombineAll(names[i]);
//                             break;
//                         default:
//                             randomName = names[i]; // Hanya sebagai fallback, secara teori tidak akan tercapai
//                             break;
//                     }
//                     InsertIntoDatabase(randomName,names[i], line);
//                 }
//                 i++;
//             }
//             Console.WriteLine("Semua data berhasil dimasukkan ke dalam database.");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Terjadi kesalahan: {ex.Message}");
//         }
//     }

//     private static void InsertIntoDatabase(string name, string realname,string text)
//     {
//         using (MySqlConnection connection = new MySqlConnection(connectionString))
//         {
//             connection.Open();

//             // Generate random NIK using GUID
//             string randomNIK = GenerateRandomNIK();


//             string query = "INSERT INTO sidik_jari (berkas_citra, nama) VALUES (@Text, @Name)";
//             using (MySqlCommand command = new MySqlCommand(query, connection))
//             {
//                 command.Parameters.AddWithValue("@Name", name);
//                 command.Parameters.AddWithValue("@Text", text);

//                 command.ExecuteNonQuery();
//             }
            
//             // Insert into biodata table with random NIK
//             string queryBiodata = "INSERT INTO biodata (NIK, nama,tempat_lahir,tanggal_lahir,jenis_kelamin,golongan_darah,alamat,agama,status_perkawinan,pekerjaan,kewarganegaraan) VALUES (@NIK, @Name,@Tl,@Tgl,@Gen,@Goldar,@Alamat,@Agama,@Sp,@Pekerjaan,@kwn)";
//             using (MySqlCommand command = new MySqlCommand(queryBiodata, connection))
//             {
//                 command.Parameters.AddWithValue("@NIK", randomNIK);
//                 command.Parameters.AddWithValue("@Name", realname);
//                 command.Parameters.AddWithValue("@Tl", GenerateRandomTempatLahir());
//                 command.Parameters.AddWithValue("@Tgl", GenerateRandomTanggalLahir());
//                 command.Parameters.AddWithValue("@Gen", GenerateRandomJenisKelamin());
//                 command.Parameters.AddWithValue("@Goldar", GenerateRandomGolonganDarah());
//                 command.Parameters.AddWithValue("@Alamat", GenerateRandomAlamat());
//                 command.Parameters.AddWithValue("@Agama", GenerateRandomAgama());
//                 command.Parameters.AddWithValue("@Sp", GenerateRandomStatusPerkawinan());
//                 command.Parameters.AddWithValue("@Pekerjaan", GenerateRandomPekerjaan());
//                 command.Parameters.AddWithValue("@kwn", GenerateRandomKewarganegaraan());


//                 command.ExecuteNonQuery();
//             }
//         }
//     }

//     private static string[] GenerateNamesArray()
//     {
//         string[] commonNames = new string[]
//         {
//             "Budi", "Siti", "Andi", "Rina", "Dewi", "Tono", "Wati", "Agus", "Sri", "Yanto",
//             "Rudi", "Ani", "Hendra", "Maya", "Ahmad", "Fitri", "Bambang", "Nur", "Fajar", "Ayu",
//             "Eko", "Lina", "Bayu", "Nia", "Udin", "Lilis", "Joko", "Indah", "Teguh", "Rita",
//             "Heri", "Aisyah", "Adi", "Rosi", "Arif", "Mega", "Rahmat", "Putri", "Yusuf", "Lina",
//             "Dani", "Irma", "Tomi", "Fina", "Hasan", "Sari", "Roni", "Lestari", "Tari", "Amir"
//         };

//         string[] namesArray = new string[6000];
//         Random random = new Random();
        
//         for (int i = 0; i < namesArray.Length; i++)
//         {
//             StringBuilder nameBuilder = new StringBuilder();
//             int randomlen = random.Next(3);
//             for (int j = 0; j < randomlen+1; j++)
//             {
//                 if (j > 0)
//                 {
//                     nameBuilder.Append(" ");
//                 }
//                 nameBuilder.Append(commonNames[random.Next(commonNames.Length)]);
//             }
            
//             namesArray[i] = nameBuilder.ToString();
//         }

//         return namesArray;
//     }


//     public static string ReadFingerprintAndMatch(string imagePath)
//     {
//          using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
//         {
//             StringBuilder binaryStringBuilder = new StringBuilder();
//             int requiredPixels = 24;
//             int rows = (int)Math.Sqrt(requiredPixels); // 4 rows
//             int cols = requiredPixels / rows; // 6 cols
//             int gridWidth = image.Width / cols;
//             int gridHeight = image.Height / rows;

//             for (int row = 0; row < rows; row++)
//             {
//                 for (int col = 0; col < cols; col++)
//                 {
//                     int x = col * gridWidth + gridWidth / 2;
//                     int y = row * gridHeight + gridHeight / 2;
//                     x = Math.Min(x, image.Width - 1);
//                     y = Math.Min(y, image.Height - 1);
//                     Rgba32 pixelColor = image[x, y];
//                     int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
//                     string binary = Convert.ToString(grayValue, 2).PadLeft(8, '0');
//                     binaryStringBuilder.Append(binary);
//                 }
//             }

//             return BinaryArrayToAscii(binaryStringBuilder.ToString());
//         }
//     }
//         private static string GenerateRandomCase(string input)
//     {
//         StringBuilder result = new StringBuilder();
//         foreach (char c in input)
//         {
//             if (char.IsLetter(c))
//             {
//                 result.Append(random.Next(2) == 0 ? char.ToLower(c) : char.ToUpper(c));
//             }
//             else
//             {
//                 result.Append(c);
//             }
//         }
//         return result.ToString();
//     }

//     private static string ReplaceWithNumbers(string input)
//     {
//         StringBuilder result = new StringBuilder();
//         foreach (char c in input)
//         {
//             switch (char.ToLower(c))
//             {
//                 case 'a': result.Append('4'); break;
//                 case 'e': result.Append('3'); break;
//                 case 'i': result.Append('1'); break;
//                 case 'o': result.Append('0'); break;
//                 case 'u': result.Append('u'); break;
//                 case 'b': result.Append('8'); break;
//                 case 't': result.Append('7'); break;
//                 case 's': result.Append('5'); break;
//                 default: result.Append(c); break;
//             }
//         }
//         return result.ToString();
//     }

//     private static string ShortenText(string input)
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

//     private static string CombineAll(string input)
//     {
//         string shortened = ShortenText(input);
//         string mixedCase = GenerateRandomCase(shortened);
//         return ReplaceWithNumbers(mixedCase);
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
//     private static string GenerateRandomNIK()
//     {
//         string guid = Guid.NewGuid().ToString("N");
//         string numbersOnly = Regex.Replace(guid, "[^0-9]", "");
//         while (numbersOnly.Length < 16)
//         {
//             numbersOnly += Regex.Replace(Guid.NewGuid().ToString("N"), "[^0-9]", "");
//         }

//         return numbersOnly.Substring(0, 16);
//     }

//     public static string GenerateRandomTanggalLahir()
// {
//     DateTime start = new DateTime(1950, 1, 1);
//     int range = (DateTime.Today - start).Days;
//     return start.AddDays(random.Next(range)).ToString("yyyy-MM-dd");
// }

// public static string GenerateRandomJenisKelamin()
// {
//     return random.Next(2) == 0 ? "Laki-Laki" : "Perempuan";
// }

// public static string GenerateRandomGolonganDarah()
// {
//     string[] golonganDarah = { "A", "B", "AB", "O" };
//     return golonganDarah[random.Next(golonganDarah.Length)] + (random.Next(2) == 0 ? "+" : "-");
// }

// public static string GenerateRandomAlamat()
// {
//     string[] KotaDiIndonesia = new string[]
//     {
//         "Jakarta", "Surabaya", "Bandung", "Medan", "Semarang", "Makassar", "Palembang",
//         "Bandar Lampung", "Padang", "Tangerang", "Depok", "Batam", "Bekasi", "Pekanbaru",
//         "Bogor", "Malang", "Surakarta", "Yogyakarta", "Cimahi", "Balikpapan"
//     };

//     return $"Jl. {random.Next(1000)} {KotaDiIndonesia[random.Next(KotaDiIndonesia.Length)]}";
// }

// public static string GenerateRandomAgama()
// {
//     string[] AgamaList = new string[]
//     {
//         "Islam", "Kristen Protestan", "Katolik", "Hindu", "Buddha", "Konghucu"
//     };
//     return AgamaList[random.Next(AgamaList.Length)];
// }

// public static string GenerateRandomStatusPerkawinan()
// {
//     String[] StatusPerkawinanList = new string[]
//     {
//         "Belum Menikah", "Menikah", "Cerai"
//     };
    
//     return StatusPerkawinanList[random.Next(StatusPerkawinanList.Length)];
// }

// public static string GenerateRandomPekerjaan()
// {
//     string[] pekerjaanList = new string[]
//     {
//         "PNS", "Dokter", "Guru", "Wiraswasta", "Pengusaha", "Programmer",
//         "Petani", "Polisi", "Pilot", "Desainer", "Akuntan", "Penyiar", "Arsitek",
//         "Konsultan", "Insinyur", "Wartawan", "Notaris", "Perawat", "Pramugari",
//         "Pramugara", "Pengacara", "Psikolog", "Dosen", "Mahasiswa", "Pensiunan"
//     };
//     return pekerjaanList[random.Next(pekerjaanList.Length)];
// }

// public static string GenerateRandomKewarganegaraan()
// {
//     string[] kewarganegaraanList = new string[]
//     {
//         "Warga Negara Indonesia", "Warga Negara Asing"
//     };
//     return kewarganegaraanList[random.Next(kewarganegaraanList.Length)];
// }
// public static string GenerateRandomTempatLahir()
// {
//     string[] kotaList = new string[]
//     {
//         "Jakarta", "Surabaya", "Bandung", "Medan", "Semarang", "Makassar",
//         "Palembang", "Depok", "Tangerang", "Bekasi", "Padang", "Bogor",
//         "Bandar Lampung", "Batam", "Pekanbaru", "Denpasar", "Surakarta",
//         "Banjarmasin", "Samarinda", "Malang", "Yogyakarta", "Cimahi", "Balikpapan",
//         "Jambi", "Pontianak", "Padang", "Ambon", "Manado", "Mataram", "Palu", "Banda Aceh",
//         "Ternate", "Sorong", "Pangkal Pinang", "Lhokseumawe", "Binjai", "Tarakan", "Tanjungpinang",
//         "Padangsidimpuan", "Parepare", "Tebing Tinggi", "Langsa", "Bima", "Subulussalam", "Pekalongan"
//     };
//     return kotaList[random.Next(kotaList.Length)];
// }



// }


