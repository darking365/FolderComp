using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using WinForm = System.Windows.Forms;
using System.Windows.Threading;
using System.Threading;

namespace FolderCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WinForm.FolderBrowserDialog folder_left = new WinForm.FolderBrowserDialog();
        WinForm.FolderBrowserDialog folder_right = new WinForm.FolderBrowserDialog();
        WinForm.FolderBrowserDialog folder_output = new WinForm.FolderBrowserDialog();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Main_Load(object sender, EventArgs e)
        {
            string lfolder = Util.GetAppConfig("lfolder");
            string rfolder = Util.GetAppConfig("rfolder");
            string ofolder = Util.GetAppConfig("ofolder");
            txt_lFolder.Text = lfolder;
            txt_rFolder.Text = rfolder;
            txt_outFolder.Text = ofolder;
            folder_left.SelectedPath = lfolder;
            folder_right.SelectedPath = rfolder;
            folder_output.SelectedPath = ofolder;
        }
        private void btn_run_Click(object sender, EventArgs e)
        {
            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += Bg_DoWork;
                bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
                if (!bg.IsBusy)
                {
                    bg.RunWorkerAsync();
                }
            }
        }

        private void btn_openfile_Click(object sender, EventArgs e)
        {

            string fileName = lbl_fileName.Text;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "Excel";
                    p.StartInfo.Arguments = fileName;
                    p.Start();
                }
                catch (Exception ex)
                {
                    ex.Message.Logger();
                }
            });
        }

        private void btn_selL_Click(object sender, EventArgs e)
        {
            folder_left.ShowNewFolderButton = true;
            folder_left.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folder_left.ShowDialog() == WinForm.DialogResult.OK)
            {
                txt_lFolder.Text = folder_left.SelectedPath;
            }
        }

        private void btn_selR_Click(object sender, EventArgs e)
        {

            folder_right.ShowNewFolderButton = true;
            folder_right.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folder_right.ShowDialog() == WinForm.DialogResult.OK)
            {
                txt_rFolder.Text = folder_right.SelectedPath;
            }
        }

        private void btn_selO_Click(object sender, EventArgs e)
        {
            folder_output.ShowNewFolderButton = true;
            folder_output.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folder_output.ShowDialog() == WinForm.DialogResult.OK)
            {
                txt_outFolder.Text = folder_output.SelectedPath;
            }
        }

        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = "completed";
            if (e.Result != null)
            {
                //generate the excel
                string result = e.Result.ToString();
                string filePath = Path.GetDirectoryName(result);
                string fName = Path.GetFileNameWithoutExtension(result);

                string lFolder = txt_lFolder.Text.Trim();
                string rFolder = txt_rFolder.Text.Trim();
                string outFolder = filePath;

                string fileName = $"{lFolder}_{rFolder}".Replace(":", "").Replace("\\", "-");
                var model = Util.DeserializeFile<bcreport>(result);
                if (model != null)
                {
                    var lst = Util.GetResult(model);
                    string xls = Path.Combine(filePath, $"{fileName}_{fName}.xlsx");
                    if (Util.Export(lst, xls))
                    {
                        txt_outFolder.Text = filePath;
                        lbl_fileName.Text = xls;
                        btn_openfile.IsEnabled = true;
                        //refresh the config file
                        Util.UpdateAppConfig("lfolder", lFolder);
                        Util.UpdateAppConfig("rfolder", rFolder);
                        Util.UpdateAppConfig("ofolder", outFolder);
                    }
                }
                else
                {
                    msg = "get model from xml file failed";
                }
            }
            else
            {
                msg = "generate xml file failed";
            }
            msg.Logger();
            MessageBox.Show(msg);
        }

        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            string exeFolder = Util.GetAppPath("BCompare");
            if (!exeFolder.IsValid())
            {
                MessageBox.Show("please make sure this computer has installed the software Beyond Compare.");
                return;
            }

            string lFolder = string.Empty;
            string rFolder = string.Empty;
            string outFolder = string.Empty;

            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {

                lFolder = txt_lFolder.Text.Trim();
                rFolder = txt_rFolder.Text.Trim();
                outFolder = txt_outFolder.Text.Trim();

            });
            if (!lFolder.IsValid())
            {
                MessageBox.Show("please confirm the left folder is correct.");
            }
            else if (!rFolder.IsValid())
            {
                MessageBox.Show("please confirm the right folder is correct.");
            }
            else if (!outFolder.IsValid())
            {
                MessageBox.Show("please confirm the output file folder is correct.");
            }
            else
            {
                var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts"));
                if (files.Length == 0)
                {
                    MessageBox.Show("script does not exist");
                    return;
                }
                string script = files[0];
                outFolder = Path.Combine(outFolder, DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml");
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(exeFolder, "BComp.exe"),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $" /silent @\"{script}\" {lFolder} {rFolder} {outFolder} "
                };

                using (Process bc = new Process() { StartInfo = psi })
                {
                    try
                    {
                        bc.Start();
                        if (CheckFileExists(outFolder))
                        {
                            e.Result = outFolder;
                        }                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public bool CheckFileExists(string filePath)
        {
            bool isExists = false;
            int counter = 0;
            do
            {
                counter++;
                if (counter == 500)
                {
                    break;
                }
                isExists = File.Exists(filePath);
                Thread.Sleep(200);
            } while (!isExists);

            return isExists;
        }
    }
}
