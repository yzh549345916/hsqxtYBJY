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

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    class Class1
    {
        string YQH08 = "7", YQM08 = "0";//报文逾期时间
        string YQH20 = "17", YQM20 = "0";//报文逾期时间
        string YQtime20 = "1700", YQtime08 = "0700";
        string con;//这里是保存连接数据库的字符串172.18.142.158 id=sa;password=134679;
        string DBconPath = Environment.CurrentDirectory + @"\config\DBconfig.txt";
        public string QXID = "",QXName="";
        string configpathPath = Environment.CurrentDirectory + @"\config\pathConfig.txt";

        public Class1()
        {
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        con = line.Substring("sql管理员=".Length);
                        
                    }
                    else if (line.Split('=')[0] == "逾期08小时")
                    {
                        YQH08 = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "逾期08分钟")
                    {
                        YQM08 = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "逾期20小时")
                    {
                        YQH20 = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "逾期20分钟")
                    {
                        YQM20 = line.Split('=')[1];
                    }
                   

                }
                while (YQH08.Length < 2)
                {
                    YQH08 = '0' + YQH08; //如果时间设置不足两位用0补位
                }
                while (YQM08.Length < 2)
                {
                    YQM08 = '0' + YQM08;
                }
                while (YQH20.Length < 2)
                {
                    YQH20 = '0' + YQH20; //如果时间设置不足两位用0补位
                }
                while (YQM20.Length < 2)
                {
                    YQM20 = '0' + YQM20;
                }
                YQtime08 = YQH08 + YQM08;
                YQtime20 = YQH20 + YQM20;

            }
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//打开
                try
                {
                    string sql = string.Format(@"select * from QXList ");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        QXID += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + ',';
                        QXName += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + ',';
                    }
                    QXID = QXID.Substring(0, QXID.Length - 1);
                    QXName = QXName.Substring(0, QXName.Length - 1);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        public string ZYZDCIMISS(string date,string sc)
        {
            string strfh = "";
            string TQPath = Environment.CurrentDirectory + @"\config\天气对照.txt";
            string TQtxt = "";
            using (StreamReader sr = new StreamReader(TQPath, Encoding.Default))
            {
                TQtxt = sr.ReadToEnd();

            }
            string[] lssz1 = Regex.Split(TQtxt, "\r\n", RegexOptions.IgnoreCase);
            string[,] TQSZ = new string[lssz1.Length, 2];
            for (int i = 0; i < lssz1.Length; i++)
            {
                string[] lssz2 = lssz1[i].Split('=');
                TQSZ[i, 0] = lssz2[0];
                TQSZ[i, 1] = lssz2[1];
            }
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId = "getSevpEleInRegionByTime";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> params1 = new Dictionary<String, String>();
            // 必选参数
            params1.Add("dataCode", "SEVP_CHN_WEFC_RFFC"); // 资料代码
            params1.Add("elements", "Station_Id_C,Bul_Center,Prod_Code,Validtime,TEM_Max_24h,TEM_Min_24h,WEP_Past_12h");// 检索要素：站号、产品代码、预报24小时最高、最低温度
            string times = date;
            if (sc == "08")
                times += "000000";
            else
                times += "120000";
            
            params1.Add("times", times); // 检索时间
            params1.Add("adminCodes", "150100");
            params1.Add("eleValueRanges", "Prod_Code:SCMOC"); // 筛选产品代码：SMOC为中央台指导产品
            /*   2.4 返回文件的格式 */
            String dataFormat = "text";
            /*   2.5 文件的本地全路径 */
            //String savePath = @"F:\Users\yzh54\Desktop\123.txt";
            /*   2.6 返回文件的描述对象 */
            RetFilesInfo retFilesInfo = new RetFilesInfo();
            StringBuilder retStr = new StringBuilder();//返回字符串

            try
            {
                string data1 = "";
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId, params1, dataFormat, retStr);
                // 输出结果
                if (rst == 0)
                {   // 正常返回
                    
                    data1 = Convert.ToString(retStr);
                    string dataLS = "";
                    string[] ybhsj1 = Regex.Split(data1, "\r\n", RegexOptions.IgnoreCase);//按行读取指导数据
                    for (int j = 0; j < ybhsj1.Length - 2; j++)
                    {
                        if (j == 0)
                        {
                            dataLS += ybhsj1[j] + ' ';
                        }
                        else if (j == 1)
                        {
                            dataLS += ybhsj1[j] + '\n';
                        }
                        else
                        {

                            try
                            {
                                int intLS = Convert.ToInt32(ybhsj1[j].Split()[3]);
                                if (intLS % 12 == 0 && intLS <= 120)
                                {
                                    if (intLS % 24 != 0)
                                    {
                                        dataLS += ybhsj1[j] + ' ';
                                    }
                                    else
                                    {
                                        dataLS += ybhsj1[j] + '\n';
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message);
                            }


                        }


                    }
                    dataLS = dataLS.Substring(0, dataLS.Length - 1);
                    string[] ybhsj = dataLS.Split('\n');
                    string[] idsz = QXID.Split(',');
                    string strYb = "";
                    for (int i = 0; i < idsz.Length; i++)//按区站号排序
                    {
                        strYb += times + '\t' + idsz[i] + '\t';
                        int aa = 0;
                        for (int j = 1; j < ybhsj.Length; j++)
                        {
                            string[] yblsj = ybhsj[j].Split(' ');


                            try
                            {
                                if (yblsj[0] == idsz[i])
                                {
                                    if (yblsj[6].Length < 2)
                                        yblsj[6] = '0' + yblsj[6];
                                    if (yblsj[13].Length < 2)
                                        yblsj[13] = '0' + yblsj[13];
                                    for (int k = 0; k < TQSZ.GetLength(0); k++)
                                    {
                                        if (yblsj[6] == TQSZ[k, 1])
                                            yblsj[6] = TQSZ[k, 0];
                                        if (yblsj[13] == TQSZ[k, 1])
                                            yblsj[13] = TQSZ[k, 0];
                                    }
                                    aa++;
                                    if (yblsj[6] != yblsj[13])
                                        strYb += yblsj[12] + '\t' + yblsj[11] + '\t' + yblsj[6] + "转" + yblsj[13] +
                                                 '\t';
                                    else
                                        strYb += yblsj[12] + '\t' + yblsj[11] + '\t' + yblsj[6] + '\t';
                                    if (aa >= 5)
                                        break;
                                }
                            }
                            catch (Exception wx)
                            {
                               // MessageBox.Show(wx.Message);
                            }

                        }

                        strYb = strYb.Substring(0, strYb.Length - 1);
                        strYb += '\n';

                    }
                    strYb = strYb.Substring(0, strYb.Length - 1);
                    string[] szls = strYb.Split('\n');
                    string datestr = date.Substring(0, 4) + '-' + date.Substring(4, 2) + '-' + date.Substring(6, 2);
                    string sqlStr = String.Format(@"insert into ZYZD (StationID, Date, SC, Tmax24,Tmin24,Rain24,Tmax48,Tmin48,Rain48,Tmax72,Tmin72,Rain72,Tmax96,Tmin96,Rain96,Tmax120,Tmin120,Rain120) values ");
                    for (int i = 0; i < szls.Length; i++)
                    {
                        string[] szls2 = szls[i].Split('\t');
                        sqlStr += String.Format(@"('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}') , ", szls2[1], datestr,sc,szls2[3], szls2[2], szls2[4], szls2[6], szls2[5], szls2[7], szls2[9], szls2[8], szls2[10], szls2[12], szls2[11], szls2[13], szls2[15], szls2[14], szls2[16]);
                    }
                    sqlStr = sqlStr.Substring(0, sqlStr.Length - 2);
                    int jlCount = 0;
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        try
                        {
                            mycon.Open();//打开
                            SqlCommand sqlman = new SqlCommand(sqlStr, mycon);
                            jlCount = sqlman.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                           // strfh += ex.Message+"\r\n";
                        }
                        if (jlCount == 0)
                        {
                            strfh += date + "日" + sc + "时中央指导预报已入库，入库失败\r\n";
                        }
                        else
                        {
                            strfh += date + "日" + sc + "时中央指导预报成功入库\r\n";
                        }
                    }

                        return strfh;

                }
                else
                {   // 异常返回
                   
                    return "error";
                }
            }
            catch (Exception e)
            {
                // 异常输出
                return e.Message;
                //e.Message();
            }
            finally
            {
                // 释放接口服务连接资源
                client.destroyResources();

            }
            //Console.ReadKey();
            return strfh;
        }
        public string TJRK(DateTime dt,string SC,string GW)//统计数据库初步建立，时间、岗位、时次、人员信息入库
        {
            string strError = "";

            try
            {
                string sql = string.Format(@"select * from USERJL where GW='{0}' AND date='{1:yyyy-MM-dd}'AND SC='{2}'", GW, dt, SC);
                SqlConnection mycon1 = new SqlConnection(con);//创建SQL连接对象
                mycon1.Open();//打开
                string PeopleID = "";
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                PeopleID = sqlreader.GetString(sqlreader.GetOrdinal("userID"));
                            }
                        }

                    }
                    mycon1.Close();


                }
                catch (Exception ex)
                {
                    strError += ex.Message + '\n';
                }
                string[] qxidSZ = QXID.Split(',');
                for (int i = 0; i < qxidSZ.Length; i++)
                {
                    
                    SqlConnection mycon2 = new SqlConnection(con);//创建SQL连接对象
                    mycon2.Open();//打开
                    sql = string.Format(@"insert into TJ (StationID,GW,PeopleID,Date,SC) values('{0}','{1}','{2}','{3}','{4}')", qxidSZ[i], GW, PeopleID, dt.ToString("yyyy-MM-dd"),SC);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon2))
                        {
                            sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数） 
                        }
                        mycon2.Close();



                    }
                    catch (Exception ex)
                    {
                       // strError += "新建" + dt.ToString("yyyy-MM-dd") + "日" + qxidSZ[i] + "的数据库统计信息字段失败，原因为：" + ex.Message + "\r\n";
                    }

                    strError += TJCZRK(qxidSZ[i], dt,SC,GW);
                }
            }
            catch(Exception ex)
            {
                strError += ex.Message + '\n';
            }

            return strError;
        }

        public string TJCZRK(string QXID, DateTime dt,string SC,string GW) //指定时间的24小时预报与实况差值入库 dt为预报时间 返回错误信息
        {
            string strError = "";
            float SKTmax = 999999, SKTmin = 999999;
            float SKRain = 999999;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//打开
                DateTime SKtime = dt.AddDays(1);//预报时间加一天的实况为24小时实况时间
                string sql = string.Format(@"select * from SK where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, SKtime.ToString("yyyy-MM-dd"),SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SKTmax = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax"));
                                SKTmin = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin"));
                                SKRain = sqlreader.GetFloat(sqlreader.GetOrdinal("Rain"));
                                if (SKRain == 999990)//CIMISS 999990为微量降水，计算时按照无降水计算
                                {
                                    SKRain = 0;
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += "获取" + QXID + "的" + SKtime.ToString("yyyy-MM-dd") + "实况失败：" + ex.Message + '\n';
                }
                float SJTmax24 = -999999, SJTmin24 = -999999;
                float SJRain24 = -999999;
                int SJBWTime = 2359;
                int intYQtime = 0;//设置的逾期时间转换为数字，便于后续比较
                if (SC == "08")
                {
                    intYQtime = Convert.ToInt32(YQtime08);
                }
                else if (SC == "20")
                    intYQtime = Convert.ToInt32(YQtime20);
                bool SFZD24 = false;
                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"),SC,GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax24"));
                                SJTmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin24"));
                                SJBWTime = Convert.ToInt32(sqlreader.GetDateTime(sqlreader.GetOrdinal("YBtime")).ToString("HHmm"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain24"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain24 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain24 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "24小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                if (SJBWTime <= intYQtime)
                {
                    SFZD24 = true;
                }
                float SJSKRain24 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    SJSKRain24 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain24 == -999999)
                    {
                        SJSKRain24 = SJRain24;
                    }
                    else
                    {
                        if (SKRainLS == SJRain24)
                        {
                            SJSKRain24 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain24 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin24 = 0, SJSKTmax24 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax24 = SKTmax;
                }
                else
                {
                    if (SJTmax24 == -999999)
                    {
                        SJSKTmax24 = SJTmax24;
                    }
                    else
                    {
                        SJSKTmax24 = Math.Round(SJTmax24 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin24 = SKTmin;
                }
                else
                {
                    if (SJTmin24 == -999999)
                    {
                        SJSKTmin24 = SJTmin24;
                    }
                    else
                    {
                        SJSKTmin24 = Math.Round(SJTmin24 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set SJ_SKTmax24='{0}',SJ_SKTmin24='{1}',SJ_Rain24='{2}',SFzhundian='{3}' where StationID='{4}' and Date='{5}'and SC='{6}'and GW='{7}'", SJSKTmax24, SJSKTmin24, SJSKRain24,SFZD24, QXID, dt.ToString("yyyy-MM-dd"),SC,GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "24小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax24 = -999999, ZYTmin24 = -999999;
                float ZYRain24 = -999999;
                
                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"),SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax24"));
                                ZYTmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin24"));

                                
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain24"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain24 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain24 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "24小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain24 == -999999)
                    {
                        ZYSKRain = ZYRain24;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain24)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin24 = 0, ZYSKTmax24 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax24 = SKTmax;
                }
                else
                {
                    if (ZYTmax24 == -999999)
                    {
                        ZYSKTmax24 = ZYTmax24;
                    }
                    else
                    {
                        ZYSKTmax24 = Math.Round(ZYTmax24 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin24 = SKTmin;
                }
                else
                {
                    if (ZYTmin24 == -999999)
                    {
                        ZYSKTmin24 = ZYTmin24;
                    }
                    else
                    {
                        ZYSKTmin24 = Math.Round(ZYTmin24 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set ZY_SKTmax24='{0}',ZY_SKTmin24='{1}',ZY_Rain24='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", ZYSKTmax24, ZYSKTmin24, ZYSKRain, QXID, dt.ToString("yyyy-MM-dd"),SC,GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "24小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #region//前一天48小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax48 = -999999, SJTmin48 = -999999;
                float SJRain48 = -999999;
                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax48"));
                                SJTmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin48"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain48"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain48 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain48 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "48小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    SJSKRain48 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain48 == -999999)
                    {
                        SJSKRain48 = SJRain48;
                    }
                    else
                    {
                        if (SKRainLS == SJRain48)
                        {
                            SJSKRain48 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain48 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin48 = 0, SJSKTmax48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax48 = SKTmax;
                }
                else
                {
                    if (SJTmax48 == -999999)
                    {
                        SJSKTmax48 = SJTmax48;
                    }
                    else
                    {
                        SJSKTmax48 = Math.Round(SJTmax48 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin48 = SKTmin;
                }
                else
                {
                    if (SJTmin48 == -999999)
                    {
                        SJSKTmin48 = SJTmin48;
                    }
                    else
                    {
                        SJSKTmin48 = Math.Round(SJTmin48 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set SJ_SKTmax48='{0}',SJ_SKTmin48='{1}',SJ_Rain48='{2}'where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", SJSKTmax48, SJSKTmin48, SJSKRain48, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "48小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax48 = -999999, ZYTmin48 = -999999;
                float ZYRain48 = -999999;

                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax48"));
                                ZYTmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin48"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain48"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain48 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain48 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "48小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain48 == -999999)
                    {
                        ZYSKRain = ZYRain48;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain48)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin48 = 0, ZYSKTmax48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax48 = SKTmax;
                }
                else
                {
                    if (ZYTmax48 == -999999)
                    {
                        ZYSKTmax48= ZYTmax48;
                    }
                    else
                    {
                        ZYSKTmax48 = Math.Round(ZYTmax48 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin48 = SKTmin;
                }
                else
                {
                    if (ZYTmin48 == -999999)
                    {
                        ZYSKTmin48 = ZYTmin48;
                    }
                    else
                    {
                        ZYSKTmin48 = Math.Round(ZYTmin48 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set ZY_SKTmax48='{0}',ZY_SKTmin48='{1}',ZY_Rain48='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", ZYSKTmax48, ZYSKTmin48, ZYSKRain, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "48小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
                #region//前两天72小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax72 = -999999, SJTmin72 = -999999;
                float SJRain72 = -999999;
                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax72"));
                                SJTmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin72"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain72"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain72 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain72 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "72小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    SJSKRain72 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain72 == -999999)
                    {
                        SJSKRain72 = SJRain72;
                    }
                    else
                    {
                        if (SKRainLS == SJRain72)
                        {
                            SJSKRain72 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain72 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin72 = 0, SJSKTmax72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax72 = SKTmax;
                }
                else
                {
                    if (SJTmax72 == -999999)
                    {
                        SJSKTmax72 = SJTmax72;
                    }
                    else
                    {
                        SJSKTmax72 = Math.Round(SJTmax72 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin72 = SKTmin;
                }
                else
                {
                    if (SJTmin72 == -999999)
                    {
                        SJSKTmin72 = SJTmin72;
                    }
                    else
                    {
                        SJSKTmin72 = Math.Round(SJTmin72 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set SJ_SKTmax72='{0}',SJ_SKTmin72='{1}',SJ_Rain72='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", SJSKTmax72, SJSKTmin72, SJSKRain72, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "72小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax72 = -999999, ZYTmin72 = -999999;
                float ZYRain72 = -999999;

                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax72"));
                                ZYTmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin72"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain72"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain72 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain72 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "72小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain72 == -999999)
                    {
                        ZYSKRain = ZYRain72;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain72)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin72 = 0, ZYSKTmax72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax72 = SKTmax;
                }
                else
                {
                    if (ZYTmax72 == -999999)
                    {
                        ZYSKTmax72 = ZYTmax72;
                    }
                    else
                    {
                        ZYSKTmax72 = Math.Round(ZYTmax72 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin72 = SKTmin;
                }
                else
                {
                    if (ZYTmin72 == -999999)
                    {
                        ZYSKTmin72 = ZYTmin72;
                    }
                    else
                    {
                        ZYSKTmin72 = Math.Round(ZYTmin72 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set ZY_SKTmax72='{0}',ZY_SKTmin72='{1}',ZY_Rain72='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", ZYSKTmax72, ZYSKTmin72, ZYSKRain, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "72小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
                #region//前三天96小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax96 = -999999, SJTmin96 = -999999;
                float SJRain96 = -999999;
                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax96"));
                                SJTmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin96"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain96"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain96 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain96 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "96小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain96 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    SJSKRain96 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain96 == -999999)
                    {
                        SJSKRain96 = SJRain96;
                    }
                    else
                    {
                        if (SKRainLS == SJRain96)
                        {
                            SJSKRain96 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain96 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin96 = 0, SJSKTmax96 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax96 = SKTmax;
                }
                else
                {
                    if (SJTmax96 == -999999)
                    {
                        SJSKTmax96 = SJTmax96;
                    }
                    else
                    {
                        SJSKTmax96 = Math.Round(SJTmax96 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin96 = SKTmin;
                }
                else
                {
                    if (SJTmin96 == -999999)
                    {
                        SJSKTmin96 = SJTmin96;
                    }
                    else
                    {
                        SJSKTmin96 = Math.Round(SJTmin96 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set SJ_SKTmax96='{0}',SJ_SKTmin96='{1}',SJ_Rain96='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", SJSKTmax96, SJSKTmin96, SJSKRain96, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "96小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax96 = -999999, ZYTmin96 = -999999;
                float ZYRain96 = -999999;

                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax96"));
                                ZYTmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin96"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain96"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain96 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain96 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "96小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYSKRain == -999999)
                    {
                        ZYSKRain = ZYRain96;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain96)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin96 = 0, ZYSKTmax96 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax96 = SKTmax;
                }
                else
                {
                    if (ZYTmax96 == -999999)
                    {
                        ZYSKTmax96 = ZYTmax96;
                    }
                    else
                    {
                        ZYSKTmax96 = Math.Round(ZYTmax96 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin96 = SKTmin;
                }
                else
                {
                    if (ZYTmin96 == -999999)
                    {
                        ZYSKTmin96 = ZYTmin96;
                    }
                    else
                    {
                        ZYSKTmin96 = Math.Round(ZYTmin96 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set ZY_SKTmax96='{0}',ZY_SKTmin96='{1}',ZY_Rain96='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", ZYSKTmax96, ZYSKTmin96, ZYSKRain, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "96小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
                #region//前四天120小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax120 = -999999, SJTmin120 = -999999;
                float SJRain120 = -999999;
                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax120"));
                                SJTmin120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin120"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain120"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain120 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain120 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "120小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain120 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    SJSKRain120 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain120 == -999999)
                    {
                        SJSKRain120 = SJRain120;
                    }
                    else
                    {
                        if (SKRainLS == SJRain120)
                        {
                            SJSKRain120 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain120 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin120 = 0, SJSKTmax120 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax120 = SKTmax;
                }
                else
                {
                    if (SJTmax120 == -999999)
                    {
                        SJSKTmax120 = SJTmax120;
                    }
                    else
                    {
                        SJSKTmax120 = Math.Round(SJTmax120 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin120 = SKTmin;
                }
                else
                {
                    if (SJTmin120 == -999999)
                    {
                        SJSKTmin120 = SJTmin120;
                    }
                    else
                    {
                        SJSKTmin120 = Math.Round(SJTmin120 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set SJ_SKTmax120='{0}',SJ_SKTmin120='{1}',SJ_Rain120='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", SJSKTmax120, SJSKTmin120, SJSKRain120, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "120小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax120 = -999999, ZYTmin120 = -999999;
                float ZYRain120 = -999999;

                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax120"));
                                ZYTmin120 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin120"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain120"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain120 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain120 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "120小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYSKRain == -999999)
                    {
                        ZYSKRain = ZYRain120;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain120)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin120 = 0, ZYSKTmax120 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax120 = SKTmax;
                }
                else
                {
                    if (ZYTmax120 == -999999)
                    {
                        ZYSKTmax120 = ZYTmax120;
                    }
                    else
                    {
                        ZYSKTmax120 = Math.Round(ZYTmax120 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin120 = SKTmin;
                }
                else
                {
                    if (ZYTmin120 == -999999)
                    {
                        ZYSKTmin120 = ZYTmin120;
                    }
                    else
                    {
                        ZYSKTmin120 = Math.Round(ZYTmin120 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set ZY_SKTmax120='{0}',ZY_SKTmin120='{1}',ZY_Rain120='{2}' where StationID='{3}' and Date='{4}'and SC='{5}'and GW='{6}'", ZYSKTmax120, ZYSKTmin120, ZYSKRain, QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "120小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
            }



            return strError;
        }
        public string Sjyb(string YBDate, string strTime,string GW)
        {
            string ss = "";
            string line = "";
            int JLGS = 0, SucGS = 0;//统计应该入库的记录总个数与成功入库的个数.
            
            string YBpath = "";
            try
            {
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "市台预报市局路径")
                        {
                            YBpath = line.Split('=')[1];
                            break;
                        }
                    }
                }
               
                string BWBS = "";
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\报文标识.txt",Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0]==GW)
                        {
                            BWBS = line.Split('=')[1];
                        }
                    }
                }

                    string strParPath = "";
                    if (strTime == "08")
                    {
                         strParPath = "*" + BWBS  + "*-SPCC-" + YBDate + "00" + "*";
                    }
                    else
                        strParPath = "*" + BWBS + "*-SPCC-" + YBDate + "12" + "*";
                    string[] fileNameList = Directory.GetFiles(YBpath, strParPath);
                if (fileNameList.Length <= 0)
                {
                    string HSBWBS = "";
                    using (StreamReader sr1 = new StreamReader(Environment.CurrentDirectory + @"\config\报文标识.txt",
                        Encoding.Default))
                    {
                        string line1 = "";
                        while ((line1 = sr1.ReadLine()) != null)
                        {
                            if (line1.Split('=')[0] == "呼市气象台")
                            {
                                HSBWBS = line1.Split('=')[1];
                                break;
                            }
                        }
                    }
                    string strPar = "";
                    if (strTime == "08")
                    {
                        strPar = "*" + HSBWBS + "*-SPCC-" + YBDate + "00" + "*";
                    }
                    else
                        strPar = "*" + HSBWBS + "*-SPCC-" + YBDate + "12" + "*";
                    string[] fileNameList1 = Directory.GetFiles(YBpath, strPar);
                    if (fileNameList1.Length <= 0)
                    {
                        SJYBTB(YBDate,strTime);
                        fileNameList1 = Directory.GetFiles(YBpath, strPar);
                    }
                    if (fileNameList1.Length > 0)
                    {
                        BWTB(YBDate, strTime,GW);
                        fileNameList = Directory.GetFiles(YBpath, strParPath);
                    }

                }
                if (fileNameList.Length > 0)
                {
                    Int16 maxXH = 0, minXH = 0; //maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS; //maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    //寻找指定日期中该旗县的最晚和最早报文在fileNameList文件列表中的序号，最晚报文的全路径的为fileNameList[maxXH]，最早的为fileNameList[minXH]
                    for (Int16 j = 0; j < fileNameList.Length; j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt16(strLS.Substring(strLS.Length - 2));
                        if (j == 0)
                        {
                            maxLS = intLS;
                            minLS = intLS;
                        }
                        else
                        {
                            if (intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if (intLS < minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }
                    DateTime YBtime = File.GetLastWriteTime(fileNameList[minXH]);
                    int lineCount = 0;
                    int intCount = 0;
                    using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                    {


                        while ((line = sr.ReadLine()) != null)
                        {
                            if (lineCount == 4)
                            {
                                intCount = Convert.ToInt32(line);
                                break;
                            }
                            lineCount++;

                        }
                    }
                    float[] Tmax24 = new float[intCount],
                        Tmin24 = new float[intCount],
                        Tmax48 = new float[intCount],
                        Tmax72 = new float[intCount],
                        Tmin48 = new float[intCount],
                        Tmax96 = new float[intCount],
                        Tmin96 = new float[intCount],
                        Tmax120 = new float[intCount],
                        Tmin120 = new float[intCount],
                        Tmin72 = new float[intCount];
                    string[] StationID = new string[intCount],
                        Rain24 = new string[intCount],
                        Rain48 = new string[intCount],
                        Rain96 = new string[intCount],
                        Rain120 = new string[intCount],
                        Rain72 = new string[intCount];
                    string[] FX24 = new string[intCount],
                        FS24 = new string[intCount],
                        FX48 = new string[intCount],
                        FS48 = new string[intCount],
                        FX96 = new string[intCount],
                        FS96 = new string[intCount],
                        FX120 = new string[intCount],
                        FS120 = new string[intCount],
                        FX72 = new string[intCount],
                        FS72 = new string[intCount];
                    string WeatherDZ = System.Environment.CurrentDirectory + @"\config\天气对照.txt";
                    float WeatherLS = 0, FXLS = 0, FSLS = 0; //保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                    string FXDZ = System.Environment.CurrentDirectory + @"\config\风向对照.txt";
                    string FSDZ = System.Environment.CurrentDirectory + @"\config\风速对照.txt";
                    using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                    {
                        lineCount = 0;
                        int k = 0;
                        while (((line = sr.ReadLine()) != null) && k < intCount) //k代表乡镇的序号
                        {
                            string[] szLS = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                            if (lineCount == (15 * k + 5))
                            {
                                StationID[k] = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)[0];
                            }
                            else if (lineCount == (15 * k + 6))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if (lineCount == (15 * k + 7))
                            {
                                Tmax24[k] = Convert.ToSingle(szLS[11]);
                                Tmin24[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);

                                if (WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                Rain24[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        Rain24[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FXLS == FXLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                FX24[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(FXLS1.ToString()))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FX24[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FSLS == FSLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                FS24[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FS24[k] = LS1 + "转" + LS2;
                                    }
                                }
                            }

                            else if (lineCount == (15 * k + 8))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if (lineCount == (15 * k + 9))
                            {
                                Tmax48[k] = Convert.ToSingle(szLS[11]);
                                Tmin48[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
                                if (WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                Rain48[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        Rain48[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FXLS == FXLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                FX48[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(FXLS1.ToString()))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FX48[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FSLS == FSLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                FS48[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FS48[k] = LS1 + "转" + LS2;
                                    }
                                }
                            }
                            else if (lineCount == (15 * k + 10))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if (lineCount == (15 * k + 11))
                            {
                                Tmax72[k] = Convert.ToSingle(szLS[11]);
                                Tmin72[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
                                if (WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                Rain72[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        Rain72[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FXLS == FXLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                FX72[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(FXLS1.ToString()))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FX72[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FSLS == FSLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                FS72[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FS72[k] = LS1 + "转" + LS2;
                                    }
                                }
                                
                            }
                            else if (lineCount == (15 * k + 12))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if (lineCount == (15 * k + 13))
                            {
                                Tmax96[k] = Convert.ToSingle(szLS[11]);
                                Tmin96[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
                                if (WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                Rain96[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        Rain96[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FXLS == FXLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                FX96[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(FXLS1.ToString()))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FX96[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FSLS == FSLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                FS96[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FS96[k] = LS1 + "转" + LS2;
                                    }
                                }
                            }
                            else if (lineCount == (15 * k + 14))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if (lineCount == (15 * k + 15))
                            {
                                Tmax120[k] = Convert.ToSingle(szLS[11]);
                                Tmin120[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
                                if (WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                Rain120[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        Rain120[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FXLS == FXLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                FX120[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(FXLS.ToString()))
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (line1.Contains(FXLS1.ToString()))
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FX120[k] = LS1 + "转" + LS2;
                                    }
                                }
                                if (FSLS == FSLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                FS120[k] = line1.Split('=')[0];
                                                break;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        string LS1 = "", LS2 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                            {
                                                LS1 = line1.Split('=')[0];
                                            }
                                            else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                            {
                                                LS2 = line1.Split('=')[0];
                                            }

                                        }
                                        FS120[k] = LS1 + "转" + LS2;
                                    }
                                }
                                k++;
                                JLGS++;
                            }
                            lineCount++;

                        }
                    }

                    string con = "";
                    using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
                    {
                        string line1;

                        // 从文件读取并显示行，直到文件的末尾 
                        while ((line1 = sr.ReadLine()) != null)
                        {
                            if (line1.Contains("sql管理员"))
                            {
                                con = line1.Substring("sql管理员=".Length);
                                break;
                            }
                        }
                    }
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        mycon.Open();
                        string myDate = YBDate.Substring(0, 4) + '-' + YBDate.Substring(4, 2) + '-' +
                                        YBDate.Substring(6, 2);
                        for (int j = 0; j < intCount; j++)
                        {
                            string sql = string.Format(
                                @"insert into SJYB values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}')",
                                StationID[j], myDate, strTime, GW, Tmax24[j], Tmin24[j], Rain24[j], FX24[j], FS24[j],
                                Tmax48[j], Tmin48[j], Rain48[j], FX48[j], FS48[j], Tmax72[j], Tmin72[j], Rain72[j],
                                FX72[j], FS72[j], YBtime, Tmax96[j], Tmin96[j], Rain96[j],
                                FX96[j], FS96[j], Tmax120[j], Tmin120[j], Rain120[j],
                                FX120[j], FS120[j]);
                            try
                            {
                                SqlCommand sqlman = new SqlCommand(sql, mycon);
                                sqlman.ExecuteNonQuery(); //执行数据库语句并返回一个int值（受影响的行数）  
                                SucGS++;
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }

                    string[] cutToPath = new string[fileNameList.Length];
                    for (int j = 0; j < fileNameList.Length; j++)
                    {
                        string[] szLS = fileNameList[j].Split('\\');
                        if (!Directory.Exists(YBpath + @"已入库\"))
                        {
                            Directory.CreateDirectory(YBpath + @"已入库\");
                        }
                        cutToPath[j] = YBpath + @"已入库\" + szLS[szLS.Length - 1];
                        try
                        {
                            File.Move(fileNameList[j], cutToPath[j]);
                        }
                        catch(Exception ee)
                        {
                            
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                ss+=ex.Message+"\r\n";
            }
            ss += DateTime.Now.ToString() + "保存" + YBDate + "日" + strTime + "时" + GW + "预报至数据库" + ',' + string.Format("共计{0}条记录，成功入库{1}条记录。", JLGS, SucGS) + "\r\n";
            return  ss;


        }

        public void BWTB(string YBDate,string SC,string GW) //市台报重命名为对应岗位报文
        {
            string GWBwbs = "";
            string BWBS = "";
            string BSPath = Environment.CurrentDirectory + @"\config\报文标识.txt";
            using (StreamReader sr2 = new StreamReader(BSPath, Encoding.Default))
            {
                string line2 = "";
                while ((line2 = sr2.ReadLine()) != null)
                {
                    if (line2.Split('=')[0] == "呼市气象台")
                    {
                        BWBS = line2.Split('=')[1];

                    }
                    else if (line2.Split('=')[0] == GW)
                    {
                        GWBwbs = line2.Split('=')[1];
                    }
                }
            }
            string pathConfig = Environment.CurrentDirectory + @"\config\pathConfig.txt";
            string BWPath = "";
            using (StreamReader sr2 = new StreamReader(pathConfig, Encoding.Default))
            {
                string line2 = "";
                while ((line2 = sr2.ReadLine()) != null)
                {
                    if (line2.Split('=')[0] == "市台预报市局路径")
                    {
                        BWPath = line2.Split('=')[1];
                    }
                }
            }
            if (SC == "08")
            {
                string strParPath = "*" + BWBS + "*-SPCC-" + YBDate + "00" + "*";
                string[] fileNameList = Directory.GetFiles(BWPath, strParPath);
                if (fileNameList.Length == 0)
                {

                    //MessageBox.Show(YBDate + "08时市台预报不存在，请同步");
                    return;
                }
                else
                {
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int64 maxLS = 0, minLS = 0, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    for (Int16 j = 0; j < fileNameList.Length; j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt64(strLS);
                        if (j == 0)
                        {
                            maxLS = intLS;
                            minLS = intLS;
                        }
                        else
                        {
                            if (intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if (intLS < minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }

                    string gwbwpath = fileNameList[maxXH].Replace(BWBS, GWBwbs);
                    File.Copy(fileNameList[maxXH], gwbwpath, true);

                }
            }
            else if (SC == "20")
            {
                string strParPath = "*" + BWBS + "*-SPCC-" + YBDate + "12" + "*";
                string[] fileNameList = Directory.GetFiles(BWPath, strParPath);
                if (fileNameList.Length == 0)
                {

                   // MessageBox.Show(YBDate + "20时市台预报不存在，请同步");
                    return;
                }
                else
                {
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int64 maxLS = 0, minLS = 0, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    for (Int16 j = 0; j < fileNameList.Length; j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt64(strLS);
                        if (j == 0)
                        {
                            maxLS = intLS;
                            minLS = intLS;
                        }
                        else
                        {
                            if (intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if (intLS < minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }

                    string gwbwpath = fileNameList[maxXH].Replace(BWBS, GWBwbs);
                    File.Copy(fileNameList[maxXH], gwbwpath, true);

                }
            }
        }
        public void SJYBTB(string YBDate,string SC)
        {
            string BWBS = "";
            string pathConfig = Environment.CurrentDirectory + @"\config\pathConfig.txt";
            string BSConfig = Environment.CurrentDirectory + @"\config\报文标识.txt";
            string qjPath = "", sjPath = "";
            using (StreamReader sr = new StreamReader(pathConfig, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市台指导区局路径")
                    {
                        qjPath = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "市台预报市局路径")
                    {
                        sjPath = line.Split('=')[1];
                    }
                }
            }
            using (StreamReader sr = new StreamReader(BSConfig, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "呼市气象台")
                    {
                        BWBS = line.Split('=')[1];
                    }
                    break;
                }
            }
            if (qjPath.Length > 0 && qjPath.Length > 0 && BWBS.Length > 0)
            {
                if (SC == "08")
                {
                    DateTime dt = DateTime.ParseExact(YBDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    qjPath += dt.AddDays(-1).ToString("yyyyMMdd") + "\\";
                    string strParPath = "*" + BWBS + "_" + dt.AddDays(-1).ToString("yyyyMMdd2245") + "*-SPCC-" + dt.ToString("yyyyMMdd") + "00" + "*";
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    for (Int16 j = 0; j < fileNameList.Length; j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt16(strLS.Substring(strLS.Length - 2));
                        if (j == 0)
                        {
                            maxLS = intLS;
                            minLS = intLS;
                        }
                        else
                        {
                            if (intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if (intLS < minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }
                    try
                    {
                        sjPath += System.IO.Path.GetFileName(fileNameList[maxXH]);
                        File.Copy(fileNameList[maxXH], sjPath, true);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                else
                {
                    qjPath += YBDate + "\\";
                    string strParPath = "*" + BWBS + "_" + YBDate + "0830" + "*-SPCC-" + YBDate + "12" + "*";
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    for (Int16 j = 0; j < fileNameList.Length; j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt16(strLS.Substring(strLS.Length - 2));
                        if (j == 0)
                        {
                            maxLS = intLS;
                            minLS = intLS;
                        }
                        else
                        {
                            if (intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if (intLS < minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }
                    try
                    {
                        sjPath += System.IO.Path.GetFileName(fileNameList[maxXH]);
                        File.Copy(fileNameList[maxXH], sjPath, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
        public string CIMISSHQQXSK(string strDate, string strTime, ref int rst1, ref string strError)
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
            string strToday = strDate + strTime + "0000";
            string strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            paramsqx.Add("times", strToday);
            paramsqx.Add("staIds", QXID);//选择区站号
            //此处增加风要素
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C,TEM_Max_24h,TEM_Min_24h,PRE_24h");// 检索要素：站号、站名、过去24h最高、最低气温、24小时降水量
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


        public void CIMISSRain12(string strDate, string strTime, ref string strError, ref string jltext)
        {

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "statSurfEleByStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx1 = new Dictionary<String, String>();//前半天
            Dictionary<String, String> paramsqx2 = new Dictionary<String, String>();//后半天
            // 必选参数
            paramsqx1.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            paramsqx2.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            //检索时间段
            string strToday = strDate + strTime + "0000";
            string strLS;
            strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            string stYesterday = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000");
            string halfDay = Convert.ToDateTime(strLS).AddHours(-12).ToUniversalTime().ToString("yyyyMMddHH0000");
            string timeRange1 = "(" + stYesterday + "," + halfDay + "]";
            string timeRange2 = "(" + halfDay + "," + strToday + "]";
            paramsqx1.Add("timeRange", timeRange1);
            paramsqx2.Add("timeRange", timeRange2);


            paramsqx1.Add("staIds", QXID);//选择区站号，从乡镇名单中获取
            paramsqx2.Add("staIds", QXID);//选择区站号，从乡镇名单中获取
            paramsqx1.Add("elements", "Station_Name,Cnty,Station_Id_C");// 检索要素：站号、旗县、区站号
            paramsqx2.Add("elements", "Station_Name,Cnty,Station_Id_C");// 检索要素：站号、旗县、区站号
            //此处增加风要素
            paramsqx1.Add("statEles", "SUM_PRE_1h");// 统计要素小时降水量
            paramsqx2.Add("statEles", "SUM_PRE_1h");// 统计要素小时降水量
            // 可选参数
            paramsqx1.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            paramsqx2.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "Text";
            StringBuilder retStrXZ1 = new StringBuilder();//返回字符串
            StringBuilder retStrXZ2 = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst1 = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx1, dataFormat, retStrXZ1);
            int rst2 = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx2, dataFormat, retStrXZ2);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData1 = Convert.ToString(retStrXZ1);
            string strData2 = Convert.ToString(retStrXZ2);
            strLS = strData1.Split('"')[1];
            rst1 = Convert.ToInt32(strLS);
            if (rst1 == 0)
            {
                string[] SZlinshi = strData1.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                strData1 = "";
                /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
                for (int i = 0; i < SZlinshi.Length; i++)
                {
                    if (i > 1)
                    {
                        strData1 += SZlinshi[i] + "\n";
                    }
                }
                strData1 = strData1.Substring(0, strData1.Length - 1);
            }
            else
            {
                strError += strDate + "前十二小时降水量CIMISS获取出错：\n" + strData1;
            }

            strLS = strData2.Split('"')[1];
            rst2 = Convert.ToInt32(strLS);
            if (rst2 == 0)
            {
                string[] SZlinshi = strData2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                strData2 = "";
                /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
                for (int i = 0; i < SZlinshi.Length; i++)
                {
                    if (i > 1)
                    {
                        strData2 += SZlinshi[i] + "\n";
                    }
                }
                strData2 = strData2.Substring(0, strData2.Length - 1);
            }
            else
            {
                strError += strDate + "后十二小时降水量CIMISS获取出错：\n" + strData2;
            }
            int XZGS = QXID.Split(',').Length;
            string[,] Rain0012 = new string[XZGS, 2], Rain1224 = new string[XZGS, 2];
            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取旗县台站号*/

            try
            {

                string[] szLS = strData1.Split('\n');
                for (int i = 0; i < XZGS; i++)
                {
                    string[] szLS2 = szLS[i].Split(' ');
                    Rain0012[i, 0] = szLS2[2];

                    Rain0012[i, 1] = szLS2[3];
                }
            }
            catch (Exception ex)
            {
                strError += ex.Message;
            }
            try
            {
                string[] szLS = strData2.Split('\n');
                for (int i = 0; i < XZGS; i++)
                {
                    string[] szLS2 = szLS[i].Split(' ');
                    Rain1224[i, 0] = szLS2[2];

                    Rain1224[i, 1] = szLS2[3];


                }
            }
            catch (Exception ex)
            {
                strError += ex.Message;
            }
            //数据库中的日期保存格式为“yyyy-MM-DD”需加“-”
            string myDate = strDate.Substring(0, 4) + '-' + strDate.Substring(4, 2) + '-' + strDate.Substring(6, 2);
            int ZS = 0, Q12 = 0, H12 = 0;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//Rain1224='{1}'
                for (int i = 0; i < XZGS; i++)
                {
                    ZS++;
                    string sql = string.Format(@"update SK set Rain0012='{0}' where StationID='{1}' and Date='{2}' and SC='{3}'", Rain0012[i, 1], Rain0012[i, 0], myDate, strTime);
                    try
                    {
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        //执行数据库语句并返回一个int值（受影响的行数）  
                        if (sqlman.ExecuteNonQuery() > 0)
                        {
                            Q12++;
                        }
                    }
                    catch (Exception ex)
                    {
                        strError += ex.Message;
                    }
                    sql = string.Format(@"update SK set Rain1224='{0}' where StationID='{1}' and Date='{2}' and SC='{3}'", Rain1224[i, 1], Rain1224[i, 0], myDate, strTime);
                    try
                    {
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        //执行数据库语句并返回一个int值（受影响的行数）  
                        if (sqlman.ExecuteNonQuery() > 0)
                        {
                            H12++;
                        }
                    }
                    catch (Exception ex)
                    {
                        strError += ex.Message;
                    }
                }
            }
            jltext += strDate + " 降水量共需入库" + ZS.ToString() + "条，成功入库" + H12.ToString() + "条。";


        }

        public bool ZYZDRK(DateTime dt,string SC,ref string fhstring)
        {
            
            bool FHBool = false;
            string ss = "";
            //数据库读取旗县列表
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from StationList");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("FromID")) + '\n';
                    }

                }
                catch (Exception ex)
                {
                    FHBool = false;
                    //MessageBox.Show(ex.Message);
                    return FHBool;
                }
            }
            ss = ss.Substring(0, ss.Length - 1);
            string[] szls = ss.Split('\n');
            string[,] qxSZ = new string[szls.Length, 3];//保存旗县名称、ID、所属旗县ID（该项目前没用）
            for (int i = 0; i < szls.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    qxSZ[i, j] = szls[i].Split('=')[j];
                }
            }
            Int16 qxCount = (Int16)szls.Length;
            string bdpath = "";
            try
            {
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\pathConfig.txt", Encoding.Default))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "读取中央指导预报服务器路径")
                        {
                            bdpath = line.Split('=')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
                return FHBool;
            }
            string strError="";
            string BWBS = "BABJ";
            ObservableCollection<people> peopleList = new ObservableCollection<people>();
            BWCX(peopleList,qxCount,bdpath, dt, BWBS, SC, qxSZ, ref strError);
            try
            {
                if (peopleList.Count != 0)
                {
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        //Int16 intBS = 0;//状态标示，初始为0，不更新为1，更新为2
                        mycon.Open();//打开
                        people[] p1ls = peopleList.ToArray();
                        for (int i = 0; i < p1ls.Length; i++)
                        {
                            int jlCount = 0;
                            try
                            {
                                string sql = string.Format(
                                    @"insert into ZYZD values('{0}','{1:yyyy-MM-dd}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                    p1ls[i].QXID, dt, SC, p1ls[i].GW24, p1ls[i].DW24, p1ls[i].QY24,
                                    p1ls[i].GW48, p1ls[i].DW48, p1ls[i].QY48, p1ls[i].GW72, p1ls[i].DW72,
                                    p1ls[i].QY72, p1ls[i].GW96, p1ls[i].DW96, p1ls[i].QY96, p1ls[i].GW120, p1ls[i].DW120, p1ls[i].QY120); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                SqlCommand sqlman = new SqlCommand(sql, mycon);
                                jlCount = sqlman.ExecuteNonQuery();
                                if (jlCount != 0)
                                {
                                    FHBool=true;
                                    fhstring = DateTime.Now.ToString() + "："+dt.ToString("yyyy年MM月dd日")+SC+"时中央指导自动入库成功\r\n";
                                }
                            }
                            catch (Exception ex1)
                            {

                            }
                            
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            return FHBool;
        }

        public void BWCX(ObservableCollection<people> peopleList,Int16 qxCount,string path, DateTime dt, string BWBS, string Time, string[,] qxSZ, ref string strError)
        {

            if (Time == "08")//滤掉10时次报文
            {
                string strParPath = "*" + BWBS + "*-SCMOC-" + dt.ToString("yyyyMMdd") + "00" + "*";
                string[] fileNameList = Directory.GetFiles(path, strParPath);
                if (fileNameList.Length == 0)
                {
                    strError += dt.ToString("yyyy年MM月dd日") + "08时预报文件不存在\n";
                    //MessageBox.Show(dt.ToString("yyyy年MM月dd日") + "08时预报文件不存在");
                    return;
                }
                int intCount = 0;//记录该报文中的站点数
                Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                Int64 maxLS = 0, minLS = 0, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                for (Int16 j = 0; j < fileNameList.Length; j++)
                {
                    string strLS = fileNameList[j].Split('_')[4];
                    intLS = Convert.ToInt64(strLS);
                    if (j == 0)
                    {
                        maxLS = intLS;
                        minLS = intLS;
                    }
                    else
                    {
                        if (intLS > maxLS)
                        {
                            maxLS = intLS;
                            maxXH = j;
                        }
                        if (intLS < minLS)
                        {
                            minLS = intLS;
                            minXH = j;
                        }
                    }
                }
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    string line = "";
                    int lineCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineCount == 4)
                        {
                            intCount = Convert.ToInt32(line);
                            break;
                        }
                        lineCount++;
                    }
                }

                float[] Tmax24 = new float[qxCount], Tmin24 = new float[qxCount], Tmax48 = new float[qxCount], Tmax72 = new float[qxCount], Tmin48 = new float[qxCount], Tmin72 = new float[qxCount], Tmax96 = new float[qxCount], Tmin96 = new float[qxCount], Tmax120 = new float[qxCount], Tmin120 = new float[qxCount];
                string[] StationID = new string[qxCount], Rain24 = new string[qxCount], Rain48 = new string[qxCount], Rain72 = new string[qxCount], Rain96 = new string[qxCount], Rain120 = new string[qxCount];
                string[] FX24 = new string[qxCount], FS24 = new string[qxCount], FX48 = new string[qxCount], FS48 = new string[qxCount], FX72 = new string[qxCount], FS72 = new string[qxCount], FX96 = new string[qxCount], FS96 = new string[qxCount], FX120 = new string[qxCount], FS120 = new string[qxCount];
                string WeatherDZ = System.Environment.CurrentDirectory + @"\config\天气对照.txt";
                float WeatherLS = 0, FXLS = 0, FSLS = 0;//保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                string FXDZ = System.Environment.CurrentDirectory + @"\config\风向对照.txt";
                string FSDZ = System.Environment.CurrentDirectory + @"\config\风速对照.txt";
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    bool bsls1 = false;
                    string line = "";
                    int lineCount = 0;
                    int k = 0;
                    Int16 xh = 0;
                    while (((line = sr.ReadLine()) != null) && k < intCount)//k代表乡镇的序号
                    {
                        string[] szLS = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (lineCount == (29 * k + 5))
                        {
                            k++;
                            string id = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];

                            for (int i = 0; i < qxCount; i++)
                            {
                                if (qxSZ[i, 1] == id)
                                {
                                    bsls1 = true;
                                    StationID[xh] = id;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 9) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 13) && bsls1 == true)
                        {
                            Tmax24[xh] = Convert.ToSingle(szLS[11]);
                            Tmin24[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);

                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS24[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }

                        else if (lineCount == (29 * (k - 1) + 17) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 21) && bsls1 == true)
                        {
                            Tmax48[xh] = Convert.ToSingle(szLS[11]);
                            Tmin48[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS48[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 23) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 25) && bsls1 == true)
                        {


                            Tmax72[xh] = Convert.ToSingle(szLS[11]);
                            Tmin72[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS72[xh] = LS1 + "转" + LS2;
                                }
                            }


                        }
                        else if (lineCount == (29 * (k - 1) + 26) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 27) && bsls1 == true)
                        {
                            Tmax96[xh] = Convert.ToSingle(szLS[11]);
                            Tmin96[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS96[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 28) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 29) && bsls1 == true)
                        {
                            Tmax120[xh] = Convert.ToSingle(szLS[11]);
                            Tmin120[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            xh++;
                            bsls1 = false;
                            if (xh >= qxCount)
                                break;
                        }

                        lineCount++;
                    }
                }
                peopleList.Clear();
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] != "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96 = Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],
                        });
                    }
                }
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] == "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96 = Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],
                        });
                        break;
                    }
                }
            }
            else
            {
                string strParPath = "*" + BWBS + "*-SCMOC-" + dt.ToString("yyyyMMdd") + "12" + "*";
                string[] fileNameList = Directory.GetFiles(path, strParPath);
                if (fileNameList.Length == 0)
                {
                    strError += dt.ToString("yyyy年MM月dd日") + "20时预报文件不存在\n";
                    //MessageBox.Show(dt.ToString("yyyy年MM月dd日") + "20时预报文件不存在");
                    return;
                }
                int intCount = 0;//记录该报文中的站点数
                Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                Int64 maxLS = 0, minLS = 0, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                for (Int16 j = 0; j < fileNameList.Length; j++)
                {
                    string strLS = fileNameList[j].Split('_')[4];
                    intLS = Convert.ToInt64(strLS);
                    if (j == 0)
                    {
                        maxLS = intLS;
                        minLS = intLS;
                    }
                    else
                    {
                        if (intLS > maxLS)
                        {
                            maxLS = intLS;
                            maxXH = j;
                        }
                        if (intLS < minLS)
                        {
                            minLS = intLS;
                            minXH = j;
                        }
                    }
                }
               
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    string line = "";
                    int lineCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineCount == 4)
                        {
                            intCount = Convert.ToInt32(line);
                            break;
                        }
                        lineCount++;
                    }
                }

                float[] Tmax24 = new float[qxCount], Tmin24 = new float[qxCount], Tmax48 = new float[qxCount], Tmax72 = new float[qxCount], Tmin48 = new float[qxCount], Tmin72 = new float[qxCount], Tmax96 = new float[qxCount], Tmin96 = new float[qxCount], Tmax120 = new float[qxCount], Tmin120 = new float[qxCount];
                string[] StationID = new string[qxCount], Rain24 = new string[qxCount], Rain48 = new string[qxCount], Rain72 = new string[qxCount], Rain96 = new string[qxCount], Rain120 = new string[qxCount];
                string[] FX24 = new string[qxCount], FS24 = new string[qxCount], FX48 = new string[qxCount], FS48 = new string[qxCount], FX72 = new string[qxCount], FS72 = new string[qxCount], FX96 = new string[qxCount], FS96 = new string[qxCount], FX120 = new string[qxCount], FS120 = new string[qxCount];
                string WeatherDZ = System.Environment.CurrentDirectory + @"\config\天气对照.txt";
                float WeatherLS = 0, FXLS = 0, FSLS = 0;//保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                string FXDZ = System.Environment.CurrentDirectory + @"\config\风向对照.txt";
                string FSDZ = System.Environment.CurrentDirectory + @"\config\风速对照.txt";
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    bool bsls1 = false;
                    string line = "";
                    int lineCount = 0;
                    int k = 0;
                    Int16 xh = 0;
                    while (((line = sr.ReadLine()) != null) && k < intCount)//k代表乡镇的序号
                    {
                        string[] szLS = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (lineCount == (29 * k + 5))
                        {
                            k++;
                            string id = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];

                            for (int i = 0; i < qxCount; i++)
                            {
                                if (qxSZ[i, 1] == id)
                                {
                                    bsls1 = true;
                                    StationID[xh] = id;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 9) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 13) && bsls1 == true)
                        {
                            Tmax24[xh] = Convert.ToSingle(szLS[11]);
                            Tmin24[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);

                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX24[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS24[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS24[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }

                        else if (lineCount == (29 * (k - 1) + 17) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 21) && bsls1 == true)
                        {
                            Tmax48[xh] = Convert.ToSingle(szLS[11]);
                            Tmin48[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX48[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS48[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS48[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 23) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 25) && bsls1 == true)
                        {


                            Tmax72[xh] = Convert.ToSingle(szLS[11]);
                            Tmin72[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX72[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS72[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS72[xh] = LS1 + "转" + LS2;
                                }
                            }


                        }
                        else if (lineCount == (29 * (k - 1) + 26) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 27) && bsls1 == true)
                        {
                            Tmax96[xh] = Convert.ToSingle(szLS[11]);
                            Tmin96[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX96[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS96[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS96[xh] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (29 * (k - 1) + 28) && bsls1 == true)
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (29 * (k - 1) + 29) && bsls1 == true)
                        {
                            Tmax120[xh] = Convert.ToSingle(szLS[11]);
                            Tmin120[xh] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS120[xh] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS120[xh] = LS1 + "转" + LS2;
                                }
                            }
                            xh++;
                            bsls1 = false;
                            if (xh >= qxCount)
                                break;
                        }
                        lineCount++;
                    }
                }
                peopleList.Clear();
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] != "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96 = Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],
                        });
                    }
                }
                for (int i = 0; i < qxCount; i++)
                {
                    if (StationID[i] == "53463")
                    {
                        peopleList.Add(new people()
                        {
                            QXID = StationID[i],
                            GW24 = Tmax24[i],
                            DW24 = Tmin24[i],
                            QY24 = Rain24[i],
                            GW48 = Tmax48[i],
                            DW48 = Tmin48[i],
                            QY48 = Rain48[i],
                            GW72 = Tmax72[i],
                            DW72 = Tmin72[i],
                            QY72 = Rain72[i],
                            GW96 = Tmax96[i],
                            DW96 = Tmin96[i],
                            QY96 = Rain96[i],
                            GW120 = Tmax120[i],
                            DW120 = Tmin120[i],
                            QY120 = Rain120[i],
                        });
                        break;
                    }
                }
            }
        }

        public class people
        {
            public string QXID { get; set; }
            public float GW24 { get; set; }
            public float DW24 { get; set; }
            public string QY24 { get; set; }
            public float GW48 { get; set; }
            public float DW48 { get; set; }
            public string QY48 { get; set; }
            public float GW72 { get; set; }
            public float DW72 { get; set; }
            public string QY72 { get; set; }
            public float GW96 { get; set; }
            public float DW96 { get; set; }
            public string QY96 { get; set; }
            public float GW120 { get; set; }
            public float DW120 { get; set; }
            public string QY120 { get; set; }

        }

        public string  SKRK(string strDate, string strTime)
        {
            string strfh = "";
            int SKRKGS = 0;
            string strError = "";
            string strSK = "";
            int rst1 = 0;
            string strQXSK = CIMISSHQQXSK(strDate, strTime, ref rst1, ref strError);
            if ((rst1 == 0))
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开


                    for (int i = 0; i < strQXSK.Split('\n').Length; i++)
                    {
                        string[] szLS1 = strQXSK.Split('\n')[i].Split(' ');
                        float myTmax, myTmin, myRain;
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
                        string myDate = strDate.Substring(0, 4) + '-' + strDate.Substring(4, 2) + '-' + strDate.Substring(6, 2);
                        string sql = string.Format(@"insert into SK (Name,StationID,Date,SC,Tmax,Tmin,Rain) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", QXName.Split(',')[i], QXID.Split(',')[i], myDate, strTime, myTmax, myTmin, myRain);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        try
                        {
                            SqlCommand sqlman = new SqlCommand(sql, mycon);
                            SKRKGS += sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数）     



                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show("数据库添加失败\n" + ex.Message);
                        }
                    }
                }
            }
            strfh += DateTime.Now.ToString() + "保存" + strDate + "日" + strTime + "时" + SKRKGS.ToString() + "条实况至数据库\n";
            string error = "";
            string jltext = "";
            CIMISSRain12(strDate, strTime, ref error, ref jltext);
            error += strError;
            strfh += jltext + '\n';
            if(error.Length>0)
                strfh += error + '\n';
            
            if (strError.Length == 0)
            {

            }
            else
            {
                strfh+="CIMISS出错，返回代码为：" + strError+'\n';
            }
            return strfh;


        }
    }
}
