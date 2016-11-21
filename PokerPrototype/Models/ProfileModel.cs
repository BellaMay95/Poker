using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Web.Helpers;

namespace PokerPrototype.Models
{
    public class ProfileModel
    {
        public UserModel User;
        public string email;
        public ProfileModel(int id)
        {
            User = new UserModel(id);
            email = User.getEmail();
        }
    }

    public class AvatarModel
    {
        public AvatarModel(int id, string src)
        {
            MySqlConnection Conn = new MySqlConnection("server=sql9.freemysqlhosting.net;database=sql9140372;user=sql9140372;password=WSx2C8iRZx;");
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Open();
            cmd.Connection = Conn;
            cmd.CommandText = "UPDATE users SET avatar=@src WHERE id=" + id;
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@src", src);
            //MySqlDataReader rdr = cmd.ExecuteReader();
            cmd.ExecuteNonQuery();
            Conn.Close();
        }
    }

    public class PasswordModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public string passwordError { get; set; }
        public string newpasswordError { get; set; }
        public string confirmError { get; set; }
        public PasswordModel(int id, string oldPassword, string newPassword, string Confirm)
        {
            passwordError = newpasswordError = confirmError = "";
            success = true;
            if (oldPassword.Length == 0)
            {
                success = false;
                passwordError = "Please enter your old password";
            }
            if (newPassword.Length == 0)
            {
                success = false;
                newpasswordError = "Enter a new password!!";
            }
            if (Confirm.Length == 0)
            {
                success = false;
                confirmError = "CONFIRM YOUR PASSWORD!";
            }
            if (success == true && !newPassword.Equals(Confirm))
            {
                success = false;
                confirmError = "Your passwords do not match.";
            }
            if (success == true)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection("server=sql9.freemysqlhosting.net;database=sql9140372;user=sql9140372;password=WSx2C8iRZx;");
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "SELECT password FROM users WHERE id=" + id;
                    //cmd.Prepare();
                    //cmd.Parameters.AddWithValue("@src", src);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read() && Crypto.VerifyHashedPassword(rdr[0].ToString(), oldPassword))
                    {
                        rdr.Close();
                        cmd.CommandText = "UPDATE users SET password=@myawesomepassword WHERE id=" + id;
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@myawesomepassword", Crypto.HashPassword(newPassword));
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                    else
                    {
                        passwordError = "Your old password is incorrect";
                    }
                }
                catch(Exception e)
                {
                    confirmError = e.Message;
                }
            }
        }
    }

    public class EmailModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public string emailError { get; set; }
        public string passwordError { get; set; }
        public EmailModel(int id, string email, string password)
        {
            success = true;
            passwordError = emailError = "";
            if (email.Length == 0)
            {
                success = false;
                emailError = "Enter your new email";
            }
            if (password.Length == 0)
            {
                success = false;
                passwordError = "Enter your password";
            }
            if (success == true)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection("server=sql9.freemysqlhosting.net;database=sql9140372;user=sql9140372;password=WSx2C8iRZx;");
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Close();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "SELECT password FROM users WHERE id=" + id;
                    //cmd.Prepare();
                    //cmd.Parameters.AddWithValue("@src", src);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read() && Crypto.VerifyHashedPassword(rdr[0].ToString(), password))
                    {
                        rdr.Close();
                        cmd.CommandText = "UPDATE users SET email=@newemail WHERE id=" + id;
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@newemail", email);
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                    else
                    {
                        passwordError = "Your password is incorrect";
                    }
                }
                catch(Exception e)
                {
                    passwordError = e.Message;
                }
            }
        }   
    }
}
 