using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JT_Suite
{
    public partial class Form5 : Form
    {
        public bool activated;
        public static Form5 form5;
        public List<Image<Gray, byte>> rImages = Form2.getInstance().rImages;
        public List<String> rLabels = Form2.getInstance().rLabels;
        public Boolean emptied;
        public Image<Gray, byte> img = Form2.getInstance().img;
        public Form5()
        {
            InitializeComponent();
            form5 = this;
            activated = true;
        }

        public void Form5_Load(object sender, EventArgs e)
        {
            emptied = false;
            label2.Text = "Face(s)" + "             " + "Name(s)";
            panel2.Controls.Clear();

            if (SharedObjects.getInstance().get_Training_Action().Equals("IdentifyFace"))
            {
                Point loc = new Point(0, 0);
                for (int i = 0; i < rImages.Count; i++)
                {
                    panel2.Controls.Add(new PictureBox()
                    {
                        Image = rImages.ElementAt(i).ToBitmap(),
                        Size = new Size(100, 100),
                        Location = loc
                    });
                    loc = new Point(0, loc.Y + 105);
                }

                loc = new Point(loc.X + 105, 25);
                for (int i = 0; i < rImages.Count; i++)
                {
                    panel2.Controls.Add(new TextBox()
                    {
                        Text = rLabels.ElementAt(i),
                        Size = new Size(250, 30),
                        Location = loc
                    });
                    loc = new Point(loc.X, loc.Y + 100);
                }
            }
            else if (SharedObjects.getInstance().get_Training_Action().Equals("AddFace"))
            {
                if (!(img == null))
                {
                    Point loc = new Point(0, 0);
                    panel2.Controls.Add(new PictureBox()
                    {
                        Image = img.ToBitmap(),
                        Size = new Size(100, 100),
                        Location = loc
                    });
                    loc = new Point(loc.X + 105, 25);
                    panel2.Controls.Add(new TextBox()
                    {
                        Size = new Size(250, 30),
                        Location = loc
                    });
                }
                else
                {
                    //Image not detected
                    MessageBox.Show("No face detected");
                    this.Hide();
                }
            }
        }

        public static Form5 getInstance()
        {
            if (form5 == null)
            {
                form5 = new Form5();
            }
            return form5;
        }
        public bool Form5_Opened()
        {
            return activated;
        }
        public void Form5_FormClosing(object sender, FormClosingEventArgs fceargs)
        {
            fceargs.Cancel = true;
            this.Hide();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (panel2.Controls.Count > 0)
            {
                foreach (Control con in panel2.Controls)
                {
                    if (con is TextBox)
                    {
                        if (String.IsNullOrEmpty(con.Text))
                        {
                            emptied = true;
                        }
                    }
                }
            }
            if (panel2.Controls.Count > 0)
            {
                foreach (Control con in panel2.Controls)
                {
                    if (con is TextBox)
                    {
                        if (!(String.IsNullOrEmpty(con.Text)))
                        {
                            emptied = false;
                        }
                    }
                }
            }
            if (emptied.Equals(true))
            {
                MessageBox.Show("Please leave none of the name field empty");
            }
            else
            {
                rLabels.Clear();
                AddtoDB();
                this.Hide();
            }
        }

        private void AddtoDB()
        {
            if (SharedObjects.getInstance().get_Training_Action() == "IdentifyFace")
            {
                foreach (Control con in panel2.Controls)
                {
                    if (con.Location.X == 105)
                    {
                        rLabels.Add(((TextBox)con).Text);
                    }
                }

                try
                {
                    //Write the number of trained faces in a file text for further load
                    File.WriteAllText(Application.StartupPath + "/jTFaces/numberoffaces.txt", rImages.ToArray().Length.ToString());

                    //Write the labels of trained faces in a file text for further load
                    for (int i = 0; i < rImages.ToArray().Length; i++)
                    {
                        rImages.ToArray()[i].Save(Application.StartupPath + "/jTFaces/Face" + i + ".bmp");
                        File.AppendAllLines(Application.StartupPath + "/jTFaces/jTLabels.txt", rLabels.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Oops! Something went wrong. \n\nReason: " + ex.Message);
                    if (!Directory.Exists(Application.StartupPath + "/jTFaces"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "/jTFaces");
                    }
                }
            }
            else
            {
                rImages.Add(img);

                foreach (Control con in panel2.Controls)
                {
                    if (con.Location.X == 105)
                    {
                        rLabels.Add(((TextBox)con).Text);
                    }
                }

                try
                {
                    //Write the number of trained faces in a file text for further load
                    File.WriteAllText(Application.StartupPath + "/jTFaces/numberoffaces.txt", rImages.ToArray().Length.ToString());

                    //Write the labels of trained faces in a file text for further load
                    for (int i = 0; i < rImages.ToArray().Length; i++)
                    {
                        rImages.ToArray()[i].Save(Application.StartupPath + "/jTFaces/Face" + i + ".bmp");
                        File.AppendAllLines(Application.StartupPath + "/jTFaces/jTLabels.txt", rLabels.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Oops! Something went wrong. \n\nReason: " + ex.Message);
                    if (!Directory.Exists(Application.StartupPath + "/jTFaces"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "/jTFaces");
                    }
                }
            }
        }

    }
}
