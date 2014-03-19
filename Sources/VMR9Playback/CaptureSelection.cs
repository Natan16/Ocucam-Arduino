using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Management;
using DirectShow;
using Sonic;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using OculusRift.Oculus;

namespace VMR9Playback
{
    public partial class CaptureSelection : Form
    {
        public string leftEyeDevicePath;
        public string rightEyeDevicePath;
        public string displayDeviceInstance;
        public int displayId;
        public bool fullScreen;

        private class DSDeviceWrap
        {
            public DSDevice dev;
            public int idx;
            public DSDeviceWrap(DSDevice d, int i) { dev = d; idx = i; }
            public override string ToString() { return dev.ToString() + " (" + idx.ToString() + ")"; }
        };

        [DllImport("user32.dll")]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        const int EDD_GET_DEVICE_INTERFACE_NAME = 1;

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public struct DISPLAY_DEVICE 
        {
              [MarshalAs(UnmanagedType.U4)]
              public int cb;
              [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
              public string DeviceName;
              [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
              public string DeviceString;
              [MarshalAs(UnmanagedType.U4)]
              public DisplayDeviceStateFlags StateFlags;
              [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
              public string DeviceID;
              [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
              public string DeviceKey;
        }

        public class Display
        {
            public Display(int i, string n, string inst) { id = i; name = n; instance = inst; }
            public int id;
            public string name;
            public string instance;
            public override string ToString() { return name; }
        }

        public CaptureSelection()
        {
            InitializeComponent();

            //Oculus code
            if (OculusClient.isHMDPresent() == true)
            {
                detectedLabel.Text = "Rift Detected; Data should be valid.";
                //Scan oculus angles. I store them in a Vector3 with 3 values for each axis.
                //It's a quaternion, so I convert it to Euler angles (X, Y, Z)
                Vector3 oculusAngles = Helpers.ToEulerAngles(OculusClient.GetPredictedOrientation());

                //Format the float angles and send them to the labels
                XLabel.Text = String.Format("{0:0,0.0000000}", oculusAngles.X);
                YLabel.Text = String.Format("{0:0,0.0000000}", oculusAngles.Y);
                ZLabel.Text = String.Format("{0:0,0.0000000}", oculusAngles.Z);

                //Scan Oculus's resolution and format it
                Vector2 oculusResolution = OculusClient.GetScreenResolution();
                resolutionLabel.Text = String.Format(oculusResolution.X + "x" + oculusResolution.Y);
            }
            else
            {
                detectedLabel.Text = "No Rift Detected; Data is invalid.";
            }

            //Scott's code
            var screens = System.Windows.Forms.Screen.AllScreens;

            var mapInstanceToUserFriendly = new Dictionary<string, string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM WmiMonitorID");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string instanceName = (string)queryObj["InstanceName"];
                    string userFriendlyName = "";

                    if ((instanceName != null) && (queryObj["UserFriendlyName"] != null))
                    {
                        UInt16[] arrUserFriendlyName = (UInt16[])(queryObj["UserFriendlyName"]);
                        foreach (UInt16 arrValue in arrUserFriendlyName)
                        {
                            if (arrValue == 0) break;
                            userFriendlyName += Convert.ToChar(arrValue);
                        }

                        instanceName = instanceName.Replace('\\', '#');
                        string[] splitInstance = instanceName.Split('_');
                        if ((splitInstance != null) && (splitInstance.Length >= 1))
                        {
                            instanceName = splitInstance[0];

                            mapInstanceToUserFriendly[instanceName] = userFriendlyName;
                        }
                    }
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show("An error occurred while querying for WMI data: " + e.Message + ".  Exiting.", "Ocucam - Error");
                Environment.Exit(-1);
            }


            var mapIdsToDisplay = new Dictionary<int, Display>();
            DISPLAY_DEVICE d = new DISPLAY_DEVICE();
            d.cb = Marshal.SizeOf(d);
            try
            {
                for (int id = 0; EnumDisplayDevices(null, (uint)id, ref d, 0); id++)
                {
                    string name = d.DeviceName;
                    d.cb = Marshal.SizeOf(d);
                    EnumDisplayDevices(name, 0, ref d, EDD_GET_DEVICE_INTERFACE_NAME);

                    foreach (string instanceName in mapInstanceToUserFriendly.Keys)
                    {
                        if (d.DeviceID.Contains(instanceName))
                        {
                            mapIdsToDisplay[id] = new Display(id, mapInstanceToUserFriendly[instanceName], instanceName);
                            break;
                        }
                    }

                    d.cb = Marshal.SizeOf(d);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error enumerating display devices.  Exiting.", "Ocucam - Error");
                Environment.Exit(-1);
            }

            DSVideoCaptureCategory captureCategory = new DSVideoCaptureCategory();

            if (captureCategory.Objects.Count < 2)
            {
                if (captureCategory.Objects.Count == 0)
                {
                    MessageBox.Show("System must have at least two distinct video capture devices.  None were found.  Exiting.", "Ocucam - Error");
                }
                else
                {
                    MessageBox.Show("System must have at least two distinct video capture devices.  Only one was found.  Exiting.", "Ocucam - Error");
                }
                Environment.Exit(-1);
            }

            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey scottCutlerKey = softwareKey.CreateSubKey("Scott Cutler");
            RegistryKey ocucamKey = scottCutlerKey.CreateSubKey("Ocucam");
            string leftEyeDevicePathReg = (string)ocucamKey.GetValue("leftEyeDevicePath", "");
            string rightEyeDevicePathReg = (string)ocucamKey.GetValue("rightEyeDevicePath", "");
            string displayDeviceInstanceReg = (string)ocucamKey.GetValue("displayDeviceInstance", "");
            checkBoxFullScreen.Checked = ((string)ocucamKey.GetValue("fullScreen", "True") == "True") ? true : false;
            ocucamKey.Close();
            scottCutlerKey.Close();
            softwareKey.Close();

            captureCategory.Objects.Sort((a, b) => { return a.DevicePath.CompareTo(b.DevicePath); });

            var objectCount = new Dictionary<string, int>();

            foreach (var captureDevice in captureCategory.Objects)
            {
                if (objectCount.ContainsKey(captureDevice.DevicePath))
                {
                    objectCount[captureDevice.DevicePath]++;
                }
                else
                {
                    objectCount.Add(captureDevice.DevicePath, 1);
                }

                DSDeviceWrap dw = new DSDeviceWrap(captureDevice, objectCount[captureDevice.DevicePath]);

                leftEyeDevice.Items.Add(dw);
                if (leftEyeDevicePathReg == captureDevice.DevicePath)
                {
                    leftEyeDevice.SelectedIndex = leftEyeDevice.Items.Count - 1;
                }

                rightEyeDevice.Items.Add(dw);
                if (rightEyeDevicePathReg == captureDevice.DevicePath)
                {
                    rightEyeDevice.SelectedIndex = rightEyeDevice.Items.Count - 1;
                }

            }

            foreach (var displayId in mapIdsToDisplay.Keys)
            {
                comboBoxDisplay.Items.Add(mapIdsToDisplay[displayId]);

                if (displayDeviceInstanceReg == mapIdsToDisplay[displayId].instance)
                {
                    comboBoxDisplay.SelectedIndex = comboBoxDisplay.Items.Count - 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DSDevice leftEyeDeviceSelected = (leftEyeDevice.SelectedItem == null) ? null : ((DSDeviceWrap)leftEyeDevice.SelectedItem).dev;
            DSDevice rightEyeDeviceSelected = (rightEyeDevice.SelectedItem == null) ? null : ((DSDeviceWrap)rightEyeDevice.SelectedItem).dev;
            leftEyeDevicePath  = (leftEyeDeviceSelected  != null) ? leftEyeDeviceSelected.DevicePath : "";
            rightEyeDevicePath = (rightEyeDeviceSelected != null) ? rightEyeDeviceSelected.DevicePath : "";
            fullScreen = checkBoxFullScreen.Checked;

            Display display = (Display)comboBoxDisplay.SelectedItem;
            displayDeviceInstance = (display == null) ? "" : display.instance; 
            displayId = (display == null) ? 1000000000 : display.id;

            if (displayDeviceInstance == "")
            {
                MessageBox.Show("Display device must be selected.  Please make a selection in the dropdown.", "Ocucam - Error");
            } 
            else if (leftEyeDevicePath == "")
            {
                MessageBox.Show("Left eye device must be selected.  Please make a selection in the dropdown.", "Ocucam - Error");
            }
            else if (rightEyeDevicePath == "")
            {
                MessageBox.Show("Right eye device must be selected.  Please make a selection in the dropdown.", "Ocucam - Error");
            }
            else if (leftEyeDevicePath == rightEyeDevicePath)
            {
                MessageBox.Show("Left and right eyes must be set to different devices.  Please change your selections.", "Ocucam - Error");
            }
            else
            {
                RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software", true);
                RegistryKey scottCutlerKey = softwareKey.CreateSubKey("Scott Cutler");
                RegistryKey ocucamKey = scottCutlerKey.CreateSubKey("Ocucam");
                ocucamKey.SetValue("leftEyeDevicePath",  leftEyeDevicePath);
                ocucamKey.SetValue("rightEyeDevicePath", rightEyeDevicePath);
                ocucamKey.SetValue("displayDeviceInstance", displayDeviceInstance);
                ocucamKey.SetValue("fullScreen", fullScreen ? "True" : "False");
                ocucamKey.Close();
                scottCutlerKey.Close();
                softwareKey.Close();

                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
