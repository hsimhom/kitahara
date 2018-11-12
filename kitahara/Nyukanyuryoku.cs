using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace kitahara
{
    public partial class Form1_loaded : Form
    {
        public Form1_loaded()
        {
            InitializeComponent();

            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();

            // データを格納するテーブル作成
            DataTable dt = new DataTable();

            // SQL文と接続情報を指定し、データアダプタを作成
            MySqlDataAdapter da = new MySqlDataAdapter("select * from tmp_nyukanyuuryoku", conn);

            // データ取得
            da.Fill(dt);

            // データ表示
            dataGridView1.DataSource = dt;

            dataGridView1.Columns[0].HeaderText = "No";
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].HeaderText = "品番";
            dataGridView1.Columns[2].HeaderText = "商品名";
            dataGridView1.Columns[3].HeaderText = "入荷数";
            dataGridView1.Columns[4].HeaderText = "単価";
            dataGridView1.Columns[5].HeaderText = "仕入金額";
            dataGridView1.Columns[6].HeaderText = "備考";


        }


    }
}