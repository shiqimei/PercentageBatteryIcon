using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace percentage
{
    class PTConfig
    {
        private string _cfgFile = null;


        public string FontSize { get; set; }
        public string XOffset { get; set; }
        public string YOffset { get; set; }
        public string NormalColor { get; set; }
        public string ChargingColor { get; set; }
        public string LowColor { get; set; }
        public string AutoHide { get; set; }


        public PTConfig() => _cfgFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                        + "/ptcfg.xml";

        public bool ConfigExist()
        {
            if (_cfgFile != null && File.Exists(_cfgFile))
            {
                try
                {
                    XDocument.Load(_cfgFile);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void InitConfig()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("configs");
            root.Add(new XElement("fontsize") { Value = "28" });
            root.Add(new XElement("xoffset") { Value = "0" });
            root.Add(new XElement("yoffset") { Value = "0" });
            root.Add(new XElement("normalColor") { Value = "255,255,255" });
            root.Add(new XElement("chargingColor") { Value = "254,190,4" });
            root.Add(new XElement("lowColor") { Value = "254,97,82" });
            root.Add(new XElement("autoHide") { Value = "false" });
            doc.Add(root);
            doc.Save(_cfgFile);
        }

        public void Load()
        {
            if (!ConfigExist())
            {
                InitConfig();
            }
            XDocument doc = XDocument.Load(_cfgFile);
            XElement root = doc.Root;
            FontSize = root.Element("fontsize").Value;
            XOffset = root.Element("xoffset").Value;
            YOffset = root.Element("yoffset").Value;
            NormalColor = root.Element("normalColor").Value;
            ChargingColor = root.Element("chargingColor").Value;
            LowColor = root.Element("lowColor").Value;
            AutoHide = root.Element("autoHide").Value;
        }

        public void Update()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("configs");
            root.Add(new XElement("fontsize") { Value = FontSize });
            root.Add(new XElement("xoffset") { Value = XOffset });
            root.Add(new XElement("yoffset") { Value = YOffset });
            root.Add(new XElement("normalColor") { Value = NormalColor });
            root.Add(new XElement("chargingColor") { Value = ChargingColor });
            root.Add(new XElement("lowColor") { Value = LowColor });
            root.Add(new XElement("autoHide") { Value = AutoHide });
            doc.Add(root);
            doc.Save(_cfgFile);
        }
    }
}
