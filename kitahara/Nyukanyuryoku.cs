using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
        Boolean[] flgsizecolor = new Boolean[10];
        public int[,,] sizecolor = new int[10,11,11];

        string row1 = "";
        string row2 = "";
        int total = 0;
        DateTime dt;

        // 入荷数入力で備考に飛ぶ不具合のフラグ
        Boolean flgcell1 = false;
        // 品番入力エラーでも入荷数に飛ぶ不具合のフラグ
        Boolean flgcell2 = false;
        // 入荷数入力エラーで単価に飛ぶ不具合のフラグ
        Boolean flgcell3 = false;
        // 単価入力エラーで備考に飛ぶ不具合のフラグ
        Boolean flgcell4 = false;

        Boolean flgtotal = false;

        public Nyukanyuryoku()
        {
            InitializeComponent();

            InitGrid();
        }

        private void InitGrid()
        {
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
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
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].HeaderText = "品番";
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].HeaderText = "商品名";
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].HeaderText = "入荷数";
            dataGridView1.Columns[3].Width = 75;
            dataGridView1.Columns[4].HeaderText = "単価";
            dataGridView1.Columns[4].Width = 75;
            dataGridView1.Columns[5].HeaderText = "仕入金額";
            dataGridView1.Columns[5].Width = 80;
            dataGridView1.Columns[5].ReadOnly = true;
            dataGridView1.Columns[6].HeaderText = "備考";
            dataGridView1.Columns[6].Width = 200;
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
                //this.SelectNextControl(this.ActiveControl, true, true, true, true);
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
            else if ((keyData & Keys.KeyCode) == Keys.F5)
            {
                btnCancel.PerformClick();
            }
            else if ((keyData & Keys.KeyCode) == Keys.F12)
            {
                btnEnd.PerformClick();
            }

            return base.ProcessDialogKey(keyData);
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                switch (dataGridView1.CurrentCellAddress.X)
                {
                    case 3:
                        if (flgtotal)
                            dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value = total;
                        break;
                }
                SendKeys.Send("{TAB}");
                e.Handled = true;
            }
        }

        private void dataGridView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {           
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                //dataGridView1.EndEdit();
                if (dataGridView1.CurrentCellAddress.X == 1)
                {
                    dataGridView1.EndEdit();
                    //サーバー接続
                    string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    conn.Open();

                    //SQL実行
                    MySqlCommand cmd = new MySqlCommand("SELECT shohinmei, sku FROM shohin where shocd = '" + dataGridView1[1, dataGridView1.CurrentCellAddress.Y].Value.ToString() + "'", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();


                    if (!reader.HasRows)
                    {
                        if (!flgcell2)
                            MessageBox.Show("該当する商品がありません。","エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        flgcell2 = true;
                        dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.CurrentCellAddress.Y];
                        return;
                    }

                    
                    //テーブル出力
                    while (reader.Read())
                    {
                        row1 = reader["shohinmei"].ToString();
                        row2 = reader["sku"].ToString();
                        //Console.WriteLine(string.Join("\t", row));
                    }

                    dataGridView1[2, dataGridView1.CurrentCellAddress.Y].Value = row1;
                    dataGridView1.EndEdit();

                    
                }
                else if (dataGridView1.CurrentCellAddress.X == 3)
                {
                    //skuが"1"のとき、ダイアログを開く
                    if (row2 == "True")
                    {
                        flgsizecolor[dataGridView1.CurrentCellAddress.Y] = true;

                        Sizecolor sc = new Sizecolor();
                        sc.ShowDialog();
                        for (int i = 0; i < 11; i++)
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                sizecolor[dataGridView1.CurrentCellAddress.Y, i, j] = sc.scdata[i, j];
                            }
                        }
                        dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value = sc.total;
                        total = sc.total;
                        flgtotal = true;
                        //dataGridView1.EndEdit();
                        flgcell1 = true;
                        sc.Dispose();
                        row2 = "False";
                        //dataGridView1.CurrentCell = dataGridView1[3, dataGridView1.CurrentCellAddress.Y];
                    }
                    else
                    {
                        flgtotal = false;
                        flgsizecolor[dataGridView1.CurrentCellAddress.Y] = false;
                        flgcell1 = true;
                    }
                    dataGridView1.CurrentCell = dataGridView1[4, dataGridView1.CurrentCellAddress.Y];

                }
                else if (dataGridView1.CurrentCellAddress.X == 4)
                {
                    
                    //    dataGridView1[5, dataGridView1.CurrentCellAddress.Y].Value = int.Parse(dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value.ToString())
                    //        * int.Parse(dataGridView1[4, dataGridView1.CurrentCellAddress.Y].Value.ToString());
                    
                    //dataGridView1.EndEdit();
                }
            }
            else if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.Up)
            {
                txtSiiresaki.Focus();
                dataGridView1.CurrentCell = null;
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
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
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
                        txtTantoumei.Text = row;
                        txtNyukasouko.Focus();
                    }
                    else
                    {
                        MessageBox.Show("該当する担当者がいません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTantoucode.Focus();
                        return;
                    }
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
                        textSoukomei.Text = row;
                        txtNyukabi.Focus();
                    }
                    else
                    {
                        MessageBox.Show("該当する倉庫がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtNyukasouko.Focus();
                        return;
                    }
                    break;
                case "txtNyukabi":

                    if (!DateTime.TryParse(txtNyukabi.Text, out dt))
                    {
                        //変換出来たら、dtにその値が入る
                        MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtNyukabi.Focus();
                    }
                    else
                        txtSiiresaki.Focus();
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
                        txtSiiresakimei.Text = row;
                        dataGridView1.Focus();
                    }
                    else
                    {
                        MessageBox.Show("該当する仕入先がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSiiresaki.Focus();
                        return;
                    }
                    
                    break;
            }

        }

        #region DB追加、更新処理
        private void Insertdata()
        {
            int i = 0;
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            while (i < 10)
            {
                if (dataGridView1[1, i].Value.ToString() != "")
                {
                    if (flgsizecolor[i])
                    {
                        MySqlCommand cmd = new MySqlCommand("insert into nyukalog ( nyuryokuymd, nyukasoko, nyukaymd, shiresaki, hinban, shohinmei, syukasu, tanka," +
                            "urikin, biko," +
                            "sku10, sku11, sku12, sku13, sku14, sku15, sku16, sku17, sku18, sku19, sku20," +
                            "sku21, sku22, sku23, sku24, sku25, sku26, sku27, sku28, sku29, sku30," +
                            "sku31, sku32, sku33, sku34, sku35, sku36, sku37, sku38, sku39, sku40," +
                            "sku41, sku42, sku43, sku44, sku45, sku46, sku47, sku48, sku49, sku50," +
                            "sku51, sku52, sku53, sku54, sku55, sku56, sku57, sku58, sku59, sku60," +
                            "sku61, sku62, sku63, sku64, sku65, sku66, sku67, sku68, sku69, sku70," +
                            "sku71, sku72, sku73, sku74, sku75, sku76, sku77, sku78, sku79, sku80," +
                            "sku81, sku82, sku83, sku84, sku85, sku86, sku87, sku88, sku89, sku90," +
                            "sku91, sku92, sku93, sku94, sku95, sku96, sku97, sku98, sku99, sku100," +
                            "sku101, sku102, sku103, sku104, sku105, sku106, sku107, sku108, sku109, sku110," +
                            "sku111, sku112, sku113, sku114, sku115, sku116, sku117, sku118, sku119, sku120," +
                            "sku121, sku122, sku123, sku124, sku125, sku126, sku127, sku128, sku129, sku130) values " +
                            "( @nyuryokuymd, @nyukasoko, @nyukaymd, @shiresaki, @hinban, @shohinmei, @syukasu, @tanka," +
                            "@urikin, @biko," +
                            "@sku10, @sku11, @sku12, @sku13, @sku14, @sku15, @sku16, @sku17, @sku18, @sku19, @sku20," +
                            "@sku21, @sku22, @sku23, @sku24, @sku25, @sku26, @sku27, @sku28, @sku29, @sku30," +
                            "@sku31, @sku32, @sku33, @sku34, @sku35, @sku36, @sku37, @sku38, @sku39, @sku40," +
                            "@sku41, @sku42, @sku43, @sku44, @sku45, @sku46, @sku47, @sku48, @sku49, @sku50," +
                            "@sku51, @sku52, @sku53, @sku54, @sku55, @sku56, @sku57, @sku58, @sku59, @sku60," +
                            "@sku61, @sku62, @sku63, @sku64, @sku65, @sku66, @sku67, @sku68, @sku69, @sku70," +
                            "@sku71, @sku72, @sku73, @sku74, @sku75, @sku76, @sku77, @sku78, @sku79, @sku80," +
                            "@sku81, @sku82, @sku83, @sku84, @sku85, @sku86, @sku87, @sku88, @sku89, @sku90," +
                            "@sku91, @sku92, @sku93, @sku94, @sku95, @sku96, @sku97, @sku98, @sku99, @sku100," +
                            "@sku101, @sku102, @sku103, @sku104, @sku105, @sku106, @sku107, @sku108, @sku109, @sku110," +
                            "@sku111, @sku112, @sku113, @sku114, @sku115, @sku116, @sku117, @sku118, @sku119, @sku120," +
                            "@sku121, @sku122, @sku123, @sku124, @sku125, @sku126, @sku127, @sku128, @sku129, @sku130 )", conn);
                        cmd.Parameters.Add(new MySqlParameter("nyuryokuymd", DateTime.Today));
                        cmd.Parameters.Add(new MySqlParameter("nyukasoko", txtNyukasouko.Text));
                        cmd.Parameters.Add(new MySqlParameter("nyukaymd", dt));
                        cmd.Parameters.Add(new MySqlParameter("shiresaki", txtSiiresaki.Text));
                        cmd.Parameters.Add(new MySqlParameter("hinban", dataGridView1[1, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("shohinmei", dataGridView1[2, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("syukasu", dataGridView1[3, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("tanka", dataGridView1[4, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("urikin", dataGridView1[5, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("biko", dataGridView1[6, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("sku10", sizecolor[i, 0, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku11", sizecolor[i, 1, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku12", sizecolor[i, 2, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku13", sizecolor[i, 3, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku14", sizecolor[i, 4, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku15", sizecolor[i, 5, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku16", sizecolor[i, 6, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku17", sizecolor[i, 7, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku18", sizecolor[i, 8, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku19", sizecolor[i, 9, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku20", sizecolor[i, 10, 0]));
                        cmd.Parameters.Add(new MySqlParameter("sku21", sizecolor[i, 0, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku22", sizecolor[i, 1, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku23", sizecolor[i, 2, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku24", sizecolor[i, 3, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku25", sizecolor[i, 4, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku26", sizecolor[i, 5, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku27", sizecolor[i, 6, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku28", sizecolor[i, 7, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku29", sizecolor[i, 8, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku30", sizecolor[i, 9, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku31", sizecolor[i, 10, 1]));
                        cmd.Parameters.Add(new MySqlParameter("sku32", sizecolor[i, 0, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku33", sizecolor[i, 1, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku34", sizecolor[i, 2, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku35", sizecolor[i, 3, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku36", sizecolor[i, 4, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku37", sizecolor[i, 5, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku38", sizecolor[i, 6, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku39", sizecolor[i, 7, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku40", sizecolor[i, 8, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku41", sizecolor[i, 9, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku42", sizecolor[i, 10, 2]));
                        cmd.Parameters.Add(new MySqlParameter("sku43", sizecolor[i, 0, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku44", sizecolor[i, 1, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku45", sizecolor[i, 2, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku46", sizecolor[i, 3, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku47", sizecolor[i, 4, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku48", sizecolor[i, 5, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku49", sizecolor[i, 6, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku50", sizecolor[i, 7, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku51", sizecolor[i, 8, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku52", sizecolor[i, 9, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku53", sizecolor[i, 10, 3]));
                        cmd.Parameters.Add(new MySqlParameter("sku54", sizecolor[i, 0, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku55", sizecolor[i, 1, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku56", sizecolor[i, 2, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku57", sizecolor[i, 3, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku58", sizecolor[i, 4, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku59", sizecolor[i, 5, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku60", sizecolor[i, 6, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku61", sizecolor[i, 7, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku62", sizecolor[i, 8, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku63", sizecolor[i, 9, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku64", sizecolor[i, 10, 4]));
                        cmd.Parameters.Add(new MySqlParameter("sku65", sizecolor[i, 0, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku66", sizecolor[i, 1, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku67", sizecolor[i, 2, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku68", sizecolor[i, 3, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku69", sizecolor[i, 4, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku70", sizecolor[i, 5, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku71", sizecolor[i, 6, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku72", sizecolor[i, 7, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku73", sizecolor[i, 8, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku74", sizecolor[i, 9, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku75", sizecolor[i, 10, 5]));
                        cmd.Parameters.Add(new MySqlParameter("sku76", sizecolor[i, 0, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku77", sizecolor[i, 1, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku78", sizecolor[i, 2, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku79", sizecolor[i, 3, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku80", sizecolor[i, 4, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku81", sizecolor[i, 5, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku82", sizecolor[i, 6, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku83", sizecolor[i, 7, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku84", sizecolor[i, 8, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku85", sizecolor[i, 9, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku86", sizecolor[i, 10, 6]));
                        cmd.Parameters.Add(new MySqlParameter("sku87", sizecolor[i, 0, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku88", sizecolor[i, 1, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku89", sizecolor[i, 2, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku90", sizecolor[i, 3, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku91", sizecolor[i, 4, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku92", sizecolor[i, 5, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku93", sizecolor[i, 6, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku94", sizecolor[i, 7, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku95", sizecolor[i, 8, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku96", sizecolor[i, 9, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku97", sizecolor[i, 10, 7]));
                        cmd.Parameters.Add(new MySqlParameter("sku98", sizecolor[i, 0, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku99", sizecolor[i, 1, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku100", sizecolor[i, 2, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku101", sizecolor[i, 3, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku102", sizecolor[i, 4, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku103", sizecolor[i, 5, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku104", sizecolor[i, 6, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku105", sizecolor[i, 7, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku106", sizecolor[i, 8, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku107", sizecolor[i, 9, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku108", sizecolor[i, 10, 8]));
                        cmd.Parameters.Add(new MySqlParameter("sku109", sizecolor[i, 0, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku110", sizecolor[i, 1, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku111", sizecolor[i, 2, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku112", sizecolor[i, 3, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku113", sizecolor[i, 4, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku114", sizecolor[i, 5, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku115", sizecolor[i, 6, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku116", sizecolor[i, 7, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku117", sizecolor[i, 8, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku118", sizecolor[i, 9, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku119", sizecolor[i, 10, 9]));
                        cmd.Parameters.Add(new MySqlParameter("sku120", sizecolor[i, 0, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku121", sizecolor[i, 1, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku122", sizecolor[i, 2, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku123", sizecolor[i, 3, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku124", sizecolor[i, 4, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku125", sizecolor[i, 5, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku126", sizecolor[i, 6, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku127", sizecolor[i, 7, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku128", sizecolor[i, 8, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku129", sizecolor[i, 9, 10]));
                        cmd.Parameters.Add(new MySqlParameter("sku130", sizecolor[i, 10, 10]));
                        MySqlCommand cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
                        try
                        {
                            // オープン
                            cmd.Connection.Open();
                            // 実行
                            cmd.ExecuteNonQuery();
                            // 更新IDを取得
                            var id = cmd2.ExecuteScalar();
                            // クローズ
                            cmd.Connection.Close();
                        }
                        catch (SqlException ex)
                        {
                            // 例外処理
                            MessageBox.Show("例外発生:" + ex.Message);
                        }

                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand("insert into nyukalog ( nyuryokuymd, nyukasoko, nyukaymd, shiresaki, hinban, shohinmei, syukasu, tanka," +
                            "urikin, biko ) values " +
                            "( @nyuryokuymd, @nyukasoko, @nyukaymd, @shiresaki, @hinban, @shohinmei, @syukasu, @tanka," +
                            "@urikin, @biko )", conn);
                        cmd.Parameters.Add(new MySqlParameter("nyuryokuymd", DateTime.Today));
                        cmd.Parameters.Add(new MySqlParameter("nyukasoko", txtNyukasouko.Text));
                        cmd.Parameters.Add(new MySqlParameter("nyukaymd", dt));
                        cmd.Parameters.Add(new MySqlParameter("shiresaki", txtSiiresaki.Text));
                        cmd.Parameters.Add(new MySqlParameter("hinban", dataGridView1[1, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("shohinmei", dataGridView1[2, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("syukasu", dataGridView1[3, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("tanka", dataGridView1[4, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("urikin", dataGridView1[5, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("biko", dataGridView1[6, i].Value));
                        MySqlCommand cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
                        try
                        {
                            // オープン
                            cmd.Connection.Open();
                            // 実行
                            cmd.ExecuteNonQuery();
                            // 更新IDを取得
                            var id = cmd2.ExecuteScalar();
                            // クローズ
                            cmd.Connection.Close();
                        }
                        catch (SqlException ex)
                        {
                            // 例外処理
                            MessageBox.Show("例外発生:" + ex.Message);
                        }
                    }

                    MySqlCommand cmd1 = new MySqlCommand("update shohin set zaiko = zaiko + @zaiko, nyukasu = nyukasu + @zaiko, nyukabi = @nyukabi where shocd = @shocd", conn);
                    // パラメータ設定
                    cmd1.Parameters.Add(new MySqlParameter("zaiko", dataGridView1[3, i].Value));
                    cmd1.Parameters.Add(new MySqlParameter("nyukabi", DateTime.Today));
                    cmd1.Parameters.Add(new MySqlParameter("shocd", dataGridView1[1, i].Value));

                    //MySqlCommand cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
                    try
                    {
                        // オープン
                        cmd1.Connection.Open();
                        // 実行
                        cmd1.ExecuteNonQuery();
                        // 更新IDを取得
                        //var id = cmd2.ExecuteScalar();
                        // クローズ
                        cmd1.Connection.Close();
                    }
                    catch (SqlException ex)
                    {
                        // 例外処理
                        MessageBox.Show("例外発生:" + ex.Message);
                    }
                }
                i++;
            }
            MessageBox.Show("登録しました。");
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("取消しますか？",
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                InitGrid();
                //dataGridView1.CurrentCell = null;
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void btnTouroku_Click(object sender, EventArgs e)
        {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("登録しますか？",
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                Insertdata();
                InitGrid();
                txtSiiresaki.Text = "";
                txtSiiresakimei.Text = "";
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("終了しますか？",
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                this.Close();
                this.Dispose();
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            switch (dataGridView1.CurrentCellAddress.X)
            {
                // （No）
                case 0:
                    Action a = () => dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    break;
                // 商品名
                case 2:
                    a = () => dataGridView1.CurrentCell = dataGridView1[3, dataGridView1.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    //dataGridView1.EndEdit();
                    break;
                    
                // 入荷数
                case 3:
                    if (flgcell2)
                    {
                        a = () => dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.CurrentCellAddress.Y];
                        BeginInvoke(a);
                        flgcell2 = false;
                    }
                    if (flgcell3)
                        flgcell3 = false;
                    break;
                    
                // 単価
                case 4:
                    if (flgcell3)
                    {
                        a = () => dataGridView1.CurrentCell = dataGridView1[3, dataGridView1.CurrentCellAddress.Y];
                        BeginInvoke(a);
                        flgcell3 = false;
                    }
                    break;
                // 仕入金額
                case 5:
                    a = () => dataGridView1.CurrentCell = dataGridView1[6, dataGridView1.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    break;
                // 備考
                case 6:
                    if (flgcell1)
                    {
                        a = () => dataGridView1.CurrentCell = dataGridView1[4, dataGridView1.CurrentCellAddress.Y];
                        BeginInvoke(a);
                        flgcell1 = false;
                    }
                    if (flgcell4)
                    {
                        a = () => dataGridView1.CurrentCell = dataGridView1[4, dataGridView1.CurrentCellAddress.Y];
                        BeginInvoke(a);
                        flgcell4 = false;
                    }
                    break;
            }
            
        }

        private void Nyukanyuryoku_Shown(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = null;
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewEx dgv = (DataGridViewEx)sender;
            switch (dataGridView1.CurrentCellAddress.X)
            {
                case 3:
                    if (e.FormattedValue.ToString() == "")
                    {
                        MessageBox.Show("数字を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //入力した値をキャンセルして元に戻すには、次のようにする
                        dgv.CancelEdit();
                        //キャンセルする
                        e.Cancel = true;
                    }
                    //dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value = total;
                    break;
                case 4:
                    if (e.FormattedValue.ToString() == "")
                    {
                        MessageBox.Show("数字を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //入力した値をキャンセルして元に戻すには、次のようにする
                        dgv.CancelEdit();
                        //キャンセルする
                        //e.Cancel = true;
                    }
                    else
                    {
                    }
                    break;
                case 5:
                    dataGridView1[5, dataGridView1.CurrentCellAddress.Y].Value = int.Parse(dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value.ToString())
                        * int.Parse(dataGridView1[4, dataGridView1.CurrentCellAddress.Y].Value.ToString());

                    break;
            }
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            switch (dataGridView1.CurrentCellAddress.X)
            {
                case 3:
                    //dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value = total;
                    break;
            }
        }

        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            switch (dataGridView1.CurrentCellAddress.X)
            {
                case 3:
                    //dataGridView1[3, dataGridView1.CurrentCellAddress.Y].Value = total;
                    break;
                case 4:

                    Action a = () => dataGridView1.CurrentCell = dataGridView1[6, dataGridView1.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    break;
            }
        }
    }
}