using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Weservice.Common;

namespace Weservice.Web.wx
{
    public partial class user_auth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string component_appid = "wx24f0419c030f897a";
            string pre_auth_code = Api.get_pre_auth_code();
            string redirect_uri = "http%3A%2F%2Fwww.liujingcan.cn%2Fwx%2Fuser_auth_result.aspx";
            Response.Redirect("https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid=" + component_appid +
                "&pre_auth_code=" + pre_auth_code + "&redirect_uri=" + redirect_uri);
        }
    }
}