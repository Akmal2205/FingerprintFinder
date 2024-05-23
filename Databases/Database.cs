using System;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Database_Operation
{
    class SelectStatement
    {
        static void Main()
        {
            Read();
            Console.ReadKey();
        }

        static void Read()
        {
            string constr;
            MySqlConnection conn; 
            MySqlDataReader dreader; 
            string sql, output = ""; 

            constr = @"Server=localhost;Database=TubesStima3;User ID=root;Password=308140;";
            conn = new MySqlConnection(constr);
            conn.Open(); 

            sql = "Select berkas_citra, nama from sidik_jari limit 10";
            MySqlCommand cmd = new MySqlCommand(sql, conn); 
            dreader = cmd.ExecuteReader(); 

            while (dreader.Read()) 
            {
                output = output + dreader.GetValue(0) + " - " + dreader.GetValue(1) + "\n";
            }

            Console.Write(output); 

            dreader.Close();
            cmd.Dispose();
            conn.Close();
        }
    }
}
