using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PokerPrototype.Models;
using System.Threading.Tasks;

namespace PokerPrototype.Hubs
{
    public class MyHub : Hub
    {
        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }
        public void GetRoomInfo(string roomID)
        {
            //Context.RequestCookies["MYCOOKIE"].Value; //this is how to access a cookie
            Groups.Add(Context.ConnectionId, roomID);
            UserModel user = new UserModel(3);
            Clients.All.alertJson(user);
        }
        public void Send(string roomID, string message)
        {

            string connid = Context.ConnectionId;
            Clients.Group(roomID).alertMessage(message + " from " + connid);
            //Clients.All.alertJson(message + " from " + connid);
            //hello
        }
    }
}