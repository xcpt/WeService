using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Weservice.Common;

namespace Weservice.Web.wx
{
    public partial class user_auth_result : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string auth_code = Tools.GetPost("auth_code");
            AuthResult ar = Api.refresh_authorizer_access_token(auth_code);
            Api.update_auth_info(ar);
        }
    }
}