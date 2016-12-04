using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace PokerPrototype.Models
{
    public class RoomModel
    {
        public int roomID { get; set; }
        public string roomName { get; set; }
        public string roomCreator { get; set; }
        public int currentNumberPlayers { get; set; }
        public int maxPlayerCount { get; set; }
        public int blinds { get; set; }
        public int seconds { get; set; }
        public int max_buy_in { get; set; }
        public RoomModel(int roomID, string roomName, int currentNumberPlayers, int maxPlayerCount, int blinds, int seconds, int max_buy_in, string roomCreator)
        {
            this.roomID = roomID;
            this.roomName = roomName;
            this.roomCreator = roomCreator;
            this.currentNumberPlayers = currentNumberPlayers;
            this.maxPlayerCount = maxPlayerCount;
            this.blinds = blinds;
            this.seconds = seconds;
            this.max_buy_in = max_buy_in;
        }
    }

    public class createRoomModel
    {
        public int id { get; set; }
        public bool success { get; set; }
        public string nameError { get; set; }
        public string maxError { get; set; }
        public string blindError { get; set; }
        public string secondsError { get; set; }
        public string buyinError { get; set; }
        public string privateError { get; set; }
        public createRoomModel(int id, string roomName, string maxPlayerCount, string blinds, string seconds, string max_buy_in, string private_room)
        {
            success = true;
            nameError = maxError = blindError = secondsError = buyinError = privateError = "";
            if (roomName.Length == 0)
            {
                success = false;
                nameError = "Enter the name of the room";
            }
            if (maxPlayerCount.Length == 0 || Convert.ToInt32(maxPlayerCount) < 2 || Convert.ToInt32(maxPlayerCount) > 6)
            {
                success = false;
                maxError = "Enter a number between 2 and 6";
            }
            if (blinds.Length == 0 || Convert.ToInt32(blinds) < 1)
            {
                success = false;
                blindError = "Enter a blind greater than 0";
            }
            if (seconds.Length == 0 || Convert.ToInt32(seconds) < 1)
            {
                success = false;
                secondsError = "Enter a turn time greater than 1 second";
            }
            if (max_buy_in.Length == 0 || Convert.ToInt32(max_buy_in) < 1)
            {
                success = false;
                buyinError = "Enter a buy-in value greater than 0";
            }
            if (Convert.ToInt32(private_room) != 0 && Convert.ToInt32(private_room) != 1)
            {
                success = false;
                privateError = "Value isn't between 0 and 1.";
            }

            if (success)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection(Connection.Str);
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "INSERT into rooms(title,current_players,max_players,big_blind,seconds,max_buy_in,private,creator_id) VALUES (@roomName, @currentPlayers, @maxPlayerCount, @blinds, @seconds, @max_buy_in, @private_room, @id) ";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@roomName", roomName);
                    cmd.Parameters.AddWithValue("@currentPlayers", "1");
                    cmd.Parameters.AddWithValue("@maxPlayerCount", maxPlayerCount);
                    cmd.Parameters.AddWithValue("@blinds", blinds);
                    cmd.Parameters.AddWithValue("@seconds", seconds);
                    cmd.Parameters.AddWithValue("@max_buy_in", max_buy_in);
                    cmd.Parameters.AddWithValue("@private_room", private_room);
                    cmd.Parameters.AddWithValue("@id", id);
                    success = cmd.ExecuteNonQuery() > 0;
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
                    privateError = ex.Message;
                }
            }
        } 
    }
}