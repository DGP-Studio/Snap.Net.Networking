using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Snap.Net.Networking
{
    public static class Network
    {
        private static readonly ManualResetEvent networkConnected = new(true);
        static Network()
        {
            NetworkChange.NetworkAvailabilityChanged += (s, e) =>
            {
                if (e.IsAvailable)
                {
                    if (Pinger.TestIP("8.8.8.8"))
                    {
                        networkConnected.Set();
                    }
                }
            };
        }

        /// <summary>
        /// 无限等待，直到网络连接成功
        /// </summary>
        public static async Task WaitConnectionAsync()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (Pinger.TestIP("8.8.8.8"))
                {
                    return;
                }
                else
                {
                    networkConnected.Reset();
                }
            }
            else
            {
                networkConnected.Reset();
                await Task.Run(() => networkConnected.WaitOne());
                return;
            }
        }
    }
}
