using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace sqlTest
{
    public partial class Form1 : Form
    {
        string CS="";
        SQLiteConnection liteConn = new SQLiteConnection();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DataTable dt=ExecuteQuery("SELECT * FROM tRPO");
            MessageBox.Show(dt.Rows.Count.ToString());
        }

        private DataTable ExecuteQuery(string sqlQuery)
        {
            var query = new DataTable();
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(CS))
                {
                    sqLiteConnection.Open();
                    using (var cmd = new SQLiteCommand(sqLiteConnection) { CommandText = sqlQuery })
                    {
                        var reader = cmd.ExecuteReader();
                        query.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return query;
        }

        private void ExecuteNonQuery(string sqlQuery)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sqlQuery, liteConn);
                if (liteConn.State == ConnectionState.Closed) { liteConn.Open(); }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void IsertRPO(string sqlQuery)
        {
            try
            {
                using (var conn = new SQLiteConnection(CS))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            cmd.CommandText = sqlQuery;
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openF = new OpenFileDialog();
            if (openF.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(openF.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader sr = new StreamReader(fs);
                string line = "";
                int countLine = File.ReadAllLines(openF.FileName).Count();
                int i = 0;
                string date = DateTime.Now.ToString();
                DateTime dt = DateTime.Now;
                using (var conn = new SQLiteConnection(CS))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {

                            while ((line = sr.ReadLine()) != null)
                            {
                                if (!String.IsNullOrEmpty(line))
                                {
                                    try
                                    {
                                        cmd.CommandText = "insert into tRPO (SPI,DATEADD) values('" + line + "','" + date + "')";
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch { }
                                }
                                i++;
                                label1.Text = countLine.ToString() + "/" + i.ToString();
                                Application.DoEvents();
                            }
                            transaction.Commit();
                        }
                        cmd.Cancel();
                    }
                    conn.Close();
                }
                sr.Close();
                fs.Close();
                MessageBox.Show((DateTime.Now - dt).ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CS = "Data Source=" + Directory.GetCurrentDirectory() + "\\RPO;Version=3;";
            liteConn.ConnectionString = CS;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ExecuteNonQuery("delete from tRPO");
            MessageBox.Show("Done");
        }

        private string getRPO()
        {
            string rpo = "";
            try
            {
                SQLiteCommand cmd = new SQLiteCommand("select SPI from tRPO where STATUS=0 limit 1", liteConn);
                if (liteConn.State == ConnectionState.Closed) { liteConn.Open(); }
                object obj=cmd.ExecuteScalar();
                if (obj != null)
                {
                    rpo = obj.ToString();
                }
                ExecuteNonQuery("update tRPO set STATUS=1 where SPI='"+rpo+"'");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return rpo;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(getRPO());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }


    }


}
