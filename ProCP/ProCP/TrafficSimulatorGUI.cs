﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ProCP
{
    public partial class TrafficSimulatorGUI : Form
    {
        private HelpGUI hGUI;
        private AboutGUI aGUI;
        bool debug;
        bool cardebug;
        bool eraseFlag = false;

        public static int count = 0;

        /// <summary>
        /// A list of controls
        /// </summary>
        List<Control> ControlList = new List<Control>();
        PictureBox selectedPicBox = null;
        List<PictureBox> crossPic = new List<PictureBox>();
        List<Crossing> tempCrossing = new List<Crossing>();

        //Crossing CurrentCrossing; //Never used
        Simulation Simulation;

        int selectedID;
        bool isLocked;
        bool play;
        bool crossLocked;

        public TrafficSimulatorGUI()
        {
            InitializeComponent();
            GetAllPictureboxes(panel1);
            AddDragDropToPictureBoxes();
            Simulation = new Simulation();
            isLocked = false;
           // btnPlay.Enabled = false;
            btnToggleLight.Enabled = false;
        }

        /// <summary>
        /// a method to populate the list of controls with pictureboxes
        /// </summary>
        /// <param name="container"></param>
        private void GetAllPictureboxes(Control container)
        {
            foreach (Control c in container.Controls)
            {
                GetAllPictureboxes(c);
                if (c is PictureBox) ControlList.Add(c);
            }
        }

        private int GetNumberOfPicturebox(PictureBox self)
        {
            switch (self.Name)
            {
                case "crossingGrid1": return 1;
                case "crossingGrid2": return 2;
                case "crossingGrid3": return 3;
                case "crossingGrid4": return 4;
                case "crossingGrid5": return 5;
                case "crossingGrid6": return 6;
                case "crossingGrid7": return 7;
                case "crossingGrid8": return 8;
                case "crossingGrid9": return 9;
                case "crossingGrid10": return 10;
                case "crossingGrid11": return 11;
                case "crossingGrid12": return 12;
                case "crossingGrid13": return 13;
                case "crossingGrid14": return 14;
                case "crossingGrid15": return 15;
                case "crossingGrid16": return 16;
                default: return 0;
            }
        }

        /// <summary>
        /// Adds the drag/drop events and makes the pictureboxes allowing drop
        /// </summary>
        private void AddDragDropToPictureBoxes()
        {
            crossingType1.MouseDown += crossingType1_MouseDown;
            crossingType2.MouseDown += crossingType2_MouseDown;

            foreach (Control c in ControlList)
            {
                c.AllowDrop = true;
                c.DragDrop += c_DragDrop;
                c.DragEnter += c_DragEnter;
            }
        }

        void c_DragEnter(object sender, DragEventArgs e)
        {
            if (isLocked)
            {
                return;
            }

            e.Effect = DragDropEffects.Copy;
        }

        void c_DragDrop(object sender, DragEventArgs e)
        {
            if (isLocked)
            {
                return;
            }

            PictureBox self = (PictureBox)sender;

            if (self.Image == null)
            {
                crossPic.Add(self);
                bool result = false;
                int picBoxNumber = GetNumberOfPicturebox(self);

                Image finalImage;

                if ((Image)e.Data.GetData(DataFormats.Bitmap) == crossingType1.Image)
                {
                    result = Simulation.AddCrossing(new Crossing_A(picBoxNumber));
                    finalImage = ProCP.Properties.Resources.Crossing_a;
                }
                else
                {
                    result = Simulation.AddCrossing(new Crossing_B(picBoxNumber));
                    finalImage = ProCP.Properties.Resources.Crossing_b;

                }

                if (result)
                    self.Image = (Image)e.Data.GetData(DataFormats.Bitmap);

                return;
            }

            MessageBox.Show("Remove the crossing first to be able to add another one on this tile", "There is already a crossing there");
        }

        void crossingType2_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(crossingType2.Image, DragDropEffects.Copy);
        }

        void crossingType1_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(crossingType1.Image, DragDropEffects.Copy);
        }

        private void togglePictureBoxSelection(PictureBox pBox)
        {
            PictureBox tempPicBox = selectedPicBox;

            if (selectedPicBox != null)
                unselectCurrentCrossing();

            if (tempPicBox != null && tempPicBox.Equals(pBox))
                return;

            selectedPicBox = pBox;

            if (isLocked)
            {
                EnableEditing();
            }
            else
            {
                btnRemove.Enabled = true;
            }
            //enableNum();

            pBox.Refresh();
        }

        private void unselectCurrentCrossing()
        {
            if (selectedPicBox == null)
            {
                return;
            }
            PictureBox selectedBefore = selectedPicBox;

            selectedPicBox = null;

            crossLocked = false;
            eraseFlag = true;

            //enableNum();//check this out
            DisableEditing();
            btnRemove.Enabled = false;

            selectedBefore.Refresh();
        }

        private void pictureBoxOnClick(object sender, EventArgs e)
        {
            if (play)
            {
                return;
            }
            int x = 0;
            int y = 0;
            string z = "";

            PictureBox self = (PictureBox)sender;
            togglePictureBoxSelection(self);

            selectedID = GetNumberOfPicturebox(self);

            if (!Simulation.CrossingExist(selectedID))
            {
                selectedID = 0;
                unselectCurrentCrossing();
                MessageBox.Show("No crossing selected.");
                return;
            }
            if (self.Image == crossingType1.Image)
            {
                Simulation.getProperties(selectedID, ref x, ref y, ref z);
                numericCars.Value = x;
                numericTrafficTime.Value = y;
                //Just placeholder for value for now and it resets it.
                cBPedTraffic.Enabled = false;
                //cBPedTraffic.SelectedItem = z1;
                //cBPedTraffic.SelectedText = z1;
            }
            else
            {
                Simulation.getProperties(selectedID, ref x, ref y, ref z);
                numericCars.Value = x;
                numericTrafficTime.Value = y;

                cBPedTraffic.Enabled = true;

                if (z == "" || z == null)
                {
                    cBPedTraffic.SelectedIndex = -1;
                }
                cBPedTraffic.SelectedItem = z;
                //cBPedTraffic.SelectedValue = z;
                //cBPedTraffic.SelectedIndex = cBPedTraffic.FindStringExact(z1);
            }
            if (Simulation.Surrounded(selectedID))
            {
                numericCars.Enabled = false;
            }
            else
            {
                numericCars.Enabled = true;
            }
            return;
        }

        private void btnFinishCrossing_Click(object sender, EventArgs e)
        {
            if (crossLocked)
            {
                CrossUnlock();
            }
            else
            {
                CrossLock();
            }

            //Simulation.EditCrossing(selectedID, 3, 5, (string)cBPedTraffic.SelectedItem);
            Simulation.EditCrossing(selectedID, (int)numericCars.Value, (int)numericTrafficTime.Value, (string)cBPedTraffic.SelectedItem);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (null == selectedPicBox)
            {
                MessageBox.Show(
                    "Please, select a crossing before removing it.",
                    "No crossing selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            if (null == selectedPicBox.Image)
            {
                MessageBox.Show(
                    "Please, add a crossing to this space before removing it.",
                    "There is no crossing in this tile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            PictureBox picBox = selectedPicBox;

            picBox.Image = null;
            Simulation.RemoveCrossing(GetNumberOfPicturebox(picBox));
            unselectCurrentCrossing();
        }

        private void btLock_Click(object sender, EventArgs e)
        {
            if (!isLocked)
            {
                Lock();

            }
            else if (isLocked)
            {
                Unlock();
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!play)
            {
                btnToggleLight.Enabled = true;
                TimerSimulation.Interval = 1000;
                TimerSimulation.Start();

                Play();
                count = Simulation.TotalNumberCars;
                Simulation.StartTimers();

            }
            else if (play)
            {
                Stop();
            }
        }

        #region buttonlogic
        private void Play()
        {
            play = true;
            this.unselectCurrentCrossing();
            btnPlay.Text = "STOP SIMULATION";
            btnToggleLight.Enabled = true;
            //enableNum();
            //this.DisableEditing();
            Simulation.Start();
            this.unselectCurrentCrossing();
        }

        private void Stop()
        {
            Simulation.Stop();
            TimerSimulation.Stop();
            play = false;
            btnPlay.Text = "PLAY SIMULATION";
            btnToggleLight.Enabled = false;
            //this.EnableEditing();
            Unlock();

            foreach (Crossing item in Simulation.Crossings)
            {
                foreach (TrafficLane j in item.Lanes)
                {
                    j.Cars.Clear();
                }
            }

            foreach (PictureBox i in ControlList)
            {
                i.Invalidate();
            }
        }

        private void Lock()
        {
            isLocked = true;
            btnLock.Text = "Unlock Grid";

            //enableNum();
            
            if (selectedPicBox != null)
            {
                EnableEditing();
                btnRemove.Enabled = false;
            }

            if (Simulation.Crossings.Count != 0)
            {
                btnPlay.Enabled = true;

                Simulation.MarkLanes();
                Simulation.LaneCrossingConnection();
            }


        }

        private void CrossLock()
        {
            crossLocked = true;
            btnFinishCrossing.Text = "Unlock Crossing"; ;

            //enableNum();
        }

        private void CrossUnlock()
        {
            crossLocked = false;
            btnFinishCrossing.Text = "Lock Crossing";

            this.numericCars.Value = 0;
            this.numericTrafficTime.Value = 5;
            this.cBPedTraffic.SelectedIndex = -1;

            btnPlay.Enabled = false;

           // enableNum();
        }

        private void Unlock()
        {
            isLocked = false;
            btnLock.Text = "Lock Grid";

            DisableEditing();

            if (selectedPicBox != null)
            {
                btnRemove.Enabled = true;
            }

            btnPlay.Enabled = false;

            //enableNum();
        }

        //public void enableNum()
        //{
        //    if (isLocked && !eraseFlag)
        //    {
        //        if (surrounded)
        //        {
        //            numericCars.Enabled = false;
        //        }
        //        if (!surrounded)
        //        {
        //            numericCars.Enabled = true;
        //        }

        //        this.numericTrafficTime.Enabled = true;
        //        //this.cBPedTraffic.Enabled = true;
        //        this.btnFinishCrossing.Enabled = true;
        //        this.btnRemove.Enabled = false;

        //    }

        //    if (isLocked && eraseFlag)
        //    {
        //        this.numericCars.Enabled = false;
        //        this.numericTrafficTime.Enabled = false;
        //        this.cBPedTraffic.Enabled = false;
        //        this.btnFinishCrossing.Enabled = false;
        //        this.btnRemove.Enabled = false;
        //        selectedID = 0;
        //    }

        //    if (!eraseFlag && !isLocked)
        //    {
        //        this.numericCars.Enabled = false;
        //        this.numericTrafficTime.Enabled = false;
        //        this.cBPedTraffic.Enabled = false;
        //        this.btnFinishCrossing.Enabled = false;
        //        this.btnRemove.Enabled = true;
        //    }

        //    if (!isLocked && eraseFlag)
        //    {
        //        this.numericCars.Enabled = false;
        //        this.numericTrafficTime.Enabled = false;
        //        this.cBPedTraffic.Enabled = false;
        //        this.btnFinishCrossing.Enabled = false;
        //        this.btnRemove.Enabled = false;
        //        selectedID = 0;
        //    }

        //    if (crossLocked)
        //    {
        //        this.numericCars.Enabled = false;
        //        this.numericTrafficTime.Enabled = false;
        //        this.cBPedTraffic.Enabled = false;
        //    }

        //    if (isLocked)
        //    {
        //        btnPlay.Enabled = true;
        //    }

        //    if (!play)
        //    {
        //        btnLock.Enabled = true;
        //        btnToggleLight.Enabled = false;
        //    }

        //    if (play)
        //    {
        //        btnLock.Enabled = false;
        //        btnToggleLight.Enabled = true;
        //    }

        //}

        #endregion

        private void pictureBoxOnPaint(object sender, PaintEventArgs e)
        {
            PictureBox self = (PictureBox)sender;

            Artist painter = new Artist(e);
            Crossing crossing = Simulation.getCrossing(GetNumberOfPicturebox(self));

            if (null != selectedPicBox)
            {
                if (self.Equals(selectedPicBox))
                {
                    painter.paintSelectionOutline();
                    crossLocked = false;
                    this.btnFinishCrossing.Text = "Lock Crossing";
                    //enableNum();
                }

                if (eraseFlag)
                {
                    eraseFlag = false;
                    this.btnFinishCrossing.Text = "Unlock Crossing";
                    //enableNum();

                    self.Invalidate();
                }
            }

            #region Drawing Stuff

            // Drawing cars and lights, yay
            List<Car> tempCars = new List<Car>();
            foreach (Crossing item in Simulation.Crossings)
            {
                if (!item.Equals(crossing))
                    continue;

                if (item is Crossing_B)
                {
                    foreach (PedestrianLane p in ((Crossing_B)item).pLanes)
                    {
                        if (p.PLight != null)
                        {
                            painter.drawPedestrianLight(p, p.PLight.State);

                            if (p.PLight.State) {
                                painter.drawPedestrians(p, ((Crossing_B)item).style);
                            }
                        }
                    }
                }

                foreach (TrafficLane j in item.Lanes)
                {
                    if (j.TrafficLight != null)
                        painter.drawTrafficLight(j, j.TrafficLight.State);

                    foreach (Car c in j.Cars)
                    {
                        painter.drawCar(c, j.Direction);
                    }
                }
            }

            #endregion

            #region Debug
            // enable/disable in the beginning of the class
            if (this.cBDebugPoint.Checked)
            {
                // Dots debug
                int picBoxNum = GetNumberOfPicturebox(self);

                bool test = Simulation.CrossingExist(picBoxNum);

                if (Simulation.CrossingExist(picBoxNum))
                {
                    crossing = Simulation.getCrossing(picBoxNum);
                    List<TrafficLane> lanes = new List<TrafficLane>(crossing.Lanes);

                    foreach (TrafficLane t in lanes)
                    {
                        foreach (Point p in t.Points)
                        {
                            e.Graphics.DrawEllipse(new Pen(Color.Red, 1), p.X, p.Y, 1, 1);
                        }
                    }


                    if (this.cBDebugCars.Checked)
                    {
                        if (crossing is Crossing_A)
                        {
                            // Car debug

                            #region Vertical Crossings
                            // Lane 4
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 77, 1, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 77, 16, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 77, 31, 10, 13);

                            // Lane 5
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 104, 1, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 104, 16, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 104, 31, 10, 13);

                            // Lane 0
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 134, 1, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 134, 16, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 134, 31, 10, 13);

                            // Lane 2
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 77, 111, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 77, 126, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 77, 141, 10, 13);

                            // Lane 9
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 107, 111, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 107, 126, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 107, 141, 10, 13);

                            // Lane 8
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 134, 111, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 134, 126, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 134, 141, 10, 13);

                            #endregion

                            #region Horizontal Crossings

                            // Lane 6
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 157, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 172, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 187, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 202, 54, 13, 10);

                            // Lane 7
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 157, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 172, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 187, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 202, 74, 13, 10);

                            // Lane 1
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 157, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 172, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 187, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 202, 94, 13, 10);

                            // Lane 3
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 5, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 20, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 35, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 50, 54, 13, 10);

                            // Lane 11
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 5, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 20, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 35, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 50, 74, 13, 10);

                            // Lane 10
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 5, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 20, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 35, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 50, 94, 13, 10);

                            #endregion
                        }

                        if (crossing is Crossing_B)
                        {
                            // Car debug

                            #region Vertical Crossings
                            // Lane 4
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 85, 1, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 85, 16, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 85, 31, 10, 13);

                            // Lane 0
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 128, 1, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 128, 16, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 128, 31, 10, 13);

                            // Lane 2
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 85, 113, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 85, 128, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 85, 143, 10, 13);

                            // Lane 7
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 128, 113, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 128, 128, 10, 13);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 128, 143, 10, 13);

                            #endregion

                            #region Horizontal Crossings

                            // Lane 6
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 157, 73, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 172, 73, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 187, 73, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 202, 73, 13, 10);

                            // Lane 8
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 5, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 20, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 35, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 50, 94, 13, 10);

                            // Lane 5
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 157, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 172, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 187, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 202, 54, 13, 10);

                            // Lane 9
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 5, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 20, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 35, 74, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 50, 74, 13, 10);

                            // Lane 1
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 157, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 172, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 187, 94, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 202, 94, 13, 10);

                            // Lane 3
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 1), 5, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 20, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 1), 35, 54, 13, 10);
                            e.Graphics.DrawRectangle(new Pen(Color.Orange, 1), 50, 54, 13, 10);

                            #endregion
                        }
                    }
                }
            }
            #endregion
        }

        private void btnToggleLight_Click(object sender, EventArgs e)
        {
            foreach (Crossing c in Simulation.Crossings)
            {
                Simulation.SwitchAll(c);
            }
        }

        private void fileToolStripMenuSave_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }

        public bool SaveToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML file|*.xml";
            saveFileDialog.Title = "Save a circuit file";
            saveFileDialog.InitialDirectory = @"c:\Libraries\Documents";
            saveFileDialog.OverwritePrompt = true;

            if (Simulation.Name == "")
            {
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }

                Simulation.Name = saveFileDialog.FileName;
                Simulation.SaveAs(saveFileDialog.FileName);
            }
            else
            {
                Simulation.SaveAs(Simulation.Name);
            }

            return true;
        }

        private void TrafficSimulatorGUI_Load(object sender, EventArgs e)
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Buttons
            toolTip1.SetToolTip(this.btnFinishCrossing, "Locks your settings for selected crossing.");
            toolTip1.SetToolTip(this.btnPlay, "Launches / Stops the simulation.");
            toolTip1.SetToolTip(this.btnRemove, "Removes a selected crossing.");
            toolTip1.SetToolTip(this.btnToggleLight, "Makes all of the lights switch to their next state.");
            toolTip1.SetToolTip(this.cBPedTraffic, "How many pedestrians per red light; Quiet, or Busy.");
            toolTip1.SetToolTip(this.numericCars, "Amount of cars that will spawn in selected crossing.");
            toolTip1.SetToolTip(this.numericTrafficTime, "Green time for all the lights in crossing.");
            toolTip1.SetToolTip(this.btnLock, "Locks the grid, allows crossings to be edited.");
            toolTip1.SetToolTip(this.crossingType1, "Crossing type A.\nA 3x3 crossing.\nNo Pedestrians.");
            toolTip1.SetToolTip(this.crossingType2, "Crossing type B.\nA 3x2 crossing.\nWith Pedestrians.");

        }

        private void newToolStripMenuNew_Click(object sender, EventArgs e)
        {
            //bool debug;
            //bool cardebug;
            debug = this.cBDebugPoint.Checked;
            cardebug = this.cBDebugCars.Checked;

            if (selectedPicBox != null)
            {
                unselectCurrentCrossing();
            }

            if (Simulation.Saved == true)
            {
                Clear();
            }
            else
            {
                DialogResult dResult = MessageBox.Show("Would you like to save your changes? Unsaved changes will be lost.", "New simulation", MessageBoxButtons.YesNoCancel);
                if (dResult == DialogResult.Yes)
                {
                    Simulation.SaveAs(Simulation.Name);
                    SaveToFile();

                    Clear();
                }
                else if (dResult == DialogResult.No)
                {
                    Clear();
                }
            }
        }

        public void Clear()
        {
            Unlock();
            //enableNum();
            Simulation = new Simulation();
            //Making a new instance of the circuit object
            ClearAll();
            this.Invalidate();
        }

        public void ClearAll()
        {
            foreach (PictureBox item in crossPic)
            {
                item.Image = null;
                item.InitialImage = null;
            }
        }

        private void exitToolStripMenuExit_Click(object sender, EventArgs e)
        {
            if (Simulation.Saved == false)
            {
                DialogResult dResult = MessageBox.Show("Would you like to save your changes? Unsaved changes will be lost.", "Close Application", MessageBoxButtons.YesNoCancel);
                if (dResult == DialogResult.Yes)
                {
                    SaveToFile();
                }
                else if (dResult == DialogResult.Cancel)
                {

                }
                else if (dResult == DialogResult.No)
                {
                    Simulation.Saved = true;
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private void TrafficSimulatorGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Simulation.Saved == false)
            {
                DialogResult dResult = MessageBox.Show("Would you like to save your changes? Unsaved changes will be lost.", "Close Application", MessageBoxButtons.YesNoCancel);
                if (dResult == DialogResult.Yes)
                {
                    SaveToFile();
                }
                else if (dResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CrossingType(Crossing crossing)
        {
            string sPattern = @"[0-9]+";

            foreach (PictureBox pic in ControlList)
            {
                Match m = Regex.Match(pic.Name, sPattern);
                int number = Convert.ToInt32(m.Value);
                if (crossing.CrossingId == number)
                {
                    if (crossing.GetType() == typeof(Crossing_A))
                    {
                        pic.Image = ProCP.Properties.Resources.Crossing_a;
                    }
                    else if (crossing.GetType() == typeof(Crossing_B))
                    {
                        pic.Image = ProCP.Properties.Resources.Crossing_b;
                    }
                }

            }

        }

        private void openToolStripMenuOpen_Click(object sender, EventArgs e)
        {
            debug = this.cBDebugPoint.Checked;
            cardebug = this.cBDebugCars.Checked;

            if (selectedPicBox != null)
            {
                unselectCurrentCrossing();
            }

            if (Simulation.Saved == true)
            {
                Clear();
            }
            else
            {
                DialogResult dResult = MessageBox.Show("Would you like to save your changes? Unsaved changes will be lost.", "New simulation", MessageBoxButtons.YesNoCancel);
                if (dResult == DialogResult.Yes)
                {
                    Simulation.SaveAs(Simulation.Name);
                    SaveToFile();

                    Clear();
                }
                else if (dResult == DialogResult.No)
                {
                    Clear();
                }
            }


            if (!GetFromFile())
            {
                MessageBox.Show("Error whilst loading file");
            }
            else
            {
                foreach (Crossing item in Simulation.Crossings)
                {
                    tempCrossing.Add(item);
                }
                Simulation.Crossings.Clear();

                foreach (Crossing cr in tempCrossing)
                {
                    if (cr.GetType() == typeof(Crossing_A))
                    {
                        Crossing_A cra = new Crossing_A(cr.CrossingId);
                        Simulation.AddCrossing(cra);
                        CrossingType(cra);
                    }
                    else
                    {
                        Crossing_B crb = new Crossing_B(cr.CrossingId);
                        Simulation.AddCrossing(crb);
                        CrossingType(crb);
                    }

                }

            }
        }

        private bool GetFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML file|*.xml";
            openFileDialog.Title = "Load a circuit file";
            openFileDialog.InitialDirectory = @"c:\Libraries\Documents";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            Simulation.Name = openFileDialog.FileName;
            Simulation.Load(openFileDialog.FileName);
            this.Invalidate();

            return true;
        }

        private void saveAsToolStripMenuSaveAs_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void viewHelpToolStripMenuVHelp_Click(object sender, EventArgs e)
        {
            hGUI = new HelpGUI();
            hGUI.Show();
        }

        private void aboutTrafficSimulatorToolStripMenuAboutTFS_Click(object sender, EventArgs e)
        {
            aGUI = new AboutGUI();
            aGUI.Show();
        }

        private void TimerSimulation_Tick(object sender, EventArgs e)
        {
            if (count > 0)
            {
                if (play)
                {

                    this.labelTime.Text = Simulation.Watch.Elapsed.Minutes + " Minutes " + Simulation.Watch.Elapsed.Seconds + " Seconds";
                    this.labelCarsAdded.Text = Simulation.TotalNumberCars.ToString();
                    this.labelCarsRemaining.Text = count.ToString();
                    this.labelLightSwitches.Text = Simulation.TotalNumberofSwitches.ToString();
                    this.labelCrossingNumbers.Text = Simulation.Crossings.Count.ToString();
                    foreach (Crossing c in Simulation.Crossings)
                    {
                        foreach (TrafficLane l in c.Lanes)
                        {
                            for (int i = 0; i < l.Cars.Count; i++)
                            {
                                GetAllPictureboxes(panel1);

                                Car car = l.Cars.ElementAt(i);

                                foreach (PictureBox p in ControlList)
                                {
                                    p.Invalidate();
                                }

                                car.DriveLane();

                                foreach (PictureBox p in ControlList)
                                {
                                    p.Invalidate();
                                }
                            }
                        }

                        //will need to be changed
                        if (c.GetType() == typeof(Crossing_B))
                        {
                            Crossing_B b = (Crossing_B)c;
                            for (int p = 0; p < b.GetNumberOfPedesToMove(); p++)
                            {
                                b.pedestrians[p].Walk();
                            }
                        }

                        //ped walk stuff
                    }
                }
            }
            else
            {
                this.labelCarsRemaining.Text = "0";
                this.Stop();
            }
        }

        private void DisableEditing()
        {
            gBSettings.Enabled = false;
        }


        private void EnableEditing()
        {
            gBSettings.Enabled = true;
        }

        //private void DisableEditing()
        //{
        //    numericCars.Enabled = false;
        //    numericTrafficTime.Enabled = false;
        //    cBPedTraffic.Enabled = false;
        //    btnFinishCrossing.Enabled = false;
        //    btnRemove.Enabled = false;
        //}

        //private void EnableEditing()
        //{
        //    numericCars.Enabled = true;
        //    numericTrafficTime.Enabled = true;
        //    cBPedTraffic.Enabled = true;
        //    btnFinishCrossing.Enabled = true;
        //    btnRemove.Enabled = true;
        //}
    }
}
