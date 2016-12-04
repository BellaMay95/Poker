using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Web.Helpers;
using System.Text.RegularExpressions;

namespace PokerPrototype.Models
{
    public class RegisterModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public int id { get; set; }
        public string usernameError { get; set; }
        public string passwordError { get; set; }
        public string confirmError { get; set; }
        public string emailError { get; set; }
        public RegisterModel(string email, string username, string password, string confirm)
        {
            id = 0;
            bool success = true;
            //NEED TO ADD ERRORS FOR OTHER FIELDS
            usernameError = emailError = passwordError = confirmError = "";
            if (username.Length == 0)
            {
                success = false;
                usernameError = "Enter a username";
            }
            else if (username.Length < 2)
            {
                success = false;
                usernameError = "Username too short";
            }
            else if (!Regex.IsMatch(username, @"^[A-Za-z0-9_.-~]+$"))
            {
                success = false;
                usernameError = "Username contains invalid characters";
            }
            //***EMAIL***
            if (email.Length == 0)
            {
                success = false;
                emailError = "Enter an email";
            }
            else if (!Regex.IsMatch(email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}"))
            {
                success = false;
                emailError = "Invalid email";
            }
            if (password.Length == 0)
            {
                success = false;
                passwordError = "Enter a password";
            }
            else if (password.Length < 8)
            {
                success = false;
                passwordError = "Password must be at least 8 characters long";
            }
            if (confirm.Length == 0)
            {
                success = false;
                confirmError = "Confirm your password";
            }
            else if (password.Equals(confirm) == false)
            {
                success = false;
                confirmError = "Passwords do not match";
            }
            if (success)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection(Connection.Str);
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "SELECT id FROM users WHERE username = @user";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@user", username);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        success = false;
                        usernameError = "Username already exists";
                        return;
                    };
                    rdr.Close();
                    cmd.CommandText = "INSERT into users(username, password, email, currency) VALUES (@user,@pass,@email, 10) ";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@pass", Crypto.HashPassword(password));
                    cmd.Parameters.AddWithValue("@email", email);
                    success = cmd.ExecuteNonQuery() > 0;
                    if (success)
                    {
                        id = Convert.ToInt32(cmd.LastInsertedId);//.ToString();
                    }
                    Conn.Close();


                    /*Shouldn't need this chunk but leaving it here just in case
                    if (rdr.Read())
                    {
                        id = Convert.ToInt32(rdr[0]);
                    } else
                    {
                    }*/
                }
                catch (Exception ex)
                {
                    passwordError = ex.Message;
                }
            }
        }
    }
}