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
        /// <param name="lFolder"></param>
        /// <param name="rFolder"></param>
        /// <returns></returns>
        public static bool Export(List<Level> lst, string fileName,string lFolder,string rFolder)
        {
            bool isSuccess = false;
            XSSFWorkbook workBook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workBook.CreateSheet();
            try
            {
                //add header style
                ICellStyle cellStyle = workBook.CreateCellStyle();
                cellStyle.Alignment = HorizontalAlignment.Left;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                IFont font = workBook.CreateFont();
                font.Boldweight = (short)FontBoldWeight.Bold;
                font.FontHeightInPoints = 12;
                font.FontName = "Arial Unicode MS";
                cellStyle.SetFont(font);
                sheet.SetColumnWidth(0, 256);
                #region header settings
                IRow first = sheet.CreateRow(0);
                ICell fst_cell = first.CreateCell(0);
                fst_cell.SetCellValue("Quarterly Source Code Comparison and Retrofit ");
                fst_cell.CellStyle = cellStyle;

                IRow second = sheet.CreateRow(1);

                IRow third = sheet.CreateRow(2);
                third.CreateCell(0).SetCellValue("Date:");

                string[,] arr = new string[2, 4] { { "GIT Version:", "", "Source Path(L):", lFolder }, { "Production Version:", "", "Source Path(R):", rFolder } };
                for (int i = 0; i < 2; i++)
                {
                    IRow forth_fifth = sheet.CreateRow(i + 3);
                    for (int j = 0; j < 4; j++)
                    {
                        forth_fifth.CreateCell(j).SetCellValue(arr[i, j]);
                    }
                }

                IRow sixth = sheet.CreateRow(5);

                IRow header = sheet.CreateRow(6);
                header.HeightInPoints = 12;
                List<string> headers = new List<string> { "Production Version", "GIT Version","" };
                ICellStyle cs = workBook.CreateCellStyle();
                cs.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                cs.FillPattern = FillPattern.SolidForeground;
                cs.Alignment = HorizontalAlignment.Center;
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.SetFont(font);
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;

                for (int m = 0; m < headers.Count; m++)
                {
                    CellRangeAddress region = new CellRangeAddress(6, 6, m * 3, m * 3 + 2);
                    sheet.AddMergedRegion(region);
                    for (int n = 0; n < 3; n++)
                    {
                        ICell cell = header.CreateCell(3 * m + n);
                        cell.SetCellValue(headers[m]);
                        cell.CellStyle = cs;
                    }
                }

                IRow title = sheet.CreateRow(7);
                title.HeightInPoints = 12;
                ICellStyle titleStyle = workBook.CreateCellStyle();
                titleStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                titleStyle.FillPattern = FillPattern.SolidForeground;
                titleStyle.Alignment = HorizontalAlignment.Left;
                titleStyle.VerticalAlignment = VerticalAlignment.Center;
                titleStyle.SetFont(font);
                titleStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                titleStyle.FillPattern = FillPattern.SolidForeground;
                List<string> titles = new List<string> { "Path", "Filename", "Size(Byte)", "Path", "FileName", "Size(Byte)", "Comparision Result","Check-in GIT","Need Check","Remark"};
                for (int j = 0; j < titles.Count; j++)
                {
                    ICell cell = title.CreateCell(j);
                    cell.SetCellValue(titles[j]);
                    cell.CellStyle = titleStyle;
                } 
                #endregion


                sheet.SetAutoFilter(new CellRangeAddress(8, 8, 0, titles.Count-2)); //line filter
                sheet.CreateFreezePane(0, 8); //line freeze


                #region body
                for (int i = 0; i < lst.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                    IRow body = sheet.CreateRow(i + 8);
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
                            if (lsize.Equals(rsize))
                            {
                                result = "same";
                            }
                            else
                            {
                                result = "same text but different size";
                            }
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
