using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace 呼和浩特市精细化天气预报评分系统
{
    class tjClass
    {
        private string con = "";

        public tjClass()
        {
            string DBconPath = Environment.CurrentDirectory + @"\config\DBconfig.txt";

            try
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
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ObservableCollection<ZRPF> zrpftj(string sc,DateTime dateTime,string gw,string sx)
        {
            ObservableCollection<ZRPF> zrpf = new ObservableCollection<ZRPF>();
            
            string QXName = "", QXID = "";
            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
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
                    mycon.Close();
                }
            }
            catch(Exception e)
            {
                
            }
            if (QXName != null)
            {
                string[] qxnameSZ = QXName.Split(','), qxidSZ = QXID.Split(',');
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    try
                    {
                        mycon.Open();
                        string sql = string.Format(@"select * from TJ where GW='{0}' AND Date='{1:yyyy-MM-dd}' AND SC='{2}'", gw, dateTime,sc);
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {
                                    string qxname = "", id = "", gw1 = "", gw2 = "", gw3 = "", dw1 = "", dw2 = "", dw3 = "", sttq = "", stqy = "", zdtq = "", zdqy = "";
                                    float stgw = 999999,
                                        stdw = 999999,
                                        zdgw = 999999,
                                        zddw = 999999,
                                        skgw = 999999,
                                        skdw = 999999,
                                        skjs = 999999;
                                    string qxid= sqlreader.GetString(sqlreader.GetOrdinal("StationID"));
                                    for (int i = 0; i < qxidSZ.Length; i++)
                                    {
                                        if (qxidSZ[i] == qxid)
                                        {
                                            qxname = qxnameSZ[i];
                                            break;
                                        }
                                    }
                                    id = sqlreader.GetString(sqlreader.GetOrdinal("PeopleID"));
                                    float floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax"+sx)));
                                    float floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax" + sx)));
                                    if (floasj < 9999)
                                    {
                                        if (floasj <= 1)
                                        {
                                            gw1 = "√";
                                            gw2 = "×";
                                            gw3 = "×";
                                        }
                                        else if (floasj <= 2 && (floasj - floazy) < 0)
                                        {
                                            gw1 = "×";
                                            gw2 = "√";
                                            gw3 = "×";
                                        }
                                        else if (floasj > 2 && (floasj - floazy) > 0)
                                        {
                                            gw1 = "×";
                                            gw2 = "×";
                                            gw3 = "√";
                                        }
                                        else
                                        {
                                            gw1 = "×";
                                            gw2 = "×";
                                            gw3 = "×";
                                        }
                                    }
                                    else
                                    {
                                        if (sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax" + sx)) > 999998)
                                        {
                                            gw1 = "缺测";
                                            gw2 = "缺测";
                                            gw3 = "缺测";
                                        }
                                        else if (sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax" + sx)) < -999998)
                                        {
                                            gw1 = "缺报";
                                            gw2 = "缺报";
                                            gw3 = "缺报";
                                        }
                                        else
                                        {
                                            gw1 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax" + sx)).ToString();
                                            gw2 = gw1;
                                            gw3 = gw1;
                                        }
                                    }
                                    floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin" + sx)));
                                    floazy = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin" + sx)));
                                    if (floasj < 9999)
                                    {
                                        if (floasj <= 1)
                                        {
                                            dw1 = "√";
                                            dw2 = "×";
                                            dw3 = "×";
                                        }
                                        else if (floasj <= 2 && (floasj - floazy) < 0)
                                        {
                                            dw1 = "×";
                                            dw2 = "√";
                                            dw3 = "×";
                                        }
                                        else if (floasj > 2 && (floasj - floazy) > 0)
                                        {
                                            dw1 = "×";
                                            dw2 = "×";
                                            dw3 = "√";
                                        }
                                        else
                                        {
                                            dw1 = "×";
                                            dw2 = "×";
                                            dw3 = "×";
                                        }
                                    }
                                    else
                                    {
                                        if (sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax" + sx)) > 999998)
                                        {
                                            dw1 = "缺测";
                                            dw2 = "缺测";
                                            dw3 = "缺测";
                                        }
                                        else if (sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax" + sx)) < -999998)
                                        {
                                            dw1 = "缺报";
                                            dw2 = "缺报";
                                            dw3 = "缺报";
                                        }
                                        else
                                        {
                                            dw1 = (sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax" + sx))).ToString();
                                            dw2 = dw1;
                                            dw3 = dw1;
                                        }
                                    }
                                    floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain" + sx)));
                                    if ((Int32)floasj == 0)
                                        stqy = "×";
                                    else if((Int32)floasj == 1)
                                        stqy = "√";
                                    else if ((Int32)floasj == 999999)
                                        stqy = "缺测";
                                    else if ((Int32)floasj == -999999)
                                        stqy = "缺报";
                                    else
                                    {
                                        stqy = floasj.ToString();
                                    }

                                    floasj = Math.Abs(sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain" + sx)));
                                    if ((Int32)floasj == 0)
                                        zdqy = "×";
                                    else if ((Int32)floasj == 1)
                                        zdqy = "√";
                                    else if ((Int32)floasj == 999999)
                                        zdqy = "缺测";
                                    else if ((Int32)floasj == -999999)
                                        zdqy = "缺报";
                                    else
                                    {
                                        zdqy = floasj.ToString();
                                    }
                                    using (SqlConnection mycon1 = new SqlConnection(con))
                                    {
                                        mycon1.Open();//打开
                                        int intDays = Convert.ToInt16(sx) / 24;
                                        DateTime dtLS = dateTime.AddDays(intDays);
                                        string SKDate = dtLS.ToString("yyyy-MM-dd");//查询日期
                                        string sql1 = string.Format(@"select * from SK where StationID='{0}' AND Date='{1}' AND SC='{2}'", qxid, SKDate,sc);
                                        using (SqlCommand sqlman1 = new SqlCommand(sql1, mycon1))
                                        {
                                            using (SqlDataReader sqlreader1 = sqlman1.ExecuteReader())
                                            {
                                                while (sqlreader1.Read())
                                                {
                                                    skgw = sqlreader1.GetFloat(sqlreader1.GetOrdinal("Tmax"));
                                                    skdw = sqlreader1.GetFloat(sqlreader1.GetOrdinal("Tmin"));
                                                    skjs= sqlreader1.GetFloat(sqlreader1.GetOrdinal("Rain"));
                                                    if ((Int32) skjs == 999990)
                                                        skjs = 0;
                                                }
                                            }
                                        }
                                        mycon1.Close();
                                    }
                                    using (SqlConnection mycon1 = new SqlConnection(con))
                                    {
                                        mycon1.Open();//打开
                                        string sql1 = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1:yyyy-MM-dd}' AND SC='{2}' AND GW='{3}'", qxid, dateTime, sc, gw);
                                        using (SqlCommand sqlman1 = new SqlCommand(sql1, mycon1))
                                        {
                                            using (SqlDataReader sqlreader1 = sqlman1.ExecuteReader())
                                            {
                                                while (sqlreader1.Read())
                                                {
                                                    stgw = sqlreader1.GetFloat(sqlreader1.GetOrdinal("Tmax"+sx));
                                                    stdw = sqlreader1.GetFloat(sqlreader1.GetOrdinal("Tmin" + sx));
                                                    sttq = sqlreader1.GetString(sqlreader1.GetOrdinal("Rain" + sx));
                                                }
                                            }
                                        }
                                        mycon1.Close();
                                    }

                                    using (SqlConnection mycon1 = new SqlConnection(con))
                                    {
                                        mycon1.Open();//打开
                                        string sql1 = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1:yyyy-MM-dd}' AND SC='{2}'", qxid,  dateTime,sc);
                                        using (SqlCommand sqlman1 = new SqlCommand(sql1, mycon1))
                                        {
                                            using (SqlDataReader sqlreader1 = sqlman1.ExecuteReader())
                                            {
                                                while (sqlreader1.Read())
                                                {
                                                    zdgw = sqlreader1.GetFloat(sqlreader1.GetOrdinal("Tmax" + sx));
                                                    zddw = sqlreader1.GetFloat(sqlreader1.GetOrdinal("Tmin" + sx));
                                                    zdtq = sqlreader1.GetString(sqlreader1.GetOrdinal("Rain" + sx));
                                                }
                                            }
                                        }
                                        mycon1.Close();
                                    }

                                    zrpf.Add(new ZRPF()
                                    {
                                        QXName = qxname,
                                        ID = id,
                                        STGW = stgw,
                                        STDW = stdw,
                                        ZDDW = zddw,
                                        ZDGW = zdgw,
                                        STQY = stqy,
                                        STTQ = sttq,
                                        ZDQY = zdqy,
                                        ZDTQ = zdtq,
                                        GW1 = gw1,
                                        GW2 = gw2,
                                        GW3 = gw3,
                                        DW1 = dw1,
                                        DW2 = dw2,
                                        DW3 = dw3,
                                        SKDW = skdw,
                                        SKGW = skgw,
                                        SKJS = skjs,
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
            return zrpf;
        }

        public class ZRPF //统计信息列表
        {

            public string QXName { get; set; }
            public string ID { get; set; }
            public float STGW { get; set; }
            public float SKGW { get; set; }
            public float ZDGW { get; set; }
            public string GW1 { get; set; }
            public string GW2 { get; set; }
            public string GW3 { get; set; }
            public float STDW { get; set; }
            public float SKDW { get; set; }
            public float ZDDW { get; set; }
            public string DW1 { get; set; }
            public string DW2 { get; set; }
            public string DW3 { get; set; }
            public string STTQ { get; set; }
            public string STQY { get; set; }
            public float SKJS { get; set; }
            public string ZDTQ { get; set; }
            public string ZDQY { get; set; }
        }

        public string tjPeople(DateTime sdt, DateTime edt)//"ID,姓名\n下一行"
        {
            string fhStr = "";
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open(); //打开
                    string sql =
                        string.Format(
                            @"select DISTINCT userID,name from USERJL where date>='{0:yyyy-MM-dd}' and date<='{1:yyyy-MM-dd}' ",
                            sdt, edt);
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {
                            fhStr +=  sqlreader.GetString(sqlreader.GetOrdinal("userID"))+ ',';
                            fhStr += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + '\n';
                        }
                        fhStr= fhStr.Substring(0, fhStr.Length - 1);
                    }
                    mycon.Close();
                }
                catch (Exception e)
                {
                    
                }
            }

            return fhStr;
        }
        public class PJWCList//旗县平均绝对误差列表
        {
            public float QX24TmaxJDWC { get; set; }
            public float QX24TminJDWC { get; set; }
            public float QX48TmaxJDWC { get; set; }
            public float QX48TminJDWC { get; set; }
            public float QX72TmaxJDWC { get; set; }
            public float QX72TminJDWC { get; set; }
        }
        public float[] GRZQL(DateTime sdt, DateTime edt, String userID) //返回指定指定时间段个人三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<PFZR> pfzrtj1 = new ObservableCollection<PFZR>();
            float[] tjsz = new float[10];
            try
            {
                using (SqlConnection mycon1 = new SqlConnection(con)) //创建SQL连接对象)
                {
                    mycon1.Open(); //打开
                    string sql =
                        string.Format(
                            @"select * from TJ where PeopleID='{0}' AND Date>='{1:yyyy-MM-dd}' AND Date<='{2:yyyy-MM-dd}'",
                            userID, sdt, edt);
                    SqlCommand sqlman = new SqlCommand(sql, mycon1);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {
                            try
                            {
                                float tmax24 = 999999,
                                    tmin24 = 999999,
                                    qy24 = 999999,
                                    tmax48 = 999999,
                                    tmin48 = 999999,
                                    qy48= 999999,
                                    tmax72 = 999999,
                                    tmin72 = 999999,
                                    qy72 = 999999;
                                try
                                {
                                    tmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24"));
                                    tmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24"));
                                    qy24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain24"));
                                    tmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48"));
                                    tmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48"));
                                    qy48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain48"));
                                    tmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72"));
                                    tmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72"));
                                    qy72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain72"));
                                }
                                catch (Exception e)
                                {
                                    
                                }
                                pfzrtj1.Add(new PFZR()
                                {
                                    QX24TmaxZQL = tmax24,
                                    QX24TminZQL = tmin24,
                                    QX24QYZQL = qy24,
                                    QX48TmaxZQL = tmax48,
                                    QX48TminZQL = tmin48,
                                    QX48QYZQL = qy48,
                                    QX72TmaxZQL = tmax72,
                                    QX72TminZQL = tmin72,
                                    QX72QYZQL = qy72,
                                });
                            }
                            catch(Exception e)
                            { }
                        }
                    }
                    mycon1.Close();
                }
                int ss = pfzrtj1.Count;
                PFZR[] s1 = pfzrtj1.ToArray();
                int[] zs = new int[9]; //保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
                for (int i = 0; i < s1.Length; i++)
                {
                    if (s1[i].QX24TmaxZQL < 999998 && s1[i].QX24TmaxZQL > -999999)
                    {
                        zs[0]++;
                        if (Math.Abs(s1[i].QX24TmaxZQL) <= 2)
                        {
                            tjsz[0]++;
                        }
                    }
                    else if (s1[i].QX24TmaxZQL == -999999)
                    {
                        tjsz[9]++;
                    }
                    if (s1[i].QX24TminZQL < 999998 && s1[i].QX24TminZQL > -999999)
                    {
                        zs[1]++;
                        if (Math.Abs(s1[i].QX24TminZQL) <= 2)
                        {
                            tjsz[1]++;
                        }
                    }
                    if (s1[i].QX24QYZQL < 999998 && s1[i].QX24QYZQL > -999999)
                    {
                        zs[2]++;
                        tjsz[2] += s1[i].QX24QYZQL;
                    }

                    if (s1[i].QX48TmaxZQL < 999998 && s1[i].QX48TmaxZQL > -999999)
                    {
                        zs[3]++;
                        if (Math.Abs(s1[i].QX48TmaxZQL) <= 2)
                        {
                            tjsz[3]++;
                        }
                    }
                    if (s1[i].QX48TminZQL < 999998 && s1[i].QX48TminZQL > -999999)
                    {
                        zs[4]++;
                        if (Math.Abs(s1[i].QX48TminZQL) <= 2)
                        {
                            tjsz[4]++;
                        }
                    }
                    if (s1[i].QX48QYZQL < 999998 && s1[i].QX48QYZQL > -999999)
                    {
                        zs[5]++;
                        tjsz[5] += s1[i].QX48QYZQL;
                    }
                    if (s1[i].QX72TmaxZQL < 999998 && s1[i].QX72TmaxZQL > -999999)
                    {
                        zs[6]++;
                        if (Math.Abs(s1[i].QX72TmaxZQL) <= 2)
                        {
                            tjsz[6]++;
                        }
                    }
                    if (s1[i].QX72TminZQL < 999998 && s1[i].QX72TminZQL > -999999)
                    {
                        zs[7]++;
                        if (Math.Abs(s1[i].QX72TminZQL) <= 2)
                        {
                            tjsz[7]++;
                        }
                    }
                    if (s1[i].QX72QYZQL < 999998 && s1[i].QX72QYZQL > -999999)
                    {
                        zs[8]++;
                        tjsz[8] += s1[i].QX72QYZQL;
                    }

                }
                for (int i = 0; i < tjsz.Length; i++)
                {
                    if (i < tjsz.Length - 1)
                    {
                        tjsz[i] = tjsz[i] / zs[i];
                        tjsz[i] = (float) Math.Round(tjsz[i] * 100, 2);
                    }
                    else
                    {
                        tjsz[i] = tjsz[i] / s1.Length;
                        tjsz[i] = (float) Math.Round(tjsz[i] * 100, 2);
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            return tjsz;
        }
        public float[] GRJDWC(DateTime sdt, DateTime edt, String userID)//返回指定区站号、指定时间段个人三天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[6];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1:yyyy-MM-dd}' AND Date<='{2:yyyy-MM-dd}'", userID, sdt, edt);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {
                                    if (sqlreader.HasRows)
                                    {
                                        pfzrtj1.Add(new PJWCList()
                                        {
                                            QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                            QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                            QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                            QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                            QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                            QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                        });
                                    } 
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[6];//保存计算平均绝对误差时候每个要素的个数总数
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].QX24TmaxJDWC < 999998 && s1[i].QX24TmaxJDWC > -999999)
                {
                    zs[0]++;
                    tjsz[0] += Math.Abs(s1[i].QX24TmaxJDWC);

                }
                if (s1[i].QX24TminJDWC < 999998 && s1[i].QX24TminJDWC > -999999)
                {
                    zs[1]++;
                    tjsz[1] += Math.Abs(s1[i].QX24TminJDWC);
                }

                if (s1[i].QX48TmaxJDWC < 999998 && s1[i].QX48TmaxJDWC > -999999)
                {
                    zs[2]++;
                    tjsz[2] += Math.Abs(s1[i].QX48TmaxJDWC);
                }
                if (s1[i].QX48TminJDWC < 999998 && s1[i].QX48TminJDWC > -999999)
                {
                    zs[3]++;
                    tjsz[3] += Math.Abs(s1[i].QX48TminJDWC);
                }
                if (s1[i].QX72TmaxJDWC < 999998 && s1[i].QX72TmaxJDWC > -999999)
                {
                    zs[4]++;
                    tjsz[4] += Math.Abs(s1[i].QX72TmaxJDWC);
                }
                if (s1[i].QX72TminJDWC < 999998 && s1[i].QX72TminJDWC > -999999)
                {
                    zs[5]++;
                    tjsz[5] += Math.Abs(s1[i].QX72TminJDWC);
                }

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }

        public float[] GRZDJDWC(DateTime sdt, DateTime edt, String userID)//返回指定人员、指定时间段中央指导三天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[6];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1:yyyy-MM-dd}' AND Date<='{2:yyyy-MM-dd}'", userID, sdt, edt);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    pfzrtj1.Add(new PJWCList()
                                    {
                                        QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax24")),
                                        QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin24")),
                                        QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax48")),
                                        QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin48")),
                                        QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax72")),
                                        QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin72")),
                                    });
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[6];//保存计算平均绝对误差时候每个要素的个数总数
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].QX24TmaxJDWC < 999998 && s1[i].QX24TmaxJDWC > -999999)
                {
                    zs[0]++;
                    tjsz[0] += Math.Abs(s1[i].QX24TmaxJDWC);

                }
                if (s1[i].QX24TminJDWC < 999998 && s1[i].QX24TminJDWC > -999999)
                {
                    zs[1]++;
                    tjsz[1] += Math.Abs(s1[i].QX24TminJDWC);
                }

                if (s1[i].QX48TmaxJDWC < 999998 && s1[i].QX48TmaxJDWC > -999999)
                {
                    zs[2]++;
                    tjsz[2] += Math.Abs(s1[i].QX48TmaxJDWC);
                }
                if (s1[i].QX48TminJDWC < 999998 && s1[i].QX48TminJDWC > -999999)
                {
                    zs[3]++;
                    tjsz[3] += Math.Abs(s1[i].QX48TminJDWC);
                }
                if (s1[i].QX72TmaxJDWC < 999998 && s1[i].QX72TmaxJDWC > -999999)
                {
                    zs[4]++;
                    tjsz[4] += Math.Abs(s1[i].QX72TmaxJDWC);
                }
                if (s1[i].QX72TminJDWC < 999998 && s1[i].QX72TminJDWC > -999999)
                {
                    zs[5]++;
                    tjsz[5] += Math.Abs(s1[i].QX72TminJDWC);
                }

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }
        public float[] GRZYZQL(DateTime sdt, DateTime edt, String userID)//返回指定区站号、指定时间段中央指导三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[10];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1:yyyy-MM-dd}' AND Date<='{2:yyyy-MM-dd}'", userID, sdt, edt);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {

                                    float tmax24 = 999999,
                                        tmin24 = 999999,
                                        qy24 = 999999,
                                        tmax48 = 999999,
                                        tmin48 = 999999,
                                        qy48 = 999999,
                                        tmax72 = 999999,
                                        tmin72 = 999999,
                                        qy72 = 999999;
                                    try
                                    {
                                        tmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax24"));
                                        tmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin24"));
                                        qy24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain24"));
                                        tmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax48"));
                                        tmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin48"));
                                        qy48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain48"));
                                        tmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax72"));
                                        tmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin72"));
                                        qy72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain72"));
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                    sjzqlTJ1.Add(new ZQLTJ1()
                                    {
                                        SJ24TmaxZQL = tmax24,
                                        SJ24TminZQL = tmin24,
                                        SJ24QYZQL = qy24,
                                        SJ48TmaxZQL = tmax48,
                                        SJ48TminZQL = tmin48,
                                        SJ48QYZQL = qy48,
                                        SJ72TmaxZQL = tmax72,
                                        SJ72TminZQL = tmin72,
                                        SJ72QYZQL = qy72,
                                    });
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = sjzqlTJ1.Count;
            ZQLTJ1[] s1 = sjzqlTJ1.ToArray();
            int[] zs = new int[9];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].SJ24TmaxZQL < 999998 && s1[i].SJ24TmaxZQL > -999999)
                {
                    zs[0]++;
                    if (Math.Abs(s1[i].SJ24TmaxZQL) <= 2)
                    {
                        tjsz[0]++;
                    }
                }
                else if (s1[i].SJ24TmaxZQL == -999999)
                {
                    tjsz[9]++;
                }
                if (s1[i].SJ24TminZQL < 999998 && s1[i].SJ24TminZQL > -999999)
                {
                    zs[1]++;
                    if (Math.Abs(s1[i].SJ24TminZQL) <= 2)
                    {
                        tjsz[1]++;
                    }
                }
                if (s1[i].SJ24QYZQL < 999998 && s1[i].SJ24QYZQL > -999999)
                {
                    zs[2]++;
                    tjsz[2] += s1[i].SJ24QYZQL;
                }

                if (s1[i].SJ48TmaxZQL < 999998 && s1[i].SJ48TmaxZQL > -999999)
                {
                    zs[3]++;
                    if (Math.Abs(s1[i].SJ48TmaxZQL) <= 2)
                    {
                        tjsz[3]++;
                    }
                }
                if (s1[i].SJ48TminZQL < 999998 && s1[i].SJ48TminZQL > -999999)
                {
                    zs[4]++;
                    if (Math.Abs(s1[i].SJ48TminZQL) <= 2)
                    {
                        tjsz[4]++;
                    }
                }
                if (s1[i].SJ48QYZQL < 999998 && s1[i].SJ48QYZQL > -999999)
                {
                    zs[5]++;
                    tjsz[5] += s1[i].SJ48QYZQL;
                }
                if (s1[i].SJ72TmaxZQL < 999998 && s1[i].SJ72TmaxZQL > -999999)
                {
                    zs[6]++;
                    if (Math.Abs(s1[i].SJ72TmaxZQL) <= 2)
                    {
                        tjsz[6]++;
                    }
                }
                if (s1[i].SJ72TminZQL < 999998 && s1[i].SJ72TminZQL > -999999)
                {
                    zs[7]++;
                    if (Math.Abs(s1[i].SJ72TminZQL) <= 2)
                    {
                        tjsz[7]++;
                    }
                }
                if (s1[i].SJ72QYZQL < 999998 && s1[i].SJ72QYZQL > -999999)
                {
                    zs[8]++;
                    tjsz[8] += s1[i].SJ72QYZQL;
                }

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                if (i < tjsz.Length - 1)
                {
                    tjsz[i] = tjsz[i] / zs[i];
                    tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                }
                else
                {
                    tjsz[i] = tjsz[i] / s1.Length;
                    tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                }
            }
            return tjsz;
        }
        public class ZBXX//个人值班信息列表
        {
            public string userID { get; set; }
            public string userName { get; set; }
        }
        public string[,] ZBXXTJ(DateTime sdt, DateTime edt, ref Int16 ZBJS)//返回指定旗县、指定时间段的人员名单及值班次数，引用参数ZBJS为该旗县值班基数
        {
            string zbxxStr = "";
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from USERJL where Date>='{0:yyyy-MM-dd}' AND Date<='{1:yyyy-MM-dd}'", sdt, edt);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    zbxxStr += sqlreader.GetString(sqlreader.GetOrdinal("userID")) + ',';

                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }
                if (zbxxStr.Length > 0)
                {
                    zbxxStr = zbxxStr.Substring(0, zbxxStr.Length - 1);
                    string[] IDJLSz = zbxxStr.Split(',');//保存指定时间段所有值班人员ID

                    string IDList = "";//保存指定时间段值班人员名单
                    for (int i = 0; i < IDJLSz.Length; i++)
                    {
                        if (!IDList.Contains(IDJLSz[i]))
                        {
                            IDList += IDJLSz[i] + ',';
                        }
                    }
                    IDList = IDList.Substring(0, IDList.Length - 1);
                    string[] SZLS1 = IDList.Split(',');//保存值班人员数组，人员ID不重复
                    double douLS1;
                    if (SZLS1.Length == 0)
                    {
                        douLS1 = 0;//防止没有记录时候报错
                    }
                    else
                        douLS1 = Convert.ToDouble((IDJLSz.Length * 2)) / (SZLS1.Length * 3);
                    ZBJS = Convert.ToInt16(Math.Round(douLS1, 0));//计算值班基数，2/3*值班总次数/（起止时间天数差+1）
                    int[] CountSZ = new int[SZLS1.Length];//保存每个人值班次数
                    string[,] TJSZ = new string[SZLS1.Length, 2];//保存返回统计信息，第一列为ID，第二列为值班次数
                    for (int i = 0; i < SZLS1.Length; i++)
                    {
                        for (int j = 0; j < IDJLSz.Length; j++)
                        {
                            if (SZLS1[i] == IDJLSz[j])
                            {
                                CountSZ[i]++;
                            }
                        }
                    }
                    for (int i = 0; i < SZLS1.Length; i++)
                    {
                        TJSZ[i, 0] = SZLS1[i];
                        TJSZ[i, 1] = CountSZ[i].ToString();
                    }
                    return TJSZ;
                }


            }
            catch (Exception ex)
            {

            }
            string[,] szls = new string[1, 1];
            szls[0, 0] = "没有数据";
            return szls;
        }
        public class ZQLTJ1//统计信息列表
        {
            public string Name { get; set; }
            public float SJ24TmaxZQL { get; set; }
            public float SJ24TminZQL { get; set; }
            public float SJ24QYZQL { get; set; }
            public float SJ48TmaxZQL { get; set; }
            public float SJ48TminZQL { get; set; }
            public float SJ48QYZQL { get; set; }
            public float SJ72TmaxZQL { get; set; }
            public float SJ72TminZQL { get; set; }
            public float SJ72QYZQL { get; set; }
        }

        public class PFZR
        {
            public string Name { get; set; }
            public float QX24TmaxZQL { get; set; }
            public float QX24TminZQL { get; set; }
            public float QX24QYZQL { get; set; }
            public float QX48TmaxZQL { get; set; }
            public float QX48TminZQL { get; set; }
            public float QX48QYZQL { get; set; }
            public float QX72TmaxZQL { get; set; }
            public float QX72TminZQL { get; set; }
            public float QX72QYZQL { get; set; }
        }
    }
}
