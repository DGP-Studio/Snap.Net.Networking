using System;
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

        /// <summary>
        /// 无限等待，直到网络连接成功
        /// </summary>
        /// <returns>任务</returns>
        public static async Task WaitConnectionAsync()
        {
            NetworkChange.NetworkAddressChanged += HandleNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += HandleNetworkAvailabilityChanged;

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!Pinger.Test(ApiTakumi))
                {
                    NetworkConnected.Reset();
                }
            }
            else
            {
                NetworkConnected.Reset();
            }

            await Task.Run(() => NetworkConnected.WaitOne());

            NetworkChange.NetworkAddressChanged -= HandleNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged -= HandleNetworkAvailabilityChanged;
        }

        private static void HandleNetworkAddressChanged(object? s, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (Pinger.Test(ApiTakumi))
                {
                    NetworkConnected.Set();
                }
            }
        }

        private static void HandleNetworkAvailabilityChanged(object? s, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                if (Pinger.Test(ApiTakumi))
                {
                    NetworkConnected.Set();
                }
            }
        }
    }
}