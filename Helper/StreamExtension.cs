using System.IO;
using System.Threading.Tasks;
using System.Text;

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
        public static async Task SaveAsFile(this Stream stream, string filename, int length)
        {
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                var buf = new byte[1024 * 512];
                int sum = 0;
                do
                {
                    var readlen = await stream.ReadAsync(buf, 0, buf.Length);
                    sum += readlen;
                    fs.Write(buf, 0, readlen);
                } while (sum < length);
            }
        }
    }
}
