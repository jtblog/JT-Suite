using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JT_Suite
{
    class JTDBConnection
    {
        public SqlConnection SQL_Conn;
        public static JTDBConnection mInstance;
        List<Facebook_User> F_Users = new List<Facebook_User>();

        public JTDBConnection()
        {
            //constructor
        }
        ~JTDBConnection()
        {
            //destructor
            SQL_Conn = null;
        }
        public static JTDBConnection getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new JTDBConnection();
            }
            return mInstance;
        }

        public void Disconnect() {
            SQL_Conn.Close();
        }

        public String Connect() {
            SQL_Conn = new SqlConnection(JT_Suite.Properties.Settings.Default.JTDBConnectionString);
            SQL_Conn.Open();
            return SQL_Conn.State.ToString();
        }

        public int Insert_Facebook_User(String _name, String _email, String phone_number, String _accesstoken) {

            if (!(SQL_Conn.State.ToString() == "Open")) {
                this.Connect();
            }

            String sqlCommandtxt = "INSERT INTO Facebook_Users VALUES(@Facebook_Name_Column,@Facebook_Email_Column,@Facebook_PhoneNumber_Column,@Facebook_AccessToken_Column)";
            SqlCommand SQL_Command = new SqlCommand(sqlCommandtxt, SQL_Conn);

            SQL_Command.Parameters.Add(new SqlParameter("Facebook_Name_Column", _name));
            SQL_Command.Parameters.Add(new SqlParameter("Facebook_Email_Column", _email));
            SQL_Command.Parameters.Add(new SqlParameter("Facebook_PhoneNumber_Column", phone_number));
            SQL_Command.Parameters.Add(new SqlParameter("Facebook_AccessToken_Column", _accesstoken));

            return SQL_Command.ExecuteNonQuery();
        }

        public void Display() {
            F_Users.Clear();

            if (!(SQL_Conn.State.ToString() == "Open"))
            {
                this.Connect();
            }

            String sqlCommandtxt = "SELECT * FROM Facebook_Users";
            SqlCommand SQL_Command = new SqlCommand(sqlCommandtxt, SQL_Conn);

            SqlDataReader reader = SQL_Command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString(0); 
                string email = reader.GetString(1);  
                string phone_number = reader.GetString(2);
                string accesstoken = reader.GetString(3);
                F_Users.Add(new Facebook_User() { Name = name, Email = email, Phone_Number = phone_number, AccessToken = accesstoken });
            }

            foreach (Facebook_User User in F_Users) {
                MessageBox.Show(User.AccessToken);
            }
        }

        //public int Delete_Facebook_UsersTable() {
            //String sqlCommand = "DROP TABLE Facebook_Users";
            //SqlCommand SQLInsert = new SqlCommand();
            //SQLInsert.Connection = SQL_Conn;
            //SQLInsert.CommandType = CommandType.Text;
            //SQLInsert.CommandText = sqlCommand;
            //return SQLInsert.ExecuteNonQuery();
        //}

        //public int Create_Facebook_UsersTable()
        //{
            //String sqlCommand = "CREATE TABLE [dbo].[Facebook_Users] (" +
                            //"[Facebook_Name_Column]        NVARCHAR (MAX) NULL," +
                            //"[Facebook_Email_Column]       NVARCHAR (MAX) NOT NULL," +
                           // "[Facebook_PhoneNumber_Column] NVARCHAR (MAX) NOT NULL," +
                            //"[Facebook_AccessToken_Column] NVARCHAR (MAX) NOT NULL);" ;
            //SqlCommand SQLInsert = new SqlCommand();
            //SQLInsert.Connection = SQL_Conn;
            //SQLInsert.CommandType = CommandType.Text;
            //SQLInsert.CommandText = sqlCommand;
            //return SQLInsert.ExecuteNonQuery();
        //} 
    }

    public class Facebook_User {
        public String Name {
            get;
            set;
        }
        public String Email {
            get;
            set;
        }
        public String Phone_Number {
            get;
            set;
        }
        public String AccessToken {
            get;
            set;
        }
        public string Get_Name() {
            return Name;
        }
        public string Get_AccessToken() {
            return AccessToken;
        }
    }
}
