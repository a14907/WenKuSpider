using System.IO;
using System.Threading.Tasks;
using System.Text;
using System;

namespace Helper
{
    public static class StreamExtension
    {
        public static async Task<string> ReadAsGB2312Async(this Stream stream, int length)
        {
            var buf = new byte[length];
            int sum = 0;
            do
            {
                var readlen = await stream.ReadAsync(buf, sum, length - sum);
                sum += readlen;
            } while (sum < length);
            return Encoding.GetEncoding("GB2312").GetString(buf);
        }
        public static async Task SaveAsFile(this Stream stream, string filename, Action<string> log)
        {
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                var buf = new byte[1024 * 512];
                int sum = 0;
                int readlen = 0;
                do
                {
                    readlen = await stream.ReadAsync(buf, 0, buf.Length);
                    log($"读取的长度：{readlen * 1.0 / 1000}k，读取总长度：{sum / 1000}k");
                    sum += readlen;
                    fs.Write(buf, 0, readlen);
                } while (readlen > 0);
            }
        }
    }
}
