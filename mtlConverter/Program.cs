using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;


namespace mtlConverter
{
    class Program
    {
        public static bool copy(string oldPath, string newPath)
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
        static String[] GetFiles(String path)
        {
            String[] filenames = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return filenames;
        }
        static void decodeMtl(string path)
        {
            string dir = Directory.GetCurrentDirectory();

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
                copy(pathDir + pathFile + ".raw", pathDir + pathFile + pathExt);
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
        static void replaceSurfaceType(string path)
        {

            try
            {
                string text = File.ReadAllText(path);
                text = text.Replace("some text", "new value");
                //dont use verts colors
                text = text.Replace("%VERTCOLORS", "");

                string input = text;
                string pattern = "SurfaceType=\"plastic(.*?)\"";
                string replacement = "SurfaceType=\"mat_plastic\"";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(input, replacement);

                input = result;
                pattern = "SurfaceType=\"metal(.*?)\"";
                replacement = "SurfaceType=\"mat_metal\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "SurfaceType=\"glass(.*?)\"";
                replacement = "SurfaceType=\"mat_glass\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "SurfaceType=\"fabric\"";
                replacement = "SurfaceType=\"mat_fabric\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "SurfaceType=\"rubber(.*?)\"";
                replacement = "SurfaceType=\"mat_rubber\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "SurfaceType=\"shield_human\"";
                replacement = "SurfaceType=\"mat_metal\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "Shader=\"HologramCIG\"";
                replacement = "Shader=\"Illum\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                ////////////////////////////////////

                /*input = result;
                pattern = "GenMask=\"8012010000800\"";//decal + normalmap
                replacement = "GenMask=\"20E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"1A018000000\"";//normalmap + pom
                replacement = "GenMask=\"80E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"1A010000040\"";//blendlayer + normalmap
                replacement = "GenMask=\"E0111\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"8012010000040\"";//blendlayer + normalmap
                replacement = "GenMask=\"E0111\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"801A010000040\"";//blendlayer + normalmap
                replacement = "GenMask=\"E0111\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"12010004004\"";//detail in alpha + detailmap + normalmap
                replacement = "GenMask=\"8E4001\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"30400001\"";//normalmap
                replacement = "GenMask=\"E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"1A010000800\"";//decal + normalmap
                replacement = "GenMask=\"20E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"801A018000800\"";//decal + normalmap + pom
                replacement = "GenMask=\"A0E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"8012000000800\"";//decal
                replacement = "GenMask=\"20C0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"8010000000800\"";//decal
                replacement = "GenMask=\"20C0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"2030400801\"";//decal + normalmap
                replacement = "GenMask=\"20E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"18018000000\"";//pom + normalmap
                replacement = "GenMask=\"80E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "GenMask=\"10010000000\"";//normalmap
                replacement = "GenMask=\"E0011\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);MtlFlags="524448"
                */

                /*
                input = result;
                pattern = " GenMask=\"(.*?)\"";
                replacement = "";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "Material MtlFlags=\"256\"";
                replacement = "Material MtlFlags=\"524544\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "MtlFlags=\"128\"";
                replacement = "MtlFlags=\"524416\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "MtlFlags=\"160\"";
                replacement = "MtlFlags=\"524448\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "MtlFlags=\"130\"";
                replacement = "MtlFlags=\"524418\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "MtlFlags=\"536871072\"";
                replacement = "MtlFlags=\"524448\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);
                */

                /////////////////////////////////

                input = result;
                pattern = "DecalFalloff=\"9.9999997e-005\"";
                replacement = "DecalFalloff=\"1\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "Shader=\"Illum\"";
                replacement = "Shader=\"IllumCIG\"";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                /*input = result;
                pattern = "%VERTCOLORS";
                replacement = "";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "%USE_DAMAGE_MAP";
                replacement = "";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "%DECAL_OPACITY_MAP"; 
                replacement = "";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "%ENVIRONMENT_MAP";
                replacement = "";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);

                input = result;
                pattern = "%BLUR_REFRACTION";
                replacement = "";
                rgx = new Regex(pattern);
                result = rgx.Replace(input, replacement);
                */

                //input = result;
                //pattern = "Shader=\"IllumCIG\"(.*)StringGenMask=\"([^\"]+)\"";
                //rgx = new Regex(pattern);
                //result = rgx.Replace(input, m => StringGenMask2GenMask("IllumCIG", m.Groups[0].Value));

                //input = result;
                //pattern = "StringGenMask";
                //replacement = "GenMask";
                //rgx = new Regex(pattern);
                //result = rgx.Replace(input, replacement);

                File.WriteAllText(path, result);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

        }
        static string StringGenMask2GenMask(string shader, string s)
        {
            string pattern = "Shader=\"" + shader + "\"(.*)StringGenMask=\"(.*)\"";
            string conector = Regex.Replace(s, pattern, "$1");
            string stringGenMask = Regex.Replace(s, pattern, "$2");
            string[] genMasks = stringGenMask.Split('%');
            int genMask = 0;//0xC0000;
            foreach (string smask in genMasks)
            {
                switch (smask)
                {
                    case "NORMAL_MAP":
                        genMask = genMask + 0x1;
                        break;
                    case "SPECULAR_MAP":
                        genMask = genMask + 0x10;
                        break;
                    case "DETAIL_MAPPING":
                        genMask = genMask + 0x4000;
                        break;
                    case "OFFSET_BUMP_MAPPING":
                        genMask = genMask + 0x20000;
                        break;
                    case "VERTCOLORS":
                        //genMask = genMask + 0x400000;
                        break;
                    case "DECAL":
                        genMask = genMask + 0x2000000;
                        break;
                    case "PARALLAX_OCCLUSION_MAPPING":
                        genMask = genMask + 0x8000000;
                        break;
                    case "DISPLACEMENT_MAPPING":
                        genMask = genMask + 0x10000000;
                        break;
                    case "PHONG_TESSELLATION":
                        genMask = genMask + 0x20000000;
                        break;
                    case "PN_TESSELLATION":
                        genMask = genMask + 0x40000000;
                        break;
                    case "DIRTLAYER":
                        genMask = genMask + 0x200000;
                        break;
                    case "BLENDLAYER":
                        genMask = genMask + 0x100;
                        break;
                    case "ALPHAMASK_DETAILMAP":
                        genMask = genMask + 0x800000;
                        break;
                    case "SILHOUETTE_PARALLAX_OCCLUSION_MAPPING":
                        genMask = genMask + 0x10000;
                        break;
                    case "ALLOW_SILHOUETTE_POM":
                        genMask = genMask + 0x40000;
                        break;
                    //case "SUBSURFACE_SCATTERING":
                    //    genMask = genMask + 0x80000;
                    //    break;

                    default:
                        break;
                }

            }
            //dont use verts colors
            stringGenMask = stringGenMask.Replace("VERTCOLORS", "");

            byte[] intBytes = BitConverter.GetBytes(genMask);
            Array.Reverse(intBytes);
            byte[] result = intBytes;
            string resultGenMask = BitConverter.ToString(result);
            return "Shader=\"" + shader + "\"" + conector + "GenMask=\"" + resultGenMask.Replace("-", "").TrimStart('0') + "\" StringGenMask=\"" + stringGenMask + "\"";
        }
        static void Main(string[] args)
        {
            foreach (string path in args)
            {
                if (path.Length > 0 && File.Exists(path))
                {
                    string extension = Path.GetExtension(path);
                    switch (extension)
                    {
                        case ".mtl":
                            Console.WriteLine("FILE {0}", path);
                            decodeMtl(path);
                            replaceSurfaceType(path);
                            Functions.ConvertLayerBlend(path);

                            break;
                        default:
                            break;
                    }
                }
                else if (path.Length > 0 && Directory.Exists(path))
                {
                    string[] filesnames = GetFiles(path);
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
                                    decodeMtl(path2);
                                    replaceSurfaceType(path2);
                                    Functions.ConvertLayerBlend(path2);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            //Console.ReadLine();
        }
    }
}
