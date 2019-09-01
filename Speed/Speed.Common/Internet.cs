using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class Internet
    {

        public static long GetFileSize(string url)
        {
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Method = "HEAD";
            System.Net.WebResponse resp = req.GetResponse();
            long length = 0;
            if (long.TryParse(resp.Headers.Get("Content-Length"), out length))
            {
                /*
                string File_Size;

                if (ContentLength >= 1073741824)
                {
                    result = ContentLength / 1073741824;
                    kbmbgb.Text = "GB";
                }
                else if (ContentLength >= 1048576)
                {
                    result = ContentLength / 1048576;
                    kbmbgb.Text = "MB";
                }
                else
                {
                    result = ContentLength / 1024;
                    kbmbgb.Text = "KB";
                }
                File_Size = result.ToString("0.00");
                sizevaluelabel.Text = File_Size;
                */
                return length;
            }
            else
                return -1;
        }

        public static double Dot2LongIP(string DottedIP)
        {
            int i;
            string[] arrDec;
            double num = 0;
            if (DottedIP == "")
            {
                return 0;
            }
            else
            {
                arrDec = DottedIP.Split('.');
                for (i = arrDec.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(arrDec[i]) % 256) * Math.Pow(256, (3 - i)));
                }
                return num;
            }
        }

    }
}
