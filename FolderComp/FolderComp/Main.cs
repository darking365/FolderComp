using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;


namespace FolderComp
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += Bg_DoWork;
                bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
                bg.RunWorkerAsync();
            }
        }

        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result!=null)
            {
                //generate the excel
                string result = e.Result.ToString();
                string filePath = Path.GetDirectoryName(result);

                string lFolder = txt_lFolder.Text.Trim();
                string rFolder = txt_rFolder.Text.Trim();
                string outFolder = filePath;

                string fileName = $"{lFolder}_{rFolder}".Replace(":","").Replace("\\","-");
                var model = Util.DeserializeFile<bcreport>(result);
                var lst = Util.GetResult(model);
                string xls = Path.Combine(filePath, $"{fileName}.xlsx");
                if (Util.Export(lst,xls))
                {
                    txt_outFolder.Text = filePath;
                    lbl_fileName.Text = xls;
                    btn_openfile.Enabled = true;
                    //refresh the config file
                    Util.UpdateAppConfig("lfolder", lFolder);
                    Util.UpdateAppConfig("rfolder", rFolder);
                    Util.UpdateAppConfig("ofolder", outFolder);
                    MessageBox.Show("completed");
                }
            }
        }

        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            string exeFolder=Util.GetAppPath("BCompare");
            if (!exeFolder.IsValid())
            {
                MessageBox.Show("please make sure this computer has installed the software Beyond Compare.");
                return;
            }
            string lFolder = txt_lFolder.Text.Trim();
            string rFolder = txt_rFolder.Text.Trim();
            string outFolder = txt_outFolder.Text.Trim();
            if (!lFolder.IsValid())
            {
                MessageBox.Show("please confirm the left folder is correct.");
            }else if (!rFolder.IsValid())
            {
                MessageBox.Show("please confirm the right folder is correct.");
            }else if(!outFolder.IsValid())
            {
                MessageBox.Show("please confirm the output file folder is correct.");
            }
            else
            {
                var files= Directory.GetFiles(Path.Combine(Application.StartupPath,"scripts"));
                if (files.Length == 0)
                {
                    MessageBox.Show("script does not exist");
                    return;
                }
                string script = files[0];
                outFolder = Path.Combine(outFolder, DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml");
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName =Path.Combine(exeFolder,"BComp.exe"),
                    UseShellExecute=false,
                    CreateNoWindow=true,
                    Arguments= $" /silent @\"{script}\" {lFolder} {rFolder} {outFolder} "
                };

                using(Process bc=new Process() { StartInfo=psi})
                {
                    try
                    {
                        bc.Start();
                        e.Result = outFolder;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void btn_selL_Click(object sender, EventArgs e)
        {
            folder_left.ShowNewFolderButton = true;
            if (folder_left.ShowDialog() == DialogResult.OK)
            {
                txt_lFolder.Text = folder_left.SelectedPath;
            }
        }

        private void btn_selR_Click(object sender, EventArgs e)
        {
            folder_right.ShowNewFolderButton = true;
            if (folder_right.ShowDialog() == DialogResult.OK)
            {
                txt_rFolder.Text = folder_right.SelectedPath;
            }
        }

        private void btn_selO_Click(object sender, EventArgs e)
        {

            folder_output.ShowNewFolderButton = true;
            if (folder_output.ShowDialog() == DialogResult.OK)
            {
                txt_outFolder.Text = folder_output.SelectedPath;
            }
        }

        private void btn_openfile_Click(object sender, EventArgs e)
        {
            new System.Threading.Tasks.TaskFactory().StartNew(() =>
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = "notepad",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = lbl_fileName.Text
                };
                using(Process p=new Process { StartInfo = psi })
                {
                    p.Start();
                }

            });
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string lfolder = Util.GetAppConfig("lfolder");
            string rfolder = Util.GetAppConfig("rfolder");
            string ofolder = Util.GetAppConfig("ofolder");
            txt_lFolder.Text = lfolder;
            txt_rFolder.Text = rfolder;
            txt_outFolder.Text = ofolder;
        }
    }
}
