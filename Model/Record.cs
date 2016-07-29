using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Record
    {
        public Record()
        {
            this.Time = DateTime.Now;
        }

        public ObjectId _id { get; set; }

        public DateTime Time { get; set; }

        public Cpu Cpu { get; set; }

        public Ram Ram { get; set; }

        public Process Process { get; set; }

        public Thread Thread { get; set; }

        public Net Net { get; set; }

        public Temperature Temperature { get; set; }
    }

    public class Cpu
    {
        public float Utilization { get; set; }
    }

    public class Ram
    {
        public float Utilization { get; set; }
    }

    public class Process
    {
        public int Number { get; set; }
    }

    public class Thread
    {
        public int Number { get; set; }
    }

    public class Net
    {
        public string Measurement { get; set; }
        public double Download { get; set; }
        public double Upload { get; set; }
        public List<NetItem> Item { get; set; }
    }

    public class NetItem
    {
        public string Name { get; set; }
        public double Download { get; set; }
        public double Upload { get; set; }
    }


    public class Temperature
    {
        public double Cpu { get; set; }
    }

}
