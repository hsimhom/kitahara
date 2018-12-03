using MySql.Data.MySqlClient;
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

namespace kitahara
{
    public partial class Syukkasiji : Form
    {
        // 在庫マスタにデータがない場合、true
        Boolean[] flgnodata = new Boolean[18];

        public int[,,] sizecolor = new int[18, 11, 11];

        string row1 = "";
        string colsku = "";
        int total = 0;
        // 入力区分
        int iKubun = 1;
        DateTime dt;
        string strkubun = "";

        Boolean flgtotal = false;

        const int colno = 0;
        const int colhinban = 1;
        const int colsyohinmei = 2;
        const int hattyusu = 3;
        const int nyukasu = 4;
        const int jittusu = 5;
        const int syukasu = 6;
        const int tzaiko = 7;
        const int kzaiko = 8;
        const int gzaiko = 9;
        const int kubun = 10;
        const int ttantocd = 11;
        const int syukkasijisu = 12;
        const int biko = 13;

        public Syukkasiji()
        {
            InitializeComponent();
            InitGrid();
        }

        #region datagridviewの初期化
        private void InitGrid()
        {
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();

            // データを格納するテーブル作成
            DataTable dt = new DataTable();

            // SQL文と接続情報を指定し、データアダプタを作成
            MySqlDataAdapter da = new MySqlDataAdapter("select * from tmp_syukkasiji", conn);

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
            dataGridView2.Columns[hattyusu].HeaderText = "発注数";
            dataGridView2.Columns[hattyusu].Width = 75;
            dataGridView2.Columns[hattyusu].ReadOnly = true;
            dataGridView2.Columns[nyukasu].HeaderText = "入荷数";
            dataGridView2.Columns[nyukasu].Width = 75;
            dataGridView2.Columns[nyukasu].ReadOnly = true;
            dataGridView2.Columns[jittusu].HeaderText = "受注数";
            dataGridView2.Columns[jittusu].Width = 75;
            dataGridView2.Columns[jittusu].ReadOnly = true;
            dataGridView2.Columns[syukasu].HeaderText = "出荷数";
            dataGridView2.Columns[syukasu].Width = 75;
            dataGridView2.Columns[syukasu].ReadOnly = true;
            dataGridView2.Columns[tzaiko].HeaderText = "取置在庫";
            dataGridView2.Columns[tzaiko].Width = 85;
            dataGridView2.Columns[tzaiko].ReadOnly = true;
            dataGridView2.Columns[kzaiko].HeaderText = "計算在庫";
            dataGridView2.Columns[kzaiko].Width = 85;
            dataGridView2.Columns[kzaiko].ReadOnly = true;
            dataGridView2.Columns[gzaiko].HeaderText = "現物在庫";
            dataGridView2.Columns[gzaiko].Width = 85;
            dataGridView2.Columns[gzaiko].ReadOnly = true;
            dataGridView2.Columns[kubun].HeaderText = "出庫区分";
            dataGridView2.Columns[kubun].Width = 85;
            dataGridView2.Columns[ttantocd].HeaderText = "取置担当CD";
            dataGridView2.Columns[ttantocd].Width = 110;
            dataGridView2.Columns[syukkasijisu].HeaderText = "出荷指示数";
            dataGridView2.Columns[syukkasijisu].Width = 100;
            dataGridView2.Columns[biko].HeaderText = "摘要";
            dataGridView2.Columns[biko].Width = 200;
        }
        #endregion

        #region textboxの初期化
        private void InitText()
        {

        }
        #endregion

        #region //DataGridView以外のコントロールのキー押下検知
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
        #endregion

        #region エンターを押したときのtextboxの処理
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
                            txtNefuda.Text = "0";
                            txtSeel.Text = "0";
                            txtKakusi.Text = "0";
                            txtYokohari.Text = "0";
                            txtKensin.Text = "0";
                            txtNohinsyo.Text = "0";
                            txtBinil.Text = "0";
                            txtTantoucode.Focus();
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
                case "txtTantoucode":
                    //SQL実行
                    //MySqlCommand cmd = new MySqlCommand("SELECT tanmei FROM tantousha where tancd = '" + txtTantoucode.Text + "'", conn);
                    MySqlCommand cmd = new MySqlCommand("SELECT tanmei FROM tantousha where tancd = @tancd", conn);
                    cmd.Parameters.Add(new MySqlParameter("tancd", txtTantoucode.Text));
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //テーブル出力
                        while (reader.Read())
                        {
                            row = reader["tanmei"].ToString();
                        }
                        txtTantoumei.Text = row;
                        txtSyukosouko.Focus();
                    }
                    else
                    {
                        MessageBox.Show("該当する担当者がいません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTantoucode.Focus();
                        return;
                    }
                    break;
                case "txtSyukosouko":
                    //SQL実行
                    cmd = new MySqlCommand("SELECT sokomei FROM souko where soucd = '" + txtSyukosouko.Text + "'", conn);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //テーブル出力
                        while (reader.Read())
                        {
                            row = reader["sokomei"].ToString();
                        }
                        txtSoukomei.Text = row;
                        txtSijibi.Focus();
                    }
                    else
                    {
                        MessageBox.Show("該当する倉庫がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSyukosouko.Focus();
                        return;
                    }
                    break;
                case "txtSijibi":

                    if (!DateTime.TryParse(txtSijibi.Text, out dt))
                    {
                        //変換出来たら、dtにその値が入る
                        MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSijibi.Focus();
                    }
                    else
                        txtTokuisakicode.Focus();
                    break;
                case "txtTokuisakicode":
                    //SQL実行
                    cmd = new MySqlCommand("SELECT * FROM tokuisaki where tokuicd = '" + txtTokuisakicode.Text + "'", conn);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //テーブル出力
                        while (reader.Read())
                        {
                            row = reader["tokuisakimei"].ToString();
                            txtTokuisakimei2.Text = reader["tokuisakimei2"].ToString();
                            txtTokuisakimei3.Text = reader["tokuisakimei3"].ToString();
                            txtNefuda.Text = reader["nefuda"].ToString();
                            txtSeel.Text = reader["siru"].ToString();
                            txtKakusi.Text = reader["hinkakusi"].ToString();
                            txtYokohari.Text = reader["yokohari"].ToString();
                            txtKensin.Text = reader["kensin"].ToString();
                            txtNohinsyo.Text = reader["senyou"].ToString();
                            txtBinil.Text = reader["biniru"].ToString();
                        }
                        txtTokuisakimei1.Text = row;
                        txtTokuisakimei2.Focus();
                        //dataGridView2.Focus();
                    }
                    else
                    {
                        MessageBox.Show("該当する得意先がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTokuisakicode.Focus();
                        return;
                    }
                    break;
                case "txtTokuisakimei2":
                    txtTokuisakimei3.Focus();
                    break;
                case "txtTokuisakimei3":
                    txtSyukabi.Focus();
                    break;
                case "txtSyukabi":
                    if (!DateTime.TryParse(txtSyukabi.Text, out dt))
                    {
                        if (txtSyukabi.TextLength != 4)
                        {
                            //変換出来たら、dtにその値が入る
                            MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtSyukabi.Focus();
                        }
                        else
                        {
                            DateTime dt1 = DateTime.Now;
                            string temp1 = txtSyukabi.Text;
                            string temp2 = temp1.Insert(2, "/");
                            string temp3 = temp2.Insert(0, "/");
                            txtSyukabi.Text = dt1.Year.ToString() + temp3;
                            txtTyakabi.Focus();
                        }
                    }
                    else
                        txtTyakabi.Focus();
                    break;
                case "txtTyakabi":
                    if (!DateTime.TryParse(txtTyakabi.Text, out dt))
                    {
                        if (txtTyakabi.TextLength != 4)
                        {
                            //変換出来たら、dtにその値が入る
                            MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtTyakabi.Focus();
                        }
                        else
                        {
                            DateTime dt1 = DateTime.Now;
                            string temp1 = txtTyakabi.Text;
                            string temp2 = temp1.Insert(2, "/");
                            string temp3 = temp2.Insert(0, "/");
                            txtTyakabi.Text = dt1.Year.ToString() + temp3;
                            txtDenpyobi.Focus();
                        }
                    }
                    else
                        txtDenpyobi.Focus();
                    break;
                case "txtDenpyobi":
                    if (!DateTime.TryParse(txtDenpyobi.Text, out dt))
                    {
                        if (txtDenpyobi.TextLength != 4)
                        {
                            //変換出来たら、dtにその値が入る
                            MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtDenpyobi.Focus();
                        }
                        else
                        {
                            DateTime dt1 = DateTime.Now;
                            string temp1 = txtDenpyobi.Text;
                            string temp2 = temp1.Insert(2, "/");
                            string temp3 = temp2.Insert(0, "/");
                            txtDenpyobi.Text = dt1.Year.ToString() + temp3;
                            txtTyui.Focus();
                        }
                    }
                    else
                        txtTyui.Focus();
                    break;
                case "txtTyui":
                    dataGridView2.Focus();
                    break;
                case "txtBiko1":
                    txtBiko2.Focus();
                    break;
                case "txtBiko2":
                    txtBiko3.Focus();
                    break;
                case "txtBiko3":
                    txtBiko4.Focus();
                    break;
                case "txtNefuda":
                    if (!Regex.IsMatch(txtNefuda.Text, @"^[0-1]{1}$"))
                    {
                        MessageBox.Show("0か1を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtNefuda.Focus();
                        return;
                    }                    
                    break;
                case "txtSeel":
                    if (!Regex.IsMatch(txtSeel.Text, @"^[0-1]{1}$"))
                    {
                        MessageBox.Show("0か1を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSeel.Focus();
                        return;
                    }
                    break;
                case "txtKakusi":
                    if (!Regex.IsMatch(txtKakusi.Text, @"^[0-1]{1}$"))
                    {
                        MessageBox.Show("0か1を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtKakusi.Focus();
                        return;
                    }
                    break;
                case "txtYokohari":
                    if (!Regex.IsMatch(txtYokohari.Text, @"^[0-1]{1}$"))
                    {
                        MessageBox.Show("0か1を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtYokohari.Focus();
                        return;
                    }
                    break;
                case "txtKensin":
                    if (!Regex.IsMatch(txtKensin.Text, @"^[0-1]{1}$"))
                    {
                        MessageBox.Show("0か1を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtKensin.Focus();
                        return;
                    }
                    break;
                case "txtNohinsyo":
                    if (!Regex.IsMatch(txtNohinsyo.Text, @"^[0-1]{1}$"))
                    {
                        MessageBox.Show("0か1を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtNohinsyo.Focus();
                        return;
                    }
                    break;
                case "txtBinil":
                    if (!Regex.IsMatch(txtBinil.Text, @"^[0-1]{1}$"))
                    {
                        MessageBox.Show("0か1を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtBinil.Focus();
                        return;
                    }
                    break;
            }
        }
        #endregion

        #region ログNoから表示を行う
        private void dispdata()
        {

        }
        #endregion

        #region DB追加、更新処理
        private void Insertdata()
        {
            int i = 0;
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);

            while (i < 18)
            {
                if (dataGridView2[colhinban, i].Value.ToString() != "")
                {
                    MySqlCommand cmd = new MySqlCommand("insert into syukalog ( gyono, nkbn, tancd, syukosocd, sijibi, tokuisaki, syuymd, chaymd," +
                        "denymd, nefudakbn,sirkbn,hinkbn,senkbn,kenkbn,sennokbn,binikbn,chuijiko,hinban,shohinmei,syukasu,tanka,urikin," +
                        "biko1,biko2,biko3,biko4," +
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
                        "( @gyono, @nkbn, @tancd, @syukosocd, @sijibi, @tokuisaki, @syuymd, @chaymd," +
                        "@denymd, @nefudakbn, @sirkbn,@hinkbn,@senkbn,@kenkbn,@sennokbn,@binikbn,@chuijiko,@hinban,@shohinmei,@syukasu,@tanka,@urikin," +
                        "@biko1,@biko2,@biko3,@biko4," +
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
                    cmd.Parameters.Add(new MySqlParameter("gyono", i));
                    cmd.Parameters.Add(new MySqlParameter("nkbn", iKubun));
                    cmd.Parameters.Add(new MySqlParameter("tancd", txtTantoucode.Text));
                    cmd.Parameters.Add(new MySqlParameter("syukosocd", txtSyukosouko.Text));
                    DateTime.TryParse(txtSijibi.Text, out dt);
                    cmd.Parameters.Add(new MySqlParameter("sijibi", dt));
                    cmd.Parameters.Add(new MySqlParameter("tokuisaki", txtTokuisakicode.Text));
                    DateTime.TryParse(txtSyukabi.Text, out dt);
                    cmd.Parameters.Add(new MySqlParameter("syuymd", dt));
                    DateTime.TryParse(txtTyakabi.Text, out dt);
                    cmd.Parameters.Add(new MySqlParameter("chaymd", dt));
                    DateTime.TryParse(txtDenpyobi.Text, out dt);
                    cmd.Parameters.Add(new MySqlParameter("denymd", dt));
                    cmd.Parameters.Add(new MySqlParameter("nefudakbn", txtNefuda.Text));
                    cmd.Parameters.Add(new MySqlParameter("sirkbn", txtSeel.Text));
                    cmd.Parameters.Add(new MySqlParameter("hinkbn", txtKakusi.Text));
                    cmd.Parameters.Add(new MySqlParameter("senkbn", txtYokohari.Text));
                    cmd.Parameters.Add(new MySqlParameter("kenkbn", txtKensin.Text));
                    cmd.Parameters.Add(new MySqlParameter("sennokbn", txtNohinsyo.Text));
                    cmd.Parameters.Add(new MySqlParameter("binikbn", txtBinil.Text));
                    cmd.Parameters.Add(new MySqlParameter("chuijiko", txtTyui.Text));
                    cmd.Parameters.Add(new MySqlParameter("hinban", dataGridView2[colhinban, i].Value));
                    cmd.Parameters.Add(new MySqlParameter("shohinmei", dataGridView2[colsyohinmei, i].Value));
                    cmd.Parameters.Add(new MySqlParameter("syukasu", dataGridView2[kubun, i].Value));
                    cmd.Parameters.Add(new MySqlParameter("tanka", dataGridView2[ttantocd, i].Value));
                    cmd.Parameters.Add(new MySqlParameter("urikin", dataGridView2[syukkasijisu, i].Value));
                    cmd.Parameters.Add(new MySqlParameter("biko1", txtBiko1.Text));
                    cmd.Parameters.Add(new MySqlParameter("biko2", txtBiko2.Text));
                    cmd.Parameters.Add(new MySqlParameter("biko3", txtBiko3.Text));
                    cmd.Parameters.Add(new MySqlParameter("biko4", txtBiko4.Text));
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
                    /*
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
                    */
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
                    
                    // 在庫マスタinsert or update
                    if (flgnodata[i])
                    {
                        cmd = new MySqlCommand("insert into zaiko ( souko, shocd, jyusyu, zaisyu, torisyu, syukasu, zaiko, syukabi," +
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
                        "( @souko, @shocd, @jyusyu, @zaisyu, @torisyu, @syukasu, @zaiko, @syukabi," +
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
                        cmd.Parameters.Add(new MySqlParameter("souko", txtSyukosouko.Text));
                        cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, i].Value));
                        int num = int.Parse(dataGridView2[syukkasijisu, i].Value.ToString());
                        int num1 = 0;
                        int num2 = 0;
                        int num3 = 0;
                        switch (dataGridView2[kubun, i].Value.ToString())
                        {
                            case "1":
                                num1 = num;
                                break;
                            case "2":
                                num2 = num;
                                break;
                            case "3":
                                num3 = num;
                                break;
                        }
                        cmd.Parameters.Add(new MySqlParameter("jyusyu", num1));
                        cmd.Parameters.Add(new MySqlParameter("zaisyu", num2));
                        cmd.Parameters.Add(new MySqlParameter("torisyu", num3));
                        cmd.Parameters.Add(new MySqlParameter("syukasu", num));
                        cmd.Parameters.Add(new MySqlParameter("zaiko", num));
                        DateTime.TryParse(txtSijibi.Text, out dt);
                        cmd.Parameters.Add(new MySqlParameter("syukabi", dt));
                        
                        strsku = "";
                        l = 10;
                        for (int j = 0; j < 11; j++)
                        {
                            for (int k = 0; k < 11; k++)
                            {
                                strsku = "sku" + l.ToString();
                                cmd.Parameters.Add(new MySqlParameter(strsku, sizecolor[i, k, j]));
                                l++;
                            }
                        }

                        //cmd2 = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
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
                    else
                    {
                        MySqlCommand cmd1 = new MySqlCommand("update zaiko set jyusyu = jyusyu + @jyusyu, zaisyu = zaisyu + @zaisyu, torisyu = torisyu + @torisyu, " +
                        "syukasu = syukasu + @syukasu, zaiko = zaiko - @zaiko, syukabi = @syukabi," +
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
                        cmd1.Parameters.Add(new MySqlParameter("souko", txtSyukosouko.Text));
                        cmd1.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, i].Value));
                        int num = int.Parse(dataGridView2[syukkasijisu, i].Value.ToString());
                        int num1 = 0;
                        int num2 = 0;
                        int num3 = 0;
                        switch (dataGridView2[kubun, i].Value.ToString())
                        {
                            case "1":
                                num1 = num;
                                break;
                            case "2":
                                num2 = num;
                                break;
                            case "3":
                                num3 = num;
                                break;
                        }
                        cmd1.Parameters.Add(new MySqlParameter("jyusyu", num1));
                        cmd1.Parameters.Add(new MySqlParameter("zaisyu", num2));
                        cmd1.Parameters.Add(new MySqlParameter("torisyu", num3));
                        cmd1.Parameters.Add(new MySqlParameter("syukasu", num));
                        cmd1.Parameters.Add(new MySqlParameter("zaiko", num));
                        DateTime.TryParse(txtSijibi.Text, out dt);
                        cmd1.Parameters.Add(new MySqlParameter("syukabi", dt));

                        strsku = "";
                        l = 10;
                        for (int j = 0; j < 11; j++)
                        {
                            for (int k = 0; k < 11; k++)
                            {
                                strsku = "sku" + l.ToString();
                                cmd1.Parameters.Add(new MySqlParameter(strsku, sizecolor[i, k, j]));
                                l++;
                            }
                        }

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
                }
                i++;
            }
            string str = strkubun + "しました。";
            MessageBox.Show(str);
        }
        #endregion

        
        private void Syukkasiji_Shown(object sender, EventArgs e)
        {
            dataGridView2.CurrentCell = null;
        }

        private void dataGridView2_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            switch (dataGridView2.CurrentCellAddress.X)
            {
                case hattyusu:
                    //dataGridView2[hattyusu, dataGridView2.CurrentCellAddress.Y].Value = total;
                    break;
                case nyukasu:

                    //Action a = () => dataGridView2.CurrentCell = dataGridView2[syukasu, dataGridView2.CurrentCellAddress.Y];
                    //BeginInvoke(a);
                    break;
            }
        }

        private void dataGridView2_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            switch (dataGridView2.CurrentCellAddress.X)
            {
                case hattyusu:
                    //dataGridView2[hattyusu, dataGridView2.CurrentCellAddress.Y].Value = total;
                    break;
            }
        }

        private void dataGridView2_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewEx dgv = (DataGridViewEx)sender;
            switch (dataGridView2.CurrentCellAddress.X)
            {
                case kubun:
                    if (e.FormattedValue.ToString() == "")
                    {
                        MessageBox.Show("数字を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //入力した値をキャンセルして元に戻すには、次のようにする
                        dgv.CancelEdit();
                        //キャンセルする
                        e.Cancel = true;
                    }
                    //dataGridView2[hattyusu, dataGridView2.CurrentCellAddress.Y].Value = total;
                    break;
                case ttantocd:
                    if (e.FormattedValue.ToString() == "")
                    {
                        MessageBox.Show("数字を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //入力した値をキャンセルして元に戻すには、次のようにする
                        dgv.CancelEdit();
                        //キャンセルする
                        //e.Cancel = true;
                    }
                    break;
                case syukkasijisu:
                    if (e.FormattedValue.ToString() == "")
                    {
                        MessageBox.Show("数字を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //入力した値をキャンセルして元に戻すには、次のようにする
                        dgv.CancelEdit();
                        //キャンセルする
                        //e.Cancel = true;
                    }
                    break;
            }
        }

        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
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
                case hattyusu:
                case nyukasu:
                case jittusu:
                case syukasu:
                case tzaiko:
                case kzaiko:
                case gzaiko:
                    a = () => dataGridView2.CurrentCell = dataGridView2[kubun, dataGridView2.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    //dataGridView2.EndEdit();
                    break;


            }
        }

        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //表示されているコントロールがDataGridViewTextBoxEditingControlか調べる
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集のために表示されているコントロールを取得
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;

                //イベントハンドラを削除
                tb.KeyDown -= dataGridView2_KeyDown;
                tb.PreviewKeyDown -= dataGridView2_PreviewKeyDown;
                tb.KeyDown += dataGridView2_KeyDown;
                tb.PreviewKeyDown += dataGridView2_PreviewKeyDown;
            }
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (dataGridView2.CurrentCellAddress.X)
            {
                case syukkasijisu:
                    if (flgtotal)
                    {
                        flgtotal = false;
                        dataGridView2[syukkasijisu, dataGridView2.CurrentCellAddress.Y].Value = total;
                        //SendKeys.Send("{TAB}");
                        //e.Handled = true;
                    }
                    break;
            }
            
         
        }

        private void dataGridView2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int ihattyusu = 0;
            int inyukasu = 0;
            int ijittusu = 0;
            int isyukasu = 0;
            int itzaiko = 0;
            int ikzaiko = 0;
            int igzaiko = 0;

            int col = dataGridView2.CurrentCellAddress.X;
            int row = dataGridView2.CurrentCellAddress.Y;

            //if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            if (e.KeyCode == Keys.Enter)
            {

                string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
                MySqlConnection conn = new MySqlConnection(connstr);
                conn.Open();

                //dataGridView2.EndEdit();
                if (col == colhinban)
                {
                    dataGridView2.EndEdit();
                    //サーバー接続


                    //SQL実行
                    MySqlCommand cmd = new MySqlCommand("SELECT shohinmei, sku, nyukasu, jyuchusu, syukasu FROM shohin where shocd = @shocd", conn);
                    cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, row].Value.ToString()));
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        MessageBox.Show("該当する商品がありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dataGridView2.CurrentCell = dataGridView2[colhinban, row];
                        return;
                    }
                    else
                    {
                        //テーブル出力
                        while (reader.Read())
                        {
                            row1 = reader["shohinmei"].ToString();
                            colsku = reader["sku"].ToString();

                            inyukasu = int.Parse(reader["nyukasu"].ToString());
                            ijittusu = int.Parse(reader["jyuchusu"].ToString());
                            isyukasu = int.Parse(reader["syukasu"].ToString());

                        }
                        dataGridView2[colsyohinmei, row].Value = row1;
                    }

                    cmd.Connection.Close();
                    cmd.Connection.Open();

                    cmd = new MySqlCommand("SELECT hatsu,torioki FROM zaiko where shocd = @shocd and souko = @souko", conn);
                    cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, row].Value.ToString()));
                    cmd.Parameters.Add(new MySqlParameter("souko", txtSyukosouko.Text));
                    MySqlDataReader reader2 = cmd.ExecuteReader();


                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            ihattyusu = int.Parse(reader2["hatsu"].ToString());
                            itzaiko = int.Parse(reader2["torioki"].ToString());
                        }
                        flgnodata[row] = false;
                    }
                    else
                    {
                        // 在庫マスタにデータがない
                        ihattyusu = 0;
                        itzaiko = 0;
                        flgnodata[row] = true;
                    }

                    ikzaiko = ihattyusu - ijittusu - isyukasu - itzaiko;
                    igzaiko = inyukasu = isyukasu;

                    dataGridView2[colsyohinmei, row].Value = row1;
                    dataGridView2[hattyusu, row].Value = ihattyusu;
                    dataGridView2[nyukasu, row].Value = inyukasu;
                    dataGridView2[jittusu, row].Value = ijittusu;
                    dataGridView2[syukasu, row].Value = isyukasu;
                    dataGridView2[tzaiko, row].Value = itzaiko;
                    dataGridView2[kzaiko, row].Value = ikzaiko;
                    dataGridView2[gzaiko, row].Value = igzaiko;
                    dataGridView2.EndEdit();
                    SendKeys.Send("{TAB}");

                }
                else if (col == syukkasijisu)
                {
                    switch (iKubun) {
                        case 1:
                            //skuが"1"のとき、ダイアログを開く
                            if (colsku == "True")
                            {
                                Sizecolor1 sc = new Sizecolor1();
                                sc.ShowDialog();
                                for (int i = 0; i < 11; i++)
                                {
                                    for (int j = 0; j < 11; j++)
                                    {
                                        sizecolor[row, i, j] = sc.scdata[i, j];
                                    }
                                }
                                dataGridView2[syukkasijisu, row].Value = sc.total;
                                total = sc.total;
                                flgtotal = true;
                                //dataGridView2.EndEdit();
                                sc.Dispose();
                                colsku = "False";
                                //SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                flgtotal = false;
                            }
                            if (itzaiko > 0 && igzaiko < int.Parse(dataGridView2[syukkasijisu, row].Value.ToString()))
                            {
                                MessageBox.Show("取置在庫から出荷してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                }
                
            }
            else if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.Up)
            {
                if (row == 0)
                {
                    txtTokuisakicode.Focus();
                    dataGridView2.CurrentCell = null;
                }
            }
        }

        #region テキストボックスのマウスクリック処理
        private void txtNefuda_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtNefuda.ReadOnly)
                txtNefuda.ReadOnly = false;
            else
                txtNefuda.ReadOnly = true;
        }

        private void txtSeel_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtSeel.ReadOnly)
                txtSeel.ReadOnly = false;
            else
                txtSeel.ReadOnly = true;

        }

        private void txtKakusi_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtKakusi.ReadOnly)
                txtKakusi.ReadOnly = false;
            else
                txtKakusi.ReadOnly = true;

        }

        private void txtYokohari_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtYokohari.ReadOnly)
                txtYokohari.ReadOnly = false;
            else
                txtYokohari.ReadOnly = true;

        }

        private void txtKensin_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtKensin.ReadOnly)
                txtKensin.ReadOnly = false;
            else
                txtKensin.ReadOnly = true;

        }

        private void txtNohinsyo_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtNohinsyo.ReadOnly)
                txtNohinsyo.ReadOnly = false;
            else
                txtNohinsyo.ReadOnly = true;

        }

        private void txtBinil_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtBinil.ReadOnly)
                txtBinil.ReadOnly = false;
            else
                txtBinil.ReadOnly = true;

        }
        #endregion

        private void btnTouroku_Click_1(object sender, EventArgs e)
        {
            string str = strkubun + "しますか？";
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show(str,
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
                //txtTokuisakicode.Text = "";
                //txtTokuisakimei1.Text = "";
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
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
                //dataGridView2.CurrentCell = null;
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void btnEnd_Click_1(object sender, EventArgs e)
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
    }
}