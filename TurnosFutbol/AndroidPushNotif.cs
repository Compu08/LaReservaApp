using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TurnosFutbol
{
    public class AndroidNotif
    {
        public AndroidNotif()
        {

        }

        public bool Successful
        {
            get;
            set;
        }

        public string Response
        {
            get;
            set;
        }

        public Exception Error { get; set; }

        public AndroidNotif SendNotification(string _title, string _message, string _topic)
        {
            AndroidNotif result = new AndroidNotif();
            try
            {
                result.Successful = true;
                result.Error = null;
                // var value = message;
                var requestUri = "https://fcm.googleapis.com/fcm/send";

                WebRequest webRequest = WebRequest.Create(requestUri);
                webRequest.Method = "POST";
                webRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAA-Dtvq-4:APA91bGZAsZoevuLp7HTZyy-OfhlVfxxfCSTCmGrP5lgczlggDjgBZLYX2nVVxGLTkcHehKKmduqsHnjOrLgMvtYs7UD4GsiB-pFGsyiVfKvhrsQPhqbsA0HR1pBJEKSfqSVH0aqkt3m"));
                webRequest.Headers.Add(string.Format("Sender: id={0}", "1066149063662"));
                webRequest.ContentType = "application/json";

                var data = new
                {
                    to = _topic, // Uncoment this if you want to test for single device
                    //to = "/topics/" + _topic, // this is for topic 
                    notification = new
                    {
                        title = _title,
                        body = _message,
                        //icon="myicon"
                    }
                };
                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                webRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = webResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                result.Response = sResponseFromServer;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Successful = false;
                result.Response = null;
                result.Error = ex;
            }
            return result;
        }

    }
}
