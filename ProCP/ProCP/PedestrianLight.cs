﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCP
{
    class PedestrianLight : Light
    {
        //Fields
        bool sensor=false;

        public bool Sensor
        {
            get { return sensor; }
            set { sensor = value; }
        }

        public PedestrianLight(bool state, bool sensor)
            : base(state)
        {
            this.Sensor = sensor;
        }

        public void sensorPressed()
        {
            sensor = true;
        }


    }
}
