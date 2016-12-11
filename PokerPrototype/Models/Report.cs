using MySql.Data.MySqlClient;
using PokerPrototype.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace PokerPrototype.Models
{
    public class ReportListModel
    {
        public int reportID { get; set; }
        public int type { get; set; }
        public string reportName { get; set; }
        public string ticketCreator { get; set; }
        public string ticketedUser { get; set; }
        public string issue { get; set; }
        public int resolved { get; set; }
        public ReportListModel(int reportID, int type, string reportName, string ticketCreator, string ticketedUser, string issue, int resolved)
        {
            this.reportID = reportID;
            this.type = type;
            this.reportName = reportName;
            this.ticketCreator = ticketCreator;
            this.ticketedUser = ticketedUser;
            this.issue = issue;
            this.resolved = resolved;
        }
    }

    public class ReportList : List<ReportListModel>
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public ReportList()
        {
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "SELECT * FROM reports";
                cmd.Prepare();
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Add(new ReportListModel(Convert.ToInt32(rdr[0]), Convert.ToInt32(rdr[1]), rdr[2].ToString(), rdr[3].ToString(), rdr[4].ToString(), rdr[5].ToString(), Convert.ToInt32(rdr[6])));
                }
                Conn.Close();
            }
            catch (Exception e)
            {
                Add(new ReportListModel(0, 0, "ERROR: " + e.Message, "-", "-", "-", 0));
            }
        }
    }

    public class ReportBugModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public string titleError { get; set; }
        public string problemError { get; set; }
        public ReportBugModel(int id, string title, string issue)
        {
            titleError = problemError = "";
            success = true;
            if (title.Length == 0)
            {
                success = false;
                titleError = "please enter a title for your report";
            }
            if (issue.Length == 0)
            {
                success = false;
                problemError = "please describe your issue";
            }
            if (success == true)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection(Connection.Str);
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "SELECT username FROM users WHERE id=" + id;
                    cmd.Prepare();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        //type of 1 means that this is a bug report
                        cmd.CommandText = "INSERT into reports(type,name,call_user,on_user,issue) VALUES (1,@title,@user,'NONE', @issue) ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@user", rdr[0].ToString());
                        cmd.Parameters.AddWithValue("@issue", issue);
                        rdr.Close();
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                    else
                    {
                        success = false;
                        titleError = "username not found";
                    }
                    
                }
                catch (Exception e)
                {
                    titleError = e.Message;
                }
            }
        }
    }

    public class ReportUserModel
    {
        [System.Web.Script.Serialization.ScriptIgnore]
        public bool success { get; set; }
        public string titleError { get; set; }
        public string toUserError { get; set; }
        public string problemError { get; set; }
        public ReportUserModel(int id, string title, string toUser, string issue)
        {
            titleError = toUserError = problemError = "";
            success = true;
            if (title.Length == 0)
            {
                success = false;
                titleError = "please summarize the issue";
            }
            if (toUser.Length == 0)
            {
                success = false;
                toUserError = "please enter the username of the person you're reporting";
            }
            if (issue.Length == 0)
            {
                success = false;
                problemError = "please describe your issue";
            }
            if (success == true)
            {
                try
                {
                    MySqlConnection Conn = new MySqlConnection(Connection.Str);
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                    Conn.Open();
                    cmd.Connection = Conn;
                    cmd.CommandText = "SELECT username FROM users WHERE id =" + id;
                    cmd.Prepare();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        //type of 2 means that this is a user report
                        cmd.CommandText = "INSERT into reports(type,name,call_user,on_user,issue) VALUES (2,@title,@user1,@user2, @issue) ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@user1", rdr[0].ToString());
                        cmd.Parameters.AddWithValue("@user2", toUser);
                        cmd.Parameters.AddWithValue("@issue", issue);
                        rdr.Close();
                        success = cmd.ExecuteNonQuery() > 0;
                        Conn.Close();
                    }
                    else
                        success = false;

                }
                catch (Exception e)
                {
                    titleError = e.Message;
                }
            }
        }
    }

    public class ResolveReport
    {
        public ResolveReport(int reportID)
        {
            try
            {
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "UPDATE resolved FROM reports WHERE id =" + reportID;
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            catch (Exception e)
            {
            }
        }
    }
}