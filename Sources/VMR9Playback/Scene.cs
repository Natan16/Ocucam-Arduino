using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using SlimDX.Direct3D9;
using SlimDX;
using Microsoft.Win32;

namespace VMR9Playback
{
    public class Scene: IDisposable 
    {
        #region Variables

        private object m_csLock = new object();
        private Surface m_RenderTarget = null;
        private Device m_Device = null;
        private Control m_Control = null;
        private Texture m_tex = null;
        private bool m_swap = false;
        private float m_pupilOffset = 0.0f;
        private float m_scale = 1.0f;
        private bool m_fullscreen = false;
        private int m_adapterId = 0;
        private SlimDX.Direct3D9.Font m_font = null;

        private Effect m_distortionEffect = null;

        #endregion

        #region Properties

        public Device Direct3DDevice
        {
            get { return m_Device; }
        }

        #endregion

        #region Constructor

        public Scene(Control _control, int adapterId, bool fullScreen)
            : this(_control, null, adapterId, fullScreen)
        {
            
        }

        public Scene(Control _control, Device _device, int adapterId, bool fullScreen)
        {
            m_Control = _control;
            m_Device = _device;
            m_fullscreen = fullScreen;
            m_adapterId = adapterId;
            if (m_Device == null)
            {
                Direct3DEx _d3d = new Direct3DEx();
                PresentParameters _parameters = new PresentParameters();
                _parameters.BackBufferWidth = 1280;// m_Control.Width;
                _parameters.BackBufferHeight = 800;// m_Control.Height;
                _parameters.BackBufferWidth = 1920;// m_Control.Width;
                _parameters.BackBufferHeight = 1080;// m_Control.Height;
                if (m_fullscreen)
                {
                    _parameters.BackBufferFormat = Format.X8R8G8B8;
                    _parameters.BackBufferCount = 1;
                    _parameters.Multisample = MultisampleType.None;
                    _parameters.SwapEffect = SwapEffect.Flip;
                    _parameters.PresentationInterval = PresentInterval.Default;
                    _parameters.Windowed = false;
                    _parameters.DeviceWindowHandle = m_Control.Parent.Handle;
                    _parameters.SwapEffect = SwapEffect.Flip;
                    _parameters.PresentationInterval = PresentInterval.Immediate;
                    _parameters.FullScreenRefreshRateInHertz = 0;
                    _parameters.PresentFlags = PresentFlags.None | PresentFlags.DiscardDepthStencil;
                    _parameters.AutoDepthStencilFormat = Format.D24X8;
                    _parameters.EnableAutoDepthStencil = true;
                    m_Device = new Device(_d3d, m_adapterId, DeviceType.Hardware, m_Control.Parent.Handle, CreateFlags.Multithreaded | CreateFlags.HardwareVertexProcessing, _parameters);
                }
                else
                {
                    DisplayMode _mode = _d3d.GetAdapterDisplayMode(0);
                    _parameters.BackBufferFormat = _mode.Format;
                    _parameters.BackBufferCount = 1;
                    _parameters.Multisample = MultisampleType.None;
                    _parameters.SwapEffect = SwapEffect.Discard;
                    _parameters.PresentationInterval = PresentInterval.Default;
                    _parameters.Windowed = true;
                    _parameters.DeviceWindowHandle = m_Control.Handle;
                    _parameters.PresentFlags = PresentFlags.DeviceClip | PresentFlags.Video;
                    m_Device = new DeviceEx(_d3d, m_adapterId, DeviceType.Hardware, m_Control.Handle, CreateFlags.Multithreaded | CreateFlags.HardwareVertexProcessing, _parameters);
                }
            }
            m_RenderTarget = m_Device.GetRenderTarget(0);

            m_tex = new Texture(m_Device, 1280, 800, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            m_font = new SlimDX.Direct3D9.Font(m_Device, new System.Drawing.Font(FontFamily.GenericSansSerif, 24.0f));

            m_distortionEffect = Effect.FromFile(m_Device, "OculusRift.fx", ShaderFlags.None);

            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey scottCutlerKey = softwareKey.CreateSubKey("Scott Cutler");
            RegistryKey ocucamKey = scottCutlerKey.CreateSubKey("Ocucam");
            //Begin fix code
            ocucamKey.SetValue("pupilOffset", m_pupilOffset);
            ocucamKey.SetValue("scale", m_scale);
            //End fix code
            m_pupilOffset = Convert.ToSingle((string)ocucamKey.GetValue("pupilOffset", m_pupilOffset));
            m_scale = Convert.ToSingle((string)ocucamKey.GetValue("scale", m_scale));
            ocucamKey.Close();
            scottCutlerKey.Close();
            softwareKey.Close();   
        }

        ~Scene()
        {
            Dispose();
        }

        public void Swap() 
        {
            lock (m_csLock)
            {
                m_swap = !m_swap;
            }
        }

        public void MovePupilLeft() { lock (m_csLock) { m_pupilOffset += 0.025f; }  }
        public void MovePupilRight() { lock (m_csLock) { m_pupilOffset -= 0.025f; }  }

        public void IncreaseScale() { lock (m_csLock) { m_scale *= 1.05f; } }
        public void DecreaseScale() { lock (m_csLock) { m_scale /= 1.05f; } }

        #endregion

        #region Scene Handling

        public void OnSurfaceReady(ref Surface surface)
        {
            lock (m_csLock)
            {
                try
                {
                    m_Device.SetRenderTarget(0, m_RenderTarget);
                    m_Device.Clear(ClearFlags.Target, Color.Blue, 1.0f, 0);
                    Surface _backbuffer = m_Device.GetBackBuffer(0, 0);


                    m_Device.BeginScene();

                    float w = _backbuffer.Description.Width;
                    float h = _backbuffer.Description.Height;

                    Surface tex = m_tex.GetSurfaceLevel(0);
                    m_Device.StretchRectangle(surface, tex, TextureFilter.Linear);
                    //m_Device.StretchRectangle(surface, _backbuffer, TextureFilter.Linear);

                    //m_Device.SetRenderTarget(0, tex);
                    //m_font.DrawString(null, "Hello", 200, 200, Color.White);
                    //m_font.DrawString(null, "Hello", 200+640, 200, Color.White);
                    tex.Dispose();

                    m_Device.SetRenderTarget(0, m_RenderTarget);

                    float a = 2.0f * h / w;

                    float aspectRatioValue = 720.0f / 1280.0f;

                    Vertex[] lverts = { new Vertex(0, 0, 1, 0.5f, -1, -a), new Vertex(w / 2, 0, 1, 0.5f, 1, -a), new Vertex(0, h, 1, 0.5f, -1, a), new Vertex(w / 2, h, 1, 0.5f, 1, a) };
                    Vertex[] rverts = { new Vertex(w / 2, 0, 1, 0.5f, -1, -a), new Vertex(w, 0, 1, 0.5f, 1, -a), new Vertex(w / 2, h, 1, 0.5f, -1, a), new Vertex(w, h, 1, 0.5f, 1, a) };

                    //Vertex[] lverts = { new Vertex(0, 0, 1, 0.5f, -1, -a), new Vertex(w / 2, 0, 1, 0.5f, 1, -a), new Vertex(0, h, 1, 0.5f, -1, a), new Vertex(w / 2, h, 1, 0.5f, 1, a) };
                    //Vertex[] rverts = { new Vertex(w / 2, 0, 1, 0.5f, -1, -a), new Vertex(w, 0, 1, 0.5f, 1, -a), new Vertex(w / 2, h, 1, 0.5f, -1, a), new Vertex(w, h, 1, 0.5f, 1, a) };

                    m_Device.VertexFormat = VertexFormat.PositionRhw | VertexFormat.Texture1;
                    m_Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
                    m_Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
                    m_Device.SetRenderState(RenderState.CullMode, Cull.None);
                    m_Device.SetRenderState(RenderState.ZFunc, Compare.Always);

                    m_Device.SetTexture(0, m_tex);
                    m_Device.SetSamplerState(0, SamplerState.BorderColor, 0x00000000);
                    m_Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Border);
                    m_Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Border);

                    EffectHandle hmdWarpParam = m_distortionEffect.GetParameter(null, "HmdWarpParam");
                    EffectHandle texCoordOffset = m_distortionEffect.GetParameter(null, "TexCoordOffset");
                    EffectHandle tcScaleMat = m_distortionEffect.GetParameter(null, "TCScaleMat");
                    EffectHandle aspectRatio = m_distortionEffect.GetParameter(null, "AspectRatio");
                    EffectHandle scale = m_distortionEffect.GetParameter(null, "Scale");
                    EffectHandle pupilOffset = m_distortionEffect.GetParameter(null, "PupilOffset");

                    for (int j = 0; j < 2; j++)
                    {
                        Vertex[] verts = (j == 0) ? lverts : rverts;
                        float texCoordOffsetValue = ((j == 0) ^ !!m_swap) ? 0.0f : 0.5f;
                        //Vector4 scaleMat = (j == 0) ? (new Vector4(0.0f, -1.0f, 1.0f, 0.0f)) : (new Vector4(0.0f, 1.0f, -1.0f, 0.0f));
                        Vector4 scaleMat = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                        float pupilOffsetValue = (j == 0) ? m_pupilOffset : -m_pupilOffset;

                        int numPasses = m_distortionEffect.Begin();
                        for (int i = 0; i < numPasses; i++)
                        {
                            m_distortionEffect.BeginPass(i);
                            m_distortionEffect.SetValue(hmdWarpParam, new Vector4(1.0f, 0.22f, 0.24f, 0.0f));
                            m_distortionEffect.SetValue(tcScaleMat, scaleMat);
                            m_distortionEffect.SetValue(texCoordOffset, new Vector2(texCoordOffsetValue, 0.0f));
                            m_distortionEffect.SetValue(scale, new Vector2(0.25f / m_scale, 0.5f / m_scale));
                            m_distortionEffect.SetValue(aspectRatio, aspectRatioValue);
                            m_distortionEffect.SetValue(pupilOffset, pupilOffsetValue);
                            m_Device.DrawUserPrimitives<Vertex>(PrimitiveType.TriangleStrip, 2, verts);
                            m_distortionEffect.EndPass();
                        }
                        m_distortionEffect.End();

                    }
                    
                    m_Device.EndScene();
                    m_Device.Present();
                }
                catch (Exception e)
                {
                    e = e;
                }
            }
        }

        public struct Vertex
        {
            public Vertex(float _x, float _y, float _z, float _rhw, float _u, float _v) { x = _x; y = _y; z = _z; rhw = _rhw; u = _u; v = _v; }
            float x, y, z, rhw;
            float u, v;
        };

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (m_csLock)
            {
                if (m_tex != null)
                {
                    m_tex.Dispose();
                    m_tex = null;
                }
                if (m_Device != null)
                {
                    m_Device.Dispose();
                    m_Device = null;
                }

                RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software", true);
                RegistryKey scottCutlerKey = softwareKey.CreateSubKey("Scott Cutler");
                RegistryKey ocucamKey = scottCutlerKey.CreateSubKey("Ocucam");
                ocucamKey.SetValue("pupilOffset", m_pupilOffset);
                ocucamKey.SetValue("scale", m_scale);
                ocucamKey.Close();
                scottCutlerKey.Close();
                softwareKey.Close();
            }
        }

        #endregion
    }
}