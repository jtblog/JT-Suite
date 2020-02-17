using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JT_Suite
{
    public class SharedObjects
    {
        public static String mUsername;
        public static String mPassword;
        public int mSelectedIndex;
        public String m_Training_Action;
        public List<String> m_cam_names;
        public string m_Cur_SocialSite;
        public List<FilterInfo> m_devices;
        public static SharedObjects mInstance;

        public SharedObjects()
        {
        }
        public static SharedObjects getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new SharedObjects();
            }
            return mInstance;
        }
        public void setUsername(String sUsername)
        {
            mUsername = sUsername;
        }
        public String getUsername()
        {
            return mUsername;
        }
        public void setPassword(String sPassword)
        {
            mPassword = sPassword;
        }
        public String getPassword()
        {
            return mPassword;
        }
        public void setselectedIndex(int sSelectedIndex)
        {
            mSelectedIndex = sSelectedIndex;
        }
        public int getselectedIndex()
        {
            return mSelectedIndex;
        }
        public void set_Training_Action(String s_Training_Action)
        {
            m_Training_Action = s_Training_Action;
        }
        public String get_Training_Action()
        {
            return m_Training_Action;
        }
        public void set_Cur_SocialSite(String s_Cur_SocialSite)
        {
            m_Cur_SocialSite = s_Cur_SocialSite;
        }
        public String get_Cur_SocialSite()
        {
            return m_Cur_SocialSite;
        }

        public void set_Cam_Devices(List<FilterInfo> s_devices)
        {
            m_devices = s_devices;
        }
        public List<FilterInfo> get_Cam_Devices()
        {
            return m_devices;
        }
    }
}
