/*
 *             //Context.RequestCookies["MYCOOKIE"].Value; //this is how to access a cookie
Use connectionIDs for playerIDs.
This way can send targeted alerts to the player whose turn it is 

TODO:
-Figure out how to retrieve player username from view for data 
    @Model.username similar to ViewProfile?
-Ensure the difference between "userID" (username) and "connID" (what most of these functions actually need
    Mostly done, need one last final check. All instances of userID replaced with more accurate connID 
-Raise currently bugged. Need to set Raise to trigger 'another' betting round (up to a predefined limit)
    Potential work around implemented. Will need to test because potentially really buggy
-Need to allow View to see data on which players have folded or not



*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PokerPrototype.Models;
using System.Threading.Tasks;
using PokerGame;
using MySql.Data.MySqlClient;

namespace PokerPrototype.Hubs
{
    public class MyHub : Hub
    {
        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }
//CONNECTION FUNCTIONS
//block dedicated to functions handling connection/disconnection
        //Joining room

        public void GetRoomInfo(string roomID ,string username)
        {
            //room table info
            string title;
            int current_players;
            int max_players;
            int big_blind;
            int seconds;
            int max_buy_in;
            int permissions;
            //user table info
            int currency;
            //read in the current status of the room
            MySqlConnection Conn = new MySqlConnection(Connection.Str);
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Open();
            cmd.Connection = Conn;
            //check username
            
            cmd.CommandText = "SELECT * FROM users WHERE username = @givenstring";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@givenstring", username);
            MySqlDataReader rdr = cmd.ExecuteReader();
            //user name exists
            if (rdr.Read())
            {
                currency = (int)rdr["currency"];
            }
            else
            {
                //user doesn't exist?
                Clients.Caller.alertMessage("Error: unable to join");
                return;
            }
            //user exists, now check if room exists
            cmd.CommandText = "SELECT * FROM rooms WHERE id = @givenid";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@givenid", roomID);
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                title = (string)rdr["title"];
                current_players = (int)rdr["current_players"];
                max_players = (int)rdr["max_players"];
                if(current_players>=max_players)
                {
                    Clients.Caller.alertMessage("Error: Room is Full");
                }
                big_blind = (int)rdr["big_blind"];
                seconds = (int)rdr["seconds"];
                max_buy_in = (int)rdr["max_buy_in"];
                //private/public are keywords
                permissions= (int)rdr["Private"];
                //don't see reason to keep creator_id, add if necessary
            }
            else
            {
                //room doesn't exist
                Clients.Caller.alertMessage("Error: Room does not exist");
                Conn.Close();
                return;
            }
            //To reach this point, room must exist and there must be room to join
            //Client user exists, information has been pulled, and is allowed to join

            //On join, getState of game
            GameManager manager = new GameManager();
            string result = manager.getState(Convert.ToInt32(roomID));

            //game has not yet started, join in the "pre-game" stage
            if(manager.allReady()==false)
            {
                //check if game data existed, fill in options if it didn't
                if (result.Equals(""))
                {
                    manager.setupRoom(max_players, big_blind, max_buy_in, permissions);

                }
                manager.joinStart(Context.ConnectionId, currency, username);
                manager.updateState(Convert.ToInt32(roomID));
            }
            else
            {
                //game has started, join in middle of game instead
                manager.joinMid(Context.ConnectionId, currency, username);
                manager.updateState(Convert.ToInt32(roomID));
            }
            //add connection to connections table
            cmd.CommandText = "SELECT * FROM connections WHERE connID = @conn";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@conn", Context.ConnectionId);
                rdr = cmd.ExecuteReader();
                //connectionID already exists
                if (rdr.Read())
                {
                    Clients.Caller.alertMessage("Error: Connection already in use");
                }
                else
                {
                    rdr.Close();
                    cmd.CommandText = "INSERT INTO connections (connID, roomID, username) VALUES (@connID, @roomID, @name)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@connID", Context.ConnectionId);
                    cmd.Parameters.AddWithValue("@roomID", roomID);
                    //change this to reflect actual username
                    cmd.Parameters.AddWithValue("@name", username);
                    cmd.ExecuteNonQuery();
                    Groups.Add(Context.ConnectionId, roomID);
                    //Do we need the below?
                    UserModel user = new UserModel(3);
                    Clients.All.alertJson(user);
                }
                Conn.Close();

  

        }
        public override Task OnDisconnected(bool stopCalled)
        {
            GameManager manager = new GameManager();
            string roomID = "";
            //SQL block retrieves this connID's particular roomID, which is also the group name
            MySqlConnection Conn = new MySqlConnection(Connection.Str);
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Open();
            cmd.Connection = Conn;
            //change later to check username against passed username
            cmd.CommandText = "SELECT * FROM connections WHERE connID = @conn";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@conn", Context.ConnectionId);
            MySqlDataReader rdr = cmd.ExecuteReader();
            //connectionID exists
            if (rdr.Read())
            {
                roomID = (string)rdr["roomID"];
                rdr.Close();
                cmd.CommandText = "DELETE FROM connections WHERE connID = @connID";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@connID", Context.ConnectionId);
                cmd.ExecuteNonQuery();
            }
            else
            {
                //connID doesn't exist
            }
            Conn.Close();
            manager.getState(Convert.ToInt32(roomID));
            Groups.Remove(Context.ConnectionId, roomID);
            manager.leave(Context.ConnectionId);
            manager.updateState(Convert.ToInt32(roomID));

            return base.OnDisconnected(stopCalled);
        }
        //END CONNECTION FUNCTIONS
        //CHAT FUNCTIONS
        //functions to handle chatting portion of game
        public void Send(string roomID, string message)
        {

            string connid = Context.ConnectionId;
            Clients.Group(roomID).alertMessage(message + " from " + connid);
            //Clients.All.alertJson(message + " from " + connid);
            //hello
        }
        //BROADCAST FUNCTIONS
        //Do not call, these will be used to call Javascript functions to update view periodically
        //If view needs these, please uncomment the "broadcasting" line in each function
        //replace .updateX function with whatever the appropriate function is

        //if view needs to check whether buttons should be enabled or not (is game running?)
        //have a "updateStatus" javascript function.
        public void broadcastStatus(bool check)
        {
            //returns true if game is running, false if not
            //Clients.Group(roomID).updateStatus(check);
        }
        public void broadcastHand(string roomID, string connID)
        {
            
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));

            Card card1 = manager.getPlayerCard1(Context.ConnectionId);
            Card card2 = manager.getPlayerCard2(Context.ConnectionId);
            Clients.Client(connID).updateHand(card1.img, card2.img);

        }
        //broadcast number of current players
        public void broadcastPlayers(string roomID)
        {

        }
        //broadcasts to client a list of cards making up the board
        //you can pull img strings from there
        public void broadcastBoard(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            // string board = manager.getBoard();
            List<Card> board = manager.getBoardCards();
            //Clients.Group(roomID).updateBoard(board);
            string boardString="";
            for (int i=0;i<board.Count;i++)
            {
                boardString += board[i].img;
            }
            Clients.Group(roomID).alertMessage(boardString);
            //Passes space delinated img files ("img1 img 2 img3 ")
            Clients.Group(roomID).updateBoard(boardString);
        }
        public bool broadcastRaise(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //Clients.Group(roomID).updateRaise(manager.getHasRaised()
            return false;// just to get function working
            //return manager.getHasRaised()
        }
        public int broadcastCallAmt(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            Clients.Group(roomID).updateCallAmt(manager.getCallAmt());
            return manager.getCallAmt();
        }
        public void broadcastPot(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            int pot = manager.getPot();
            Clients.Group(roomID).updatePot(pot);
        }
//END BROADCAST FUNCTIONS
//POKER FUNCTIONS
//Bind to buttons and pass relevant input.
        
        public void Leave(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            manager.leave(Context.ConnectionId);
            manager.updateState(Convert.ToInt32(roomID));
            MySqlConnection Conn = new MySqlConnection(Connection.Str);
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Open();
            cmd.Connection = Conn;
            //change later to check username against passed username
            cmd.CommandText = "DELETE FROM connections WHERE connID = @conn";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@conn", Context.ConnectionId);
            int success = cmd.ExecuteNonQuery();
            Conn.Close();
            Groups.Remove(Context.ConnectionId, roomID);
            Clients.Caller.alertMessage("You have successfully left the game");
        }
        public void Fold(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //if user is allowed to move
            if (Context.ConnectionId.Equals(manager.getCurrentPlayer().ID))
            {
                //fold
                manager.fold(Context.ConnectionId);
                //if the game is over
                if (manager.gameOver())
                {
                    //handleGameOver
                    manager.updateState(Convert.ToInt32(roomID));
                    gameOver(roomID);
                }
                manager.cycle();
                manager.updateState(Convert.ToInt32(roomID));
                //need to take place after updating state, in order to grab up to date information
                adjustBoard(roomID);
                broadcastPot(roomID);
                alertPlayerTurn(manager.getCurrentPlayer().ID);
            }
            //broadcast to allclients
        }
        public int Call(string roomID)
        {
            GameManager manager= new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            Player temp = manager.getCurrentPlayer();
            if (temp != null)
            {
                if (Context.ConnectionId.Equals(temp.ID))
                {
                    manager.call(Context.ConnectionId);
                    manager.cycle();
                    manager.updateState(Convert.ToInt32(roomID));
                    //need to take place after updating state, in order to grab up to date information
                    adjustBoard(roomID);
                    broadcastPot(roomID);
                    alertPlayerTurn(manager.getCurrentPlayer().ID);
                    return manager.getCallAmt();
                }
                else
                {
                    Clients.Caller.alertMessage("Error: Not your turn");
                    return -1;
                }
            }
            else
            {
                Clients.Caller.alertMessage("Error: Game has not started");
                return -1;
            }
        }     
        public int Check(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //It must be their turn, and no pets should have been made in order to check
            //verify with group later on exact check rulings
            if((Context.ConnectionId.Equals(manager.getCurrentPlayer().ID))&&(manager.getPot()==0))
            {
                //a check effectively passes the turn, move on to the next player
                manager.cycle();
                manager.updateState(Convert.ToInt32(roomID));
                //need to take place after updating state, in order to grab up to date information
                adjustBoard(roomID);
                broadcastPot(roomID);
                alertPlayerTurn(manager.getCurrentPlayer().ID);
                return 0;
            }
            //identify exact source of error
            else if(manager.getPot()==0)
            {
                //if checking disallowed, notify player
                Clients.Caller.alertMessage("Error: Checking disallowed");
                return -1;
            }
            else
            {
                //not player's turn
                Clients.Caller.alertMessage("Error: Not your turn");
                return -1;
            }
        }
        //NOTE: need to implement Raise causing another round of betting "resetting the cycle" so to speak
        public int Raise(string roomID, int amount)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //verify it is user's turn
            if (Context.ConnectionId.Equals(manager.getCurrentPlayer().ID))
            {
                //verify raises are legal under the current state
                if (manager.getRaiseCount() < 3)
                {
                    //verify user can raise by that amount/call at tsame time
                    if (manager.raise(Context.ConnectionId, amount) >= 0)//check for nonnegative number
                    {

                        manager.cycle();
                        manager.updateState(Convert.ToInt32(roomID));
                        //need to take place after updating state, in order to grab up to date information
                        adjustBoard(roomID);
                        broadcastPot(roomID);
                        alertPlayerTurn(manager.getCurrentPlayer().ID);
                        return amount;
                    }
                    else
                    {
                        Clients.Caller.alertMessage("Error: Not enough currency");
                        return -1;
                    }
                }
                else
                {
                    Clients.Caller.alertMessage("Error: Not your turn");
                    return -1;
                }
            }
            else
            {
                Clients.Caller.alertMessage("Error: Not your turn");
                return -1;
            }
        }
        //call this function when player confirms they are ready
        //should disallow use of button after function returns true, to prevent spam 
        //This function is what starts the game, when the last player confirms ready
        public bool confirmReady(string roomID)
        {
            GameManager manager = new PokerGame.GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //first off, check to see the game hasn't already started.
            //if allReady already returns true, this function shouldn't be able to be called
            //check anyways
            if (manager.allReady() == false)
            {
                manager.readyPlayer(Context.ConnectionId);
                //start the game if all ready
                if (manager.allReady())
                {
                    //assign all players their cards
                    manager.init();

                    //broadcast fact that game is now running
                    broadcastStatus(true);
                    manager.updateState(Convert.ToInt32(roomID));
                    //broadcast hands to players
                    List<Player> players = manager.getActivePlayers();
                    for (int i = 0; i < players.Count; i++)
                    {
                        broadcastHand(roomID, players[i].ID);
                    }
                    //grab activePlayer[0].ID and notify them it is there turn
                    Clients.Group(roomID).alertMessage("Game has started!");
                    alertPlayerTurn(manager.getCurrentPlayer().ID);
                    
                    return true;
                    //wooooooo start the game
                }
                else
                {
                    manager.readyPlayer(Context.ConnectionId);
                    Clients.Caller.alertMessage("Ready Confirmed");
                    manager.updateState(Convert.ToInt32(roomID));
                    return true;
                    //should we formally wait? shouldn't need to right?
                }
            }
            else
            {
                Clients.Caller.alertMessage("Unable to Ready");
                return false;
            }
        }
        public void gameOver(string roomID)
        {
            GameManager manager = new PokerGame.GameManager();
            manager.getState(Convert.ToInt32(roomID));

            List<String> winners =manager.getWinner();
            manager.award(winners);
            manager.reset();
            manager.updateState(Convert.ToInt32(roomID));
            //broadcast fact that game is now over
            broadcastStatus(false);
        }
        //adds cards to board at end of every betting cycle
        //may need adjustments if asyncs occur
        public void adjustBoard(string roomID)
        {
            GameManager manager = new PokerGame.GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //if a betting cycle has finished
            if(manager.getCycleComplete()==true)
            {
                //check which round was completed
                if(manager.getRoundNumber()==1)//0 completed
                {
                    manager.addBoard();
                    manager.addBoard();
                    manager.addBoard();
                    //broadcastBoard(roomID);
                    manager.nextRound();
                    manager.updateState(Convert.ToInt32(roomID));
                    broadcastBoard(roomID); 
                }
                else if (manager.getRoundNumber()<4)//1, 2, 3 completed
                {
                    manager.addBoard();
                    //broadcastBoard(roomID);
                    manager.nextRound();
                    manager.updateState(Convert.ToInt32(roomID));
                    broadcastBoard(roomID);

                }
                else
                {
                    //final round of betting complete, game is over
                    gameOver(roomID);
                }
            }

        }
        //function to alert given conn ID that it is its turn
        public void alertPlayerTurn(string connID)
        {
            Clients.Client(connID).alertMessage("Your turn");
        }
//END POKER FUNCTIONS
//GETTER FUNCTIONS
//get functions
        public int getNumPlayers(string roomID)
        {
            GameManager manager = new PokerGame.GameManager();
            manager.getState(Convert.ToInt32(roomID));

            return manager.getPlayerCount();
        }

    }
}
