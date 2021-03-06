﻿using Common.Networking;
using Common.Networking.Definitions;
using System;
using System.Collections.Generic;
using static Common.Networking.Session;

namespace WalkieTalkieServer
{
    public class Server
    {
        private Session session;
        private Dictionary<long, Client> clients;
        private Dictionary<ClientOperation, Action<Session, InPacket>> handlers;

        public Server()
        {
            session = new Session();
            clients = new Dictionary<long, Client>();
            handlers = new Dictionary<ClientOperation, Action<Session, InPacket>>();

            session.Listen(4242);
            Console.WriteLine("Listening");
            session.ClientConnected += OnClientConnected;
            session.ClientDisconnected += OnClientDisconnected;
            session.MessageReceived += OnMessageReceived;

            CreateHandlers();
        }

        public Client GetClient(long id)
        {
            return clients[id];
        }

        #region Events

        private void OnClientConnected(object sender, SessionEventArgs e)
        {
            clients.Add(e.Session.Id, new Client(e.Session));
            Console.WriteLine("Socket connceted!");
        }

        private void OnClientDisconnected(object sender, SessionEventArgs e)
        {
            clients.Remove(e.Session.Id);
        }

        private void OnMessageReceived(object sender, MessageReceiveEventArgs e)
        {
            ClientOperation messageType = (ClientOperation)e.Message.ReadByte();
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
            handlers.Add(ClientOperation.SIGN_IN, OperationHandlers.SignIn);
            handlers.Add(ClientOperation.SIGN_UP, OperationHandlers.SignUp);
        }
    }
}
