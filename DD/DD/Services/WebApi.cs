using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using DD.Models;
using System.IO;
using System.Globalization;

namespace DD.Services
{
    public static class WebApi
    {
        public static string API_URL = "http://79.174.13.233/api.php";
        public static ApiServer API_SERVER = null;

        public static JObject ExecuteGetApi(Dictionary<string, string> getParams) {
            JObject answer;

            try
            {
                string answerString = Utils.GetRequest(API_URL, getParams);
                answer = JsonConvert.DeserializeObject<JObject>(answerString);
            }
            catch (Exception ex) 
            {
                Dictionary<string, string> errorParams = new Dictionary<string, string>() {
                    ["code"] = "0",
                    ["message"] = ex.Message,
                };

                string stringAnswer = JsonConvert.SerializeObject(errorParams);
                answer = JObject.Parse(stringAnswer);
            }

            return answer;
        }

        public static JObject ExecuteServerApi(Dictionary<string, string> getParams)
        {
            JObject answer;

            try
            {
                string answerString = Utils.GetRequest(API_SERVER.Address, getParams);
                answer = JsonConvert.DeserializeObject<JObject>(answerString);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> errorParams = new Dictionary<string, string>()
                {
                    ["code"] = "0",
                    ["message"] = ex.Message,
                    ["type"] = "local",
                };

                string stringAnswer = JsonConvert.SerializeObject(errorParams);
                answer = JObject.Parse(stringAnswer);
            }

            return answer;
        }

        public static JObject ExecuteServerApiPost(Dictionary<string, object> postParams)
        {
            JObject answer;

            try
            {
                string answerString = Utils.PostRequest(API_SERVER.Address, postParams);
                answer = JsonConvert.DeserializeObject<JObject>(answerString);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> errorParams = new Dictionary<string, string>()
                {
                    ["code"] = "0",
                    ["message"] = ex.Message,
                };

                string stringAnswer = JsonConvert.SerializeObject(errorParams);
                answer = JObject.Parse(stringAnswer);
            }

            return answer;
        }

        public static ApiServer[] GetApiServers() {
            List<ApiServer> servers = new List<ApiServer>();
            JObject answer = ExecuteGetApi(new Dictionary<string, string>() {
                ["method"] = "servers"
            });

            if (answer.Value<string>("code") == "1") {
                JArray serversArray = answer.Value<JArray>("servers");
                foreach (JObject serverObj in serversArray) {
                    ApiServer server = new ApiServer(serverObj.Value<string>("address"), serverObj.Value<string>("name"));
                    servers.Add(server);
                }
            }

            return servers.ToArray();
        }

        public static JObject GetCategories() {
            JObject answer = ExecuteServerApi(new Dictionary<string, string>()
            {
                ["method"] = "categories"
            });

            return answer;
        }

        public static JObject SendReport(string hash, string content, double longitude, double latitude, ReportAttachment[] attachments)
        {
            List<string> attList = new List<string>();
            foreach (var att in attachments) if (!att.IsLocal) attList.Add(att.Id.ToString());

            JObject answer = ExecuteServerApiPost(new Dictionary<string, object>()
            {
                ["method"] = "send_data",
                ["hash"] = hash,
                ["longitude"] = longitude.ToString("#.000000", CultureInfo.InvariantCulture),
                ["latitude"] = latitude.ToString("#.000000", CultureInfo.InvariantCulture),
                ["attachments"] = String.Join(",", attList),
                ["content"] = content,
            });

            return answer;
        }

        public static string LoginUser(string login, string password)
        {
            JObject answer = ExecuteServerApi(new Dictionary<string, string>()
            {
                ["method"] = "login",
                ["login"] = login.Trim(),
                ["password"] = password.Trim(),
            });

            if (answer.Value<string>("code") == "1") {
                string hash = answer.Value<string>("hash");
                return hash;
            }

            throw new ApiException(answer.Value<string>("message"));
        }

        public static string RegisterUser(string login, string password)
        {
            JObject answer = ExecuteServerApi(new Dictionary<string, string>()
            {
                ["method"] = "register",
                ["login"] = login.Trim(),
                ["password"] = password.Trim(),
            });

            if (answer.Value<string>("code") == "1")
            {
                string hash = answer.Value<string>("hash");
                return hash;
            }

            throw new ApiException(answer.Value<string>("message"));
        }

        public static Account UserAccount(string hash)
        {
            JObject answer = ExecuteServerApi(new Dictionary<string, string>()
            {
                ["method"] = "account",
                ["hash"] = hash,
            });

            if (answer.Value<string>("code") == "1")
            {
                JObject userJson = answer.Value<JObject>("user");
                Account account = new Account(userJson);
                return account;
            }
            else if(!answer.ContainsKey("type")) 
            {
                throw new ApiException(answer.Value<string>("message"));
            }

            throw new Exception(answer.Value<string>("message"));
        }

        public static Report[] UserReports(string hash)
        {
            JObject answer = ExecuteServerApi(new Dictionary<string, string>()
            {
                ["method"] = "mydata",
                ["hash"] = hash,
            });

            if (answer.Value<string>("code") == "1")
            {
                JArray reportsJson = answer.Value<JArray>("data");
                List<Report> reports = new List<Report>();
                foreach (JObject reportJson in reportsJson) {
                    Report report = new Report(reportJson);
                    reports.Add(report);
                }

                return reports.ToArray();
            }

            throw new ApiException(answer.Value<string>("message"));
        }

        public static ReportAttachment[] LoadAttachments(string hash, ReportAttachment[] attachments) {
            List<KeyValuePair<int, FileInfo>> onLoad = new List<KeyValuePair<int, FileInfo>>();
            for(int i = 0; i < attachments.Length; i++) {
                ReportAttachment att = attachments[i];
                if (att.IsLocal) {
                    FileInfo file = new FileInfo(att.PhotoUrl);
                    onLoad.Add(new KeyValuePair<int, FileInfo>(i, file));
                }
            }

            JObject answer = ExecuteServerApiPost(new Dictionary<string, object>() {
                ["method"] = "upload",
                ["hash"] = hash,
                ["file[]"] = onLoad.ConvertAll(new Converter<KeyValuePair<int, FileInfo>, FileInfo>((kv) => kv.Value)).ToArray(),
            });

            List<ReportAttachment> newAttachments = new List<ReportAttachment>();
            if (answer.Value<string>("code") == "1")
            {
                JArray attachmentsJson = answer.Value<JArray>("attachments");
                int attIndex = 0;
                for (int i = 0; i < attachments.Length; i++) {
                    ReportAttachment att = attachments[i];

                    bool isLoaded = false;
                    foreach(var kv in onLoad) if(kv.Key == i) { isLoaded = true; break; }

                    if (isLoaded)
                    {
                        if (attachmentsJson[attIndex].Type == JTokenType.String)
                        {
                            att.Status = attachmentsJson[attIndex].Value<string>();
                        }
                        else if (attachmentsJson[attIndex].Type == JTokenType.Object)
                        {
                            att = new ReportAttachment(attachmentsJson[attIndex].Value<JObject>())
                            {
                                Status = "загружен"
                            };
                        }

                        attIndex++;
                    }

                    newAttachments.Add(att);
                }
            }
            else 
            {
                string tt = answer.ToString();
                foreach (var att in attachments) {
                    att.Status = answer.Value<string>("message");

                    newAttachments.Add(att);
                }
            }

            return newAttachments.ToArray();
        }
    }

    public class ApiException : Exception {
        public ApiException() : base() { }
        public ApiException(string message) : base(message) { }
    }
}
