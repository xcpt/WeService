using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Weservice.Common;

namespace Weservice.Web.wx
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //5.获取（刷新）授权公众号或小程序的接口调用凭据（令牌）
            //该API用于在授权方令牌（authorizer_access_token）失效时，可用刷新令牌（authorizer_refresh_token）获取新的令牌。
            //请注意，此处token是2小时刷新一次，开发者需要自行进行token的缓存，避免token的获取次数达到每日的限定额度。
            string authorizer_appid = "wx6e25266d24577e88";
            //Api.refresh_user_authorizer_access_token(authorizer_appid);

            //6、获取授权方的帐号基本信息
            Api.get_authorizer_info(authorizer_appid);
        }
    }
}