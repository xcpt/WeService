using Senparc.Weixin.HttpUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Weservice.Common
{
    public class Api
    {
        public static void update_ticket(XDocument xdoc)
        {
            LogHelper.WriteInfo(xdoc.ToString());
            WeserviceEntities entity = new WeserviceEntities();
            string InfoType = xdoc.Root.XPathSelectElement("//InfoType").Value;
            string AppId = xdoc.Root.XPathSelectElement("//AppId").Value;
            string CreateTime = xdoc.Root.XPathSelectElement("//CreateTime").Value;
            string ComponentVerifyTicket = "";
            if (InfoType == "component_verify_ticket")
                ComponentVerifyTicket = xdoc.Root.XPathSelectElement("//ComponentVerifyTicket").Value;
            var list = entity.dt_wx_token.Where(s => s.info_type == InfoType && s.appid == AppId).ToList();
            //有对应记录则更新
            if (list.Count > 0)
            {
                var item = list[0];
                item.last_time = Tools.GetTimeFromUnix(int.Parse(CreateTime));
                item.token = ComponentVerifyTicket;
            }
            //没有则插入新记录
            else
            {
                dt_wx_token item = new dt_wx_token
                {
                    info_type = InfoType,
                    appid = AppId,
                    last_time = Tools.GetTimeFromUnix(int.Parse(CreateTime)),
                    token = ComponentVerifyTicket
                };
                entity.dt_wx_token.Add(item);
            }
            int count = entity.SaveChanges();
            LogHelper.WriteInfo("update_ticket => count1 : " + count);
        }

        public static void get_authorizer_info(string authorizer_appid)
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_info?component_access_token=" + get_component_access_token();
                string json = "{\"component_appid\":\"wx24f0419c030f897a\",\"authorizer_appid\":" +
                    "\"" + authorizer_appid + "\"}";
                //string str = RequestUtility.HttpPost(url, null, new MemoryStream(System.Text.Encoding.Default.GetBytes(json)));
                AccountResult ar = Post.PostGetJson<AccountResult>(url, null, new MemoryStream(System.Text.Encoding.Default.GetBytes(json)));
                //LogHelper.WriteInfo(str);
                //LogHelper.WriteInfo(Tools.JsonToString(ar));
            }
            catch (Exception e)
            {
                LogHelper.WriteException(e);
            }
        }

        public static void refresh_user_authorizer_access_token(string authorizer_appid)
        {
            WeserviceEntities entity = new WeserviceEntities();
            var list = entity.dt_wx_auth_info.Where(s => s.authorizer_appid == authorizer_appid).ToList();
            if (list.Count > 0)
            {
                var item = list[0];
                double ts = DateTime.Now.Subtract((DateTime)item.last_time).TotalSeconds;
                if (ts > (item.expires_in - 600))//已过期
                {
                    try
                    {
                        string url = "https:// api.weixin.qq.com /cgi-bin/component/api_authorizer_token?component_access_token=" + get_component_access_token();
                        string json = "{\"component_appid\":\"wx24f0419c030f897a\",\"authorizer_appid\":" +
                            "\"" + authorizer_appid + "\",\"authorizer_refresh_token\":\"" + item.authorizer_refresh_token + "\"}";
                        AuthResult ar = Post.PostGetJson<AuthResult>(url, null, new MemoryStream(System.Text.Encoding.Default.GetBytes(json)));
                        item.authorizer_access_token = ar.authorization_info.authorizer_access_token;
                        item.expires_in = ar.authorization_info.expires_in;
                        item.authorizer_refresh_token = ar.authorization_info.authorizer_refresh_token;

                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteException(e);
                    }
                }
            }
            int count = entity.SaveChanges();
            LogHelper.WriteInfo("refresh_user_authorizer_access_token => count5 : " + count);
        }

        public static void update_auth_info(AuthResult ar)
        {
            WeserviceEntities entity = new WeserviceEntities();
            var list = entity.dt_wx_auth_info.Where(s => s.authorizer_appid == ar.authorization_info.authorizer_appid).ToList();
            if (list.Count == 0)
            {
                var item = new dt_wx_auth_info
                {
                    authorizer_appid = ar.authorization_info.authorizer_appid,
                    authorizer_access_token = ar.authorization_info.authorizer_access_token,
                    authorizer_refresh_token = ar.authorization_info.authorizer_refresh_token,
                    expires_in = ar.authorization_info.expires_in,
                    last_time = DateTime.Now,
                    func_info = Tools.JsonToString(ar.authorization_info.func_info)
                };
                entity.dt_wx_auth_info.Add(item);
            }
            else
            {
                var item = list[0];
                item.authorizer_access_token = ar.authorization_info.authorizer_access_token;
                item.authorizer_refresh_token = ar.authorization_info.authorizer_refresh_token;
                item.expires_in = ar.authorization_info.expires_in;
                item.last_time = DateTime.Now;
                item.func_info = Tools.JsonToString(ar.authorization_info.func_info);
            }
            int count = entity.SaveChanges();
            LogHelper.WriteInfo("update_auth_info => count4 : " + count);
        }

        public static AuthResult refresh_authorizer_access_token(string auth_code)
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token=" + get_component_access_token();
                string json = "{\"component_appid\":\"wx24f0419c030f897a\",\"authorization_code\":" +
                    "\"" + auth_code + "\"}";
                AuthResult ar = Post.PostGetJson<AuthResult>(url, null, new MemoryStream(System.Text.Encoding.Default.GetBytes(json)));
                return ar;
                //LogHelper.WriteInfo(Tools.JsonToString(ar));

            }
            catch (Exception e)
            {
                LogHelper.WriteException(e);
            }
            return null;
        }

        private static string get_component_access_token()
        {
            WeserviceEntities entity = new WeserviceEntities();
            var list = entity.dt_wx_token.Where(s => s.info_type == "component_verify_ticket").ToList();
            if (list.Count == 0)
                return "";
            var ticket_item = list[0];
            //刷新获取第三方平台component_access_token
            list = entity.dt_wx_token.Where(s => s.info_type == "component_access_token").ToList();
            string component_access_token = "";
            if (list.Count > 0)
            {
                var token = list[0];
                double ts = DateTime.Now.Subtract((DateTime)token.last_time).TotalSeconds;
                if (ts > (token.expires_in - 600) || token.token == null)//已过期
                {
                    //刷新token
                    TokenResult tr = refresh_component_access_token(ticket_item.token);
                    if (tr != null)
                    {
                        token.last_time = DateTime.Now;
                        token.token = tr.component_access_token;
                        token.expires_in = tr.expires_in;
                        token.appid = ticket_item.appid;
                    }
                }
                component_access_token = token.token;
            }
            else
            {
                TokenResult tr = refresh_component_access_token(ticket_item.token);
                if (tr != null)
                {
                    var new_token = new dt_wx_token
                    {
                        info_type = "component_access_token",
                        last_time = DateTime.Now,
                        token = tr.component_access_token,
                        expires_in = tr.expires_in,
                        appid = ticket_item.appid
                    };
                    component_access_token = new_token.token;
                    entity.dt_wx_token.Add(new_token);
                }
            }
            int count = entity.SaveChanges();
            LogHelper.WriteInfo("update_ticket => count2 : " + count);
            return component_access_token;
        }

        public static string get_pre_auth_code()
        {
            //刷新获取预授权码pre_auth_code
            TokenResult trp = refresh_pre_auth_code(get_component_access_token());
            return trp.pre_auth_code;
        }

        private static TokenResult refresh_component_access_token(string ticket)
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/component/api_component_token";
                string json = "{\"component_appid\":\"wx24f0419c030f897a\",\"component_appsecret\":" +
                    "\"6fba5eb2c4f3f118478608e7d854bace\",\"component_verify_ticket\":" +
                    "\"" + ticket + "\"}";
                TokenResult tr = Post.PostGetJson<TokenResult>(url, null, new MemoryStream(System.Text.Encoding.Default.GetBytes(json)));
                return tr;
            }
            catch (Exception e)
            {
                LogHelper.WriteException(e);
            }
            return null;
        }

        private static TokenResult refresh_pre_auth_code(string component_access_token)
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token=" + component_access_token;
                string json = "{\"component_appid\":\"wx24f0419c030f897a\"}";
                TokenResult tr = Post.PostGetJson<TokenResult>(url, null, new MemoryStream(System.Text.Encoding.Default.GetBytes(json)));
                return tr;
            }
            catch (Exception e)
            {
                LogHelper.WriteException(e);
            }
            return null;
        }
    }

    public class TokenResult
    {
        public string component_access_token { get; set; }
        public string pre_auth_code { get; set; }
        public int expires_in { get; set; }
    }

    public class AuthResult
    {
        public authorization_info authorization_info { get; set; }
    }

    public class authorization_info
    {
        public string authorizer_appid { get; set; }
        public string authorizer_access_token { get; set; }
        public int expires_in { get; set; }
        public string authorizer_refresh_token { get; set; }
        public List<func_info> func_info { get; set; }
    }

    public class func_info
    {
        public funcscope_category funcscope_category { get; set; }
        public confirm_info confirm_info { get; set; }
    }

    public class funcscope_category
    {
        public int id { get; set; }
    }

    public class confirm_info
    {
        public int need_confirm { get; set; }
        public int already_confirm { get; set; }
        public int can_confirm { get; set; }
    }

    public class AccountResult
    {
        public authorizer_info authorizer_info { get; set; }
        public authorization_info authorization_info { get; set; }
    }

    public class authorizer_info
    {
        public string nick_name { get; set; }
        public string head_img { get; set; }
        public service_type_info service_type_info { get; set; }
        public verify_type_info verify_type_info { get; set; }
        public string user_name { get; set; }
        public string principal_name { get; set; }
        public string signature { get; set; }
        public int idc { get; set; }
        public business_info business_info { get; set; }
        public string alias { get; set; }
        public string qrcode_url { get; set; }
    }

    public class service_type_info
    {
        public int id { get; set; }
    }

    public class verify_type_info
    {
        public int id { get; set; }
    }

    public class business_info
    {
        public int open_store { get; set; }
        public int open_scan { get; set; }
        public int open_pay { get; set; }
        public int open_card { get; set; }
        public int open_shake { get; set; }
    }
}
