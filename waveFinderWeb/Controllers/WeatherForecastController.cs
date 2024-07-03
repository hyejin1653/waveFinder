using Microsoft.AspNetCore.Mvc;
using Pixoneer.NXDL;
using System.Net.Sockets;
using System.Text;
using waveFinderWeb;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace waveFinderWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class waveFinderController : ControllerBase
    {
        //private string str = waveFinderWeb.waveFinder.strData;

        public class location
        {
            public string lon;
            public string lat;
        }

        public class resultData
        {
            public string lon;
            public string lat;
            public decimal waveHeight;
            public decimal wavePeriod;
            public int waveDirection;
        }

        private string messageData(string strMessage)
        {
            string[] msg = strMessage.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            int count = msg.Length;

            JArray array = new JArray();    

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (msg[i].Length > 0)
                    {
                        string[] result = msg[i].Split(',');

                        string localNo = result[1]; //���ù�ȣ
                        DateTime targtDt = Convert.ToDateTime(result[2]); //�����ð�
                        string redarNo = result[3]; //���÷��̴����� �ؼ� ������ ��ȣ
                        int redarMeter = Convert.ToInt32(result[4]); //���÷��̴����� �ؼ� ���������� �Ÿ�(m)
                        int redarDeg = Convert.ToInt32(result[5]); //���÷��̴����� �ؼ� ������ ���� ������ [deg]
                        decimal height = Convert.ToDecimal(result[6]); //Wave Significant Height [m]
                        decimal period = Convert.ToDecimal(result[7]); //Wave Mean Period [sec]
                        int direction = Convert.ToInt32(result[8]); //Wave Peak Direction [deg]

                        location lonLat = getLonLat(localNo, redarDeg, redarMeter);
                        //lonLat.Add("waveHeight", waveHeight.ToString());
                        //lonLat.Add("wavePeriod", wavePeriod.ToString());
                        //lonLat.Add("waveDirection", waveDirection.ToString());
                        ////string localNm = (localNo == "1") ? "�︪��" : "����";
                        ////lonLat.Add("local_num", "\"" + localNm + "\"");
                        ////lonLat.Add("point_num", "\"" + redarNo + "\"");

                        resultData NresultData = new resultData
                        {
                            lon = lonLat.lon,
                            lat = lonLat.lat,
                            waveHeight = height,
                            wavePeriod = period,
                            waveDirection = direction
                        };
                        var json = JObject.FromObject(NresultData);
                        Console.WriteLine(json);

                        array.Add(json);
                    }
                }
            }
            Console.WriteLine(array);
            return array.ToString();
        }

        ////�Ÿ��� �������� ������ ��� ��ǥ �����ϱ�
        private location getLonLat(string no, int bear, int dis)
        {
            XAngle lon1 = new XAngle();
            XAngle lat1 = new XAngle();
            //�︪��
            if (no == "1")
            {
                //�︪�� : 37��31��20��N, 130��48��50��E
                lon1 = XAngle.FromDegree(130.8138);
                lat1 = XAngle.FromDegree(37.5222);
            }
            else if (no == "2")
            {
                //���� : 38��11��23��N, 128��36��14��E
                lon1 = XAngle.FromDegree(128.6038);
                lat1 = XAngle.FromDegree(38.18972);
            }

            XAngle bearing = XAngle.FromDegree(bear);
            double dist = dis;

            XAngle lon2 = new XAngle();
            XAngle lat2 = new XAngle();

            XAngle ang = Xfn.CalcPosByBearingAndDist(lon1, lat1, bearing, dist, ref lon2, ref lat2);

            location loc = new location
            {
                lon = lon2.deg.ToString(),
                lat = lat2.deg.ToString(),
            };

            return loc;
        }

        [HttpGet(Name = "waveFinder")]
        public string Get()
        {
            string msg = waveFinder.strData;
            //string msg = output;
            //messageData(msg);
            string dataList = messageData(msg);
            string result = dataList.Replace("\r\n", "");

            return result;
        }
    }
}