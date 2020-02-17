using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Suite
{
    public class PoseParams
    {

        public static PoseParams mInstance;
        public int x;
        public int y;
        public int px;
        public int py;

        public void Initiallize() {
            if (C_Width > 0 && C_Height > 0)
            {
                C_Image = new Bitmap(C_Width, C_Height);
                C_graphics = Graphics.FromImage(C_Image);

                x = 0; 
                px = 0;
                y = C_Height - 20;
                py = C_Height;
            }
            else {
            
            }
        }

        public Bitmap AnimatedImage() {
            C_graphics.Clear(Color.White);

            int k = C_Width / 2;
            int l = C_Height;

            if (x == 0)
            {
                if (y < py) {
                    y = y - 20;
                    if (y < 0)
                    {
                        x = 20;
                        py = 0;
                        y = 0;
                    }
                } else if(y > py){
                    y = y + 20;
                }
            }
            if (y == 0)
            {
                if (x > px) {
                    x = x + 20;
                    if (x > C_Width)
                    {
                        py = 0;
                        y = 20;
                        x = C_Width;
                    }
                }
                else if (x < px) {
                    x = x - 20;
                    if (x < 0)
                    {
                        py = 0;
                        y = 20;
                        x = 0;
                    }
                }
            }
            if (x == C_Width)
            {
                if (y > py)
                {
                    y = y + 20;
                }
                else if(y < py) {
                    y = y - 20;
                    if (y < 0)
                    {
                        px = C_Width;
                        x = C_Width - 20;
                        y = 0;
                    }
                }
            }

            if (y > C_Height)
            {
                y = C_Height - 20;
                py = C_Height;
                if (x < px)
                {
                    px = 0;
                    x = 0;
                }
                else
                {
                    x = C_Width;
                }
            }

            C_graphics.DrawEllipse(new Pen(Color.Gray, 1), new Rectangle(0, C_Width / 2, C_Width, C_Height));
            C_graphics.DrawLine(new Pen(Color.Gray, 1), x, y, k, l);
            C_graphics.DrawLine(new Pen(Color.Gray, 1), 0, 0, k, l);
            C_graphics.DrawLine(new Pen(Color.Gray, 1), C_Width, 0, k, l);
            return C_Image;
        }

        public Graphics C_graphics
        {
            get;
            set;
        }

        public Bitmap C_Image
        {
            get;
            set;
        }

        public int C_Height
        {
            get;
            //Void "set"
            set;
        }

        public int C_Width
        {
            get;
            //Void "set"
            set;
        }

        public static PoseParams getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new PoseParams();
            }
            return mInstance;
        }
    }
}
