using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DS_Texture_Sound_Randomizer
{
    class SPUP
    {
        public ConcurrentQueue<string> Log;
        private string gameDirectory;
        private Thread[] threads;
        private int fileCount;
        int timeOut = 1000 * 60 * 10; //5 minutes

        public void Unpack(string directoryToUnpack, int numThreads, string gameDir)
        {
            Log = new ConcurrentQueue<string>();
            gameDirectory = gameDir;

            if (!Directory.Exists(directoryToUnpack))
            {
                Directory.CreateDirectory(directoryToUnpack);
            }

            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();
            Thread[] threads = new Thread[numThreads];

            foreach (string filepath in Directory.EnumerateFiles(gameDir + "\\sound", "*.fsb"))
            {
                filepaths.Enqueue(filepath);
            }

            fileCount = filepaths.Count;

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
                Log.Enqueue("Unpacking sound file " + (fileCount - filepaths.Count) + " of " + fileCount);

                string unpackDirectory = gameDirectory + @"\TextSoundRando\Unpack\Sound\" + Path.GetFileName(filepath);

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

                //one fsb keep extracting as .wav instead of .wav.mp3 which ruins everything
                foreach (string file in Directory.EnumerateFiles(unpackDirectory, "*.wav"))
                {
                    File.Copy(file, file + ".mp3");
                    File.Delete(file);
                }

                using (StreamWriter sw = new StreamWriter(unpackDirectory + "\\fsblist.lst"))
                {
                    foreach (string file in Directory.EnumerateFiles(unpackDirectory, "*.mp3"))
                    {
                        sw.WriteLine(Path.GetFileName(file));
                    }
                }
            }
        }

        public void Repack(string directoryToRepack, int numThreads, string gameDir)
        {
            gameDirectory = gameDir;
            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();
            Log = new ConcurrentQueue<string>();

            if(directoryToRepack == gameDirectory + "\\TextSoundRando\\Temp\\Sound\\frpg_main.fsb")
            {
                filepaths.Enqueue(gameDirectory + "\\TextSoundRando\\Temp\\Sound\\frpg_main.fsb");
            }

            foreach (string filepath in Directory.EnumerateDirectories(directoryToRepack))
            {
                filepaths.Enqueue(filepath);
            }

            fileCount = filepaths.Count;

            if(!Directory.Exists(gameDirectory + "\\TextSoundRando\\Output\\sound"))
            {
                Directory.CreateDirectory(gameDirectory + "\\TextSoundRando\\Output\\sound");
            }

            //Thread[] threads = new Thread[numThreads];
            //string masterDssi = gameDirectory + "\\TextSoundRando\\Binaries\\DSSI";

            //for (int i = 0; i < threads.Length; i++)
            //{
            //    string cloneDirectory = masterDssi + i.ToString();

            //    if (!Directory.Exists(cloneDirectory))
            //    {
            //        Directory.CreateDirectory(Path.Combine(cloneDirectory + "\\FSBViewer"));
            //        Directory.CreateDirectory(Path.Combine(cloneDirectory + "\\INPUT"));
            //        Directory.CreateDirectory(Path.Combine(cloneDirectory + "\\OUTPUT"));

            //        //the dumbest thing ive ever coded - make copies of dssi in different folders so this runs faster
            //        foreach (var item in Directory.EnumerateFiles(masterDssi, "*", SearchOption.AllDirectories))
            //        {
            //            File.Copy(item, Path.Combine(cloneDirectory + "\\" + item.Substring(masterDssi.Length + 1)));
            //        }
            //    }

            //    Thread thread = new Thread(() => RepackFSBs2(cloneDirectory, filepaths, filepaths.Count));
            //    threads[i] = thread;
            //    thread.Start();
            //}

            //foreach (Thread thread in threads)
            //    thread.Join();

            Thread thread = new Thread(() => RepackFSBs2(gameDirectory + "\\TextSoundRando\\Binaries\\DSSI", filepaths, filepaths.Count));
            thread.Start();
            thread.Join();


            //copy the files into the sound directory
            foreach (string fsbFile in Directory.EnumerateFiles(gameDirectory + "\\TextSoundRando\\Temp\\Sound", "*.fsb", SearchOption.AllDirectories))
            {
                File.Copy(fsbFile, gameDirectory + "\\TextSoundRando\\Output\\sound\\" + Path.GetFileName(fsbFile), true);
            }
        }

        private void RepackFSBs2(string dssiPath, ConcurrentQueue<string> filepaths, int totalAmount)
        {
            //make sure dssi input/output folders are clear
            foreach (var file in Directory.GetFiles(dssiPath + "\\input"))
            {
                File.Delete(file);
            }

            foreach (var file in Directory.GetFiles(dssiPath + "\\output"))
            {
                File.Delete(file);
            }

            string filepath;
            while (filepaths.TryDequeue(out filepath))
            {
                Log.Enqueue("Repacking sound file " + (fileCount - filepaths.Count) + " of " + fileCount);

                foreach (var file in Directory.GetFiles(filepath))
                {
                    File.Copy(file, dssiPath + "\\input\\" + Path.GetFileName(file), true);
                }
                //wait a short time or else youll get file access error sometimes
                Thread.Sleep(5000);

                //runProcess(dssiPath, "listrearranger.exe", Path.GetFileName(filepath));
                string baseDir = @"D:\Program Files (x86)\Steam\steamapps\common\Dark Souls Prepare to Die Edition\DATA\TextSoundRando\Binaries\DSSI";

                runProcess(baseDir, "listrearranger.exe", Path.GetFileName(filepath));
                runProcess(baseDir, "fsbankcl.exe", @"-o OUTPUT\asd.fsb -f mp3 -q 50 INPUT\fsblist.lst");
                runProcess(baseDir, "inserter.exe", Path.GetFileName(filepath));

                //wait a short time or else youll get file access error sometimes
                Thread.Sleep(5000);

                File.Copy(dssiPath + "\\output\\" + Path.GetFileName(filepath), gameDirectory + "\\TextSoundRando\\Temp\\Sound\\" + Path.GetFileName(filepath) + "\\" + Path.GetFileName(filepath), true);

                //clear input + output folder
                foreach (var file in Directory.GetFiles(dssiPath + "\\input"))
                {
                    File.Delete(file);
                }

                foreach (var file in Directory.GetFiles(dssiPath + "\\output"))
                {
                    File.Delete(file);
                }
            }
        }

        private void runProcess(string directory, string fileName, string arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = fileName;
            info.WorkingDirectory = directory;
            info.Arguments = arguments;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            using (Process p = Process.Start(info))
            {
                p.WaitForExit();
            }
        }

    }
}
