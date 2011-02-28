using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Winnowing.Models
{
    static public class paperInfoRepository
    {
        public static int Insert(string hashValue, string address, string fingerPrint)
        {
            if (!Find(fingerPrint))
            {
                string insertSQL;
                insertSQL = "INSERT INTO PaperInfo (";
                insertSQL += "HashValue, Address, FingerPrint";
                insertSQL += "VALUES('";
                insertSQL += hashValue + "', '";
                insertSQL += address + "', '";
                insertSQL += fingerPrint + "')";
                SqlCommand cmd = new SqlCommand(insertSQL, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return 1;
            }
            else
                return -1;
        }
        public static int Delete(string fingerPrint)
        {
            if (!Find(fingerPrint))
                return -1;
            else
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM " + "PaperInfo" + " WHERE FingerPrint = '" + fingerPrint + "'", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return 1;
            }
        }
        public static int Update(string newHashValue, string newAddress, string originalFingerPrint)
        {
            if (!Find(originalFingerPrint))
                return -1;
            else
            {
                string updateSQL;
                updateSQL = "UPDATE " + "PaperInfo " + "SET ";
                updateSQL += "HashValue = @HashValue, Address = @Address, FingerPrint = @FingerPrint ";
                updateSQL += "WHERE FingerPrint = @orginalFingerPrint";
                SqlCommand cmd = new SqlCommand(updateSQL, con);

                cmd.Parameters.AddWithValue("@HashValue", newHashValue);
                cmd.Parameters.AddWithValue("@Address", newAddress);
                cmd.Parameters.AddWithValue("@FingerPrint", originalFingerPrint);
                cmd.Parameters.AddWithValue("@originalFingerPrint", originalFingerPrint);

                cmd.ExecuteNonQuery();
                con.Close();
                return 1;
            }
        }

        public static int ShowOverAll()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + "PaperInfo", con);
            con.Open();
            SqlDataReader myReader = cmd.ExecuteReader();
            while (myReader.Read())
            {

            }
            myReader.Close(); con.Close();
            return 1;
        }
        public static bool Find(string fingerPrint)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + "PaperInfo" + " WHERE FingerPring = '" + fingerPrint + "'", con);
            con.Open();
            SqlDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                myReader.Close(); con.Close();
                return true;
            }
            myReader.Close(); con.Close();
            return false;
        }

        private static SqlConnection con = new SqlConnection(@"Data Source = .\SQLEXPRESS;" + "Initial Catalog = paper;Integrated Security = SSPI");
        private static int currentID = 0;
        //public paperInfoRepository()
        //{
        //    //con.ConnectionString = @"Data Source = zeus-pc\SQLEXPRESS;" + "Initial Catalog = paper;Integrated Security = SSPI";
        //}

        public static info GetCurrent()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + "PaperInfo" + " WHERE ID = " + currentID.ToString(), con);
            con.Open();
            SqlDataReader myReader = cmd.ExecuteReader();
            info result = new info();
            if (myReader.Read())
            {
                string s = myReader["FingerPrint"].ToString();
                string[] strlist;
                strlist = s.Split(' ');
                foreach (string str in strlist)
                {
                    int tmp = System.Convert.ToInt32(str);
                    result.fingerPrint.Add(tmp);
                }
                s = myReader["Position"].ToString();
                string[] strlist2;
                strlist2 = s.Split(' ');
                foreach (string str in strlist2)
                {
                    int tmp = System.Convert.ToInt32(str);
                    result.position.Add(tmp);
                }
            }
            myReader.Close(); con.Close();
            return result;
        }
        public static int SetNext()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + "PaperInfo" + " WHERE ID = " + (currentID + 1).ToString(), con);
            con.Open();
            SqlDataReader myReader = cmd.ExecuteReader();
            if (myReader.Read()){
                currentID++;
                con.Close();
                myReader.Close();
                return currentID;
            }
            else{
                con.Close();
                myReader.Close();
                return 0;
            }
        }



           // //if (currentID == 0)
            //SqlCommand cmd = new SqlCommand("SELECT * FROM " + "PaperInfo" + " WHERE ID = " + currentID.ToString(), con);
            //con.Open();
            //SqlDataReader myReader = cmd.ExecuteReader();
            //myReader.Read();
            //if (myReader.Read())
            //    currentID = int.Parse(myReader["ID"].ToString());
            //else
            //    currentID = 0;
            //con.Close(); myReader.Close();
            //return currentID;
        
    }
}
