using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Model;
using Calculate;
using System.Management;

namespace MongoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //MongoServerSettings settings = new MongoServerSettings()
            //{
            //    Server = new MongoServerAddress("localhost")
            //};
            //MongoServer server = new MongoServer(settings);
            //var db = server.GetDatabase("mongodb_learn");


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
                        },
                        Temperature = new Temperature()
                        {
                            Cpu = computer.CpuTemperature
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


                    Console.WriteLine(DateTime.Now);
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }).Start();

            //string cpuTemperature = "MSAcpi_ThermalZoneTemperature";
            //string diskTemperature = "MSStorageDriver_ATAPISmartData";


            //ManagementObjectSearcher management = new ManagementObjectSearcher(@"root\WMI", "Select * From " + cpuTemperature);
            //while (true)
            //{
            //    foreach (var item in management.Get())
            //    {
            //        var t = (Convert.ToDouble(item.GetPropertyValue("CurrentTemperature").ToString()) - 2732d) / 10d;
            //        Console.WriteLine(t);
            //    }
            //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            //}
        }
    }
}
