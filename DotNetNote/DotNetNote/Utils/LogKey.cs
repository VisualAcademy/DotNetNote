using System.Security.Cryptography;

namespace DotNetNote.Utils
{
    public static class LogKey
    {
        // 레코드 내용을 바탕으로 안정적인 해시 생성 (URL에 담을 짧은 키)
        public static string MakeKey(AppLog x)
        {
            var payload = $"{x.TimeStamp?.UtcDateTime.ToString("o")}|{x.Level}|{x.Message}|{x.MessageTemplate}|{x.Exception}|{x.Properties}";
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString(); // hex
        }
    }
}
