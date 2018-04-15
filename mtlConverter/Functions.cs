using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace mtlConverter
{
    class Functions
    {
        static string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        public static void decodeMtl(string path)
        {
            string dir = Directory.GetCurrentDirectory();

            if (Functions.checkIfCryXml(path))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo("HoloXPLOR.DataForge.exe", "\"" + path + "\"")
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };
                    var process = Process.Start(psi);
                    process.WaitForExit();
                    string pathDir = Path.GetDirectoryName(path) + "\\";
                    string pathFile = Path.GetFileNameWithoutExtension(path);
                    string pathExt = Path.GetExtension(path);
                    if (File.Exists(pathDir + pathFile + ".raw")) File.Delete(path);
                    Functions.Copy(pathDir + pathFile + ".raw", pathDir + pathFile + pathExt);
                    File.Delete(pathDir + pathFile + ".raw");
                }
                catch (Exception e)
                {
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("HoloXPLOR.DataForge.exe not found");
                    Console.WriteLine("Get it here: https://github.com/dolkensp/HoloXPLOR/releases");
                    Console.WriteLine("and put in same directory as mtlConverter.exe");
                    Console.Read();
                    throw;
                }
            }



        }
        public static bool ProcessLayersLibrary()
        {
            bool processed = false;
            if (!Directory.Exists(currentDir + "Materials"))
            {
                Console.WriteLine("Materials directory not found.");
                Console.WriteLine("Copy Materials directory from DataXML.pak into folder where mtlConverter is located");
                return false;
            }
            if (!checkIfDecoded(currentDir + "Materials/alpha.mtl"))
            {
                Console.WriteLine("no decoded");

                string[] filesnames = Functions.GetFiles(currentDir + "Materials");
                Console.WriteLine("Found {0} files", filesnames.Count());
                int count = 0;
                foreach (string path2 in filesnames)
                {
                    count++;
                    if (path2.Length > 0 && File.Exists(path2))
                    {
                        Console.WriteLine("[{0}/{1}]", count, filesnames.Count());
                        string extension = Path.GetExtension(path2);
                        switch (extension)
                        {
                            case ".mtl":
                                Console.WriteLine("FILE {0}", path2);
                                Functions.decodeMtl(path2);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (checkIfDecoded(currentDir + "Materials/alpha.mtl")) processed = true;
            }
            else
            {
                processed = true;
            }

            return processed;
        }
        static bool checkIfDecoded(string path)
        {
            bool decoded = false;
            BinaryReader br = new BinaryReader(File.Open(
                    path, FileMode.Open, FileAccess.Read));
            string fileLabel = new string(br.ReadChars(6));
            if (fileLabel != "CryXml") decoded = true;
            br.Close();

            return decoded;
        }
        public static bool Copy(string oldPath, string newPath)
        {
            FileStream input = null;
            FileStream output = null;
            try
            {
                input = new FileStream(oldPath, FileMode.Open);
                output = new FileStream(newPath, FileMode.Create, FileAccess.ReadWrite);

                byte[] buffer = new byte[32768];
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }
                input.Close();
                input.Dispose();
                output.Close();
                output.Dispose();
                return true;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return false;
            }
            finally
            {

            }
        }
        public static String[] GetFiles(String path)
        {
            String[] filenames = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return filenames;
        }
        public static bool checkIfCryXml(string path)
        {
            bool isCryXml = false;
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                string fileSignature = new string(br.ReadChars(6));
                if (fileSignature == "CryXml")
                {
                    isCryXml = true;
                }
                br.Close();
            }
            return isCryXml;
        }
    }
}
