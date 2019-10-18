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
        string gameDirectory = "";
        string[] fileTypesToBackup = { ".chrbnd", ".ffxbnd", ".fgbnd", ".objbnd", ".partsbnd", ".tpf", ".tpfbhd", ".fsb" };
        string[] gameFileDirectories = { "chr", "font", "map", "menu", "other", "parts", "sfx", "sound" };
        string[] validImageExtensions = { ".png", ".jpg", ".jpeg", ".tga" };
        string[] validSoundExtensions = { ".mp3", ".wav" };
        string[] uiTextureFiles = { "DSFont24_0000.dds", "DSFont24_0001.dds", "TalkFont24_0000.dds", "TalkFont24_0001.dds" };

        int soundSmallFileThreshold = 40000;
        int soundMediumFileThreshold = 100000;
        int minMainSoundFileSize = 5500000;
        int maxMainSoundFileSize = 8400000;

        int seed = 0;
        int threadCount;
        bool isBackupsExist = false;
        bool isGameUnpacked = false;
        bool randomizeTextures, onlyCustomTextures, randomizeUiTextures, randomizeSounds, onlyCustomSounds;

        private MPUP mpup;
        private SPUP spup;
        private Thread mpupThread;
        private Thread spupThread;

        ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();

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
                    EnableButtons(false);
                }
            }
            else
            {
                EnableButtons(false);
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
                    EnableButtons(false);
                }
                else if (!File.Exists(gameDirectory + "\\param\\GameParam\\GameParam.parambnd"))
                {
                    //user hasn't unpacked their game
                    LogError("You don't seem to have an unpacked Dark Souls installation. Please run UDSFM and come back :)");
                    EnableButtons(false);
                }
                else
                {
                    EnableButtons(true);
                }

                if (File.Exists(gameDirectory + @"\TextSoundRando\Backup\sfx\FRPG_SfxBnd_m18_01.ffxbnd"))
                {
                    isBackupsExist = true;
                    btnCreateBackups.Enabled = false;
                    LogMessage("Backups found.");
                }
                else
                {
                    btnRestoreBackups.Enabled = false;
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
            btnRestoreBackups.Enabled = true;
        }

        private void btnRestoreBackups_Click(object sender, EventArgs e)
        {
            LogMessage("Restoring backups.");

            Thread t = new Thread(RestoreBackups);
            t.Start();
            t.Join();

            LogMessage("Backups restored.");
        }

        private void btnExtractFiles_Click(object sender, EventArgs e)
        {
            threadCount = (int)numThreads.Value;

            UnpackTextures();
            UnpackSounds();

            isGameUnpacked = true;
            btnExtractFiles.Enabled = false;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            threadCount = (int)numThreads.Value;

            if (!ValidateInput())
            {
                return;
            }

            ClearTempFolder();

            if (randomizeTextures)
            {
                ConvertTextures();
                RandomizeTextures();
                RepackTextures();
            }

            if(randomizeSounds)
            {
                RandomizeSounds();
                RepackSounds();
                FixMainSoundFile();
            }

            if(chkFixMainSoundFile.Checked)
            {
                FixMainSoundFile();
            }

            ClearTempFolder();

            LogMessage("Randomizing complete!");
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

            if (!isBackupsExist)
            {
                LogError("Create a backup first.");
                return false;
            }

            if (!isGameUnpacked)
            {
                LogError("Unpack your game files first.");
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

            if(!randomizeTextures && !randomizeSounds && !chkFixMainSoundFile.Checked)
            {
                LogError("Select an operation.");
                return false;
            }

            return true;
        }

        private void ClearTempFolder()
        {
            LogMessage("Clearing temp folders.");
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
                if(!Directory.Exists(backupDirectory + "\\" + gameFileDirectories[i]))
                {
                    foreach (string file_name in Directory.GetFiles(gameDirectory + "\\" + gameFileDirectories[i], "*", SearchOption.AllDirectories))
                    {
                        if(fileTypesToBackup.Contains(Path.GetExtension(file_name)))
                        {
                            Directory.CreateDirectory(backupDirectory + Path.GetDirectoryName(file_name).Substring(gameDirectory.Length));
                            File.Copy(file_name, backupDirectory + file_name.Substring(gameDirectory.Length));
                        }
                    }
                }
            }
        }

        private void RestoreBackups()
        {
            string backupDirectory = gameDirectory + @"\TextSoundRando\backup\";

            foreach (var item in Directory.EnumerateFiles(backupDirectory, "*", SearchOption.AllDirectories))
            {
                File.Copy(item, item.Replace(@"\TextSoundRando\backup\", "\\"), true);
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

            mpup = new MPUP();
            mpupThread = new Thread(() => mpup.Unpack(directoriesToUnpack, threadCount, gameDirectory, unpackDirectory));
            mpupThread.Start();
            mpupThread.Join();
        }

        private void ConvertTextures()
        {
            LogMessage("Converting custom textures to .DDS format");

            foreach (string item in Directory.GetFiles(gameDirectory + "\\TextSoundRando\\Custom Textures"))
            {
                if(validImageExtensions.Contains(Path.GetExtension(item).ToLower()))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.WorkingDirectory = gameDirectory + @"\TextSoundRando\binaries\texconv";
                    startInfo.Arguments = $"/C texconv -o \"{gameDirectory + "\\TextSoundRando\\Custom Textures"}\" -y -singleproc -pow2 \"{item}\"";

                    using (Process process = Process.Start(startInfo))
                    {
                        process.WaitForExit();
                    }
                }                
            }
        }

        private void RandomizeTextures()
        {
            LogMessage("Randomizing textures.");

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
            if(!File.Exists(fileName) || Path.GetExtension(fileName).ToLower() != ".dds")
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
            LogMessage("Repacking textures.");

            string[] directoriesToRepack = new string[gameFileDirectories.Length];
            for (int i = 0; i < gameFileDirectories.Length; i++)
            {
                directoriesToRepack[i] = gameDirectory + "\\" + gameFileDirectories[i];
            }

            mpup = new MPUP();
            mpupThread = new Thread(() => mpup.Repack(directoriesToRepack, threadCount, gameDirectory, gameDirectory + @"\TextSoundRando\Temp\Textures"));
            mpupThread.Start();
            mpupThread.Join();
        }

        private void UnpackSounds()
        {
            spup = new SPUP();
            spupThread = new Thread(() => spup.Unpack(gameDirectory + "\\TextSoundRando\\Unpack\\Sounds", threadCount, gameDirectory));
            spupThread.Start();
            spupThread.Join();
        }

        private void UnpackFSBs(ConcurrentQueue<string> filepaths)
        {
            string filepath;
            while (filepaths.TryDequeue(out filepath))
            {
                string unpackDirectory = gameDirectory + @"\TextSoundRando\Unpack\Sounds\" + Path.GetFileName(filepath);

                Directory.CreateDirectory(unpackDirectory);

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



        private void RandomizeSounds()
        {
            LogMessage("Randomizing sounds.");

            using (StreamWriter sw = new StreamWriter(gameDirectory + @"\TextSoundRando\soundMaps.csv"))
            {
                string inputFolder = gameDirectory + @"\TextSoundRando\Unpack\Sounds";


                List<string> smallSoundFiles, mediumSoundFiles, largeSoundFiles;

                GetSoundsToSwap(out smallSoundFiles, out mediumSoundFiles, out largeSoundFiles);

                Random r = new Random(seed);

                foreach (string directory in Directory.EnumerateDirectories(inputFolder, "*", SearchOption.AllDirectories))
                {
                    string tempDirectory = Path.Combine(gameDirectory + @"\TextSoundRando\Temp\Sounds\", Path.GetFileName(directory));
                    Directory.CreateDirectory(tempDirectory);

                    foreach (string file in Directory.GetFiles(directory))
                    {
                        if (CheckIsValidSoundForSwapping(file))
                        {
                            long fileSize = new FileInfo(file).Length;

                            //sound files must be replaced by a similar-ish sized or else the game might not load
                            if (fileSize < soundSmallFileThreshold)
                            {
                                int rando = r.Next(smallSoundFiles.Count);
                                File.Copy(smallSoundFiles[rando], tempDirectory + "\\" + Path.GetFileName(file), true);

                                sw.WriteLine(file, smallSoundFiles[rando]);

                                if (!onlyCustomSounds)
                                {
                                    smallSoundFiles.RemoveAt(rando);
                                }
                            }
                            else if (fileSize < soundMediumFileThreshold)
                            {
                                int rando = r.Next(mediumSoundFiles.Count);
                                File.Copy(mediumSoundFiles[rando], tempDirectory + "\\" + Path.GetFileName(file), true);

                                sw.WriteLine(file, mediumSoundFiles[rando]);

                                if (!onlyCustomSounds)
                                {
                                    mediumSoundFiles.RemoveAt(rando);
                                }
                            }
                            else
                            {
                                int rando = r.Next(largeSoundFiles.Count);
                                File.Copy(largeSoundFiles[rando], tempDirectory + "\\" + Path.GetFileName(file), true);

                                sw.WriteLine(file, largeSoundFiles[rando]);

                                if (!onlyCustomSounds)
                                {
                                    largeSoundFiles.RemoveAt(rando);
                                }
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

        private void GetSoundsToSwap(out List<string> smallSoundFiles, out List<string> mediumSoundFiles, out List<string> largeSoundFiles)
        {
            smallSoundFiles = new List<string>();
            mediumSoundFiles = new List<string>();
            largeSoundFiles = new List<string>();

            string[] soundDirs = { "\\TextSoundRando\\Custom Sounds", "\\TextSoundRando\\Unpack\\Sounds" };

            for (int i = 0; i < soundDirs.Length; i++)
            {
                foreach (string directory in Directory.EnumerateDirectories(gameDirectory + soundDirs[i], "*", SearchOption.AllDirectories))
                {
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        if(CheckIsValidSoundForSwapping(file))
                        {
                            long fileSize = new FileInfo(file).Length;

                            if (fileSize < soundSmallFileThreshold)
                            {
                                smallSoundFiles.Add(file);
                            }
                            else if (fileSize < soundMediumFileThreshold)
                            {
                                mediumSoundFiles.Add(file);
                            }
                            else
                            {
                                largeSoundFiles.Add(file);
                            }
                        }
                    }
                }
            }
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
            spup = new SPUP();
            spupThread = new Thread(() => spup.Repack(gameDirectory + "\\TextSoundRando\\Temp\\Sounds", threadCount, gameDirectory));
            spupThread.Start();
            spupThread.Join();
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
                startInfo.Arguments = $"/C fsbext -d \"{unpackDirectory}\" -r \"{unpackDirectory + "\\" + filepath}\"";

                //run fsbext on each fsb file
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
        }

        private void RepackFSBs2(string dssiPath, ConcurrentQueue<string> filepaths, int totalAmount)
        {
            string filepath;
            while (filepaths.TryDequeue(out filepath))
            {
                logQueue.Enqueue("Repacking sound file" + (totalAmount - filepaths.Count) + " of " + totalAmount);

                foreach (var file in Directory.GetFiles(filepath))
                {
                    File.Copy(file, dssiPath + "\\input\\" + Path.GetFileName(file));
                }

                //wait a short time or else youll get file access error sometimes
                Thread.Sleep(5000);

                ProcessStartInfo startInfo = new ProcessStartInfo(dssiPath + "\\DSSI.bat");
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = dssiPath;

                using(Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                //wait a short time or else youll get file access error sometimes
                Thread.Sleep(5000);

                //clear input folder
                foreach (var file in Directory.GetFiles(dssiPath + "\\input"))
                {
                    File.Delete(file);
                }

                File.Copy(dssiPath + "\\output\\" + Path.GetFileName(filepath), gameDirectory + "\\TextSoundRando\\Output\\Sound\\" + Path.GetFileName(filepath), true);
                File.Delete(dssiPath + "\\output\\" + Path.GetFileName(filepath));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mpupThread != null)
            {
                if (mpupThread.IsAlive)
                {
                    updateLogs();
                }
                else
                {
                    updateLogs();
                    mpup = null;
                    mpupThread = null;
                }
            }

            if (spupThread != null)
            {
                if (spupThread.IsAlive)
                {
                    updateLogs();
                }
                else
                {
                    updateLogs();
                    spup = null;
                    spupThread = null;
                }
            }

        }

        private void updateLogs()
        {
            string line = "";

            if(mpup != null)
            {
                while (mpup.Log.TryDequeue(out line))
                    LogMessage(line);
            }
            
            if(spup != null)
            {
                while (spup.Log.TryDequeue(out line))
                    LogMessage(line);
            }           
        }

        private void FixMainSoundFile()
        {
            LogMessage("Checking main sound file");

            bool isValid = false;
            string mainSoundFileFolder = gameDirectory + "\\TextSoundRando\\Temp\\Sounds\\frpg_main.fsb";
            
            if (File.Exists(mainSoundFileFolder + "\\frpg_main.fsb"))
            {
                long mainFileSize = new FileInfo(mainSoundFileFolder + "\\frpg_main.fsb").Length;
                if(mainFileSize > minMainSoundFileSize && mainFileSize < maxMainSoundFileSize)
                {
                    isValid = true;
                }
            }

            while (!isValid)
            {
                LogMessage("Main sound file is invalid, retrying.");

                Directory.CreateDirectory(mainSoundFileFolder);

                //clear main sound temp folder
                foreach (var file in Directory.GetFiles(mainSoundFileFolder))
                {
                    File.Delete(file);
                }

                List<string> smallSoundFiles, mediumSoundFiles, largeSoundFiles;

                GetSoundsToSwap(out smallSoundFiles, out mediumSoundFiles, out largeSoundFiles);

                //cant use seed for fixing main file or itll just give the same files over
                Random r = new Random();

                foreach (string file in Directory.GetFiles(gameDirectory + "\\TextSoundRando\\Unpack\\Sounds\\frpg_main.fsb"))
                {
                    if (CheckIsValidSoundForSwapping(file))
                    {
                        long fileSize = new FileInfo(file).Length;

                        //sound files must be replaced by a similar-ish sized or else the game might not load
                        if (fileSize < soundSmallFileThreshold)
                        {
                            int rando = r.Next(smallSoundFiles.Count);
                            File.Copy(smallSoundFiles[rando], mainSoundFileFolder + "\\" + Path.GetFileName(file), true);

                            if (!onlyCustomSounds)
                            {
                                smallSoundFiles.RemoveAt(rando);
                            }
                        }
                        else if (fileSize < soundMediumFileThreshold)
                        {
                            int rando = r.Next(mediumSoundFiles.Count);
                            File.Copy(mediumSoundFiles[rando], mainSoundFileFolder + "\\" + Path.GetFileName(file), true);

                            if (!onlyCustomSounds)
                            {
                                mediumSoundFiles.RemoveAt(rando);
                            }
                        }
                        else
                        {
                            int rando = r.Next(largeSoundFiles.Count);
                            File.Copy(largeSoundFiles[rando], mainSoundFileFolder + "\\" + Path.GetFileName(file), true);

                            if (!onlyCustomSounds)
                            {
                                largeSoundFiles.RemoveAt(rando);
                            }
                        }
                    }
                    else
                    {
                        //not a sound file - copy it to directory as is
                        File.Copy(file, mainSoundFileFolder + "\\" + Path.GetFileName(file), true);
                    }
                }

                spup = new SPUP();
                spupThread = new Thread(() => spup.Repack(gameDirectory + "\\TextSoundRando\\Temp\\Sounds\\frpg_main.fsb", 1, gameDirectory));
                spupThread.Start();
                spupThread.Join();

                long mainFileSize = new FileInfo(gameDirectory + "TextSoundRando\\Output\\sound\\frpg_main.fsb").Length;

                if (File.Exists(mainSoundFileFolder + "\\frpg_main.fsb") && mainFileSize > minMainSoundFileSize && mainFileSize < maxMainSoundFileSize)
                {
                    isValid = true;
                }
            }

            File.Copy(mainSoundFileFolder + "\\frpg_main.fsb", gameDirectory + "\\sound\\frpg_main.fsb", true);

            LogMessage("Main sound file is valid!");
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

        private void EnableButtons(bool enabled)
        {
            btnCreateBackups.Enabled = enabled;
            btnRestoreBackups.Enabled = enabled;
            btnExtractFiles.Enabled = enabled;
            btnOpenCustomTextureFolder.Enabled = enabled;
            btnOpenCustomSoundFolder.Enabled = enabled;
            btnSubmit.Enabled = enabled;
        }
    }
}
