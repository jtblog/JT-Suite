using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using Emgu.CV;
using Emgu.CV.Structure;
using Encog.ML.Data.Image;
using Encog.Util.DownSample;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using java.util;
using Encog.ML.Data.Basic;
using Encog.Util.Simple;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.ML.Train.Strategy;
using Accord.Vision.Detection.Cascades;
using System.Runtime.ExceptionServices;
using System.Security;
using AForge.Math;
using AForge.Math.Geometry;

namespace JT_Suite
{
    public partial class Form2 : Form
    {
        public static ComboBox comboBox2 = Form1.mainform.comboBox1;
        public Emgu.CV.HaarCascade face, eye, nose;
        public bool activated;
        public static Form2 form2;
        public MotionDetector d = new MotionDetector(new TwoFramesDifferenceDetector(), new BlobCountingObjectsProcessing());
        public RectanglesMarker fmarker, nmarker;
        public Image<Gray, byte> rFace = null;
        public List<Image<Gray, byte>> rImages = new List<Image<Gray, byte>>();
        public List<string> rLabels = new List<string>();
        public int maxIteration, numberoffaces, t;
        public List<MotionDetector> detectors = new List<MotionDetector>();
        public IMotionProcessing IMP;
        public MCvTermCriteria termCrit;
        public List<Capture> _cap;
        public Image<Gray, byte> img;
        public BackgroundWorker BW0 = new BackgroundWorker();
        public BackgroundWorker BW1 = new BackgroundWorker();
        public BackgroundWorker BW2 = new BackgroundWorker();
        public Capture grab;
        public HaarObjectDetector facedetector;
        public HaarObjectDetector nosedetector;
        public Accord.Vision.Detection.HaarCascade faceCascade;
        public Accord.Vision.Detection.HaarCascade noseCascade;
        public List<VideoCaptureDevice> vidsources1 = new List<VideoCaptureDevice>();
        public List<String> dMonikerStrings = new List<String>();
        public int counter;

        public RGBDownsample RDSample;
        public ImageMLDataSet training;
        int downsampleHeight = 16;
        int downsampleWidth = 16;
        
        public HashMap identity2neuron = new HashMap();
        public HashMap neuron2identity = new HashMap();
        public int outputCount;
        public Encog.Neural.Networks.BasicNetwork network;
        private List<ImagePair> imageList;
        public Rectangle[] fregions;
        public String AI = " ";
        public String identity;
        public EventHandlerList Form2_NewFrames;
        List<forzoom> Z4;

        public AForge.Point[] imagePoints = new AForge.Point[4];
        public Color[] pointsColors = new Color[4];
        public readonly AForge.Point emptyPoint = new AForge.Point(-30000, -30000);
        public Vector3[] modelPoints = new Vector3[4];
        public float focalLength = 160;
        // estimated transformation
        public Matrix3x3 rotationMatrix, bestRotationMatrix, alternateRotationMatrix;
        public Vector3 translationVector, bestTranslationVector, alternateTranslationVector;
        public Matrix4x4 poseMatrix;
        public float modelRadius;
        public List<BackgroundWorker> bgws = new List<BackgroundWorker>();

        public Form2()
        {
            imageList = new List<ImagePair>();
            InitializeComponent();
            form2 = this;
            activated = true;
        }

        public void Form2_Load(object sender, EventArgs e)
        {
            BW0.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW0_RunWorkerCompleted);
            BW0.DoWork += new DoWorkEventHandler(BW0_DoWork);
            BW0.WorkerReportsProgress = true; BW0.WorkerSupportsCancellation = true;

            BW1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW1_RunWorkerCompleted);
            BW1.DoWork += new DoWorkEventHandler(BW1_DoWork);
            BW1.WorkerReportsProgress = true; BW1.WorkerSupportsCancellation = true;

            BW2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW2_RunWorkerCompleted);
            BW2.DoWork += new DoWorkEventHandler(BW2_DoWork);
            BW2.WorkerReportsProgress = true; BW2.WorkerSupportsCancellation = true;



            Z4 = new List<forzoom>();

            if (!BW0.IsBusy)
            {
                BW0.RunWorkerAsync();
            }

            if (!BW1.IsBusy)
            {
                //BW1.RunWorkerAsync();
            }

        }

        public void BW2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        public void BW2_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        public void BW1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BW1.CancelAsync();
            AI = "Ready";
        }

        public void BW1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LoadFaces();

                ProcessCreateTraining();
            }
            catch (Exception ex) {
            }
        }

        public void BW0_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BW0.CancelAsync();
        }

        public void BW0_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.splitContainer1.Panel1.Invoke(new MethodInvoker(delegate
                {
                    //faceCascade = Accord.Vision.Detection.HaarCascade.FromXml(Application.StartupPath + @"\haarcascade_frontalface_default.xml");
                    Accord.Vision.Detection.HaarCascade cascade0 = new FaceHaarCascade();
                    Accord.Vision.Detection.HaarCascade cascade1 = new NoseHaarCascade();

                    facedetector = new HaarObjectDetector(cascade0, 5, ObjectDetectorSearchMode.Single, 1.2f, ObjectDetectorScalingMode.GreaterToSmaller);
                    nosedetector = new HaarObjectDetector(cascade1, 1, ObjectDetectorSearchMode.Single, 1.2f, ObjectDetectorScalingMode.GreaterToSmaller);

                    //face = new Emgu.CV.HaarCascade("haarcascade_frontalface_default.xml");

                    vidsources1.Clear(); dMonikerStrings.Clear(); detectors.Clear();
                    for (int i = 0; i < SharedObjects.getInstance().get_Cam_Devices().Count; i++)
                    {
                        vidsources1.Add(new VideoCaptureDevice(SharedObjects.getInstance().get_Cam_Devices().ElementAt(i).MonikerString) { });
                        dMonikerStrings.Add(SharedObjects.getInstance().get_Cam_Devices().ElementAt(i).MonikerString);
                        detectors.Add(new MotionDetector(new TwoFramesDifferenceDetector(), IMP));
                    }

                    splitContainer1.Panel1.Controls.Clear();

                    tableLayoutPanel1 = new TableLayoutPanel();
                    tableLayoutPanel1.Name = "tableLayoutPanel1";
                    tableLayoutPanel1.ColumnCount = vidsources1.Count;
                    tableLayoutPanel1.RowCount = 2;
                    tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
                    tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
                    tableLayoutPanel1.AutoSize = false;

                    for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
                    {
                        tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165));
                    }
                    for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                    {
                        tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125));
                    }

                    for (int i = 0; i < vidsources1.Count; i++)
                    {
                        tableLayoutPanel1.Controls.Add(new System.Windows.Forms.PictureBox()
                        {
                            Dock = DockStyle.Fill,
                            Size = new Size(160, 120),
                            Cursor = Cursors.Cross,
                            Capture = true
                        }, i, 0);
                        tableLayoutPanel1.Controls.Add(new System.Windows.Forms.PictureBox()
                        {
                            Dock = DockStyle.Fill,
                            Size = new Size(160, 120),
                            Cursor = Cursors.Cross,
                            Capture = true
                        }, i, 1);
                    }

                    tableLayoutPanel1.ColumnCount = tableLayoutPanel1.ColumnStyles.Count;
                    tableLayoutPanel1.RowCount = tableLayoutPanel1.RowStyles.Count;

                    for (int i = 0; i < vidsources1.Count; i++)
                    {
                        ((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(i, 0)).MouseClick
                            += new MouseEventHandler(PicBoxes1_MouseClick);
                        ((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(i, 1)).MouseClick
                            += new MouseEventHandler(PicBoxes2_MouseClick);
                        //((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(i, 0)).Paint += new PaintEventHandler(Form2_Paint);
                    }

                    Z4.Clear();
                    for (int i = 0; i < vidsources1.Count; i++)
                    {
                        vidsources1.ElementAt(i).Start();
                        vidsources1.ElementAt(i).NewFrame += new NewFrameEventHandler(Form2_NewFrame);
                        Z4.Add(new forzoom() { M_bmp = null, Index = 0 });
                    }

                    splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
                }));

                pointsColors[0] = Color.Yellow;
                pointsColors[1] = Color.Navy;
                pointsColors[2] = Color.Red;
                pointsColors[3] = Color.Green;

                modelPoints[0].X = -75;
                modelPoints[0].Y = 0;
                modelPoints[0].Z = 100;
                modelPoints[1].X = 75;
                modelPoints[1].Y = 0;
                modelPoints[1].Z = 100;
                modelPoints[2].X = 75;
                modelPoints[2].Y = 0;
                modelPoints[2].Z = -100;
                modelPoints[3].X = -75;
                modelPoints[3].Y = 0;
                modelPoints[3].Z = -100;
            }
            catch (Exception ex)
            {

            }
        }

        public void Form2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void Form2_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
            Sleep.PreventSleep();
            
            VideoCaptureDevice VCD = sender as VideoCaptureDevice;
            int CurrentVCDIndex = dMonikerStrings.IndexOf(VCD.Source.ToString());

                Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
                Bitmap bmp0 = (Bitmap)eventArgs.Frame.Clone();

                Image<Bgr, Byte> EmguImage = new Image<Bgr, Byte>(bmp).Resize(160, 120, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC).Flip(Emgu.CV.CvEnum.FLIP.HORIZONTAL);
                Image<Bgr, Byte> EmguImage0 = new Image<Bgr, Byte>(bmp0).Flip(Emgu.CV.CvEnum.FLIP.HORIZONTAL);

                Form4ZoomView(EmguImage0.ToBitmap(), VCD);

                Bitmap b0 = EmguImage.ToBitmap();
                Bitmap b1 = EmguImage.ToBitmap();

                ((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(CurrentVCDIndex, 0)).Image
                    = b0;
                ((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(CurrentVCDIndex, 1)).Image
                    = b1;

                Image<Gray, Byte> gray = EmguImage.Convert<Gray, Byte>();
                
                //Motion detection algoriithm starts
                SoundPlayer SP = new SoundPlayer(Application.StartupPath + @"\alarm.wav");
                if (detectors.ElementAt(CurrentVCDIndex).ProcessFrame(bmp) > 0.03)
                {
                    //Motion detected
                    //SP.Play();
                    this.label1.Invoke(new MethodInvoker(delegate
                    {
                        //label1.Text = CurrentVCDIndex.ToString();
                    }));
                }
                //Motion detection algorithm ends

                Rectangle[] face_rects = facedetector.ProcessFrame(b0);

                if (face_rects.Length > 0)
                {
                    RectanglesMarker f_marker = new RectanglesMarker(face_rects, Color.White);
                    fmarker = f_marker;
                    
                    Rectangle f_roi = gray.ROI;
                    gray.ROI = face_rects[0];

                    Rectangle[] nose_rect = nosedetector.ProcessFrame(EmguImage.GetSubRect(f_roi).ToBitmap());
                    RectanglesMarker n_marker = new RectanglesMarker(nose_rect, Color.White);

                    /*
                    if (((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(CurrentVCDIndex, 0)) != null)
                    {
                        ((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(CurrentVCDIndex, 0)).Image = f_marker.Apply(EmguImage.ToBitmap());
                    }
                     */

                    if (nose_rect.Length > 0)
                    {
                        if (f_roi.Contains(nose_rect[0]))
                        {
                            int p = f_marker.Rectangles.ElementAt(0).X;
                            int q = f_marker.Rectangles.ElementAt(0).Y;
                            int r = f_marker.Rectangles.ElementAt(0).Width;
                            int s = f_marker.Rectangles.ElementAt(0).Height;
                                
                            int x0 = Math.Max(0, Math.Min(p, EmguImage.Width - 1));
                            int y0 = Math.Max(0, Math.Min(q, EmguImage.Height - 1));
                            int x1 = Math.Max(0, Math.Min(p + r, EmguImage.Width - 1));
                            int y1 = Math.Max(0, Math.Min(q, EmguImage.Height - 1));
                            int x2 = Math.Max(0, Math.Min(p + r, EmguImage.Width - 1));
                            int y2 = Math.Max(0, Math.Min(q + s, EmguImage.Height - 1));
                            int x3 = Math.Max(0, Math.Min(p, EmguImage.Width - 1));
                            int y3 = Math.Max(0, Math.Min(q + s, EmguImage.Height - 1));

                            imagePoints[0] = new AForge.Point(x0 - EmguImage.Width / 2, EmguImage.Height / 2 - y0);
                            imagePoints[1] = new AForge.Point(x1 - EmguImage.Width / 2, EmguImage.Height / 2 - y1);
                            imagePoints[2] = new AForge.Point(x2 - EmguImage.Width / 2, EmguImage.Height / 2 - y2);
                            imagePoints[3] = new AForge.Point(x3 - EmguImage.Width / 2, EmguImage.Height / 2 - y3);

                            /*
                            Bitmap b = EmguImage.ToBitmap();
                            Graphics g = Graphics.FromImage(b);

                            int cx = 160 / 2;
                            int cy = 120 / 2;

                            for (int i = 0; i < 4; i++)
                            {
                                if (imagePoints[i] != emptyPoint)
                                {
                                    using (Brush brush = new SolidBrush(pointsColors[i]))
                                    {
                                        g.FillEllipse(brush, new Rectangle(
                                            (int)(cx + imagePoints[i].X - 3),
                                            (int)(cy - imagePoints[i].Y - 3),
                                            7, 7));
                                    }
                                }
                            }
                            ((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(CurrentVCDIndex, 0)).Image = b;
                            */

                            CoplanarPosit coposit = new CoplanarPosit(modelPoints, focalLength);
                            coposit.EstimatePose(imagePoints, out rotationMatrix, out translationVector);

                            bestRotationMatrix = coposit.BestEstimatedRotation;
                            bestTranslationVector = coposit.BestEstimatedTranslation;

                            alternateRotationMatrix = coposit.AlternateEstimatedRotation;
                            alternateTranslationVector = coposit.AlternateEstimatedTranslation;

                            poseMatrix = Matrix4x4.CreateTranslation(translationVector) * Matrix4x4.CreateFromRotation(rotationMatrix);
                            
                            this.label1.Invoke(new MethodInvoker(delegate
                            {
                                //String.Format("{0:0.####}", floatValue);
                                label1.Text = String.Format("{0:0.####}", poseMatrix.V23);
                            }));

                            if (bgws.Count < 1)
                            {
                                bgws.Add(new BGW(poseMatrix.V23, CurrentVCDIndex));
                            }
                            else {
                                bgws.RemoveAt(0);
                                bgws.Add(new BGW(poseMatrix.V23, CurrentVCDIndex));
                            }

                            bgws.ElementAt(0).RunWorkerAsync();

                        }
                    }
                    /*
                    if (fmarker != null)
                    {
                        try
                        {
                            img = new Image<Bgr, Byte>(EmguImage.ToBitmap()).Copy(fmarker.Rectangles.ElementAt(0)).Convert<Gray, byte>().Resize(
                                    100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                        }
                        catch (Exception ex)
                        {
                            //OpenCV does not allow the conversion of Image<Bgr, Byte> to Image<Gray, Byte> exception caught
                        }
                    }
                    */
                }

                /*
                    //Face detection algorithm starts
                    UnmanagedImage im = UnmanagedImage.FromManagedImage(bmp);

                    float xscale = bmp.Width / 160f;
                    float yscale = bmp.Height / 120f;

                    ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
                    UnmanagedImage downsample = resize.Apply(im);
                    fregions = nosedetector.ProcessFrame(downsample);
                    int numberofdetectedfaces = fregions.Length;
                    //Rectangle[] nregions = nosedetector.ProcessFrame(downsample);

                    if (numberofdetectedfaces > 0)
                    {
                        Rectangle face = fregions[0];
                        Rectangle window = new Rectangle((int)((face.X  + face.Width / 2f) * xscale) - 3,
                                                         (int)((face.Y  + face.Height / 2f) * yscale) - 4, 14, 14);
                        window.Inflate((int)(0.35f * face.Width * xscale),
                                       (int)(0.45f * face.Height * yscale));

                        fmarker = new RectanglesMarker(window);
                        fmarker.ApplyInPlace(im);
                        ((System.Windows.Forms.PictureBox)tableLayoutPanel1.GetControlFromPosition(CurrentVCDIndex, 0)).Image =
                            new Image<Bgr, Byte>(im.ToManagedImage()).Resize(160, 120, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC).ToBitmap();
                    }
                    //Face detection algorithm ends
                */
                
                
            /*
                if (AI == "Ready" && img != null)
                {
                    try
                    {
                        Image<Gray, Byte> img0 = img.Clone();
                        String name0 = RecognizeImage(img0.ToBitmap());

                        this.label1.Invoke(new MethodInvoker(delegate
                        {
                            label1.Text = numberofdetectedfaces.ToString() + "\n\n" + name0 + "\n ####";// +name1 + "\n ####";
                        }));
                    }
                    catch (Exception ex)
                    {
                        //Object in use exception caught
                    }
                }

                //Face recognition algorithm starts
                if (!(rImages.ToArray().Length == 0))
                {

                    RecognizeImage(img.ToBitmap());
                }
                else
                {
                    rImages.Add(new Image<Bgr, Byte>(bmp).Copy(fmarker.Rectangles.ElementAt(0)).Convert<Gray, byte>().Resize(
                    100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC));

                    //rLabels.Add("Customer " + t);
                    t = t + 1;

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

                    LoadFaces();
                }
                //Face recognition algorithm ends
            */


                }
                catch (Exception ex) {
                    //MessageBox.Show(ex.Message);
                }

        }

        public void Form4ZoomView(Bitmap bmp0, VideoCaptureDevice VCD) {

            ((forzoom)Z4.ElementAt(dMonikerStrings.IndexOf(VCD.Source.ToString()))).M_bmp = bmp0;
            ((forzoom)Z4.ElementAt(dMonikerStrings.IndexOf(VCD.Source.ToString()))).Index = dMonikerStrings.IndexOf(VCD.Source.ToString());

            RecordParams.getInstance().Z = Z4;

            //Big view on form4 starts
            /*
            if (Form4.getInstance() != null)
            {
                if (Form4.getInstance().Form4_Opened())
                {
                    if (Form4.form4.tabControl1.TabPages.Count == dMonikerStrings.Count && Form4.form4.tabControl1.TabPages[dMonikerStrings.IndexOf(VCD.Source.ToString())].Controls.Count != 0)
                    {
                        try
                        {
                            Form4.form4.Invoke(new MethodInvoker(delegate
                            {
                                Form4.form4.tabControl1.TabPages[dMonikerStrings.IndexOf(VCD.Source.ToString())].Invoke(new MethodInvoker(delegate
                                {
                                    try
                                    {
                                        ((System.Windows.Forms.PictureBox)Form4.form4.tabControl1.TabPages[dMonikerStrings.IndexOf(VCD.Source.ToString())].Controls[0]).Invoke(new MethodInvoker(delegate
                                        {
                                            ((System.Windows.Forms.PictureBox)Form4.form4.tabControl1.TabPages[dMonikerStrings.IndexOf(VCD.Source.ToString())].Controls[0])
                                            .Image = bmp0;
                                        }));

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }));
                            }));
                        }
                        catch (Exception excpt) {
                        
                        }
                    }
                }
                else
                {
                    //Form4 is not opened
                }
            }
             */
            //Big View on form 4 ends
        }

        public void LoadFaces()
        {
            rImages.Clear();
            rLabels.Clear();
            try
            {
                numberoffaces = Convert.ToInt16(File.ReadAllLines(Application.StartupPath + "/jTFaces/numberoffaces.txt").ElementAt(0));
                t = numberoffaces;
                maxIteration = numberoffaces;

                for (int i = 0; i < numberoffaces; i++)
                {
                    rImages.Add(new Image<Gray, byte>(Application.StartupPath + "/jTFaces/" + "Face" + i + ".bmp").Resize(
                            100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC));
                    rLabels.Add(File.ReadAllLines(Application.StartupPath + "/jTFaces/jTLabels.txt").ElementAt(i));
                }

                termCrit = new MCvTermCriteria(maxIteration, 0.001);
            }
            catch (Exception ex)
            {

            }

        }

        public void Form2_FormClosing(object sender, FormClosingEventArgs fceargs)
        {
            fceargs.Cancel = true;
            this.Hide();
        }

        public void PicBoxes1_MouseClick(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.PictureBox PicBox1 = sender as System.Windows.Forms.PictureBox;
            if (PicBox1 != null)
            {
                SharedObjects.getInstance().setselectedIndex(tableLayoutPanel1.GetPositionFromControl(PicBox1).Column);
            }

            if (Form3.getInstance() != null)
            {
                if (Form3.getInstance().Form3_Opened())
                {
                    Form3.getInstance().Form3_Load(sender, e);
                    Form3.getInstance().Show();
                    Form3.getInstance().BringToFront();
                }
                else
                {
                    Form3 form3 = new Form3();
                    form3.Form3_Load(sender, e);
                    form3.Show();
                }
            }
        }
        public void PicBoxes2_MouseClick(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.PictureBox PicBox2 = sender as System.Windows.Forms.PictureBox;
            if (PicBox2 != null)
            {
                SharedObjects.getInstance().setselectedIndex(tableLayoutPanel1.GetPositionFromControl(PicBox2).Column);
            }
            if (Form4.getInstance() != null)
            {
                if (Form4.getInstance().Form4_Opened())
                {
                    Form4.getInstance().Form4_Load(sender, e);
                    Form4.getInstance().BringToFront();
                    Form4.getInstance().Show();
                }
                else
                {
                    Form4 form4 = new Form4();
                    form4.Form4_Load(sender, e);
                    form4.BringToFront();
                    form4.Show();
                }
            }
        }

        public bool Form2_Opened()
        {
            return activated;
        }
        public void Form2_Shown(object sender, EventArgs e)
        {

        }
        public static Form2 getInstance()
        {
            if (form2 == null)
            {
                form2 = new Form2();
            }
            return form2;
        }

        public void button1_Click(object sender, EventArgs e)
        {

        }

        public void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void identifyFacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharedObjects.getInstance().set_Training_Action("IdentifyFace");
            if (Form5.getInstance() != null)
            {
                if (Form5.getInstance().Form5_Opened())
                {
                    Form5.getInstance().Form5_Load(sender, e);
                    Form5.getInstance().Show();
                    Form5.getInstance().BringToFront();
                }
                else
                {
                    Form5 form5 = new Form5();
                    form5.Form5_Load(sender, e);
                    form5.Show();
                }
            }
        }

        public void addFaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharedObjects.getInstance().set_Training_Action("AddFace");
            if (Form5.getInstance() != null)
            {
                if (Form5.getInstance().Form5_Opened())
                {
                    Form5.getInstance().Form5_Load(sender, e);
                    Form5.getInstance().Show();
                    Form5.getInstance().BringToFront();
                }
                else
                {
                    Form5 form5 = new Form5();
                    form5.Form5_Load(sender, e);
                    form5.Show();
                }
            }
        }

        public void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            SharedObjects.getInstance().setselectedIndex(0);
            for (int i = 0; i < vidsources1.Count; i++)
            {
                vidsources1.ElementAt(i).Stop();//.NewFrame -= new NewFrameEventHandler(Form2_NewFrame);
            }
            Form1.mainform.refreshToolStripMenuItem.PerformClick();
            Form1.mainform.button4.PerformClick();
            this.Form2_Load(sender, e);
            if (Form4.getInstance() != null)
            {
                if (Form4.getInstance().Form4_Opened())
                {
                    Form4.getInstance().Form4_Load(sender, e);
                }
                else
                {
                    //Form4 is not opened
                }
            }
            if (Form3.getInstance() != null)
            {
                if (Form3.getInstance().Form3_Opened())
                {
                    Form3.getInstance().Form3_Load(sender, e);
                }
                else
                {
                    //Form4 is not opened
                }
            }
        }

        public void ProcessCreateTraining()
        {
            try
            {
                RDSample = new RGBDownsample();

                training = new ImageMLDataSet(RDSample, false, 1, -1);

                ProcessInput();
            }
            catch (Exception ex) {
                
            }
        }

        public void ProcessInput()
        {
            try
            {
                imageList.Clear();
                for (int i = 0; i < rImages.Count; i++) {
                    imageList.Add(new ImagePair(rImages.ElementAt(i).ToBitmap(), assignIdentity(rLabels.ElementAt(i))));
                }

                ProcessNetwork();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Input E Image: " + ex.Message);
            }
        }

        public void ProcessNetwork()
        {
            try
            {
                foreach (ImagePair Pair in imageList)
                {
                    BasicMLData ideal = new BasicMLData(outputCount);
                    int idx = Pair.getIdentity();

                    for (int i = 0; i < this.outputCount; i++)
                    {
                        if (i == idx)
                        {
                            ideal.Data[i] = 1;
                        }
                        else
                        {
                            ideal.Data[i] = -1;
                        }
                    }

                    Bitmap img = (Bitmap)Pair.getImage();
                    ImageMLData data = new ImageMLData(img);

                    this.training.Add(data, ideal);
                    
                    // String strHidden1 = getArg("hidden1");
		    //String strHidden2 = getArg("hidden2");

		//training.Downsample(this.downsampleHeight, this.downsampleWidth);
        training.Downsample(16, 16);

		 //int hidden1 = int.Parse(strHidden1);
		 //int hidden2 = int.Parse(strHidden2);

		network = EncogUtility.SimpleFeedForward(this.training.InputSize, 100, 0, training.IdealSize, true);

        processTrain();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI Process Net: " + ex.Message);
            }
        }

        public void processTrain(){

		ResilientPropagation train = new ResilientPropagation(network, training);
        ResetStrategy RS = new ResetStrategy(0.25, 50);
        train.Strategies.Clear();
            train.AddStrategy(RS);

            //train.AddStrategy(new ResetStrategy(strategyError, strategyCycles));
            //try
            //{
                for (int i = 0; i < numberoffaces * 100; i++) {
                    train.Iteration();
                }
                
                train.Pause();
                train.FinishTraining();
            //}
            //catch (Exception ex) {
            
            //}
		
	}

        public String RecognizeImage(System.Drawing.Bitmap img) {
                ImageMLData input = new ImageMLData(img);
                input.Downsample(RDSample, false, this.downsampleHeight, this.downsampleWidth, 1, -1);
                int winner = this.network.Winner(input);
                identity = this.neuron2identity.get(winner).ToString();

            return identity;
        }

        public int assignIdentity(String identity)
        {

		if (this.identity2neuron.containsKey(identity.ToLower())) {
			return ((int)this.identity2neuron.get(identity.ToLower()));
		}

		int result = this.outputCount;
        this.identity2neuron.put(identity.ToLower(), result);
        this.neuron2identity.put(result, identity.ToLower());
		this.outputCount++;
		return result;
	}

        public void facebookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharedObjects.getInstance().set_Cur_SocialSite("Facebook");
            if (Form6.getInstance() != null)
            {
                if (Form6.getInstance().Form6_Opened())
                {
                    Form6.getInstance().Form6_Load(sender, e);
                    Form6.getInstance().Show();
                    Form6.getInstance().BringToFront();
                }
                else
                {
                    Form6 form6 = new Form6();
                    form6.Form6_Load(sender, e);
                    form6.Show();
                }
            }
        }

        public void twitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Development in progress");
            //SharedObjects.getInstance().set_Cur_SocialSite("Twitter");
            //if (Form6.getInstance() != null)
            //{
                //if (Form6.getInstance().Form6_Opened())
                //{
                    //Form6.getInstance().Form6_Load(sender, e);
                    //Form6.getInstance().Show();
                    //Form6.getInstance().BringToFront();
                //}
                //else
                //{
                    //Form6 form6 = new Form6();
                    //form6.Form6_Load(sender, e);
                    //form6.Show();
                //}
            //}
        }

        public void removeAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }

    public class BGW : BackgroundWorker {
        private float p;
        private int CurrentVCDIndex;

        public BGW(float p, int CurrentVCDIndex)
        {
            // TODO: Complete member initialization
            this.p = p;
            this.CurrentVCDIndex = CurrentVCDIndex;
            this.DoWork += new DoWorkEventHandler(BGW_DoWork);
        }

        public void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show(String.Format("{0:0.####}", p));
        }
        /*
        void OnDoWork() {
            MessageBox.Show(String.Format("{0:0.####}", p));
        }
        */
    }

    class ImagePair {
		public System.Drawing.Image img;
        public int identity;

        public ImagePair(System.Drawing.Image img, int identity)
        {
            this.img = img;
			this.identity = identity;
		}

        public System.Drawing.Image getImage()
        {
            return this.img;
		}

		public int getIdentity() {
			return this.identity;
		}
	}

    internal static class Sleep
    {
        public static void PreventSleep()
        {
            SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired | ExecutionState.EsDisplayRequired);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [FlagsAttribute]
        private enum ExecutionState :
            uint
        {
            EsAwaymodeRequired = 0x00000040, EsContinuous = 0x80000000,
            EsDisplayRequired = 0x00000002, EsSystemRequired = 0x00000001
        }
    }

}
