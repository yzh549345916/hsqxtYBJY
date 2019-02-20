using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.ObjectModel;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    partial class 智能网格类1
    {
        
        string con = "";
        public 智能网格类1()
        {
            CSH();
          
            
        }

        public string YBRK(string strTime,Int16 sc,ref int count)
        {
            count = 0;
            string error="";
            double[] dbsz1 = HQWDJDFW(ref error);
            var d = HQJWDSQL(ref error);
            List<SXList> sXLists = new List<SXList>();
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = "select * from SX where dataname='省级格点预报订正产品' ORDER BY XH";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        sXLists.Add(new SXList()
                        {
                            ysName= sqlreader.GetString(sqlreader.GetOrdinal("YSName")),
                            sx = sqlreader.GetString(sqlreader.GetOrdinal("sx")),
                        });
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }

            if (sXLists.Count > 0)
            {
                foreach (SXList sXList in sXLists)
                {
                    try
                    {
                        string[] szls = sXList.sx.Split(',');
                        foreach (string sx in szls)
                        {
                            if (PDSJ(strTime, sc, sx, sXList.ysName))
                            {
                                string strData = CIMISSQJZNbyTimeJWD(strTime, sc, sx, "", sXList.ysName, dbsz1[0], dbsz1[1], dbsz1[2], dbsz1[3], ref error);
                                if (strData.Length > 0 && d.Count > 0)
                                {
                                    CLQJZN(strTime, sc, sx, sXList.ysName, strData, d, ref error);
                                    count++;
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                    }
                }
            }

            return error;
        }
        /// <summary>
        /// 如果有数据不存在则返回true
        /// </summary>
        /// <param name="strTime"></param>
        /// <param name="sc"></param>
        /// <param name="sx"></param>
        /// <param name="ysName"></param>
        /// <returns></returns>
        private bool PDSJ(string strTime, Int16 sc, string sx, string ysName)
        {
            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    string sql = String.Format("SELECT * from 省级格点预报订正产品 where date='{0}' and sc='{1}' and sx='{2}' and {3} is  null", strTime,sc,sx,ysName);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader dr = sqlman.ExecuteReader(); dr.Read();
                    if (dr.HasRows)
                    {
                        return true;
                    }

                }
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    string sql = String.Format("SELECT * from 省级格点预报订正产品 where date='{0}' and sc='{1}' and sx='{2}'", strTime, sc, sx);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader dr = sqlman.ExecuteReader(); dr.Read();
                    if (dr.HasRows)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            catch(Exception ex)
            {
                return true;
            }
            return true;
        }
        /// <summary>
        /// 获取数据库中所有站点的经纬度信息保存至List中
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public List<ZDList> HQJWDSQL(ref string error)
        {
            List<ZDList> zDLists = new List<ZDList>();
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = "select * from Station";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    while (sqlreader.Read())
                    {
                       
                        zDLists.Add(new ZDList()
                            {
                                ID=sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                                lon = sqlreader.GetDouble(sqlreader.GetOrdinal("jd")),
                                lat= sqlreader.GetDouble(sqlreader.GetOrdinal("wd")),
                        }
                        );
                    }

                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            return zDLists;

        }
        public void CSH()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
            con = util.Read("OtherConfig", "DB");
           
            
        }

        /// <summary>
        /// 根据起报时间、时效、预报要素、经纬度范围从CIMISS获取区局智能网格要素格点数据
        /// </summary>
        /// <param name="strTime">起报时间，北京时格式：YYYYMMDD</param>
        /// <param name="sx">预报时效</param>
        /// <param name="cc">预报层次，如果没有则空，区局智能网格除了风要素是10意外都为空</param>
        /// <param name="fcstEle">预报要素</param>
        /// <param name="swd">起始纬度</param>
        /// <param name="ewd">结束纬度</param>
        /// <param name="sjd">起始经度</param>
        /// <param name="ejd">结束经度</param>
        /// <returns></returns>
        public string CIMISSQJZNbyTimeJWD(string strTime, Int16 sc, string sx,string cc,string fcstEle, double swd, double ewd, double sjd, double ejd,ref string error)
        {
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "getNafpEleGridInRectByTimeAndLevelAndValidtime";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "NAFP_NWFD_SPCC"); // 资料代码
            //检索时间段
            paramsqx.Add("time", strTime+sc.ToString().PadLeft(2,'0')+"0000");
            paramsqx.Add("minLon", sjd.ToString());
            paramsqx.Add("maxLon", ejd.ToString());
            paramsqx.Add("minLat", swd.ToString());
            paramsqx.Add("maxLat", ewd.ToString());
            paramsqx.Add("fcstEle", fcstEle);
            paramsqx.Add("validTime", sx);
            if (cc.Trim().Length>0)
                paramsqx.Add("fcstLevel", cc);
            
            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "Text";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            
            try
            {
                string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                if (rst == 0)
                {
                    return strData;
                }
                else
                {
                    error += strTime + "日" + sc + "时" + sx.ToString().PadLeft(2, '0') + fcstEle +
                             "获取失败\r\n";

                    strData = "";
                }
            }
            catch
            {
                strData = "";
            }
            return strData;
        }
        /// <summary>
        /// 处理CIMISS获取的区局智能网格数据，当输入预报要素为温度时候自动将开尔文转换为摄氏度
        /// </summary>
        /// <param name="strTime">起报时间YYYYMMDD</param>
        /// <param name="sc">时次</param>
        /// <param name="sx">时效</param>
        /// <param name="fcstEle">预报要素</param>
        /// <param name="strData">CIMISS预报要素数据</param>
        /// <param name="zDLists">站点经纬度列表</param>
        /// <param name="error">错误信息</param>
        public void CLQJZN(string strTime,Int16 sc, string sx, string fcstEle,string strData, List<ZDList> zDLists ,ref string error)
        {
            
            string[] szData = strData.Split(new char[]{ '\n' },StringSplitOptions.RemoveEmptyEntries);
            string[] szCon = szData[0].Split();
            double startLat = 0,
                startLon = 0,
                endLat = 0,
                endLon = 0,
                latStep = 0,
                lonStep = 0;
            try
            {
                strTime = strTime.Substring(0, 4) + '-' + strTime.Substring(4, 2) + '-' + strTime.Substring(6, 2);
                foreach (string ss in szCon)
                {
                    string[] szls = ss.Split('=');
                    if (szls[0] == "startLat")
                    {
                        startLat = Convert.ToDouble(szls[1].Trim());
                    }
                    else if (szls[0] == "startLon")
                    {
                        startLon = Convert.ToDouble(szls[1].Trim());
                    }
                    else if (szls[0] == "endLat")
                    {
                        endLat = Convert.ToDouble(szls[1].Trim());
                    }
                    else if (szls[0] == "endLon")
                    {
                        endLon = Convert.ToDouble(szls[1].Trim());
                    }
                    else if (szls[0] == "latStep")
                    {
                        latStep = Convert.ToDouble(szls[1].Trim());
                    }
                    else if (szls[0] == "lonStep")
                    {
                        lonStep = Convert.ToDouble(szls[1].Trim());
                    }

                }
                for (int i = 0; i < zDLists.Count; i++)
                {
                    int intJd1, intWd1;//指定位置数据数组中经度、纬度序号
                    
                    intJd1 = (int)Math.Round((zDLists[i].lon - startLon) / lonStep,0);//经度个数向上取整
                    intWd1 = (int)Math.Round((zDLists[i].lat - startLat) / latStep,0);//纬度个数向上取整
                    zDLists[i].data =Math.Round(Convert.ToDouble(szData[intWd1 + 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[intJd1].Trim()),4);
                    if (fcstEle == "TMAX" || fcstEle == "TMIN" || fcstEle == "TEM")//如果预报要素为温度类，CIMISS返回开尔文，入库前转换为摄氏度
                    {
                        zDLists[i].data = Math.Round(zDLists[i].data - 273.15,3);
                    }
                }
                //数据保存至数据库当中
                try
                {
                    Stopwatch sw = new Stopwatch();
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        string sql2 = "insert into 省级格点预报订正产品(StatioID,date,sc,sx,"+ fcstEle + ") VALUES(@id,@date,@sc,@sx,@data)";
                        string sql1 = "update 省级格点预报订正产品 set " + fcstEle + "=@data where StatioID=@id and date=@date and sc=@sc and sx=@sx";
                        mycon.Open();//打开
                        int jlCount = 0;
                        foreach (var list1 in zDLists)
                        {
                            jlCount = 0;
                            using (SqlCommand sqlman = new SqlCommand(sql1, mycon))
                            {
                                sqlman.Parameters.AddWithValue("@id", list1.ID);
                                sqlman.Parameters.AddWithValue("@date", strTime);
                                sqlman.Parameters.AddWithValue("@sx", sx);
                                sqlman.Parameters.AddWithValue("@sc", sc);
                                sqlman.Parameters.AddWithValue("@data", list1.data);
                                sw.Start();
                                try
                                {
                                    jlCount = sqlman.ExecuteNonQuery();
                                }
                                catch(Exception ex)
                                {

                                }
                            }

                            if (jlCount == 0)
                            {
                                using (SqlCommand sqlman = new SqlCommand(sql2, mycon))
                                {
                                    sqlman.Parameters.AddWithValue("@id", list1.ID);
                                    sqlman.Parameters.AddWithValue("@date", strTime);
                                    
                                    sqlman.Parameters.AddWithValue("@sx", sx);
                                    sqlman.Parameters.AddWithValue("@sc", sc);
                                    sqlman.Parameters.AddWithValue("@data", list1.data);
                                    sw.Start();
                                    try
                                    {
                                        jlCount = sqlman.ExecuteNonQuery();
                                    }
                                    catch(Exception ex)
                                    {
                                        error += ex.Message + "\r\n";
                                    }
                                }
                            }
                        }


                    }

                }
                catch (Exception ex)
                {
                    error += ex.Message + "\r\n";
                }
            }
            catch(Exception ex)
            {
                error += ex.Message + "\r\n";
            }
        }

        /// <summary>
        /// 获取数据库站点信息的经纬度范围，返回double数组，分别为最小纬度、最大纬度、最小经度、最大经度
        /// </summary>
        /// <returns></returns>
        public double[] HQWDJDFW(ref string error)
        {
            double[] doubleFH = new double[4];
            doubleFH[0] = 999;
            doubleFH[1] = -999;
            doubleFH[2] = 999;
            doubleFH[3] = -999;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = "select * from Station";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        double wd = sqlreader.GetDouble(sqlreader.GetOrdinal("wd")) ;
                        double jd = sqlreader.GetDouble(sqlreader.GetOrdinal("jd"));
                        if (wd < doubleFH[0])
                            doubleFH[0] = wd;
                        if (wd > doubleFH[1])
                            doubleFH[1] = wd;
                        if (jd < doubleFH[2])
                            doubleFH[2] = jd;
                        if (jd > doubleFH[3])
                            doubleFH[3] = jd;
                    }

                    doubleFH[0] = Math.Round(doubleFH[0] - 0.1,4);
                    doubleFH[2] =  Math.Round(doubleFH[2] - 0.1, 4);
                    doubleFH[1] =  Math.Round(doubleFH[1] + 0.1, 4);
                    doubleFH[3] = Math.Round(doubleFH[3] + 0.1,4);

                }
                catch (Exception ex)
                {
   
                    error+=ex.Message+"\r\n";
                }
            }

            return doubleFH;

        }
        public string CIMISS_ZDbyID(string StationID, ref string error)
        {

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "getStaInfoByStaId";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "STA_INFO_SURF_CHN"); // 资料代码
                                                          //检索时间段
            paramsqx.Add("staIds", StationID);//选择区站号
            //此处增加风要素
            paramsqx.Add("elements", "Station_Id_C,Station_Name,Station_levl,Lat,Lon,Alti");// 检索要素：站号、站名、过去24h最高、最低气温、24小时降水量
            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "Text";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            string strLS = strData.Split('"')[1];
            rst = Convert.ToInt32(strLS);

            try
            {
                if (rst == 0)
                {
                    string[] SZlinshi = strData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    strData = "";
                    /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
                    for (int i = 0; i < SZlinshi.Length; i++)
                    {
                        if (i > 1)
                        {
                            strData += SZlinshi[i] + '\n';
                        }
                    }
                    strData = strData.Substring(0, strData.Length - 1);
                }
                else
                {
                    error+=strData+"\r\n";

                    strData = "";
                }
            }
            catch
            {
                strData = "";
            }

            return strData;

        }
        /// <summary>
        /// 根据输入的区站号、站名从CIMISS查询测站级别、纬度、经度、海拔高度，保存至数据库中
        /// </summary>
        /// <param name="StationID">多个测站号用英文逗号分隔</param>
        /// <param name="Name">多个站名用英文逗号分隔，如果对应区站号的站名使用CIMISS中的，则空下，但逗号需要保持</param>
        /// <returns></returns>
        public bool InsertSqlbyID(string StationID,string Name, ref string error)
        {
            bool FHBool = false;
            try
            {
                
                string strData = CIMISS_ZDbyID(StationID,ref error);
                if (strData.Length > 0)
                {

                    Stopwatch sw = new Stopwatch();
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        string sql = "insert into Station(StatioID,Name,Station_levl,WD,JD,High) VALUES(@id,@name,@stationlev,@wd,@jd,@high)";
                        mycon.Open();//打开
                        string[] szName = Name.Split(',');
                        int i = 0;
                        int jlCount = 0;
                        foreach (string ss in strData.Split('\n'))
                        {
                            jlCount = 0;
                            sql = "insert into Station(StatioID,Name,Station_levl,WD,JD,High) VALUES(@id,@name,@stationlev,@wd,@jd,@high)";
                            string[] strSZ = ss.Split();
                            if (strSZ.Length >= 6)
                            {
                                using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                                {
                                    sqlman.Parameters.AddWithValue("@id", strSZ[0]);
                                    try
                                    {
                                        if (szName[i].Length > 0)
                                            sqlman.Parameters.AddWithValue("@name", szName[i]);
                                        else
                                        {
                                            sqlman.Parameters.AddWithValue("@name", strSZ[1]);
                                        }
                                    }
                                    catch
                                    {
                                        sqlman.Parameters.AddWithValue("@name", strSZ[1]);
                                    }
                                    sqlman.Parameters.AddWithValue("@stationlev", strSZ[2]);
                                    sqlman.Parameters.AddWithValue("@wd", strSZ[3]);
                                    sqlman.Parameters.AddWithValue("@jd", strSZ[4]);
                                    sqlman.Parameters.AddWithValue("@high", strSZ[5]);
                                    sw.Start();
                                    
                                    try
                                    {
                                        jlCount = sqlman.ExecuteNonQuery();
                                    }
                                    catch
                                    {

                                    }
                                   
                                }
                                if (jlCount == 0)//如果插入失败，则说明已经存在，进行更新字段操作
                                {
                                    try
                                    {
                                        sql = @"update Station set name=@name ,Station_levl=@stationlev,wd=@wd,jd=@jd,high=@high where StatioID=@id";
                                        using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                                        {
                                            sqlman.Parameters.AddWithValue("@id", strSZ[0]);
                                            try
                                            {
                                                if (szName[i].Length > 0)
                                                    sqlman.Parameters.AddWithValue("@name", szName[i]);
                                                else
                                                {
                                                    sqlman.Parameters.AddWithValue("@name", strSZ[1]);
                                                }
                                                
                                            }
                                            catch
                                            {
                                                sqlman.Parameters.AddWithValue("@name", strSZ[1]);
                                            }
                                            sqlman.Parameters.AddWithValue("@stationlev", strSZ[2]);
                                            sqlman.Parameters.AddWithValue("@wd", strSZ[3]);
                                            sqlman.Parameters.AddWithValue("@jd", strSZ[4]);
                                            sqlman.Parameters.AddWithValue("@high", strSZ[5]);
                                            sw.Start();
                                            try
                                            {
                                                jlCount = sqlman.ExecuteNonQuery();
                                            }
                                            catch
                                            {

                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        error+=ex.Message+ "\r\n";
                                    }
                                    if (jlCount == 0)//如果更新字段失败，说明连接数据库失败，则保存至登陆信息至发报文件夹
                                    {
                                        error += "连接数据库失败\r\n";
                                    }

                                }
                                if (jlCount > 0)
                                    FHBool = true;
                                i++;


                            }
                        }

                    
                    }
                    
                }
                
            }
            catch(Exception ex)
            {
                error += ex.Message + "\r\n";
            }
            return FHBool;
        }

        public class ZDList
        {
            public string ID { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public double data { get; set; }//要素值

        }

        public class SXList
        {
            public string ysName { get; set; }
            public string sx { get; set; }
            public int minSX { get; set; }
            public int maxSX { get; set; }
        }
    }
}
