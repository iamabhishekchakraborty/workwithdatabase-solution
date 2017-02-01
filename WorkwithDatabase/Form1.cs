using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Data.Sql;
using System.Web.Security;
using System.Runtime.CompilerServices;

namespace WorkwithDatabase
{
    public partial class Form1 : Form
    {
        private string userName = string.Empty;

        //static SqlConnection databaseConnection = null;
        
        public Form1()
        {
            InitializeComponent();            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //get the current logged in user's username(NTID)
                userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                /*
                DataTable dt = new DataTable();
                dt = DatabaseInterface.getData();

                BindingSource primaryGroupsBinding = new BindingSource();
                primaryGroupsBinding.DataSource = dt;
                dataGridView1.DataSource = primaryGroupsBinding;
                */
                //populate server names
                //string[] localServers = new string[] { "IGTEHYDZSDB01", "IGTEHYDZSDB02", ".\\SQLEXPRESS"};
                //comboBox1.Items.AddRange(localServers);
                
                DataTable dtServer = SqlDataSourceEnumerator.Instance.GetDataSources();
                List<string> server = new List<string>();
                BindingSource bs = new BindingSource();
                
                foreach (DataRow item in dtServer.Rows)
                {
                    server.Add(item["ServerName"].ToString());
                }
                bs.DataSource = server;
                comboBox1.DataSource = bs;
                
                
                //authentication process
                radioButton1.Checked = true;
                textBox1.Text = userName;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                comboBox1.Text = "Please select any server name from the list";
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                //dataGridView1.AutoSize = true;
                //dataGridView1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                //dataGridView2.AutoSize = true;
                dataGridView1.Visible = false;
                dataGridView2.Visible = false;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error!!!");
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            comboBox2.Text = "Please, select any database name";
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            comboBox2.Text = "Please, select any database name";
        }

        //Executed when any radio button is changed
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in this.groupBox4.Controls)
            {
                if (control is RadioButton)
                {
                    RadioButton radio = control as RadioButton;
                    if (radio.Checked)
                    {
                        if (radio.Text == "Use Sql Server Authentication")
                        {                           
                            textBox1.Clear();
                            textBox2.Clear();                            
                            comboBox2.SelectedIndex = -1;
                            comboBox3.SelectedIndex = -1;
                            comboBox4.SelectedIndex = -1;                           
                            textBox1.Enabled = true;
                            textBox2.Enabled = true;
                            textBox1.ReadOnly = false;
                            textBox2.ReadOnly = false;
                            comboBox3.Enabled = false;
                            comboBox4.Enabled = false;
                        }
                        else
                        {
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox1.Enabled = false;
                            textBox2.Enabled = false;                            
                            comboBox2.SelectedIndex = -1;
                            comboBox3.SelectedIndex = -1;
                            comboBox4.SelectedIndex = -1;
                            textBox1.Text = userName;
                            comboBox3.Enabled = false;
                            comboBox4.Enabled = false;
                        }
                    }
                }
            }
            comboBox2.Text = "Please, select any database name";
        }

        private void GetDatabaseConnection([CallerMemberName] string callerName = "")
        {
            try
            {
                string serverName = string.Empty;
                string userId = string.Empty;
                string password = string.Empty;
                string connectionString = string.Empty;
                string sqlStatement = string.Empty;
                string selectedObjectType = string.Empty;
                string sqlStatementObjectName = string.Empty;
                string databaseName = string.Empty;

                //MessageBox.Show(callerName + "called me.");

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                BindingSource primaryGroupsBinding = new BindingSource();

                serverName = comboBox1.SelectedIndex == -1 ? comboBox1.Text : comboBox1.SelectedItem.ToString();
                databaseName = comboBox2.SelectedIndex == -1 ? "master" : comboBox2.GetItemText(comboBox2.SelectedItem);

                if (radioButton1.Checked == true)
                {
                    connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Integrated Security=True;";
                }
                else
                {
                    userId = textBox1.Text;
                    password = textBox2.Text;
                    connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";User ID=" + userId + ";Password=" + password + ";";
                }

                using (SqlConnection databaseConnection = new SqlConnection())
                {
                    databaseConnection.ConnectionString = connectionString;
                    if (callerName == "comboBox2_Click")
                    {
                        sqlStatement = "SELECT name FROM sys.sysdatabases WHERE HAS_DBACCESS(name) = 1 ORDER BY name";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "DatabaseNames");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            comboBox2.DisplayMember = "name";
                            comboBox2.DataSource = dt;
                            //comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                            //comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;
                        }
                    }
                    else if (callerName == "comboBox2_SelectionChangeCommitted")
                    {
                        sqlStatement = "SELECT DISTINCT o.type_desc AS ObjectName,o.type AS ObjectType FROM sys.objects o INNER JOIN sys.schemas s ON o.schema_id = s.schema_id ORDER BY o.type_desc";
                        //upon changing the database list get object types and populate comboBox3
                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectTypes");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            comboBox3.DisplayMember = "ObjectName";
                            comboBox3.ValueMember = "ObjectType";
                            comboBox3.DataSource = dt;
                        }

                        selectedObjectType = comboBox3.SelectedIndex == 0 ? comboBox3.Text : comboBox3.GetItemText(comboBox3.SelectedItem);
                        sqlStatementObjectName = "with objects_cte as ("
                                   + "select case when o.type_desc = 'TYPE_TABLE' then schema_name(t.schema_id)+'.'+t.name else schema_name(o.schema_id)+'.'+o.name end as name,"
                                   + "o.type_desc,o.type,case when o.principal_id is null then s.principal_id else o.principal_id end as principal_id"
                                   + " from sys.objects o inner join sys.schemas s on o.schema_id = s.schema_id"
                                   + " left join sys.table_types t on o.object_id = t.type_table_object_id"
                                   + " where o.type_desc = '" + selectedObjectType + "')"
                                   + " select cte.name as Name"
                                   + " from objects_cte cte inner join sys.database_principals dp on cte.principal_id = dp.principal_id order by cte.name";
                        //+ " where dp.name = <logged in user name>";
                        //populate combo box Object Names
                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatementObjectName, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectNames");
                            dt = ds.Tables[1];
                            primaryGroupsBinding.DataSource = dt;
                            comboBox4.DisplayMember = "Name";
                            comboBox4.DataSource = dt;
                        }

                    }
                    else if (callerName == "comboBox3_SelectionChangeCommitted")
                    {
                        selectedObjectType = comboBox3.SelectedIndex == 0 ? comboBox3.Text : comboBox3.GetItemText(comboBox3.SelectedItem);
                        //populate combo box Object Names               
                        sqlStatementObjectName = "with objects_cte as ("
                                   + "select case when o.type_desc = 'TYPE_TABLE' then schema_name(t.schema_id)+'.'+t.name else schema_name(o.schema_id)+'.'+o.name end as name,"
                                   + "o.type_desc,o.type,case when o.principal_id is null then s.principal_id else o.principal_id end as principal_id"
                                   + " from sys.objects o inner join sys.schemas s on o.schema_id = s.schema_id"
                                   + " left join sys.table_types t on o.object_id = t.type_table_object_id"
                                   + " where o.type_desc = '" + selectedObjectType + "')"
                                   + " select cte.name as Name"
                                   + " from objects_cte cte inner join sys.database_principals dp on cte.principal_id = dp.principal_id order by cte.name";
                        //+ " where dp.name = <logged in user name>";
                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatementObjectName, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectNames");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            comboBox4.DisplayMember = "Name";
                            comboBox4.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!!!");
            }
        }

        //executed when database list is invoked - list of database will be displayed
        private void comboBox2_Click(object sender, EventArgs e)
        {
            GetDatabaseConnection();
            //System.Threading.Thread.Sleep(500); //slepp for 500 milliseconds                   
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
 
            GetDatabaseConnection();
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;
        }

        //Executed when any combo box is selection is changed - group box 3
        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            GetDatabaseConnection();
            comboBox4.Enabled = true;
            checkBox1.Checked = false;
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView2.Refresh();
        }

        private void comboBox4_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            dataGridView2.Refresh();
            checkBox1.Checked = false;
            GetObjectMetaData();
        }

        private void GetObjectMetaData()
        {
            try
            {
                string userId = string.Empty;
                string password = string.Empty;
                string databaseName = string.Empty;
                string serverName = string.Empty;
                string connectionString = string.Empty;
                string sqlStatement = string.Empty;
                string selectedObjectType = string.Empty;
                string selectedObjectName = string.Empty;

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                BindingSource primaryGroupsBinding = new BindingSource();

                serverName = comboBox1.SelectedIndex == -1 ? comboBox1.Text : comboBox1.SelectedItem.ToString();
                databaseName = comboBox2.SelectedIndex == -1 ? "master" : comboBox2.GetItemText(comboBox2.SelectedItem);
                selectedObjectType = comboBox3.SelectedIndex == 0 ? comboBox3.Text : comboBox3.GetItemText(comboBox3.SelectedItem);
                selectedObjectName = comboBox4.SelectedIndex == 0 ? comboBox4.Text : comboBox4.GetItemText(comboBox4.SelectedItem);                
                selectedObjectName = selectedObjectName.Split('.').Last(); //get only the objectname - removing the schema information

                if (selectedObjectType == "VIEW" || selectedObjectType == "USER_TABLE")
                {
                    checkBox1.Enabled = true;                    
                }

                if (radioButton1.Checked == true)
                {
                    connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Integrated Security=True;";
                }
                else
                {
                    userId = textBox1.Text;
                    password = textBox2.Text;
                    connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";User ID=" + userId + ";Password=" + password + ";";
                }

                using (SqlConnection databaseConnection = new SqlConnection())
                {
                    databaseConnection.ConnectionString = connectionString;
                    if (selectedObjectType == "INTERNAL_TABLE" || selectedObjectType == "TYPE_TABLE" || selectedObjectType == "SYSTEM_TABLE" || selectedObjectType == "VIEW" || selectedObjectType == "USER_TABLE")
                    {
                        sqlStatement = "SELECT c.name AS [Column Name],cd.value AS [Column Description],st.name AS [Data Type] "
                                        + "FROM  sys.objects t "
                                        + "INNER JOIN  sys.columns c ON c.object_id = t.object_id "
                                        + "INNER JOIN sys.types st ON c.system_type_id = st.system_type_id AND ST.user_type_id = c.user_type_id "
                                        + "LEFT OUTER JOIN sys.extended_properties cd ON cd.major_id = c.object_id "
                                        + "AND cd.minor_id = c.column_id AND cd.name = 'MS_Description' "
                                        + "LEFT OUTER JOIN sys.table_types tt ON t.object_id = tt.type_table_object_id "
                                        + "WHERE t.type_desc ='" + selectedObjectType + "' AND (t.name = '" + selectedObjectName + "' OR tt.name = '" + selectedObjectName + "') "
                                        + "ORDER BY c.column_id";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectMetaData");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            dataGridView1.DataSource = dt;
                        }
                    }
                    else if (selectedObjectType == "SQL_SCALAR_FUNCTION" || selectedObjectType == "SQL_INLINE_TABLE_VALUED_FUNCTION" || selectedObjectType == "SQL_STORED_PROCEDURE" || selectedObjectType == "SQL_TABLE_VALUED_FUNCTION" || selectedObjectType == "SQL_TRIGGER")
                    {
                        sqlStatement = "SELECT sm.definition AS [Script] FROM sys.objects o INNER JOIN sys.sql_modules sm"
                                       + " ON o.object_id = sm.object_id"
                                       + " AND o.name = '" + selectedObjectName + "'";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectMetaData");                           
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                            dataGridView1.DataSource = dt;
                        }
                    }
                    else if (selectedObjectType == "SYNONYM")
                    {
                        sqlStatement = "SELECT s.name AS [Synonym Name] , s.base_object_name AS [Base Object Name]"
                                       + " FROM sys.synonyms s WHERE s.name = '" + selectedObjectName +"'";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectMetaData");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            dataGridView1.DataSource = dt;
                        }
                    }
                    else if (selectedObjectType == "DEFAULT_CONSTRAINT")
                    {
                        sqlStatement = "select t.name as [Table Name],c.name as [Column Name],d.definition as [Definition]"
                                       + " from sys.all_columns c"
                                       + " join sys.tables t on t.object_id = c.object_id"
                                       + " join sys.schemas s on s.schema_id = t.schema_id"
                                       + " join sys.default_constraints d on c.default_object_id = d.object_id"
	                                   + " and d.name = '" + selectedObjectName + "'";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectMetaData");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            dataGridView1.DataSource = dt;
                        }
                    }
                    else if (selectedObjectType == "PRIMARY_KEY_CONSTRAINT" || selectedObjectType == "UNIQUE_CONSTRAINT")
                    {
                        sqlStatement = "select t.name as [Table Name], c.name as [Column Name], ic.key_ordinal AS [Column Ordinal Position],i.type_desc as [Index Type]"
                                      + " from sys.key_constraints as k"
                                      + " join sys.tables as t on t.object_id = k.parent_object_id"
                                      + " join sys.schemas as s on s.schema_id = t.schema_id"
                                      + " join sys.index_columns as ic on ic.object_id = t.object_id and ic.index_id = k.unique_index_id"
                                      + " join sys.indexes i on ic.index_id = i.index_id and i.object_id = t.object_id"
                                      + " join sys.columns as c on c.object_id = t.object_id  and c.column_id = ic.column_id"
                                      + " and k.name ='" + selectedObjectName + "' order by ic.key_ordinal";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectMetaData");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            dataGridView1.DataSource = dt;
                        }
                    }
                    else if (selectedObjectType == "FOREIGN_KEY_CONSTRAINT")
                    {
                        sqlStatement = "select object_name(fk.parent_object_id) as [Parent Table],col_name(fkc.parent_object_id,parent_column_id) as [Constraint Key],"
                                    + " object_name(fk.referenced_object_id) as [Referece Table], col_name(fkc.referenced_object_id,referenced_column_id) as [Reference Table Key]"
                                    + " from sys.foreign_keys fk inner join sys.foreign_key_columns fkc on fk.object_id = fkc.constraint_object_id"
                                    + " where name ='" + selectedObjectName + "'";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectMetaData");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            dataGridView1.DataSource = dt;
                        }
                    }
                    else if (selectedObjectType == "CHECK_CONSTRAINT")
                    {
                        sqlStatement = "SELECT OBJECT_NAME(parent_object_id) AS TableName,OBJECT_DEFINITION(object_id) AS CheckConstraintDefinition"
	                                   + " FROM sys.Check_constraints WHERE name ='" + selectedObjectName + "'";

                        using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                        {
                            myDatabaseAdapter.Fill(ds, "ObjectMetaData");
                            dt = ds.Tables[0];
                            primaryGroupsBinding.DataSource = dt;
                            dataGridView1.DataSource = dt;
                        }
                    }                    
                }
                dataGridView1.ClearSelection();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!!!");
            }
        }
        private void GetObjectData()
        {
            try
            {
                string userId = string.Empty;
                string password = string.Empty;
                string databaseName = string.Empty;
                string serverName = string.Empty;
                string connectionString = string.Empty;
                string sqlStatement = string.Empty;
                string selectedObjectType = string.Empty;
                string selectedObjectName = string.Empty;

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                BindingSource primaryGroupsBinding = new BindingSource();

                serverName = comboBox1.SelectedIndex == -1 ? comboBox1.Text : comboBox1.SelectedItem.ToString();
                databaseName = comboBox2.SelectedIndex == -1 ? "master" : comboBox2.GetItemText(comboBox2.SelectedItem);
                selectedObjectType = comboBox3.SelectedIndex == 0 ? comboBox3.Text : comboBox3.GetItemText(comboBox3.SelectedItem);
                selectedObjectName = comboBox4.SelectedIndex == 0 ? comboBox4.Text : comboBox4.GetItemText(comboBox4.SelectedItem);
                //selectedObjectName = selectedObjectName.Split('.').Last(); //get only the objectname - removing the schema information

                if (radioButton1.Checked == true)
                {
                    connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Integrated Security=True;";
                }
                else
                {
                    userId = textBox1.Text;
                    password = textBox2.Text;
                    connectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";User ID=" + userId + ";Password=" + password + ";";
                }

                //resize the groupbox controls
                int width = dataGridView2.Width;
                dataGridView1.Width = width - 10;

                using (SqlConnection databaseConnection = new SqlConnection())
                {
                    databaseConnection.ConnectionString = connectionString;
                    sqlStatement = "SELECT TOP 10 * FROM " + selectedObjectName;
                    using (SqlDataAdapter myDatabaseAdapter = new SqlDataAdapter(sqlStatement, databaseConnection))
                    {
                        myDatabaseAdapter.Fill(ds, "ObjectData");
                        dt = ds.Tables[0];
                        primaryGroupsBinding.DataSource = dt;
                        dataGridView2.DataSource = dt;
                    }
                }
            }
            catch (Exception e) 
            {
                MessageBox.Show(e.Message,"Error!!!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dataGridView2.Visible = true;
                GetObjectData();
            }
            else
            {
                dataGridView2.Visible = false;                
            }
            //checkBox1.Checked = false;
        }
    }
}
