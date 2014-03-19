using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using OculusRift.Oculus;
using Microsoft.Xna.Framework;

namespace Ocucam
{
    public class Send
    {
        bool _shouldStop = false; 

        Vector3 oculusAngles = Helpers.ToEulerAngles(OculusClient.GetPredictedOrientation());
        SerialPort port = new SerialPort("COM1", 9600, Parity.None);
        
        
        public void sendOrientation()
        {
            while (!_shouldStop)
            {
                //Get and store angles
                float angleX = oculusAngles.X;
                float angleY = oculusAngles.Y;
                float angleZ = oculusAngles.Z;

                //Format angle string before sending it
                string orientationData = String.Format(angleX + "|" + angleY + "|" + angleZ);

                //Convert the string into a char array (max size 14)
                char[] orientationArrayBuffer = orientationData.ToCharArray();

                //If the port isn't open, 
                if (!port.IsOpen)
                {
                    //Open it and send the chars one by one from 0 to 14
                    port.Open();
                    port.Write(orientationArrayBuffer, 0, 14);
                }
                else
                {
                    //Send the chars one by one from 0 to 14
                    port.Write(orientationArrayBuffer, 0, 14);
                }
                //Sleep 10ms to allow the servos to catch up
                Thread.Sleep(10);            
            }
            
        }

        public void RequestStop()
        {
            _shouldStop = true;
            port.Close();
        }
    }
}
