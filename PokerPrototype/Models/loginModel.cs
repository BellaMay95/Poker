using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Web.Helpers;

namespace PokerPrototype.Models
{
    public class LoginModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public int id { get; set; }
        public string usernameError { get; set; }
        public string passwordError { get; set; }
        public LoginModel(string username, string password)
        {
            id = 0;
            usernameError = passwordError = "";
            if (password.Length == 0)
            {
                passwordError = "Enter a password";
            }
            if (username.Length == 0)
            {
                usernameError = "Enter a username";
            }
            else if (password.Length > 0)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection(Connection.Str);
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "SELECT id, password, disabled FROM users WHERE username = @user";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@user", username);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read() && Convert.ToInt32(rdr[2]) == 1)
                    {
                        passwordError = "Your account has been disabled. An administrator will contact you with instructions for reactivating your account";
                    }
                    else if (Crypto.VerifyHashedPassword(rdr[1].ToString(), password))
                    {
                        id = Convert.ToInt32(rdr[0]);

                    } else
                    {
                        passwordError = "Username and password didn't match";
                    }
                    Conn.Close();
                }
                catch (Exception ex)
                {
                    passwordError = ex.Message;
                }
            }
        }
    }
}