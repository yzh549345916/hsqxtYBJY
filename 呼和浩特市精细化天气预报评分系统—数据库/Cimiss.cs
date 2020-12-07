using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    class Cimiss
    {
        /*   2.1 用户名&密码 */
        String userId = "BEHT_BFHT_2131";// 
        String pwd = "YZHHGDJM";// 

        //根据起报时间从CIMISS获取中央指导预报
        public List<CIMISS文件信息> 获取中央指导预报(DateTime qbDatetime)
        {
            List<CIMISS文件信息> fileLists = new List<CIMISS文件信息>();
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */

            /*   2.2 接口ID */
            String interfaceId1 = "getSevpFileByTime";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SEVP_WEFC"); // 资料代码
            paramsqx.Add("times", qbDatetime.ToUniversalTime().ToString("yyyyMMddHHmmss"));
            paramsqx.Add("elements", "File_URL,Prod_Code,FILE_NAME_ORIG");

            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "text";
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
                strData = Regex.Replace(strData, "\r\n", "\n");
                string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                if (rst == 0)
                {
                    string[] szls = strData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 2; i < szls.Length; i++)
                    {
                        string[] datals = szls[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        fileLists.Add(new CIMISS文件信息() { fileName = datals[6], fileUrl = datals[3] });
                    }
                    return fileLists;
                }
            }
            catch
            {

            }
            return fileLists;
        }

    }

    public class CIMISS文件信息
    {
        public string fileUrl { get; set; }
        public string fileName { get; set; }
    }
}
