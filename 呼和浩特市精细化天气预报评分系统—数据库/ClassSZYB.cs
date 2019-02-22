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
            //此处增加风要素
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

        public class SKList
        {
            public string ID { get; set; }
            public double TEM { get; set; }
            public double Tmin { get; set; }
            public double Tmax { get; set; }
            public double Pre { get; set; }
            public double FX10 { get; set; }
            public double FS10 { get; set; }
            public double FXJD { get; set; }
            public double FSJD { get; set; }

        }
    }
}
