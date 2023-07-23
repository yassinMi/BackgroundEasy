using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Common
{
    /// <summary>
    ///(first created for fshScr)
    /// </summary>
    public class SqliteMigrator
    {
        private static object _lock = new object();
        private static int GetCurrentPragmaUserVersion(SQLiteConnection connection)
        {
            var com = connection.CreateCommand();
            com.CommandText = "PRAGMA user_version";
            var res = com.ExecuteScalar();
            if (Convert.IsDBNull(res)) return 0;
            return Convert.ToInt32(res);
        }
        private static void SetCurrentPragmaUserVersion(SQLiteConnection connection, int value)
        {
            var com = connection.CreateCommand();
            com.CommandText = $"PRAGMA user_version = {value}";
            com.ExecuteNonQuery();
            return;
        }
        /// <summary>
        /// executes the registred transitions that need to be execute, in order based on the current db user_version
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="TargetAppVesrion"></param>
        /// <returns>an aggregate of the changes that took place as specified in the individual transitions context</returns>
        public static SqliteMigrationRunResult Run(SQLiteConnection connection)
        {
            lock (_lock)
            {
                if (HasRan)
                {
                    throw new SqliteMigrationException("Run was called before");
                }
                HasRan = true;
                if (HasRegistredTransitions == false)
                {
                    throw new SqliteMigrationException("must register transitions and call EndTransitionsRegistration before CreateOrUpdate");
                }
                int currentActualDbVer = GetCurrentPragmaUserVersion(connection);
                List<string> allAddedFields = new List<string>();
                List<string> allRemovedFields = new List<string>();
                if (Transistions.Count == currentActualDbVer)
                    return new SqliteMigrationRunResult() { AddedFields = allAddedFields, RemovedFields = allRemovedFields };
                var non_applied_transitions = Transistions.Where(t => t.DbUserVersion > currentActualDbVer);

                bool fromInitial = false;
                foreach (var t in non_applied_transitions.OrderBy(t => t.DbUserVersion))
                {
                    var tr = connection.BeginTransaction();
                    try
                    {
                        var ctx = new SqliteMigrationContext() { Connection = connection };
                        t.Migrate(ctx);
                        tr.Commit();
                        if (ctx.AddedFields != null)
                            allAddedFields.AddRange(ctx.AddedFields);
                        if (ctx.RemovedFields != null)
                            allRemovedFields.AddRange(ctx.RemovedFields);
                        if (ctx.IsInitial) fromInitial = true;
                    }
                    catch (Exception err)
                    {
                        tr.Rollback();
                        throw new SqliteMigrationException($"transitions {t} threw an exception", err);
                    }

                    SetCurrentPragmaUserVersion(connection, t.DbUserVersion);

                }
                return new SqliteMigrationRunResult() { AddedFields = allAddedFields, RemovedFields = allRemovedFields , FromInitial=fromInitial};

            }
           
           
        }
        private static bool HasRegistredTransitions = false;
        private static bool HasRan = false;
        /// <summary>
        /// for safety, this must be called after registering all transitions and before calling <see cref="SqliteMigrator.CreateOrUpdate(SQLiteConnection, string)"/>
        /// </summary>
        public static void EndTransitionsRegistration()
        {
            lock (_lock)
            {
                if (Transistions.Any() == false)
                {
                    throw new SqliteMigrationException("cannot call EndTransitionsRegistration before registing any transitions");
                }
                HasRegistredTransitions = true;
            }
           
        }
        public static void RegisterTransition(SqliteSchemaTransistion m)
        {
            lock (_lock)
            {
                if (HasRegistredTransitions == true)
                {
                    throw new SqliteMigrationException($"cannot Register Transitions after calling EndTransitionsRegistration");
                }
                if (Transistions.Any(t => t.DbUserVersion >= m.DbUserVersion))
                {
                    throw new SqliteMigrationException($"cannot Register Transition {m} because There are transitions with higher DbUserVersion");
                }
                Transistions.Add(m);
            }
        }
        private static List<SqliteSchemaTransistion> Transistions { get; set; } = new List<SqliteSchemaTransistion>();
    }
    public class SqliteSchemaTransistion
    {
        public SqliteSchemaTransistion(string appVer, int dbVer, Action<SqliteMigrationContext> migrate)
        {
            AppVersion = appVer;
            DbUserVersion = dbVer;
            Migrate = migrate;
        }
        /// <summary>
        /// the app version at which this transition was adedd (not used by SqliteMigrator)
        /// </summary>
        public string AppVersion { get; set; }
        /// <summary>
        /// the pragma user_version integer that this migration results in
        /// </summary>
        public int DbUserVersion { get; set; }
        /// <summary>
        /// the migration or creation process, must run sync and use the Conenction without closing it
        /// </summary>
        public Action<SqliteMigrationContext> Migrate { get; set; }
        public override string ToString()
        {
            return $"{AppVersion}@{DbUserVersion}";
        }
    }
    public class SqliteMigrationRunResult
    {
        /// <summary>
        /// indicates that the db was fully created (useful to void notifying the user about schema chages in case of a db creation)
        /// </summary>
        public bool FromInitial { get; set; }
        /// <summary>
        /// as supplied to the context at Migrate actions
        /// </summary>
        public List<string> AddedFields { get; set; }
        /// <summary>
        /// as supplied to the context at Migrate actions
        /// </summary>
        public List<string> RemovedFields { get; set; }

        /// <summary>
        /// get a user friendly summary of the changes or null if nothing to show
        /// </summary>
        /// <returns></returns>
        public string ToSummaryString()
        {
            StringBuilder sb = new StringBuilder();
            if (AddedFields.Any())
            {
                sb.AppendLine($"- added fields: {string.Join(",", AddedFields)}");
            }
            if (RemovedFields.Any())
            {
                sb.AppendLine($"- removed fields: {string.Join(",", RemovedFields)}");
            }
            return sb.Length==0? null : sb.ToString();
        }


    }
    public class SqliteMigrationContext
    {
        /// <summary>
        /// to be used for the chages (keep open)
        /// </summary>
        public SQLiteConnection Connection { get; set; }
        /// <summary>
        /// setting this is optional, and will be concatenated as is to the Run result. 
        /// </summary>
        public string[] AddedFields;
        /// <summary>
        /// setting this is optional, and will be concatenated as is to the Run result. 
        /// </summary>
        public string[] RemovedFields;
        public bool IsInitial { get; set; }
    }

    public class SqliteMigrationException:Exception
    {
        public SqliteMigrationException(string s) : base(s) { }
        public SqliteMigrationException(string s,Exception inner) : base(s,inner) { }
    }
}
