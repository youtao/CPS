﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Diagnostics;
using Model;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace WebUI.SignalR
{
    public class ChartHub : Hub<IChartHub>
    {
        private readonly Broadcaster broadcaster;
        public ChartHub() : this(Broadcaster.Instance)
        {
        }
        public ChartHub(Broadcaster broadcaster)
        {
            this.broadcaster = broadcaster;
        }

        public override Task OnConnected()
        {
            Interlocked.Increment(ref this.broadcaster.ConnectNumber);            
            MongoServerSettings settings = new MongoServerSettings();
            settings.Server = new MongoServerAddress("localhost", 27017);
            MongoServer server = new MongoServer(settings);
            MongoDatabase db = server.GetDatabase("Computer_Profiler_Statistic");
            var collection = db.GetCollection<Record>("record");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var data = collection.FindAll().OrderByDescending(e => e.Time)
                .Take(180)
                .ToList()
                .OrderBy(e => e.Time)
                .ToList();
            foreach (var item in data)
            {
                item.Time = item.Time.ToLocalTime();
            }            

            watch.Stop();
            Clients.Caller.Start(data, watch.Elapsed.Milliseconds);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (this.broadcaster.ConnectNumber>0)
            {
                Interlocked.Decrement(ref this.broadcaster.ConnectNumber);
            }                  
            return base.OnDisconnected(stopCalled);
        }
    }

    public interface IChartHub
    {
        void Start(object data, double time);
        void Update(object data);
    }

    public class Broadcaster
    {
        private static readonly Lazy<Broadcaster> _instance = new Lazy<Broadcaster>(() => new Broadcaster());

        private readonly TimeSpan interval = TimeSpan.FromSeconds(2);

        private readonly IHubContext hubContext;

        private readonly object _clock = new object();

        private Timer timer;

        public int ConnectNumber = 0;


        public static Broadcaster Instance => _instance.Value;

        public Broadcaster()
        {
            this.hubContext = GlobalHost.ConnectionManager.GetHubContext<ChartHub>();
            this.timer = new Timer(e =>
            {
                lock (_clock)
                {
                    if (ConnectNumber > 0)
                    {
                        MongoServerSettings settings = new MongoServerSettings();
                        settings.Server = new MongoServerAddress("localhost", 27017);
                        MongoServer server = new MongoServer(settings);
                        MongoDatabase db = server.GetDatabase("Computer_Profiler_Statistic");
                        var collection = db.GetCollection<Record>("record");
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        var json = collection.FindAll().OrderByDescending(c => c.Time).FirstOrDefault();
                        if (json != null)
                        {
                            json.Time = json.Time.ToLocalTime();
                        }
                        watch.Stop();
                        this.hubContext.Clients.All.update(json, watch.Elapsed.Milliseconds);
                    }
                }
                
            }, null, this.interval, this.interval);
        }
    }
}