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
            OutPacket outP = new OutPacket(ServerOperation.SIGN_IN);
            int usernameLen = 0;
            if (!Int32.TryParse(p.ReadString(2), out usernameLen))
            {
                outP.WriteByte((byte)ResponseType.OTHER);
                goto ToSend;
            }

            string username = p.ReadString(usernameLen);
            int passwordLen = 0;
            if (!Int32.TryParse(p.ReadString(2), out passwordLen))
            {
                outP.WriteByte((byte)ResponseType.OTHER);
                goto ToSend;
            }

            if (s.IsConnected)
            {
                outP.WriteByte((byte)ResponseType.USER_ALREADY_CONNETED);
                goto ToSend;
            }

            string password = p.ReadString(passwordLen);
            Query query = s.ExecuteQuery($"SELECT pass FROM users WHERE username='{username}';");
            if (query.IsNullOrEmpty("user"))
                outP.WriteByte((byte)ResponseType.USER_DOESNT_EXIST);
            else
            {
                string realPassword = query.Get<string>("pass");
                if (password == realPassword)
                    outP.WriteByte((byte)ResponseType.SUCCESS);
                else
                    outP.WriteByte((byte)ResponseType.WRONG_DETAILS);
            }

            ToSend:
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
