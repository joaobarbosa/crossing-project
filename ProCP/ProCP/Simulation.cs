﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Threading;

namespace ProCP
{
    [DataContract(Name = "Simulation")]
    class Simulation
    {
        /// <summary>
        /// properties
        /// </summary>
        public int TotalNumberCars { get; set; }
        public int TotalNumberofSwitches { get; set; }
        public System.Diagnostics.Stopwatch Watch = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// fields
        /// </summary>
        private List<Crossing> crossings;
        private Car car;
        private List<Car> cars;
        private bool saved;
        private string name;

        /// <summary>
        /// name of the simulation
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// if it has been saved
        /// </summary>
        public bool Saved
        {
            get { return saved; }
            set { saved = value; }
        }

        /// <summary>
        /// All the crossings in the simulation
        /// </summary>
        public List<Crossing> Crossings
        {
            get { return crossings; }
            set { crossings = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public Simulation(string name = "")
        {
            Crossings = new List<Crossing>();
            Name = name;
            Saved = true;
        }

        /// <summary>
        /// Starts the simulation in which the statistics are started and the cars are called to be created
        /// </summary>
        public void Start()
        {
            TotalNumberCars = 0;
            Watch.Reset();
            TotalNumberofSwitches = 0;
            foreach (Crossing i in Crossings)
            {
                TotalNumberCars += i.NumCars;
                //if (i.GetType() == typeof(Crossing_B))
                //{
                //    Crossing_B temp = (Crossing_B)i;
                //}
            }
            Watch.Start();
            CreateCars();
        }
        /// <summary>
        /// stops the stopwatch
        /// </summary>
        public void Stop()
        {
            StopTimers();
            Watch.Stop();
        }

        /// <summary>
        /// Adds a crossing to the list of crossing
        /// </summary>
        /// <param name="Crossing"></param>
        /// <returns></returns>
        public bool AddCrossing(Crossing Crossing)
        {
            if (Crossings.Count < 12)
            {
                Crossings.Add(Crossing);
                Saved = false;
                return true;
            }

            MessageBox.Show("Remove a crossing to be able to add another one. You can have only 12 crossings at the same time.", "Crossing limit reached");
            return false;
        }

        /// <summary>
        /// Will determine whether lanes are sink or feeders or normal lanes.
        /// </summary>
        public void MarkLanes()
        {
            foreach (Crossing i in Crossings)
            {
                //NORTH
                if (Crossings.Find(x => x.CrossingId == (i.CrossingId) - 4) == null)
                {
                    foreach (TrafficLane j in i.Lanes)
                    {
                        if (j.Direction == Direction.NORTH && (!j.ToFromCross))
                        {
                            j.LaneType = false;
                        }
                        if (j.Direction == Direction.SOUTH && (j.ToFromCross))
                        {
                            j.LaneType = true;
                        }

                    }
                }
                //EAST
                if (Crossings.Find(x => x.CrossingId == (i.CrossingId) + 1) == null || (i.CrossingId % 4 == 0))
                {
                    foreach (TrafficLane j in i.Lanes)
                    {
                        if (j.Direction == Direction.EAST && (!j.ToFromCross))
                        {
                            j.LaneType = false;
                        }
                        if (j.Direction == Direction.WEST && (j.ToFromCross))
                        {
                            j.LaneType = true;
                        }
                    }
                }
                //SOUTH
                if (Crossings.Find(x => x.CrossingId == (i.CrossingId) + 4) == null)
                {
                    foreach (TrafficLane j in i.Lanes)
                    {
                        if (j.Direction == Direction.SOUTH && (!j.ToFromCross))
                        {
                            j.LaneType = false;
                        }
                        if (j.Direction == Direction.NORTH && (j.ToFromCross))
                        {
                            j.LaneType = true;
                        }
                    }
                }
                //WEST
                if (Crossings.Find(x => x.CrossingId == (i.CrossingId) - 1) == null || (i.CrossingId % 4 == 1))
                {
                    foreach (TrafficLane j in i.Lanes)
                    {
                        if (j.Direction == Direction.WEST && (!j.ToFromCross))
                        {
                            j.LaneType = false;
                        }
                        if (j.Direction == Direction.EAST && (j.ToFromCross))
                        {
                            j.LaneType = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the inter corssing lane connections
        /// </summary>
        public void LaneCrossingConnection()
        {
            foreach (Crossing i in Crossings)
            {
                foreach (TrafficLane j in i.Lanes)
                {
                    if (!(j.ToFromCross) && (j.LaneType == null))
                    {
                        if (j.Direction == Direction.NORTH)
                        {
                            j.Lanes.AddRange(Crossings.Find(x => x.CrossingId == i.CrossingId - 4).LanesInDirection(j.Direction));
                        }
                        if (j.Direction == Direction.EAST)
                        {
                            j.Lanes.AddRange(Crossings.Find(x => x.CrossingId == i.CrossingId + 1).LanesInDirection(j.Direction));
                        }
                        if (j.Direction == Direction.SOUTH)
                        {
                            j.Lanes.AddRange(Crossings.Find(x => x.CrossingId == i.CrossingId + 4).LanesInDirection(j.Direction));
                        }
                        if (j.Direction == Direction.WEST)
                        {
                            j.Lanes.AddRange(Crossings.Find(x => x.CrossingId == i.CrossingId - 1).LanesInDirection(j.Direction));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Edits the properties of the crossing
        /// eg. number of cars for that crossing, time the light is green, etc.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="numCars"></param>
        /// <param name="time"></param>
        /// <param name="style"></param>
        public void EditCrossing(int id, int numCars, int time, string style)
        {
            Crossing cr = Crossings.Find(x => x.CrossingId == (id));

            if (cr.GetType() == typeof(Crossing_A))
            {
                cr.NumCars = numCars;
                cr.Time = new TimeSpan(0, 0, time);
            }
            else
            {
                Crossing_B cr1 = (Crossing_B)cr;
                cr1.NumCars = numCars;
                cr1.Time = new TimeSpan(0, 0, time);
                cr1.style = style;
                cr1.CreatePedestrians();
            }

        }

        /// <summary>
        /// turns the style into a string
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private int whichPedStyle(string style)
        {
            if (style == "Quiet") return 1;
            if (style == "Busy") return 2;
            else return -1;
        }

        /// <summary>
        /// Gets the properties of the individual crossing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nCars"></param>
        /// <param name="time"></param>
        /// <param name="style"></param>
        public void getProperties(int id, ref int nCars, ref int time, ref string style)
        {
            Crossing cr = Crossings.Find(x => x.CrossingId == (id));

            nCars = cr.NumCars;
            time = cr.Time.Seconds;

            if (cr.GetType() == typeof(Crossing_B))
            {
                Crossing_B cr1 = (Crossing_B)cr;
                //nPed = cr1.NumPeds;
                style = cr1.style;
            }

        }

        /// <summary>
        /// checks if that specific crossing exists in the list of crossing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CrossingExist(int id)
        {
            return Crossings.Exists(x => x.CrossingId == (id));
        }

        /// <summary>
        /// removes the crossing from the list of crossings
        /// </summary>
        /// <param name="id"></param>
        public void RemoveCrossing(int id)
        {
            Crossings.Remove(Crossings.Find(x => x.CrossingId == (id)));
            Saved = false;
        }

        /// <summary>
        /// creates the cars for each lane in each crossing
        /// </summary>
        public void CreateCars()
        {
            cars = new List<Car>();
            List<TrafficLane> tmp = new List<TrafficLane>();
            Random rnd = new Random();
            Color c;

            foreach (Crossing item in Crossings)
            {
                tmp.AddRange(item.Lanes.FindAll(x => x.LaneType == true));

                for (int i = 0; i < item.NumCars; i++)
                {
                    c = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));

                    // car = new Car(count, c, item); // DEBUG
                    car = new Car(c, tmp.ElementAt(rnd.Next(tmp.Count())));


                    cars.Add(car);
                }
                tmp = new List<TrafficLane>();

            }
        }

        /// <summary>
        /// returns a crossing with that id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Crossing getCrossing(int id)
        {
            return Crossings.Find(x => x.CrossingId == (id));
        }

        /// <summary>
        /// Save the simulation to a file
        /// </summary>
        /// <param name="filename">The name of the file to which to save the simulation</param>
        public void SaveAs(string filename)
        {
            Saved = true;

            FileStream writer = new FileStream(filename, FileMode.Create);
            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(List<Crossing>));
            dataContractSerializer.WriteObject(writer, crossings);

            writer.Close();
        }

        /// <summary>
        /// Returns true when loaded, passes new loaded BaseBate list to the exisiting BaseGate list.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Load(string filename)
        {
            //List<Crossing> ret;

            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(List<Crossing>));
            List<Crossing> deserializedCrossings = (List<Crossing>)ser.ReadObject(reader, true);
           // ret = (Simulation)ser.ReadObject(reader, true);
            Crossings.Clear();
            Crossings = deserializedCrossings;
            reader.Close();
            fs.Close();

            return true;
        }

        /// <summary>
        /// checks whether the crossing is surrounded
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Surrounded(int id)
        {
            if ((crossings.Exists(x => x.CrossingId == id - 4)) && ((crossings.Exists(x => x.CrossingId == id - 1) && (crossings.Find(x => x.CrossingId == id).CrossingId % 4 != 1)))
                 && (crossings.Exists(x => x.CrossingId == id + 4)) && ((crossings.Exists(x => x.CrossingId == id + 1) && (crossings.Find(x => x.CrossingId == id).CrossingId % 4 != 0))))
            {
                crossings.Find(x => x.CrossingId == id).NumCars = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Light Logic
        /// <summary>
        /// fields of the light logic
        /// </summary>
        System.Windows.Forms.Timer temp;
        List<System.Windows.Forms.Timer> timers = new List<System.Windows.Forms.Timer>();

        /// <summary>
        /// starts the thread
        /// </summary>
        public void StartTimers()
        {
            foreach (Crossing c in Crossings)
            {
                temp = new System.Windows.Forms.Timer();
                temp.Interval = c.Time.Seconds * 1000;
                temp.Enabled = true;
                temp.Tick += new EventHandler((sender, e) => TickEvent(sender, e, c));
                c.ATimer = temp;
            }
            this.StartT();
        }

        /// <summary>
        /// Stops the Timers
        /// </summary>
        private void StopTimers()
        {
            foreach (Crossing i in crossings)
            {
                i.ATimer.Stop();
            }
        }

        /// <summary>
        /// changes the lights to the next iteration
        /// </summary>
        /// <param name="c"></param>
        public void SwitchAll(Crossing c)
        {
            TotalNumberofSwitches++;

            if (c.GetType() == typeof(Crossing_A))
            {
                /*
                #region A Crossing

                foreach (TrafficLane l in c.Lanes)
                {
                    if (l.TrafficLight != null)
                    {
                        if (l.Direction == (Direction)(c.Turn - 1))
                        {
                            l.TrafficLight.State = true;
                        }
                        else
                        {
                            l.TrafficLight.State = false;
                        }
                    }
                    c.Turn = ((c.Turn + 1) % 4);

                    if (c.Turn == 0)
                        c.Turn = 1;
                }

                #endregion
                */

                #region A Crossing
                switch (c.Turn)
                {
                    case 1:
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                if (l.Direction == Direction.NORTH)
                                {
                                    l.TrafficLight.State = true;
                                }
                                else
                                {
                                    l.TrafficLight.State = false;
                                }
                            }
                        }
                        c.Turn++;
                        break;
                    case 2:
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                if (l.Direction == Direction.EAST)
                                {
                                    l.TrafficLight.State = true;
                                }
                                else
                                {
                                    l.TrafficLight.State = false;
                                }
                            }
                        }
                        c.Turn++;
                        break;
                    case 3:
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                if (l.Direction == Direction.SOUTH)
                                {
                                    l.TrafficLight.State = true;
                                }
                                else
                                {
                                    l.TrafficLight.State = false;
                                }
                            }
                        }
                        c.Turn++;
                        break;
                    case 4:
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                if (l.Direction == Direction.WEST)
                                {
                                    l.TrafficLight.State = true;
                                }
                                else
                                {
                                    l.TrafficLight.State = false;
                                }
                            }
                        }
                        c.Turn = 1;
                        break;
                }
                #endregion
            }
            else
            {
                #region B Crossing


                Crossing_B cb = (Crossing_B)c;
                switch (c.Turn)
                {
                    case 1:
                        cb.ATimer.Interval = 1000 * cb.Time.Seconds;
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                if (l.Direction == Direction.NORTH || l.Direction == Direction.SOUTH)
                                {
                                    l.TrafficLight.State = true;
                                }
                                else
                                {
                                    l.TrafficLight.State = false;
                                }
                            }
                        }
                        foreach (PedestrianLane pl in cb.pLanes)
                        {
                            if (pl.PLight != null)
                            {
                                pl.PLight.State = false;
                            }
                        }
                        c.Turn++;
                        break;
                    case 2:
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                if (l.Direction == Direction.EAST)
                                {
                                    l.TrafficLight.State = true;
                                }
                                else
                                {
                                    l.TrafficLight.State = false;
                                }
                            }
                        }
                        c.Turn++;
                        break;
                    case 3:
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                if (l.Direction == Direction.WEST)
                                {
                                    l.TrafficLight.State = true;
                                }
                                else
                                {
                                    l.TrafficLight.State = false;
                                }
                            }
                        }
                        bool sensor = false;
                        cb.CreatePedestrians();
                        foreach (PedestrianLane pl in cb.pLanes)
                        {
                            if (pl.PLight.Sensor)
                            {
                                sensor = true;
                            }
                        }
                        if (sensor)
                        {
                            c.Turn++;
                        }
                        else
                        {
                            c.Turn = 1;
                        }
                        break;
                    case 4:
                        if (cb.style == "Quiet")
                        {
                            cb.ATimer.Interval = (int)(1000* (double)cb.Time.Seconds * 0.25);
                        }
                        else
                        {
                            cb.ATimer.Interval = (int)(1000* (double)cb.Time.Seconds * 0.75);
                        }
                        foreach (PedestrianLane pl in cb.pLanes)
                        {
                            if (pl.PLight != null)
                            {
                                pl.PLight.State = true;
                            }
                        }
                        foreach (TrafficLane l in c.Lanes)
                        {
                            if (l.TrafficLight != null)
                            {
                                l.TrafficLight.State = false;
                            }
                        }
                        c.Turn = 1;
                        break;

                }
                #endregion
            }
        }

        /// <summary>
        /// runs the thread
        /// </summary>
        private void StartT()
        {
            foreach (Crossing i in crossings)
            {
                i.ATimer.Start();
            }
        }

        /// <summary>
        /// the tick event for the timer thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="c"></param>
        private void TickEvent(object sender, EventArgs args, Crossing c)
        {
            SwitchAll(c);
        }

        #endregion

    }
}
