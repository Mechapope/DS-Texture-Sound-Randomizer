using System;
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
        string[] gameFileDirectories = { "chr", "font", "map", "menu", "other", "parts", "sfx", "sound" };
        string[] validImageExtensions = { ".png", ".dds", ".jpg", ".tga" };
        string[] validSoundExtensions = { ".mp3", ".wav" };
        string[] uiTextureFiles = { "DSFont24_0000.dds", "DSFont24_0001.dds", "TalkFont24_0000.dds", "TalkFont24_0001.dds" };
        private Thread[] threads;
        int seed = 0;

        bool randomizeTextures, onlyCustomTextures, randomizeUiTextures, randomizeSounds, onlyCustomSounds;

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

            if (!ValidateInput())
            {
                return;
            }

            if (!File.Exists(gameDirectory + @"\TextSoundRando\Backup\sfx\FRPG_SfxBnd_m18_01.ffxbnd"))
            {
                CreateBackups();
            }

            //if last unpacked texture exists, ill just assume its all unpacked
            if (!File.Exists(gameDirectory + @"\TextSoundRando\Unpack\Textures\sfx\FRPG_SfxBnd_m18_01\s13510.dds"))
            {
                UnpackTextures();
            }

            //if last unpacked sound exists, ill just assume its all unpacked
            if (!File.Exists(gameDirectory + @"\TextSoundRando\Unpack\Sounds\frpg_xm18.fsb\y1800.wav.mp3"))
            {
                UnpackSounds();
            }

            RandomizeTextures();
            RandomizeSounds();

            RepackTextures();
            RepackSounds();
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

            seed = txtSeed.Text.GetHashCode();
            
            randomizeTextures = chkRandomizeTextures.Checked;
            onlyCustomTextures = chkOnlyCustomTextures.Checked;
            randomizeUiTextures = chkRandomizeUiTextures.Checked;
            randomizeSounds = chkRandomizeSounds.Checked;
            onlyCustomSounds = chkOnlyCustomSounds.Checked;

            return true;
        }

        private void CreateBackups()
        {            
            string backupDirectory = gameDirectory + @"\TextSoundRando\backup\";

            for (int i = 0; i < gameFileDirectories.Length; i++)
            {
                string sourceDirectory = gameDirectory + "\\" + gameFileDirectories[i];
                string destinationDirectory = backupDirectory + gameFileDirectories[i];

                if (!Directory.Exists(gameDirectory + gameFileDirectories[i]))
                {
                    Directory.CreateDirectory(backupDirectory + gameFileDirectories[i]);

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
            string[] directoriesToUnpack = new string[gameFileDirectories.Length];
            for (int i = 0; i < gameFileDirectories.Length; i++)
            {
                directoriesToUnpack[i] = gameDirectory + "\\" + gameFileDirectories[i];
            }

            var t = new MPUP();
            t.Unpack(directoriesToUnpack, 4, gameDirectory, gameDirectory + @"\TextSoundRando\Unpack\Textures");
        }

        private void RandomizeTextures()
        {
            string inputFolder = gameDirectory + @"\TextSoundRando\Unpack\Textures";
            List<string> pathsToSwap = GetTexturesToSwap();
            Random r = new Random(seed);

            foreach (string directory in Directory.EnumerateDirectories(inputFolder, "*", SearchOption.AllDirectories))
            {
                string tempDirectory = Path.Combine(gameDirectory + @"\TextSoundRando\Temp\Textures\", directory.Replace(gameDirectory + @"\TextSoundRando\Unpack\Textures\", ""));
                Directory.CreateDirectory(tempDirectory);

                foreach (string file in Directory.GetFiles(directory))
                {
                    if(CheckIsValidTextureForSwapping(file))
                    {
                        int rando = r.Next(pathsToSwap.Count);
                        File.Copy(pathsToSwap[rando], tempDirectory + "\\" + Path.GetFileName(file));
                        pathsToSwap.RemoveAt(rando);
                    }
                    else
                    {
                        //either not a valid image file or a normal/specular map - copy it to temp folder as is
                        File.Copy(file, tempDirectory + "\\" + Path.GetFileName(file));
                    }
                }
            }
        }

        private bool CheckIsValidTextureForSwapping(string fileName)
        {
            if(!File.Exists(fileName) || !validImageExtensions.Contains(Path.GetExtension(fileName)))
            {
                return false;
            }

            //normal/specular maps bad
            if (Path.GetFileNameWithoutExtension(fileName).EndsWith("_n") && Path.GetFileNameWithoutExtension(fileName).EndsWith("_s"))
            {                
                return false;
            }

            //this file is fucked, thanks from
            if(Path.GetFileName(fileName) == "DBGTEX_DETAIL.dds")
            {
                return false;
            }

            return true;
        }

        private List<string> GetTexturesToSwap()
        {
            string[] textDirs = { "\\TextSoundRando\\Custom Textures", "\\TextSoundRando\\Unpack\\Textures" };
            List<string> woof = new List<string>();

            for (int i = 0; i < textDirs.Length; i++)
            {
                foreach (string directory in Directory.EnumerateDirectories(gameDirectory + textDirs[i], "*", SearchOption.AllDirectories))
                {
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        //check this is a valid image file and not a normal/specular map
                        if (CheckIsValidTextureForSwapping(file))
                        {
                            woof.Add(file);
                        }
                    }
                }
            }

            return woof;
        }

        private void RepackTextures()
        {
            string[] directoriesToRepack = new string[gameFileDirectories.Length];
            for (int i = 0; i < gameFileDirectories.Length; i++)
            {
                directoriesToRepack[i] = gameDirectory + "\\" + gameFileDirectories[i];
            }

            var t = new MPUP();
            t.Repack(directoriesToRepack, 4, gameDirectory, gameDirectory + @"\TextSoundRando\Temp\Textures");
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

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = gameDirectory + @"\TextSoundRando\Binaries\fsbext";
                startInfo.Arguments = $"/C fsbext -m -d \"{unpackDirectory}\" -s \"{unpackDirectory + "\\files.dat"}\" \"{filepath}\"";

                //run fsbext on each fsb file
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
        }

        private void RandomizeSounds()
        {
            string inputFolder = gameDirectory + @"\TextSoundRando\Unpack\Sounds";
            List<string> pathsToSwap = GetSoundsToSwap();
            Random r = new Random(seed);

            foreach (string directory in Directory.EnumerateDirectories(inputFolder, "*", SearchOption.AllDirectories))
            {
                string tempDirectory = Path.Combine(gameDirectory + @"\TextSoundRando\Temp\Sounds\", Path.GetFileName(directory));
                Directory.CreateDirectory(tempDirectory);

                foreach (string file in Directory.GetFiles(directory))
                {
                    if (CheckIsValidSoundForSwapping(file))
                    {
                        int rando = r.Next(pathsToSwap.Count);
                        File.Copy(pathsToSwap[rando], tempDirectory + "\\" + Path.GetFileName(file));
                        pathsToSwap.RemoveAt(rando);
                    }
                    else
                    {
                        //not a sound file - copy it to directory as is
                        File.Copy(file, tempDirectory + "\\" + Path.GetFileName(file));
                    }
                }
            }
        }

        private List<string> GetSoundsToSwap()
        {
            string[] soundDirs = { "\\TextSoundRando\\Custom Sounds", "\\TextSoundRando\\Unpack\\Sounds" };
            List<string> woof = new List<string>();

            for (int i = 0; i < soundDirs.Length; i++)
            {
                foreach (string directory in Directory.EnumerateDirectories(gameDirectory + soundDirs[i], "*", SearchOption.AllDirectories))
                {
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        //check this is a valid image file and not a normal/specular map
                        if (CheckIsValidSoundForSwapping(file))
                        {
                            woof.Add(file);
                        }
                    }
                }
            }

            return woof;
        }

        private bool CheckIsValidSoundForSwapping(string fileName)
        {
            if (!File.Exists(fileName) || !validSoundExtensions.Contains(Path.GetExtension(fileName)))
            {
                return false;
            }

            //normal/specular maps bad
            if (Path.GetFileName(fileName).Contains("blank"))
            {
                return false;
            }

            return true;
        }

        private void RepackSounds()
        {
            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();

            foreach (string filepath in Directory.EnumerateDirectories(gameDirectory + "\\TextSoundRando\\Temp\\Sounds"))
            {
                filepaths.Enqueue(Path.GetFileName(filepath));
            }

            for (int i = 0; i < threads.Length; i++)
            {
                Thread thread = new Thread(() => RepackFSBs(filepaths));
                threads[i] = thread;
                thread.Start();
            }

            foreach (Thread thread in threads)
                thread.Join();
        }

        private void RepackFSBs(ConcurrentQueue<string> filepaths)
        {
            //TODO check if fsbext actually sounds correct
            string filepath;
            while (filepaths.TryDequeue(out filepath))
            {
                string unpackDirectory = gameDirectory + @"\TextSoundRando\Temp\Sounds\" + Path.GetFileName(filepath);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = gameDirectory + @"\TextSoundRando\Binaries\fsbext";
                startInfo.Arguments = $"/C fsbext -m -d \"{unpackDirectory}\" -s \"{unpackDirectory + "\\files.dat"}\" -r \"{unpackDirectory + "\\" + filepath}\"";

                //run fsbext on each fsb file
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
        }
    }
}
