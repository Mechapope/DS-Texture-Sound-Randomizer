using SoulsFormats;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using TeximpNet.DDS;

namespace DS_Texture_Sound_Randomizer
{
    class TPUP
    {
        // BNDs without texture files:
        // anibnd, luabnd, menuesdbnd, msgbnd, mtdbnd, parambnd, paramdefbnd, remobnd, rumblebnd

        private static readonly string[] validExtensions =
        {
            ".chrbnd",
            ".ffxbnd",
            ".fgbnd",
            ".objbnd",
            ".partsbnd",
            ".tpf",
            ".tpfbhd",
        };

        private const string TEXCONV_PATH = @"bin\texconv.exe";
        private const string TEXCONV_SUFFIX = "_autoconvert";

        private readonly bool repack;
        private readonly Dictionary<string, string> swaps;
        private readonly string gameDir, looseDir;
        private readonly object countLock, progressLock, writeLock;
        private int progress, progressMax;
        private bool stop;
        private Thread[] threads;
        private int fileCount, textureCount;

        public ConcurrentQueue<string> Log, Error;

        public TPUP(string setGameDir, string setLooseDir, bool setRepack, Dictionary<string, string> setSwaps, int threadCount)
        {
            stop = false;
            gameDir = Path.GetFullPath(setGameDir);
            looseDir = Path.GetFullPath(setLooseDir);
            repack = setRepack;
            swaps = setSwaps;
            Log = new ConcurrentQueue<string>();
            Error = new ConcurrentQueue<string>();
            threads = new Thread[threadCount];
            writeLock = new object();
            fileCount = 0;
            textureCount = 0;
            countLock = new object();
            progress = 0;
            progressMax = 0;
            progressLock = new object();
        }

        public void Stop()
        {
            appendLog("Stopping...");
            stop = true;
        }

        public int GetProgressMax()
        {
            return progressMax;
        }

        public int GetProgress()
        {
            lock (progressLock)
                return progress;
        }

        public void Start()
        {
            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();
            foreach (string filepath in Directory.EnumerateFiles(gameDir, "*", SearchOption.AllDirectories))
            {
                string decompressedExtension = Path.GetExtension(filepath);
                if (decompressedExtension == ".dcx")
                    decompressedExtension = Path.GetExtension(Path.GetFileNameWithoutExtension(filepath));

                bool valid = false;
                if (validExtensions.Contains(decompressedExtension))
                {
                    valid = true;
                    if (repack)
                    {
                        string relative = Path.GetDirectoryName(filepath.Substring(gameDir.Length + 1));
                        string filename = Path.GetFileNameWithoutExtension(filepath);
                        if (Path.GetExtension(filepath) == ".dcx")
                            filename = Path.GetFileNameWithoutExtension(filename);
                        //valid = false;
                        //foreach (string path in swaps.Keys)
                        //{
                        //    if (Path.GetDirectoryName(path + ".dds") == relative + "\\" + filename)
                        //    {
                        //        valid = true;
                        //        break;
                        //    }
                        //}
                    }
                }

                //TODO fix better
                if (valid)
                    filepaths.Enqueue(filepath);
            }
            progressMax = filepaths.Count;

            if (repack)
                appendLog("Checking {0:n0} files for randomizing...", filepaths.Count);
            else
            {
                appendLog("Unpacking {0:n0} files...", filepaths.Count);
                fileCount = filepaths.Count;
            }

            for (int i = 0; i < threads.Length; i++)
            {
                Thread thread = new Thread(() => iterateFiles(filepaths));
                threads[i] = thread;
                thread.Start();
            }

            foreach (Thread thread in threads)
                thread.Join();

            if (stop)
            {
                if (repack)
                    appendLog("Repacking stopped.");
                else
                    appendLog("Unpacking stopped.");
            }
            else
            {
                if (repack)
                    appendLog("Repacked {0:n0} textures in {1:n0} files!", textureCount, fileCount);
                else
                    appendLog("Unpacked {0:n0} textures from {1:n0} files!", textureCount, fileCount);
            }
        }

        private void iterateFiles(ConcurrentQueue<string> filepaths)
        {
            string filepath;
            while (!stop && filepaths.TryDequeue(out filepath))
            {
                // These are already full paths, but trust no one, not even yourself
                string absolute = Path.GetFullPath(filepath);
                string relative = absolute.Substring(gameDir.Length + 1);

                if (repack)
                    appendLog("Checking: " + relative);
                else
                    appendLog("Unpacking: " + relative);

                bool dcx = false;
                byte[] bytes = File.ReadAllBytes(absolute);
                string extension = Path.GetExtension(absolute);
                string subpath = Path.GetDirectoryName(relative) + "\\" + Path.GetFileNameWithoutExtension(absolute);
                if (extension == ".dcx")
                {
                    dcx = true;
                    bytes = DCX.Decompress(bytes);
                    extension = Path.GetExtension(Path.GetFileNameWithoutExtension(absolute));
                    subpath = subpath.Substring(0, subpath.Length - extension.Length);
                }

                bool edited = false;
                switch (extension)
                {
                    case ".tpf":
                        TPF tpf = TPF.Read(bytes);
                        if (processTPF(tpf, looseDir, subpath))
                        {
                            edited = true;
                            byte[] tpfBytes = tpf.Write();
                            if (dcx)
                                tpfBytes = DCX.Compress(tpfBytes, DCX.Type.DarkSouls1);
                            writeRepack(absolute, tpfBytes);
                            lock (countLock)
                                fileCount++;
                        }
                        break;

                    //case ".tpfbhd":
                    //    string dir = Path.GetDirectoryName(absolute);
                    //    string name = Path.GetFileNameWithoutExtension(absolute);
                    //    string bdtPath = dir + "\\" + name + ".tpfbdt";
                    //    if (File.Exists(bdtPath))
                    //    {
                    //        byte[] bdtBytes = File.ReadAllBytes(bdtPath);
                    //        BDT bdt = BDT.Read(bytes, bdtBytes);
                    //        if (processBDT(bdt, looseDir, subpath))
                    //        {
                    //            edited = true;
                    //            (byte[], byte[]) repacked = bdt.Write();
                    //            if (dcx)
                    //            {
                    //                repacked.Item1 = DCX.Compress(repacked.Item1);
                    //            }
                    //            writeRepack(absolute, repacked.Item1);
                    //            writeRepack(bdtPath, repacked.Item2);
                    //            lock (countLock)
                    //                fileCount++;
                    //        }
                    //    }
                    //    else
                    //        throw new FileNotFoundException("Data file not found for header: " + relative);
                    //    break;

                    case ".chrbnd":
                    case ".ffxbnd":
                    case ".fgbnd":
                    case ".objbnd":
                    case ".partsbnd":
                        BND3 bnd = BND3.Read(bytes);
                        foreach (var entry in bnd.Files)
                        {
                            if (stop)
                                break;

                            string entryExtension = Path.GetExtension(entry.Name);
                            if (entryExtension == ".tpf")
                            {
                                TPF bndTPF = TPF.Read(entry.Bytes);
                                if (processTPF(bndTPF, looseDir, subpath))
                                {
                                    entry.Bytes = bndTPF.Write();
                                    edited = true;
                                }
                            }
                            //else if (entryExtension == ".chrtpfbhd")
                            //{
                            //    string bndDir = Path.GetDirectoryName(absolute);
                            //    string bndName = Path.GetFileNameWithoutExtension(absolute);
                            //    if (dcx)
                            //        bndName = Path.GetFileNameWithoutExtension(bndName);
                            //    string bndBDTPath = bndDir + "\\" + bndName + ".chrtpfbdt";
                            //    if (File.Exists(bndBDTPath))
                            //    {
                            //        byte[] bdtBytes = File.ReadAllBytes(bndBDTPath);
                            //        BDT bndBDT = BDT.Read(entry.Bytes, bdtBytes);
                            //        if (processBDT(bndBDT, looseDir, subpath))
                            //        {
                            //            (byte[], byte[]) repacked = bndBDT.Write();
                            //            entry.Bytes = repacked.Item1;
                            //            writeRepack(bndBDTPath, repacked.Item2);
                            //            edited = true;
                            //        }
                            //    }
                            //    else
                            //        throw new FileNotFoundException("Data file not found for header: " + relative);
                            //}
                        }

                        if (edited && !stop)
                        {
                            byte[] bndBytes = bnd.Write();
                            if (dcx)
                            {
                                bndBytes = DCX.Compress(bndBytes, DCX.Type.DarkSouls1);
                            }
                            writeRepack(absolute, bndBytes);
                            lock (countLock)
                                fileCount++;
                        }
                        break;
                }

                if (repack && !edited && !stop)
                    appendError("Notice: {0}\r\n\u2514\u2500 No overrides found.", relative);

                lock (progressLock)
                    progress++;
            }
        }

        //private bool processBDT(BDT bdt, string baseDir, string subPath)
        //{
        //    bool edited = false;
        //    foreach (BDT.File bdtEntry in bdt.Files)
        //    {
        //        if (stop)
        //            return false;

        //        bool dcx = false;
        //        byte[] bdtEntryBytes = bdtEntry.Bytes;
        //        string bdtEntryExtension = Path.GetExtension(bdtEntry.Name);
        //        if (bdtEntryExtension == ".dcx")
        //        {
        //            dcx = true;
        //            bdtEntryBytes = DCX.Decompress(bdtEntryBytes);
        //            bdtEntryExtension = Path.GetExtension(bdtEntry.Name.Substring(0, bdtEntry.Name.Length - 4));
        //        }

        //        if (bdtEntryExtension == ".tpf")
        //        {
        //            TPF tpf = TPF.Read(bdtEntryBytes);
        //            if (processTPF(tpf, baseDir, subPath))
        //            {
        //                bdtEntry.Bytes = tpf.Write();
        //                if (dcx)
        //                    bdtEntry.Bytes = DCX.Compress(bdtEntry.Bytes);
        //                edited = true;
        //            }
        //        }
        //        // This whouldn't really be a problem, but I would like to know about it
        //        else
        //            appendError("Error: {0}\r\n\u2514\u2500 Non-tpf found in tpfbdt: {1}", subPath, bdtEntry.Name);
        //    }
        //    return edited;
        //}

        private bool processTPF(TPF tpf, string baseDir, string subDir)
        {
            // parts\HR_F_0010 and parts\HR_F_0010_M have duplicate filenames in the same tpf
            // thx QLOC
            List<string> names = new List<string>();
            List<string> dupes = new List<string>();
            foreach (TPF.Texture tpfEntry in tpf.Textures)
            {
                if (names.Contains(tpfEntry.Name))
                    dupes.Add(tpfEntry.Name);
                else
                    names.Add(tpfEntry.Name);
            }

            bool edited = false;
            for (int i = 0; i < tpf.Textures.Count; i++)
            {
                if (stop)
                    return false;

                TPF.Texture tpfEntry = tpf.Textures[i];
                string name = tpfEntry.Name;
                if (dupes.Contains(name))
                    name += "_" + i;

                if (repack)
                    edited |= repackFile(tpfEntry, name, baseDir, subDir);
                else
                    unpackFile(tpfEntry, name, baseDir, subDir);
            }
            return edited;
        }

        private void unpackFile(TPF.Texture tpfEntry, string name, string baseDir, string subDir)
        {
            string subPath = subDir + "\\" + name + ".dds";
            string ddsPath = baseDir + "\\" + subPath;

            lock (writeLock)
            {
                if (!File.Exists(ddsPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ddsPath));
                    File.WriteAllBytes(ddsPath, tpfEntry.Bytes);
                    lock (countLock)
                        textureCount++;
                }
                else
                    appendError("Error: {0}\r\n\u2514\u2500 Duplicate file found.", subPath);
            }
        }

        private bool repackFile(TPF.Texture tpfEntry, string name, string baseDir, string subDir)
        {
            bool dds = true;
            string inputPath = baseDir + "\\" + subDir + "\\" + name + ".dds";

            if (inputPath != null && File.Exists(inputPath))
            {
                byte[] inputBytes = File.ReadAllBytes(inputPath);

                DXGIFormat originalFormat = DDSFile.Read(new MemoryStream(tpfEntry.Bytes)).Format;
                if (originalFormat == DXGIFormat.Unknown)
                    appendError("Error: {0}\r\n\u2514\u2500 Could not determine format of game file.", subDir + "\\" + name + ".dds");

                bool convert = !dds;
                if (dds)
                {
                    DXGIFormat newFormat = DDSFile.Read(new MemoryStream(inputBytes)).Format;

                    if (newFormat == DXGIFormat.Unknown)
                        appendError("Error: {0}\r\n\u2514\u2500 Could not determine format of override file.", inputPath);

                    if (originalFormat != DXGIFormat.Unknown && newFormat != DXGIFormat.Unknown && originalFormat != newFormat)
                        convert = true;
                }

                if (convert)
                {
                    byte[] newBytes = convertFile(inputPath, originalFormat);
                    if (newBytes != null)
                        inputBytes = newBytes;
                }

                tpfEntry.Bytes = inputBytes;
                lock (countLock)
                    textureCount++;

                return true;
            }
            else
                return false;
        }

        private string getSwappedPath(string name, string subDir, out bool dds)
        {
            string swappedPath = null;
            string extension = null;

            if (name.Substring(name.Length - 2) == "_n")
            {
                string swapName = name.Substring(0, name.Length - 2);
                //if (swaps.ContainsKey(subDir + "\\" + swapName))
                //{
                //    swappedPath = swaps[subDir + "\\" + swapName];
                //    extension = Path.GetExtension(swappedPath);
                //    swappedPath = Path.GetDirectoryName(swappedPath) + "\\" + Path.GetFileNameWithoutExtension(swappedPath) + "_n" + extension;
                //}
            }
            else if (name.Substring(name.Length - 2) == "_s")
            {
                string swapName = name.Substring(0, name.Length - 2);
                //if (swaps.ContainsKey(subDir + "\\" + swapName))
                //{
                //    swappedPath = swaps[subDir + "\\" + swapName];
                //    extension = Path.GetExtension(swappedPath);
                //    swappedPath = Path.GetDirectoryName(swappedPath) + "\\" + Path.GetFileNameWithoutExtension(swappedPath) + "_s" + extension;
                //}
            }
            else if (swaps.ContainsKey(subDir + "\\" + name))
            {
                //swappedPath = swaps[subDir + "\\" + name];
                //extension = Path.GetExtension(swappedPath);
            }

            dds = extension != null && extension == ".dds";
            return swappedPath;
        }

        private int convertInc = 0;

        private byte[] convertFile(string filepath, DXGIFormat format)
        {
            if (!File.Exists(TEXCONV_PATH))
            {
                appendError("Error: texconv.exe not found.");
                return null;
            }

            filepath = Path.GetFullPath(filepath);
            string directory = Path.GetDirectoryName(filepath);
            string filename = Path.GetFileName(filepath);
            string suffix = TEXCONV_SUFFIX + (convertInc++).ToString();
            string outPath = string.Format("{0}\\{1}{2}.dds",
                directory, Path.GetFileNameWithoutExtension(filename), suffix);

            string args = string.Format("-sx {0} -f {1} -o \"{2}\" \"{2}\\{3}\" -y -singleproc -pow2",
                suffix, PrintDXGIFormat(format), directory, filename);
            ProcessStartInfo startInfo = new ProcessStartInfo(TEXCONV_PATH, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Process texconv = Process.Start(startInfo);
            texconv.WaitForExit();

            byte[] result = null;
            if (texconv.ExitCode == 0 && File.Exists(outPath))
            {
                result = File.ReadAllBytes(outPath);

                try
                {
                    File.Delete(outPath);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    appendError("Error: {0}\r\n\u2514\u2500 Could not delete converted file.", outPath);
                }
            }
            else
                appendError("Error: {0}\\{1}\r\n\u2514\u2500 Conversion failed.", directory, filename);
            return result;
        }

        private void appendLog(string format, params object[] args)
        {
            string line = string.Format(format, args);
            Log.Enqueue(line);
        }

        private void appendError(string format, params object[] args)
        {
            string line = string.Format(format, args);
            Error.Enqueue(line);
        }

        private static void writeRepack(string path, byte[] bytes)
        {
            if (!File.Exists(path + ".trbak"))
                File.Copy(path, path + ".trbak");
            File.WriteAllBytes(path, bytes);
        }

        private static Dictionary<DXGIFormat, string> dxgiFormatOverride = new Dictionary<DXGIFormat, string>()
        {
            [DXGIFormat.BC1_UNorm] = "DXT1",
            [DXGIFormat.BC2_UNorm] = "DXT3",
            [DXGIFormat.BC3_UNorm] = "DXT5",
            // It's 420_OPAQUE officially, but you can't start an enum member with a number
            [DXGIFormat.Opaque_420] = "420_OPAQUE",
        };

        public static string PrintDXGIFormat(DXGIFormat format)
        {
            if (dxgiFormatOverride.ContainsKey(format))
                return dxgiFormatOverride[format];
            else
                return format.ToString().ToUpper();
        }
    }
}
