using Snap.Core.Logging;
using System.Net.NetworkInformation;
using System.Text;

namespace Snap.Net.Networking
{
    public static class Pinger
    {
        public static bool TestIP(string host)
        {
            Logger.LogStatic("Ping Start");
            byte[] buffer = Encoding.ASCII.GetBytes("ping test data");
            PingOptions options = new() { DontFragment = true };
            try
            {
                PingReply reply = new Ping().Send(host, 120, buffer, options);
                Logger.LogStatic("Ping End");
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
