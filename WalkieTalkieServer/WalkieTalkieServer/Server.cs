using Common.Networking;
using Common.Networking.Definitions;
using System;
using System.Collections.Generic;
using static Common.Networking.Session;

namespace WalkieTalkieServer
{
    public class Server
    {
        private Session session;
        private Dictionary<long, Session> clients;
        private Dictionary<ClientOperation, Action<Session, InPacket>> handlers;

        public Server()
        {
            session = new Session();
            clients = new Dictionary<long, Session>();
            handlers = new Dictionary<ClientOperation, Action<Session, InPacket>>();

            session.Listen(4242);
            session.ClientConnected += OnClientConnected;
            session.ClientDisconnected += OnClientDisconnected;
            session.MessageReceived += OnMessageReceived;

            CreateHandlers();
        }

        #region Events

        private void OnClientConnected(object sender, SessionEventArgs e)
        {
            clients.Add(e.Session.Id, e.Session);
        }

        private void OnClientDisconnected(object sender, SessionEventArgs e)
        {
            clients.Remove(e.Session.Id);
        }

        private void OnMessageReceived(object sender, MessageReceiveEventArgs e)
        {
            ClientOperation messageType = (ClientOperation)e.Message.ReadInt();
            if (!handlers.ContainsKey(messageType))
            {
                Console.WriteLine($"Unhandled message: {messageType}");
                return;
            }
            handlers[messageType](e.Session, e.Message);
        }

        #endregion

        private void CreateHandlers()
        {
            handlers.Add(ClientOperation.SignIn, OperationHandlers.SignIn);
        }
    }
}
