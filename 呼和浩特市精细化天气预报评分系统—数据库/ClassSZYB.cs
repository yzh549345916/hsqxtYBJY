using cma.cimiss.client;
using cma.cimiss.client;
using cma.cimiss;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    class ClassSZYB
    {
        string con = "";
        public ClassSZYB()
        {
            CSH();
        }
        public void CSH()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
            con = util.Read("OtherConfig", "SZDB");


        }
        public void SKRK(string strDate, int hour, ref string strError,ref bool insertBS)
        {
            try
            {
                string stationIDs  = HQStationID(ref strError);
                if (stationIDs.Trim().Length > 0)
                {
                    int rst = 0;
                    string strSK=CimissHQSKbyHour(strDate, hour, stationIDs, ref rst, ref strError);
                    if ((rst == 0))
                    {
                       try
                       {
                            string sqlInsert = "insert into SK" + "(StationID,date,sc,TEM,Tmin,Tmax,PRE,FX10,FS10,FXJD,FSJD) VALUES(@id,@date,@sc,@tem,@tmin,@tmax,@pre,@fx10,@fs10,@fxjd,@fsjd)";
                            string sqlupdate = "update SK set  TEM=@tem,Tmin=@tmin,Tmax=@tmax,PRE=@pre,FX10=@fx10,FS10=@fs10,FXJD=@fxjd,FSJD=@fsjd where StationID=@id and date=@date and sc=@sc";
                            Stopwatch sw = new Stopwatch();
                            string myDate = strDate.Substring(0, 4) + '-' + strDate.Substring(4, 2) + '-' + strDate.Substring(6, 2);
                            using (SqlConnection mycon = new SqlConnection(con))
                            {
                                mycon.Open();//打开
                                int jlCount = 0;
                                for (int i = 0; i < strSK.Split('\n').Length; i++)
                                {
                                    string[] szLS1 = strSK.Split('\n')[i].Split(' ');
                                    float myTmax, myTmin, myRain, myTem, myFX10, myFS10, myFXJD, myFSJD;
                                    try
                                    {
                                        myTem = Convert.ToSingle(szLS1[2]);
                                    }
                                    catch
                                    {
                                        myTem = 999999;
                                    }
                                    try
                                    {
                                        myTmax = Convert.ToSingle(szLS1[3]);
                                    }
                                    catch
                                    {
                                        myTmax = 999999;
                                    }
                                    try
                                    {
                                        myTmin = Convert.ToSingle(szLS1[4]);
                                    }
                                    catch
                                    {
                                        myTmin = 999999;
                                    }

                                    if (myTmin == myTmax)//如果最高最低温度相等，按照缺测处理
                                    {
                                        myTmin = 999999;
                                        myTmax = 999999;
                                    }
                                    try
                                    {
                                        myRain = Convert.ToSingle(szLS1[5]);
                                    }
                                    catch
                                    {
                                        myRain = 999999;
                                    }
                                    try
                                    {
                                        myFX10 = Convert.ToSingle(szLS1[6]);
                                    }
                                    catch
                                    {
                                        myFX10 = 999999;
                                    }
                                    try
                                    {
                                        myFS10 = Convert.ToSingle(szLS1[7]);
                                    }
                                    catch
                                    {
                                        myFS10 = 999999;
                                    }
                                    try
                                    {
                                        myFXJD = Convert.ToSingle(szLS1[8]);
                                    }
                                    catch
                                    {
                                        myFXJD = 999999;
                                    }
                                    try
                                    {
                                        myFSJD = Convert.ToSingle(szLS1[9]);
                                    }
                                    catch
                                    {
                                        myFSJD = 999999;
                                    }
                                    
                                    jlCount = 0;
                                    using (SqlCommand sqlman = new SqlCommand(sqlInsert, mycon))
                                    {
                                        sqlman.Parameters.AddWithValue("@id", szLS1[1]);
                                        sqlman.Parameters.AddWithValue("@date", myDate);
                                        sqlman.Parameters.AddWithValue("@sc", hour);
                                        sqlman.Parameters.AddWithValue("@tem", myTem);
                                        sqlman.Parameters.AddWithValue("@tmin", myTmin);
                                        sqlman.Parameters.AddWithValue("@tmax", myTmax);
                                        sqlman.Parameters.AddWithValue("@pre", myRain);
                                        sqlman.Parameters.AddWithValue("@fx10", myFX10);
                                        sqlman.Parameters.AddWithValue("@fs10", myFS10);
                                        sqlman.Parameters.AddWithValue("@fxjd", myFXJD);
                                        sqlman.Parameters.AddWithValue("@fsjd", myFSJD);
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
                                        using (SqlCommand sqlman = new SqlCommand(sqlupdate, mycon))
                                        {
                                            sqlman.Parameters.AddWithValue("@id", szLS1[1]);
                                            sqlman.Parameters.AddWithValue("@date", myDate);
                                            sqlman.Parameters.AddWithValue("@sc", hour);
                                            sqlman.Parameters.AddWithValue("@tem", myTem);
                                            sqlman.Parameters.AddWithValue("@tmin", myTmin);
                                            sqlman.Parameters.AddWithValue("@tmax", myTmax);
                                            sqlman.Parameters.AddWithValue("@pre", myRain);
                                            sqlman.Parameters.AddWithValue("@fx10", myFX10);
                                            sqlman.Parameters.AddWithValue("@fs10", myFS10);
                                            sqlman.Parameters.AddWithValue("@fxjd", myFXJD);
                                            sqlman.Parameters.AddWithValue("@fsjd", myFSJD);
                                            sw.Start();
                                            try
                                            {
                                                jlCount = sqlman.ExecuteNonQuery();
                                            }
                                            catch (Exception ex)
                                            {
                                                strError += ex.Message + "\r\n";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        insertBS = true;
                                    }

                                }
                            }
                         
                        }
                       catch(Exception ex)
                       {
                            strError += ex.Message + "\r\n";
                       }
                    }
                }
            }
            catch (Exception ex)
            {
                strError += ex.Message + "\r\n";
            }
        }
        /// <summary>
        /// 获取数据库中中站点的区站号并保存在list中
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        
        public List<WindList> CLECWind(DateTime sdt,DateTime edt,ref string error)
        {
            List<WindList> windLists = new List<WindList>();
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from EC预报 where  date>='{0:yyyy-MM-dd}' and date<='{1:yyyy-MM-dd}' and sx<96", sdt,edt);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            float v = sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10"));
                            float u = sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10"));
                            double fx = 0;double fs = 0;
                            GetFXFS(v, u, ref fx, ref fs);
                            windLists.Add(new WindList()
                            {
                                fx=fx,
                                fs=fs,
                                sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX")),
                                sc = sqlreader.GetInt16(sqlreader.GetOrdinal("SC")),
                                date = sqlreader.GetDateTime(sqlreader.GetOrdinal("date")),
                                stationID = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
           
            sdt = Convert.ToDateTime(sdt.ToString("yyyy-MM-dd"));
            edt = Convert.ToDateTime(edt.ToString("yyyy-MM-dd"));
            for(int i=0;DateTime.Compare(sdt.AddDays(i),edt) <= 0; i++)
            {
                List<WindList> winds= windLists.FindAll(y => y.date == sdt.AddDays(i)&&y.sc==8);
                for (int j = 0; j < 96;j=j+3)
                {
                    if(!winds.Exists(y => y.sx == j))
                    {
                        string id = "53463,53368,53464,53466,53467,53469,53562";
                        string[] szstr = id.Split(',');
                        foreach(string idd in szstr)
                        {
                            windLists.Add(new WindList()
                            {
                                fx = -999999,
                                fs = -999999,
                                sx = j,
                                sc = 8,
                                date = sdt.AddDays(i),
                                stationID = idd,
                            });
                        }
                        
                    }
                    if(j>=72)
                    {
                        j = j + 3;
                    }
                }
                winds = windLists.FindAll(y => y.date == sdt.AddDays(i) && y.sc == 20);
                for (int j = 0; j < 96; j = j + 3)
                {
                    if (!winds.Exists(y => y.sx == j))
                    {
                        string id = "53463,53368,53464,53466,53467,53469,53562";
                        string[] szstr = id.Split(',');
                        foreach (string idd in szstr)
                        {
                            windLists.Add(new WindList()
                            {
                                fx = -999999,
                                fs = -999999,
                                sx = j,
                                sc = 20,
                                date = sdt.AddDays(i),
                                stationID = idd,
                            });
                        }

                    }
                    if (j >= 72)
                    {
                        j = j + 3;
                    }
                }
            }

            windLists = windLists.OrderBy(y => y.stationID).ThenBy(y => y.date).ThenBy(y => y.sc).ThenBy(y => y.sx).ToList();
            for (int i = 0; i < windLists.Count; i++)
            {
                windLists[i].ybDate = Convert.ToDateTime(windLists[i].date.ToString("yyyy-MM-dd")).AddHours(windLists[i].sx + windLists[i].sc);
            }
            return windLists;
        }

        public void GetFXFS(double v, double u,ref double fx, ref double fs)
        {
            string fxfs = "";
            fx = 999.9; //风向

            if ((u > 0) & (v > 0))
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u < 0) & (v > 0))
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u < 0) & (v < 0))
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u > 0) & (v < 0))
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u == 0) & (v > 0))
            {
                fx = 180;
            }
            else if ((u == 0) & (v < 0))
            {
                fx = 0;
            }
            else if ((u > 0) & (v == 0))
            {
                fx = 270;
            }
            else if ((u < 0) & (v == 0))
            {
                fx = 90;
            }
            else if ((u == 0) & (v == 0))
            {
                fx = 999.9;
            }


            fs = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));

           
        }
        public string HQStationID(ref string error)
        {
            string stationIDs = "";
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
                        stationIDs += sqlreader.GetString(sqlreader.GetOrdinal("StatioID")) + ',';
                    }
                    stationIDs = stationIDs.Substring(0, stationIDs.Length - 1);
                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            return stationIDs;

        }
        public string CimissHQSKbyHour(string strDate, int hour,string stationIDs ,ref int rst1, ref string strError)
        {

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfEleByTimeAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
                                                          //检索时间段
            string strToday = strDate + hour.ToString().PadLeft(2,'0') + "0000";
            string strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            paramsqx.Add("times", strToday);
            paramsqx.Add("staIds", stationIDs);//选择区站号
            paramsqx.Add("elements", "Station_Name,Station_Id_C,TEM,TEM_Max,TEM_Min,PRE_1h,WIN_D_Avg_10mi,WIN_S_Avg_10mi,WIN_D_INST_Max,WIN_S_Inst_Max");// 检索要素：站号、站名、过去24h最高、最低气温、24小时降水量
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
            strLS = strData.Split('"')[1];
            rst = Convert.ToInt32(strLS);
            rst1 = rst;

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
                strError += strData + '\n';
            }

            return strData;

        }

        public bool CLYB(DateTime dt,int sc,ref string error)
        {
            List<StationList> stationLists = new List<StationList>();
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
                        try
                        {
                            stationLists.Add(new StationList()
                            {
                                Id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                                temGS = sqlreader.IsDBNull(sqlreader.GetOrdinal("temGS")) ? "ECSK" : sqlreader.GetString(sqlreader.GetOrdinal("temGS")),
                                tmaxGS = sqlreader.IsDBNull(sqlreader.GetOrdinal("tmaxGS")) ? "ECSK" : sqlreader.GetString(sqlreader.GetOrdinal("tmaxGS")),
                                tminGS = sqlreader.IsDBNull(sqlreader.GetOrdinal("tminGS")) ? "ECSK" : sqlreader.GetString(sqlreader.GetOrdinal("tminGS")),
                                PREGS = sqlreader.IsDBNull(sqlreader.GetOrdinal("PREGS")) ? "EC" : sqlreader.GetString(sqlreader.GetOrdinal("PREGS")),
                                FXGS = sqlreader.IsDBNull(sqlreader.GetOrdinal("FXGS")) ? "ECSK" : sqlreader.GetString(sqlreader.GetOrdinal("FXGS")),
                                FSGS = sqlreader.IsDBNull(sqlreader.GetOrdinal("FSGS")) ? "ECSK" : sqlreader.GetString(sqlreader.GetOrdinal("FSGS")),
                            });
                        }
                        catch(Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            for(int i=0;i<stationLists.Count;i++)
            {
                string temgs = stationLists[i].temGS;
                string tmaxgs = stationLists[i].tmaxGS;
                string tmings = stationLists[i].tminGS;
                if (temgs.Split(',').Length>1)
                {
                    Int16 count = 0;
                    foreach(string gs in temgs.Split(','))
                    {
                        if(count==0)
                        {
                            stationLists[i].temGS = gs;
                            switch (gs)
                            {
                                case "ECSK"://根据不同的公式定义，执行不同的计算
                                    stationLists[i].TEM = temByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                                default:
                                    stationLists[i].TEM = temByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;

                            }
                        }
                        else
                        {
                            List<ECList> tem = new List<ECList>();
                            switch (gs)
                            {
                                case "ECSK"://根据不同的公式定义，执行不同的计算
                                    tem = temByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                                default:
                                    tem = temByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                            }
                            stationLists.Add(new StationList()
                            {
                                Id = stationLists[i].Id,
                                temGS =gs,
                                TEM=tem,
                            });
                        }
                    }
                }
                else
                {
                    switch (temgs)
                    {
                        case "ECSK"://根据不同的公式定义，执行不同的计算
                            stationLists[i].TEM = temByECSK(stationLists[i].Id, dt, sc, ref error);
                            break;
                        default:
                            stationLists[i].TEM = temByECSK(stationLists[i].Id, dt, sc, ref error);
                            break;

                    }
                }

                if (tmaxgs.Split(',').Length > 1)
                {
                    Int16 count = 0;
                    foreach (string gs in tmaxgs.Split(','))
                    {
                        if (count == 0)
                        {
                            stationLists[i].tmaxGS = gs;
                            switch (gs)
                            {
                                case "ECSK"://根据不同的公式定义，执行不同的计算
                                    stationLists[i].Tmax = tmaxByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                                default:
                                    stationLists[i].Tmax = tmaxByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;

                            }
                        }
                        else
                        {
                            List<ECList> tem = new List<ECList>();
                            switch (gs)
                            {
                                case "ECSK"://根据不同的公式定义，执行不同的计算
                                    tem = tmaxByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                                default:
                                    tem = tmaxByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                            }
                            stationLists.Add(new StationList()
                            {
                                Id = stationLists[i].Id,
                                tmaxGS = gs,
                                Tmax = tem,
                            });
                        }
                    }
                }
                else
                {
                    switch (tmaxgs)
                    {
                        case "ECSK"://根据不同的公式定义，执行不同的计算
                            stationLists[i].Tmax = tmaxByECSK(stationLists[i].Id, dt, sc, ref error);
                            break;
                        default:
                            stationLists[i].Tmax = tmaxByECSK(stationLists[i].Id, dt, sc, ref error);
                            break;

                    }
                }

                if (tmings.Split(',').Length > 1)
                {
                    Int16 count = 0;
                    foreach (string gs in tmings.Split(','))
                    {
                        if (count == 0)
                        {
                            stationLists[i].tminGS = gs;
                            switch (gs)
                            {
                                case "ECSK"://根据不同的公式定义，执行不同的计算
                                    stationLists[i].Tmin = tminByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                                default:
                                    stationLists[i].Tmin = tminByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;

                            }
                        }
                        else
                        {
                            List<ECList> tem = new List<ECList>();
                            switch (gs)
                            {
                                case "ECSK"://根据不同的公式定义，执行不同的计算
                                    tem = tminByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                                default:
                                    tem = tminByECSK(stationLists[i].Id, dt, sc, ref error);
                                    break;
                            }
                            stationLists.Add(new StationList()
                            {
                                Id = stationLists[i].Id,
                                tminGS = gs,
                                Tmin = tem,
                            });
                        }
                    }
                }
                else
                {
                    switch (tmings)
                    {
                        case "ECSK"://根据不同的公式定义，执行不同的计算
                            stationLists[i].Tmin = tminByECSK(stationLists[i].Id, dt, sc, ref error);
                            break;
                        default:
                            stationLists[i].Tmin = tminByECSK(stationLists[i].Id, dt, sc, ref error);
                            break;

                    }
                }

            }
            return YBRK(stationLists, dt, sc, ref error);
        }
        public bool YBRK(List<StationList> stationLists, DateTime dt, int sc, ref string error)
        {
            bool insertBS = false;
            foreach (StationList stationList in stationLists)
            {
                if(stationList.TEM!=null&& TEMRK(stationList, dt, sc, ref error))
                {
                    insertBS = true;

                }
                if(stationList.Tmax!=null&& TMaxRK(stationList, dt, sc, ref error))
                {
                    insertBS = true;
                }
                if(stationList.Tmin!=null && TMinRK(stationList, dt, sc, ref error))
                    {
                        insertBS = true;
                    }
            }
            return insertBS;
        }

        public bool TEMRK(StationList stationList, DateTime dt, int sc,ref string error)
        {
            bool insertBS = false;
            try
            {
                string sqlInsert = "insert into YB_TEM" + "(StatioID,Date,SC,SX,TEM,GS) VALUES(@id,@date,@sc,@sx,@tem,@gs)";
                string sqlupdate = "update YB_TEM set  TEM=@tem where StatioID=@id and date=@date and sc=@sc and sx=@sx and gs=@gs";
                Stopwatch sw = new Stopwatch();
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    int jlCount = 0;
                    foreach(ECList eCList in stationList.TEM)
                    {
                        if (eCList.ys == 888888)//对于不进行计算的时次不做入库处理
                            continue;
                        jlCount = 0;
                        using (SqlCommand sqlman = new SqlCommand(sqlInsert, mycon))
                        {
                            sqlman.Parameters.AddWithValue("@id", stationList.Id);
                            sqlman.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                            sqlman.Parameters.AddWithValue("@sc", sc);
                            sqlman.Parameters.AddWithValue("@tem", eCList.ys);
                            sqlman.Parameters.AddWithValue("@sx", eCList.sx);
                            sqlman.Parameters.AddWithValue("@gs", stationList.temGS);
                            sw.Start();
                            try
                            {
                                jlCount = sqlman.ExecuteNonQuery();
                            }
                            catch 
                            {
                               
                            }
                        }
                        if (jlCount == 0)
                        {
                            using (SqlCommand sqlman = new SqlCommand(sqlupdate, mycon))
                            {
                                sqlman.Parameters.AddWithValue("@id", stationList.Id);
                                sqlman.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                                sqlman.Parameters.AddWithValue("@sc", sc);
                                sqlman.Parameters.AddWithValue("@tem", eCList.ys);
                                sqlman.Parameters.AddWithValue("@sx", eCList.sx);
                                sqlman.Parameters.AddWithValue("@gs", stationList.temGS);
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
                        else
                        {
                            insertBS = true;
                        }
                    }
                    
                }

            }
            catch (Exception ex)
            {
                error += ex.Message + "\r\n";
            }
            return insertBS;
        }
        public bool TMinRK(StationList stationList, DateTime dt, int sc, ref string error)
        {
            bool insertBS = false;
            try
            {
                string sqlInsert = "insert into YB_TMIN" + "(StatioID,Date,SC,SX,TMIN,GS) VALUES(@id,@date,@sc,@sx,@tem,@gs)";
                string sqlupdate = "update YB_TMIN set  TMIN=@tem where StatioID=@id and date=@date and sc=@sc and sx=@sx and gs=@gs";
                Stopwatch sw = new Stopwatch();
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    int jlCount = 0;
                    foreach (ECList eCList in stationList.Tmin)
                    {
                        if (eCList.ys == 888888)//对于不进行计算的时次不做入库处理
                            continue;
                        jlCount = 0;
                        using (SqlCommand sqlman = new SqlCommand(sqlInsert, mycon))
                        {
                            sqlman.Parameters.AddWithValue("@id", stationList.Id);
                            sqlman.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                            sqlman.Parameters.AddWithValue("@sc", sc);
                            sqlman.Parameters.AddWithValue("@tem", Math.Round(eCList.ys, 2));
                            sqlman.Parameters.AddWithValue("@sx", eCList.sx);
                            sqlman.Parameters.AddWithValue("@gs", stationList.tmaxGS);
                            sw.Start();
                            try
                            {
                                jlCount = sqlman.ExecuteNonQuery();
                            }
                            catch
                            {

                            }
                        }
                        if (jlCount == 0)
                        {
                            using (SqlCommand sqlman = new SqlCommand(sqlupdate, mycon))
                            {
                                sqlman.Parameters.AddWithValue("@id", stationList.Id);
                                sqlman.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                                sqlman.Parameters.AddWithValue("@sc", sc);
                                sqlman.Parameters.AddWithValue("@tem", Math.Round(eCList.ys, 2));
                                sqlman.Parameters.AddWithValue("@sx", eCList.sx);
                                sqlman.Parameters.AddWithValue("@gs", stationList.tmaxGS);
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
                        else
                        {
                            insertBS = true;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                error += ex.Message + "\r\n";
            }
            return insertBS;
        }
        public bool TMaxRK(StationList stationList, DateTime dt, int sc, ref string error)
        {
            bool insertBS = false;
            try
            {
                string sqlInsert = "insert into YB_TMAX" + "(StatioID,Date,SC,SX,TMAX,GS) VALUES(@id,@date,@sc,@sx,@tem,@gs)";
                string sqlupdate = "update YB_TMAX set  TMAX=@tem where StatioID=@id and date=@date and sc=@sc and sx=@sx and gs=@gs";
                Stopwatch sw = new Stopwatch();
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    int jlCount = 0;
                    foreach (ECList eCList in stationList.Tmax)
                    {
                        if (eCList.ys == 888888)//对于不进行计算的时次不做入库处理
                            continue;
                        jlCount = 0;
                        using (SqlCommand sqlman = new SqlCommand(sqlInsert, mycon))
                        {
                            sqlman.Parameters.AddWithValue("@id", stationList.Id);
                            sqlman.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                            sqlman.Parameters.AddWithValue("@sc", sc);
                            sqlman.Parameters.AddWithValue("@tem", Math.Round(eCList.ys,2));
                            sqlman.Parameters.AddWithValue("@sx", eCList.sx);
                            sqlman.Parameters.AddWithValue("@gs", stationList.tmaxGS);
                            sw.Start();
                            try
                            {
                                jlCount = sqlman.ExecuteNonQuery();
                            }
                            catch
                            {

                            }
                        }
                        if (jlCount == 0)
                        {
                            using (SqlCommand sqlman = new SqlCommand(sqlupdate, mycon))
                            {
                                sqlman.Parameters.AddWithValue("@id", stationList.Id);
                                sqlman.Parameters.AddWithValue("@date", dt.ToString("yyyy-MM-dd"));
                                sqlman.Parameters.AddWithValue("@sc", sc);
                                sqlman.Parameters.AddWithValue("@tem", Math.Round(eCList.ys, 2));
                                sqlman.Parameters.AddWithValue("@sx", eCList.sx);
                                sqlman.Parameters.AddWithValue("@gs", stationList.tmaxGS);
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
                        else
                        {
                            insertBS = true;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                error += ex.Message + "\r\n";
            }
            return insertBS;
        }
        /// <summary>
        /// 返回通过EC与实况差值计算出的72小时气温预报值，如果值为-999999说明该时效EC缺报，999999为实况缺测，888888为该时效不做预报要求
        /// </summary>
        /// <param name="stationID">站号</param>
        /// <param name="dt">起报时间</param>
        /// <param name="sc">起报时次</param>
        /// <param name="error">返回错误信息</param>
        /// <returns></returns>
        public List<ECList> temByECSK(string stationID,DateTime dt,int sc,ref string error)
        {
            List < SKList > sKLists= new List<SKList>();
            List<ECList> eCLists = new List<ECList>();
            DateTime dtls = Convert.ToDateTime(dt.ToString("yyyy-MM-dd " + sc.ToString().PadLeft(2, '0') + ":00:00"));
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                   
                    string sqlString = "";
                    for(int i=-24;i<0;i=i+3)
                    {
                        DateTime dtls2 = dtls.AddHours(i);
                        sqlString += string.Format("(Date = '{0:yyyy-MM-dd}') AND (sc = '{0:HH" + "}') OR", dtls2);
                    }
                    try
                    {
                        sqlString = sqlString.Substring(0, sqlString.Length - 3);
                    }
                    catch { }
                    string sql = String.Format("select * from SK where StationID='{0}' and (" +sqlString+')', stationID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            sKLists.Add(new SKList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEM")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")),
                                sx=24-Convert.ToInt32(dtls.Subtract(sqlreader.GetDateTime(sqlreader.GetOrdinal("date")).AddHours(sqlreader.GetInt32(sqlreader.GetOrdinal("sc")))).TotalHours)//用24减是为了时效与EC预报对齐
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from EC预报 where StatioID='{0}' and date='{1:yyyy-MM-dd}' and sc='{2}' and sx<'96'", stationID,dt.AddDays(-1),sc);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            eCLists.Add(new ECList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEF0")) ? -999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("TEF0")),
                                sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX"))
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            for(int i=0;i<24;i=i+3)
            {
                ECList eCList= eCLists.Find(y=>y.sx==i);
                if (eCList == null)
                {
                    eCLists.Add(new ECList()
                    {
                        ys = -999999,
                        sx = i
                    });
                    eCList = eCLists.Find(y => y.sx == i);
                }
                SKList sKList = sKLists.Find(y => y.sx == i);
                int count = eCLists.IndexOf(eCList);
                if (sKList==null)
                {
                    eCLists[count].ys = 999999;
                    error += string.Format("{0}站{1:yyyy年MM月dd日}{2}时实况缺测，未对{3}、{4}、{5}小时EC进行订正\r\n",stationID,dtls.AddDays(-1).AddHours(i), dtls.AddDays(-1).AddHours(i).Hour,i+24,i+48, i+72);
                }
                else if(sKList.ys== 999999)
                {
                    eCLists[count].ys = 999999;
                    error += string.Format("{0}站{1:yyyy年MM月dd日}{2}时实况缺测，未对{3}、{4}、{5}小时EC进行订正\r\n", stationID, dtls.AddDays(-1).AddHours(i), dtls.AddDays(-1).AddHours(i).Hour, i + 24, i + 48, i + 72);
                }
                else if(eCLists[count].ys == -999999)
                {
                    eCLists[count].ys = -999999;
                }
                else
                {
                    eCLists[count].ys = sKList.ys- eCLists[count].ys;
                }
            }
            for(int i=24;i<96;i=i+3)
            {
                ECList eCList = eCLists.Find(y => y.sx == i);
                if (eCList == null)
                {
                    eCLists.Add(new ECList()
                    {
                        ys = -999999,
                        sx = i
                    });
                    eCList = eCLists.Find(y => y.sx == i);
                    
                }
                int count = eCLists.IndexOf(eCList);
                ECList eCListCZ= eCLists.Find(y => y.sx == i%24);
                if(eCListCZ.ys==-999999)
                {
                    eCLists[count].ys = eCListCZ.ys;
                }
                else if( eCListCZ.ys == 999999)
                {
                    //如果实况缺测，则保存错误信息，不对该时次EC进行订正
                    eCListCZ.ys = Convert.ToSingle(Math.Round(eCListCZ.ys,2));
                }
                else if(eCLists[count].ys != -999999)
                {
                    eCLists[count].ys = Convert.ToSingle(Math.Round(eCListCZ.ys+ eCLists[count].ys,2));
                }
                if (i >= 72)
                {
                    i = i + 3;
                    eCLists.Add(new ECList()
                    {
                        ys = 888888,
                        sx = i
                    });
                }
                

            }
            List<ECList> eCListsDC = new List<ECList>();
            for(int i=0;i<72;i=i+3)
            {
                ECList eCListLS = eCLists.Find(y => y.sx == i + 24);
                eCListLS.sx = i;
                eCListsDC.Add(eCListLS);
            }
            eCListsDC = eCListsDC.OrderBy(y => y.sx).ToList();
            return eCListsDC;
        }

        public List<ECList> temByGJZNSK(string stationID, DateTime dt, int sc, ref string error)
        {
            List<SKList> sKLists = new List<SKList>();
            List<ECList> eCLists = new List<ECList>();
            DateTime dtls = Convert.ToDateTime(dt.ToString("yyyy-MM-dd " + sc.ToString().PadLeft(2, '0') + ":00:00"));
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开

                    string sqlString = "";
                    for (int i = -24; i < 0; i = i + 3)
                    {
                        DateTime dtls2 = dtls.AddHours(i);
                        sqlString += string.Format("(Date = '{0:yyyy-MM-dd}') AND (sc = '{0:HH" + "}') OR", dtls2);
                    }
                    try
                    {
                        sqlString = sqlString.Substring(0, sqlString.Length - 3);
                    }
                    catch { }
                    string sql = String.Format("select * from SK where StationID='{0}' and (" + sqlString + ')', stationID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            sKLists.Add(new SKList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEM")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")),
                                sx = 24 - Convert.ToInt32(dtls.Subtract(sqlreader.GetDateTime(sqlreader.GetOrdinal("date")).AddHours(sqlreader.GetInt32(sqlreader.GetOrdinal("sc")))).TotalHours)//用24减是为了时效与EC预报对齐
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
            string con2 = util.Read("OtherConfig", "DB");
            using (SqlConnection mycon = new SqlConnection(con2))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID='{0}' and date='{1:yyyy-MM-dd}' and sc='{2}' and sx<'96'", stationID, dt.AddDays(-1), sc);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            eCLists.Add(new ECList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEM")) ? -999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")),
                                sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX"))
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            for (int i = 0; i < 24; i = i + 3)
            {
                ECList eCList = eCLists.Find(y => y.sx == i);
                if (eCList == null)
                {
                    eCLists.Add(new ECList()
                    {
                        ys = -999999,
                        sx = i
                    });
                    eCList = eCLists.Find(y => y.sx == i);
                }
                SKList sKList = sKLists.Find(y => y.sx == i);
                int count = eCLists.IndexOf(eCList);
                if (sKList == null)
                {
                    eCLists[count].ys = 999999;
                    error += string.Format("{0}站{1:yyyy年MM月dd日}{2}时实况缺测，未对{3}、{4}、{5}小时EC进行订正\r\n", stationID, dtls.AddDays(-1).AddHours(i), dtls.AddDays(-1).AddHours(i).Hour, i + 24, i + 48, i + 72);
                }
                else if (sKList.ys == 999999)
                {
                    eCLists[count].ys = 999999;
                    error += string.Format("{0}站{1:yyyy年MM月dd日}{2}时实况缺测，未对{3}、{4}、{5}小时EC进行订正\r\n", stationID, dtls.AddDays(-1).AddHours(i), dtls.AddDays(-1).AddHours(i).Hour, i + 24, i + 48, i + 72);
                }
                else if (eCLists[count].ys == -999999)
                {
                    eCLists[count].ys = -999999;
                }
                else
                {
                    eCLists[count].ys = sKList.ys - eCLists[count].ys;
                }
            }
            for (int i = 24; i < 96; i = i + 3)
            {
                ECList eCList = eCLists.Find(y => y.sx == i);
                if (eCList == null)
                {
                    eCLists.Add(new ECList()
                    {
                        ys = -999999,
                        sx = i
                    });
                    eCList = eCLists.Find(y => y.sx == i);

                }
                int count = eCLists.IndexOf(eCList);
                ECList eCListCZ = eCLists.Find(y => y.sx == i % 24);
                if (eCListCZ.ys == -999999)
                {
                    eCLists[count].ys = eCListCZ.ys;
                }
                else if (eCListCZ.ys == 999999)
                {
                    //如果实况缺测，则保存错误信息，不对该时次EC进行订正
                    eCListCZ.ys = Convert.ToSingle(Math.Round(eCListCZ.ys, 2));
                }
                else if (eCLists[count].ys != -999999)
                {
                    eCLists[count].ys = Convert.ToSingle(Math.Round(eCListCZ.ys + eCLists[count].ys, 2));
                }
                if (i >= 72)
                {
                    i = i + 3;
                    eCLists.Add(new ECList()
                    {
                        ys = 888888,
                        sx = i
                    });
                }


            }
            List<ECList> eCListsDC = new List<ECList>();
            for (int i = 0; i < 72; i = i + 3)
            {
                ECList eCListLS = eCLists.Find(y => y.sx == i + 24);
                eCListLS.sx = i;
                eCListsDC.Add(eCListLS);
            }
            eCListsDC = eCListsDC.OrderBy(y => y.sx).ToList();
            return eCListsDC;
        }

        public List<ECList> tmaxByECSK(string stationID, DateTime dt, int sc, ref string error)
        {
            List<SKList> sKLists = new List<SKList>();
            List<ECList> eCLists = new List<ECList>();
            List<ECList> ecTmaxLists = new List<ECList>();
            SKList sKTmax = new SKList();
            DateTime dtls = Convert.ToDateTime(dt.ToString("yyyy-MM-dd " + sc.ToString().PadLeft(2, '0') + ":00:00"));
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开

                    string sqlString = "";
                    for (int i = -24; i < 0; i ++)
                    {
                        DateTime dtls2 = dtls.AddHours(i);
                        sqlString += string.Format("(Date = '{0:yyyy-MM-dd}') AND (sc = '{0:HH" + "}') OR", dtls2);
                    }
                    try
                    {
                        sqlString = sqlString.Substring(0, sqlString.Length - 3);
                    }
                    catch { }
                    string sql = String.Format("select * from SK where Tmax<200 and Tmax>-200 and StationID='{0}' and (" + sqlString + ')', stationID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            sKLists.Add(new SKList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("Tmax")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax")),
                                sx = 24 - Convert.ToInt32(dtls.Subtract(sqlreader.GetDateTime(sqlreader.GetOrdinal("date")).AddHours(sqlreader.GetInt32(sqlreader.GetOrdinal("sc")))).TotalHours)//用24减是为了时效与EC预报对齐
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from EC预报 where StatioID='{0}' and date='{1:yyyy-MM-dd}' and sc='{2}' and sx<'96'", stationID, dt.AddDays(-1), sc);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            eCLists.Add(new ECList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEF0")) ? -999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("TEF0")),
                                sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX"))
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            List<ECList> listLS = eCLists.FindAll(y => y.sx < 24);
            float tmaxls = -999999;
            foreach(ECList eCList in listLS)
            {
                if (eCList.ys > tmaxls)
                    tmaxls = eCList.ys;
            }
            ecTmaxLists.Add(new ECList()
            {
                ys = tmaxls,
                sx = 24
            });
            listLS = eCLists.FindAll(y => y.sx >= 24 && y.sx <48);
            tmaxls = -999999;
            foreach (ECList eCList in listLS)
            {
                if (eCList.ys > tmaxls)
                    tmaxls = eCList.ys;
            }
            ecTmaxLists.Add(new ECList()
            {
                ys = tmaxls,
                sx = 48
            });
            listLS = eCLists.FindAll(y => y.sx >= 48 && y.sx < 72);
            tmaxls = -999999;
            foreach (ECList eCList in listLS)
            {
                if (eCList.ys > tmaxls)
                    tmaxls = eCList.ys;
            }
            ecTmaxLists.Add(new ECList()
            {
                ys = tmaxls,
                sx = 72
            });
            listLS = eCLists.FindAll(y => y.sx >= 72 && y.sx < 96);
            tmaxls = -999999;
            foreach (ECList eCList in listLS)
            {
                if (eCList.ys > tmaxls)
                    tmaxls = eCList.ys;
            }
            ecTmaxLists.Add(new ECList()
            {
                ys = tmaxls,
                sx = 96
            });
            List<SKList> sklistLS = sKLists.FindAll(y => y.sx < 24);
            tmaxls = -999999;
            foreach (SKList sk in sKLists)
            {
                if (sk.ys > tmaxls)
                    tmaxls = sk.ys;
            }
            sKTmax.sx = 24;
            sKTmax.ys = tmaxls;
            ECList eCListLS = ecTmaxLists.Find(y => y.sx == 24);
            ecTmaxLists[ecTmaxLists.IndexOf(eCListLS)].ys = sKTmax.ys - eCListLS.ys;
            eCListLS = ecTmaxLists.Find(y => y.sx == 24);
            foreach (ECList ee in ecTmaxLists)
            {
                if(ee.sx>24)
                {
                    int count = ecTmaxLists.IndexOf(ee);
                    ecTmaxLists[count].ys += eCListLS.ys;
                }
            }
            eCLists = new List<ECList>();
            foreach (ECList ee in ecTmaxLists)
            {
                
                if (ee.sx > 24)
                {
                    if (ee.ys > 200 || ee.ys < -200)
                    {
                        error += stationID+dt.ToString("yyyy年MM月dd日")+(ee.sx - 24) + "小时高温异常\r\n";
                    }
                        eCLists.Add(new ECList()
                    {
                        ys = ee.ys,
                        sx = ee.sx-24
                    });
                }
            }
            eCLists = eCLists.OrderBy(y => y.sx).ToList();
            return eCLists;
        }

        public List<ECList> tminByECSK(string stationID, DateTime dt, int sc, ref string error)
        {
            List<SKList> sKLists = new List<SKList>();
            List<ECList> eCLists = new List<ECList>();
            List<ECList> ecTminLists = new List<ECList>();
            SKList sKTmin = new SKList();
            DateTime dtls = Convert.ToDateTime(dt.ToString("yyyy-MM-dd " + sc.ToString().PadLeft(2, '0') + ":00:00"));
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开

                    string sqlString = "";
                    for (int i = -24; i < 0; i++)
                    {
                        DateTime dtls2 = dtls.AddHours(i);
                        sqlString += string.Format("(Date = '{0:yyyy-MM-dd}') AND (sc = '{0:HH" + "}') OR", dtls2);
                    }
                    try
                    {
                        sqlString = sqlString.Substring(0, sqlString.Length - 3);
                    }
                    catch { }
                    string sql = String.Format("select * from SK where Tmin<200 and Tmin>-200 and StationID='{0}' and (" + sqlString + ')', stationID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            sKLists.Add(new SKList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("Tmin")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin")),
                                sx = 24 - Convert.ToInt32(dtls.Subtract(sqlreader.GetDateTime(sqlreader.GetOrdinal("date")).AddHours(sqlreader.GetInt32(sqlreader.GetOrdinal("sc")))).TotalHours)//用24减是为了时效与EC预报对齐
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from EC预报 where StatioID='{0}' and date='{1:yyyy-MM-dd}' and sc='{2}' and sx<'96'", stationID, dt.AddDays(-1), sc);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            eCLists.Add(new ECList()
                            {
                                ys = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEF0")) ? -999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("TEF0")),
                                sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX"))
                            });
                        }
                        catch (Exception ex)
                        {
                            error += ex.Message + "\r\n";
                        }
                    }


                }
                catch (Exception ex)
                {

                    error += ex.Message + "\r\n";
                }
            }
            List<ECList> listLS = eCLists.FindAll(y => y.sx < 24);
            float tminls = 999999;
            foreach (ECList eCList in listLS)
            {
                if (eCList.ys < tminls)
                    tminls = eCList.ys;
            }
            ecTminLists.Add(new ECList()
            {
                ys = tminls,
                sx = 24
            });
            listLS = eCLists.FindAll(y => y.sx >= 24 && y.sx < 48);
            tminls = 999999;
            foreach (ECList eCList in listLS)
            {
                if (eCList.ys < tminls)
                    tminls = eCList.ys;
            }
            ecTminLists.Add(new ECList()
            {
                ys = tminls,
                sx = 48
            });
            listLS = eCLists.FindAll(y => y.sx >= 48 && y.sx < 72);
            tminls = 999999;
            foreach (ECList eCList in listLS)
            {
                if (eCList.ys < tminls)
                    tminls = eCList.ys;
            }
            ecTminLists.Add(new ECList()
            {
                ys = tminls,
                sx = 72
            });
            listLS = eCLists.FindAll(y => y.sx >= 72 && y.sx < 96);
            tminls = 999999;
            foreach (ECList eCList in listLS)
            {
                if (eCList.ys < tminls)
                    tminls = eCList.ys;
            }
            ecTminLists.Add(new ECList()
            {
                ys = tminls,
                sx = 96
            });
            List<SKList> sklistLS = sKLists.FindAll(y => y.sx < 24);
            tminls = 999999;
            foreach (SKList sk in sKLists)
            {
                if (sk.ys < tminls)
                    tminls = sk.ys;
            }
            sKTmin.sx = 24;
            sKTmin.ys = tminls;
            ECList eCListLS = ecTminLists.Find(y => y.sx == 24);
            ecTminLists[ecTminLists.IndexOf(eCListLS)].ys = sKTmin.ys - eCListLS.ys;
            eCListLS = ecTminLists.Find(y => y.sx == 24);
            foreach (ECList ee in ecTminLists)
            {
                if (ee.sx > 24)
                {
                    int count = ecTminLists.IndexOf(ee);
                    ecTminLists[count].ys += eCListLS.ys;
                }
            }
            eCLists = new List<ECList>();
            foreach (ECList ee in ecTminLists)
            {

                if (ee.sx > 24)
                {
                    if (ee.ys > 200 || ee.ys < -200)
                    {
                        error += stationID + dt.ToString("yyyy年MM月dd日") + (ee.sx - 24) + "小时低温异常\r\n";
                    }
                    eCLists.Add(new ECList()
                    {
                        ys = ee.ys,
                        sx = ee.sx - 24
                    });
                }
            }
            eCLists = eCLists.OrderBy(y => y.sx).ToList();
            return eCLists;
        }

        public class SKList
        {
            public int sx { get; set; }
            public float ys { get; set; }

        }
        public class ECList
        {
            public int sx { get; set; }
            public float ys { get; set; }

        }
        public class WindList
        {
            public string stationID { get; set; }
            public DateTime date { get; set; }
            public int sx { get; set; }
            public int sc { get; set; }
            public double fs { get; set; }
            public double fx { get; set; }
            public DateTime ybDate { get; set; }

        }

        public class StationList
        {
            public string Id { get; set; }
            public List<ECList> TEM { get; set; }
            public List<ECList> Tmax { get; set; }
            public List<ECList> Tmin { get; set; }
            public float[] PRE { get; set; }
            public float[] FS { get; set; }
            public string[] FX { get; set; }
            public string temGS { get; set; }
            public string tmaxGS { get; set; }
            public string tminGS { get; set; }
            public string PREGS { get; set; }
            public string FXGS { get; set; }
            public string FSGS { get; set; }
        }
    }
}
