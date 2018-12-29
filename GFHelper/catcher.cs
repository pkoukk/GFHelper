using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace decoder
{
    public class catcher
    {
        //    private Decoder decoder;

        //    private HttpClient client;

        //    private int seria;

        //    private int uid = 1489384;

        //    private string lastChange;

        //    public catcher()
        //    {
        //        seria = 2;
        //        decoder = new Decoder();
        //        client = new HttpClient();
        //        decoder.decodeSin("#fmYK18PgbzdRD679ap1fm/7EtDPyQb3LOGnnarYZD3QtVc7Pru0eOtigbbv8v21RSYBlE2eVkf6eRvyqNQjioN7pdlKSO9hgT0shuu4EcU1diAZKjGNekaLiRhhfibVh4uxeMNR1wtLWucs9WVME7EjM2/HCglxKPORZyilR65SNHkpSBUG9irweZx30mVJ8KzToYd5oNbXeQnFC6Kbgk7uQ8AHCRjFTANMuHBVhYvGIuHUNjzqzKJnVSQHClgfSmaWmQ2B7vHfvw1pO2ccbi0dGoaCOw9fzWy291uiCmTaI0WuY");
        //    }

        //    public void start()
        //    {
        //        //startCatch();
        //        System.Timers.Timer timer = new Timer(30000);
        //        timer.AutoReset = true;
        //        timer.Enabled = true;
        //        timer.Elapsed += Timer_Elapsed;
        //    }

        //    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        //    {
        //        startCatch();
        //    }

        //    async public void startCatch()
        //    {
        //        try
        //        {
        //            var time = decoder.time();
        //            var req_id = time.ToString() + seria.ToString().PadLeft(4, '0');
        //            var outcode = decoder.encode();

        //            var dict = new Dictionary<string, string>();
        //            dict.Add("uid", uid.ToString());
        //            dict.Add("req_id", req_id);
        //            dict.Add("outdatacode", outcode);
        //            var client = new HttpClient();
        //            var req = new HttpRequestMessage(HttpMethod.Post, "http://gf-adrgw-cn-zs-game-0001.ppgame.com/index.php/1000/Gun/developLog") { Content = new FormUrlEncodedContent(dict) };
        //            var res = await client.SendAsync(req);
        //            var responseText = await res.Content.ReadAsStringAsync();
        //            var decoded = decoder.autoDecode(responseText);
        //            if (lastChange != decoded)
        //            {
        //                seria++;
        //                lastChange = decoded;
        //                File.WriteAllText($"./json/{req_id}.json", decoded);

        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }


        //    }
    }
}
