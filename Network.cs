using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Snap.Net.Networking
{
    /// <summary>
    /// 网络状态
    /// 仅针对米哈游服务器检测
    /// </summary>
    public static class Network
    {
        private const string ApiTakumi = "api-takumi.mihoyo.com";
        private static readonly ManualResetEvent NetworkConnected = new(true);

        static Network()
        {
            NetworkChange.NetworkAddressChanged += (s, e) =>
            {
                if (Pinger.Test(ApiTakumi))
                {
                    NetworkConnected.Set();
                }
            };

            NetworkChange.NetworkAvailabilityChanged += (s, e) =>
            {
                if (e.IsAvailable)
                {
                    if (Pinger.Test(ApiTakumi))
                    {
                        NetworkConnected.Set();
                    }
                }
            };
        }

        /// <summary>
        /// 无限等待，直到网络连接成功
        /// </summary>
        /// <returns>任务</returns>
        public static async Task WaitConnectionAsync()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                await Task.Delay(1000);
                if (Pinger.Test(ApiTakumi))
                {
                    return;
                }
                else
                {
                    NetworkConnected.Reset();
                }
            }
            else
            {
                NetworkConnected.Reset();
                await Task.Run(() => NetworkConnected.WaitOne());
                return;
            }
        }
    }
}