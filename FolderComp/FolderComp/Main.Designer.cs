namespace FolderComp
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_lFolder = new System.Windows.Forms.TextBox();
            this.txt_rFolder = new System.Windows.Forms.TextBox();
            this.txt_outFolder = new System.Windows.Forms.TextBox();
            this.btn_run = new System.Windows.Forms.Button();
            this.btn_openfile = new System.Windows.Forms.Button();
            this.diag_exeFolder = new System.Windows.Forms.OpenFileDialog();
            this.folder_left = new System.Windows.Forms.FolderBrowserDialog();
            this.folder_right = new System.Windows.Forms.FolderBrowserDialog();
            this.folder_output = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_selL = new System.Windows.Forms.Button();
            this.btn_selR = new System.Windows.Forms.Button();
            this.btn_selO = new System.Windows.Forms.Button();
            this.lbl_fileName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txt_lFolder
            // 
            resources.ApplyResources(this.txt_lFolder, "txt_lFolder");
            this.txt_lFolder.Name = "txt_lFolder";
            // 
            // txt_rFolder
            // 
            resources.ApplyResources(this.txt_rFolder, "txt_rFolder");
            this.txt_rFolder.Name = "txt_rFolder";
            // 
            // txt_outFolder
            // 
            resources.ApplyResources(this.txt_outFolder, "txt_outFolder");
            this.txt_outFolder.Name = "txt_outFolder";
            // 
            // btn_run
            // 
            resources.ApplyResources(this.btn_run, "btn_run");
            this.btn_run.Name = "btn_run";
            this.btn_run.UseVisualStyleBackColor = true;
            this.btn_run.Click += new System.EventHandler(this.btn_run_Click);
            // 
            // btn_openfile
            // 
            resources.ApplyResources(this.btn_openfile, "btn_openfile");
            this.btn_openfile.Name = "btn_openfile";
            this.btn_openfile.UseVisualStyleBackColor = true;
            this.btn_openfile.Click += new System.EventHandler(this.btn_openfile_Click);
            // 
            // diag_exeFolder
            // 
            this.diag_exeFolder.FileName = "BComp.exe";
            // 
            // folder_left
            // 
            this.folder_left.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // folder_right
            // 
            this.folder_right.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // folder_output
            // 
            this.folder_output.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // btn_selL
            // 
            resources.ApplyResources(this.btn_selL, "btn_selL");
            this.btn_selL.Name = "btn_selL";
            this.btn_selL.UseVisualStyleBackColor = true;
            this.btn_selL.Click += new System.EventHandler(this.btn_selL_Click);
            // 
            // btn_selR
            // 
            resources.ApplyResources(this.btn_selR, "btn_selR");
            this.btn_selR.Name = "btn_selR";
            this.btn_selR.UseVisualStyleBackColor = true;
            this.btn_selR.Click += new System.EventHandler(this.btn_selR_Click);
            // 
            // btn_selO
            // 
            resources.ApplyResources(this.btn_selO, "btn_selO");
            this.btn_selO.Name = "btn_selO";
            this.btn_selO.UseVisualStyleBackColor = true;
            this.btn_selO.Click += new System.EventHandler(this.btn_selO_Click);
            // 
            // lbl_fileName
            // 
            resources.ApplyResources(this.lbl_fileName, "lbl_fileName");
            this.lbl_fileName.Name = "lbl_fileName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Main
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_fileName);
            this.Controls.Add(this.btn_selO);
            this.Controls.Add(this.btn_selR);
            this.Controls.Add(this.btn_selL);
            this.Controls.Add(this.btn_openfile);
            this.Controls.Add(this.btn_run);
            this.Controls.Add(this.txt_outFolder);
            this.Controls.Add(this.txt_rFolder);
            this.Controls.Add(this.txt_lFolder);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_lFolder;
        private System.Windows.Forms.TextBox txt_rFolder;
        private System.Windows.Forms.TextBox txt_outFolder;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Button btn_openfile;
        private System.Windows.Forms.OpenFileDialog diag_exeFolder;
        private System.Windows.Forms.FolderBrowserDialog folder_left;
        private System.Windows.Forms.FolderBrowserDialog folder_right;
        private System.Windows.Forms.FolderBrowserDialog folder_output;
        private System.Windows.Forms.Button btn_selL;
        private System.Windows.Forms.Button btn_selR;
        private System.Windows.Forms.Button btn_selO;
        private System.Windows.Forms.Label lbl_fileName;
        private System.Windows.Forms.Label label1;
    }
}

