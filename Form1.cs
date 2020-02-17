using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace JT_Suite
{
    public partial class Form1 : Form
    {
        public FilterInfoCollection Webcams1;
        public static Form1 mainform;
        public ToolStripMenuItem refreshToolStripMenuItem;
        public SoundPlayer SP;
        public static Form1 form1;
        public List<FilterInfo> devices = new List<FilterInfo>();


        public Form1()
        {
            InitializeComponent();
            mainform = this;
            textBox1.Text = "Joseph T. Obagbemisoye";
        }

        public void button1_Click(object sender, EventArgs e)
        {
            SharedObjects.getInstance().setUsername(textBox1.Text);
            SharedObjects.getInstance().setPassword(textBox2.Text);
            login();
        }
        public void login()
        {
            if (SharedObjects.getInstance().getUsername() == "Joseph T. Obagbemisoye" && SharedObjects.getInstance().getPassword() == "11052012")
            {

                label1.Hide(); label2.Hide(); label3.Hide(); textBox1.Hide(); textBox2.Hide();
                button1.Hide(); button2.Hide(); this.Text = "Welcome " + textBox1.Text;

                menuStrip1 = new MenuStrip();
                label4 = new Label();
                fileToolStripMenuItem = new ToolStripMenuItem();
                helpToolStripMenuItem = new ToolStripMenuItem();
                refreshToolStripMenuItem = new ToolStripMenuItem();
                exitToolStripMenuItem = new ToolStripMenuItem();
                aboutJTSuiteToolStripMenuItem = new ToolStripMenuItem();
                comboBox1 = new ComboBox();
                button3 = new Button();
                button4 = new Button();
                Webcams1 = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                panel1 = new Panel();
                panel2 = new Panel();
                //
                //label4
                //
                label4.Text = "© JT Suite 2015";
                label4.Dock = DockStyle.Bottom;
                this.label4.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.label4.ForeColor = System.Drawing.Color.Navy;
                //
                //comboBox1
                //
                comboBox1.Location = new System.Drawing.Point(5, 40);
                comboBox1.Name = "comboBox1";
                comboBox1.Size = new System.Drawing.Size(270, 18);
                comboBox1.Items.Clear();
                devices.Clear();
                foreach (FilterInfo device in Webcams1)
                {
                    devices.Add(device);
                    comboBox1.Items.Add(device.Name);
                }
                SharedObjects.getInstance().set_Cam_Devices(devices);
                //
                //button3
                //
                button3.BackColor = System.Drawing.Color.White;
                button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
                button3.Font = new System.Drawing.Font("Segoe UI", 9F);
                button3.Location = new System.Drawing.Point(5, 68);
                button3.Name = "button3";
                button3.Size = new System.Drawing.Size(120, 23);
                button3.Text = "Add Url";
                button3.Click += new System.EventHandler(this.button3_Click);
                button3.ForeColor = System.Drawing.Color.Navy;
                button3.MouseEnter += new System.EventHandler(form1_buttons_MouseEnter);
                button3.MouseLeave += new System.EventHandler(form1_buttons_MouseLeave);
                //
                //button4
                //
                button4.Location = new System.Drawing.Point(155, 68);
                button4.Name = "button4";
                button4.Size = new System.Drawing.Size(120, 23);
                button4.Text = "Show Cams";
                button4.Click += new System.EventHandler(this.button4_Click);
                button4.BackColor = System.Drawing.Color.White;
                button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
                button4.Font = new System.Drawing.Font("Segoe UI", 9F);
                button4.ForeColor = System.Drawing.Color.Navy;
                button4.MouseEnter += new System.EventHandler(form1_buttons_MouseEnter);
                button4.MouseLeave += new System.EventHandler(form1_buttons_MouseLeave);
                // 
                // menuStrip1
                // 
                menuStrip1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                fileToolStripMenuItem,
                helpToolStripMenuItem});
                menuStrip1.Location = new System.Drawing.Point(0, 0);
                menuStrip1.Name = "menuStrip1";
                menuStrip1.Size = new System.Drawing.Size(440, 24);
                menuStrip1.TabIndex = 0;
                menuStrip1.Text = "menuStrip1";
                // 
                // fileToolStripMenuItem
                // 
                fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    refreshToolStripMenuItem, exitToolStripMenuItem});
                fileToolStripMenuItem.Name = "fileToolStripMenuItem";
                fileToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
                fileToolStripMenuItem.Text = "File";
                // 
                // helpToolStripMenuItem
                // 
                helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                aboutJTSuiteToolStripMenuItem});
                helpToolStripMenuItem.Name = "helpToolStripMenuItem";
                helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
                helpToolStripMenuItem.Text = "Help";
                // 
                // refreshToolStripMenuItem
                // 
                refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
                refreshToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.F5)));
                refreshToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
                refreshToolStripMenuItem.Text = "Refresh";
                refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
                // 
                // exitToolStripMenuItem
                // 
                exitToolStripMenuItem.Name = "exitToolStripMenuItem";
                exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
                exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
                exitToolStripMenuItem.Text = "Exit";
                exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
                // 
                // aboutJTSuiteToolStripMenuItem
                // 
                aboutJTSuiteToolStripMenuItem.Name = "aboutJTSuiteToolStripMenuItem";
                aboutJTSuiteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
                aboutJTSuiteToolStripMenuItem.Text = "About JT Suite";
                aboutJTSuiteToolStripMenuItem.Click += new System.EventHandler(this.aboutJTSuiteToolStripMenuItem_Click);
                //
                //panel1
                //
                panel1.AutoScroll = true;
                panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                panel1.Dock = System.Windows.Forms.DockStyle.Fill;
                panel1.Location = new System.Drawing.Point(0, 0);
                panel1.Name = "panel1";
                panel1.Size = new System.Drawing.Size(280, 100);
                panel1.Controls.Add(menuStrip1);
                panel1.Controls.Add(comboBox1);
                panel1.Controls.Add(button3);
                panel1.Controls.Add(button4);
                //
                //panel2
                //
                panel2.AutoScroll = true;
                panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
                panel2.Name = "panel2";
                panel2.Controls.Add(label4);
                panel1.Size = new System.Drawing.Size(280, 100);


                Controls.Add(panel1);
                Controls.Add(panel2);
            }
            else
            {
                label3.Text = "Invalid Username or Password";
                button1.Visible = true; button2.Visible = true;
            }
        }
        public void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void button3_Click(object sender, EventArgs e)
        {

        }
        public void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                if (Form2.getInstance() != null)
                {
                    if (Form2.getInstance().Form2_Opened())
                    {
                        Form2.getInstance().Form2_Load(sender, e);
                        Form2.getInstance().Show();
                    }
                    else
                    {
                        Form2 form2 = new Form2();
                        form2.Form2_Load(sender, e);
                        form2.Show();
                    }
                }
            }
            else
            {
                button4.Text = "No Cam found";
            }
        }
        public void textBox1_Click(object sender, EventArgs e)
        {
            label3.Text = "";
        }
        public void textBox2_Click(object sender, EventArgs e)
        {
            label3.Text = "";
        }
        public static Form1 getInstance()
        {
            if (form1 == null)
            {
                form1 = new Form1();
            }
            return form1;
        }
        public void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            login();
        }
        public void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeComponent();
            textBox1.Text = "Joseph T. Obagbemisoye";
            button1.Visible = false; button2.Visible = false;
            button5.Visible = true;
            this.AcceptButton = button5;
        }
        public void aboutJTSuiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(" A Joseph T. Obagbemisoye's \t\t\n\n Phone: 08077651557 \n Tweets: @jtob91 \n Google Search: Joseph T. Obagbemisoye \n\n © JT Suite 2015");
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            
        }
        public void Form1_Closing(object sender, FormClosingEventArgs fceargs)
        {
            fceargs.Cancel = true;
            textBox2.Focus();
        }

        public void button5_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "Joseph T. Obagbemisoye" && textBox2.Text == "11052012")
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                try
                {
                    SP = new SoundPlayer(Application.StartupPath + @"\alarm.wav");
                    SP.PlayLooping();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                button5.Visible = false;
                login();
            }
        }

        public void form1_buttons_MouseEnter(object sender, System.EventArgs e)
        {
            (sender as Control).BackColor = System.Drawing.Color.Pink;
        }

        public void form1_buttons_MouseLeave(object sender, System.EventArgs e)
        {
            (sender as Control).BackColor = System.Drawing.Color.White;
        }
    }
}
