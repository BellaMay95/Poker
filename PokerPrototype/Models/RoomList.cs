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
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "SELECT rooms.id, title, current_players, max_players, big_blind, seconds, max_buy_in, username FROM rooms JOIN users on users.id = creator_id WHERE private = 0 AND current_players != 0";
                cmd.Prepare();
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Add(new RoomModel(Convert.ToInt32(rdr[0]), rdr[1].ToString(), Convert.ToInt32(rdr[2]), Convert.ToInt32(rdr[3]), Convert.ToInt32(rdr[4]), Convert.ToInt32(rdr[5]), Convert.ToInt32(rdr[6]), rdr[7].ToString()));
                }
            }
            catch (Exception e)
            {
                Add(new RoomModel(0, "ERROR: " +  e.Message, 1, 2, 2, 60, 40, "-"));
            }
        }
    }
}