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

namespace ext_chg_detect
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.directory == "")
            {
                DialogResult result = MessageBox.Show("検知するフォルダーを設定しますか？", "未登録", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    BrowseFolder();
                }
                else if (result == System.Windows.Forms.DialogResult.No)
                {
                    this.Close();
                }
                
                
            }

            label1.Text = Properties.Settings.Default.directory;

            //監視対象のフォルダの設定
            fsw.Path = Properties.Settings.Default.directory;
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
            fsw.Created += fsw_Created;
            fsw.Changed += fsw_Changed;
            fsw.Deleted += fsw_Deleted;
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
                    Properties.Settings.Default.directory = Path.GetDirectoryName(ofd.FileName);
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

            //拡張子が変わっている場合のみ処理を行う
            if (Old_ext != New_ext)
            {
                if (New_ext == ".png")
                {
                    string fileName = e.FullPath;
                    string newFileName = System.IO.Path.ChangeExtension(fileName, Old_ext);
                    System.IO.File.Move(fileName, newFileName);

                    changeFormatOfPicture(e.OldFullPath, e.FullPath);
                    File.Delete(e.OldFullPath);
                }
                else if (New_ext == ".jpg")
                {
                    string fileName = e.FullPath;
                    string newFileName = System.IO.Path.ChangeExtension(fileName, Old_ext);
                    System.IO.File.Move(fileName, newFileName);

                    changeFormatOfPicture(e.OldFullPath, e.FullPath);
                    File.Delete(e.OldFullPath);
                }

            }

            //イベントの発生を再開
            fsw.EnableRaisingEvents = true;
        }

        void fsw_Deleted(object sender, FileSystemEventArgs e)
        {
        }

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
        }

        void fsw_Created(object sender, FileSystemEventArgs e)
        {
        }


        void changeFormatOfPicture(string fromFilePath, string toFilePath)
        {
            Image orgImage = Image.FromFile(fromFilePath);
            orgImage.Save(toFilePath);

            orgImage.Dispose();
            MessageBox.Show("拡張子を変更しました。");
        }
    }
}