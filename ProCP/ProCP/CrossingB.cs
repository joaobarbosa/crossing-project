﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProCP
{
    class Crossing_B:Crossing
    {

        int numPeds;
        List<TrafficLane> tempLanes;

        public int NumPeds
        {
            get { return numPeds; }
            set { numPeds = value; }
        }

        /// <summary>
        /// The crossing type B constructor
        /// </summary>
        public Crossing_B(int crossingId, Point position)
            : base(crossingId, position)
        {
            this.NumPeds = 0;

            List<TrafficLane> lanes = new List<TrafficLane>();
            List<TrafficLane> tLanes;
            tempLanes = new List<TrafficLane>();
            lanes = new List<TrafficLane>();

            tLanes = new List<TrafficLane>();

            for (int i = 0; i < 4; i++)
            {
                lanes.Add(new TrafficLane(i, false, (Direction)i, null, false, null, tempLanes));
                tempLanes = new List<TrafficLane>();
            }

            //Adding the list of lanes that a certain lane can go to, as well as creating the lanes.

            //For South
            tLanes.Add(lanes.ElementAt(2));
            lanes.Add(new TrafficLane(4, true, Direction.SOUTH, null, false, null, tLanes));
            tLanes = new List<TrafficLane>();

            //For West
            tLanes.AddRange(new TrafficLane[] { lanes.ElementAt(0), lanes.ElementAt(3) });
            lanes.Add(new TrafficLane(5, true, Direction.WEST, null, false, null, tLanes));
            tLanes = new List<TrafficLane>();

            tLanes.Add(lanes.ElementAt(2));
            lanes.Add(new TrafficLane(6, true, Direction.WEST, null, false, null, tLanes));
            tLanes = new List<TrafficLane>();

            //For North
            tLanes.Add(lanes.ElementAt(0));
            lanes.Add(new TrafficLane(7, true, Direction.NORTH, null, false, null, tLanes));
            tLanes = new List<TrafficLane>();

            //For East
            tLanes.AddRange(new TrafficLane[] { lanes.ElementAt(1), lanes.ElementAt(2) });
            lanes.Add(new TrafficLane(8, true, Direction.EAST, null, false, null, tLanes));
            tLanes = new List<TrafficLane>();

            tLanes.Add(lanes.ElementAt(0));
            lanes.Add(new TrafficLane(9, true, Direction.EAST, null, false, null, tLanes));
            tLanes = new List<TrafficLane>();

            base.Lanes.AddRange(lanes);
        }
    }
}