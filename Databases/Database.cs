// using System;
// using MySql.Data.MySqlClient;

// namespace Database_Operation
// {
//     class SelectStatement
//     {
//         static void Main()
//         {
//             string result = Read();
//             Console.WriteLine(result);
//             Console.ReadKey();
//         }

//         static string Read()
//         {
//             string constr;
//             MySqlConnection conn; 
//             MySqlDataReader dreader; 
//             string sql, output = ""; 

//             constr = @"Server=localhost;Database=TubesStima3;User ID=root;Password=308140;";
//             conn = new MySqlConnection(constr);
//             conn.Open(); 

//             sql = "SELECT berkas_citra, nama FROM sidik_jari LIMIT 10";
//             MySqlCommand cmd = new MySqlCommand(sql, conn); 
//             dreader = cmd.ExecuteReader(); 

//             while (dreader.Read()) 
//             {
//                 output = output + dreader.GetValue(0) + " - " + dreader.GetValue(1) + "\n";
//             }

//             dreader.Close();
//             cmd.Dispose();
//             conn.Close();

//             return output;
//         }
//     }
// }
