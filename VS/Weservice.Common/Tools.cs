using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Weservice.Common
{
    public class Tools
    {
        public static string GetPost(string key)
        {
            string text = "";
            try
            {
                text = HttpContext.Current.Request[key];
            }
            catch (Exception)
            {
            }
            if (text != null)
            {
                return text;
            }
            return string.Empty;
        }

        public static DateTime GetTimeFromUnix(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }

        public static string JsonToString(Object obj)
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(obj, iso);

        }
    }

    public class CustomMessageHandler : ThirdPartyMessageHandler
    {
        public CustomMessageHandler(Stream inputStream, PostModel postModel = null)
            : base(inputStream, postModel)
        {
        }
    }
}
