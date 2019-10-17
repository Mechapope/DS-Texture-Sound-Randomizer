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
        int seed = 0;
        int threadCount;
        bool isBackupsExist = false;
        bool isGameUnpacked = false;

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
                    LogError("You don't seem to have an unpacked Dark Souls installation. Please run UDSFM and come back :)");
                }
            }

            if (File.Exists(gameDirectory + @"\TextSoundRando\Backup\sfx\FRPG_SfxBnd_m18_01.ffxbnd"))
            {
                isBackupsExist = true;
                btnCreateBackups.Enabled = false;
                LogMessage("Backups found.");
            }

            if (File.Exists(gameDirectory + @"\TextSoundRando\Unpack\Sounds\frpg_xm18.fsb\y1800.wav.mp3"))
            {
                isGameUnpacked = true;
                btnExtractFiles.Enabled = false;
                LogMessage("Extracted game files found.");
            }

            int cores = Environment.ProcessorCount;

            if (cores > 4)
                numThreads.Value = cores - 2;
            else if (cores > 1)
                numThreads.Value = cores - 1;
            else
                numThreads.Value = 1;
        }
        
        private void btnGamePathSelect_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtGamePath.Text = dialog.FileName;
                gameDirectory = dialog.FileName;

                if (!File.Exists(gameDirectory + "\\DARKSOULS.exe"))
                {
                    LogError("Not a valid Data directory!");
                }
                else if (!File.Exists(gameDirectory + "\\param\\GameParam\\GameParam.parambnd"))
                {
                    //user hasn't unpacked their game
                    LogError("You don't seem to have an unpacked Dark Souls installation. Please run UDSFM and come back :)");
                }

                if (File.Exists(gameDirectory + @"\TextSoundRando\Backup\sfx\FRPG_SfxBnd_m18_01.ffxbnd"))
                {
                    isBackupsExist = true;
                    btnCreateBackups.Enabled = false;
                    LogMessage("Backups found.");
                }

                if (File.Exists(gameDirectory + @"\TextSoundRando\Unpack\Sounds\frpg_xm18.fsb\y1800.wav.mp3"))
                {
                    isGameUnpacked = true;
                    btnExtractFiles.Enabled = false;
                    LogMessage("Extracted game files found.");
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
                LogError("Please set your game path first.");
            }
            else if (!File.Exists(gameDirectory + "\\DARKSOULS.exe"))
            {
                LogError("Not a valid Data directory.");
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
                LogError("Please set your game path first.");
            }
            else if (!File.Exists(gameDirectory + "\\DARKSOULS.exe"))
            {
                LogError("Not a valid Data directory.");
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

        private void btnCreateBackups_Click(object sender, EventArgs e)
        {
            LogMessage("Creating backups.");

            Thread t = new Thread(CreateBackups);
            t.Start();
            t.Join();

            LogMessage("Backups created.");
            isBackupsExist = true;
            btnCreateBackups.Enabled = false;

        }

        private void btnRestoreBackups_Click(object sender, EventArgs e)
        {
            LogMessage("Restoring backups.");

            //Thread t = new Thread(CreateBackups);
            //t.Start();
            //t.Join();

            LogMessage("Backups restored.");
        }

        private void btnExtractFiles_Click(object sender, EventArgs e)
        {
            LogMessage("Extracting textures.");

            threadCount = (int)numThreads.Value;

            Thread t2 = new Thread(UnpackTextures);
            t2.Start();
            t2.Join();

            LogMessage("Textures extracted.");
            LogMessage("Extracting sounds.");

            Thread t = new Thread(UnpackSounds);
            t.Start();
            t.Join();

            LogMessage("Sounds extracted.");
            LogMessage("Extracting game files complete.");

            isGameUnpacked = true;
            btnExtractFiles.Enabled = false;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            threadCount = (int)numThreads.Value;

            ClearTempFolder();

            //TODO doin stuff on threads, writing to label, disable button on submit etc

            if (!ValidateInput())
            {
                return;
            }

            if (!isBackupsExist)
            {
                LogError("Create a backup first.");
                return;
            }

            if(!isGameUnpacked)
            {
                LogError("Unpack your game files first.");
                return;
            }

            if(randomizeTextures)
            {
                RandomizeTextures();
                RepackTextures();
            }

            if(randomizeSounds)
            { 
                RandomizeSounds();
                RepackSounds();
            }

            //TODO fix main sound file


            ClearTempFolder();
        }

        private bool ValidateInput()
        {
            //check that entered path is valid
            gameDirectory = txtGamePath.Text;

            if (!File.Exists(gameDirectory + "\\DARKSOULS.exe"))
            {
                LogError("Not a valid Data directory!");
                return false;
            }
            else if (!File.Exists(gameDirectory + "\\param\\GameParam\\GameParam.parambnd"))
            {
                //user hasn't unpacked their game
                LogError("You don't seem to have an unpacked Dark Souls installation. Please run UDSFM and come back :)");
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

        private void ClearTempFolder()
        {
            //empty temp directories and re-create empty
            if(Directory.Exists(gameDirectory + "\\TextSoundRando\\Temp\\Textures"))
            {
                Directory.Delete(gameDirectory + "\\TextSoundRando\\Temp\\Textures", true);
            }

            if (Directory.Exists(gameDirectory + "\\TextSoundRando\\Temp\\Sounds"))
            {
                Directory.Delete(gameDirectory + "\\TextSoundRando\\Temp\\Sounds", true);
            }           
            
            Directory.CreateDirectory(gameDirectory + "\\TextSoundRando\\Temp\\Sounds");
            Directory.CreateDirectory(gameDirectory + "\\TextSoundRando\\Temp\\Textures");
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

        private void RestoreBackups()
        {
            string backupDirectory = gameDirectory + @"\TextSoundRando\backup\";

            foreach (var item in Directory.EnumerateFiles(backupDirectory, "*", SearchOption.AllDirectories))
            {
                File.Copy(item, item.Replace(@"\TextSoundRando\backup\", ""), true);
            }
        }

        private void UnpackTextures()
        {
            string unpackDirectory = gameDirectory + "\\TextSoundRando\\Unpack\\Textures";
            if (!Directory.Exists(unpackDirectory))
            {
                Directory.CreateDirectory(unpackDirectory);
            }

            string[] directoriesToUnpack = new string[gameFileDirectories.Length];
            for (int i = 0; i < gameFileDirectories.Length; i++)
            {
                directoriesToUnpack[i] = gameDirectory + "\\" + gameFileDirectories[i];
            }

            var t = new MPUP();
            t.Unpack(directoriesToUnpack, threadCount, gameDirectory, unpackDirectory);
        }

        private void RandomizeTextures()
        {
            using (StreamWriter sw = new StreamWriter(gameDirectory + @"\TextSoundRando\texMaps.csv"))
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
                            File.Copy(pathsToSwap[rando], tempDirectory + "\\" + Path.GetFileName(file), true);

                            sw.WriteLine(file + "," + pathsToSwap[rando]);

                            //TODO convert to DDS file
                            //texconv BC1_UNORM -y -singleproc -pow2 cat.jpg
                            if (!onlyCustomTextures)
                            {
                                pathsToSwap.RemoveAt(rando);
                            }
                            
                        }
                        else
                        {
                            //either not a valid image file or a normal/specular map - copy it to temp folder as is
                            File.Copy(file, tempDirectory + "\\" + Path.GetFileName(file), true);
                        }
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
            if (Path.GetFileNameWithoutExtension(fileName).EndsWith("_n") || Path.GetFileNameWithoutExtension(fileName).EndsWith("_s"))
            {                
                return false;
            }

            if(!randomizeUiTextures && uiTextureFiles.Contains(Path.GetFileName(fileName)))
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
            List<string> textures = new List<string>();

            //add custom textures
            textures.AddRange(Directory.GetFiles(gameDirectory + "\\TextSoundRando\\Custom Textures").Where(t => CheckIsValidTextureForSwapping(t)));

            if(!onlyCustomTextures)
            {
                foreach (string directory in Directory.EnumerateDirectories(gameDirectory + "\\TextSoundRando\\Unpack\\Textures", "*", SearchOption.AllDirectories))
                {
                    textures.AddRange(Directory.GetFiles(directory).Where(t => CheckIsValidTextureForSwapping(t)));
                }
            }

            return textures;
        }

        private void RepackTextures()
        {
            string[] directoriesToRepack = new string[gameFileDirectories.Length];
            for (int i = 0; i < gameFileDirectories.Length; i++)
            {
                directoriesToRepack[i] = gameDirectory + "\\" + gameFileDirectories[i];
            }

            var t = new MPUP();
            t.Repack(directoriesToRepack, 1, gameDirectory, gameDirectory + @"\TextSoundRando\Temp\Textures");
        }

        private void UnpackSounds()
        {
            string unpackDirectory = gameDirectory + "\\TextSoundRando\\Unpack\\Sounds";
            if (!Directory.Exists(unpackDirectory))
            {
                Directory.CreateDirectory(unpackDirectory);
            }

            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();
            Thread[] threads = new Thread[threadCount];

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
            using (StreamWriter sw = new StreamWriter(gameDirectory + @"\TextSoundRando\soundMaps.csv"))
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
                            File.Copy(pathsToSwap[rando], tempDirectory + "\\" + Path.GetFileName(file), true);

                            sw.WriteLine(file, pathsToSwap[rando]);

                            if (!onlyCustomSounds)
                            {
                                pathsToSwap.RemoveAt(rando);
                            }
                        }
                        else
                        {
                            //not a sound file - copy it to directory as is
                            File.Copy(file, tempDirectory + "\\" + Path.GetFileName(file), true);
                        }
                    }
                }
            }
        }

        private List<string> GetSoundsToSwap()
        {
            string[] soundDirs = { "\\TextSoundRando\\Custom Sounds", "\\TextSoundRando\\Unpack\\Sounds" };
            List<string> sounds = new List<string>();

            for (int i = 0; i < soundDirs.Length; i++)
            {
                foreach (string directory in Directory.EnumerateDirectories(gameDirectory + soundDirs[i], "*", SearchOption.AllDirectories))
                {
                    sounds.AddRange(Directory.GetFiles(directory).Where(s => CheckIsValidSoundForSwapping(s)));
                }
            }

            return sounds;
        }

        private bool CheckIsValidSoundForSwapping(string fileName)
        {
            if (!File.Exists(fileName) || !validSoundExtensions.Contains(Path.GetExtension(fileName)))
            {
                return false;
            }

            //i have no idea what the blank sound files even do but fuck em
            if (Path.GetFileName(fileName).Contains("blank"))
            {
                return false;
            }

            return true;
        }

        private void RepackSounds()
        {
            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();
            Thread[] threads = new Thread[threadCount];

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

            //copy the files into the sound directory
            foreach (string fsbFile in Directory.EnumerateFiles(gameDirectory + "\\TextSoundRando\\Temp\\Sounds", "*.fsb", SearchOption.AllDirectories))
            {
                File.Copy(fsbFile, gameDirectory + "\\sound\\" + Path.GetFileName(fsbFile), true);
            }
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

        private void LogMessage(string message)
        {
            if (rtfLog.Text.Length > 0)
                rtfLog.AppendText("\n");

            rtfLog.AppendText(message);
        }

        private void LogError(string message)
        {
            if (rtfLog.Text.Length > 0)
                rtfLog.AppendText("\n");

            rtfLog.Select(rtfLog.TextLength, 0);
            rtfLog.SelectionColor = Color.Red;
            rtfLog.AppendText(message);
        }
    }
}
