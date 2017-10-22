using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weservice.Common
{
    public class ResultMessage
    {
        public int code;
        public string msg;
        public Object obj;
        public PageInfo page;

        public ResultMessage()
        {
            this.code = 0;
            this.msg = "success";
        }

        public ResultMessage(Object data)
        {
            this.code = 0;
            this.msg = "success";
            this.obj = data;
        }

        public ResultMessage(String msg)
        {
            this.code = 0;
            this.msg = "success";
            this.obj = msg;
        }

        public ResultMessage(String errmsg, int code)
        {
            this.code = code;
            this.msg = errmsg;
            this.obj = null;
        }
    }

    public class PageInfo
    {
        public int total_count;
        public int page_count;
        public int page_no;
    }
}
