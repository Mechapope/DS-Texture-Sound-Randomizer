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
            this.lblMessage = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(13, 306);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(428, 43);
            this.lblMessage.TabIndex = 35;
            this.lblMessage.Text = "lblMessage";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMessage.Visible = false;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(190, 280);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 34;
            this.btnSubmit.Text = "Go";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnGamePathSelect
            // 
            this.btnGamePathSelect.Location = new System.Drawing.Point(412, 12);
            this.btnGamePathSelect.Name = "btnGamePathSelect";
            this.btnGamePathSelect.Size = new System.Drawing.Size(28, 20);
            this.btnGamePathSelect.TabIndex = 33;
            this.btnGamePathSelect.Text = "...";
            this.btnGamePathSelect.UseVisualStyleBackColor = true;
            this.btnGamePathSelect.Click += new System.EventHandler(this.btnGamePathSelect_Click);
            // 
            // txtGamePath
            // 
            this.txtGamePath.Location = new System.Drawing.Point(78, 12);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.Size = new System.Drawing.Size(328, 20);
            this.txtGamePath.TabIndex = 29;
            // 
            // lblGamePath
            // 
            this.lblGamePath.AutoSize = true;
            this.lblGamePath.Location = new System.Drawing.Point(9, 16);
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
            this.lblSeed.Location = new System.Drawing.Point(37, 38);
            this.lblSeed.Name = "lblSeed";
            this.lblSeed.Size = new System.Drawing.Size(35, 13);
            this.lblSeed.TabIndex = 30;
            this.lblSeed.Text = "Seed:";
            // 
            // txtSeed
            // 
            this.txtSeed.Location = new System.Drawing.Point(78, 38);
            this.txtSeed.Name = "txtSeed";
            this.txtSeed.Size = new System.Drawing.Size(362, 20);
            this.txtSeed.TabIndex = 31;
            // 
            // chkRandomizeTextures
            // 
            this.chkRandomizeTextures.AutoSize = true;
            this.chkRandomizeTextures.Location = new System.Drawing.Point(14, 176);
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
            this.chkFixMainSoundFile.Location = new System.Drawing.Point(329, 176);
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
            this.chkOnlyCustomTextures.Enabled = false;
            this.chkOnlyCustomTextures.Location = new System.Drawing.Point(29, 199);
            this.chkOnlyCustomTextures.Name = "chkOnlyCustomTextures";
            this.chkOnlyCustomTextures.Size = new System.Drawing.Size(144, 17);
            this.chkOnlyCustomTextures.TabIndex = 49;
            this.chkOnlyCustomTextures.Text = "Only use custom textures";
            this.chkOnlyCustomTextures.UseVisualStyleBackColor = true;
            // 
            // chkRandomizeUiTextures
            // 
            this.chkRandomizeUiTextures.AutoSize = true;
            this.chkRandomizeUiTextures.Enabled = false;
            this.chkRandomizeUiTextures.Location = new System.Drawing.Point(29, 222);
            this.chkRandomizeUiTextures.Name = "chkRandomizeUiTextures";
            this.chkRandomizeUiTextures.Size = new System.Drawing.Size(133, 17);
            this.chkRandomizeUiTextures.TabIndex = 50;
            this.chkRandomizeUiTextures.Text = "Randomize UI textures";
            this.chkRandomizeUiTextures.UseVisualStyleBackColor = true;
            // 
            // chkRandomizeSounds
            // 
            this.chkRandomizeSounds.AutoSize = true;
            this.chkRandomizeSounds.Location = new System.Drawing.Point(173, 176);
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
            this.chkOnlyCustomSounds.Enabled = false;
            this.chkOnlyCustomSounds.Location = new System.Drawing.Point(188, 199);
            this.chkOnlyCustomSounds.Name = "chkOnlyCustomSounds";
            this.chkOnlyCustomSounds.Size = new System.Drawing.Size(141, 17);
            this.chkOnlyCustomSounds.TabIndex = 52;
            this.chkOnlyCustomSounds.Text = "Only use custom sounds";
            this.chkOnlyCustomSounds.UseVisualStyleBackColor = true;
            // 
            // btnOpenCustomTextureFolder
            // 
            this.btnOpenCustomTextureFolder.Location = new System.Drawing.Point(105, 73);
            this.btnOpenCustomTextureFolder.Name = "btnOpenCustomTextureFolder";
            this.btnOpenCustomTextureFolder.Size = new System.Drawing.Size(111, 46);
            this.btnOpenCustomTextureFolder.TabIndex = 53;
            this.btnOpenCustomTextureFolder.Text = "Open Custom Texture Folder";
            this.btnOpenCustomTextureFolder.UseVisualStyleBackColor = true;
            this.btnOpenCustomTextureFolder.Click += new System.EventHandler(this.btnOpenCustomTextureFolder_Click);
            // 
            // btnOpenCustomSoundFolder
            // 
            this.btnOpenCustomSoundFolder.Location = new System.Drawing.Point(238, 73);
            this.btnOpenCustomSoundFolder.Name = "btnOpenCustomSoundFolder";
            this.btnOpenCustomSoundFolder.Size = new System.Drawing.Size(111, 46);
            this.btnOpenCustomSoundFolder.TabIndex = 54;
            this.btnOpenCustomSoundFolder.Text = "Open Custom Sound Folder";
            this.btnOpenCustomSoundFolder.UseVisualStyleBackColor = true;
            this.btnOpenCustomSoundFolder.Click += new System.EventHandler(this.btnOpenCustomSoundFolder_Click);
            // 
            // numThreads
            // 
            this.numThreads.Location = new System.Drawing.Point(294, 133);
            this.numThreads.Name = "numThreads";
            this.numThreads.Size = new System.Drawing.Size(38, 20);
            this.numThreads.TabIndex = 55;
            // 
            // lblThreads
            // 
            this.lblThreads.AutoSize = true;
            this.lblThreads.Location = new System.Drawing.Point(187, 135);
            this.lblThreads.Name = "lblThreads";
            this.lblThreads.Size = new System.Drawing.Size(101, 13);
            this.lblThreads.TabIndex = 56;
            this.lblThreads.Text = "Number of Threads:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 361);
            this.Controls.Add(this.lblThreads);
            this.Controls.Add(this.numThreads);
            this.Controls.Add(this.btnOpenCustomSoundFolder);
            this.Controls.Add(this.btnOpenCustomTextureFolder);
            this.Controls.Add(this.chkOnlyCustomSounds);
            this.Controls.Add(this.chkRandomizeSounds);
            this.Controls.Add(this.chkRandomizeUiTextures);
            this.Controls.Add(this.chkOnlyCustomTextures);
            this.Controls.Add(this.chkFixMainSoundFile);
            this.Controls.Add(this.chkRandomizeTextures);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnGamePathSelect);
            this.Controls.Add(this.txtGamePath);
            this.Controls.Add(this.lblGamePath);
            this.Controls.Add(this.lblSeed);
            this.Controls.Add(this.txtSeed);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
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
    }
}

