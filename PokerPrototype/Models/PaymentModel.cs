﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                    MySqlConnection Conn = new MySqlConnection("server=sql9.freemysqlhosting.net;database=sql9140372;user=sql9140372;password=WSx2C8iRZx;");
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "INSERT INTO payment_info (user_id, name, card_number, cvc) VALUES (@id, @name, @card_number, @cvc)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@card_number", cardNumber);
                    cmd.Parameters.AddWithValue("@cvc", cvc);
                    success = cmd.ExecuteNonQuery() > 0;
                    amountError = cmd.LastInsertedId.ToString();
                }
                catch (Exception ex)
                {
                    amountError = ex.Message;
                }
            }
        }
    }
}