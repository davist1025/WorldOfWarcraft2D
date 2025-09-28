using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.DevKit
{
    /// <summary>
    /// Global handler for mysql management (connection, cmd execution, etc)
    /// </summary>
    public class MySQLManager
    {
        private static MySqlConnection _mySqlConnection;
        private static string _initialConnectionString = "server={0};uid={1};pwd={2};database=wpp_auth";

        public static bool IsValid(string server = "127.0.0.1", string uid = "admin", string pwd = "1111", string database = "wpp_auth")
        {
            bool isValid = false;

            try
            {
                _mySqlConnection = new MySqlConnection();
                _mySqlConnection.ConnectionString = string.Format(_initialConnectionString, server, uid, pwd, database);
                _mySqlConnection.Open();
                isValid = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured opening a connection to the database: {ex.Message} ({ex.StackTrace})");
            }

            return isValid;
        }

        public static string[] QueryDatabases()
        {
            MySqlCommand allDbCmd = _mySqlConnection.CreateCommand();
            allDbCmd.CommandText = "show databases;";
            List<string> databaseNames = new List<string>();

            try
            {
                MySqlDataReader reader = allDbCmd.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        databaseNames.Add(reader.GetValue(i).ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to gather database information: {ex.Message} ({ex.StackTrace})");
            }

            return databaseNames.ToArray();
        }

        public static void Closeout()
        {
            _mySqlConnection.Close();
            Debug.WriteLine("Closing mysql...");
        }
    }
}
