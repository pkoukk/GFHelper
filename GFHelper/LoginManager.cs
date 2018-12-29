using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GFHelper
{
    class LoginManager
    {
        private Encoding encoding = Encoding.UTF8;

        internal Models.LoginResponse LoginResponse { get; private set; }

        internal Models.SignedData SignedData { get; private set; }

        delegate void LoginFinishDelegate();

        readonly LoginFinishDelegate loginFinish;

        delegate void LoginErrorDelegate(Exception exception);

        readonly LoginErrorDelegate loginError;

        LoginManager(LoginFinishDelegate loginFinish, LoginErrorDelegate loginError)
        {
            this.loginFinish = loginFinish;
            this.loginError = loginError;
        }

        async void Login(string usr,string pwd)
        {
            try
            {
                var stream = createLoginStream(usr, pwd, "0002000100021001");
                await PostLoginStream(stream);
                await GetSign();
                loginFinish();
            }
            catch(Exception ex)
            {
                loginError(ex);
            }
        }

        private byte[] createLoginStream(string usr, string pwd, string appId)
        {
            var encodedPwd = XXTEA.stringToMD5(pwd);
            var outJson = JsonConvert.SerializeObject(new { app_id = appId, login_identify = usr, login_pwd = encodedPwd, encrypt_mode = "md5" });
            byte[] key = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes("1234567890"), 0, key, 0, Math.Min(key.Length, Encoding.UTF8.GetBytes("1234567890").Length));
            var encodedBytes = XXTEA.encrypt(encoding.GetBytes(outJson), key);
            var headJson = JsonConvert.SerializeObject(new { app_id = appId, version = "1.0" });
            var buffer = new List<byte>();
            buffer.AddRange(encoding.GetBytes(headJson));
            buffer.Add(0);
            buffer.AddRange(encodedBytes);
            return buffer.ToArray();
        }

        async private Task PostLoginStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            HttpClient c = new HttpClient();
            HttpContent content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var res = await c.PostAsync("http://gf.ucenter.ppgame.com/normal_login", content);
            var text = await res.Content.ReadAsStringAsync();
            var loginResult = JsonConvert.DeserializeObject<Models.LoginResponse>(text);
            LoginResponse = loginResult;
        }

        async private Task GetSign()
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("open_id", LoginResponse.openid);
            valuePairs.Add("access_token", LoginResponse.access_token);
            valuePairs.Add("app_id", "");
            valuePairs.Add("channelid", "GWPZ");
            valuePairs.Add("idfa", "");
            valuePairs.Add("mac", "02:00:00:00:00:00");
            valuePairs.Add("androidid", RandomAndroidId());
            valuePairs.Add("req_id", $"{CommonHelper.TimeStampInSecond()}{CommonHelper.ReqSerial}");
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(valuePairs);
            var response = await client.PostAsync("" + RequestUrls.GetDigitalUid, content);
            var encryptedText = await response.Content.ReadAsStringAsync();
            var decoded = AuthCode.AutoDecode(encryptedText, "yundoudou");
            var signData = JsonConvert.DeserializeObject<Models.SignedData>(decoded);
            this.SignedData = signData;
        }

        private string RandomAndroidId()
        {
            Random r = new Random();
            string s = "";
            for (int i = 0; i < 16; i++)
            {
                s += r.Next(16).ToString("x");
            }

            return s;
        }
    }
}


