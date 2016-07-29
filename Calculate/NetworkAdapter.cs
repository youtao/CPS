using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate
{
    /// <summary>
    /// 网络适配器
    /// </summary>
    public class NetworkAdapter
    {
        internal NetworkAdapter(string adapterName)
        {
            this._adapterName = adapterName;
        }

        /// <summary>
        /// 下载上传速度(bytes/s)
        /// </summary>
        private long downloadSpeed, uploadSpeed;

        /// <summary>
        ///  计数器下载上传的值(bytes)
        /// </summary>
        private long downloadValue, uploadValue;

        /// <summary>
        /// 上一秒计数器下载上传的值(bytes)
        /// </summary>
        private long downloadValueOld, uploadValueOld;

        /// <summary>
        /// 适配器名
        /// </summary>
        private string _adapterName;

        /// <summary>
        ///  下载性能计数器
        /// </summary>
        internal PerformanceCounter downloadCounter;

        /// <summary>
        /// 上传性能计数器
        /// </summary>
        internal PerformanceCounter uploadCounter;

        /// <summary>
        /// 开始监视
        /// </summary>
        internal void Init()
        {
            this.downloadValueOld = this.downloadCounter.NextSample().RawValue;
            this.uploadValueOld = this.uploadCounter.NextSample().RawValue;
        }
        /// <summary>
        /// Obtain new sample from performance counters, and refresh the values saved in dlSpeed, ulSpeed, etc.
        /// This method is supposed to be called only in NetworkMonitor, one time every second.
        /// </summary>
        internal void Refresh()
        {
            this.downloadValue = this.downloadCounter.NextSample().RawValue;
            this.uploadValue = this.uploadCounter.NextSample().RawValue;

            this.downloadSpeed = this.downloadValue - this.downloadValueOld;
            this.uploadSpeed = this.uploadValue - this.uploadValueOld;

            this.downloadValueOld = this.downloadValue;
            this.uploadValueOld = this.uploadValue;
        }

        public override string ToString()
        {
            return this._adapterName;
        }
        /// <summary>
        /// The name of the network adapter.
        /// </summary>
        public string AdapterName
        {
            get { return this._adapterName; }
        }
        /// <summary>
        /// Current download speed in bytes per second.
        /// </summary>
        public long DownloadSpeed
        {
            get { return this.downloadSpeed; }
        }
        /// <summary>
        /// Current upload speed in bytes per second.
        /// </summary>
        public long UploadSpeed
        {
            get { return this.uploadSpeed; }
        }
        /// <summary>
        /// Current download speed in kbytes per second.
        /// </summary>
        public double DownloadSpeedKbps
        {
            get { return this.downloadSpeed / 1024d; }
        }
        /// <summary>
        /// Current upload speed in kbytes per second.
        /// </summary>
        public double UploadSpeedKbps
        {
            get { return this.uploadSpeed / 1024d; }
        }
    }
}
