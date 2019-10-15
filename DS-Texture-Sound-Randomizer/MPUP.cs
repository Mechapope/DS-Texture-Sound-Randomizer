/*
 * Restructured version of TKGP's TPUP.cs from his DSR Texture Rando: https://github.com/JKAnderson/DSR-Texture-Randomizer
 * I have no idea what TPUP stands for but I'm naming mine Mechapope's Path Unpacking Program :)
 */

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
    class MPUP
    {
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

        private string gameDir, looseDir;
        private Thread[] threads;

        private int convertInc = 0;
        private const string TEXCONV_PATH = @"bin\texconv.exe";
        private const string TEXCONV_SUFFIX = "_autoconvert";

        public void Unpack(string[] directoriesToUnpack, int numThreads, string gameDir, string looseDir)
        {
            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();
            this.gameDir = gameDir;
            this.looseDir = looseDir;

            //Search directories for valid files
            for (int i = 0; i < directoriesToUnpack.Length; i++)
            {
                foreach (string filepath in Directory.EnumerateFiles(directoriesToUnpack[i], "*", SearchOption.AllDirectories))
                {
                    string decompressedExtension = Path.GetExtension(filepath);
                    if (decompressedExtension == ".dcx")
                        decompressedExtension = Path.GetExtension(Path.GetFileNameWithoutExtension(filepath));
                    if (validExtensions.Contains(decompressedExtension))
                    {
                        filepaths.Enqueue(filepath);
                    }
                }
            }

            threads = new Thread[numThreads];

            for (int i = 0; i < threads.Length; i++)
            {
                Thread thread = new Thread(() => UnpackTPFs(filepaths));
                threads[i] = thread;
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        private void UnpackTPFs(ConcurrentQueue<string> filepaths)
        {
            string filepath;
            while (filepaths.TryDequeue(out filepath))
            {
                // These are already full paths, but trust no one, not even yourself
                string absolute = Path.GetFullPath(filepath);
                string relative = absolute.Substring(gameDir.Length + 1);

                byte[] bytes = File.ReadAllBytes(absolute);
                string extension = Path.GetExtension(absolute);
                string subpath = Path.GetDirectoryName(relative) + "\\" + Path.GetFileNameWithoutExtension(absolute);

                if (extension == ".dcx")
                {
                    bytes = DCX.Decompress(bytes);
                    extension = Path.GetExtension(Path.GetFileNameWithoutExtension(absolute));
                    subpath = subpath.Substring(0, subpath.Length - extension.Length);
                }

                switch (extension)
                {
                    case ".tpf":
                        TPF tpf = TPF.Read(bytes);
                        UnpackTPF(tpf, looseDir, subpath);                        
                        break;

                    case ".chrbnd":
                    case ".ffxbnd":
                    case ".fgbnd":
                    case ".objbnd":
                    case ".partsbnd":
                        BND3 bnd = BND3.Read(bytes);
                        foreach (var entry in bnd.Files)
                        {
                            string entryExtension = Path.GetExtension(entry.Name);
                            if (entryExtension == ".tpf")
                            {
                                TPF bndTPF = TPF.Read(entry.Bytes);
                                UnpackTPF(bndTPF, looseDir, subpath);
                            }
                        }
                        break;
                }
            }
        }

        private void UnpackTPF(TPF tpf, string baseDir, string subDir)
        {
            // parts\HR_F_0010 and parts\HR_F_0010_M have duplicate filenames in the same tpf
            // thx QLOC
            // not the case in PTDE but might as well leave this
            List<string> names = new List<string>();
            List<string> dupes = new List<string>();

            foreach (TPF.Texture tpfEntry in tpf.Textures)
            {
                if (names.Contains(tpfEntry.Name))
                    dupes.Add(tpfEntry.Name);
                else
                    names.Add(tpfEntry.Name);
            }

            for (int i = 0; i < tpf.Textures.Count; i++)
            {

                TPF.Texture tpfEntry = tpf.Textures[i];
                string name = tpfEntry.Name;
                if (dupes.Contains(name))
                    name += "_" + i;

                UnpackFile(tpfEntry, name, baseDir, subDir);
            }
        }

        private void UnpackFile(TPF.Texture tpfEntry, string name, string baseDir, string subDir)
        {
            string subPath = subDir + "\\" + name + ".dds";
            string ddsPath = baseDir + "\\" + subPath;

            if (!File.Exists(ddsPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ddsPath));
                File.WriteAllBytes(ddsPath, tpfEntry.Bytes);
            }
        }

        public void Repack(string[] directoriesToRepack, int numThreads, string gameDir, string looseDir)
        {
            ConcurrentQueue<string> filepaths = new ConcurrentQueue<string>();
            this.gameDir = gameDir;
            this.looseDir = looseDir;

            //Search directories for valid files
            for (int i = 0; i < directoriesToRepack.Length; i++)
            {
                foreach (string filepath in Directory.EnumerateFiles(directoriesToRepack[i], "*", SearchOption.AllDirectories))
                {
                    string decompressedExtension = Path.GetExtension(filepath);
                    if (decompressedExtension == ".dcx")
                        decompressedExtension = Path.GetExtension(Path.GetFileNameWithoutExtension(filepath));
                    if (validExtensions.Contains(decompressedExtension))
                    {
                        filepaths.Enqueue(filepath);
                    }
                }
            }

            threads = new Thread[numThreads];

            for (int i = 0; i < threads.Length; i++)
            {
                Thread thread = new Thread(() => RepackTPFs(filepaths));
                threads[i] = thread;
                thread.Start();
            }

            foreach (Thread thread in threads)
                thread.Join();
        }

        private void RepackTPFs(ConcurrentQueue<string> filepaths)
        {
            string filepath;
            while (filepaths.TryDequeue(out filepath))
            {
                // These are already full paths, but trust no one, not even yourself
                string absolute = Path.GetFullPath(filepath);
                string relative = absolute.Substring(gameDir.Length + 1);

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

                switch (extension)
                {
                    case ".tpf":
                        TPF tpf = TPF.Read(bytes);
                        repackTPF(tpf, looseDir, subpath);
                        byte[] tpfBytes = tpf.Write();
                        if (dcx)
                            tpfBytes = DCX.Compress(tpfBytes, DCX.Type.DarkSouls1);
                    break;

                    case ".chrbnd":
                    case ".ffxbnd":
                    case ".fgbnd":
                    case ".objbnd":
                    case ".partsbnd":
                        BND3 bnd = BND3.Read(bytes);
                        foreach (var entry in bnd.Files)
                        {
                            string entryExtension = Path.GetExtension(entry.Name);
                            if (entryExtension == ".tpf")
                            {
                                TPF bndTPF = TPF.Read(entry.Bytes);
                                repackTPF(bndTPF, looseDir, subpath);
                                entry.Bytes = bndTPF.Write();
                            }
                        }

                        byte[] bndBytes = bnd.Write();
                        if (dcx)
                        {
                            bndBytes = DCX.Compress(bndBytes, DCX.Type.DarkSouls1);
                        }

                        break;
                }
            }
        }

        private void repackTPF(TPF tpf, string baseDir, string subDir)
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

            for (int i = 0; i < tpf.Textures.Count; i++)
            {

                TPF.Texture tpfEntry = tpf.Textures[i];
                string name = tpfEntry.Name;
                if (dupes.Contains(name))
                    name += "_" + i;

                repackFile(tpfEntry, name, baseDir, subDir);
            }
        }

        private bool repackFile(TPF.Texture tpfEntry, string name, string baseDir, string subDir)
        {
            string inputPath = baseDir + "\\" + subDir + "\\" + name + ".dds";

            //DBGTEX_DETAIL crashes this, what in the heck
            if (inputPath != null && File.Exists(inputPath) && name != "DBGTEX_DETAIL")
            {
                byte[] inputBytes = File.ReadAllBytes(inputPath);

                DXGIFormat originalFormat = DDSFile.Read(new MemoryStream(tpfEntry.Bytes)).Format;

                DXGIFormat newFormat = DDSFile.Read(new MemoryStream(inputBytes)).Format;

                if (originalFormat != DXGIFormat.Unknown && newFormat != DXGIFormat.Unknown && originalFormat != newFormat)
                {
                    byte[] newBytes = convertFile(inputPath, originalFormat);
                    if (newBytes != null)
                        inputBytes = newBytes;
                }

                tpfEntry.Bytes = inputBytes;

                return true;
            }
            else
                return false;
        }


        private byte[] convertFile(string filepath, DXGIFormat format)
        {
            if (!File.Exists(TEXCONV_PATH))
            {
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
                catch (Exception ex)
                {
                    
                }
            }

            return result;
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
