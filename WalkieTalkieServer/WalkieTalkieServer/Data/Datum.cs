using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Server.Data
{
    public class Datum : DynamicObject
    {
        public string Table { get; private set; }

        internal Dictionary<string, object> Dictionary { get; set; }

        public Datum(string table)
        {
            Table = table;
            Dictionary = new Dictionary<string, object>();
        }

        internal Datum(string table, Dictionary<string, object> values)
        {
            Table = table;
            Dictionary = values;
        }

        internal void ExecuteQuery(string query)
        {
            using (MySqlDataReader reader = Database.Instance.ExecuteReader(query))
            {
                if (reader.RecordsAffected > 1)
                    throw new RowNotUniqueException();
                if (!reader.HasRows)
                    throw new RowNotInTableException();
                reader.Read();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    object value = reader.GetValue(i);
                    if (Dictionary.ContainsKey(name))
                        Dictionary[name] = value;
                    else
                        Dictionary.Add(name, value);
                }
            }
        }

        public dynamic Select(string constraints, params object[] args)
        {
            ExecuteQuery($"SELECT * FROM {Table} WHERE {string.Format(constraints, args)}");
            return this;
        }

        public dynamic SelectFields(string fields, string constraints, params object[] args)
        {
            ExecuteQuery($"SELECT {Database.Instance.CorrectFields(fields)} FROM {Table} WHERE {string.Format(constraints, args)}");
            return this;
        }

        public void Insert()
        {
            string fields = "( ";
            int processed = 0;
            foreach (KeyValuePair<string, object> entry in Dictionary)
            {
                fields += entry.Key;
                if (++processed < Dictionary.Count)
                    fields += ", ";
            }
            fields += " ) VALUES ( ";

            processed = 0;
            foreach (KeyValuePair<string, object> entry in Dictionary)
            {
                fields += string.Format("'{0}'", entry.Value is bool ? (((bool)entry.Value) ? 1 : 0) : entry.Value);
                if (++processed < Dictionary.Count)
                    fields += ", ";
            }
            fields += " )";

            Database.Instance.Execute($"INSERT INTO {Table} {fields}");
        }

        public void Update(string constraints, params object[] args)
        {
            int processed = 0;
            string fields = string.Empty;
            foreach (KeyValuePair<string, object> entry in Dictionary)
            {
                fields += string.Format("{0}='{1}'", entry.Key, entry.Value is bool ? (((bool)entry.Value) ? 1 : 0) : entry.Value);
                if (++processed < Dictionary.Count)
                    fields += ", ";
            }
            Database.Instance.Execute($"UPDATE {Table} SET {fields} WHERE {string.Format(constraints, args)}");
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!Dictionary.ContainsKey(binder.Name))
            {
                result = default(object);
                return false;
            }
            if (Dictionary[binder.Name] is DBNull)
                result = null;
            else if (Dictionary[binder.Name] is byte && Database.Instance.IsBool(Table, binder.Name))
                result = (byte)Dictionary[binder.Name] > 0;
            else
                result = Dictionary[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value is DateTime)
            {
                if (Database.Instance.IsDate(Table, binder.Name))
                    value = ((DateTime)value).ToString("yyyy-MM-dd");
                else if (Database.Instance.IsDateTime(Table, binder.Name))
                    value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (Dictionary.ContainsKey(binder.Name))
                Dictionary[binder.Name] = value;
            else
                Dictionary.Add(binder.Name, value);
            return true;
        }

        public override string ToString()
        {
            string result = Table + " [ ";
            int processed = 0;
            foreach (KeyValuePair<string, object> entry in Dictionary)
            {
                result += entry.Key;
                if (++processed < Dictionary.Count)
                    result += ", ";
            }
            result += " ]";
            return result;
        }
    }
}
