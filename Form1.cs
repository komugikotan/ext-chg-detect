using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//拡張子変更用
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Runtime.InteropServices;
//Webp用
//using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace ext_chg_detect
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            this.InitializeComponent();
            this.WindowState = FormWindowState.Minimized;

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;

            NotifyIcon notifyIcon;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("favicon.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "拡張子変更ディテクター";


            // コンテキストメニュー
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "&終了";
            toolStripMenuItem.Click += ToolStripMenuItem_Click;
            contextMenuStrip.Items.Add(toolStripMenuItem);
            notifyIcon.ContextMenuStrip = contextMenuStrip;


            if (GetValueString("basic", "path", "data\\info.ini") == "")
            {
                DialogResult result = MessageBox.Show("検知するフォルダーを設定しますか？", "未登録", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    BrowseFolder();

                    DialogResult result_startup = MessageBox.Show("フォルダの設定が完了しました。このアプリをコンピュータの起動時に自動的に起動するよう設定しますか？", "スタートアップ", MessageBoxButtons.YesNo);
                    if (result_startup == System.Windows.Forms.DialogResult.Yes)
                    {
                        SetValueString("basic", "startup", "true", "data\\info.ini");
                        RegisterStartupApp("Ext-Change-Detect", Application.ExecutablePath);
                        MessageBox.Show("スタートアップアプリに登録しました。このアプリはバックグラウンドで実行されるため、ウィンドウなどは表示されませんのでご注意ください。自動起動設定と、検知するフォルダーの設定は後から変更できます。");
                    }
                    else
                    { 
                        
                    }

                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
            }

            if (GetValueString("basic", "startup", "data\\info.ini") == "true")
            {
                RegisterStartupApp("Ext-Change-Detect", Application.ExecutablePath);
            }
            else
            {
                UnregisterStartupApp("Ext-Change-Detect");
            }


            label1.Text = GetValueString("basic", "path", "data\\info.ini");

            //監視対象のフォルダの設定
            fsw.Path = GetValueString("basic", "path", "data\\info.ini");
            //監視する種類の設定
            fsw.NotifyFilter =
                (NotifyFilters.Attributes
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName);
            //サブディレクトリも監視
            fsw.IncludeSubdirectories = true;
            //すべてのファイルを監視しているならば
            fsw.Filter = "";
            //WindowFormなどUI用
            //コンソールではいらない
            fsw.SynchronizingObject = this;
            //イベント設定
            fsw.Renamed += fsw_Renamed;
            //監視を開始
            fsw.EnableRaisingEvents = true;

        }

        void BrowseFolder()
        {
            using (var ofd = new OpenFileDialog() { FileName = "SelectFolder", Filter = "Folder|.", CheckFileExists = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SetValueString("basic", "path", Path.GetDirectoryName(ofd.FileName), "data\\info.ini");
                }
            }
        }

        FileSystemWatcher fsw = new FileSystemWatcher();
        private void FileWatcher_Load(object sender, EventArgs e)
        {

        }

        void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            //イベントの発生を一時的に停止
            fsw.EnableRaisingEvents = false;

            //MessageBox.Show(e.OldName);

            string Old_ext = System.IO.Path.GetExtension(e.OldName);
            string New_ext = System.IO.Path.GetExtension(e.Name);

            label1.Text = Old_ext;

            ChangeFileExtension(e.FullPath, Old_ext);
           

            //拡張子が変わっている場合のみ処理を行う
            if (Old_ext != New_ext)
            {
                if (New_ext == ".png" && Old_ext == ".jpg")
                {
                    if (!File.Exists("tmp\\" + e.OldName))
                    {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }

                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "png", "jpg", e.OldName);
                    File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".jpg" && Old_ext == ".png")
                {
                    if (!File.Exists("tmp\\" + e.OldName)) {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }

                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "jpg", "png", e.OldName);
                   File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".jpeg" && Old_ext == ".png")
                {
                    if (!File.Exists("tmp\\" + e.OldName))
                    {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }

                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "jpg", "png", e.OldName);
                    File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".jpg" && Old_ext == ".webp")
                {
                    if (!File.Exists("tmp\\" + e.OldName))
                    {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }

                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "jpg", "webp", e.OldName);

                    File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".jpeg" && Old_ext == ".webp")
                {
                    if (!File.Exists("tmp\\" + e.OldName))
                    {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }

                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "jpg", "webp", e.OldName);

                    File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".png" && Old_ext == ".webp")
                {
                    if (!File.Exists("tmp\\" + e.OldName))
                    {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }
                        
                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "png", "webp", e.OldName);
                    
                    File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".webp" && Old_ext == ".jpg")
                {
                    if (!File.Exists("tmp\\" + e.OldName))
                    {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }

                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "webp", "jpg", e.OldName);

                    File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".webp" && Old_ext == ".png")
                {
                    if (!File.Exists("tmp\\" + e.OldName))
                    {
                        File.Copy(e.OldFullPath, "tmp\\" + e.OldName);
                    }

                    changeFormatOfPicture(e.OldFullPath, e.FullPath, "webp", "png", e.OldName);

                    File.Delete(e.OldFullPath);
                }

            }

            //イベントの発生を再開
            fsw.EnableRaisingEvents = true;
        }

        void changeFormatOfPicture(string fromFilePath, string toFilePath, string toExt, string fromExt, string OldName)
        {
            //File.Copy(fromFilePath, "tmp\\" + OldName);

            if (fromExt == "png")
            {
                if (toExt == "jpg")
                {
                    using (Image imageData = Image.FromFile("tmp\\" + OldName))
                    {
                        // 新しいビットマップを作成する
                        Bitmap bmp = new Bitmap(imageData.Width, imageData.Height);
                        // Graphicsオブジェクトを作成する
                        Graphics g = Graphics.FromImage(bmp);
                        // 背景を白色で塗りつぶす
                        g.Clear(Color.White);
                        // 元の画像を描画する
                        g.DrawImageUnscaled(imageData, 0, 0);
                        // Graphicsオブジェクトを破棄する
                        g.Dispose();
                        // ビットマップをJPEG形式で保存する
                        bmp.Save(toFilePath, ImageFormat.Jpeg);
                        // ビットマップを破棄する
                        bmp.Dispose();
                    }
                }
                else if(toExt == "webp"){
                    var wf = new WebPFormat();

                    using (var image = new Bitmap("tmp\\" + OldName))
                    {
                        wf.Save(toFilePath, image, 0);
                    }
                }

            }
            else if (fromExt == "jpg")
            {
                if (toExt == "png")
                {
                    using (Image imageData = Image.FromFile("tmp\\" + OldName))
                    {
                        // ビットマップをJPEG形式で保存する
                        imageData.Save(toFilePath, ImageFormat.Png);
                        // ビットマップを破棄する
                        imageData.Dispose();
                    }
                }
                else if (toExt == "webp")
                {
                    var wf = new WebPFormat();

                    using (var image = new Bitmap("tmp\\" + OldName))
                    {
                        wf.Save(toFilePath, image, 0);
                    }
                }
            }
            else if (fromExt == "webp")
            {
                var wf = new WebPFormat();

                if (toExt == "jpg")
                {
                    using (var image = (Bitmap)wf.Load(new FileStream("tmp\\" + OldName, FileMode.Open, FileAccess.Read)))
                    {
                        image.Save(toFilePath, ImageFormat.Jpeg);
                    }
                }
                else if (toExt == "png")
                {
                    using (var image = (Bitmap)wf.Load(new FileStream("tmp\\" + OldName, FileMode.Open, FileAccess.Read)))
                    {
                        image.Save(toFilePath, ImageFormat.Png);
                    }
                }
            }
        }




        // ファイル名と新しい拡張子を引数にとる関数
        public static void ChangeFileExtension(string fileName, string newExtension)
        {
            try
            {
                // ファイルが存在するかどうかをチェックする
                if (File.Exists(fileName))
                {
                    // ファイルのパスと名前を分離する
                    string filePath = Path.GetDirectoryName(fileName);
                    string fileBaseName = Path.GetFileNameWithoutExtension(fileName);

                    // 新しいファイル名を作る
                    string newFileName = Path.Combine(filePath, fileBaseName + newExtension);

                    // ファイル名を変更する
                    File.Move(fileName, newFileName);
                }
                else
                {
                    // ファイルが存在しない場合はエラーメッセージを表示する
                    Console.WriteLine("ファイルが見つかりませんでした。");
                }
            }
            catch { }
            
        }

        public static void RegisterStartupApp(string appName, string appPath)
        {
            // レジストリのキーを開く
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // アプリケーション名と実行可能パスを値として設定する
            rkApp.SetValue(appName, appPath);
        }

        // アプリケーション名を引数にとり、レジストリからスタートアップアプリを解除する関数
        public static void UnregisterStartupApp(string appName)
        {
            // レジストリのキーを開く
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // アプリケーション名に対応する値が存在するかチェックする
            if (rkApp.GetValue(appName) != null)
            {
                // 値が存在する場合は削除する
                rkApp.DeleteValue(appName, false);
            }
        }

        //---DLL関数の定義 START---//
        //GetPrivateProfileString関数の定義
        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            uint nSize,
            string lpFileName);

        //GetPrivateProfileInt関数の定義
        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileInt(
            string lpAppName,
            string lpKeyName,
            int nDefault,
            string lpFileName);
        //WritePrivateProfileString関数の定義
        [DllImport("KERNEL32.DLL")]
        public static extern bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);

        //---DLL関数の定義 END ---//


        // DLL関数をラップしたメソッドです。
        public string GetValueString(string section, string key, string fileName)
        {
            var sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "", sb, Convert.ToUInt32(sb.Capacity), fileName);
            return sb.ToString();
        }

        public int GetValueInt(string section, string key, string fileName)
        {
            var sb = new StringBuilder(1024);
            return (int)GetPrivateProfileInt(section, key, 0, fileName);
        }

        // DLL関数をラップしたメソッドです。
        public bool SetValueString(string section, string key, string value, string fileName)
        {
            return WritePrivateProfileString(section, key, value, fileName);
        }
         
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // アプリケーションの終了
            Application.Restart();
            Environment.Exit(0);
        }


    }


}
