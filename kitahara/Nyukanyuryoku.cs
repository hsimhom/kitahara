using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace kitahara
{
    public partial class Nyukanyuryoku : Form
    {
        // 在庫マスタにデータがない場合、true
        Boolean[] flgnodata = new Boolean[10];

        // SKUがTrueでSKUデータが入っているとき、True
        Boolean[] flgsizecolor = new Boolean[10];

        // SKUデータ
        public int[,,] sizecolor = new int[10,11,11];

        // SKUデータバックアップ
        int[,] sizecolor_bk = new int[11, 11];

        // 数量バックアップ
        int suryou_bk = 0;

        string flgsku = "";

        // SKU数量のトータル
        int total = 0;

        DateTime dt;

        // 品番入力エラーでも入荷数に飛ぶ不具合のフラグ
        Boolean flgcell2 = false;
        

        Boolean flgtotal = false;
        Boolean flgsku0 = false;
        Boolean showsku = true;

        // 入力区分
        int iKubun = 1;

        // 伝票区分
        int iDpKubun = 1;
        string strkubun;

        const int colno = 0;
        const int colhinban = 1;
        const int colsyohinmei = 2;
        const int colsuryou = 3;
        const int coltanka = 4;
        const int colkingaku = 5;
        const int colbiko = 6;

        public Nyukanyuryoku()
        {
            InitializeComponent();
        }

        // データグリッドビューの初期化
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
            dataGridView2.DataSource = dt;

            dataGridView2.Columns[colno].HeaderText = "No";
            dataGridView2.Columns[colno].Width = 30;
            dataGridView2.Columns[colno].ReadOnly = true;
            dataGridView2.Columns[colhinban].HeaderText = "品番";
            dataGridView2.Columns[colhinban].Width = 100;
            dataGridView2.Columns[colsyohinmei].HeaderText = "商品名";
            dataGridView2.Columns[colsyohinmei].Width = 200;
            dataGridView2.Columns[colsyohinmei].ReadOnly = true;
            dataGridView2.Columns[colsuryou].HeaderText = "数量";
            dataGridView2.Columns[colsuryou].Width = 80;
            dataGridView2.Columns[coltanka].HeaderText = "単価";
            dataGridView2.Columns[coltanka].Width = 75;
            dataGridView2.Columns[colkingaku].HeaderText = "仕入金額";
            dataGridView2.Columns[colkingaku].Width = 80;
            dataGridView2.Columns[colkingaku].ReadOnly = true;
            dataGridView2.Columns[colbiko].HeaderText = "備考";
            dataGridView2.Columns[colbiko].Width = 200;

            // 
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        sizecolor[k, i, j]=0;
                    }
                    sizecolor_bk[i, j] = 0;
                }
            }
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

        #region 各テキストボックスがアクティブ時の処理
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
                case "txtKubun":
                    if (Regex.IsMatch(txtKubun.Text, @"^[1-3]{1}$"))
                    {
                        iKubun = int.Parse(txtKubun.Text);
                    }
                    else
                    {
                        MessageBox.Show("1,2,3を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtKubun.Focus();
                        return;
                    }
                    switch (iKubun)
                    {
                        case 1:
                            strkubun = "登録";
                            txtLogno.ReadOnly = true;
                            btnTouroku.Text = "F10:登録";
                            
                            txtDpKubun.Focus();
                            break;
                        case 2:
                            strkubun = "修正";
                            txtLogno.ReadOnly = false;
                            btnTouroku.Text = "F10:修正";
                            
                            txtLogno.Focus();
                            break;
                        case 3:
                            strkubun = "削除";
                            txtLogno.ReadOnly = false;
                            btnTouroku.Text = "F10:削除";
                            
                            txtLogno.Focus();
                            break;
                    }
                    break;
                case "txtLogno":
                    switch (iKubun)
                    {
                        case 2:
                        case 3:
                            dispdata();
                            txtTantoucode.Focus();
                            break;
                    }
                    break;
                case "txtDpKubun":
                    if (Regex.IsMatch(txtDpKubun.Text, @"^[1-4]{1}$"))
                    {
                        iDpKubun = int.Parse(txtDpKubun.Text);
                        txtTantoucode.Focus();
                    }
                    else
                    {
                        MessageBox.Show("1,2,3,4を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtDpKubun.Focus();
                        return;
                    }
                    switch (iDpKubun)
                    {
                        case 1:
                            dataGridView2.Columns[colsuryou].HeaderText = "受注数量";
                            txtSiiresaki.Enabled = true;
                            break;
                        case 2:
                            dataGridView2.Columns[colsuryou].HeaderText = "取置数量";
                            txtSiiresaki.Enabled = false;
                            break;
                        case 3:
                            dataGridView2.Columns[colsuryou].HeaderText = "入荷数量";
                            txtSiiresaki.Enabled = true;
                            break;
                        case 4:
                            dataGridView2.Columns[colsuryou].HeaderText = "発注数量";
                            txtSiiresaki.Enabled = true;
                            break;
                    }
                    break;
                case "txtTantoucode":
                    //SQL実行
                    MySqlCommand cmd1 = new MySqlCommand("SELECT tanmei FROM tantousha where tancd = '" + txtTantoucode.Text + "'", conn);
                    MySqlDataReader reader = cmd1.ExecuteReader();
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
                    cmd1 = new MySqlCommand("SELECT sokomei FROM souko where soucd = '" + txtNyukasouko.Text + "'", conn);
                    reader = cmd1.ExecuteReader();
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
                        if (txtNyukabi.TextLength != 4)
                        {
                            //変換出来たら、dtにその値が入る
                            MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtNyukabi.Focus();
                        }
                        else
                        {
                            DateTime dt1 = DateTime.Now;
                            string temp1 = txtNyukabi.Text;
                            string temp2 = temp1.Insert(2, "/");
                            string temp3 = temp2.Insert(0, "/");
                            txtNyukabi.Text = dt1.Year.ToString() + temp3;
                            txtSiiresaki.Focus();
                        }
                    }
                    else
                        txtSiiresaki.Focus();
                    if (iDpKubun == 2)
                        dataGridView2.Focus();
                    break; 
                case "txtSiiresaki":
                    
                    cmd1 = new MySqlCommand("", conn);
                    switch (iDpKubun)
                    {
                        case 1:
                            cmd1 = new MySqlCommand("SELECT tokuisakimei FROM tokuisaki where tokuicd = '" + txtSiiresaki.Text + "'", conn);
                            //reader = cmd1.ExecuteReader();
                            break;
                        case 2:

                            break;
                        case 3:
                        case 4:
                            cmd1 = new MySqlCommand("SELECT shiresakimei FROM shiresaki where scd = '" + txtSiiresaki.Text + "'", conn);
                            //reader = cmd1.ExecuteReader();
                            break;

                    }
                    //SQL実行
                    //cmd = new MySqlCommand("SELECT shiresakimei FROM shiresaki where scd = '" + txtSiiresaki.Text + "'", conn);
                    MySqlDataReader reader1 = cmd1.ExecuteReader();
                    if (reader1.HasRows)
                    {
                        //テーブル出力
                        while (reader1.Read())
                        {
                            switch (iDpKubun)
                            {
                                case 1:
                                    txtSiiresakimei.Text = reader1["tokuisakimei"].ToString();
                                    break;
                                case 2:
                                    break;
                                case 3:
                                case 4:
                                    txtSiiresakimei.Text = reader1["shiresakimei"].ToString();
                                    break;
                            }
                        }
                        dataGridView2.Focus();
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
        #endregion

        #region ログNoから表示を行う
        private void dispdata()
        {
            int col = dataGridView2.CurrentCellAddress.X;
            int row = dataGridView2.CurrentCellAddress.Y;

            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();

            //SQL実行
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM nyukalog where renban = @renban", conn);
            cmd.Parameters.Add(new MySqlParameter("renban", txtLogno.Text));
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //テーブル出力
                while (reader.Read())
                {
                    txtDpKubun.Text = reader["nkbn"].ToString();
                    txtTantoucode.Text = reader["tancd"].ToString();
                    txtNyukasouko.Text = reader["nyukasoko"].ToString();
                    txtNyukabi.Text = reader["nyukaymd"].ToString().Substring(0, 10);
                    txtSiiresaki.Text = reader["shiresaki"].ToString();
                    dataGridView2[colhinban, 0].Value = reader["hinban"].ToString();
                    dataGridView2[colsyohinmei, 0].Value = reader["shohinmei"].ToString();
                    dataGridView2[colsuryou, 0].Value = reader["syukasu"].ToString();
                    dataGridView2[coltanka, 0].Value = reader["tanka"].ToString();
                    dataGridView2[colkingaku, 0].Value = reader["urikin"].ToString();
                    suryou_bk = int.Parse(reader["syukasu"].ToString());
                }
            }
            else
            {
                MessageBox.Show("該当するログがありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //txtTokuisakicode.Focus();
                return;
            }

            cmd.Connection.Close();
            cmd.Connection.Open();

            //SQL実行
            cmd = new MySqlCommand("SELECT shohinmei, sku, nyukasu, jyuchusu, syukasu FROM shohin where shocd = @shocd", conn);
            cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, 0].Value.ToString()));
            reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                MessageBox.Show("該当する商品がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView2.CurrentCell = dataGridView2[colhinban, 0];
                return;
            }
            else
            {
                //テーブル出力
                while (reader.Read())
                {
                    dataGridView2[colsyohinmei, 0].Value = reader["shohinmei"].ToString();
                    flgsku = reader["sku"].ToString();

                }
                 
            }

            cmd.Connection.Close();
            cmd.Connection.Open();

            cmd = new MySqlCommand("SELECT * FROM zaiko where shocd = @shocd and souko = @souko", conn);
            cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, 0].Value.ToString()));
            cmd.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
            MySqlDataReader reader2 = cmd.ExecuteReader();


            if (reader2.HasRows)
            {
                while (reader2.Read())
                {
                    //ihattyusu = int.Parse(reader2["hatsu"].ToString());
                    //itzaiko = int.Parse(reader2["torioki"].ToString());
                    // 色サイズの数量の格納
                    int l = 10;
                    for (int i = 0; i < 11; i++)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            string str = "sku" + l.ToString();
                            sizecolor_bk[i, j] = int.Parse(reader2[str].ToString());
                            sizecolor[0, i, j] = sizecolor_bk[i, j];
                            l++;
                        }
                    }
                }
                flgnodata[0] = false;
            }
            else
            {
                // 在庫マスタにデータがない
                //ihattyusu = 0;
                //itzaiko = 0;
                // 色サイズの数量の格納
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        sizecolor[0, i, j] = 0;
                    }
                }
                flgnodata[0] = true;
            }

            //ikzaiko = ihattyusu - ijittusu - isyukasu - itzaiko;
            //igzaiko = inyukasu = isyukasu;

            cmd.Connection.Close();
            cmd.Connection.Open();

            cmd = new MySqlCommand("SELECT tanmei FROM tantousha where tancd = '" + txtTantoucode.Text + "'", conn);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //テーブル出力
                while (reader.Read())
                {
                    txtTantoumei.Text = reader["tanmei"].ToString();
                }
            }

            cmd.Connection.Close();
            cmd.Connection.Open();

            cmd = new MySqlCommand("SELECT sokomei FROM souko where soucd = '" + txtNyukasouko.Text + "'", conn);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //テーブル出力
                while (reader.Read())
                {
                    textSoukomei.Text = reader["sokomei"].ToString();
                }
            }

            cmd.Connection.Close();
            cmd.Connection.Open();

            cmd = new MySqlCommand("", conn);
            switch (iKubun)
            {
                case 1:
                    cmd = new MySqlCommand("SELECT tokuisakimei FROM tokuisaki where tokuicd = '" + txtSiiresaki.Text + "'", conn);
                    //reader = cmd1.ExecuteReader();
                    break;
                case 2:
                case 3:
                    cmd = new MySqlCommand("SELECT shiresakimei FROM shiresaki where scd = '" + txtSiiresaki.Text + "'", conn);
                    //reader = cmd1.ExecuteReader();
                    break;

            }
            //SQL実行
            //cmd = new MySqlCommand("SELECT shiresakimei FROM shiresaki where scd = '" + txtSiiresaki.Text + "'", conn);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //テーブル出力
                while (reader.Read())
                {
                    switch (iKubun)
                    {
                        case 1:
                            txtSiiresakimei.Text = reader["tokuisakimei"].ToString();
                            break;
                        case 2:
                        case 3:
                            txtSiiresakimei.Text = reader["shiresakimei"].ToString();
                            break;
                    }
                }
                dataGridView2.Focus();
            }
        }
        #endregion
        
        #region DB追加処理
        private void Insertdata()
        {
            log_insert();
            zaiko_insert(0);
            
            MessageBox.Show("登録しました。");
        }
        #endregion

        #region DB更新処理
        private void updatedata()
        {
            log_delete();
            log_insert();
            zaiko_update_minas();
            zaiko_update_plus();
            zaiko_update_new();
            zaiko_update_last(0);
        }
        #endregion

        #region DB削除処理
        private void deletedata()
        {
            log_delete();
            zaiko_delete();
            zaiko_update_last(0);
        }
        #endregion

        #region log追加処理 log_insert()
        private void log_insert()
        {
            int i = 0;
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            while (i < 10)
            {
                if (dataGridView2[colhinban, i].Value.ToString() != "")
                {
                    if (!flgsizecolor[i])
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            for (int k = 0; k < 11; k++)
                            {
                                sizecolor[i, k, j] = 0;
                            }
                        }
                    }
                        MySqlCommand cmd = new MySqlCommand("insert into nyukalog ( nkbn, nyuryokuymd, nyukasoko, nyukaymd, shiresaki, hinban, shohinmei, syukasu, tanka," +
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
                            "( @nkbn, @nyuryokuymd, @nyukasoko, @nyukaymd, @shiresaki, @hinban, @shohinmei, @syukasu, @tanka," +
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
                        cmd.Parameters.Add(new MySqlParameter("nkbn", txtDpKubun.Text));
                    cmd.Parameters.Add(new MySqlParameter("nyuryokuymd", DateTime.Today));
                    cmd.Parameters.Add(new MySqlParameter("nyukasoko", txtNyukasouko.Text));
                    DateTime.TryParse(txtNyukabi.Text, out dt);
                    cmd.Parameters.Add(new MySqlParameter("nyukaymd", dt));
                        cmd.Parameters.Add(new MySqlParameter("shiresaki", txtSiiresaki.Text));
                        cmd.Parameters.Add(new MySqlParameter("hinban", dataGridView2[colhinban, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("shohinmei", dataGridView2[colsyohinmei, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("syukasu", dataGridView2[colsuryou, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("tanka", dataGridView2[coltanka, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("urikin", dataGridView2[colkingaku, i].Value));
                        cmd.Parameters.Add(new MySqlParameter("biko", dataGridView2[colbiko, i].Value));
                        string strsku = "";
                        int l = 10;
                        for (int j = 0; j < 11; j++)
                        {
                            for (int k = 0; k < 11; k++)
                            {
                                strsku = "sku" + l.ToString();
                                cmd.Parameters.Add(new MySqlParameter(strsku, sizecolor[i, k, j]));
                                l++;
                            }
                        }
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
                i++;
            }
        }
        #endregion

        #region log削除処理 log_delete()
        private void log_delete()
        {
            // 出荷指示入力データから削除
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("delete from nyukalog where renban = @renban", conn);
            cmd.Parameters.Add(new MySqlParameter("renban", txtLogno.Text));
            //MySqlDataReader reader = cmd.ExecuteReader();
            try
            {
                // オープン
                //cmd.Connection.Open();
                // 実行
                cmd.ExecuteNonQuery();
                // 更新IDを取得
                //var id = cmd2.ExecuteScalar();
                // クローズ
                //cmd.Connection.Close();
            }
            catch (SqlException ex)
            {
                // 例外処理
                MessageBox.Show("例外発生:" + ex.Message);
            }
        }
        #endregion

        #region 在庫マスタ追加処理 zaiko_insert(int start)
        private void zaiko_insert(int start)
        {
            int i = start;
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            while (i < 10)
            {
                if (dataGridView2[colhinban, i].Value.ToString() != "")
                {
                    // 在庫マスタinsert or update
                    string nyukabi0 = "";
                    string nyukabi1 = "";
                    string nyukabi2 = "";
                    int num1 = 0;
                    int num2 = 0;
                    int num3 = 0;
                    int num4 = 0;
                    switch (iDpKubun)
                    {
                        case 1:
                            nyukabi0 = "";
                            nyukabi1 = "";
                            nyukabi2 = "";
                            num1 = int.Parse(dataGridView2[colsuryou, i].Value.ToString());
                            num2 = 0;
                            num3 = 0;
                            num4 = 0;
                            break;
                        case 2:
                            nyukabi0 = "";
                            nyukabi1 = "";
                            nyukabi2 = "";
                            num1 = 0;
                            num2 = int.Parse(dataGridView2[colsuryou, i].Value.ToString());
                            num3 = 0;
                            num4 = 0;
                            break;
                        case 3:
                            nyukabi0 = "nyukabi,";
                            nyukabi1 = "@nyukabi,";
                            nyukabi2 = "nyukabi = @nyukabi,";
                            num1 = 0;
                            num2 = 0;
                            num3 = int.Parse(dataGridView2[colsuryou, i].Value.ToString());
                            num4 = 0;
                            break;
                        case 4:
                            nyukabi0 = "";
                            nyukabi1 = "";
                            nyukabi2 = "";
                            num1 = 0;
                            num2 = 0;
                            num3 = 0;
                            num4 = int.Parse(dataGridView2[colsuryou, i].Value.ToString());
                            break;
                    }
                    // 在庫マスタにデータがない場合、insert
                    if (flgnodata[i])
                    {
                        MySqlCommand cmd3 = new MySqlCommand("insert into zaiko ( souko, shocd, hatsu, nyukasu, jyuchusu, torioki, zaiko, " +
                            nyukabi0 +
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
                        "( @souko, @shocd, @hatsu, @nyukasu, @jyuchusu, @torioki, @zaiko, " +
                        nyukabi1 +
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
                        cmd3.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
                        cmd3.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, i].Value));
                        cmd3.Parameters.Add(new MySqlParameter("hatsu", num4));
                        cmd3.Parameters.Add(new MySqlParameter("nyukasu", num3));
                        cmd3.Parameters.Add(new MySqlParameter("jyuchusu", num1));
                        cmd3.Parameters.Add(new MySqlParameter("torioki", num2));
                        cmd3.Parameters.Add(new MySqlParameter("zaiko", num3));
                        DateTime.TryParse(txtNyukabi.Text, out dt);
                        if (iDpKubun == 3)
                            cmd3.Parameters.Add(new MySqlParameter("nyukabi", dt));

                        string strsku = "";
                        int l = 10;
                        for (int j = 0; j < 11; j++)
                        {
                            for (int k = 0; k < 11; k++)
                            {
                                strsku = "sku" + l.ToString();
                                if (flgsku0)
                                {
                                    cmd3.Parameters.Add(new MySqlParameter(strsku, sizecolor[i, k, j]));
                                }
                                else
                                {
                                    cmd3.Parameters.Add(new MySqlParameter(strsku, sizecolor[i, k, j]));
                                }
                                l++;
                            }
                        }

                        //cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
                        try
                        {
                            // オープン
                            cmd3.Connection.Open();
                            // 実行
                            cmd3.ExecuteNonQuery();
                            // 更新IDを取得
                            //var id = cmd2.ExecuteScalar();
                            // クローズ
                            cmd3.Connection.Close();
                        }
                        catch (SqlException ex)
                        {
                            // 例外処理
                            MessageBox.Show("例外発生:" + ex.Message);
                        }
                    }
                    // 在庫マスタにデータがある場合、update
                    else
                    {
                        MySqlCommand cmd3 = new MySqlCommand("update zaiko set hatsu = hatsu + @hatsu, nyukasu = nyukasu + @nyukasu, jyuchusu = jyuchusu + @jyuchusu, torioki = torioki + @torioki, " +
                            "zaiko = zaiko + @zaiko, " + nyukabi2 +
                        "sku10 = sku10 + @sku10,sku11 = sku11 + @sku11,sku12 = sku12 + @sku12,sku13 = sku13 + @sku13,sku14 = sku14 + @sku14,sku15 = sku15 + @sku15," +
                        "sku16 = sku16 + @sku16,sku17 = sku17 + @sku17,sku18 = sku18 + @sku18,sku19 = sku19 + @sku19,sku20 = sku20 + @sku20," +
                        "sku21 = sku21 + @sku21, sku22 = sku22 + @sku22, sku23 = sku23 + @sku23, sku24 = sku24 + @sku24, sku25 = sku25 + @sku25," +
                        "sku26 = sku26 + @sku26, sku27 = sku27 + @sku27, sku28 = sku28 + @sku28, sku29 = sku29 + @sku29, sku30 = sku30 + @sku30," +
                        "sku31 = sku31 + @sku31, sku32 = sku32 + @sku32, sku33 = sku33 + @sku33, sku34 = sku34 + @sku34, sku35 = sku35 + @sku35," +
                        "sku36 = sku36 + @sku36, sku37 = sku37 + @sku37, sku38 = sku38 + @sku38, sku39 = sku39 + @sku39, sku40 = sku40 + @sku40," +
                        "sku41 = sku41 + @sku41, sku42 = sku42 + @sku42, sku43 = sku43 + @sku43, sku44 = sku44 + @sku44, sku45 = sku45 + @sku45," +
                        "sku46 = sku46 + @sku46, sku47 = sku47 + @sku47, sku48 = sku48 + @sku48, sku49 = sku49 + @sku49, sku50 = sku50 + @sku50," +
                        "sku51 = sku51 + @sku51, sku52 = sku52 + @sku52, sku53 = sku53 + @sku53, sku54 = sku54 + @sku54, sku55 = sku55 + @sku55," +
                        "sku56 = sku56 + @sku56, sku57 = sku57 + @sku57, sku58 = sku58 + @sku58, sku59 = sku59 + @sku59, sku60 = sku60 + @sku60," +
                        "sku61 = sku61 + @sku61, sku62 = sku62 + @sku62, sku63 = sku63 + @sku63, sku64 = sku64 + @sku64, sku65 = sku65 + @sku65," +
                        "sku66 = sku66 + @sku66, sku67 = sku67 + @sku67, sku68 = sku68 + @sku68, sku69 = sku69 + @sku69, sku70 = sku70 + @sku70," +
                        "sku71 = sku71 + @sku71, sku72 = sku72 + @sku72, sku73 = sku73 + @sku73, sku74 = sku74 + @sku74, sku75 = sku75 + @sku75," +
                        "sku76 = sku76 + @sku76, sku77 = sku77 + @sku77, sku78 = sku78 + @sku78, sku79 = sku79 + @sku79, sku80 = sku80 + @sku80," +
                        "sku81 = sku81 + @sku81, sku82 = sku82 + @sku82, sku83 = sku83 + @sku83, sku84 = sku84 + @sku84, sku85 = sku85 + @sku85," +
                        "sku86 = sku86 + @sku86, sku87 = sku87 + @sku87, sku88 = sku88 + @sku88, sku89 = sku89 + @sku89, sku30 = sku30 + @sku30," +
                        "sku91 = sku91 + @sku91, sku92 = sku92 + @sku92, sku93 = sku93 + @sku93, sku94 = sku94 + @sku94, sku95 = sku95 + @sku95," +
                        "sku96 = sku96 + @sku96, sku97 = sku97 + @sku97, sku98 = sku98 + @sku98, sku99 = sku99 + @sku99, sku100 = sku100 + @sku100," +
                        "sku101 = sku101 + @sku101, sku102 = sku102 + @sku102, sku103 = sku103 + @sku103, sku104 = sku104 + @sku104, sku105 = sku105 + @sku105," +
                        "sku106 = sku106 + @sku106, sku107 = sku107 + @sku107, sku108 = sku108 + @sku108, sku109 = sku109 + @sku109, sku110 = sku110 + @sku110," +
                        "sku111 = sku111 + @sku111, sku112 = sku112 + @sku112, sku113 = sku113 + @sku113, sku114 = sku114 + @sku114, sku115 = sku115 + @sku115," +
                        "sku116 = sku116 + @sku116, sku117 = sku117 + @sku117, sku118 = sku118 + @sku118, sku119 = sku119 + @sku119, sku120 = sku120 + @sku120," +
                        "sku121 = sku121 + @sku121, sku122 = sku122 + @sku122, sku123 = sku123 + @sku123, sku124 = sku124 + @sku124, sku125 = sku125 + @sku125," +
                        "sku126 = sku126 + @sku126, sku127 = sku127 + @sku127, sku128 = sku128 + @sku128, sku129 = sku129 + @sku129, sku130 = sku130 + @sku130" +
                        " where souko = @souko and shocd = @shocd", conn);
                        // パラメータ設定
                        cmd3.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
                        cmd3.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, i].Value));
                        cmd3.Parameters.Add(new MySqlParameter("hatsu", num4));
                        cmd3.Parameters.Add(new MySqlParameter("nyukasu", num3));
                        cmd3.Parameters.Add(new MySqlParameter("jyuchusu", num1));
                        cmd3.Parameters.Add(new MySqlParameter("torioki", num2));
                        cmd3.Parameters.Add(new MySqlParameter("zaiko", num3));
                        DateTime.TryParse(txtNyukabi.Text, out dt);
                        if (iDpKubun == 3)
                            cmd3.Parameters.Add(new MySqlParameter("nyukabi", dt));

                        string strsku = "";
                        int l = 10;
                        for (int j = 0; j < 11; j++)
                        {
                            for (int k = 0; k < 11; k++)
                            {
                                strsku = "sku" + l.ToString();
                                if (flgsku0)
                                {
                                    cmd3.Parameters.Add(new MySqlParameter(strsku, sizecolor[i, k, j]));
                                }
                                else
                                {
                                    cmd3.Parameters.Add(new MySqlParameter(strsku, (object)0));
                                }
                                l++;
                            }
                        }

                        //MySqlCommand cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
                        try
                        {
                            // オープン
                            cmd3.Connection.Open();
                            // 実行
                            cmd3.ExecuteNonQuery();
                            // 更新IDを取得
                            //var id = cmd2.ExecuteScalar();
                            // クローズ
                            cmd3.Connection.Close();
                        }
                        catch (SqlException ex)
                        {
                            // 例外処理
                            MessageBox.Show("例外発生:" + ex.Message);
                        }
                    }
                    zaiko_update_last(i);
                }
                i++;
                
            }
        }
        #endregion

        #region 在庫マスタ更新処理（マイナス更新）zaiko_update_minas()
        private void zaiko_update_minas()
        {
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            string nyukabi0 = "";
            string nyukabi1 = "";
            string nyukabi2 = "";
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            switch (iDpKubun)
            {
                case 1:
                    num1 = suryou_bk;
                    num2 = 0;
                    num3 = 0;
                    num4 = 0;
                    break;
                case 2:
                    num1 = 0;
                    num2 = suryou_bk;
                    num3 = 0;
                    num4 = 0;
                    break;
                case 3:
                    nyukabi0 = "nyukabi,";
                    nyukabi1 = "@nyukabi,";
                    nyukabi2 = "nyukabi = @nyukabi,";
                    num1 = 0;
                    num2 = 0;
                    num3 = suryou_bk;
                    num4 = 0;
                    break;
                case 4:
                    num1 = 0;
                    num2 = 0;
                    num3 = 0;
                    num4 = suryou_bk;
                    break;
            }
            MySqlCommand cmd3 = new MySqlCommand("update zaiko set hatsu = hatsu - @hatsu, nyukasu = nyukasu - @nyukasu, jyuchusu = jyuchusu - @jyuchusu,  " +
                "torioki = torioki - @torioki, zaiko = zaiko - @zaiko, " + nyukabi2 +
                "sku10 = sku10 - @sku10,sku11 = sku11 - @sku11,sku12 = sku12 - @sku12,sku13 = sku13 - @sku13,sku14 = sku14 - @sku14,sku15 = sku15 - @sku15," +
                "sku16 = sku16 - @sku16,sku17 = sku17 - @sku17,sku18 = sku18 - @sku18,sku19 = sku19 - @sku19,sku20 = sku20 - @sku20," +
                "sku21 = sku21 - @sku21, sku22 = sku22 - @sku22, sku23 = sku23 - @sku23, sku24 = sku24 - @sku24, sku25 = sku25 - @sku25," +
                "sku26 = sku26 - @sku26, sku27 = sku27 - @sku27, sku28 = sku28 - @sku28, sku29 = sku29 - @sku29, sku30 = sku30 - @sku30," +
                "sku31 = sku31 - @sku31, sku32 = sku32 - @sku32, sku33 = sku33 - @sku33, sku34 = sku34 - @sku34, sku35 = sku35 - @sku35," +
                "sku36 = sku36 - @sku36, sku37 = sku37 - @sku37, sku38 = sku38 - @sku38, sku39 = sku39 - @sku39, sku40 = sku40 - @sku40," +
                "sku41 = sku41 - @sku41, sku42 = sku42 - @sku42, sku43 = sku43 - @sku43, sku44 = sku44 - @sku44, sku45 = sku45 - @sku45," +
                "sku46 = sku46 - @sku46, sku47 = sku47 - @sku47, sku48 = sku48 - @sku48, sku49 = sku49 - @sku49, sku50 = sku50 - @sku50," +
                "sku51 = sku51 - @sku51, sku52 = sku52 - @sku52, sku53 = sku53 - @sku53, sku54 = sku54 - @sku54, sku55 = sku55 - @sku55," +
                "sku56 = sku56 - @sku56, sku57 = sku57 - @sku57, sku58 = sku58 - @sku58, sku59 = sku59 - @sku59, sku60 = sku60 - @sku60," +
                "sku61 = sku61 - @sku61, sku62 = sku62 - @sku62, sku63 = sku63 - @sku63, sku64 = sku64 - @sku64, sku65 = sku65 - @sku65," +
                "sku66 = sku66 - @sku66, sku67 = sku67 - @sku67, sku68 = sku68 - @sku68, sku69 = sku69 - @sku69, sku70 = sku70 - @sku70," +
                "sku71 = sku71 - @sku71, sku72 = sku72 - @sku72, sku73 = sku73 - @sku73, sku74 = sku74 - @sku74, sku75 = sku75 - @sku75," +
                "sku76 = sku76 - @sku76, sku77 = sku77 - @sku77, sku78 = sku78 - @sku78, sku79 = sku79 - @sku79, sku80 = sku80 - @sku80," +
                "sku81 = sku81 - @sku81, sku82 = sku82 - @sku82, sku83 = sku83 - @sku83, sku84 = sku84 - @sku84, sku85 = sku85 - @sku85," +
                "sku86 = sku86 - @sku86, sku87 = sku87 - @sku87, sku88 = sku88 - @sku88, sku89 = sku89 - @sku89, sku30 = sku30 - @sku30," +
                "sku91 = sku91 - @sku91, sku92 = sku92 - @sku92, sku93 = sku93 - @sku93, sku94 = sku94 - @sku94, sku95 = sku95 - @sku95," +
                "sku96 = sku96 - @sku96, sku97 = sku97 - @sku97, sku98 = sku98 - @sku98, sku99 = sku99 - @sku99, sku100 = sku100 - @sku100," +
                "sku101 = sku101 - @sku101, sku102 = sku102 - @sku102, sku103 = sku103 - @sku103, sku104 = sku104 - @sku104, sku105 = sku105 - @sku105," +
                "sku106 = sku106 - @sku106, sku107 = sku107 - @sku107, sku108 = sku108 - @sku108, sku109 = sku109 - @sku109, sku110 = sku110 - @sku110," +
                "sku111 = sku111 - @sku111, sku112 = sku112 - @sku112, sku113 = sku113 - @sku113, sku114 = sku114 - @sku114, sku115 = sku115 - @sku115," +
                "sku116 = sku116 - @sku116, sku117 = sku117 - @sku117, sku118 = sku118 - @sku118, sku119 = sku119 - @sku119, sku120 = sku120 - @sku120," +
                "sku121 = sku121 - @sku121, sku122 = sku122 - @sku122, sku123 = sku123 - @sku123, sku124 = sku124 - @sku124, sku125 = sku125 - @sku125," +
                "sku126 = sku126 - @sku126, sku127 = sku127 - @sku127, sku128 = sku128 - @sku128, sku129 = sku129 - @sku129, sku130 = sku130 - @sku130" +
                " where souko = @souko and shocd = @shocd", conn);
            // パラメータ設定
            cmd3.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
            cmd3.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, 0].Value));
            cmd3.Parameters.Add(new MySqlParameter("hatsu", num4));
            cmd3.Parameters.Add(new MySqlParameter("nyukasu", num3));
            cmd3.Parameters.Add(new MySqlParameter("jyuchusu", num1));
            cmd3.Parameters.Add(new MySqlParameter("torioki", num2));
            cmd3.Parameters.Add(new MySqlParameter("zaiko", num3));
            DateTime.TryParse(txtNyukabi.Text, out dt);
            if (iDpKubun == 3)
                cmd3.Parameters.Add(new MySqlParameter("nyukabi", dt));

            string strsku = "";
            int l = 10;
            for (int j = 0; j < 11; j++)
            {
                for (int k = 0; k < 11; k++)
                {
                    strsku = "sku" + l.ToString();
                    if (iDpKubun == 3)
                    {
                        cmd3.Parameters.Add(new MySqlParameter(strsku, sizecolor_bk[k, j]));
                    }
                    else
                    {
                        cmd3.Parameters.Add(new MySqlParameter(strsku, (object)0));
                    }
                    l++;
                }
            }

            //MySqlCommand cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
            try
            {
                // オープン
                cmd3.Connection.Open();
                // 実行
                cmd3.ExecuteNonQuery();
                // 更新IDを取得
                //var id = cmd2.ExecuteScalar();
                // クローズ
                cmd3.Connection.Close();
            }
            catch (SqlException ex)
            {
                // 例外処理
                MessageBox.Show("例外発生:" + ex.Message);
            }
        }
        #endregion

        #region 在庫マスタ更新処理（プラス更新）zaiko_update_plus()
        private void zaiko_update_plus()
        {//サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            string nyukabi0 = "";
            string nyukabi1 = "";
            string nyukabi2 = "";
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            switch (iDpKubun)
            {
                case 1:
                    num1 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    num2 = 0;
                    num3 = 0;
                    num4 = 0;
                    break;
                case 2:
                    num1 = 0;
                    num2 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    num3 = 0;
                    num4 = 0;
                    break;
                case 3:
                    nyukabi0 = "nyukabi,";
                    nyukabi1 = "@nyukabi,";
                    nyukabi2 = "nyukabi = @nyukabi,";
                    num1 = 0;
                    num2 = 0;
                    num3 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    num4 = 0;
                    break;
                case 4:
                    num1 = 0;
                    num2 = 0;
                    num3 = 0;
                    num4 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    break;
            }
            MySqlCommand cmd3 = new MySqlCommand("update zaiko set hatsu = hatsu + @hatsu, nyukasu = nyukasu + @nyukasu, jyuchusu = jyuchusu + @jyuchusu, " +
                "torioki = torioki + @torioki, zaiko = zaiko + @zaiko, " + nyukabi2 +
                "sku10 = sku10 + @sku10,sku11 = sku11 + @sku11,sku12 = sku12 + @sku12,sku13 = sku13 + @sku13,sku14 = sku14 + @sku14,sku15 = sku15 + @sku15," +
                "sku16 = sku16 + @sku16,sku17 = sku17 + @sku17,sku18 = sku18 + @sku18,sku19 = sku19 + @sku19,sku20 = sku20 + @sku20," +
                "sku21 = sku21 + @sku21, sku22 = sku22 + @sku22, sku23 = sku23 + @sku23, sku24 = sku24 + @sku24, sku25 = sku25 + @sku25," +
                "sku26 = sku26 + @sku26, sku27 = sku27 + @sku27, sku28 = sku28 + @sku28, sku29 = sku29 + @sku29, sku30 = sku30 + @sku30," +
                "sku31 = sku31 + @sku31, sku32 = sku32 + @sku32, sku33 = sku33 + @sku33, sku34 = sku34 + @sku34, sku35 = sku35 + @sku35," +
                "sku36 = sku36 + @sku36, sku37 = sku37 + @sku37, sku38 = sku38 + @sku38, sku39 = sku39 + @sku39, sku40 = sku40 + @sku40," +
                "sku41 = sku41 + @sku41, sku42 = sku42 + @sku42, sku43 = sku43 + @sku43, sku44 = sku44 + @sku44, sku45 = sku45 + @sku45," +
                "sku46 = sku46 + @sku46, sku47 = sku47 + @sku47, sku48 = sku48 + @sku48, sku49 = sku49 + @sku49, sku50 = sku50 + @sku50," +
                "sku51 = sku51 + @sku51, sku52 = sku52 + @sku52, sku53 = sku53 + @sku53, sku54 = sku54 + @sku54, sku55 = sku55 + @sku55," +
                "sku56 = sku56 + @sku56, sku57 = sku57 + @sku57, sku58 = sku58 + @sku58, sku59 = sku59 + @sku59, sku60 = sku60 + @sku60," +
                "sku61 = sku61 + @sku61, sku62 = sku62 + @sku62, sku63 = sku63 + @sku63, sku64 = sku64 + @sku64, sku65 = sku65 + @sku65," +
                "sku66 = sku66 + @sku66, sku67 = sku67 + @sku67, sku68 = sku68 + @sku68, sku69 = sku69 + @sku69, sku70 = sku70 + @sku70," +
                "sku71 = sku71 + @sku71, sku72 = sku72 + @sku72, sku73 = sku73 + @sku73, sku74 = sku74 + @sku74, sku75 = sku75 + @sku75," +
                "sku76 = sku76 + @sku76, sku77 = sku77 + @sku77, sku78 = sku78 + @sku78, sku79 = sku79 + @sku79, sku80 = sku80 + @sku80," +
                "sku81 = sku81 + @sku81, sku82 = sku82 + @sku82, sku83 = sku83 + @sku83, sku84 = sku84 + @sku84, sku85 = sku85 + @sku85," +
                "sku86 = sku86 + @sku86, sku87 = sku87 + @sku87, sku88 = sku88 + @sku88, sku89 = sku89 + @sku89, sku30 = sku30 + @sku30," +
                "sku91 = sku91 + @sku91, sku92 = sku92 + @sku92, sku93 = sku93 + @sku93, sku94 = sku94 + @sku94, sku95 = sku95 + @sku95," +
                "sku96 = sku96 + @sku96, sku97 = sku97 + @sku97, sku98 = sku98 + @sku98, sku99 = sku99 + @sku99, sku100 = sku100 + @sku100," +
                "sku101 = sku101 + @sku101, sku102 = sku102 + @sku102, sku103 = sku103 + @sku103, sku104 = sku104 + @sku104, sku105 = sku105 + @sku105," +
                "sku106 = sku106 + @sku106, sku107 = sku107 + @sku107, sku108 = sku108 + @sku108, sku109 = sku109 + @sku109, sku110 = sku110 + @sku110," +
                "sku111 = sku111 + @sku111, sku112 = sku112 + @sku112, sku113 = sku113 + @sku113, sku114 = sku114 + @sku114, sku115 = sku115 + @sku115," +
                "sku116 = sku116 + @sku116, sku117 = sku117 + @sku117, sku118 = sku118 + @sku118, sku119 = sku119 + @sku119, sku120 = sku120 + @sku120," +
                "sku121 = sku121 + @sku121, sku122 = sku122 + @sku122, sku123 = sku123 + @sku123, sku124 = sku124 + @sku124, sku125 = sku125 + @sku125," +
                "sku126 = sku126 + @sku126, sku127 = sku127 + @sku127, sku128 = sku128 + @sku128, sku129 = sku129 + @sku129, sku130 = sku130 + @sku130" +
                " where souko = @souko and shocd = @shocd", conn);
            // パラメータ設定
            cmd3.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
            cmd3.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, 0].Value));
            cmd3.Parameters.Add(new MySqlParameter("hatsu", num4));
            cmd3.Parameters.Add(new MySqlParameter("nyukasu", num3));
            cmd3.Parameters.Add(new MySqlParameter("jyuchusu", num1));
            cmd3.Parameters.Add(new MySqlParameter("torioki", num2));
            cmd3.Parameters.Add(new MySqlParameter("zaiko", num3));
            DateTime.TryParse(txtNyukabi.Text, out dt);
            if (iDpKubun == 3)
                cmd3.Parameters.Add(new MySqlParameter("nyukabi", dt));

            string strsku = "";
            int l = 10;
            for (int j = 0; j < 11; j++)
            {
                for (int k = 0; k < 11; k++)
                {
                    strsku = "sku" + l.ToString();
                    if (iDpKubun == 3)
                    {
                        cmd3.Parameters.Add(new MySqlParameter(strsku, sizecolor[0, k, j]));
                    }
                    else
                    {
                        cmd3.Parameters.Add(new MySqlParameter(strsku, (object)0));
                    }
                    l++;
                }
            }

            //MySqlCommand cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
            try
            {
                // オープン
                cmd3.Connection.Open();
                // 実行
                cmd3.ExecuteNonQuery();
                // 更新IDを取得
                //var id = cmd2.ExecuteScalar();
                // クローズ
                cmd3.Connection.Close();
            }
            catch (SqlException ex)
            {
                // 例外処理
                MessageBox.Show("例外発生:" + ex.Message);
            }
        }
        #endregion

        #region 在庫マスタ更新処理（新規追加）zaiko_update_new()
        private void zaiko_update_new()
        {
            zaiko_insert(1);
        }
        #endregion

        #region 在庫マスタ更新処理（担当者コードを倉庫コードにＳＥＴ） zaiko_update_last()
        private void zaiko_update_last(int i2)
        {
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            if (iDpKubun == 2)
            {
                MySqlCommand cmd = new MySqlCommand("update zaiko set souko = @tancd where souko = @souko and shocd = @shocd", conn);
                // パラメータ設定
                cmd.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
                cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, i2].Value));
                cmd.Parameters.Add(new MySqlParameter("tancd", txtTantoucode.Text));
                try
                {
                    // オープン
                    cmd.Connection.Open();
                    // 実行
                    cmd.ExecuteNonQuery();
                    // 更新IDを取得
                    //var id = cmd2.ExecuteScalar();
                    // クローズ
                    cmd.Connection.Close();
                }
                catch (SqlException ex)
                {
                    // 例外処理
                    MessageBox.Show("例外発生:" + ex.Message);
                }
            }
        }
        #endregion

        #region 在庫マスタ削除処理 zaiko_delete()
        private void zaiko_delete()
        {
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            string nyukabi0 = "";
            string nyukabi1 = "";
            string nyukabi2 = "";
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            switch (iDpKubun)
            {
                case 1:
                    num1 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    num2 = 0;
                    num3 = 0;
                    num4 = 0;
                    break;
                case 2:
                    num1 = 0;
                    num2 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    num3 = 0;
                    num4 = 0;
                    break;
                case 3:
                    nyukabi0 = "nyukabi,";
                    nyukabi1 = "@nyukabi,";
                    nyukabi2 = "nyukabi = @nyukabi,";
                    num1 = 0;
                    num2 = 0;
                    num3 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    num4 = 0;
                    break;
                case 4:
                    num1 = 0;
                    num2 = 0;
                    num3 = 0;
                    num4 = int.Parse(dataGridView2[colsuryou, 0].Value.ToString());
                    break;
            }
            MySqlCommand cmd3 = new MySqlCommand("update zaiko set hatsu = hatsu - @hatsu, nyukasu = nyukasu - @nyukasu, jyuchusu = jyuchusu - @jyuchusu,  " +
                "torioki = torioki - @torioki, zaiko = zaiko - @zaiko, " + nyukabi2 +
                "sku10 = sku10 - @sku10,sku11 = sku11 - @sku11,sku12 = sku12 - @sku12,sku13 = sku13 - @sku13,sku14 = sku14 - @sku14,sku15 = sku15 - @sku15," +
                "sku16 = sku16 - @sku16,sku17 = sku17 - @sku17,sku18 = sku18 - @sku18,sku19 = sku19 - @sku19,sku20 = sku20 - @sku20," +
                "sku21 = sku21 - @sku21, sku22 = sku22 - @sku22, sku23 = sku23 - @sku23, sku24 = sku24 - @sku24, sku25 = sku25 - @sku25," +
                "sku26 = sku26 - @sku26, sku27 = sku27 - @sku27, sku28 = sku28 - @sku28, sku29 = sku29 - @sku29, sku30 = sku30 - @sku30," +
                "sku31 = sku31 - @sku31, sku32 = sku32 - @sku32, sku33 = sku33 - @sku33, sku34 = sku34 - @sku34, sku35 = sku35 - @sku35," +
                "sku36 = sku36 - @sku36, sku37 = sku37 - @sku37, sku38 = sku38 - @sku38, sku39 = sku39 - @sku39, sku40 = sku40 - @sku40," +
                "sku41 = sku41 - @sku41, sku42 = sku42 - @sku42, sku43 = sku43 - @sku43, sku44 = sku44 - @sku44, sku45 = sku45 - @sku45," +
                "sku46 = sku46 - @sku46, sku47 = sku47 - @sku47, sku48 = sku48 - @sku48, sku49 = sku49 - @sku49, sku50 = sku50 - @sku50," +
                "sku51 = sku51 - @sku51, sku52 = sku52 - @sku52, sku53 = sku53 - @sku53, sku54 = sku54 - @sku54, sku55 = sku55 - @sku55," +
                "sku56 = sku56 - @sku56, sku57 = sku57 - @sku57, sku58 = sku58 - @sku58, sku59 = sku59 - @sku59, sku60 = sku60 - @sku60," +
                "sku61 = sku61 - @sku61, sku62 = sku62 - @sku62, sku63 = sku63 - @sku63, sku64 = sku64 - @sku64, sku65 = sku65 - @sku65," +
                "sku66 = sku66 - @sku66, sku67 = sku67 - @sku67, sku68 = sku68 - @sku68, sku69 = sku69 - @sku69, sku70 = sku70 - @sku70," +
                "sku71 = sku71 - @sku71, sku72 = sku72 - @sku72, sku73 = sku73 - @sku73, sku74 = sku74 - @sku74, sku75 = sku75 - @sku75," +
                "sku76 = sku76 - @sku76, sku77 = sku77 - @sku77, sku78 = sku78 - @sku78, sku79 = sku79 - @sku79, sku80 = sku80 - @sku80," +
                "sku81 = sku81 - @sku81, sku82 = sku82 - @sku82, sku83 = sku83 - @sku83, sku84 = sku84 - @sku84, sku85 = sku85 - @sku85," +
                "sku86 = sku86 - @sku86, sku87 = sku87 - @sku87, sku88 = sku88 - @sku88, sku89 = sku89 - @sku89, sku30 = sku30 - @sku30," +
                "sku91 = sku91 - @sku91, sku92 = sku92 - @sku92, sku93 = sku93 - @sku93, sku94 = sku94 - @sku94, sku95 = sku95 - @sku95," +
                "sku96 = sku96 - @sku96, sku97 = sku97 - @sku97, sku98 = sku98 - @sku98, sku99 = sku99 - @sku99, sku100 = sku100 - @sku100," +
                "sku101 = sku101 - @sku101, sku102 = sku102 - @sku102, sku103 = sku103 - @sku103, sku104 = sku104 - @sku104, sku105 = sku105 - @sku105," +
                "sku106 = sku106 - @sku106, sku107 = sku107 - @sku107, sku108 = sku108 - @sku108, sku109 = sku109 - @sku109, sku110 = sku110 - @sku110," +
                "sku111 = sku111 - @sku111, sku112 = sku112 - @sku112, sku113 = sku113 - @sku113, sku114 = sku114 - @sku114, sku115 = sku115 - @sku115," +
                "sku116 = sku116 - @sku116, sku117 = sku117 - @sku117, sku118 = sku118 - @sku118, sku119 = sku119 - @sku119, sku120 = sku120 - @sku120," +
                "sku121 = sku121 - @sku121, sku122 = sku122 - @sku122, sku123 = sku123 - @sku123, sku124 = sku124 - @sku124, sku125 = sku125 - @sku125," +
                "sku126 = sku126 - @sku126, sku127 = sku127 - @sku127, sku128 = sku128 - @sku128, sku129 = sku129 - @sku129, sku130 = sku130 - @sku130" +
                " where souko = @souko and shocd = @shocd", conn);
            // パラメータ設定
            cmd3.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
            cmd3.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, 0].Value));
            cmd3.Parameters.Add(new MySqlParameter("hatsu", num4));
            cmd3.Parameters.Add(new MySqlParameter("nyukasu", num3));
            cmd3.Parameters.Add(new MySqlParameter("jyuchusu", num1));
            cmd3.Parameters.Add(new MySqlParameter("torioki", num2));
            cmd3.Parameters.Add(new MySqlParameter("zaiko", num3));
            DateTime.TryParse(txtNyukabi.Text, out dt);
            if (iDpKubun == 3)
                cmd3.Parameters.Add(new MySqlParameter("nyukabi", dt));

            string strsku = "";
            int l = 10;
            for (int j = 0; j < 11; j++)
            {
                for (int k = 0; k < 11; k++)
                {
                    strsku = "sku" + l.ToString();
                    if (iDpKubun == 3)
                    {
                        cmd3.Parameters.Add(new MySqlParameter(strsku, sizecolor[0,k, j]));
                    }
                    else
                    {
                        cmd3.Parameters.Add(new MySqlParameter(strsku, (object)0));
                    }
                    l++;
                }
            }

            //MySqlCommand cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
            try
            {
                // オープン
                cmd3.Connection.Open();
                // 実行
                cmd3.ExecuteNonQuery();
                // 更新IDを取得
                //var id = cmd2.ExecuteScalar();
                // クローズ
                cmd3.Connection.Close();
            }
            catch (SqlException ex)
            {
                // 例外処理
                MessageBox.Show("例外発生:" + ex.Message);
            }
            
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("取消しますか？",
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                InitGrid();
                //dataGridView2.CurrentCell = null;
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void btnTouroku_Click(object sender, EventArgs e)
        {
            string str = strkubun + "しますか？";
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show(str,
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                switch (iKubun)
                {
                    case 1:
                        Insertdata();
                        InitGrid();
                        break;
                    case 2:
                        updatedata();
                        //deletedata();
                        //Insertdata();
                        break;
                    case 3:
                        deletedata();
                        InitGrid();
                        break;
                }
                //txtTokuisakicode.Text = "";
                //txtTokuisakimei1.Text = "";
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
                MessageBoxDefaultButton.Button1);

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

        
        private void Nyukanyuryoku_Shown(object sender, EventArgs e)
        {
            dataGridView2.CurrentCell = null;
        }

        
        
        private void dataGridView2_CellParsing_1(object sender, DataGridViewCellParsingEventArgs e)
        {
            switch (dataGridView2.CurrentCellAddress.X)
            {
                case colsuryou:
                    //dataGridView2[colsuryou, dataGridView2.CurrentCellAddress.Y].Value = total;
                    break;
                case coltanka:

                    //Action a = () => dataGridView2.CurrentCell = dataGridView2[colbiko, dataGridView2.CurrentCellAddress.Y];
                    //BeginInvoke(a);
                    break;
            }
        }

        private void dataGridView2_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            switch (dataGridView2.CurrentCellAddress.X)
            {
                case colsuryou:
                    if (flgsku0)
                        dataGridView2[colsuryou, dataGridView2.CurrentCellAddress.Y].Value = total;
                    break;
            }
        }

        private void dataGridView2_CellValidating_1(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewEx dgv = (DataGridViewEx)sender;
            switch (dataGridView2.CurrentCellAddress.X)
            {
                case colsuryou:
                    if (e.FormattedValue.ToString() == "")
                    {
                        MessageBox.Show("数字を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //入力した値をキャンセルして元に戻すには、次のようにする
                        dgv.CancelEdit();
                        //キャンセルする
                        e.Cancel = true;
                    }
                    //dataGridView2[colsuryou, dataGridView2.CurrentCellAddress.Y].Value = total;
                    break;
                case coltanka:
                    if (e.FormattedValue.ToString() == "")
                    {
                        MessageBox.Show("数字を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //入力した値をキャンセルして元に戻すには、次のようにする
                        dgv.CancelEdit();
                        //キャンセルする
                        //e.Cancel = true;
                    }
                    break;
                case colkingaku:
                    dataGridView2[colkingaku, dataGridView2.CurrentCellAddress.Y].Value = int.Parse(dataGridView2[colsuryou, dataGridView2.CurrentCellAddress.Y].Value.ToString())
                        * int.Parse(dataGridView2[coltanka, dataGridView2.CurrentCellAddress.Y].Value.ToString());

                    break;
            }
        }

        private void dataGridView2_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            switch (dataGridView2.CurrentCellAddress.X)
            {
                // （No）
                case colno:
                    Action a = () => dataGridView2.CurrentCell = dataGridView2[colhinban, dataGridView2.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    break;
                // 商品名
                case colsyohinmei:
                    a = () => dataGridView2.CurrentCell = dataGridView2[colsuryou, dataGridView2.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    //dataGridView2.EndEdit();
                    break;

                // 入荷数
                case colsuryou:
                    showsku = true;
                    Showsku(dataGridView2.CurrentCellAddress.Y);
                    break;

                // 単価
                case coltanka:
                    if (flgsku0)
                        dataGridView2[colsuryou, dataGridView2.CurrentCellAddress.Y].Value = total;
                    break;
                // 仕入金額
                case colkingaku:
                    a = () => dataGridView2.CurrentCell = dataGridView2[colbiko, dataGridView2.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    break;
                // 備考
                case colbiko:
                    
                    break;
            }
        }

        private void dataGridView2_EditingControlShowing_1(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //表示されているコントロールがDataGridViewTextBoxEditingControlか調べる
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集のために表示されているコントロールを取得
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;

                //イベントハンドラを削除
                tb.KeyDown -= dataGridView2_KeyDown_1;
                tb.PreviewKeyDown -= dataGridView2_PreviewKeyDown_1;
                tb.KeyDown += dataGridView2_KeyDown_1;
                tb.PreviewKeyDown += dataGridView2_PreviewKeyDown_1;
            }
        }

        private void dataGridView2_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                switch (dataGridView2.CurrentCellAddress.X)
                {
                    case colhinban:
                        Detectsku(dataGridView2.CurrentCellAddress.Y);
                        break;
                    case colsuryou:
                                             
                            //if (flgtotal)
                            
                            dataGridView2[colsuryou, dataGridView2.CurrentCellAddress.Y].Value = total;
                        
                        break;
                }
                SendKeys.Send("{TAB}");
                e.Handled = true;
            }
        }

        private void dataGridView2_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            int col = dataGridView2.CurrentCellAddress.X;
            int row = dataGridView2.CurrentCellAddress.Y;
            //Boolean flgzero = true;
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                switch (col)
                {
                    case colhinban:
                        dataGridView2.EndEdit();
                        
                        break;
                    case colsuryou:
                        
                        //dataGridView2.CurrentCell = dataGridView2[coltanka, row];
                        break;
                        
                }
                
            }
            else if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.Up)
            {
                txtSiiresaki.Focus();
                dataGridView2.CurrentCell = null;
            }
        }

        private void Nyukanyuryoku_Load(object sender, EventArgs e)
        {
            InitGrid();
            txtKubun.Focus();
        }

        private void Showsku(int row)
        {
            //skuが"1"のとき、ダイアログを開く
            if (flgsku == "True" && iDpKubun == 3)
            {
                flgsku0 = true;
                flgsizecolor[row] = true;

                Sizecolor2 sc = new Sizecolor2();
                if (iKubun == 2)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            sc.scdata[i, j] = sizecolor_bk[i, j];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 11; i++)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            sc.scdata[i, j] = sizecolor[row, i, j];
                        }
                    }
                }
                if (showsku)
                    sc.ShowDialog();
                showsku = false;
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        sizecolor[row, i, j] = sc.scdata[i, j];
                        
                    }
                }
                //if (sc.scdata[0,0] == null)
                //Array.Copy(sc.scdata, sizecolor[row], sc.scdata.Length);
                dataGridView2[colsuryou, row].Value = sc.total;
                total = sc.total;
                flgtotal = true;
                //dataGridView2.EndEdit();
                //flgcell1 = true;
                sc.Dispose();
                //flgsku = "False";
                //if (flgzero)
                // MessageBox.Show("nosku");
            }
            else
            {
                flgsku0 = false;
                flgtotal = false;
                flgsizecolor[row] = false;
                total = int.Parse(dataGridView2[colsuryou, row].Value.ToString());
            }
        }

        private void Detectsku(int row)
        {
            //DataGridViewEx dgv = (DataGridViewEx)Enter;
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();

            //SQL実行
            MySqlCommand cmd = new MySqlCommand("SELECT shohinmei, sku FROM shohin where shocd = '" + dataGridView2[colhinban, row].Value.ToString() + "'", conn);
            MySqlDataReader reader = cmd.ExecuteReader();


            if (!reader.HasRows)
            {
                if (!flgcell2)
                    MessageBox.Show("該当する商品がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                flgcell2 = true;
                //入力した値をキャンセルして元に戻すには、次のようにする
                //dgv.CancelEdit();
                //キャンセルする
                //e.Cancel = true;
                return;
            }
            //テーブル出力
            while (reader.Read())
            {
                dataGridView2[colsyohinmei, row].Value = reader["shohinmei"].ToString();
                flgsku = reader["sku"].ToString();
                //Console.WriteLine(string.Join("\t", row));
            }

            cmd.Connection.Close();
            cmd.Connection.Open();

            cmd = new MySqlCommand("SELECT * FROM zaiko where shocd = @shocd and souko = @souko", conn);
            cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, row].Value.ToString()));
            cmd.Parameters.Add(new MySqlParameter("souko", txtNyukasouko.Text));
            MySqlDataReader reader2 = cmd.ExecuteReader();


            if (reader2.HasRows)
            {
                flgnodata[row] = false;
            }
            else
            {
                // 在庫マスタにデータがない
                flgnodata[row] = true;
            }

            dataGridView2.EndEdit();
        }
    }
}