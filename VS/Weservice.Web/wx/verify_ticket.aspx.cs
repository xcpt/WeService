using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Weservice.Common;
using Senparc.Weixin.Open.Entities.Request;

namespace Weservice.Web.wx
{
    //该页面由微信服务器每隔十分钟调用一次
    public partial class verify_ticket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string signature = Tools.GetPost("signature");
            string timestamp = Tools.GetPost("timestamp");
            string msg_signature = Tools.GetPost("msg_signature");
            string nonce = Tools.GetPost("nonce");

            var postModel = new PostModel()
            {
                AppId = "wx24f0419c030f897a",
                Token = "weihaofu",
                EncodingAESKey = "w1e2i3h4a5o6f7u8i9s0g1o2o3d4v5e6r7y8g9o0o1d",
                Msg_Signature = msg_signature,
                Signature = signature,
                Timestamp = timestamp,
                Nonce = nonce
            };

            
            try
            {
                //解密XML消息
                CustomMessageHandler cmh = new
                    CustomMessageHandler(Request.InputStream, postModel);
                //更新到数据库
                Api.update_ticket(cmh.RequestDocument);
            }
            catch(Exception ex)
            {
                LogHelper.WriteException(ex);
            }
        }
    }
}