using System;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

using System.IO;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

public class Program
{
     public static string connectionString = "Server=localhost;Database=TubesStima3;Uid=root;Pwd=220504;";


    private static Random random = new Random();
   
        public static void Main(string[] args)
    {
        string datasetPath = "dataset/Real"; 
        string[] names = GenerateNamesArray();
        Dictionary<int, List<string>> sidikJari = new Dictionary<int, List<string>>();

        foreach (string imagePath in Directory.GetFiles(datasetPath, "*.BMP"))
        {
            string fileName = Path.GetFileName(imagePath);
            int underscoreIndex = fileName.IndexOf('_');
            if (underscoreIndex > 0)
            {
                string numberPart = fileName.Substring(0, underscoreIndex);
                if (int.TryParse(numberPart, out int number))
                {
                    string datasetBinary = ProcessImage(imagePath);

                    if (!sidikJari.ContainsKey(number))
                    {
                        sidikJari[number] = new List<string>();
                    }

                    sidikJari[number].Add(datasetBinary);
                    sidikJari[number].Add(imagePath);
                }
            }
        }

        try
        {
            Random random = new Random();
            int i = 0;
            foreach (var entry in sidikJari)
            {
                List<string> line = entry.Value;
                int methodIndex = random.Next(4);
                if (!string.IsNullOrWhiteSpace(line[0]))
                {
                    string randomName = "";
                    switch (methodIndex)
                    {
                        case 0:
                            randomName = GenerateRandomCase(names[i]);
                            break;
                        case 1:
                            randomName = ReplaceWithNumbers(names[i]);
                            break;
                        case 2:
                            randomName = ShortenText(names[i]);
                            break;
                        case 3:
                            randomName = CombineAll(names[i]);
                            break;
                        default:
                            randomName = names[i]; 
                            break;
                    }
                    for (int j = 0; j < line.Count; j += 2)
                    {
                        InsertIntoDatabase(names[i], line[j], line[j + 1]);
                    }
                    InsertIntoDatabaseBiodata(randomName);
                }
                i++;
            }
            Console.WriteLine("Semua data berhasil dimasukkan ke dalam database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Terjadi kesalahan: {ex.Message}");
        }
    }
    private static void InsertIntoDatabaseBiodata(string realname){
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string randomNIK = GenerateRandomNIK();
            string queryBiodata = "INSERT INTO biodata (NIK, nama,tempat_lahir,tanggal_lahir,jenis_kelamin,golongan_darah,alamat,agama,status_perkawinan,pekerjaan,kewarganegaraan) VALUES (@NIK, @Name,@Tl,@Tgl,@Gen,@Goldar,@Alamat,@Agama,@Sp,@Pekerjaan,@kwn)";
            using (MySqlCommand command = new MySqlCommand(queryBiodata, connection))
            {
                command.Parameters.AddWithValue("@NIK", randomNIK);
                command.Parameters.AddWithValue("@Name", RSADecryption(realname, isEncrypting: true));
                command.Parameters.AddWithValue("@Tl", RSADecryption(GenerateRandomTempatLahir(), isEncrypting: true));
                command.Parameters.AddWithValue("@Tgl", GenerateRandomTanggalLahir());
                command.Parameters.AddWithValue("@Gen", GenerateRandomJenisKelamin());
                command.Parameters.AddWithValue("@Goldar", RSADecryption(GenerateRandomGolonganDarah(), isEncrypting: true));
                command.Parameters.AddWithValue("@Alamat", RSADecryption(GenerateRandomAlamat(), isEncrypting: true));
                command.Parameters.AddWithValue("@Agama", RSADecryption(GenerateRandomAgama(), isEncrypting: true));
                command.Parameters.AddWithValue("@Sp", GenerateRandomStatusPerkawinan());
                command.Parameters.AddWithValue("@Pekerjaan", RSADecryption(GenerateRandomPekerjaan(), isEncrypting: true));
                command.Parameters.AddWithValue("@kwn", RSADecryption(GenerateRandomKewarganegaraan(), isEncrypting: true));


                command.ExecuteNonQuery();
            }
        }
    }

    private static void InsertIntoDatabase(string name,string text,string path)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string randomNIK = GenerateRandomNIK();
            string query = "INSERT INTO sidik_jari (berkas_citra, nama, path_image) VALUES (@Text, @Name, @Path)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Text", text);
                command.Parameters.AddWithValue("@Path", path);
                command.ExecuteNonQuery();
            }
        }
    }

    private static string[] GenerateNamesArray()
{
    string[] commonNames = new string[]
    {
        "Budi", "Siti", "Andi", "Rina", "Dewi", "Tono", "Wati", "Agus", "Sri", "Yanto",
        "Rudi", "Ani", "Hendra", "Maya", "Ahmad", "Fitri", "Bambang", "Nur", "Fajar", "Ayu",
        "Eko", "Lina", "Bayu", "Nia", "Udin", "Lilis", "Joko", "Indah", "Teguh", "Rita",
        "Heri", "Aisyah", "Adi", "Rosi", "Arif", "Mega", "Rahmat", "Putri", "Yusuf", "Lina",
        "Dani", "Irma", "Tomi", "Fina", "Hasan", "Sari", "Roni", "Lestari", "Tari", "Amir"
    };

    string[] namesArray = new string[6000];
    HashSet<string> namesSet = new HashSet<string>();
    Random random = new Random();
    
    for (int i = 0; i < namesArray.Length; i++)
    {
        StringBuilder nameBuilder = new StringBuilder();
        int randomlen = random.Next(3);
        for (int j = 0; j < randomlen + 1; j++)
        {
            if (j > 0)
            {
                nameBuilder.Append(" ");
            }
            nameBuilder.Append(commonNames[random.Next(commonNames.Length)]);
        }
        
        string name = nameBuilder.ToString();
        while (namesSet.Contains(name))
        {
            nameBuilder.Clear();
            randomlen = random.Next(3);
            for (int j = 0; j < randomlen + 1; j++)
            {
                if (j > 0)
                {
                    nameBuilder.Append(" ");
                }
                nameBuilder.Append(commonNames[random.Next(commonNames.Length)]);
            }
            name = nameBuilder.ToString();
        }

        namesSet.Add(name);
        namesArray[i] = name;
    }

    return namesArray;
}



   public static string ProcessImage(string imagePath)
{
    using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
    {
        StringBuilder binaryStringBuilder = new StringBuilder();
        image.Mutate(x => x.Resize(90, 100));
        image.Mutate(x => x.Grayscale());
        image.Mutate(x => x.HistogramEqualization());
        int threshold = CalculateOtsuThreshold(image);
        Debug.WriteLine(threshold);
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Rgba32 pixelColor = image[x, y];
                int grayValue = pixelColor.R; 
                binaryStringBuilder.Append(grayValue >= threshold ? '1' : '0');
            }
        }

        return BinaryStringToAscii(binaryStringBuilder.ToString());
    }
}

private static int CalculateOtsuThreshold(Image<Rgba32> image)
{
    int[] histogram = new int[256];
    for (int y = 0; y < image.Height; y++)
    {
        for (int x = 0; x < image.Width; x++)
        {
            Rgba32 pixelColor = image[x, y];
            histogram[pixelColor.R]++;
        }
    }

    int totalPixels = image.Width * image.Height;
    float sum = 0;
    for (int i = 0; i < 256; i++)
    {
        sum += i * histogram[i];
    }

    float sumB = 0;
    int wB = 0;
    int wF = 0;
    float varMax = 0;
    int threshold = 0;

    for (int t = 0; t < 256; t++)
    {
        wB += histogram[t];
        if (wB == 0) continue;

        wF = totalPixels - wB;
        if (wF == 0) break;

        sumB += t * histogram[t];

        float mB = sumB / wB;
        float mF = (sum - sumB) / wF;

        float varBetween = wB * wF * (mB - mF) * (mB - mF);

        if (varBetween > varMax)
        {
            varMax = varBetween;
            threshold = t;
        }
    }

    return threshold;
}


    private static string BinaryStringToAscii(string binaryString)
    {
        StringBuilder asciiStringBuilder = new StringBuilder();
        int len = binaryString.Length;
        if (len%8 != 0){
            len = len - binaryString.Length%8;
        }
        for (int i = 0; i < len; i += 8)
        {
            string byteString = binaryString.Substring(i, 8);
            byte byteValue = Convert.ToByte(byteString, 2);
            asciiStringBuilder.Append((char)byteValue);
        }

        return asciiStringBuilder.ToString();
    }

        private static string GenerateRandomCase(string input)
    {
        StringBuilder result = new StringBuilder();
        foreach (char c in input)
        {
            if (char.IsLetter(c))
            {
                result.Append(random.Next(2) == 0 ? char.ToLower(c) : char.ToUpper(c));
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }

    private static string ReplaceWithNumbers(string input)
    {
        StringBuilder result = new StringBuilder();
        foreach (char c in input)
        {
            switch (char.ToLower(c))
            {
                case 'a': result.Append('4'); break;
                case 'e': result.Append('3'); break;
                case 'i': result.Append('1'); break;
                case 'o': result.Append('0'); break;
                case 'u': result.Append('u'); break;
                case 'b': result.Append('8'); break;
                case 't': result.Append('7'); break;
                case 's': result.Append('5'); break;
                default: result.Append(c); break;
            }
        }
        return result.ToString();
    }

    private static string ShortenText(string input)
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

    private static string CombineAll(string input)
    {
        string shortened = ShortenText(input);
        string mixedCase = GenerateRandomCase(shortened);
        return ReplaceWithNumbers(mixedCase);
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
    private static string GenerateRandomNIK()
    {
        string guid = Guid.NewGuid().ToString("N");
        string numbersOnly = Regex.Replace(guid, "[^0-9]", "");
        while (numbersOnly.Length < 16)
        {
            numbersOnly += Regex.Replace(Guid.NewGuid().ToString("N"), "[^0-9]", "");
        }

        return numbersOnly.Substring(0, 16);
    }

    public static string GenerateRandomTanggalLahir()
{
    DateTime start = new DateTime(1950, 1, 1);
    int range = (DateTime.Today - start).Days;
    return start.AddDays(random.Next(range)).ToString("yyyy-MM-dd");
}

public static string GenerateRandomJenisKelamin()
{
    return random.Next(2) == 0 ? "Laki-Laki" : "Perempuan";
}

public static string GenerateRandomGolonganDarah()
{
    string[] golonganDarah = { "A", "B", "AB", "O" };
    return golonganDarah[random.Next(golonganDarah.Length)] + (random.Next(2) == 0 ? "+" : "-");
}

public static string GenerateRandomAlamat()
{
    string[] KotaDiIndonesia = new string[]
    {
        "Jakarta", "Surabaya", "Bandung", "Medan", "Semarang", "Makassar", "Palembang",
        "Bandar Lampung", "Padang", "Tangerang", "Depok", "Batam", "Bekasi", "Pekanbaru",
        "Bogor", "Malang", "Surakarta", "Yogyakarta", "Cimahi", "Balikpapan"
    };

    return $"Jl. {random.Next(1000)} {KotaDiIndonesia[random.Next(KotaDiIndonesia.Length)]}";
}

public static string GenerateRandomAgama()
{
    string[] AgamaList = new string[]
    {
        "Islam", "Kristen Protestan", "Katolik", "Hindu", "Buddha", "Konghucu"
    };
    return AgamaList[random.Next(AgamaList.Length)];
}

public static string GenerateRandomStatusPerkawinan()
{
    String[] StatusPerkawinanList = new string[]
    {
        "Belum Menikah", "Menikah", "Cerai"
    };
    
    return StatusPerkawinanList[random.Next(StatusPerkawinanList.Length)];
}

public static string GenerateRandomPekerjaan()
{
    string[] pekerjaanList = new string[]
    {
        "PNS", "Dokter", "Guru", "Wiraswasta", "Pengusaha", "Programmer",
        "Petani", "Polisi", "Pilot", "Desainer", "Akuntan", "Penyiar", "Arsitek",
        "Konsultan", "Insinyur", "Wartawan", "Notaris", "Perawat", "Pramugari",
        "Pramugara", "Pengacara", "Psikolog", "Dosen", "Mahasiswa", "Pensiunan"
    };
    return pekerjaanList[random.Next(pekerjaanList.Length)];
}

public static string GenerateRandomKewarganegaraan()
{
    string[] kewarganegaraanList = new string[]
    {
        "Warga Negara Indonesia", "Warga Negara Asing"
    };
    return kewarganegaraanList[random.Next(kewarganegaraanList.Length)];
}
public static string GenerateRandomTempatLahir()
{
    string[] kotaList = new string[]
    {
        "Jakarta", "Surabaya", "Bandung", "Medan", "Semarang", "Makassar",
        "Palembang", "Depok", "Tangerang", "Bekasi", "Padang", "Bogor",
        "Bandar Lampung", "Batam", "Pekanbaru", "Denpasar", "Surakarta",
        "Banjarmasin", "Samarinda", "Malang", "Yogyakarta", "Cimahi", "Balikpapan",
        "Jambi", "Pontianak", "Padang", "Ambon", "Manado", "Mataram", "Palu", "Banda Aceh",
        "Ternate", "Sorong", "Pangkal Pinang", "Lhokseumawe", "Binjai", "Tarakan", "Tanjungpinang",
        "Padangsidimpuan", "Parepare", "Tebing Tinggi", "Langsa", "Bima", "Subulussalam", "Pekalongan"
    };
    return kotaList[random.Next(kotaList.Length)];
}

public static string RSADecryption(string plaintext, bool isEncrypting)
    {
        int private_key = 39653; //1019; //d
        int public_key = 65537; //79; //e
        int p =  211; //47; //bebas berapa
        int q = 223; //71; //bebas berapa
        int n = p * q; //3337
        int m = (p - 1) * (q - 1); //46620 //3220
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


