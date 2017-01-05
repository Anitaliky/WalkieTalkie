using Common.Networking;
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
            using (Query query = client.ExecuteQuery($"SELECT pass FROM users WHERE username='{username}';"))
                if (!query.NextRow())
                    outP.WriteByte((byte)ResponseType.USER_DOESNT_EXIST);
                else
                {
                    string realPassword = query.Get<string>("pass");
                    if (password == realPassword)
                    {
                        outP.WriteByte((byte)ResponseType.SUCCESS);
                        client.IsConnected = true;
                    }
                    else
                        outP.WriteByte((byte)ResponseType.WRONG_DETAILS);
                }
            s.Send(outP);
        }

        public static void SignUp(Session s, InPacket p)
        {
            string username = p.ReadString();
            string password = p.ReadString();
            Client client = Program.Server.GetClient(s.Id);
            OutPacket outP = new OutPacket(ServerOperation.SIGN_UP);
            using (Query query = client.ExecuteQuery($"SELECT pass FROM users WHERE username='{username}';"))
                if (query.NextRow())
                {
                    outP.WriteByte((byte)ResponseType.USERNAME_ALREADY_EXISTS);
                    s.Send(outP);
                    return;
                }
            if (Validator.IsValidUsername(username))
                if (Validator.IsValidPassword(password))
                    if (client.ExecuteNonQuery($"INSERT INTO users(username,pass) values('{username}','{password}');"))
                        outP.WriteByte((byte)ResponseType.SUCCESS);
                    else
                        outP.WriteByte((byte)ResponseType.OTHER);
                else
                    outP.WriteByte((byte)ResponseType.INVALID_PASSWORD);
            else
                outP.WriteByte((byte)ResponseType.INVALID_USERNAME);
            s.Send(outP);
        }
    }
}
