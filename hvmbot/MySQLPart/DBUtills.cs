using hvmbot;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace vkMCBot.Mysql
{
    class DBUtills
    {
        public static MySqlConnection GetDBConnection()
        {
            //CREATE USER 'myuser'@'%' IDENTIFIED BY 'mypass';
            //GRANT ALL ON databasename.* TO 'myuser'@'%';

            string host = Configuration.DBAuth.GetHost(); 
            int port = Configuration.DBAuth.GetPort();
            string database = Configuration.DBAuth.GetDatabaseName();
            string username = Configuration.DBAuth.GetUsername();
            string password = Configuration.DBAuth.GetPassword();

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
