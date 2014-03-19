using System;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;
using System.Drawing;
using DirectShow;
using System.Collections.Generic;
using SlimDX;
using Sonic;
using System.Windows.Forms;

namespace VMR9
{
    #region Declarations

    /// <summary>
    /// From VMR_ASPECT_RATIO_MODE
    /// </summary>
    public enum VMRAspectRatioMode
    {
        None,
        LetterBox
    }

    /// <summary>
    /// From VMRMixerPrefs
    /// </summary>
    [Flags]
    public enum VMRMixerPrefs
    {
        None = 0,
        NoDecimation = 0x00000001,
        DecimateOutput = 0x00000002,
        ARAdjustXorY = 0x00000004,
        DecimationReserved = 0x00000008,
        DecimateMask = 0x0000000F,

        BiLinearFiltering = 0x00000010,
        PointFiltering = 0x00000020,
        FilteringMask = 0x000000F0,

        RenderTargetRGB = 0x00000100,
        RenderTargetYUV = 0x00001000,

        RenderTargetYUV420 = 0x00000200,
        RenderTargetYUV422 = 0x00000400,
        RenderTargetYUV444 = 0x00000800,
        RenderTargetReserved = 0x0000E000,
        RenderTargetMask = 0x0000FF00,

        DynamicSwitchToBOB = 0x00010000,
        DynamicDecimateBy2 = 0x00020000,

        DynamicReserved = 0x000C0000,
        DynamicMask = 0x000F0000
    }

    /// <summary>
    /// From VMR9PresentationFlags
    /// </summary>
    [Flags]
    public enum VMR9PresentationFlags
    {
        SyncPoint = 0x00000001,
        Preroll = 0x00000002,
        Discontinuity = 0x00000004,
        TimeValid = 0x00000008,
        SrcDstRectsValid = 0x00000010
    }

    /// <summary>
    /// From VMR9SurfaceAllocationFlags
    /// </summary>
    [Flags]
    public enum VMR9SurfaceAllocationFlags
    {
        ThreeDRenderTarget = 0x0001,
        DXVATarget = 0x0002,
        TextureSurface = 0x0004,
        OffscreenSurface = 0x0008,
        UsageReserved = 0x00F0,
        UsageMask = 0x00FF
    }

    /// <summary>
    /// From VMR9ProcAmpControlFlags
    /// </summary>
    [Flags]
    public enum VMR9ProcAmpControlFlags
    {
        Brightness = 0x00000001,
        Contrast = 0x00000002,
        Hue = 0x00000004,
        Saturation = 0x00000008,
        Mask = 0x0000000F
    }


    /// <summary>
    /// From VMR9MixerPrefs
    /// </summary>
    [Flags]
    public enum VMR9MixerPrefs
    {
        None = 0,
        NoDecimation = 0x00000001, // No decimation - full size
        DecimateOutput = 0x00000002, // decimate output by 2 in x & y
        ARAdjustXorY = 0x00000004, // adjust the aspect ratio in x or y
        NonSquareMixing = 0x00000008, // assume AP can handle non-square mixing, avoids intermediate scales
        DecimateMask = 0x0000000F,

        BiLinearFiltering = 0x00000010, // use bi-linear filtering
        PointFiltering = 0x00000020, // use point filtering
        AnisotropicFiltering = 0x00000040, //
        PyramidalQuadFiltering = 0x00000080, // 4-sample tent
        GaussianQuadFiltering = 0x00000100, // 4-sample gaussian
        FilteringReserved = 0x00000E00, // bits reserved for future use.
        FilteringMask = 0x00000FF0, // OR of all above flags

        RenderTargetRGB = 0x00001000,
        RenderTargetYUV = 0x00002000, // Uses DXVA to perform mixing
        RenderTargetReserved = 0x000FC000, // bits reserved for future use.
        RenderTargetMask = 0x000FF000, // OR of all above flags

        DynamicSwitchToBOB = 0x00100000,
        DynamicDecimateBy2 = 0x00200000,

        DynamicReserved = 0x00C00000,
        DynamicMask = 0x00F00000
    }

    /// <summary>
    /// From VMR9DeinterlaceTech
    /// </summary>
    [Flags]
    public enum VMR9DeinterlaceTech
    {
        Unknown = 0x0000,
        BOBLineReplicate = 0x0001,
        BOBVerticalStretch = 0x0002,
        MedianFiltering = 0x0004,
        EdgeFiltering = 0x0010,
        FieldAdaptive = 0x0020,
        PixelAdaptive = 0x0040,
        MotionVectorSteered = 0x0080
    }

    /// <summary>
    /// From VMR9AlphaBitmapFlags
    /// </summary>
    [Flags]
    public enum VMR9AlphaBitmapFlags
    {
        Disable = 0x00000001,
        hDC = 0x00000002,
        EntireDDS = 0x00000004,
        SrcColorKey = 0x00000008,
        SrcRect = 0x00000010,
        FilterMode = 0x00000020
    }

    /// <summary>
    /// From VMR9DeinterlacePrefs
    /// </summary>
    [Flags]
    public enum VMR9DeinterlacePrefs
    {
        NextBest = 0x01,
        BOB = 0x02,
        Weave = 0x04,
        Mask = 0x07
    }

    /// <summary>
    /// From VMR9RenderPrefs
    /// </summary>
    [Flags]
    public enum VMR9RenderPrefs
    {
        DoNotRenderBorder = 0x00000001, // app paints color keys
        Mask = 0x00000001, // OR of all above flags
    }

    /// <summary>
    /// From VMR9Mode
    /// </summary>
    [Flags]
    public enum VMR9Mode
    {
        Windowed = 0x00000001,
        Windowless = 0x00000002,
        Renderless = 0x00000004,
        Mask = 0x00000007
    }

    /// <summary>
    /// From VMR9AspectRatioMode
    /// </summary>
    public enum VMR9AspectRatioMode
    {
        None,
        LetterBox,
    }

    /// <summary>
    /// From VMR9_SampleFormat
    /// </summary>
    public enum VMR9SampleFormat
    {
        Reserved = 1,
        ProgressiveFrame = 2,
        FieldInterleavedEvenFirst = 3,
        FieldInterleavedOddFirst = 4,
        FieldSingleEven = 5,
        FieldSingleOdd = 6
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9NormalizedRect
    {
        public float left;
        public float top;
        public float right;
        public float bottom;

        public VMR9NormalizedRect(float l, float t, float r, float b)
        {
            this.left = l;
            this.top = t;
            this.right = r;
            this.bottom = b;
        }

        public VMR9NormalizedRect(RectangleF r)
        {
            this.left = r.Left;
            this.top = r.Top;
            this.right = r.Right;
            this.bottom = r.Bottom;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1} - {2}, {3}]", this.left, this.top, this.right, this.bottom);
        }

        public override int GetHashCode()
        {
            return this.left.GetHashCode() |
                this.top.GetHashCode() |
                this.right.GetHashCode() |
                this.bottom.GetHashCode();
        }

        public static implicit operator RectangleF(VMR9NormalizedRect r)
        {
            return r.ToRectangleF();
        }

        public static implicit operator VMR9NormalizedRect(Rectangle r)
        {
            return new VMR9NormalizedRect(r);
        }

        public static bool operator ==(VMR9NormalizedRect r1, VMR9NormalizedRect r2)
        {
            return ((r1.left == r2.left) && (r1.top == r2.top) && (r1.right == r2.right) && (r1.bottom == r2.bottom));
        }

        public static bool operator !=(VMR9NormalizedRect r1, VMR9NormalizedRect r2)
        {
            return ((r1.left != r2.left) || (r1.top != r2.top) || (r1.right != r2.right) || (r1.bottom != r2.bottom));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VMR9NormalizedRect))
                return false;

            VMR9NormalizedRect other = (VMR9NormalizedRect)obj;
            return (this == other);
        }


        public RectangleF ToRectangleF()
        {
            return new RectangleF(this.left, this.top, (this.right - this.left), (this.bottom - this.top));
        }

        public static VMR9NormalizedRect FromRectangle(RectangleF r)
        {
            return new VMR9NormalizedRect(r);
        }
    }

    /// <summary>
    /// From VMR9PresentationInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9PresentationInfo
    {
        public VMR9PresentationFlags dwFlags;
        public IntPtr lpSurf; //IDirect3DSurface9
        public long rtStart;
        public long rtEnd;
        public Size szAspectRatio;
        public DsRect rcSrc;
        public DsRect rcDst;
        public int dwReserved1;
        public int dwReserved2;
    }

    /// <summary>
    /// From VMR9AllocationInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9AllocationInfo
    {
        public VMR9SurfaceAllocationFlags dwFlags;
        public int dwWidth;
        public int dwHeight;
        public int Format; // D3DFORMAT
        public int Pool; // D3DPOOL
        public int MinBuffers;
        public Size szAspectRatio;
        public Size szNativeSize;
    }

    /// <summary>
    /// From VMR9ProcAmpControl
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9ProcAmpControl
    {
        public int dwSize; // should be 24
        public VMR9ProcAmpControlFlags dwFlags;
        public float Brightness;
        public float Contrast;
        public float Hue;
        public float Saturation;
    }

    /// <summary>
    /// From VMR9MonitorInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VMR9MonitorInfo
    {
        public int uDevID;
        public DsRect rcMonitor;
        public int hMon;
        public int dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string szDescription;
        public long liDriverVersion;
        public int dwVendorId;
        public int dwDeviceId;
        public int dwSubSysId;
        public int dwRevision;
    }

    /// <summary>
    /// From VMR9DeinterlaceCaps
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9DeinterlaceCaps
    {
        public int dwSize;
        public int dwNumPreviousOutputFrames;
        public int dwNumForwardRefSamples;
        public int dwNumBackwardRefSamples;
        public VMR9DeinterlaceTech DeinterlaceTechnology;
    }

    /// <summary>
    /// From VMR9VideoStreamInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9VideoStreamInfo
    {
        public IntPtr pddsVideoSurface; // IDirect3DSurface9
        public int dwWidth;
        public int dwHeight;
        public int dwStrmID;
        public float fAlpha;
        public VMR9NormalizedRect rNormal;
        public long rtStart;
        public long rtEnd;
        public VMR9SampleFormat SampleFormat;
    }

    /// <summary>
    /// From VMR9VideoDesc
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9VideoDesc
    {
        public int dwSize;
        public int dwSampleWidth;
        public int dwSampleHeight;
        public VMR9SampleFormat SampleFormat;
        public int dwFourCC;
        public VMR9Frequency InputSampleFreq;
        public VMR9Frequency OutputFrameFreq;
    }

    /// <summary>
    /// From VMR9Frequency
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9Frequency
    {
        public int dwNumerator;
        public int dwDenominator;
    }

    /// <summary>
    /// From VMR9AlphaBitmap
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9AlphaBitmap
    {
        public VMR9AlphaBitmapFlags dwFlags;
        public IntPtr hdc; // HDC
        public IntPtr pDDS; // IDirect3DSurface9
        public DsRect rSrc;
        public VMR9NormalizedRect rDest;
        public float fAlpha;
        public int clrSrcKey;
        public VMRMixerPrefs dwFilterMode;
    }

    /// <summary>
    /// From VMR9ProcAmpControlRange
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMR9ProcAmpControlRange
    {
        public int dwSize; // should be 24
        public VMR9ProcAmpControlFlags dwProperty;
        public float MinValue;
        public float MaxValue;
        public float DefaultValue;
        public float StepSize;
    }

    #endregion

    #region Interfaces

    [Guid("dfc581a1-6e1f-4c3a-8d0a-5e9792ea2afc"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRSurface9
    {
        [PreserveSig]
        int IsSurfaceLocked();

        [PreserveSig]
        int LockSurface([Out] out IntPtr lpSurface); // BYTE**

        [PreserveSig]
        int UnlockSurface();

        [PreserveSig]
        int GetSurface([Out, MarshalAs(UnmanagedType.IUnknown)] out object lplpSurface);
    }

    [ComVisible(true),
    Guid("69188c61-12a3-40f0-8ffc-342e7b433fd7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImagePresenter9
    {
        [PreserveSig]
        int StartPresenting([In] IntPtr dwUserID);

        [PreserveSig]
        int StopPresenting([In] IntPtr dwUserID);

        [PreserveSig]
        int PresentImage([In] IntPtr dwUserID, [In] ref VMR9PresentationInfo lpPresInfo);

    }

    [ComVisible(true),
    Guid("6de9a68a-a928-4522-bf57-655ae3866456"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRSurfaceAllocatorEx9 : IVMRSurfaceAllocator9
    {

        #region IVMRSurfaceAllocator9 Methods

        [PreserveSig]
        new int InitializeDevice(
            [In] IntPtr dwUserID,
            [In] ref VMR9AllocationInfo lpAllocInfo,
            [In, Out] ref int lpNumBuffers
            );

        [PreserveSig]
        new int TerminateDevice([In] IntPtr dwID);

        [PreserveSig]
        new int GetSurface(
            [In] IntPtr dwUserID,
            [In] int SurfaceIndex,
            [In] int SurfaceFlags,
            [Out] out IntPtr lplpSurface
            );

        [PreserveSig]
        new int AdviseNotify([In] IVMRSurfaceAllocatorNotify9 lpIVMRSurfAllocNotify);

        #endregion

        [PreserveSig]
        int GetSurfaceEx(
            [In] IntPtr dwUserID,
            [In] int SurfaceIndex,
            [In] int SurfaceFlags,
            [Out] out IntPtr lplpSurface,
            [Out] out DsRect lprcDst
            );
    }


    [Guid("dca3f5df-bb3a-4d03-bd81-84614bfbfa0c"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRSurfaceAllocatorNotify9
    {
        [PreserveSig]
        int AdviseSurfaceAllocator(
            [In] IntPtr dwUserID,
            [In] IVMRSurfaceAllocator9 lpIVRMSurfaceAllocator
            );

        [PreserveSig]
        int SetD3DDevice(
            [In] IntPtr lpD3DDevice,
            [In] IntPtr hMonitor
            );

        [PreserveSig]
        int ChangeD3DDevice(
            [In] IntPtr lpD3DDevice,
            [In] IntPtr hMonitor
            );

        [PreserveSig]
        int AllocateSurfaceHelper(
            [In] ref VMR9AllocationInfo lpAllocInfo,
            [In, Out] ref int lpNumBuffers,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysInt)] IntPtr[] lplpSurface
            );

        [PreserveSig]
        int NotifyEvent(
            [In] DsEvCode EvCode,
            [In] IntPtr Param1,
            [In] IntPtr Param2
            );
    }

    [ComVisible(true),
    Guid("8d5148ea-3f5d-46cf-9df1-d1b896eedb1f"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRSurfaceAllocator9
    {
        [PreserveSig]
        int InitializeDevice(
            [In] IntPtr dwUserID,
            [In] ref VMR9AllocationInfo lpAllocInfo,
            [In, Out] ref int lpNumBuffers
            );

        [PreserveSig]
        int TerminateDevice([In] IntPtr dwID);

        [PreserveSig]
        int GetSurface(
            [In] IntPtr dwUserID,
            [In] int SurfaceIndex,
            [In] int SurfaceFlags,
            [Out] out IntPtr lplpSurface
            );

        [PreserveSig]
        int AdviseNotify([In] IVMRSurfaceAllocatorNotify9 lpIVMRSurfAllocNotify);
    }

    [Guid("5a804648-4f66-4867-9c43-4f5c822cf1b8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRFilterConfig9
    {
        [PreserveSig]
        int SetImageCompositor([In] IVMRImageCompositor9 lpVMRImgCompositor);

        [PreserveSig]
        int SetNumberOfStreams([In] int dwMaxStreams);

        [PreserveSig]
        int GetNumberOfStreams([Out] out int pdwMaxStreams);

        [PreserveSig]
        int SetRenderingPrefs([In] VMR9RenderPrefs dwRenderFlags);

        [PreserveSig]
        int GetRenderingPrefs([Out] out VMR9RenderPrefs pdwRenderFlags);

        [PreserveSig]
        int SetRenderingMode([In] VMR9Mode Mode);

        [PreserveSig]
        int GetRenderingMode([Out] out VMR9Mode Mode);
    }

    [Guid("8f537d09-f85e-4414-b23b-502e54c79927"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRWindowlessControl9
    {
        int GetNativeVideoSize(
            [Out] out int lpWidth,
            [Out] out int lpHeight,
            [Out] out int lpARWidth,
            [Out] out int lpARHeight
            );

        int GetMinIdealVideoSize(
            [Out] out int lpWidth,
            [Out] out int lpHeight
            );

        int GetMaxIdealVideoSize(
            [Out] out int lpWidth,
            [Out] out int lpHeight
            );

        int SetVideoPosition(
            [In] DsRect lpSRCRect,
            [In] DsRect lpDSTRect
            );

        int GetVideoPosition(
            [Out] DsRect lpSRCRect,
            [Out] DsRect lpDSTRect
            );

        int GetAspectRatioMode([Out] out VMR9AspectRatioMode lpAspectRatioMode);

        int SetAspectRatioMode([In] VMR9AspectRatioMode AspectRatioMode);

        int SetVideoClippingWindow([In] IntPtr hwnd); // HWND

        int RepaintVideo(
            [In] IntPtr hwnd, // HWND
            [In] IntPtr hdc // HDC
            );

        int DisplayModeChanged();

        int GetCurrentImage([Out] out IntPtr lpDib); // BYTE**

        int SetBorderColor([In] int Clr);

        int GetBorderColor([Out] out int lpClr);
    }

    [Guid("00d96c29-bbde-4efc-9901-bb5036392146"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRAspectRatioControl9
    {
        [PreserveSig]
        int GetAspectRatioMode([Out] out VMRAspectRatioMode lpdwARMode);

        [PreserveSig]
        int SetAspectRatioMode([In] VMRAspectRatioMode lpdwARMode);
    }

    [Guid("a215fb8d-13c2-4f7f-993c-003d6271a459"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRDeinterlaceControl9
    {
        [PreserveSig]
        int GetNumberOfDeinterlaceModes(
            [In] ref VMR9VideoDesc lpVideoDescription,
            [In, Out] ref int lpdwNumDeinterlaceModes,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] Guid[] lpDeinterlaceModes
            );

        [PreserveSig]
        int GetDeinterlaceModeCaps(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid lpDeinterlaceMode,
            [In] ref VMR9VideoDesc lpVideoDescription,
            [In, Out] ref VMR9DeinterlaceCaps lpDeinterlaceCaps
            );

        [PreserveSig]
        int GetDeinterlaceMode(
            [In] int dwStreamID,
            [Out] out Guid lpDeinterlaceMode
            );

        [PreserveSig]
        int SetDeinterlaceMode(
            [In] int dwStreamID,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid lpDeinterlaceMode
            );

        [PreserveSig]
        int GetDeinterlacePrefs([Out] out VMR9DeinterlacePrefs lpdwDeinterlacePrefs);

        [PreserveSig]
        int SetDeinterlacePrefs([In] VMR9DeinterlacePrefs lpdwDeinterlacePrefs);

        [PreserveSig]
        int GetActualDeinterlaceMode(
            [In] int dwStreamID,
            [Out] out Guid lpDeinterlaceMode
            );
    }

    [Guid("4a5c89eb-df51-4654-ac2a-e48e02bbabf6"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImageCompositor9
    {
        [PreserveSig]
        int InitCompositionDevice([In] IntPtr pD3DDevice);

        [PreserveSig]
        int TermCompositionDevice([In] IntPtr pD3DDevice);

        [PreserveSig]
        int SetStreamMediaType(
            [In] int dwStrmID,
            [In] AMMediaType pmt,
            [In, MarshalAs(UnmanagedType.Bool)] bool fTexture
            );

        [PreserveSig]
        int CompositeImage(
            [In] IntPtr pD3DDevice,
            [In] IntPtr pddsRenderTarget, // IDirect3DSurface9
            [In] AMMediaType pmtRenderTarget,
            [In] long rtStart,
            [In] long rtEnd,
            [In] int dwClrBkGnd,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 7)] VMR9VideoStreamInfo[] pVideoStreamInfo,
            [In] int cStreams
            );
    }

    [ComVisible(true),
    Guid("45c15cab-6e22-420a-8043-ae1f0ac02c7d"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImagePresenterConfig9
    {
        [PreserveSig]
        int SetRenderingPrefs([In] VMR9RenderPrefs dwRenderFlags);

        [PreserveSig]
        int GetRenderingPrefs([Out] out VMR9RenderPrefs dwRenderFlags);
    }

    [Guid("ced175e5-1935-4820-81bd-ff6ad00c9108"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRMixerBitmap9
    {
        [PreserveSig]
        int SetAlphaBitmap([In] ref VMR9AlphaBitmap pBmpParms);

        [PreserveSig]
        int UpdateAlphaBitmapParameters([In] ref VMR9AlphaBitmap pBmpParms);

        [PreserveSig]
        int GetAlphaBitmapParameters([Out] out VMR9AlphaBitmap pBmpParms);
    }

    [Guid("1a777eaa-47c8-4930-b2c9-8fee1c1b0f3b"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRMixerControl9
    {
        [PreserveSig]
        int SetAlpha(
            [In] int dwStreamID,
            [In] float Alpha
            );

        [PreserveSig]
        int GetAlpha(
            [In] int dwStreamID,
            [Out] out float Alpha
            );

        [PreserveSig]
        int SetZOrder(
            [In] int dwStreamID,
            [In] int dwZ
            );

        [PreserveSig]
        int GetZOrder(
            [In] int dwStreamID,
            [Out] out int dwZ
            );

        [PreserveSig]
        int SetOutputRect(
            [In] int dwStreamID,
            [In] ref VMR9NormalizedRect pRect
            );

        [PreserveSig]
        int GetOutputRect(
            [In] int dwStreamID,
            [Out] out VMR9NormalizedRect pRect
            );

        [PreserveSig]
        int SetBackgroundClr([In] int ClrBkg);

        [PreserveSig]
        int GetBackgroundClr([Out] out int ClrBkg);

        [PreserveSig]
        int SetMixingPrefs([In] VMR9MixerPrefs dwMixerPrefs);

        [PreserveSig]
        int GetMixingPrefs([Out] out VMR9MixerPrefs dwMixerPrefs);

        [PreserveSig]
        int SetProcAmpControl(
            [In] int dwStreamID,
            [In] ref VMR9ProcAmpControl lpClrControl
            );

        [PreserveSig]
        int GetProcAmpControl(
            [In] int dwStreamID,
            [In, Out] ref VMR9ProcAmpControl lpClrControl
            );

        [PreserveSig]
        int GetProcAmpControlRange(
            [In] int dwStreamID,
            [In, Out] ref VMR9ProcAmpControlRange lpClrControl
            );
    }

    [Guid("46c2e457-8ba0-4eef-b80b-0680f0978749"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRMonitorConfig9
    {
        [PreserveSig]
        int SetMonitor([In] int uDev);

        [PreserveSig]
        int GetMonitor([Out] out int uDev);

        [PreserveSig]
        int SetDefaultMonitor([In] int uDev);

        [PreserveSig]
        int GetDefaultMonitor([Out] out int uDev);

        [PreserveSig]
        int GetAvailableMonitors(
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] VMR9MonitorInfo[] pInfo,
            [In] int dwMaxInfoArraySize,
            [Out] out int pdwNumDevices
            );
    }

    [Guid("d0cfe38b-93e7-4772-8957-0400c49a4485"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRVideoStreamControl9
    {
        [PreserveSig]
        int SetStreamActiveState([In, MarshalAs(UnmanagedType.Bool)] bool fActive);

        [PreserveSig]
        int GetStreamActiveState([Out, MarshalAs(UnmanagedType.Bool)] out bool fActive);
    }

    #endregion

    #region Filter

    [Guid("51b4abf3-748f-4e3b-a276-c828330e926a")]
    public class VMR9Renderer : DSFilter
    {
        public VMR9Renderer()
            : base()
        {
        }
    }

    #endregion

    public delegate void SurfaceReadyHandler(ref Surface _surface);

#region Video Capture Graph

    public class DSVideoCaptureVMR9 : DSFilterGraphBase, IVMRSurfaceAllocator9, IVMRImagePresenter9
    {
        #region Variables

        protected VMR9Renderer m_Renderer = null;
        protected Device m_Device = null;
        protected const int g_ciUsedID = 0x01020304;
        protected IVMRSurfaceAllocatorNotify9 m_lpIVMRSurfAllocNotify = null;
        protected object m_csLock = new object();
        protected List<Surface> m_Surfaces = new List<Surface>();
        protected Texture m_PrivateTexture = null;

        protected int m_nWidth = 0;
        protected int m_nHeight = 0;

        protected string m_leftEyeDevicePath;
        protected string m_rightEyeDevicePath;

        IVMRMixerControl9 m_mixerControl = null;

        #endregion

        #region Properties

        public int Width
        {
            get { return m_nWidth; }
        }

        public int Height
        {
            get { return m_nHeight; }
        }

        public Device Direct3DDevice
        {
            get { return m_Device; }
            set { m_Device = value; }
        }

        #endregion

        #region Constructor

        public DSVideoCaptureVMR9(Device device, string leftEyeDevicePath, string rightEyeDevicePath)
        {
            m_Device = device;
            m_leftEyeDevicePath  = leftEyeDevicePath;
            m_rightEyeDevicePath = rightEyeDevicePath;
        }

        #endregion

        #region Events

        public event SurfaceReadyHandler OnSurfaceReady;

        #endregion

        #region Overridden Methods

        protected override HRESULT OnInitInterfaces()
        {
            m_Renderer = new VMR9Renderer();
            m_Renderer.FilterGraph = m_GraphBuilder;
            IVMRFilterConfig9 _config = (IVMRFilterConfig9)m_Renderer.QueryInterface(typeof(IVMRFilterConfig9).GUID);
            HRESULT hr;
            if (_config != null)
            {
                hr = (HRESULT)_config.SetRenderingMode(VMR9Mode.Renderless);
                hr.Assert();
                hr = (HRESULT)_config.SetNumberOfStreams(2);
                hr.Assert();
            }

            m_mixerControl = (IVMRMixerControl9)m_Renderer.QueryInterface(typeof(IVMRMixerControl9).GUID);

            m_mixerControl.SetBackgroundClr((int)0xbf7f2f);

            VMR9MixerPrefs mixerPrefs;
            m_mixerControl.GetMixingPrefs(out mixerPrefs);
            m_mixerControl.SetMixingPrefs(mixerPrefs);

            IVMRSurfaceAllocatorNotify9 _notify = (IVMRSurfaceAllocatorNotify9)m_Renderer.QueryInterface(typeof(IVMRSurfaceAllocatorNotify9).GUID);
            if (_notify != null)
            {
                hr = (HRESULT)_notify.AdviseSurfaceAllocator(new IntPtr(g_ciUsedID), this);
                hr.Assert();
                hr = (HRESULT)this.AdviseNotify(_notify);
                hr.Assert();
            }

            DSVideoCaptureCategory captureCategory = new DSVideoCaptureCategory();
            List<DSFilter> capFilters = new List<DSFilter>();
            foreach (var captureDevice in captureCategory.Objects)
            {
                if (captureDevice.DevicePath == m_leftEyeDevicePath)
                {
                    capFilters.Add(captureDevice.Filter);
                    break;
                }
            }

            foreach (var captureDevice in captureCategory.Objects)
            {
                if (captureDevice.DevicePath == m_rightEyeDevicePath)
                {
                    capFilters.Add(captureDevice.Filter);
                    break;
                }
            }

            if (capFilters.Count < 2)
            {
                MessageBox.Show("Not enough capture devices found (" + capFilters.Count.ToString() + " device(s))");
                throw new Exception();
            }


            for (int i = 0; i < 2; i++)
            {
                if (capFilters[i] == null)
                {
                    return E_FAIL;
                }

                DSPin capturePin = null;

                foreach (var outputPin in capFilters[i].Output)
                {
                    if (outputPin.Name == "Capture")
                    {
                        capturePin = outputPin;
                        break;
                    }
                }

                AMMediaType mjpgMediaType = null;

                int maxPixels = -1;
                foreach (var mediaType in capturePin.MediaTypes)
                {
                    VideoInfoHeader videoInfo = new VideoInfoHeader();
                    videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));

                    // pick the highest res mode...
                    if ((videoInfo.BmiHeader.Width * videoInfo.BmiHeader.Height) > maxPixels)
                    {
                        maxPixels = videoInfo.BmiHeader.Width * videoInfo.BmiHeader.Height;
                        mjpgMediaType = mediaType;
                        //break;
                    }
                }

                capFilters[i].OutputPin.Format = mjpgMediaType;

                capFilters[i].FilterGraph = m_GraphBuilder;
                capFilters[i].Connect(m_Renderer);
            }


            VMR9NormalizedRect r1 = new VMR9NormalizedRect(0, 0, 0.5f, 1f);
            VMR9NormalizedRect r2 = new VMR9NormalizedRect(0.5f, 0f, 1f, 1f);

            int rt0 = m_mixerControl.SetOutputRect(0, ref r1);
            int rt1 = m_mixerControl.SetOutputRect(1, ref r2);

            hr = base.OnInitInterfaces();

            return hr;
        }

        protected override HRESULT OnCloseInterfaces()
        {
            if (m_Renderer)
            {
                m_Renderer.Dispose();
                m_Renderer = null;
            }
            if (m_lpIVMRSurfAllocNotify != null)
            {
                Marshal.ReleaseComObject(m_lpIVMRSurfAllocNotify);
                m_lpIVMRSurfAllocNotify = null;
            }
            return base.OnCloseInterfaces();
        }

        #endregion

        #region IVMRSurfaceAllocator9 Members

        public int InitializeDevice(IntPtr dwUserID, ref VMR9AllocationInfo lpAllocInfo, ref int lpNumBuffers)
        {
            ASSERT(dwUserID.ToInt32() == g_ciUsedID);
            if ((object)lpAllocInfo == null) return E_FAIL;
            if ((object)lpNumBuffers == null) return E_POINTER;

            HRESULT hr;
            int dwWidth = 1;
            int dwHeight = 1;
            if ((int)(m_Device.Capabilities.TextureCaps & TextureCaps.CubeMapPow2) != 0)
            {
                while (dwWidth < lpAllocInfo.dwWidth)
                    dwWidth = dwWidth << 1;
                while (dwHeight < lpAllocInfo.dwHeight)
                    dwHeight = dwHeight << 1;

                lpAllocInfo.dwWidth = dwWidth;
                lpAllocInfo.dwHeight = dwHeight;
            }
            m_nWidth = lpAllocInfo.dwWidth;
            m_nHeight = lpAllocInfo.dwHeight;

            lpAllocInfo.dwFlags |= VMR9SurfaceAllocationFlags.TextureSurface;
            m_Surfaces.Clear();
            IntPtr[] lplpSurfaces = new IntPtr[lpNumBuffers];
            hr = (HRESULT)m_lpIVMRSurfAllocNotify.AllocateSurfaceHelper(ref lpAllocInfo, ref lpNumBuffers, lplpSurfaces);
            hr.Assert();
            if (hr.Succeeded)
            //if (0)
            {
                for (int i = 0; i < lplpSurfaces.Length; i++)
                {
                    Marshal.AddRef(lplpSurfaces[i]);
                    Surface _surface = Surface.FromPointer(lplpSurfaces[i]);
                    m_Surfaces.Add(_surface);
                }
            }
            else
            {
                int nYUV = (int)(((int)'0') << 24 + ((int)'0') << 16 + ((int)'0') << 8 + ((int)'0'));
                if (lpAllocInfo.Format > nYUV)
                {
                    DisplayMode _mode = m_Device.GetDisplayMode(0);
                    m_PrivateTexture = new Texture(m_Device, _mode.Width, _mode.Height, 1, Usage.RenderTarget, _mode.Format, Pool.Default);
                }
                lpAllocInfo.dwFlags &= ~VMR9SurfaceAllocationFlags.TextureSurface;
                lpAllocInfo.dwFlags |= VMR9SurfaceAllocationFlags.OffscreenSurface;

                hr = (HRESULT)m_lpIVMRSurfAllocNotify.AllocateSurfaceHelper(ref lpAllocInfo, ref lpNumBuffers, lplpSurfaces);
                hr.Assert();
                for (int i = 0; i < lplpSurfaces.Length; i++)
                {
                    Marshal.AddRef(lplpSurfaces[i]);
                    Surface _surface = Surface.FromPointer(lplpSurfaces[i]);
                    m_Surfaces.Add(_surface);
                }
            }
            return hr;
        }

        public int TerminateDevice(IntPtr dwID)
        {
            ASSERT(dwID.ToInt32() == g_ciUsedID);
            lock (m_csLock)
            {
                m_PrivateTexture = null;
                while (m_Surfaces.Count > 0)
                {
                    Surface _surface = m_Surfaces[0];
                    m_Surfaces.Remove(_surface);
                    Marshal.Release(_surface.ComPointer);
                    _surface.Dispose();
                    _surface = null;
                }
            }
            return NOERROR;
        }

        public int GetSurface(IntPtr dwUserID, int SurfaceIndex, int SurfaceFlags, out IntPtr lplpSurface)
        {
            ASSERT(dwUserID.ToInt32() == g_ciUsedID);
            lplpSurface = IntPtr.Zero;
            if (SurfaceIndex > m_Surfaces.Count) return E_INVALIDARG;
            lock (m_csLock)
            {
                lplpSurface = m_Surfaces[SurfaceIndex].ComPointer;
                Marshal.AddRef(lplpSurface);
            }
            return NOERROR;
        }

        public int AdviseNotify(IVMRSurfaceAllocatorNotify9 lpIVMRSurfAllocNotify)
        {
            lock (m_csLock)
            {
                m_lpIVMRSurfAllocNotify = lpIVMRSurfAllocNotify;

                if (m_lpIVMRSurfAllocNotify != null)
                {
                    IntPtr hMonitor = m_Device.Direct3D.GetAdapterMonitor(m_Device.CreationParameters.AdapterOrdinal);
                    HRESULT hr = (HRESULT)m_lpIVMRSurfAllocNotify.SetD3DDevice(m_Device.ComPointer, hMonitor);
                    hr.Assert();
                    return hr;
                }
            }
            return NOERROR;
        }

        #endregion

        #region IVMRImagePresenter9 Members

        public int StartPresenting(IntPtr dwUserID)
        {
            return NOERROR;
        }

        public int StopPresenting(IntPtr dwUserID)
        {
            return NOERROR;
        }

        public int PresentImage(IntPtr dwUserID, ref VMR9PresentationInfo lpPresInfo)
        {
            if ((object)lpPresInfo == null)
            {
                return E_POINTER;
            }
            else if (lpPresInfo.lpSurf == IntPtr.Zero)
            {
                return E_POINTER;
            }
            lock (m_csLock)
            {
                Surface _source = Surface.FromPointer(lpPresInfo.lpSurf);
                if (_source != null && OnSurfaceReady != null)
                {
                    if (m_PrivateTexture != null)
                    {
                        Surface _target = m_PrivateTexture.GetSurfaceLevel(0);
                        m_Device.StretchRectangle(_source, _target, TextureFilter.None);
                        OnSurfaceReady(ref _target);
                    }
                    else
                    {
                        OnSurfaceReady(ref _source);
                    }
                }
            }
            return NOERROR;
        }

        #endregion
    }

    #endregion
}
