using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace mtlConverter
{
    class Functions
    {
        public static void ConvertLayerBlend(String path)
        {
            try
            {
                if (File.Exists(path))
                {
                    XDocument xml = XDocument.Load(path);

                    if (xml != null)
                    {
                        xml = CreateXML(xml);
                        xml.Save(path);

                    }
                    else
                    {
                        Console.WriteLine("{0} already in XML format", path);
                        // Console.Read();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting {0}: {1}", path, ex.Message);
                // Console.Read();
            }
        }
        private static XDocument CreateXML(XDocument xml)
        {
            IEnumerable<XAttribute> rootAttributes = xml.Root.Attributes();
            XDocument xmlDocument = new XDocument();
            XElement xmlDoc = new XElement("Material");
            foreach (XAttribute at in rootAttributes)
            {
                xmlDoc.Add(at);
            }
            xmlDoc.Attribute("MtlFlags").Value = "524544";
            XElement subMaterials = new XElement("SubMaterials");

            foreach (XElement el in xml.Descendants("SubMaterials").Descendants("Material"))
            {
                //string mes = el.Element("Material").Attribute("Name").Value;

                IEnumerable<XAttribute> matAttributes = el.Attributes();
                IEnumerable<XElement> matElements = el.Elements();

                el.Attribute("SurfaceType").Value = "";
                if (el.Attribute("Shader").Value == "Illum")//IllumCIG
                {
                    el.Attribute("Shader").Value = "IllumCIG";
                }

                //Console.WriteLine("/////////////////////////////");
                //Console.WriteLine(el.Attribute("Shader").Value);
                if (el.Attribute("Shader").Value == "LayerBlend")
                {
                    XElement material = new XElement("Material");
                    foreach (XAttribute at in el.Attributes()) material.Add(at);
                    try
                    {
                        //material.Add(new XAttribute("GenMask", "5000402"));
                    }
                    catch (Exception ex)
                    {
                        //material.Attribute("GenMask").Value = "5000402";
                    }
                    //material.Attribute("StringGenMask").Value = "%BLENDLAYER%DETAIL_MAPPING%HIGHRES_LAYERS%NORMAL_MAP";
                    material.Attribute("StringGenMask").Value = "%HIGHRES_LAYERS";
                    try
                    {
                        material.Attribute("GenMask").Value = "1000000";
                    }
                    catch (Exception ex)
                    {
                        material.Add(new XAttribute("GenMask", "1000000"));
                    }
                    material.Attribute("MtlFlags").Value = "524416";
                    XElement publicparams = new XElement("PublicParams");
                    foreach (XAttribute at in el.Descendants("PublicParams").Attributes()) publicparams.Add(at);
                    XElement textures = new XElement("Textures");
                    int editGenMask = 1000000;
                    string editStringGenMask = "%HIGHRES_LAYERS";
                    foreach (XElement elt in el.Descendants("Texture"))
                    {
                        XElement texture = new XElement("Texture");
                        foreach (XAttribute at in elt.Attributes())
                        {
                            if ((at.Name == "Map") && (at.Value == "[1] Custom")) texture.Add(new XAttribute("Map", "Detail"));
                            else texture.Add(at);//%BLENDLAYER%DETAIL_MAPPING%HIGHRES_LAYERS%NORMAL_MAP
                            if ((at.Name == "Map") && (at.Value == "Opacity")) { editGenMask = editGenMask + 402; editStringGenMask = editStringGenMask + "%BLENDLAYER%NORMAL_MAP"; }//material.Attribute("GenMask").Value = "1000402";
                            if ((at.Name == "Map") && (at.Value == "Custom")) { editGenMask = editGenMask + 4000000; editStringGenMask = editStringGenMask + "%DETAIL_MAPPING"; }// material.Attribute("GenMask").Value = "5000402";
                        }
                        textures.Add(texture);
                    }
                    material.Attribute("GenMask").Value = editGenMask.ToString();
                    material.Attribute("StringGenMask").Value = editStringGenMask;
                    publicparams.Add(new XAttribute("WearDirtBlendFalloff", "1"));
                    foreach (XElement elt in el.Descendants("MatRef"))
                    {
                        foreach (XAttribute at in elt.Attributes())
                        {
                            if ((at.Name == "Slot") && (at.Value == "0"))
                            {
                                XElement texture = new XElement("Texture");
                                texture.Add(new XAttribute("Map", "Diffuse"));
                                string layerPath = at.NextAttribute.Value;
                                texture.Add(new XAttribute("File", GetLayerDiffuseMap(layerPath)));
                                textures.Add(texture);

                                texture = new XElement("Texture");
                                XAttribute bumpAt = new XAttribute("Map", "Bumpmap");
                                string bumpmapPath = GetLayerBumpmap(layerPath);
                                if (bumpmapPath != "null")
                                {
                                    texture.Add(bumpAt);
                                    texture.Add(new XAttribute("File", bumpmapPath));
                                }
                                textures.Add(texture);
                                publicparams.Add(new XAttribute("Diff1", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("Spec1", GetLayerSpec(layerPath)));
                                publicparams.Add(new XAttribute("Smooth1", GetLayerSmooth(layerPath)));
                            }
                            if ((at.Name == "Slot") && (at.Value == "1"))
                            {
                                XElement texture = new XElement("Texture");
                                texture.Add(new XAttribute("Map", "SubSurface"));
                                string layerPath = at.NextAttribute.Value;
                                texture.Add(new XAttribute("File", GetLayerDiffuseMap(layerPath)));
                                textures.Add(texture);

                                texture = new XElement("Texture");
                                XAttribute bumpAt = new XAttribute("Map", "Specular");
                                string bumpmapPath = GetLayerBumpmap(layerPath);
                                if (bumpmapPath != "null")
                                {
                                    texture.Add(bumpAt);
                                    texture.Add(new XAttribute("File", bumpmapPath));
                                }
                                textures.Add(texture);
                                publicparams.Add(new XAttribute("Diff2", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("Spec2", GetLayerSpec(layerPath)));
                                publicparams.Add(new XAttribute("Smooth2", GetLayerSmooth(layerPath)));
                            }
                            if ((at.Name == "Slot") && (at.Value == "2"))
                            {
                                XElement texture = new XElement("Texture");
                                texture.Add(new XAttribute("Map", "[1] Custom"));
                                string layerPath = at.NextAttribute.Value;
                                texture.Add(new XAttribute("File", GetLayerDiffuseMap(layerPath)));
                                textures.Add(texture);

                                texture = new XElement("Texture");
                                XAttribute bumpAt = new XAttribute("Map", "Heightmap");
                                string bumpmapPath = GetLayerBumpmap(layerPath);
                                if (bumpmapPath != "null")
                                {
                                    texture.Add(bumpAt);
                                    texture.Add(new XAttribute("File", bumpmapPath));
                                }
                                textures.Add(texture);
                                publicparams.Add(new XAttribute("Diff3", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("Spec3", GetLayerSpec(layerPath)));
                                publicparams.Add(new XAttribute("Smooth3", GetLayerSmooth(layerPath)));
                            }
                            if ((at.Name == "Slot") && (at.Value == "3"))
                            {
                                XElement texture = new XElement("Texture");
                                texture.Add(new XAttribute("Map", "Emittance"));
                                string layerPath = at.NextAttribute.Value;
                                texture.Add(new XAttribute("File", GetLayerDiffuseMap(layerPath)));
                                textures.Add(texture);

                                texture = new XElement("Texture");
                                XAttribute bumpAt = new XAttribute("Map", "Decal");
                                string bumpmapPath = GetLayerBumpmap(layerPath);
                                if (bumpmapPath != "null")
                                {
                                    texture.Add(bumpAt);
                                    texture.Add(new XAttribute("File", bumpmapPath));
                                }
                                textures.Add(texture);
                                publicparams.Add(new XAttribute("Diff4", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("Spec4", GetLayerSpec(layerPath)));
                                publicparams.Add(new XAttribute("Smooth4", GetLayerSmooth(layerPath)));
                            }
                            //wear layers
                            if ((at.Name == "Slot") && (at.Value == "4"))
                            {
                                string layerPath = at.NextAttribute.Value;
                                publicparams.Add(new XAttribute("WearDiff1", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("WearSpec1", GetLayerSpec(layerPath)));
                                publicparams.Attribute("WearDirtBlendFalloff").Value = GetLayerFalloff(layerPath);
                            }
                            if ((at.Name == "Slot") && (at.Value == "5"))
                            {
                                string layerPath = at.NextAttribute.Value;
                                publicparams.Add(new XAttribute("WearDiff2", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("WearSpec2", GetLayerSpec(layerPath)));
                                // publicparams.Add(new XAttribute("WearDirtBlendFalloff", GetLayerFalloff(layerPath)));
                            }
                            if ((at.Name == "Slot") && (at.Value == "6"))
                            {
                                string layerPath = at.NextAttribute.Value;
                                publicparams.Add(new XAttribute("WearDiff3", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("WearSpec3", GetLayerSpec(layerPath)));
                                //publicparams.Add(new XAttribute("WearDirtBlendFalloff", GetLayerFalloff(layerPath)));
                            }
                            if ((at.Name == "Slot") && (at.Value == "7"))
                            {
                                string layerPath = at.NextAttribute.Value;
                                publicparams.Add(new XAttribute("WearDiff4", GetLayerDiff(layerPath)));
                                publicparams.Add(new XAttribute("WearSpec4", GetLayerSpec(layerPath)));
                                //publicparams.Add(new XAttribute("WearDirtBlendFalloff", GetLayerFalloff(layerPath)));
                            }
                        }

                    }
                    publicparams.Add(new XAttribute("WearScale", "3"));
                    publicparams.Add(new XAttribute("TearScale", "3"));

                    material.Add(textures);
                    material.Add(publicparams);
                    subMaterials.Add(material);
                }
                else
                {
                    subMaterials.Add(el);
                }

                //Console.WriteLine("/////////////////////////////");
                //foreach (XAttribute at in matAttributes)
                //{
                //    Console.WriteLine(at);
                //}
                //foreach (XElement at in matElements)
                //{
                //    Console.WriteLine(at);
                //}
            }
            xmlDoc.Add(subMaterials);
            xmlDocument.Add(xmlDoc);
            //Console.Write(xmlDocument);
            //Console.Read();
            return xmlDocument;
        }

        private static string GetLayerDiffuseMap(string path)
        {
            XDocument xml = XDocument.Load("D:/Programy/Crytek/CRYENGINE Launcher/Crytek/gamesdk_5.3/GameSDK/Assets/" + path);
            foreach (XElement el in xml.Descendants("Texture"))
            {
                foreach (XAttribute at in el.Attributes())
                {
                    if ((at.Name == "Map") && (at.Value == "Diffuse")) return at.NextAttribute.Value;
                }
            }
            return "textures/colors/white.dds";
        }
        private static string GetLayerBumpmap(string path)
        {
            XDocument xml = XDocument.Load("D:/Programy/Crytek/CRYENGINE Launcher/Crytek/gamesdk_5.3/GameSDK/Assets/" + path);
            foreach (XElement el in xml.Descendants("Texture"))
            {
                foreach (XAttribute at in el.Attributes())
                {
                    if ((at.Name == "Map") && (at.Value == "Bumpmap")) return at.NextAttribute.Value;
                }
            }
            return "null";
        }
        private static string GetLayerDiff(string path)
        {
            XDocument xml = XDocument.Load("D:/Programy/Crytek/CRYENGINE Launcher/Crytek/gamesdk_5.3/GameSDK/Assets/" + path);
            IEnumerable<XAttribute> rootAttributes = xml.Root.Attributes();
            foreach (XAttribute at in rootAttributes)
            {
                if (at.Name == "Diffuse") return at.Value;
            }

            return "null";
        }
        private static string GetLayerSpec(string path)
        {
            XDocument xml = XDocument.Load("D:/Programy/Crytek/CRYENGINE Launcher/Crytek/gamesdk_5.3/GameSDK/Assets/" + path);
            IEnumerable<XAttribute> rootAttributes = xml.Root.Attributes();
            foreach (XAttribute at in rootAttributes)
            {
                if (at.Name == "Specular") return at.Value;
            }

            return "null";
        }
        private static string GetLayerFalloff(string path)
        {
            XDocument xml = XDocument.Load("D:/Programy/Crytek/CRYENGINE Launcher/Crytek/gamesdk_5.3/GameSDK/Assets/" + path);
            IEnumerable<XAttribute> rootAttributes = xml.Root.Attributes();
            foreach (XAttribute at in xml.Root.Descendants("PublicParams").Attributes())
            {
                if (at.Name == "WearDirtBlendFalloff") return at.Value;
            }

            return "1";
        }
        private static string GetLayerSmooth(string path)
        {
            XDocument xml = XDocument.Load("D:/Programy/Crytek/CRYENGINE Launcher/Crytek/gamesdk_5.3/GameSDK/Assets/" + path);
            IEnumerable<XAttribute> rootAttributes = xml.Root.Attributes();
            foreach (XAttribute at in rootAttributes)
            {
                string val = at.Value;
                if (at.Name == "Shininess") return ((float.Parse(val)) / 255).ToString();
            }

            return "null";
        }
    }
}
