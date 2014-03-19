using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using SlimDX.Direct3D9;
using VMR9;
using Sonic;
using SlimDX;
using OculusRift.Oculus;
using Ocucam;

namespace VMR9Playback
{
    public partial class MainForm : Form
    {
        #region Variables

        private string m_leftEyeDevicePath;
        private string m_rightEyeDevicePath;

        private object m_csSceneLock = new object();

        private Scene m_Scene = null;

        //private DSFilePlayback m_Playback = null;
        private DSVideoCaptureVMR9 m_capture = null;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            //Code for sending orientation data
            //Initialize Send class
            Send oSend = new Send();

            //Create thread object for X
            Thread oThreadOrientation = new Thread(new ThreadStart(oSend.sendOrientation));
            oThreadOrientation.IsBackground = true;


            //Start thread
            oThreadOrientation.Start();


            //Wait until data has begun sending
            while (oThreadOrientation.IsAlive == false)
            {
                Thread.Sleep(10);
            }

            CaptureSelection captureSelectionDialog = new CaptureSelection();

            captureSelectionDialog.ShowDialog();
            m_leftEyeDevicePath = captureSelectionDialog.leftEyeDevicePath;
            m_rightEyeDevicePath = captureSelectionDialog.rightEyeDevicePath;

            lock (m_csSceneLock)
            {
                m_Scene = new Scene(this.pbView, captureSelectionDialog.displayId, captureSelectionDialog.fullScreen);
            }
        }

        #endregion

        #region Form Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {            

            m_capture = new DSVideoCaptureVMR9(m_Scene.Direct3DDevice, m_leftEyeDevicePath, m_rightEyeDevicePath);

            m_capture.OnSurfaceReady += new VMR9.SurfaceReadyHandler(m_Scene.OnSurfaceReady);

            m_capture.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_capture != null)
            {
                m_capture.Dispose();
                m_capture = null;
            }
            if (m_Scene != null)
            {
                m_Scene.Dispose();
                m_Scene = null;
            }
            Send send = new Send();
            send.RequestStop();
        }

        #endregion

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.S)
            {
                m_Scene.Swap();
            }
            else if (e.KeyData == Keys.PageUp)
            {
                m_Scene.MovePupilRight();
            }
            else if (e.KeyData == Keys.PageDown)
            {
                m_Scene.MovePupilLeft();
            }
            else if (e.KeyData == Keys.Home)
            {
                m_Scene.IncreaseScale();
            }
            else if (e.KeyData == Keys.End)
            {
                m_Scene.DecreaseScale();
            }
            else if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

    }
}
