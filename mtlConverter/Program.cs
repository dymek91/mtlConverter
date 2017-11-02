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
        static void Main(string[] args)
        {
            if (Functions.ProcessLayersLibrary())
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
                                Functions.decodeMtl(path);
                                Functions.replaceSurfaceType(path);
                                Functions.ConvertLayerBlend(path);

                                break;
                            default:
                                break;
                        }
                    }
                    else if (path.Length > 0 && Directory.Exists(path))
                    {
                        string[] filesnames = Functions.GetFiles(path);
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
                                        Functions.replaceSurfaceType(path2);
                                        Functions.ConvertLayerBlend(path2);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("DONE");
            }
            Console.ReadLine();
        }
    }
}
