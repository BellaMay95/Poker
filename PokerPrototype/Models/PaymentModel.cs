using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;

namespace PokerPrototype.Models
{
    public class PaymentModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public string amountError { get; set; }
        public string nameError { get; set; }
        public string cardNumberError { get; set; }
        public string cvcError { get; set; }
        public string expiresError { get; set; }
        public string passwordError { get; set; }
        public PaymentModel(int id, string amount, string name, string cardNumber, string cvc, string expires, string password)
        {
            success = true;
            amountError = nameError = cardNumberError = cvcError = expiresError = passwordError = "";
            if (amount.Length == 0)
            {
                success = false;
                amountError = "Enter an Amount";
            }
            if (name.Length == 0)
            {
                success = false;
                nameError = "Enter the Name on the Card";
            }
            if (cardNumber.Length == 0)
            {
                success = false;
                cardNumberError = "Enter Card Number";
            }
            else if (!Regex.IsMatch(cardNumber, @"^[0-9]{4}[- ]?[0-9]{4}[- ]?[0-9]{4}[- ]?[0-9]{4}$"))
            {
                cardNumberError = "Invalid Credit Card";
            }
            if (cvc.Length == 0)
            {
                success = false;
                cvcError = "Enter the Card's CVC";
            }
            if (expires.Length == 0)
            {
                success = false;
                expiresError = "Enter the Card's expiration date";
            }
            if (expires.Length == 0)
            {
                success = false;
                passwordError = "Enter your password";
            }
            if (success)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection(Connection.Str);
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    /*cmd.Connection = Conn;
                    cmd.CommandText = "INSERT INTO payment_info (user_id, name, card_number, cvc) VALUES (@id, @name, @card_number, @cvc)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@card_number", cardNumber);
                    cmd.Parameters.AddWithValue("@cvc", cvc);
                    success = cmd.ExecuteNonQuery() > 0;
                    amountError = cmd.LastInsertedId.ToString();*/

                    cmd.Connection = Conn;
                    cmd.CommandText = "SELECT password FROM users WHERE id=" + id;
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read() && Crypto.VerifyHashedPassword(rdr[0].ToString(), password))
                    {
                        rdr.Close();
                        cmd.CommandText = "UPDATE users SET currency = currency + @amount WHERE id = @id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@id", id);
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    else
                    {
                        passwordError = "Your password is incorrect";
                    }

                    Conn.Close();
                }
                catch (Exception ex)
                {
                    amountError = ex.Message;
                }
            }
        }
    }
}