using Calculate;
using Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Service
{
    partial class ComputerMonitorService : ServiceBase
    {
        public ComputerMonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            new System.Threading.Thread(() =>
            {
                MongoServerSettings settings = new MongoServerSettings()
                {
                    Server = new MongoServerAddress("localhost")
                };
                MongoServer server = new MongoServer(settings);
                MongoDatabase db = server.GetDatabase("Computer_Profiler_Statistic");
                var collection = db.GetCollection<Record>("record");
                ComputerMonitor computer = new ComputerMonitor();
                while (true)
                {
                    Record record = new Record()
                    {
                        Cpu = new Cpu()
                        {
                            Utilization = computer.CpuUtilization,
                        },
                        Ram = new Ram()
                        {
                            Utilization = computer.RamUtilization
                        },
                        Process = new Model.Process()
                        {
                            Number = computer.ProcessCount,
                        },
                        Thread = new Model.Thread()
                        {
                            Number = computer.ThreadCount
                        },
                        Net = new Net()
                        {
                            Measurement = "Kbps"
                        }
                    };

                    double download = 0;
                    double upload = 0;

                    record.Net.Item = new List<NetItem>();

                    foreach (NetworkAdapter adapter in computer.NetworkAdapters)
                    {
                        record.Net.Item.Add(new NetItem()
                        {
                            Name = adapter.AdapterName,
                            Download = adapter.DownloadSpeedKbps,
                            Upload = adapter.UploadSpeedKbps,
                        });
                        if (adapter.DownloadSpeedKbps > download)
                        {
                            download = adapter.DownloadSpeedKbps;
                        }
                        if (adapter.UploadSpeedKbps > upload)
                        {
                            upload = adapter.UploadSpeedKbps;
                        }
                    }
                    record.Net.Download = download;
                    record.Net.Upload = upload;
                    collection.Insert(record);
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }).Start();
        }

        protected override void OnStop()
        {

        }
    }
}
