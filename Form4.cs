using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using AForge.Video.VFW;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JT_Suite
{
    public partial class Form4 : Form
    {
        public static Form4 form4;
        public static bool activated;
        public AVIWriter AW;
        public VideoWriter VW;
        public Timer t0 = new Timer();
        public Timer t1 = new Timer();

        public Form4()
        {
            InitializeComponent();
            form4 = this;
            activated = true;
        }

        public void Form4_Load(object sender, EventArgs e)
        {
            AW = new AVIWriter();
            AW.FrameRate = 25;

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
                tabControl1.Controls.Add(
                    new TabPage()
                    {
                        Name = SharedObjects.getInstance().get_Cam_Devices()[i].Name,
                        Text = SharedObjects.getInstance().get_Cam_Devices()[i].Name
                    });
            }


            for(int i=0; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages[i].Controls.Add(new System.Windows.Forms.PictureBox() {
                    Name = "PictureBox" + i,
                    Dock = DockStyle.Fill,
                    Location = new Point(0,0)
                });
            }

            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                if(tabControl1.TabPages[i].Controls[0] is PictureBox)
                    tabControl1.TabPages[i].Controls[0].MouseDoubleClick += new MouseEventHandler(Form4_MouseDoubleClick);
            }

            try
            {
                tabControl1.SelectTab(SharedObjects.getInstance().getselectedIndex());
            }
            catch (Exception ex) {
                tabControl1.SelectTab(0);
            }

            t0.Tick += new EventHandler(t0_Tick);
            t1.Tick += new EventHandler(t1_Tick);
            t0.Start();
        }

        public void t0_Tick(object sender, EventArgs e)
        {
            try
            {
                RecordParams.getInstance().State = recordToolStripMenuItem.CheckState;
                RecordParams.getInstance().RecordFrame = (Bitmap)
                    ((PictureBox)tabControl1.TabPages[
                        tabControl1.SelectedIndex].Controls[0]).Image;

                if (!RecordParams.getInstance().Z[tabControl1.SelectedIndex].M_bmp.Equals(null))
                    ((PictureBox)tabControl1.TabPages[
                            tabControl1.SelectedIndex].Controls[0]).Image = RecordParams.getInstance().Z[tabControl1.SelectedIndex].M_bmp;
            }
            catch (Exception ex) {
            
            }
        }

        public void t1_Tick(object sender, EventArgs e)
        {
            VW.WriteFrame((new Image<Bgr, Byte>(RecordParams.getInstance().RecordFrame)));
            //AW.AddFrame(RecordParams.getInstance().RecordFrame);
        }

        public void Form4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PictureBox PB = sender as PictureBox;
            String name = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + 
                DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();

            if (!Directory.Exists(Application.StartupPath + "/jTSnapshots")) {
                Directory.CreateDirectory(Application.StartupPath + "/jTSnapshots");
            }

            PB.Image.Save(Application.StartupPath + "/jTSnapshots/" + name + ".bmp");
            
            MessageBox.Show("Picture taken");
        }

        public static Form4 getInstance()
        {
            if (form4 == null)
            {
                form4 = new Form4();
            }
            return form4;
        }
        public bool Form4_Opened()
        {
            return activated;
        }
        public void Form4_FormClosing(object sender, FormClosingEventArgs fceargs)
        {
            t0.Stop();
            if (Form2.getInstance() != null)
            {
                if (Form2.getInstance().Form2_Opened())
                {
                    Form2.form2.refreshToolStripMenuItem.Enabled = true;
                }
                else
                {
                    //Form4 is not opened
                }
            }
            if (tabControl1.Controls.Count > 0) {
                foreach (TabPage tab in tabControl1.TabPages) {
                    tab.Controls.Clear();
                }
            }
            
            fceargs.Cancel = true;
            this.Hide();
        }
        public void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            SharedObjects.getInstance().setselectedIndex(tabControl1.SelectedIndex);
        }

        public void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void recordToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;

            MessageBox.Show(RecordParams.getInstance().Width.ToString() + "-" + RecordParams.getInstance().Height.ToString());
            /*
            if (tsmi.CheckState == CheckState.Checked)
            {
                //VFW.Open(Application.StartupPath + @"\jtRecordings\n.avi", 
                    //RecordParams.getInstance().Width, 
                    //RecordParams.getInstance().Height, 25, 
                    //VideoCodec.MPEG4);

                //AW.Codec = VideoCodec.MPEG4.ToString();

                VW = new VideoWriter(Application.StartupPath + @"\jtRecordings\n.avi", CvInvoke.CV_FOURCC('I', 'Y', 'U', 'V'), 25, 
                    RecordParams.getInstance().Width, 
                    RecordParams.getInstance().Height, true);

                //AW.Open(Application.StartupPath + @"\jtRecordings\n.avi",
                    //RecordParams.getInstance().Width,
                    //RecordParams.getInstance().Height);
                t1.Start();
                tsmi.Text = "Stop recording";
            }
            else {
                t1.Stop();
                //AW.Close();
                VW.Dispose();
                tsmi.Text = "Start recording";
            }
             */
        }
    }
}
 