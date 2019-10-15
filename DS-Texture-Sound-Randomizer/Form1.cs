﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

namespace DS_Texture_Sound_Randomizer
{
    public partial class Form1 : Form
    {
        public string gameDirectory = "";
        string[] textureDirectories = { "chr", "font", "map", "menu", "other", "parts", "sfx" };
        private Thread[] threads;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gameDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (File.Exists(gameDirectory + "\\DARKSOULS.exe"))
            {
                //exe is in a valid game directory, just use this as the path instead of asking for input
                txtGamePath.Text = gameDirectory;

                if (!File.Exists(gameDirectory + "\\param\\GameParam\\GameParam.parambnd"))
                {
                    //user hasn't unpacked their game
                    lblMessage.Text = "You don't seem to have an unpacked Dark Souls installation. Please run UDSFM and come back :)";
                    lblMessage.Visible = true;
                    lblMessage.ForeColor = Color.Red;
                }
            }
        }
        
        private void btnGamePathSelect_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtGamePath.Text = dialog.FileName;
                gameDirectory = dialog.FileName;

                lblMessage.Text = "";
                lblMessage.Visible = true;

                if (!File.Exists(gameDirectory + "\\DARKSOULS.exe"))
                {
                    lblMessage.Text = "Not a valid Data directory!";
                    lblMessage.ForeColor = Color.Red;
                }
                else if (!File.Exists(gameDirectory + "\\param\\GameParam\\GameParam.parambnd"))
                {
                    //user hasn't unpacked their game
                    lblMessage.Text = "You don't seem to have an unpacked Dark Souls installation. Please run UDSFM and come back :)";
                    lblMessage.ForeColor = Color.Red;
                }
            }
        }


        private void chkRandomizeTextures_CheckedChanged(object sender, EventArgs e)
        {
            if(!chkRandomizeTextures.Checked)
            {
                chkOnlyCustomTextures.Checked = false;
                chkRandomizeUiTextures.Checked = false;
                chkFixMainSoundFile.Checked = false;
            }

            chkOnlyCustomTextures.Enabled = chkRandomizeTextures.Checked;
            chkRandomizeUiTextures.Enabled = chkRandomizeTextures.Checked;
            chkFixMainSoundFile.Enabled = !chkRandomizeTextures.Checked && !chkRandomizeSounds.Checked;
        }

        private void chkRandomizeSounds_CheckedChanged(object sender, EventArgs e)
        {
            if(!chkRandomizeSounds.Checked)
            {
                chkOnlyCustomSounds.Checked = false;
                chkFixMainSoundFile.Checked = false;
            }

            chkOnlyCustomSounds.Enabled = chkRandomizeSounds.Checked;
            chkFixMainSoundFile.Enabled = !chkRandomizeSounds.Checked && !chkRandomizeTextures.Checked;
        }

        private void chkFixMainSoundFile_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFixMainSoundFile.Checked)
            {
                chkRandomizeTextures.Checked = false;
                chkOnlyCustomTextures.Checked = false;
                chkRandomizeUiTextures.Checked = false;
                chkRandomizeSounds.Checked = false;
                chkOnlyCustomSounds.Checked = false;
            }

            chkRandomizeTextures.Enabled = !chkFixMainSoundFile.Checked;
            chkRandomizeSounds.Enabled = !chkFixMainSoundFile.Checked;
        }

        private void btnOpenCustomTextureFolder_Click(object sender, EventArgs e)
        {
            if(txtGamePath.Text == "")
            {
                lblMessage.Text = "Please set the game path first";
                lblMessage.Visible = true;
                lblMessage.ForeColor = Color.Red;
            }
            else
            {
                if (!Directory.Exists(gameDirectory + @"\TextSoundRando\Custom Textures"))
                {
                    Directory.CreateDirectory((gameDirectory + @"\TextSoundRando\Custom Textures"));
                }

                Process.Start(gameDirectory + @"\TextSoundRando\Custom Textures");
            }
        }

        private void btnOpenCustomSoundFolder_Click(object sender, EventArgs e)
        {
            if (txtGamePath.Text == "")
            {
                lblMessage.Text = "Please set the game path first";
                lblMessage.Visible = true;
                lblMessage.ForeColor = Color.Red;
            }
            else
            {
                if(!Directory.Exists(gameDirectory + @"\TextSoundRando\Custom Sounds"))
                {
                    Directory.CreateDirectory((gameDirectory + @"\TextSoundRando\Custom Sounds"));
                }

                Process.Start(gameDirectory + @"\TextSoundRando\Custom Sounds");
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            threads = new Thread[4];

            if(!ValidateInput())
            {
                return;
            }

            if(!File.Exists(gameDirectory + @"\TextSoundRando\Backup\sfx\FRPG_SfxBnd_m18_01.ffxbnd"))
            {
                CreateBackups();
            }

            //if last unpacked texture exists, ill just assume its all unpacked
            if (!File.Exists(gameDirectory + @"\TextSoundRando\Unpack\Textures\sfx\FRPG_SfxBnd_m18_01\s13510.dds"))
            {
                UnpackTextures();
            }

            //if last unpacked sound exists, ill just assume its all unpacked
            if (!File.Exists(gameDirectory + @"\TextSoundRando\Unpack\Sounds\frpg_xm18\y1800.wav.mp3"))
            {
                UnpackSounds();
            }

            //RandomizeTextures();
            //RandomizeSounds();

            //RepackTextures();
            //RepackSounds();


        }

        private bool ValidateInput()
        {
            //check that entered path is valid
            gameDirectory = txtGamePath.Text;

            //reset message label
            lblMessage.Text = "";
            lblMessage.ForeColor = new Color();
            lblMessage.Visible = true;

            if (!File.Exists(gameDirectory + "\\DARKSOULS.exe"))
            {
                lblMessage.Text = "Not a valid Data directory!";
                lblMessage.ForeColor = Color.Red;
                return false;
            }
            else if (!File.Exists(gameDirectory + "\\param\\GameParam\\GameParam.parambnd"))
            {
                //user hasn't unpacked their game
                lblMessage.Text = "You don't seem to have an unpacked Dark Souls installation. Please run UDSFM and come back :)";
                lblMessage.ForeColor = Color.Red;
                return false;
            }

            //generate a seed if needed
            if (txtSeed.Text == "")
            {
                string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random seedGen = new Random();
                for (int i = 0; i < 15; i++)
                {
                    txtSeed.Text += validChars[seedGen.Next(validChars.Length)];
                }
            }

            return true;
        }

        private void CreateBackups()
        {            
            string backupDirectory = gameDirectory + @"\TextSoundRando\backup\";

            for (int i = 0; i < textureDirectories.Length; i++)
            {
                //from https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
                string sourceDirectory = gameDirectory + @"\" + textureDirectories[i];
                string destinationDirectory = backupDirectory + textureDirectories[i];

                if (!Directory.Exists(gameDirectory + textureDirectories[i]))
                {
                    Directory.CreateDirectory(backupDirectory + textureDirectories[i]);

                    foreach (string dir in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(Path.Combine(destinationDirectory, dir.Substring(sourceDirectory.Length + 1)));
                    }

                    foreach (string file_name in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
                    {
                        File.Copy(file_name, Path.Combine(destinationDirectory, file_name.Substring(sourceDirectory.Length + 1)));
                    }
                }
            }            
        }

        private void UnpackTextures()
        {
            //TODO only search specified game directories
            var t = new TPUP(gameDirectory, gameDirectory + @"\TextSoundRando\Unpack\Textures", false, null, 4);
            t.Start();
        }

        private void UnpackSounds()
        {
            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();

            foreach (string filepath in Directory.EnumerateFiles(gameDirectory + "\\sound", "*.fsb"))
            {
                filepaths.Enqueue(filepath);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                Thread thread = new Thread(() => UnpackFSBs(filepaths));
                threads[i] = thread;
                thread.Start();
            }

            foreach (Thread thread in threads)
                thread.Join();
        }

        private void UnpackFSBs(ConcurrentQueue<string> filepaths)
        {
            string filepath;
            while (filepaths.TryDequeue(out filepath))
            {
                string unpackDirectory = gameDirectory + @"\TextSoundRando\Unpack\Sounds\" + Path.GetFileName(filepath);

                Directory.CreateDirectory(unpackDirectory);
                //copy the .fsb file too
                File.Copy(filepath, unpackDirectory + "\\" + Path.GetFileName(filepath));               

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = gameDirectory + @"\TextSoundRando\Binaries\fsbext";
                startInfo.Arguments = $"/C fsbext -d \"{unpackDirectory}\" \"{filepath}\"";

                //run fsbext on each fsb file
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
        }

        private void RepackTextures()
        {
            Dictionary<string, string> dammitk = new Dictionary<string, string>();
            dammitk.Add(@"\chr\c1200\c1200_rat", @"\chr\c1200\c1200_rat");

            var t = new TPUP(gameDirectory, gameDirectory + @"\TextSoundRando\Unpack\Textures", true, null, 1);
            t.Start();
        }

        private void RepackSounds()
        {

        }
    }
}
