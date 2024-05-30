using System;
using System.Text;




public class Encryption
{
    public static void Main()
    {   
        string plaintext = "GG ez 17 bro";
        string encrypted = RSADecryption(plaintext, isEncrypting: true);
        Console.WriteLine("Encrypted: " + encrypted);
        string decrypted = RSADecryption(encrypted, isEncrypting: false);
        Console.WriteLine("Decrypted: " + decrypted);
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
