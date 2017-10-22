using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Weservice.Common;

namespace Weservice.BLL
{
    public partial class wechatservice
    {
      
        public wechatservice()
        {
             
        }

 
 

        public void refresh_authorizer_access_token(string auth_code)
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token="  ;
                string json = "{\"component_appid\":\"wx24f0419c030f897a\",\"authorization_code\":" +
                    "\"" + auth_code + "\"}";
               // string result = RequestUtility.HttpPost(url, null, new MemoryStream(System.Text.Encoding.Default.GetBytes(json)));
               // LogHelper.WriteInfo(result);
            }
            catch (Exception e)
            {
                LogHelper.WriteException(e);
            }
        }

    }




}