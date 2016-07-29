using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Calculate
{
    public class ComputerMonitor
    {
        private PerformanceCounter pc;

        private ComputerInfo computerInfo;

        ManagementObjectSearcher management;

        private NetworkMonitor networkMonitor;

        private List<NetworkAdapter> networkAdapters;

        public ComputerMonitor()
        {
            pc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            pc.ReadOnly = true;
            computerInfo = new ComputerInfo();
            management = new ManagementObjectSearcher(@"root\WMI", "Select * From MSAcpi_ThermalZoneTemperature");
            networkMonitor = new NetworkMonitor();
            networkAdapters = networkMonitor.Adapters;
            foreach (NetworkAdapter adapter in networkAdapters)
            {
                networkMonitor.StartMonitoring(adapter);
            }
        }

        /// <summary>
        /// cpu利用率
        /// </summary>
        public float CpuUtilization => pc.NextValue();

        public double CpuTemperature
        {
            get
            {
                var t = 0d;
                foreach (var item in management.Get())
                {
                    t = (Convert.ToDouble(item.GetPropertyValue("CurrentTemperature").ToString()) - 2732d) / 10d;
                }
                return t;
            }
        }


        /// <summary>
        /// 内存使用率
        /// </summary>
        public float RamUtilization => (1 - computerInfo.AvailablePhysicalMemory * 1f / computerInfo.TotalPhysicalMemory) * 100;

        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadCount
        {
            get
            {
                var count = 0;
                foreach (var process in Process.GetProcesses())
                {
                    count += process.Threads.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// 进程数
        /// </summary>
        public int ProcessCount => System.Diagnostics.Process.GetProcesses().Count();

        /// <summary>
        /// 网络适配器
        /// </summary>
        public List<NetworkAdapter> NetworkAdapters { get { return this.networkAdapters; } }
    }
}
