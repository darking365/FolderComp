using System;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Win32;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
namespace FolderCompare
{
    public class Util
    {
        /// <summary>
        /// JSON序列化
        /// </summary>
        public static string JsonSerializer<T>(T t)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// XML序列化某一类型到指定的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        public static void SerializeToXml<T>(string filePath, T obj)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static T DeserializeFile<T>(string filePath)
        {
            T result = default(T);
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    result = (T)xmlSerializer.Deserialize(reader);
                }
            }
            return result;
        }

        ///<summary> 
        ///get value from config file
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string strKey)
        {
            string value = string.Empty;
            try
            {
                string file = System.Windows.Forms.Application.ExecutablePath;
                Configuration config = ConfigurationManager.OpenExeConfiguration(file);
                foreach (string key in config.AppSettings.Settings.AllKeys)
                {
                    if (key == strKey)
                    {
                        value = config.AppSettings.Settings[strKey].Value.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.Logger();
            }
            return value;
        }

        ///<summary>  
        ///update the config file  
        ///</summary>  
        ///<param name="newKey"></param>  
        ///<param name="newValue"></param>  
        public static void UpdateAppConfig(string newKey, string newValue)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            try
            {
                bool exist = false;
                foreach (string key in config.AppSettings.Settings.AllKeys)
                {
                    if (key == newKey)
                    {
                        exist = true;
                    }
                }
                if (exist)
                {
                    config.AppSettings.Settings.Remove(newKey);
                }
                config.AppSettings.Settings.Add(newKey, newValue);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                ex.Message.Logger();
            }
        }

        /// <summary>
        /// get the Beyond Compare install path
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetAppPath(string appName)
        {
            string appPath = string.Empty;
            try
            {
                string softPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
                RegistryKey regKey = Registry.LocalMachine;
                RegistryKey regSubKey = regKey.OpenSubKey($"{softPath}\\{appName}.exe");
                if (regSubKey == null)
                {
                    regKey = Registry.CurrentUser;
                    regSubKey = regKey.OpenSubKey($"{softPath}\\{appName}.exe");
                    if (regSubKey == null) return appPath;
                }
                var val = regSubKey.GetValue("");
                if (val != null)
                {
                    appPath = Path.GetDirectoryName(val.ToString());
                }

            }
            catch (Exception ex)
            {
                ex.Message.Logger();
            }
            return appPath;
        }

        /// <summary>
        /// export the data
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Export(List<Level> lst, string fileName)
        {
            bool isSuccess = false;
            XSSFWorkbook workBook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workBook.CreateSheet();
            try
            {
                //add header style
                ICellStyle cellStyle = workBook.CreateCellStyle();
                cellStyle.Alignment = HorizontalAlignment.Center;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                IFont font = workBook.CreateFont();
                font.Boldweight = (Int16)FontBoldWeight.Bold;
                font.FontHeightInPoints = 12;
                cellStyle.SetFont(font);
                //header settings
                IRow header = sheet.CreateRow(0);
                header.HeightInPoints = 35;
                List<string> headers = new List<string> { "Left Folder", "Right Folder" };
                for (int m = 0; m < headers.Count; m++)
                {
                    ICell cell = header.CreateCell(m * 3);
                    cell.SetCellValue(headers[m]);
                    cell.CellStyle = cellStyle;
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, m * 3, m * 3 + 2));
                }

                IRow title = sheet.CreateRow(1);
                title.HeightInPoints = 20;
                cellStyle.Alignment = HorizontalAlignment.Left;
                List<string> titles = new List<string> { "Path", "Filename","Size(Byte)", "Path", "FileName","Size(Byte)", "Comparision Result" };
                for (int j = 0; j < titles.Count; j++)
                {
                    ICell cell = title.CreateCell(j);
                    cell.SetCellValue(titles[j]);
                    cell.CellStyle = cellStyle;
                }
                #region body
                for (int i = 0; i < lst.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                    IRow body = sheet.CreateRow(i + 2);
                    string lpath = string.Empty;
                    string lfile = "NA";
                    string lsize = "NA";
                    string rpath = string.Empty;
                    string rfile = "NA";
                    string rsize = "NA";
                    string result = "NA";

                    if (lst[i].LeftNode != null)
                    {
                        lpath = lst[i].LeftNode.Path;
                        lfile = lst[i].LeftNode.FileName;
                        result = lst[i].LeftNode.Result;
                        lsize = lst[i].LeftNode.Size;
                    }
                    else
                    {
                        lpath = lst[i].LtPath;
                    }
                    if (lst[i].RightNode != null)
                    {
                        rpath = lst[i].RightNode.Path;
                        rfile = lst[i].RightNode.FileName;
                        result = lst[i].RightNode.Result;
                        rsize = lst[i].RightNode.Size;
                    }
                    else
                    {
                        rpath = lst[i].RtPath;
                    }
                    switch (result)
                    {
                        case "ltonly":
                            result = $"Only exists in {lst[i].LeftNode.Path}";
                            break;
                        case "rtonly":
                            result = $"Only exists in {lst[i].RightNode.Path}";
                            break;
                        case "same":
                            result = "same";
                            break;
                        default:
                            result = "Text files are different";
                            break;
                    }
                    body.CreateCell(0).SetCellValue(lpath);
                    body.CreateCell(1).SetCellValue(lfile);
                    body.CreateCell(2).SetCellValue(lsize);
                    body.CreateCell(3).SetCellValue(rpath);
                    body.CreateCell(4).SetCellValue(rfile);
                    body.CreateCell(5).SetCellValue(rsize);
                    body.CreateCell(6).SetCellValue(result);
                }
                #endregion

                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    workBook.Write(fs);
                    workBook.Close();
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                ex.Message.Logger();
                workBook.Close();
            }
            return isSuccess;
        }

        public static List<Level> GetResult(bcreport report)
        {
            List<Level> lst = new List<Level>();
            int layer = 0;
            string preLtPath = string.Empty;
            string preRtPath = string.Empty;
            
            return ProcessFolderComp(report.foldercomps, lst, layer, preLtPath, preRtPath);
        }

        private static List<Level> ProcessFolderComp(List<foldercomp> folders, List<Level> lst, int layer, string preLtPath, string preRtPath)
        {
            foreach (var fc in folders)
            {
                layer++;
                preLtPath = layer == 1 ? fc.ltpath : Path.Combine(preLtPath, fc.lt != null ? fc.lt.name : "");
                preRtPath = layer == 1 ? fc.rtpath : Path.Combine(preRtPath, fc.rt != null ? fc.rt.name : "");

                foreach (var f in fc.filecomps)
                {
                    Level level = new Level() { Layer = layer, LtPath = preLtPath, RtPath = preRtPath };
                    if (f.lt != null)
                    {
                        Node lnode = new Node
                        {
                            Path = preLtPath,
                            FileName = f.lt.name,
                            Result = f.status,
                            Size=f.lt.size
                        };
                        level.LeftNode = lnode;
                    }
                    if (f.rt != null)
                    {
                        Node rnode = new Node
                        {
                            Path = preRtPath,
                            FileName = f.rt.name,
                            Result = f.status,
                            Size=f.rt.size
                        };
                        level.RightNode = rnode;
                    }
                    lst.Add(level);
                }

                ProcessFolderComp(fc.foldercomps, lst, layer, preLtPath, preRtPath);
            }

            return lst;
        }
    }

    public static class StringExtention
    {
        public static bool IsValid(this string value)
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(value.Trim()) && Directory.Exists(value))
            {
                isValid = true;
            }
            return isValid;
        }

        public static async void Logger(this string msg)
        {
            string log = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{DateTime.Now.ToString("yyyyMMddHH")}.log");
            using (FileStream fs = new FileStream(log, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    await sw.WriteLineAsync($"{DateTime.Now.ToString("yyyyMMddHHmmss")},info:{msg}");
                }
            }
        }
    }

    [Serializable]
    [XmlRoot("bcreport")]
    public class bcreport
    {
        [XmlAttribute("created")]
        public string created { get; set; }
        [XmlElement("foldercomp")]
        public List<foldercomp> foldercomps { get; set; }
    }

    public class foldercomp
    {
        [XmlElement("ltpath")]
        public string ltpath { get; set; }
        [XmlElement("rtpath")]
        public string rtpath { get; set; }
        [XmlElement("filters")]
        public string filters { get; set; }
        [XmlElement("foldercomp")]
        public List<foldercomp> foldercomps { get; set; }
        [XmlElement("filecomp")]
        public List<filecomp> filecomps { get; set; }
        [XmlElement("lt")]
        public lt lt { get; set; }
        [XmlElement("rt")]
        public rt rt { get; set; }
    }

    public class filecomp
    {
        [XmlAttribute("status")]
        public string status { get; set; }
        [XmlElement("lt")]
        public lt lt { get; set; }
        [XmlElement("rt")]
        public rt rt { get; set; }
    }

    public class FileModel
    {
        [XmlElement("name")]
        public string name { get; set; }
        [XmlElement("size")]
        public string size { get; set; }
        [XmlElement("modified")]
        public string modified { get; set; }
        [XmlElement("attr")]
        public string attr { get; set; }
    }

    public class lt : FileModel
    {

    }

    public class rt : FileModel
    {

    }

    public class Level
    {
        public int Layer { get; set; }
        public string RtPath { get; set; }
        public string LtPath { get; set; }

        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }
    }

    public class Node
    {
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Result { get; set; }
        public string Size { get; set; }
    }
}
