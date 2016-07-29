using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Calculate
{
    /// <summary>
    /// 监控每一个网的速度
    /// </summary>
    public class NetworkMonitor
    {
        /// <summary>
        /// The timer event executes every second to refresh the values in adapters.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The list of adapters on the computer.
        /// </summary>
        private List<NetworkAdapter> allAdapters;

        /// <summary>
        ///  The list of currently monitored adapters.
        /// </summary>
        private List<NetworkAdapter> monitoredAdapters;

        public NetworkMonitor()
        {
            this.allAdapters = new List<NetworkAdapter>();
            this.monitoredAdapters = new List<NetworkAdapter>();
            this.EnumerateNetworkAdapters();

            timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                foreach (NetworkAdapter adapter in this.monitoredAdapters)
                {
                    adapter.Refresh();
                }
            };
        }
        /// <summary>
        /// 枚举安装在计算机上的网络适配器。
        /// </summary>
        private void EnumerateNetworkAdapters()
        {
            PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");

            foreach (string name in category.GetInstanceNames())
            {
                if (name == "MS TCP Loopback interface")
                {
                    continue;
                }
                NetworkAdapter adapter = new NetworkAdapter(name)
                {
                    downloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name),
                    uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name)
                };
                this.allAdapters.Add(adapter);
            }
        }

        public List<NetworkAdapter> Adapters
        {
            get { return this.allAdapters; }
        }
        /// <summary>
        /// 启用计时器和所有适配器添加到受监视适配器列表，除非适配器列表为空。
        /// </summary>
        public void StartMonitoring()
        {
            if (this.allAdapters.Count > 0)
            {
                foreach (NetworkAdapter adapter in this.allAdapters)
                {
                    if (!this.monitoredAdapters.Contains(adapter))
                    {
                        this.monitoredAdapters.Add(adapter);
                        adapter.Init();
                    }
                }
                this.timer.Enabled = true;
            }
        }
        /// <summary>
        /// 启用定时器，以及指定的适配器添加到显示器适配器列表
        /// </summary>
        public void StartMonitoring(NetworkAdapter adapter)
        {
            if (!this.monitoredAdapters.Contains(adapter))
            {
                this.monitoredAdapters.Add(adapter);
                adapter.Init();
            }
            timer.Enabled = true;
        }

        /// <summary>
        /// 禁用计时器，并清除显示器适配器列表。
        /// </summary>
        public void StopMonitoring()
        {
            this.monitoredAdapters.Clear();
            timer.Enabled = false;
        }

        /// <summary>
        /// 从监测的适配器列表中删除指定的适配器，并禁用监测适配器列表的计时器是空的。
        /// </summary>
        /// <param name="adapter"></param>
        public void StopMonitoring(NetworkAdapter adapter)
        {
            if (this.monitoredAdapters.Contains(adapter))
            {
                this.monitoredAdapters.Remove(adapter);
            }
            if (this.monitoredAdapters.Count == 0)
            {
                timer.Enabled = false;
            }
        }
    }
}

