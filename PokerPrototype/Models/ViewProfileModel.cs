using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerPrototype.Models
{
    public class ViewProfileModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        //private UserModel User;
        public string username;
        public int currency;
        public string avatar;
        public bool canEdit;
        public bool isFriend;
        public bool disabled;
        public bool canAdmin;
        public List <string> friendAvatar;
        public List <string> friendUser;

        public ViewProfileModel(int sessionid, string username)
        {
            success = true;
            canEdit = isFriend = canAdmin = disabled = false;
            friendAvatar = new List<string>();
            friendUser = new List<string>();
            //User = new UserModel(sessionid);
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                if (username.Length == 0)
                {
                    cmd.CommandText = "SELECT id, username,currency,avatar,isAdmin,diabled FROM users WHERE id = " + sessionid;
                }else
                {
                    cmd.CommandText = "SELECT id, username,currency,avatar,isAdmin,disabled FROM users WHERE username = @user";
                }
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", username);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    this.username = rdr[1].ToString();
                    currency = Convert.ToInt32(rdr[2]);
                    avatar = rdr[3].ToString();
                    if (avatar.Length == 0)
                        avatar = Avatar.Str;
                    int user_id = Convert.ToInt32(rdr[0]);
                    if (sessionid == user_id)
                    {
                        canEdit = true;
                    }
                    else
                    {
                        if (rdr[4].ToString() == "1")
                        {
                            canAdmin = true;
                        }
                        if (rdr[5].ToString() == "1")
                        {
                            disabled = true;
                        }
                    }
                    rdr.Close();
                    cmd.CommandText = "SELECT avatar, username, friend_id, user_id FROM users JOIN friends ON id = friend_id WHERE user_id = " + user_id + " OR friend_id = " + user_id;
                    //MySqlDataReader 
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (Convert.ToInt32(rdr[2]) == user_id)
                        {
                            if (Convert.ToInt32(rdr[3]) == sessionid)
                            {
                                isFriend = true;
                            }
                            continue;
                        }
                        if (rdr[0].ToString().Length > 0)
                            friendAvatar.Add(rdr[0].ToString());
                        else
                        {
                            friendAvatar.Add(Avatar.Str);
                        }
                        friendUser.Add(rdr[1].ToString());
                    }
                }
                else
                {
                    success = false;
                    return;
                }
                Conn.Close();
            }
            catch (Exception e)
            {
                success = false;
                return;
            }
 
        }
    }

    public class AddFriendModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public string username { get; set; }
        public string connectError { get; set; }

        public AddFriendModel(int id, string newUser)
        {
            connectError = "";
            username = newUser;
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "SELECT id FROM users WHERE username = @user";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", newUser);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (!rdr.Read())
                {
                    success = false;
                    connectError = "failed to return id of friend";
                    return;
                }
                int friend_id = Convert.ToInt32(rdr[0]);
                rdr.Close();
                cmd.CommandText = "INSERT into friends(user_id, friend_id) VALUES (" + id + ", " + friend_id + ") ";
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            catch(Exception ex)
            {
                connectError = ex.Message;
            }
        }
    }

    public class RemoveFriendModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public string username { get; set; }
        public string connectError { get; set; }

        public RemoveFriendModel(int id, string newUser)
        {
            connectError = "";
            username = newUser;
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "SELECT id FROM users WHERE username = @user";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", newUser);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (!rdr.Read())
                {
                    success = false;
                    connectError = "failed to return id of friend";
                    return;
                }
                int friend_id = Convert.ToInt32(rdr[0]);
                rdr.Close();
                cmd.CommandText = "DELETE FROM friends WHERE user_id=" + id + " AND friend_id=" + friend_id;
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            catch (Exception ex)
            {
                connectError = ex.Message;
            }
        }
    }

    public class BanUser
    {
        public BanUser(string user)
        {
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "UPDATE users SET disabled=1 WHERE username = @user";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", user);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
        }
    }

    public class unbanUser
    {
        public unbanUser(string user)
        {
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "UPDATE users SET disabled=0 WHERE username = @user";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", user);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
        }
    }

    public class promoteUser
    {
        public promoteUser(string user)
        {
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "UPDATE users SET isAdmin=1 WHERE username = @user";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", user);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
        }
    }

    public class demoteUser
    {
        public demoteUser(string user)
        {
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "UPDATE users SET isAdmin=0 WHERE username = @user";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", user);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
        }
    }
}