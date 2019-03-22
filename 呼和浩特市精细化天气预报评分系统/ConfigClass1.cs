using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace 呼和浩特市精细化天气预报评分系统
{
    class ConfigClass1
    {
        string _con = "";
        public ConfigClass1()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\数值预报设置.xml");
            _con = util.Read("OtherConfig", "SZDB");
        }
        public string IDName(string temGS)//返回指定上级ID下的所有ID和名称，ID与名称以','分割，每组以换行符分割
        {
            temGS = '%' + temGS + '%';
            string strFH = "";
            List<menuList> menulist1 = new List<menuList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from Station where temGS like'{0}' order by StatioID", temGS);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new menuList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            name = sqlreader.GetString(sqlreader.GetOrdinal("name")),
                        });
                    }
                }

                if (menulist1.Count > 0)
                {
                    foreach (var v1 in menulist1)
                    {
                        strFH += v1.id + ',' + v1.name  + '\n';
                    }

                    strFH = strFH.Substring(0, strFH.Length - 1);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strFH;
        }
        public float GetTaxZqLbyIDandSx(List<YBList> yBLists, List<SKList> sKLists, string id)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists = yBLists.FindAll(y=> y.id == id);
            foreach (YBList yBList in temLists)
            {
                List< SKList> sKList = sKLists.FindAll(y => y.dateTime <= yBList.dateTime&& y.dateTime>= yBList.dateTime.AddDays(-1) && y.id == yBList.id);
                
                if(sKList != null)
                {
                    float tmax = -99999;
                    foreach (SKList sk in sKList)
                    {
                        if(Math.Abs(sk.Tmax) < 1000)
                        {
                            if (tmax < sk.Tmax)
                                tmax = sk.Tmax;
                        }
                    }
                    if (Math.Abs(yBList.ys) < 1000 && Math.Abs(tmax)<1000)
                    {
                        countAll++;
                        if (Math.Abs(tmax - yBList.ys) <= 2)
                        {
                            countTrue++;
                        }
                    }
                }
               
            }
            if (countAll > 0)
            {
                zql = (float)Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }
        public float GetTaxZqLbyIDandSx(List<YBList> yBLists, List<SKList> sKLists)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists = yBLists;
            foreach (YBList yBList in temLists)
            {
                List<SKList> sKList = sKLists.FindAll(y => y.dateTime <= yBList.dateTime && y.dateTime >= yBList.dateTime.AddDays(-1) && y.id == yBList.id);

                if (sKList != null)
                {
                    float tmax = -99999;
                    foreach (SKList sk in sKList)
                    {
                        if (Math.Abs(sk.Tmax) < 1000)
                        {
                            if (tmax < sk.Tmax)
                                tmax = sk.Tmax;
                        }
                    }
                    if (Math.Abs(yBList.ys) < 1000 && Math.Abs(tmax) < 1000)
                    {
                        countAll++;
                        if (Math.Abs(tmax - yBList.ys) <= 2)
                        {
                            countTrue++;
                        }
                    }
                }

            }
            if (countAll > 0)
            {
                zql = (float)Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }
        public float GetTminZqLbyIDandSx(List<YBList> yBLists, List<SKList> sKLists, string id)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists = yBLists.FindAll(y => y.id == id);
            foreach (YBList yBList in temLists)
            {
                List<SKList> sKList = sKLists.FindAll(y => y.dateTime <= yBList.dateTime && y.dateTime >= yBList.dateTime.AddDays(-1) && y.id == yBList.id);

                if (sKList != null)
                {
                    float tmin = 99999;
                    foreach (SKList sk in sKList)
                    {
                        if (Math.Abs(sk.Tmin) < 1000)
                        {
                            if (tmin > sk.Tmin)
                                tmin = sk.Tmin;
                        }
                    }
                    if (Math.Abs(yBList.ys) < 1000 && Math.Abs(tmin) < 1000)
                    {
                        countAll++;
                        if (Math.Abs(tmin - yBList.ys) <= 2)
                        {
                            countTrue++;
                        }
                    }
                }

            }
            if (countAll > 0)
            {
                zql = (float)Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }
        public float GetTminZqLbyIDandSx(List<YBList> yBLists, List<SKList> sKLists)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists = yBLists;
            foreach (YBList yBList in temLists)
            {
                List<SKList> sKList = sKLists.FindAll(y => y.dateTime <= yBList.dateTime && y.dateTime >= yBList.dateTime.AddDays(-1) && y.id == yBList.id);

                if (sKList != null)
                {
                    float tmin =99999;
                    foreach (SKList sk in sKList)
                    {
                        if (Math.Abs(sk.Tmin) < 1000)
                        {
                            if (tmin > sk.Tmin)
                                tmin = sk.Tmin;
                        }
                    }
                    if (Math.Abs(yBList.ys) < 1000 && Math.Abs(tmin) < 1000)
                    {
                        countAll++;
                        if (Math.Abs(tmin - yBList.ys) <= 2)
                        {
                            countTrue++;
                        }
                    }
                }

            }
            if (countAll > 0)
            {
                zql = (float)Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }
        public float GetTemZqLbyIDandSx(List<YBList> yBLists,List<SKList> sKLists,string id,int sx)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists= yBLists.FindAll(y => y.sx==sx&&y.id==id);
            foreach(YBList yBList in temLists)
            {
                SKList sKList = sKLists.Find(y => y.dateTime == yBList.dateTime && y.id == yBList.id);
                if(sKList!=null && Math.Abs(sKList.TEM) < 1000 && Math.Abs(yBList.ys) < 1000)
                {
                    countAll++;
                    if(Math.Abs(sKList.TEM - yBList.ys)<=2)
                    {
                        countTrue++;
                    }
                }
            }
            if (countAll > 0)
            {
                zql = (float) Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }

        public float GetTemZqLbyIDandSx(List<YBList> yBLists, List<SKList> sKLists, int sx)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists = yBLists.FindAll(y => y.sx == sx );
            foreach (YBList yBList in temLists)
            {
                SKList sKList = sKLists.Find(y => y.dateTime == yBList.dateTime && y.id == yBList.id);
                if (sKList != null && Math.Abs(sKList.TEM) < 1000 && Math.Abs(yBList.ys) < 1000)
                {
                    countAll++;
                    if (Math.Abs(sKList.TEM - yBList.ys) <= 2)
                    {
                        countTrue++;
                    }
                }
            }
            if (countAll > 0)
            {
                zql = (float)Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }
        public float GetTemPjzqLbyIDandSx(List<YBList> yBLists, List<SKList> sKLists, int minsx, int maxsx)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists = yBLists.FindAll(y => y.sx >= minsx && y.sx <= maxsx );
            foreach (YBList yBList in temLists)
            {
                SKList sKList = sKLists.Find(y => y.dateTime == yBList.dateTime && y.id == yBList.id);
                if (sKList != null && Math.Abs(sKList.TEM) < 1000 && Math.Abs(yBList.ys) < 1000)
                {
                    countAll++;
                    if (Math.Abs(sKList.TEM - yBList.ys) <= 2)
                    {
                        countTrue++;
                    }
                }
            }
            if (countAll > 0)
            {
                zql = (float)Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }
        public float GetTemPJZQLbyIDandSX(List<YBList> yBLists, List<SKList> sKLists, string id, int minsx,int maxsx)
        {
            float zql = 0;
            int countAll = 0, countTrue = 0;
            List<YBList> temLists = yBLists.FindAll(y => y.sx >= minsx&& y.sx <= maxsx && y.id == id);
            foreach (YBList yBList in temLists)
            {
                SKList sKList = sKLists.Find(y => y.dateTime == yBList.dateTime && y.id == yBList.id);
                if (sKList != null && Math.Abs(sKList.TEM) < 1000 && Math.Abs(yBList.ys) < 1000)
                {
                    countAll++;
                    if (Math.Abs(sKList.TEM - yBList.ys) <= 2)
                    {
                        countTrue++;
                    }
                }
            }
            if (countAll > 0)
            {
                zql = (float)Math.Round((double)100 * countTrue / countAll, 2);
            }
            return zql;
        }

        public string getGS()//返回指定上级ID下的所有ID和名称，ID与名称以','分割，每组以换行符分割
        {
            string strFH = "";
            List<menuList> menulist1 = new List<menuList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from Station order by StatioID");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new menuList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            name = sqlreader.GetString(sqlreader.GetOrdinal("name")),
                            temGS= sqlreader.GetString(sqlreader.GetOrdinal("temGS")),
                        });
                    }
                }

                if (menulist1.Count > 0)
                {
                    strFH = menulist1[0].temGS+',';
                    foreach (var v1 in menulist1)
                    {
                        string[] szls = v1.temGS.Split(',');
                        string[] szls2= strFH.Split(',');
                        foreach (string ss in szls)
                        {
                            bool bs = false;
                            foreach(string ss2 in szls2)
                            {
                                if (ss2.Trim() == ss)
                                    bs = true;
                            }
                            if(!bs)
                                strFH += ss + ',';
                        }
                        
                    }

                    strFH = strFH.Substring(0, strFH.Length - 1);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strFH;
        }
        /// <summary>
        /// 根据时间范围、公式、时次、最小和最大时效获取预报信息
        /// </summary>
        /// <param name="sDateTime">开始时间</param>
        /// <param name="eDateTime">结束时间</param>
        /// <param name="GS">计算公式</param>
        /// <param name="scint">时次</param>
        /// <param name="minsx">最小时效</param>
        /// <param name="maxsx">最大时效</param>
        /// <returns></returns>
        public List<YBList> GetTemListsbyGSandDate(DateTime sDateTime,DateTime eDateTime,string GS,Int16 scint,Int16 minsx,Int16 maxsx)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_TEM where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}' and gs='{GS}' and sc={scint} and TEM is not null and TEM<2000 and TEM>-2000 and sx>={minsx} and sx<= {maxsx} order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt= sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");
                        
                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime=dt,
                            ys=sqlreader.GetFloat(sqlreader.GetOrdinal("TEM"))
                        });
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return yBLists;
        }
        public List<YBList> GetTaxListsbyGSandDate(DateTime sDateTime, DateTime eDateTime, string GS, Int16 scint, Int16 sxint)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_TMAX where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}' and gs='{GS}' and sc={scint} and TMAX is not null and TMAX<2000 and TMAX>-2000 and sx={sxint} order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("TMAX"))
                        });
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return yBLists;
        }
        public List<YBList> GetTminListsbyGSandDate(DateTime sDateTime, DateTime eDateTime, string GS, Int16 scint, Int16 sxint)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_TMIN where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}' and gs='{GS}' and sc={scint} and TMIN is not null and TMIN<2000 and TMIN>-2000 and sx={sxint} order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("TMIN"))
                        });
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return yBLists;
        }
        public List<SKList> GetSKListsbyGSandDate(DateTime sDateTime, DateTime eDateTime)
        {
            List<SKList> skLists = new List<SKList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from SK where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}'  order by date,sc";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        int sc = sqlreader.GetInt32(sqlreader.GetOrdinal("sc"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc );
                        skLists.Add(new SKList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StationID")),
                            dateTime = dt,
                            TEM = sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")),
                            Tmin = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin")),
                            Tmax = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax")),
                            PRE = sqlreader.GetFloat(sqlreader.GetOrdinal("PRE")),
                            FX = sqlreader.GetFloat(sqlreader.GetOrdinal("FX10")),
                            FS = sqlreader.GetFloat(sqlreader.GetOrdinal("FS10")),
                            FXJD = sqlreader.GetFloat(sqlreader.GetOrdinal("FXJD")),
                            FSJD = sqlreader.GetFloat(sqlreader.GetOrdinal("FSJD")),
                        });
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return skLists;
        }
        public class menuList
        {
            public string name { get; set; }
            public string id { get; set; }
            public string temGS { get; set; }
        }
        public class YBList
        {
            public string id { get; set; }
            public string date { get; set; }
            public Int16 sc { get; set; }
            public Int16 sx { get; set; }
            public DateTime dateTime { get; set; }
            public float ys { get; set; }
        }
        public class SKList
        {
            public string id { get; set; }
            public DateTime dateTime { get; set; }
            public float TEM { get; set; }
            public float Tmax { get; set; }
            public float Tmin { get; set; }
            public float PRE { get; set; }
            public float FS { get; set; }
            public float FX { get; set; }
            public float FXJD { get; set; }
            public float FSJD { get; set; }
        }
    }
}
