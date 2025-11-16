using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LenfLock.Utility.Communication {
    internal class CommandHanlder {
        public enum ActionCode {
            None,
            OpenApp,
            HideApp,
            FreezeApp
        }
        public class HanlderPack {
            public ActionCode code { get; set; }
            public string msg {  get; set; }
            public HanlderPack(ActionCode code) {
                this.code = code;
                this.msg = "";
            }
            public HanlderPack(ActionCode code, string msg) {
                this.code = code;
                this.msg = msg;
            }
        }
        public CommandHanlder() { }
        public static HanlderPack Command(string command) {
            if (command == "secondApp") {
                return new HanlderPack(ActionCode.OpenApp);
            }
            if (command == "exit") {
                return new HanlderPack(ActionCode.None);
            }
            if (command == "openapp" || command.Contains("HTTP")) {
                return new HanlderPack(ActionCode.OpenApp);
            }
            if (command == "status") {
                string str = "";
                MainInterface.instance.Invoke((MethodInvoker)delegate {
                    var a = MainInterface.instance.isShow;
                    str = a.ToString();
                });
                return new HanlderPack(ActionCode.None, str);
            }
            JObject? json = null;
            try {
                if (command == "") return new HanlderPack(ActionCode.None);
                json = JObject.Parse(command);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return new HanlderPack(ActionCode.None);
            }
            if (json.TryGetValue("type", out var typeToken) &&
                        typeToken.Type == JTokenType.String) {
                if (typeToken.ToString() == "ping") {
                    var responseObj = new {
                        success = true,
                        message = "pong",
                        data = new { device = "screenlock" },
                        timestamp = DateTime.Now.ToString()
                    };

                    string jsonResponse = JsonConvert.SerializeObject(responseObj);
                    return new HanlderPack(ActionCode.None, jsonResponse);
                }
                if (typeToken.ToString() == "status") {
                    bool isShow = false;
                    MainInterface.instance.Invoke((MethodInvoker)delegate {
                        isShow = MainInterface.instance.isShow;
                    });

                    var responseObj = new {
                        success = true,
                        message= "Status retrieved",
                        data = new {
                            status = isShow ? "locked" : "unlocked",
                            device = "screenLock",
                            version = "1.0"
                        },
                        timestamp = DateTime.Now.ToString()
                    };

                    string jsonResponse = JsonConvert.SerializeObject(responseObj);
                    return new HanlderPack(ActionCode.None, jsonResponse);
                }
                if (typeToken.ToString() == "lock") {
                    var responseObj = new {
                        success = true,
                        message = "Screen locked successfully",
                        data = new { status = "locked" },
                        timestamp = DateTime.Now.ToString()
                    };

                    string jsonResponse = JsonConvert.SerializeObject(responseObj);
                    return new HanlderPack(ActionCode.OpenApp, jsonResponse);
                }
                if (typeToken.ToString() == "unlock") {
                    var responseObj = new {
                        success = true,
                        message = "Screen unlocked successfully",
                        data = new { status = "unlocked" },
                        timestamp = DateTime.Now.ToString()
                    };

                    string jsonResponse = JsonConvert.SerializeObject(responseObj);
                    return new HanlderPack(ActionCode.HideApp, jsonResponse);
                }
                if (typeToken.ToString() == "freeze") {
                    var responseObj = new {
                        success = true,
                        message = "Screen frozen successfully",
                        data = new { status = "frozen" },
                        timestamp = DateTime.Now.ToString()
                    };

                    string jsonResponse = JsonConvert.SerializeObject(responseObj);
                    return new HanlderPack(ActionCode.FreezeApp, jsonResponse);
                }
            }
            return new HanlderPack(ActionCode.None);
        }
    }
}
