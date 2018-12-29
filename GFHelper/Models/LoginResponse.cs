using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFHelper.Models
{
    public class LoginResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string register_mode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string app_id { get; set; }
    }
}
