using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    class 市局指导预报同步
    {
        string qjPath = "";
        string savePath = "";
        public  市局指导预报同步()
        {
            string pathConfig = Environment.CurrentDirectory + @"\config\pathConfig.txt";
            using (StreamReader sr = new StreamReader(pathConfig, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市台指导区局路径")
                    {
                        savePath = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "市台指导预报报文路径")
                    {
                        qjPath = line.Split('=')[1];
                    }
                }
            }
        }
        public string getFile()
        {
            string fh = "";
            try
            {
                FileGet fileGet1 = new FileGet();
                FileGet fileGet2 = new FileGet();
                fileGet1.getFile(qjPath, ".TXT");
                fileGet2.getFile(savePath, ".TXT");
                List<FileInfo> fromLists = fileGet1.lst.OrderByDescending(y => y.CreationTime).Take(100).ToList();
                List<FileInfo> toLists = fileGet2.lst; ;
                int count = 0;
                foreach (FileInfo fileInfo in fromLists)
                {
                    
                    string fileName = fileInfo.Name;
                    if (!toLists.Exists(y => y.Name == fileName))
                    {
                        try
                        {
                            string[] szls = fileName.Split('_');
                            if (!Directory.Exists(savePath + szls[4].Substring(0, 8)))
                            {
                                Directory.CreateDirectory(savePath + szls[4].Substring(0, 8));
                            }
                            File.Copy(fileInfo.FullName, savePath + szls[4].Substring(0, 8) + '\\' + fileName);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            fh += ex.Message + "\r\n";
                        }
                    }
                }
                if(count>0)
                {
                    fh += $"{DateTime.Now}同步{count}条市局报文数据\r\n";
                }
            }
            catch (Exception ex)
            {
                fh += ex.Message + "\r\n";
            }
            return fh;
        }
    }

    public  class FileGet
    {
        /// <summary>
        /// 私有变量
        /// </summary>
        public List<FileInfo> lst = new List<FileInfo>();
        /// <summary>
        /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="extName">扩展名可以多个 例如 .mp3.wma.rm</param>
        /// <returns>List<FileInfo></returns>
        public  List<FileInfo> getFile(string path, string extName)
        {
            lst.Clear();
            getdir(path, extName);
            return lst;
        }
        /// <summary>
        /// 私有方法,递归获取指定类型文件,包含子文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extName"></param>
        private  void getdir(string path, string extName)
        {
            try
            {
                string[] dir = Directory.GetDirectories(path); //文件夹列表   
                DirectoryInfo fdir = new DirectoryInfo(path);
                FileInfo[] file = fdir.GetFiles();
                //FileInfo[] file = Directory.GetFiles(path); //文件列表   
                if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空                   
                {
                    foreach (FileInfo f in file) //显示当前目录所有文件   
                    {
                        if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
                        {
                            lst.Add(f);
                        }
                    }
                    foreach (string d in dir)
                    {
                        getdir(d, extName);//递归   
                    }
                }
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
        }
    }


    public partial class FileGet1
    {
        /// <summary>
        /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="extName">扩展名可以多个 例如 .mp3.wma.rm</param>
        /// <returns>List<FileInfo></returns>
        public static List<FileInfo> getFile(string path, string extName)
        {
            try
            {
                List<FileInfo> lst = new List<FileInfo>();
                string[] dir = Directory.GetDirectories(path); //文件夹列表   
                DirectoryInfo fdir = new DirectoryInfo(path);
                FileInfo[] file = fdir.GetFiles();
                //FileInfo[] file = Directory.GetFiles(path); //文件列表   
                if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空                   
                {
                    foreach (FileInfo f in file) //显示当前目录所有文件   
                    {
                        if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
                        {
                            lst.Add(f);
                        }
                    }
                    foreach (string d in dir)
                    {
                        getFile(d, extName);//递归   
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
