/*
Use connectionIDs for playerIDs.
This way can send targeted alerts to the player whose turn it is 

TODO:
-Figure out how to retrieve player username from view
-Edit usermodel call so it is the ID of the player, not Josh
-Triple check broadcasts working functionally (integrate with Tim and Jamie, not just my janky console)
-Raise currently bugged. Need to set Raise to trigger 'another' betting round (up to a predefined limit)
-Need to allow View to see data on which players have folded or not


Tenative game loop plan:
-When player joins empty room, have room be in permanent wait status
-On second player join, call init, have both players press a "ready" button"
-Players may continually join (at which point init is recalled) until either cap or all current players have marked ready
-The MOMENT all players have sent a "ready", init into the game loop.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PokerPrototype.Models;
using System.Threading.Tasks;
using PokerGame;

namespace PokerPrototype.Hubs
{
    public class MyHub : Hub
    {
        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }

        //Joining room
        public void GetRoomInfo(string roomID)
        {
            //Context.RequestCookies["MYCOOKIE"].Value; //this is how to access a cookie
            //On join, getState of game
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //if room is empty
            if (manager.getPlayerCount() == 0)
            {
                
                /*Need to grab player's username at the very least, can SQL pull the currency
                 * 
                 * */
                //change below to match up with grabbed information
                manager.joinStart(Context.ConnectionId, 0, "Bob");
                manager.updateState(Convert.ToInt32(roomID));
                Groups.Add(Context.ConnectionId, roomID);
                UserModel user = new UserModel(3);
                Clients.All.alertJson(user);

            }
            //else if room is not yet filled
            else if (manager.getPlayerCount() < manager.getRoomCap())
            {
                /*Need to grab player's username at the very least, can SQL pull the currency
* 
* */
                //change below to match up with grabbed information
                //if all active players have readied, then this player joins in the middle of the game
                if (manager.allReady() == true)
                {
                    manager.joinMid(Context.ConnectionId, 0, "Bob");
                }
                //otherwise they join at prestart of game
                else
                {
                    manager.joinStart(Context.ConnectionId, 0, "Bob");
                }
                manager.updateState(Convert.ToInt32(roomID));
                Groups.Add(Context.ConnectionId, roomID);
                UserModel user = new UserModel(3);
                Clients.All.alertJson(user);
            }
            //room is full, alert client
            else
            {
                Clients.Caller.alertMessage("Error: Room is full");
            }
        }
        public void Send(string roomID, string message)
        {

            string connid = Context.ConnectionId;
            Clients.Group(roomID).alertMessage(message + " from " + connid);
            //Clients.All.alertJson(message + " from " + connid);
            //hello
        }
        public void broadcastHand(string roomID, string userID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            string hand = manager.getPlayerHand(userID);
            //Clients.Client(userID).updateHand(string hand);
        }
        public void broadcastBoard(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            string board = manager.getBoard();
            //Clients.Group(roomID).updateBoard(string board);
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
            //Clients.Group(roomID).updateCallAmt(manager.getCallAmt());
            return manager.getCallAmt();
        }
        public void broadcastPot(string roomID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            int pot = manager.getPot();
            //Clients.Group(roomID).updatePot(int pot);
        }
        public void Fold(string roomID, string userID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //if user is allowed to move
            if (userID.Equals(manager.getCurrentPlayer().ID))
            {
                //fold
                manager.fold(userID);
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
        public int Call(string roomID, string userID)
        {
            GameManager manager= new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            if ( userID.Equals(manager.getCurrentPlayer().ID))
            {
                manager.call(userID);
                manager.cycle();
                manager.updateState(Convert.ToInt32(roomID));
                //need to take place after updating state, in order to grab up to date information
                adjustBoard(roomID);
                broadcastPot(roomID);
                return manager.getCallAmt();
            }
            else
            {
                Clients.Caller.alertMessage("Error: Not your turn");
                return -1;
            }
        }     
        public int Check(string roomID, string userID)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //It must be their turn, and no pets should have been made in order to check
            //verify with group later on exact check rulings
            if((userID.Equals(manager.getCurrentPlayer().ID))&&(manager.getPot()==0))
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
        public int Raise(string roomID, string userID, int amount)
        {
            GameManager manager = new GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //verify user can raise by that amount/call at tsame time
            if (manager.raise(userID, amount) >= 0)//check for nonnegative number
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
                Clients.Caller.alertMessage("Error: Not your turn");
                return -1;
            }
        }
        //call this function when player confirms they are ready
        //should disallow use of button after function returns true, to prevent spam 
        //This function is what starts the game, when the last player confirms ready
        public bool confirmReady(string roomID, string userID)
        {
            GameManager manager = new PokerGame.GameManager();
            manager.getState(Convert.ToInt32(roomID));
            //first off, check to see the game hasn't already started.
            //if allReady already returns true, this function shouldn't be able to be called
            //check anyways
            if (manager.allReady() == false)
            {
                manager.readyPlayer(userID);
                //start the game if all ready
                if (manager.allReady())
                {
                    //assign all players their cards
                    manager.init();
                    //grab activePlayer[0].ID and notify them it is there turn
                    alertPlayerTurn(manager.getCurrentPlayer().ID);
                    return true;
                    //wooooooo start the game
                }
                else
                {
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
                    broadcastBoard(roomID);
                    manager.nextRound();
                    manager.updateState(Convert.ToInt32(roomID));
                }
                else if (manager.getRoundNumber()<4)//1, 2, 3 completed
                {
                    manager.addBoard();
                    broadcastBoard(roomID);
                    manager.nextRound();
                    manager.updateState(Convert.ToInt32(roomID));
                }
                else
                {
                    //final round of betting complete, game is over
                    gameOver(roomID);
                }
            }

        }
        //function to alert given conn ID that it is its turn
        public void alertPlayerTurn(string userID)
        {
            Clients.Client(userID).alertMessage("Your turn");
        }

    }
}
