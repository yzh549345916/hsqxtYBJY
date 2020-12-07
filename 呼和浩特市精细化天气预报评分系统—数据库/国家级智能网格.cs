using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using cma.cimiss.client;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    internal partial class 智能网格类1
    {
        public string GJYBRK(string strTime, short sc, ref int count)
        {
            count = 0;
            string error = "";

            List<ZDList> d = HQGJJWDSQL(ref error);
            double[] dbsz1 = HQGJWDJDFW(d, ref error);
            List<SXList> sXLists = new List<SXList>();
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open(); //打开
                    string sql = "select * from SX where dataname='全国智能网格预报服务产品' ORDER BY XH"; //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        sXLists.Add(new SXList
                        {
                            ysName = sqlreader.GetString(sqlreader.GetOrdinal("YSName")),
                            sx = sqlreader.GetString(sqlreader.GetOrdinal("sx")),
                            minSX = sqlreader.GetInt32(sqlreader.GetOrdinal("minXS")),
                            maxSX = sqlreader.GetInt32(sqlreader.GetOrdinal("maxXS"))
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
                            if (PDGJSJ(strTime, sc, sx, sXList.ysName, sXList.minSX, sXList.maxSX))
                            {
                                string strData = CIMISSGJZNbyTimeJWD(strTime, sc, sx, "", sXList.ysName, dbsz1[0], dbsz1[1], dbsz1[2], dbsz1[3], ref error);
                                if (strData.Length > 0 && d.Count > 0)
                                {
                                    CLGJZN(strTime, sc, sx, sXList.ysName, strData, d, sXList.minSX, sXList.maxSX, ref error);
                                    count++;
                                }
                            }
                        }

                        //时效间隔3小时
                        if (sXList.minSX == 3)
                        {
                        }
                        //24小时
                    }
                    catch (Exception ex)
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
        private bool PDGJSJ(string strTime, short sc, string sx, string ysName, int minSX, int maxSX)
        {
            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open(); //打开
                    string sql = string.Format("SELECT * from 全国智能网格预报服务产品{4}h{5} where date='{0}' and sc='{1}' and sx='{2}' and {3} is  null", strTime, sc, sx, ysName, minSX, maxSX); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader dr = sqlman.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        return true;
                    }
                }

                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open(); //打开
                    string sql = string.Format("SELECT * from 全国智能网格预报服务产品{3}h{4} where date='{0}' and sc='{1}' and sx='{2}'", strTime, sc, sx, minSX, maxSX); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader dr = sqlman.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                return true;
            }

            return true;
        }


        /// <summary>
        /// 根据起报时间、时效、预报要素、经纬度范围从CIMISS获取国家智能网格要素格点数据
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
        public string CIMISSGJZNbyTimeJWD(string strTime, short sc, string sx, string cc, string fcstEle, double swd, double ewd, double sjd, double ejd, ref string error)
        {
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            string userId = "BEHT_BFHT_2131"; // 
            string pwd = "YZHHGDJM"; // 
            /*   2.2 接口ID */
            string interfaceId1 = "getNafpEleGridInRectByTimeAndLevelAndValidtime";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<string, string> paramsqx = new Dictionary<string, string>();
            // 必选参数
            paramsqx.Add("dataCode", "NAFP_NWFD_SCMOC"); // 资料代码
            //检索时间段
            paramsqx.Add("time", strTime + sc.ToString().PadLeft(2, '0') + "0000");
            paramsqx.Add("minLon", sjd.ToString());
            paramsqx.Add("maxLon", ejd.ToString());
            paramsqx.Add("minLat", swd.ToString());
            paramsqx.Add("maxLat", ewd.ToString());
            paramsqx.Add("fcstEle", fcstEle);
            paramsqx.Add("validTime", sx);
            if (cc.Trim().Length > 0)
                paramsqx.Add("fcstLevel", cc);

            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            string dataFormat = "SPACEText";
            StringBuilder QXSK = new StringBuilder(); //返回字符串
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

                error += strTime + "日" + sc + "时" + sx.PadLeft(2, '0') + fcstEle + "获取失败\r\n";
                strData = "";
            }
            catch
            {
                strData = "";
            }

            return strData;
        }

        //获取国家局智能网格站点经纬度范围
        public double[] HQGJWDJDFW(List<ZDList> list, ref string error)
        {
            double[] doubleFH = new double[4];
            doubleFH[0] = 999;
            doubleFH[1] = -999;
            doubleFH[2] = 999;
            doubleFH[3] = -999;
            try
            {
                foreach (ZDList l in list)
                {
                    double wd = l.lat;
                    double jd = l.lon;
                    if (wd < doubleFH[0])
                        doubleFH[0] = wd;
                    if (wd > doubleFH[1])
                        doubleFH[1] = wd;
                    if (jd < doubleFH[2])
                        doubleFH[2] = jd;
                    if (jd > doubleFH[3])
                        doubleFH[3] = jd;
                }

                doubleFH[0] = Math.Round(doubleFH[0] - 0.1, 4);
                doubleFH[2] = Math.Round(doubleFH[2] - 0.1, 4);
                doubleFH[1] = Math.Round(doubleFH[1] + 0.1, 4);
                doubleFH[3] = Math.Round(doubleFH[3] + 0.1, 4);
            }
            catch (Exception ex)
            {
                error += ex.Message + "\r\n";
            }

            return doubleFH;
        }

        /// <summary>
        /// 获取数据库中所有国家智能网格站点的经纬度信息保存至List中
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public List<ZDList> HQGJJWDSQL(ref string error)
        {
            List<ZDList> zDLists = new List<ZDList>();
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open(); //打开
                    string sql = "select * from GJStation"; //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        zDLists.Add(new ZDList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            lon = sqlreader.GetDouble(sqlreader.GetOrdinal("jd")),
                            lat = sqlreader.GetDouble(sqlreader.GetOrdinal("wd"))
                        });
                    }
                }
                catch (Exception ex)
                {
                    error += ex.Message + "\r\n";
                }
            }

            return zDLists;
        }

        /// <summary>
        /// 处理CIMISS获取的国家智能网格数据，当输入预报要素为温度时候自动将开尔文转换为摄氏度
        /// </summary>
        /// <param name="strTime">起报时间YYYYMMDD</param>
        /// <param name="sc">时次</param>
        /// <param name="sx">时效</param>
        /// <param name="fcstEle">预报要素</param>
        /// <param name="strData">CIMISS预报要素数据</param>
        /// <param name="zDLists">站点经纬度列表</param>
        /// <param name="error">错误信息</param>
        public void CLGJZN(string strTime, short sc, string sx, string fcstEle, string strData, List<ZDList> zDLists, int minSX, int maxSX, ref string error)
        {
            string[] szData = strData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            string[] szCon = szData[0].Split();
            double startLat = 0, startLon = 0, endLat = 0, endLon = 0, latStep = 0, lonStep = 0;
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
                    int intJd1, intWd1; //指定位置数据数组中经度、纬度序号

                    intJd1 = (int) Math.Round((zDLists[i].lon - startLon) / lonStep, 0); //经度个数向上取整
                    intWd1 = (int) Math.Round((zDLists[i].lat - startLat) / latStep, 0); //纬度个数向上取整
                    zDLists[i].data = Math.Round(Convert.ToDouble(szData[intWd1 + 1].Split(new[]
                    {
                        ' '
                    }, StringSplitOptions.RemoveEmptyEntries)[intJd1].Trim()), 4);
                    if (fcstEle == "TMAX" || fcstEle == "TMIN" || fcstEle == "TEM") //如果预报要素为温度类，CIMISS返回开尔文，入库前转换为摄氏度
                    {
                        zDLists[i].data = Math.Round(zDLists[i].data - 273.15, 3);
                    }
                }

                //数据保存至数据库当中
                try
                {
                    Stopwatch sw = new Stopwatch();
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        string s1lls = "全国智能网格预报服务产品" + minSX + 'h' + maxSX;
                        string sql2 = "insert into " + s1lls + "(StatioID,date,sc,sx," + fcstEle + ") VALUES(@id,@date,@sc,@sx,@data)";
                        string sql1 = "update " + s1lls + " set " + fcstEle + "=@data where StatioID=@id and date=@date and sc=@sc and sx=@sx";
                        mycon.Open(); //打开
                        int jlCount = 0;
                        foreach (var list1 in zDLists)
                        {
                            if (fcstEle == "TMAX" || fcstEle == "TMIN" || fcstEle == "TEM" || fcstEle == "PRE_3H") //数据有效性检验
                            {
                                if (list1.data > 900)
                                {
                                    break;
                                }
                            }

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
                                catch (Exception ex)
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
                                    catch (Exception ex)
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
            catch (Exception ex)
            {
                error += ex.Message + "\r\n";
            }
        }

        public string CIMISS6H(DateTime dateTime)
        {
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            string userId = "BEHT_BFHT_2131"; // 
            string pwd = "YZHHGDJM"; // 
            /*   2.2 接口ID */
            string interfaceId1 = "getSevpFileByTime";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<string, string> paramsqx = new Dictionary<string, string>();
            // 必选参数
            paramsqx.Add("dataCode", "SEVP_WEFC_6H"); // 资料代码
            //检索时间段
            paramsqx.Add("times", dateTime.ToUniversalTime().ToString("yyyyMMddHHmmss"));
            paramsqx.Add("elements", "File_URL,FILE_NAME_ORIG");

            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            string dataFormat = "tabText";
            StringBuilder QXSK = new StringBuilder(); //返回字符串
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
                    XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
                    string myPath = util.Read("Path6Hour");

                    string[] dataSz = strData.Split('\n')[2].Split();
                    myPath += dataSz[dataSz.Length - 1];
                    bool fhbs = 保存网址文件(dataSz[3], myPath);
                    if (fhbs)
                        return "";
                    return $"{dateTime.ToString("yyyy年MM月dd日HH时")}6小时指导预保存失败\r\n";
                }

                return $"{dateTime.ToString("yyyy年MM月dd日HH时")}6小时指导预报获取失败\r\n";
            }
            catch
            {
                return $"{dateTime.ToString("yyyy年MM月dd日HH时")}6小时指导预保存失败\r\n";
            }
        }

        public bool 保存网址文件(string url, string filePath)
        {
            try
            {
                HttpWebRequest Myrq = (HttpWebRequest) WebRequest.Create(url);
                HttpWebResponse myrp = (HttpWebResponse) Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                //if (prog != null)
                //{
                //    prog.Maximum = (int)totalBytes;
                //}
                Stream st = myrp.GetResponseStream();
                Stream so = new FileStream(filePath, FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    //   System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    //if (prog != null)
                    //{
                    //    prog.Value = (int)totalDownloadedByte;
                    //}
                    osize = st.Read(by, 0, by.Length);
                }

                so.Close();
                st.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                ;
            }
        }
    }
}