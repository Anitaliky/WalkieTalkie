using Common.Networking;
using System;
using Common.Networking.Definitions;
using Data;

namespace WalkieTalkieServer
{
    public static class OperationHandlers
    {
        public static void SignIn(Session s, InPacket p)
        {
            string username = p.ReadString();
            string password = p.ReadString();
            Client client = Program.Server.GetClient(s.Id);
            OutPacket outP = new OutPacket(ServerOperation.SIGN_IN);
            if (client.IsConnected)
            {
                outP.WriteByte((byte)ResponseType.USER_ALREADY_CONNETED);
                s.Send(outP);
                return;
            }
            using (Query query = client.ExecuteQuery($"SELECT pass FROM users WHERE username='{username}'"))
                if (!query.NextRow())
                    outP.WriteByte((byte)ResponseType.USER_DOESNT_EXIST);
                else
                {
                    string realPassword = query.Get<string>("pass");
                    if (password == realPassword)
                        outP.WriteByte((byte)ResponseType.SUCCESS);
                    else
                        outP.WriteByte((byte)ResponseType.WRONG_DETAILS);
                }
            s.Send(outP);
        }

        //public static void SignUp(Session s, InPacket p)
        //{
        //    OutPacket outP = new OutPacket(ServerOperation.SIGN_UP);
        //    int usernameLen = 0;
        //    if(Int32.TryParse(p.ReadString(2),out usernameLen))
        //    {
        //        string username = p.ReadString(usernameLen);
        //        int passwordLen = 0;
        //        if (Int32.TryParse(p.ReadString(2), out passwordLen))
        //        {

        //        }
        //    }
        //}
    }
}
