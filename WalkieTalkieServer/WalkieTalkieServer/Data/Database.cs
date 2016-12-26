using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{
    public class Database
    {
        public static Database Instance { get; } = new Database();
        private const string m_file = "database.json";
        private Dictionary<string, string> m_config;
        private Dictionary<string, Dictionary<string, Column>> Tables;
        private object m_lock = new object();

        public MySqlConnection Connection { get; set; }

        private Database()
        {
            LoadConfiguration();
            InitializeTables();
        }

        private void LoadConfiguration()
        {
            m_config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(m_file));
            StringBuilder sb = new StringBuilder();
            sb.Append($"server={m_config["host"]}; ");
            sb.Append($"database={m_config["schema"]}; ");
            sb.Append($"uid={m_config["username"]}; ");
            sb.Append($"password={m_config["password"]}; ");
            sb.Append("convertzerodatetime=yes;");
            Connection = new MySqlConnection(sb.ToString());
            Connection.Open();
        }

        private void InitializeTables()
        {
            Tables = new Dictionary<string, Dictionary<string, Column>>();
            string query = $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{m_config["schema"]}'";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(Connection, query))
            {
                while (reader.Read())
                {
                    Dictionary<string, object> entries = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        entries.Add(reader.GetName(i), reader.GetValue(i));
                    dynamic datum = new Datum("COLUMNS", entries);
                    string tableName = datum.TABLE_NAME;
                    if (!Tables.ContainsKey(tableName))
                        Tables.Add(tableName, new Dictionary<string, Column>());
                    Tables[tableName].Add(datum.COLUMN_NAME, new Column(datum));
                }
            }
        }

        public string CorrectFields(string fields)
        {
            string final = string.Empty;
            string[] tokens = fields.Replace(",", " ").Replace(";", " ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int processed = 0;
            foreach (string field in tokens)
            {
                final += field;
                if (++processed < tokens.Length)
                    final += ", ";
            }
            return final;
        }

        public long Execute(string query, bool sync = true)
        {
            if (!sync)
            {
                //MySqlHelper.ExecuteNonQuery(Connection, query);
                MySqlCommand cmd = Connection.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
            lock (m_lock)
            {
                //MySqlHelper.ExecuteNonQuery(Connection, query);
                MySqlCommand cmd = Connection.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
        }

        public MySqlDataReader ExecuteReader(string query)
        {
            return MySqlHelper.ExecuteReader(Connection, query);
        }

        public void Execute(IEnumerable<string> queries, bool sync = true)
        {
            if (!sync)
            {
                foreach (string query in queries)
                    Execute(query);
                return;
            }
            lock (m_lock)
                foreach (string query in queries)
                    Execute(query, false);
        }

        public void ExecuteSynchronously(Action action)
        {
            lock (m_lock)
                action();
        }

        public bool Exists(string table, string constraints, params object[] args)
        {
            using (MySqlDataReader reader = ExecuteReader($"SELECT * FROM {table} WHERE {string.Format(constraints, args)}"))
                return reader.HasRows;
        }

        public dynamic Fetch(string table, string field, string constraints, params object[] args)
        {
            dynamic value = new Datum(table).SelectFields(field, constraints, args).Dictionary[field];
            if (value is DBNull)
                return null;
            else if (value is byte && IsBool(table, field))
                return (byte)value != 0;
            return value;
        }

        #region Metadata

        public bool IsBool(string tableName, string fieldName)
        {
            return Tables[tableName][fieldName].ColumnType == "tinyint(1) unsigned";
        }

        public bool IsDate(string tableName, string fieldName)
        {
            return Tables[tableName][fieldName].ColumnType == "date";
        }

        public bool IsDateTime(string tableName, string fieldName)
        {
            return Tables[tableName][fieldName].ColumnType == "datetime";
        }
        
        #endregion
    }
}
