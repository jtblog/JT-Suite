using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace JT_Suite
{
    public partial class Form3 : Form
    {
        public static Form3 form3;
        public bool activated;
        public static ComboBox comboBox3 = Form1.mainform.comboBox1;
        public Timer t0 = new Timer();
        public Graphics graphics;

        public Form3()
        {
            InitializeComponent();
            form3 = this;
            activated = true;
            t0.Tick += new EventHandler(t0_Tick);

        }

        public void t0_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                if (tabControl1.TabPages[i].Controls[0] is PictureBox)
                {
                    ((PictureBox)tabControl1.TabPages[i].Controls[0]).Image = PoseParams.getInstance().AnimatedImage();

                    Bitmap img = new Bitmap(((PictureBox)tabControl1.TabPages[i].Controls[0]).ClientSize.Width, ((PictureBox)tabControl1.TabPages[i].Controls[0]).ClientSize.Height);
                    graphics = Graphics.FromImage(img);

                    graphics.DrawImage(PoseParams.getInstance().AnimatedImage(), new Rectangle(0, 0, PoseParams.getInstance().AnimatedImage().Width, PoseParams.getInstance().AnimatedImage().Height));
                    graphics.DrawEllipse(new Pen(Color.Green, 4), new Rectangle(0, 0, 200, 200));
                    ((PictureBox)tabControl1.TabPages[0].Controls[0]).Image = img;
                }
            }
            
            //Bitmap b = (Bitmap)((PictureBox)tabControl1.TabPages[0].Controls[0]).Image;
            //graphics = Graphics.FromImage(b);

            //graphics.DrawEllipse(new Pen(Color.Green, 4), new Rectangle(0, 0, b.Width, b.Height));
            //((PictureBox)tabControl1.TabPages[0].Controls[0]).Image = b;
        }

        public void Form3_Load(object sender, EventArgs e)
        {
            if (Form2.getInstance() != null)
            {
                if (Form2.getInstance().Form2_Opened())
                {
                    //Form2.form2.refreshToolStripMenuItem.Enabled = false;
                }
                else
                {
                    //Form2 is not opened
                }
            }
            tabControl1.TabPages.Clear();
            for (int i = 0; i < SharedObjects.getInstance().get_Cam_Devices().Count; i++)
            {
                tabControl1.Controls.Add(new TabPage() {
                    Name = SharedObjects.getInstance().get_Cam_Devices()[i].Name,
                    Text = SharedObjects.getInstance().get_Cam_Devices()[i].Name, 
                });
            }

            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages[i].Controls.Add(new System.Windows.Forms.PictureBox()
                {
                    Name = "PictureBox" + i,
                    Dock = DockStyle.Fill,
                    Location = new Point(0, 0)
                });
            }

            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                if (tabControl1.TabPages[i].Controls[0] is PictureBox)
                    ((PictureBox)tabControl1.TabPages[i].Controls[0]).Paint += new PaintEventHandler(Form3_Paint);
            }
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                if (tabControl1.TabPages[i].Controls[0] is PictureBox)

                    PoseParams.getInstance().C_Width = ((PictureBox)tabControl1.TabPages[i].Controls[0]).ClientSize.Width;
                    PoseParams.getInstance().C_Height = ((PictureBox)tabControl1.TabPages[i].Controls[0]).ClientSize.Height;
                    PoseParams.getInstance().Initiallize();
            }

                tabControl1.SelectTab(SharedObjects.getInstance().getselectedIndex());
                t0.Start();
        }

        public void Form3_Paint(object sender, PaintEventArgs e)
        {
            
            //e.Graphics.Clear(Color.Black);
            //e.Graphics.Dispose();
        }

        public bool Form3_Opened()
        {
            return activated;
        }
        public static Form3 getInstance()
        {
            if (form3 == null)
            {
                form3 = new Form3();
            }
            return form3;
        }
        public void Form3_FormClosing(object sender, FormClosingEventArgs fceargs)
        {
            fceargs.Cancel = true;
            this.Hide();
        }
        public void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            SharedObjects.getInstance().setselectedIndex(tabControl1.SelectedIndex);
        }
    }
}
