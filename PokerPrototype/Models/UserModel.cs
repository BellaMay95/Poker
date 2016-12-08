using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerPrototype.Models
{
    public class UserModel
    {
        public string username { get; set; }
        public int currency { get; set; }
        public string avatar { get; set; }
        public string isAdmin { get; set; }
        private string email { get; set; }
        public UserModel(int id)
        {
            try {
                isAdmin = "0";
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "SELECT username,currency,avatar,email,isAdmin FROM users WHERE id = @id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    username = rdr[0].ToString();
                    currency = Convert.ToInt32(rdr[1]);
                    avatar = rdr[2].ToString();
                    if (avatar.Length == 0)
                    {
                        avatar = Avatar.Str;
                    }
                    email = rdr[3].ToString();
                    isAdmin = rdr[4].ToString();
                }
                else
                {
                    username = "DEBUGGING MODE";
                    currency = 0;
                    avatar = "";
                }
                Conn.Close();
            } catch(Exception e)
            {

            }
        }
        public string getEmail()
        {
            return email;
        }
    }
}