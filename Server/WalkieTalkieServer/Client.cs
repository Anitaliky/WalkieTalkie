using Common.Networking;
using Data;

namespace WalkieTalkieServer
{
    public class Client
    {
        private Session m_session;
        private Schema m_schema;
        public bool IsConnected;

        public Client(Session session)
        {
            m_session = session;
            IsConnected = false;
        }

        public Query ExecuteQuery(string cmd)
        {
            if (m_schema == null)
                m_schema = Database.Instance.Connect("walkietalkie");
            return m_schema.ExecuteQuery(cmd);
        }

        public bool ExecuteNonQuery(string cmd)
        {
            if (m_schema == null)
                m_schema = Database.Instance.Connect("walkietalkie");
            return m_schema.ExecuteNonQuery(cmd);
        }
    }
}
