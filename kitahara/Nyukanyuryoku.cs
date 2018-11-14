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
    public partial class Nyukanyuryoku : Form
    {
        Boolean[] flgsizecolor = new Boolean[9];
        int[,,] sizecolor = new int[9,10,10];

        public Nyukanyuryoku()
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
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].HeaderText = "入荷数";
            dataGridView1.Columns[4].HeaderText = "単価";
            dataGridView1.Columns[5].HeaderText = "仕入金額";
            dataGridView1.Columns[5].ReadOnly = true;
            dataGridView1.Columns[6].HeaderText = "備考";



        }


        //DataGridView以外のコントロールのキー押下検知
        [System.Security.Permissions.UIPermission(
    System.Security.Permissions.SecurityAction.Demand,
    Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //Enterキーが押されているか調べる
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                check_textbox();
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
                //本来の処理（左側のコントロールにフォーカスを移す）を
                //させたくないときは、trueを返す
                return true;
            }
            //Tabキーが押されているか調べる
            else if ((keyData & Keys.KeyCode) == Keys.Tab)
            {
                check_textbox();
                
                //MessageBox.Show(cControl.Name);
            }
            else if ((keyData & Keys.KeyCode) == Keys.F10)
            {
                btnTouroku.PerformClick();
            }

            return base.ProcessDialogKey(keyData);
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dataGridView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            string row1 ="";
            string row2 ="";

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                //dataGridView1.EndEdit();
                if (dataGridView1.CurrentCellAddress.X == 2)
                {
                    //サーバー接続
                    string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    conn.Open();

                    //SQL実行
                    MySqlCommand cmd = new MySqlCommand("SELECT shohinmei, sku FROM shohin where shocd = '" + dataGridView1[1, dataGridView1.CurrentCellAddress.Y].Value.ToString() + "'", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        MessageBox.Show("該当する商品がありません。","エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }

                    
                    //テーブル出力
                    while (reader.Read())
                    {
                        row1 = reader["shohinmei"].ToString();
                        row2 = reader["sku"].ToString();
                        //Console.WriteLine(string.Join("\t", row));
                    }

                    if (row2 == "True")
                        flgsizecolor[dataGridView1.CurrentCellAddress.Y] = true;
                    else
                        flgsizecolor[dataGridView1.CurrentCellAddress.Y] = false;

                    dataGridView1[2, dataGridView1.CurrentCellAddress.Y].Value = row1;
                    dataGridView1.EndEdit();
                }
                else if (dataGridView1.CurrentCellAddress.X == 4)
                {   // 0番目の列のいずれかのセルにフォーカスがある場合
                    // 1番目の列にフォーカスを移動する（行は移動しない）
                    //dataGridView1.CurrentCell = dataGridView1[3, dataGridView1.CurrentCellAddress.Y];
                    dataGridView1[5, dataGridView1.CurrentCellAddress.Y].Value = int.Parse(dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value.ToString())
                        * int.Parse(dataGridView1[4, dataGridView1.CurrentCellAddress.Y].Value.ToString());
                    dataGridView1.EndEdit();
                    //dataGridView1.Refresh();
                    //dataGridView1.UpdateCellValue(5, dataGridView1.CurrentCellAddress.Y);
                    //dataGridView1.CurrentCell = dataGridView1[6, dataGridView1.CurrentCellAddress.Y];
                    
                    
                }
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //表示されているコントロールがDataGridViewTextBoxEditingControlか調べる
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集のために表示されているコントロールを取得
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;

                //イベントハンドラを削除
                tb.KeyDown -= DataGridView1_KeyDown;
                tb.PreviewKeyDown -= dataGridView1_PreviewKeyDown;
                tb.KeyDown += DataGridView1_KeyDown;
                tb.PreviewKeyDown += dataGridView1_PreviewKeyDown;
            }
        }

        private void check_textbox()
        {
            Control cControl = this.ActiveControl;
            string row = "";

            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();

            switch (cControl.Name)
            {
                case "txtTantoucode":
                    //SQL実行
                    MySqlCommand cmd = new MySqlCommand("SELECT tanmei FROM tantousha where tancd = '" + txtTantoucode.Text + "'", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //テーブル出力
                        while (reader.Read())
                        {
                            row = reader["tanmei"].ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("該当する担当者がいません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    txtTantoumei.Text = row;
                    break;
                case "txtNyukasouko":
                    //SQL実行
                    cmd = new MySqlCommand("SELECT sokomei FROM souko where soucd = '" + txtNyukasouko.Text + "'", conn);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //テーブル出力
                        while (reader.Read())
                        {
                            row = reader["sokomei"].ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("該当する倉庫がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    textSoukomei.Text = row;
                    break;
                case "txtNyukabi":
                    DateTime dt;
                    if (!DateTime.TryParse(txtNyukabi.Text, out dt))
                    {
                        //変換出来たら、dtにその値が入る
                        MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "txtSiiresaki":
                    //SQL実行
                    cmd = new MySqlCommand("SELECT shiresakimei FROM shiresaki where scd = '" + txtSiiresaki.Text + "'", conn);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //テーブル出力
                        while (reader.Read())
                        {
                            row = reader["shiresakimei"].ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("該当する仕入先がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    txtSiiresakimei.Text = row;
                    break;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnTouroku_Click(object sender, EventArgs e)
        {

        }

        private void btnEnd_Click(object sender, EventArgs e)
        {

        }
    }
}