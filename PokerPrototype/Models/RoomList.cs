using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace PokerPrototype.Models
{
    public class RoomList : List<RoomModel>
    {
        public RoomList()
        {
            MySqlConnection Conn = new MySqlConnection("server=sql9.freemysqlhosting.net;database=sql9140372;user=sql9140372;password=WSx2C8iRZx;");
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Open();
            cmd.Connection = Conn;
            cmd.CommandText = "SELECT * FROM rooms WHERE private = 0";
            cmd.Prepare();
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Add(new RoomModel(Convert.ToInt32(rdr[0]), rdr[1].ToString(), Convert.ToInt32(rdr[2]), Convert.ToInt32(rdr[3]), Convert.ToInt32(rdr[4]), Convert.ToInt32(rdr[5]), Convert.ToInt32(rdr[6])));
            }
        }
    }
}