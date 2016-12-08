/*

 * TODO:
 * -Need gameData to track phase of the game
 * -GameOver function should only check if game is over by player Folds
 * -award needs to update player database with new currency totals
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Web.Helpers;
using Newtonsoft.Json;
using HoldemHand;
using System.Web.Mvc;
using PokerGame;
using PokerPrototype.Models;
//using System.Security.Cryptography;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace PokerGame
{
    //CITEE http://stackoverflow.com/questions/273313/randomize-a-listt
    //Copied from link above, should improve Random so that our shuffle can handle 
    //threads, and reduce possibility of shuffling in predictable ways
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
    //MODEL
    public class Card
    {
        //future note, treat all 1's as spades. Pseudo "14" and "1"
        //using values 1-13, where 1 is ace, 11 is jack, 12 is queen, and 13 is king
        //represent as string in order for hand scoring using Keith's Rule (Format "As"= ace of spades)
        public string value { get; set; }
        public void printCard()
        {
            Console.WriteLine("{0}", this.value);
        }
    }
    //MODEL
    public class Deck
    {
        //General Master List of cards. We're keeping this just in case
        private Card[] cards;
        //List of all cards currently located in the deck
        List<Card> deckCards;
        //List of all cards currently in game. Either on table, or in hand
        List<Card> gameCards;
        //constructor creates array of 52 cards to serve as a deck
        public Deck()
        {
            cards = new Card[52];
            //initialize empty lists
            deckCards = new List<Card> { };
            gameCards = new List<Card> { };
            int i = 0;
            //Adding Spades Cards
            for (i = 0; i < 52; i++)
            {
                cards[i] = new Card();
            }
            cards[0].value = "2s";
            cards[1].value = "2h";
            cards[2].value = "2d";
            cards[3].value = "2c";
            cards[4].value = "3s";
            cards[5].value = "3h";
            cards[6].value = "3d";
            cards[7].value = "3c";
            cards[8].value = "4s";
            cards[9].value = "4h";
            cards[10].value = "4d";
            cards[11].value = "4c";
            cards[12].value = "5s";
            cards[13].value = "5h";
            cards[14].value = "5d";
            cards[15].value = "5c";
            cards[16].value = "6s";
            cards[17].value = "6h";
            cards[18].value = "6d";
            cards[19].value = "6c";
            cards[20].value = "7s";
            cards[21].value = "7h";
            cards[22].value = "7d";
            cards[23].value = "7c";
            cards[24].value = "8s";
            cards[25].value = "8h";
            cards[26].value = "8d";
            cards[27].value = "8c";
            cards[28].value = "9s";
            cards[29].value = "9h";
            cards[30].value = "9d";
            cards[31].value = "9c";
            cards[32].value = "10s";
            cards[33].value = "10h";
            cards[34].value = "10d";
            cards[35].value = "10c";
            cards[36].value = "js";
            cards[37].value = "jh";
            cards[38].value = "jd";
            cards[39].value = "jc";
            cards[40].value = "qs";
            cards[41].value = "qh";
            cards[42].value = "qd";
            cards[43].value = "qc";
            cards[44].value = "ks";
            cards[45].value = "kh";
            cards[46].value = "kd";
            cards[47].value = "kc";
            cards[48].value = "as";
            cards[49].value = "ah";
            cards[50].value = "ad";
            cards[51].value = "ac";
            for (i = 0; i < 52; i++)
            {
                deckCards.Add(cards[i]);
            }
        }
        //CITE:http://stackoverflow.com/questions/273313/randomize-a-listt 
        public void shuffle()
        {
            int n = deckCards.Count;//for amount of cards in deck currently
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                Card temp = deckCards[k];
                deckCards[k] = deckCards[n];
                deckCards[n] = temp;
            }

        }
        //draw continually from index 0, which is the top of the deck
        public string draw()
        {
            //make sure deck actually has a card to draw
            if (deckCards.Count > 0)
            {
                //retrieve card from deck
                Card drawn = deckCards[0];
                //remove from deck
                deckCards.RemoveAt(0);
                //add to lsit of cards in game
                gameCards.Add(drawn);
                /*
                Console.WriteLine("Drawn: ");
                drawn.printCard();
                */
                return drawn.value;
            }
            return "";
        }
        //takes all cards in play, readds them to deck, then reshuffles
        public void cleanup()
        {
            //re-add from latest index to 0 for simplicity
            //and to work around removes
            while (gameCards.Count > 0)
            {
                deckCards.Add(gameCards[0]);
                gameCards.RemoveAt(0);
            }
            //deck now contains all cards again
            shuffle();
        }
        public List<Card> getDeckCards()
        {
            return deckCards;
        }
        public List<Card> getGameCards()
        {
            return gameCards;
        }
        public void setDeckCards(List<Card> list)
        {
            deckCards = list;
        }
        public void setGameCards(List<Card> list)
        {
            gameCards = list;
        }
        //TESTING FUNCTIONS-----------------------------------------------------------------------------------
        //These functions are prefaced with either "check" or "print", and are used for testing exclusively
        public bool checkShuffle()
        {
            //first shuffle, then print deck for visual comparison
            shuffle();
            printDeck();
            //code chunk checks for duplicate cards
            //for every card currently in the deck
            for (int i = 0; i < deckCards.Count; i++)
            {

                //go through the rest of the deck
                for (int x = 0; x < deckCards.Count; x++)
                {
                    //Note: Probably a more efficient way to do this
                    //Set up equivalency function later in Card class?
                    //duplicate occurs if two cards in different locations hold same value
                    if ((deckCards[x].value.Equals(deckCards[i].value)) && (x != i))
                    {
                        Console.WriteLine("Cards Equivalent were:");
                        deckCards[x].printCard();
                        deckCards[i].printCard();
                        Console.WriteLine("{0}=x and i={1} ", x, i);
                        return false;
                    }
                }
            }
            return true;
        }
        public void checkDraw()
        {
            for (int i = 0; i < 53; i++)
            {
                draw();
            }
        }
        public void checkCleanup()
        {
            for (int i = 0; i < deckCards.Count; i++)
            {
                draw();
            }
            cleanup();
            //clean up already shuffles so printdeck
            printDeck();
            Console.WriteLine(checkShuffle());
        }
        //function to print deck.
        //This is not to display deck, prints to console for error checking
        public void printDeck()
        {
            for (int i = 0; i < deckCards.Count; i++)
            {
                deckCards[i].printCard();
            }
            //Console.WriteLine("Size of Deck={0}", deckCards.Count);
        }
        public void printGameCards()
        {
            for (int i = 0; i < gameCards.Count; i++)
            {
                gameCards[i].printCard();
            }
        }
    }
    //container class for Player information (currency amt, name, hand[represented as string])
    public class Player
    {
        public int currency { get; set; }
        public string ID { get; set; }
        public string name { get; set; }
        public string hand { get; set; }
        //whether or not Player is in the current game (They have not folded)
        public bool folded { get; set; }
        //is player ready to start match
        public bool ready { get; set; }
        //mark if player has left the game
        //public bool leave { get; set; }
    }

    //Holds all data necessary for GameManager
    //Will be serialized to JSON for storage/retrieval of game state
    //MODEL
    public class GameData
    {
        //Attributes-------------------------------------------------------------------
        //ID of room itself
        public int roomID { get; set; }
        //maximum size of room, default to six;
        public int roomCap { get; set; }
        //current number of players in room
        public int playerCount { get; set; }
        //last bet number. calls must meet this, raises must beat it
        public int callAmt { get; set; }
        //track number of raises (limit 3)
        public int raiseCount { get; set; }
        //total amount in pot, init to zero
        public int pot { get; set; }
        //what number betting round game is on. 0 for pre flop, 1 for post flop (three cards added to board), 
        //2 for next card added, 3 for the fith card added (final betting round) 
        public int bettingRound { get; set; }
        //potential check against too many cards on board
        public int boardCount { get; set; }
        //string representation of board. Included for internal hand evaluator.
        public string board { get; set; }
        //list representation of cards currently in the board. Included in case needed for graphical representation
        public List<Card> boardCards;
        //list of active players currently in game. Players are listed in turn order (index 0 goes first, then index 1, etc)        
        public List<Player> activePlayers;
        //list of inactive players not in the game. Included for potential integration issues with SignalR
        public List<Player> inactivePlayers;
        //list of cards (in order) in deck
        public List<Card> deckCards;
        //list of cards (in order drawn) out on field
        public List<Card> gameCards;
        //two ways to track which player is currently going
        public Player currentPlayer { get; set; }
        public int currentIndex { get; set; }
        //checks if players have accomplished one cycle (marked by passing the "end" of list and returning to beginning
        public bool cycleComplete { get; set; }
        public Deck deck;

        public GameData()
        {
            //set roomID to invalid number. May need to test if room was just made/not in database
            //alternatively let SQL return nothing?
            roomID = -1;
            roomCap = 6;
            playerCount = 0;
            callAmt = 0;
            raiseCount = 0;
            pot = 0;
            bettingRound = 0;
            boardCount = 0;
            board = "";
            boardCards = new List<Card> { };
            activePlayers = new List<Player> { };
            inactivePlayers = new List<Player> { };
            deckCards = new List<Card> { };
            gameCards = new List<Card> { };
            currentIndex = 0;
            cycleComplete = false;
            deck = new Deck();
        }

    }


    //Responsible for: All game logic, processing all player actions
    //(call, raise, fold, check)
    //draw/deal cards,determine winner
    //SignalR Hub handles updating clients/maintaining turn order
    //CONTROLLER
    //* getCurrentPlayer()
    // * cycle()
    // * addPlayerTurn()//add player in the cycle of turns
    // * gameOver()
    public class GameManager
    {
        GameData data;
        //include parameter default overrides ("int size=9") later
        public GameManager()
        {
            data = new PokerGame.GameData();

        }
        //Functions--------------------------------------------------------------------
        //TODO: \\ check(), blind(char p, );
        //getwinner(){ 
        // list of winners, usually only contains
        // loop through the active players
        // Hand h1 = new Hand("ad kd", board);
        /// evaluate hand for all players and find maximum
        // in the event of a tie, push onto the winners list
        //intializes beginning state of game
        public void init()
        {
            //now responsibility or reset() function
            /*for (int i = 0; i < data.inactivePlayers.Count; i++)
            {
                //data.inactivePlayers[i].ready = true;
                data.inactivePlayers[i].folded = false;
                data.activePlayers.Add(data.inactivePlayers[i]);
            }
            data.inactivePlayers.Clear();*/
            //clean and shuffle deck
            data.bettingRound = 0;
            data.deck.cleanup();
            data.deck.shuffle();
            data.cycleComplete = false;
            //reset board count to zero
            data.boardCount = 0;
            //empty board
            data.board = "";
            //Cycle through player list twice to deal cards to players, done to aid potential graphics integration
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                //Overwrite will empty previous player hand
                data.activePlayers[i].hand = data.deck.draw();
                //set all active players as currently participating in round
                data.activePlayers[i].folded = false;
            }
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                //append " " and second card to each players hand
                data.activePlayers[i].hand += " " + data.deck.draw();
            }
            data.currentIndex = 0;
            data.currentPlayer = data.activePlayers[0];
        }

        //adds card to board
        public void addBoard()
        {
            //if board is empty
            if (data.board.Equals(""))
            {
                data.board = data.deck.draw();
                data.boardCount = 1;
            }
            else
            {
                //if room to add to board, add
                if (data.boardCount < 5)
                {
                    data.board += " " + data.deck.draw();
                    data.boardCount++;
                }
            }
        }
        //cycle to next player's turn. If they have folded, move on to the next player.
        //call cycle AFTER current player is done moving
        public int cycle()
        {
            int i = data.currentIndex;
            //if we have reached the last player in the list
            if (i == data.activePlayers.Count - 1)
            {
                //restart
                i = 0;
                //we have also completed one betting cycle
                data.cycleComplete = true;
                //this particular betting round has been reset 3 times or not at all
                if ((data.raiseCount > 3) || (data.raiseCount == 0))
                {
                    //progress betting round
                    data.raiseCount = 0;//reset the raise counter
                    data.bettingRound++;
                }
                else
                {
                    //need to cycle again, set cycleComplete to false
                    data.cycleComplete = false;
                }
                //if player at index 0 has folded, search to find next valid player
                if (data.activePlayers[i].folded==true)
                {
                    for(int x=0;x<data.activePlayers.Count;x++)
                    {
                        if (data.activePlayers[x].folded == false)
                        {
                            data.currentIndex = x;
                            data.currentPlayer = data.activePlayers[x];
                            //return betting round
                            return data.bettingRound;
                        }
                    }
                }
                else
                {
                    //go ahead and set player at index 0 as current player
                    data.currentIndex =0;
                    data.currentPlayer = data.activePlayers[0];
                    return data.bettingRound;
                }

            }
            for(int x=i+1; x< data.activePlayers.Count;x++)
            {
                if(data.activePlayers[x].folded==false)
                {
                    data.currentIndex = x;
                    data.currentPlayer = data.activePlayers[x];
                    //return betting round
                    return data.bettingRound;
                }
            }
            //error, no one can move (everyone managed to all fold).
            return -1;
        }
        //move to next betting round, resseting cycleComplete status
        public void nextRound()
        {
            data.cycleComplete = false;
        }
        //checks whether game is over or not
        //in event of "true" call getWinner to determine winner of the game
        public bool gameOver()
        {
            //how many players are still allowed to move?
            int ablePlayers = 0;

            for(int i=0; i < data.activePlayers.Count;i++)
            {
                //if they haven't folded, they are able to move
                if(data.activePlayers[i].folded==false)
                {
                    ablePlayers++;
                }
            }
            //only one or fewer players remaining
            if(ablePlayers<=1)
            {
                return true;
            }
            /*Controller should catch natural game end
            //if five cards on the board
            if(data.boardCount==5)
            {
                return true;
            }*/
            return false;
        }
        //marks player as folded.
        public void fold(string ID)
        {
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                if (data.activePlayers[i].ID.Equals(ID))
                {
                    data.activePlayers[i].folded = true;
                }
            }
        }
        //validates and checks raising
        //amount is amount to raise BY (so 10 means beat current bet by 10)
        public int raise(string ID, int amount)
        {
            int raiseTotal = amount + data.callAmt;
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                if (data.activePlayers[i].ID.Equals(ID))
                {
                    //amount must be greater than current call amt, and player must actually have the money
                    if ((raiseTotal > data.callAmt) && (data.activePlayers[i].currency - raiseTotal >= 0))
                    {
                        data.activePlayers[i].currency -= raiseTotal;
                        data.pot += raiseTotal;
                        data.callAmt = raiseTotal;
                        data.raiseCount++;
                        return amount;
                    }
                }
            }
            return -1;
        }
        //validates and checks calling
        //what should happen if not enough to call, automatically deduct currency?
        public int call(string ID)
        {
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                if (data.activePlayers[i].ID.Equals(ID))
                {
                    if (data.activePlayers[i].currency - data.callAmt >= 0)
                    {
                        data.activePlayers[i].currency -= data.callAmt;
                        data.pot += data.callAmt;
                        return data.callAmt;
                    }
                }
            }
            return -1;
        }
        //returns list of ID of winning player(s), delinated by space if more than one winner
        public List<string> getWinner()
        {
            List<string> winners = new List<string> { };
            //copy all non folded players into new list of potential winners
            List<Player> finalPlayers = new List<Player> { };
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                if (data.activePlayers[i].folded == false)
                {
                    finalPlayers.Add(data.activePlayers[i]);
                }
            }
            //start by examining first hand
            Hand h1 = new Hand(finalPlayers[0].hand, data.board);
            //default to first hand being winner (a hand better than no hand)
            winners.Add(finalPlayers[0].ID);
            //comparison hand
            Hand h2;
            for (int i = 1; i < finalPlayers.Count; i++)
            {
                //for every other active player examine hand, compare, replace if necessary
                h2 = new Hand(finalPlayers[i].hand, data.board);

                //new hand is better
                if (h2 > h1)
                {
                    winners.Clear();
                    winners.Add(finalPlayers[0].ID);
                    h1 = h2;
                }
                else if (h1 > h2)
                {
                    //do nothing
                }
                else
                {
                    //tie, push onto winners list
                    winners.Add(finalPlayers[0].ID);
                }
            }
            return winners;
        }
        public void award(List<string> winners)
        {
            //divide pot among all winners
            int award = data.pot / (winners.Count);
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                for (int x = 0; x < winners.Count; i++)
                {
                    if (data.activePlayers[i].ID.Equals(winners[i]))
                    {
                        data.activePlayers[i].currency += award;
                    }
                }
            }
            //pot is now empty
            data.pot = 0;
        }
        //player joins before start of game. Add to active players, and await start of game
        public void joinStart(string IDtag, int money, string username)
        {
            Player temp = new Player();
            temp.ID = IDtag;
            temp.currency = money;
            temp.name = username;
            temp.ready = false;
            //add to inactive players, to become active next round
            data.activePlayers.Add(temp);
            data.playerCount++;
        }
        //player joins midgame. Handles adjusting data, NOT actual network join
        public void joinMid(string IDtag, int money, string username)
        {
            Player temp = new Player();
            temp.ID = IDtag;
            temp.currency = money;
            temp.name = username;
            temp.ready = false;
            //add to inactive players, to become active next round
            data.inactivePlayers.Add(temp);
            data.playerCount++;
        }
        //What to do if player leaves (OnDisconnect)
        //potential error if two simultaneous leaves result in an overflow?
        //may want to try marking for deletion, then having a set clear() at somepoint
        //would interfere with other players joining however.
        public void leave(string IDtag)
        {
            //check if player is leaving as an active Player
            for (int i = 0; i < data.activePlayers.Count; i++)
            {

                if (data.activePlayers[i].ID.Equals(IDtag))
                {
                    //if we want to preserve his hand for some reason, we can play around it
                    /*
                        data.activePlayers[i].folded = true;
                        data.activePlayers[i].leave = true;
                        */
                    //for now just remove them from the list and decrement Playercount
                    data.activePlayers.RemoveAt(i);
                }
            }
                //if player is inactivePlayer, we can go ahead and just remove them
                for (int i = 0; i < data.inactivePlayers.Count; i++)
                {
                    //mark for deletion
                    if (data.inactivePlayers[i].ID.Equals(IDtag))
                    {
                        data.inactivePlayers.RemoveAt(i);

                    }
                }
            }
        //call to mark player with string ID as ready
        public void readyPlayer(string ID)
            {
                for(int i=0; i<data.activePlayers.Count; i++)
                {
                    if(data.activePlayers[i].ID.Equals(ID))
                    {
                        data.activePlayers[i].ready = true;
                    }
                }
            }
        //after game is over, reset ready status of all players, and move inactive players to active.
        public void reset()
            {
                for (int i = 0; i < data.inactivePlayers.Count; i++)
                {
                    data.inactivePlayers[i].ready = false;
                    data.inactivePlayers[i].folded = false;
                    data.activePlayers.Add(data.inactivePlayers[i]);
                }
                data.inactivePlayers.Clear();
            }
        //marks player as having "left", keeping them 
        //returns true if all active players have signaled they are ready to start the game
        public bool allReady()
            {
                //can't play poker with just yourself
                if(data.activePlayers.Count<2)
                {
                    return false;
                }
                for(int i=0; i< data.activePlayers.Count; i++)
                {
                    //return false if one player isn't ready
                    if(data.activePlayers[i].ready==false)
                    {
                        return false;
                    }
                }
                //otherwise return true
                return true;            
            }
        //Database functions
        //also fix to INSERT only if entry doesn't actually exist
        public void updateState(int roomID)
            {
                int room = roomID;
                //save deck/game cards
                data.deckCards = data.deck.getDeckCards();
                data.gameCards = data.deck.getGameCards();
                string output = JsonConvert.SerializeObject(data);
                MySqlConnection Conn = new MySqlConnection(Connection.Str);
                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                Conn.Open();
                cmd.Connection = Conn;
                cmd.CommandText = "SELECT * FROM games WHERE roomID = @room";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@room", room);
                MySqlDataReader rdr = cmd.ExecuteReader();
                //if entry already exists update, if not, insert
                if (rdr.Read())
                {
                    rdr.Close();
                    cmd.CommandText = "UPDATE games SET jsondata = @output WHERE roomID = @roomID";
                    cmd.Parameters.AddWithValue("@output", output);
                    cmd.Parameters.AddWithValue("@roomID", room);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    rdr.Close();
                    cmd.CommandText = "INSERT INTO games (roomID, jsondata) VALUES (@roomID,@output)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@roomID", room);
                    cmd.Parameters.AddWithValue("@output", output);
                    cmd.ExecuteNonQuery();
                    /*                    cmd.CommandText = "INSERT into users(username, password, email, currency) VALUES (@user,@pass,@email, 10) ";
                            cmd.Prepare();
                            cmd.Parameters.AddWithValue("@pass", Crypto.HashPassword(password));
                            cmd.Parameters.AddWithValue("@email", email);
                            success = cmd.ExecuteNonQuery() > 0;
                            if (success)
                            {
                                id = Convert.ToInt32(cmd.LastInsertedId);//.ToString();
                            }
                            Conn.Close();
                     * 
                     * 
                     * */
                }
                Conn.Close();
        }
        public string getState(int roomID)
        {
            MySqlConnection Conn = new MySqlConnection(Connection.Str);
            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            Conn.Close();
            Conn.Open();
            cmd.Connection = Conn;
            cmd.CommandText = "SELECT jsondata FROM games WHERE roomID = " + roomID;
            cmd.Prepare();
            MySqlDataReader rdr = cmd.ExecuteReader();
            //if the entry exists
            if (rdr.Read())
            {
                string json = (string)rdr["jsondata"];
                //change to point to data class held by this
                data = JsonConvert.DeserializeObject<GameData>(json);
                //set deck to be of the values given
                data.deck.setDeckCards(data.deckCards);
                data.deck.setGameCards(data.gameCards);
                Conn.Close();
                return json;
            }
            else
            {
                Conn.Close();
                //on creation, GameManager creates a default Gamedata with a roomID of -1
                //check for this, then update roomID and store in db
                //return empty string, state doesn't exist as room hasn't been created
                return "";
            }
            
            
        }
        //Need to be able to set roomID value
        public void setRoomID(int num)
        {
            data.roomID = num;
        }
        //GET functions for integration with web interface
        //Unless otherwise stated, returns the specified variable with no modifications
        public int getRoomID()
        {
            return data.roomID;
        }
        public int getCallAmt()
        {
            return data.callAmt;
        }
        public int getRaiseCount()
        {
            return data.raiseCount;
        }
        public string getBoard()
        {
            return data.board;
        }
        public int getBoardSize()
        {
            return data.boardCount;
        }
        public int getPlayerCurrency(string ID)
        {
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                if (data.activePlayers[i].ID.Equals(ID))
                {
                    return data.activePlayers[i].currency;
                }
            }

            return 0;

        }
        public string getPlayerHand(string ID)
        {
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                if (data.activePlayers[i].ID.Equals(ID))
                {
                    return data.activePlayers[i].hand;
                }
            }

            return "";

        }
        public bool getFold(string ID)
        {
            for (int i = 0; i < data.activePlayers.Count; i++)
            {
                if (data.activePlayers[i].ID.Equals(ID))
                {
                    return data.activePlayers[i].folded;
                }
            }

            return false;

        }
        public int getPot()
        {
            return data.pot;

        }
        public Player getCurrentPlayer()
        {
            return data.currentPlayer;
        }
        public int getCurrentIndex()
        {
            return data.currentIndex;
        }
        public int getPlayerCount()
        {
            return data.playerCount;
        }
        public int getRoomCap()
        {
            return data.roomCap;
        }
        public bool getCycleComplete()
        {
            return data.cycleComplete;
        }
        public int getRoundNumber()
        {
            return data.bettingRound;
        }
        }

}
class Program
{
    static void Main(string[] args)
    {
        /*         Deck testDeck = new Deck();
                 // testDeck.shuffle();
                 testDeck.printDeck();
                 Console.WriteLine(testDeck.checkShuffle());
                 testDeck.checkDraw();
                 testDeck.checkCleanup();
                 Console.ReadLine();
        */
        /*deprecated
        GameManager manager = new GameManager();
        manager.join("1", 100, "Bob");
        manager.join("2", 150, "Joe");
        manager.init();
        manager.updateState();*/
    }
}


