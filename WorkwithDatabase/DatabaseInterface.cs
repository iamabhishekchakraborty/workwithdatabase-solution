using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

namespace WorkwithDatabase
{
    class DatabaseInterface
    {
        static SqlConnection databaseConnection = null;

        /// Provides database connection    
        public static SqlConnection getDBConnection(string connectionString)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
            databaseConnection = new SqlConnection(connectionString);
            return databaseConnection;
        }// Database Connection    

        /// Retrive data from table        
        /*
        public static DataTable getData()
        {
            try
            {
                DataSet ds = new DataSet();
                string sqlStatement = "SELECT Name,Address,Active FROM FeedFiletoDB";
                getDBConnection().Open();
                SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection);
                myDatabaseAdapter.Fill(ds, "FeedFiletoDB");
                return ds.Tables["FeedFiletoDB"];
            }
            catch (Exception excp)
            {
                throw excp;
            }
            finally
            {
                if (databaseConnection != null)
                {
                    databaseConnection.Close();
                }
            } 
        }
        */
    }
}
