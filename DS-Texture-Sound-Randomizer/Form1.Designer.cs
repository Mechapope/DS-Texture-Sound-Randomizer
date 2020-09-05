namespace DS_Texture_Sound_Randomizer
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnGamePathSelect = new System.Windows.Forms.Button();
            this.txtGamePath = new System.Windows.Forms.TextBox();
            this.lblGamePath = new System.Windows.Forms.Label();
            this.lblSeed = new System.Windows.Forms.Label();
            this.txtSeed = new System.Windows.Forms.TextBox();
            this.chkRandomizeTextures = new System.Windows.Forms.CheckBox();
            this.chkFixMainSoundFile = new System.Windows.Forms.CheckBox();
            this.chkOnlyCustomTextures = new System.Windows.Forms.CheckBox();
            this.chkRandomizeUiTextures = new System.Windows.Forms.CheckBox();
            this.chkRandomizeSounds = new System.Windows.Forms.CheckBox();
            this.chkOnlyCustomSounds = new System.Windows.Forms.CheckBox();
            this.btnOpenCustomTextureFolder = new System.Windows.Forms.Button();
            this.btnOpenCustomSoundFolder = new System.Windows.Forms.Button();
            this.numThreads = new System.Windows.Forms.NumericUpDown();
            this.lblThreads = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCreateBackups = new System.Windows.Forms.Button();
            this.btnRestoreBackups = new System.Windows.Forms.Button();
            this.btnExtractFiles = new System.Windows.Forms.Button();
            this.rtfLog = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(12, 251);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(464, 30);
            this.btnSubmit.TabIndex = 34;
            this.btnSubmit.Text = "Go";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnGamePathSelect
            // 
            this.btnGamePathSelect.Location = new System.Drawing.Point(447, 12);
            this.btnGamePathSelect.Name = "btnGamePathSelect";
            this.btnGamePathSelect.Size = new System.Drawing.Size(29, 21);
            this.btnGamePathSelect.TabIndex = 33;
            this.btnGamePathSelect.Text = "...";
            this.btnGamePathSelect.UseVisualStyleBackColor = true;
            this.btnGamePathSelect.Click += new System.EventHandler(this.btnGamePathSelect_Click);
            // 
            // txtGamePath
            // 
            this.txtGamePath.Location = new System.Drawing.Point(113, 13);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.Size = new System.Drawing.Size(328, 20);
            this.txtGamePath.TabIndex = 29;
            // 
            // lblGamePath
            // 
            this.lblGamePath.AutoSize = true;
            this.lblGamePath.Location = new System.Drawing.Point(47, 16);
            this.lblGamePath.Margin = new System.Windows.Forms.Padding(0);
            this.lblGamePath.Name = "lblGamePath";
            this.lblGamePath.Size = new System.Drawing.Size(63, 13);
            this.lblGamePath.TabIndex = 32;
            this.lblGamePath.Text = "Game Path:";
            // 
            // lblSeed
            // 
            this.lblSeed.AutoSize = true;
            this.lblSeed.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblSeed.Location = new System.Drawing.Point(27, 42);
            this.lblSeed.Name = "lblSeed";
            this.lblSeed.Size = new System.Drawing.Size(83, 13);
            this.lblSeed.TabIndex = 30;
            this.lblSeed.Text = "Seed (Optional):";
            // 
            // txtSeed
            // 
            this.txtSeed.Location = new System.Drawing.Point(114, 39);
            this.txtSeed.Name = "txtSeed";
            this.txtSeed.Size = new System.Drawing.Size(362, 20);
            this.txtSeed.TabIndex = 31;
            // 
            // chkRandomizeTextures
            // 
            this.chkRandomizeTextures.AutoSize = true;
            this.chkRandomizeTextures.Location = new System.Drawing.Point(19, 19);
            this.chkRandomizeTextures.Name = "chkRandomizeTextures";
            this.chkRandomizeTextures.Size = new System.Drawing.Size(119, 17);
            this.chkRandomizeTextures.TabIndex = 47;
            this.chkRandomizeTextures.Text = "Randomize textures";
            this.chkRandomizeTextures.UseVisualStyleBackColor = true;
            this.chkRandomizeTextures.CheckedChanged += new System.EventHandler(this.chkRandomizeTextures_CheckedChanged);
            // 
            // chkFixMainSoundFile
            // 
            this.chkFixMainSoundFile.AutoSize = true;
            this.chkFixMainSoundFile.Location = new System.Drawing.Point(334, 19);
            this.chkFixMainSoundFile.Name = "chkFixMainSoundFile";
            this.chkFixMainSoundFile.Size = new System.Drawing.Size(112, 17);
            this.chkFixMainSoundFile.TabIndex = 48;
            this.chkFixMainSoundFile.Text = "Fix main sound file";
            this.chkFixMainSoundFile.UseVisualStyleBackColor = true;
            this.chkFixMainSoundFile.CheckedChanged += new System.EventHandler(this.chkFixMainSoundFile_CheckedChanged);
            // 
            // chkOnlyCustomTextures
            // 
            this.chkOnlyCustomTextures.AutoSize = true;
            this.chkOnlyCustomTextures.Location = new System.Drawing.Point(33, 42);
            this.chkOnlyCustomTextures.Name = "chkOnlyCustomTextures";
            this.chkOnlyCustomTextures.Size = new System.Drawing.Size(144, 17);
            this.chkOnlyCustomTextures.TabIndex = 49;
            this.chkOnlyCustomTextures.Text = "Only use custom textures";
            this.chkOnlyCustomTextures.UseVisualStyleBackColor = true;
            // 
            // chkRandomizeUiTextures
            // 
            this.chkRandomizeUiTextures.AutoSize = true;
            this.chkRandomizeUiTextures.Location = new System.Drawing.Point(33, 65);
            this.chkRandomizeUiTextures.Name = "chkRandomizeUiTextures";
            this.chkRandomizeUiTextures.Size = new System.Drawing.Size(133, 17);
            this.chkRandomizeUiTextures.TabIndex = 50;
            this.chkRandomizeUiTextures.Text = "Randomize UI textures";
            this.chkRandomizeUiTextures.UseVisualStyleBackColor = true;
            // 
            // chkRandomizeSounds
            // 
            this.chkRandomizeSounds.AutoSize = true;
            this.chkRandomizeSounds.Location = new System.Drawing.Point(178, 19);
            this.chkRandomizeSounds.Name = "chkRandomizeSounds";
            this.chkRandomizeSounds.Size = new System.Drawing.Size(116, 17);
            this.chkRandomizeSounds.TabIndex = 51;
            this.chkRandomizeSounds.Text = "Randomize sounds";
            this.chkRandomizeSounds.UseVisualStyleBackColor = true;
            this.chkRandomizeSounds.CheckedChanged += new System.EventHandler(this.chkRandomizeSounds_CheckedChanged);
            // 
            // chkOnlyCustomSounds
            // 
            this.chkOnlyCustomSounds.AutoSize = true;
            this.chkOnlyCustomSounds.Location = new System.Drawing.Point(192, 42);
            this.chkOnlyCustomSounds.Name = "chkOnlyCustomSounds";
            this.chkOnlyCustomSounds.Size = new System.Drawing.Size(141, 17);
            this.chkOnlyCustomSounds.TabIndex = 52;
            this.chkOnlyCustomSounds.Text = "Only use custom sounds";
            this.chkOnlyCustomSounds.UseVisualStyleBackColor = true;
            // 
            // btnOpenCustomTextureFolder
            // 
            this.btnOpenCustomTextureFolder.Location = new System.Drawing.Point(297, 198);
            this.btnOpenCustomTextureFolder.Name = "btnOpenCustomTextureFolder";
            this.btnOpenCustomTextureFolder.Size = new System.Drawing.Size(85, 46);
            this.btnOpenCustomTextureFolder.TabIndex = 53;
            this.btnOpenCustomTextureFolder.Text = "Open Custom Texture Folder";
            this.btnOpenCustomTextureFolder.UseVisualStyleBackColor = true;
            this.btnOpenCustomTextureFolder.Click += new System.EventHandler(this.btnOpenCustomTextureFolder_Click);
            // 
            // btnOpenCustomSoundFolder
            // 
            this.btnOpenCustomSoundFolder.Location = new System.Drawing.Point(392, 199);
            this.btnOpenCustomSoundFolder.Name = "btnOpenCustomSoundFolder";
            this.btnOpenCustomSoundFolder.Size = new System.Drawing.Size(85, 46);
            this.btnOpenCustomSoundFolder.TabIndex = 54;
            this.btnOpenCustomSoundFolder.Text = "Open Custom Sound Folder";
            this.btnOpenCustomSoundFolder.UseVisualStyleBackColor = true;
            this.btnOpenCustomSoundFolder.Click += new System.EventHandler(this.btnOpenCustomSoundFolder_Click);
            // 
            // numThreads
            // 
            this.numThreads.Location = new System.Drawing.Point(114, 69);
            this.numThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreads.Name = "numThreads";
            this.numThreads.Size = new System.Drawing.Size(362, 20);
            this.numThreads.TabIndex = 55;
            this.numThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblThreads
            // 
            this.lblThreads.AutoSize = true;
            this.lblThreads.Location = new System.Drawing.Point(9, 71);
            this.lblThreads.Name = "lblThreads";
            this.lblThreads.Size = new System.Drawing.Size(101, 13);
            this.lblThreads.TabIndex = 56;
            this.lblThreads.Text = "Number of Threads:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkRandomizeTextures);
            this.groupBox1.Controls.Add(this.chkFixMainSoundFile);
            this.groupBox1.Controls.Add(this.chkOnlyCustomTextures);
            this.groupBox1.Controls.Add(this.chkRandomizeUiTextures);
            this.groupBox1.Controls.Add(this.chkRandomizeSounds);
            this.groupBox1.Controls.Add(this.chkOnlyCustomSounds);
            this.groupBox1.Location = new System.Drawing.Point(12, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 93);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // btnCreateBackups
            // 
            this.btnCreateBackups.Location = new System.Drawing.Point(12, 199);
            this.btnCreateBackups.Name = "btnCreateBackups";
            this.btnCreateBackups.Size = new System.Drawing.Size(85, 46);
            this.btnCreateBackups.TabIndex = 58;
            this.btnCreateBackups.Text = "Create Backups";
            this.btnCreateBackups.UseVisualStyleBackColor = true;
            this.btnCreateBackups.Click += new System.EventHandler(this.btnCreateBackups_Click);
            // 
            // btnRestoreBackups
            // 
            this.btnRestoreBackups.Location = new System.Drawing.Point(107, 199);
            this.btnRestoreBackups.Name = "btnRestoreBackups";
            this.btnRestoreBackups.Size = new System.Drawing.Size(85, 46);
            this.btnRestoreBackups.TabIndex = 59;
            this.btnRestoreBackups.Text = "Restore Backups";
            this.btnRestoreBackups.UseVisualStyleBackColor = true;
            this.btnRestoreBackups.Click += new System.EventHandler(this.btnRestoreBackups_Click);
            // 
            // btnExtractFiles
            // 
            this.btnExtractFiles.Location = new System.Drawing.Point(202, 199);
            this.btnExtractFiles.Name = "btnExtractFiles";
            this.btnExtractFiles.Size = new System.Drawing.Size(85, 46);
            this.btnExtractFiles.TabIndex = 60;
            this.btnExtractFiles.Text = "Extract Game Files";
            this.btnExtractFiles.UseVisualStyleBackColor = true;
            this.btnExtractFiles.Click += new System.EventHandler(this.btnExtractFiles_Click);
            // 
            // rtfLog
            // 
            this.rtfLog.Location = new System.Drawing.Point(12, 287);
            this.rtfLog.Name = "rtfLog";
            this.rtfLog.ReadOnly = true;
            this.rtfLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtfLog.Size = new System.Drawing.Size(464, 204);
            this.rtfLog.TabIndex = 61;
            this.rtfLog.Text = "";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 500);
            this.Controls.Add(this.rtfLog);
            this.Controls.Add(this.btnExtractFiles);
            this.Controls.Add(this.btnRestoreBackups);
            this.Controls.Add(this.btnCreateBackups);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblThreads);
            this.Controls.Add(this.numThreads);
            this.Controls.Add(this.btnOpenCustomSoundFolder);
            this.Controls.Add(this.btnOpenCustomTextureFolder);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnGamePathSelect);
            this.Controls.Add(this.txtGamePath);
            this.Controls.Add(this.lblGamePath);
            this.Controls.Add(this.lblSeed);
            this.Controls.Add(this.txtSeed);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "DS1 Texture and Sound Randomizer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnGamePathSelect;
        private System.Windows.Forms.TextBox txtGamePath;
        private System.Windows.Forms.Label lblGamePath;
        private System.Windows.Forms.Label lblSeed;
        private System.Windows.Forms.TextBox txtSeed;
        private System.Windows.Forms.CheckBox chkRandomizeTextures;
        private System.Windows.Forms.CheckBox chkFixMainSoundFile;
        private System.Windows.Forms.CheckBox chkOnlyCustomTextures;
        private System.Windows.Forms.CheckBox chkRandomizeUiTextures;
        private System.Windows.Forms.CheckBox chkRandomizeSounds;
        private System.Windows.Forms.CheckBox chkOnlyCustomSounds;
        private System.Windows.Forms.Button btnOpenCustomTextureFolder;
        private System.Windows.Forms.Button btnOpenCustomSoundFolder;
        private System.Windows.Forms.NumericUpDown numThreads;
        private System.Windows.Forms.Label lblThreads;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCreateBackups;
        private System.Windows.Forms.Button btnRestoreBackups;
        private System.Windows.Forms.Button btnExtractFiles;
        private System.Windows.Forms.RichTextBox rtfLog;
        private System.Windows.Forms.Timer timer1;
    }
}

