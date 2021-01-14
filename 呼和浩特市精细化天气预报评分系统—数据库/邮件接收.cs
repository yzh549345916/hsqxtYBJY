using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Aspose.Email.Clients.Pop3;
using Aspose.Email.Tools.Search;

namespace 呼和浩特市精细化天气预报评分系统_数据库
{
    internal class 邮件接收
    {
        public static void 查看气象台163pop3()
        {
            var util = new XmlConfig(Environment.CurrentDirectory + @"\config\智能网格设置.xml");
            // Connect and log in to POP3
            const string host = "pop.163.com";
            const int port = 995;
            var username = util.Read("emailConfig", "emailFrom", "address");
            var password = util.Read("emailConfig", "emailFrom", "password");
            var yjjl = util.Read("emailConfig", "空气邮件查收记录");
            var client = new Pop3Client(host, port, username, password);
            try
            {
                var jlLists = new List<string>();
                var dateLists = new List<DateTime>();
                var szls = yjjl.Split(',');
                if (szls.Length > 0)
                    foreach (var item in szls)
                        if (item.Trim() != "")
                            try
                            {
                                var strDate = item.Trim();
                                jlLists.Add(strDate);
                                dateLists.Add(Convert.ToDateTime(strDate.Substring(0, 4) + "-" +
                                                                 strDate.Substring(4, 2) + "-" +
                                                                 strDate.Substring(6, 2)));
                            }
                            catch
                            {
                            }

                var builder1 = new MailQueryBuilder();
                builder1.From.Contains("yewu_zzy", true);
                builder1.InternalDate.Before(DateTime.Now.AddDays(1));
                builder1.InternalDate.Since(DateTime.Now.AddDays(-7));
                var query1 = builder1.GetQuery();
                var messageInfoCol1 = client.ListMessages(query1);
                var changeBS = false;
                for (var i = 0; i < messageInfoCol1.Count; i++)
                    if (!jlLists.Exists(y => messageInfoCol1[i].Subject.Contains(y)))
                    {
                        changeBS = true;
                        var msg = client.FetchMessage(messageInfoCol1[i].UniqueId);
                        try
                        {
                            var regex = new Regex("\\d{10}");
                            var m = regex.Match(messageInfoCol1[i].Subject);
                            if (m.Success)
                            {
                                var strDate = m.Groups[0].Value;
                                var dateTimels = Convert.ToDateTime(strDate.Substring(0, 4) + "-" +
                                                                    strDate.Substring(4, 2) + "-" +
                                                                    strDate.Substring(6, 2));
                                var dataDir = util.Read("路径", "Path空气质量中长期预报") +
                                              $@"{dateTimels:yyyy}\{dateTimels:MM}\{dateTimels:dd}\\";
                                if (!File.Exists(dataDir)) Directory.CreateDirectory(dataDir);

                                foreach (var item in msg.Attachments) item.Save(dataDir + item.Name);
                                dateLists.Add(dateTimels);
                            }
                        }
                        catch
                        {
                        }
                    }

                try
                {
                    if (changeBS)
                        if (dateLists.Count > 0)
                        {
                            var saveDate = new List<DateTime>();
                            foreach (var item in dateLists)
                                if (item.CompareTo(DateTime.Now.Date.AddDays(-8)) >= 0)
                                    saveDate.Add(item);

                            var saveStr = "";
                            foreach (var item in saveDate) saveStr += $"{item:yyyyMMdd},";
                            if (saveStr.Length > 0) saveStr = saveStr.Substring(0, saveStr.Length - 1);
                            util.Write(saveStr, "emailConfig", "空气邮件查收记录");
                        }
                }
                catch
                {
                }

                client.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
    }
}