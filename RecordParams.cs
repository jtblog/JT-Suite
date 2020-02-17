using AForge.Video.VFW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JT_Suite
{
    public class RecordParams
    {
        public static RecordParams mInstance;

        public Bitmap RecordFrame
        {
            get;
            set;
        }
        public List<forzoom> Z
        {
            get;
            set;
        }
        public CheckState State
        {
            get;
            set;
        }
        public int Width
        {
            get
            {
                if (RecordFrame != null)
                {
                    return this.RecordFrame.Width;
                }
                else {
                    return 0;
                }
            }
            set
            {

            }
        }
        public int Height
        {
            get {
                if (RecordFrame != null)
                {
                    return this.RecordFrame.Height;
                }
                else
                {
                    return 0;
                } 
            }
            set {

            }
        }
        public AVIWriter aviWriter
        {
            get;
            set;
        }
        public static RecordParams getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new RecordParams();
            }
            return mInstance;
        }
    }
    public class forzoom {

        public static forzoom mInstance;

        public Bitmap M_bmp
        {
            get;
            set;
        }
        public int Index
        {
            get;
            set;
        }
        public static forzoom getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new forzoom();
            }
            return mInstance;
        }
    }
}
