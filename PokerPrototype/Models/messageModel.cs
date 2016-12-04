using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerPrototype.Models
{
    public class MessageModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string date { get; set; }
        public string message { get; set; }
        public MessageModel(int id, string username, string date, string message)
        {
            this.id = id;
            this.username = username;
            this.date = date;
            this.message = message;
        }
    }
    public class InboxList : List<MessageModel>
    {
        public InboxList(int id)
        {
            MySqlConnection Conn = new MySqlConnection(Connection.Str);
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Open();
            cmd.Connection = Conn;
            cmd.CommandText = "SELECT messages.id, username, date_sent, message FROM messages JOIN users ON users.id = sender_id WHERE receiver_id = " + id;
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Add(new MessageModel(Convert.ToInt32(rdr[0]), rdr[1].ToString(), rdr[2].ToString(), rdr[3].ToString()));
            }
        }
    }
    public class SentList : List<MessageModel>
    {
        public SentList(int id)
        {
            MySqlConnection Conn = new MySqlConnection(Connection.Str);
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Open();
            cmd.Connection = Conn;
            cmd.CommandText = "SELECT messages.id, username, date_sent, message FROM messages JOIN users ON users.id = receiver_id WHERE sender_id = " + id;
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Add(new MessageModel(Convert.ToInt32(rdr[0]), rdr[1].ToString(), rdr[2].ToString(), rdr[3].ToString()));
            }
        }
    }
    public class SendMessageModel
    {
        public string toError { get; set; }
        public string messageError { get; set; }
        public SendMessageModel(int id, string to, string message)
        {
            toError = messageError = "";
            if (to.Length == 0)
            {
                toError = "Enter a recipient";
            }
            if (message.Length == 0)
            {
                messageError = "Enter a message";
            }
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "SELECT id FROM users WHERE username = @user";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", to);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    int reciever_id = Convert.ToInt32(rdr[0]);
                    rdr.Close();
                    cmd.CommandText = "INSERT into messages (sender_id, receiver_id, date_sent, message) VALUES (" + id + "," + reciever_id + ",now(), @message)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    toError = "Username does not exist";
                };
            } catch (Exception ex)
            {
                messageError = ex.Message;
            }
        }
    }
}