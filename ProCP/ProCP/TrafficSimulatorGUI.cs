﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProCP
{
    public partial class TrafficSimulatorGUI : Form
    {
        /// <summary>
        /// A list of controls
        /// </summary>
        List<Control> ControlList = new List<Control>();
        PictureBox selectedPicBox = null;
        Crossing CurrentCrossing;
        Simulation Simulation;

        int selectedID;
        bool isLocked;
        bool play;

        public TrafficSimulatorGUI()
        {
            InitializeComponent();
            GetAllPictureboxes(panel1);
            AddDragDropToPictureBoxes();
            Simulation = new Simulation();
            isLocked = false;
            btnPlay.Enabled = false;
            btnRemove.Enabled = true;
            btnToggleLight.Enabled = false;
            btnFinishCrossing.Enabled = false;

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
                default:return 0;

            
            }
        }
        /// <summary>
        /// Adds the drag/drop events and makes the pictureboxes allowing drop
        /// </summary>
        private void AddDragDropToPictureBoxes()
        {
            crossingType1.MouseDown += crossingType1_MouseDown;
            crossingType2.MouseDown += crossingType2_MouseDown;
            foreach (Control c in ControlList )
            {
               
                c.AllowDrop = true;
                c.DragDrop += c_DragDrop;
                c.DragEnter += c_DragEnter;
            }
           
        }

        void c_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;                
        }

        void c_DragDrop(object sender, DragEventArgs e)
        {
            PictureBox self = (PictureBox)sender;
            if (self.Image == null)
            {
                self.Image = (Image)e.Data.GetData(DataFormats.Bitmap);
                if ((Image)e.Data.GetData(DataFormats.Bitmap) == crossingType1.Image)
              {
                   Crossing_A test1 = new Crossing_A(GetNumberOfPicturebox(self), new Point(self.Location.X, self.Location.Y));
                   Simulation.AddCrossing(test1);
                      
              }
              else {
                  Simulation.AddCrossing(new Crossing_B(GetNumberOfPicturebox(self),
                                                         new Point(self.Location.X, self.Location.Y)));
              }
            }
            else MessageBox.Show("Remove the crossing first to be able to add another one on this tile",
                "There is already a crossing there");
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
            if (null != selectedPicBox)
            {
                applyStylesToPicBox(selectedPicBox, false);
                selectedPicBox = null;
                applyStylesToPicBox(pBox, false);
            }

            selectedPicBox = pBox;
            applyStylesToPicBox(pBox, true);
        }

        private void applyStylesToPicBox(PictureBox pBox, Boolean selected)
        {
            if (selected)
            {
                // Styles... 
                return;
            }

            // Styles...
        }

        private void pictureBoxOnClick(object sender, EventArgs e)
        {
            int x = 1;
            int y = 2;
            int z = 3;

            PictureBox self = (PictureBox)sender;
            togglePictureBoxSelection(self);

            selectedID = GetNumberOfPicturebox(self);


            if (!Simulation.CrossingExist(selectedID))
            {
                selectedID = 0;
                MessageBox.Show("No crossing selected.");
                return;
            }

            if (!isLocked)
            {
                return;
            }

            if (self.Image == crossingType1.Image)
            {
                Simulation.getProperties(selectedID, ref x, ref y, ref z);
                numericCars.Value = x;
                numericTrafficTime.Value = y;
                numericPedestrians.Enabled = false;

            }
            else
            {
                Simulation.getProperties(selectedID, ref x, ref y, ref z);
                numericCars.Value = x;
                numericTrafficTime.Value = y;
                numericPedestrians.Value = z;
                numericPedestrians.Enabled = true;
            }


            return;
        }

        private void btnFinishCrossing_Click(object sender, EventArgs e)
        {
            Simulation.EditCrossing(selectedID, (int)numericCars.Value, (int)numericTrafficTime.Value, (int)numericPedestrians.Value);
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
                Play();
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
            btnPlay.Text = "STOP";
            btnLock.Enabled = false;
            btnRemove.Enabled = false;
            btnToggleLight.Enabled = false;
            btnFinishCrossing.Enabled = false;
            Simulation.CreateCars();
        }

        private void Stop()
        {
            play = false;
            btnPlay.Text = "PLAY";
            btnLock.Enabled = true;
            Unlock();
        }

        private void Lock()
        {
            isLocked = true;
            btnLock.Text = "Unlock Grid";
            btnPlay.Enabled = true;
            btnRemove.Enabled = false;
            btnToggleLight.Enabled = true;

            Simulation.MarkLanes();
            Simulation.LaneCrossingConnection();

            this.numericCars.Enabled = true;
            this.numericPedestrians.Enabled = true;
            this.numericTrafficTime.Enabled = true;
            btnFinishCrossing.Enabled = true;
        }

        private void Unlock()
        {
            isLocked = false;
            btnLock.Text = "Lock Grid";
            btnPlay.Enabled = false;
            btnRemove.Enabled = true;
            btnToggleLight.Enabled = false;

            this.numericCars.Enabled = false;
            this.numericPedestrians.Enabled = false;
            this.numericTrafficTime.Enabled = false;
            btnFinishCrossing.Enabled = false;
        }
        #endregion
    }
}
