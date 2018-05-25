using System;
using System.IO;

namespace XSummitToDo.Helpers
{
    public static class ImageBaseConverter
    {
        public static string ConvertToBase64(Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);

            var bytes = ms.ToArray();

            return Convert.ToBase64String(bytes);
        }

        public static byte[] ConvertToByteArray(Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);

            return ms.ToArray();
        }
    }
}
