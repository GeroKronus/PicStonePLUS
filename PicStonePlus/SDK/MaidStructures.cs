using System;
using System.Runtime.InteropServices;

namespace PicStonePlus.SDK
{
    // NkMAIDObject - pack 2 como no SDK C (pragma pack(push, 2))
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDObject
    {
        public uint ulType;      // eNkMAIDObjectType
        public uint ulID;
        public IntPtr refClient; // NKREF
        public IntPtr refModule; // NKREF (set by module on Open)
    }

    // NkMAIDCallback
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDCallback
    {
        public IntPtr pProc;     // function pointer
        public IntPtr refProc;   // NKREF
    }

    // NkMAIDCapInfo
    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public struct NkMAIDCapInfo
    {
        public uint ulID;           // eNkMAIDCapability
        public uint ulType;         // eNkMAIDCapType
        public uint ulVisibility;   // eNkMAIDCapVisibility
        public uint ulOperations;   // eNkMAIDCapOperations

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szDescription;
    }

    // NkMAIDEnum
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDEnum
    {
        public uint ulType;          // eNkMAIDArrayType
        public uint ulElements;      // total number of elements
        public uint ulValue;         // current index
        public uint ulDefault;       // default index
        public short wPhysicalBytes; // bytes per element (SWORD no SDK)
        public IntPtr pData;         // allocated by the client
    }

    // NkMAIDRange
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDRange
    {
        public double lfValue;
        public double lfDefault;
        public uint ulValueIndex;
        public uint ulDefaultIndex;
        public double lfLower;
        public double lfUpper;
        public uint ulSteps;
    }

    // NkMAIDString
    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public struct NkMAIDString
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string str;
    }

    // NkMAIDArray
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDArray
    {
        public uint ulType;          // eNkMAIDArrayType
        public uint ulElements;
        public uint ulDimSize1;
        public uint ulDimSize2;
        public uint ulDimSize3;
        public ushort wPhysicalBytes;
        public ushort wLogicalBits;
        public IntPtr pData;
    }

    // NkMAIDDateTime
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDDateTime
    {
        public ushort nYear;
        public ushort nMonth;    // 0-11
        public ushort nDay;      // 1-31
        public ushort nHour;     // 0-23
        public ushort nMinute;   // 0-59
        public ushort nSecond;   // 0-59
        public uint nSubsecond;
    }

    // NkMAIDPoint
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDPoint
    {
        public int x;
        public int y;
    }

    // NkMAIDSize
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDSize
    {
        public uint w;
        public uint h;
    }

    // NkMAIDRect
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDRect
    {
        public int x;
        public int y;
        public uint w;
        public uint h;
    }

    // NkMAIDDataInfo
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDDataInfo
    {
        public uint ulType; // eNkMAIDDataObjType
    }

    // NkMAIDFileInfo
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDFileInfo
    {
        public NkMAIDDataInfo baseInfo;
        public uint ulFileDataType;  // eNkMAIDFileDataTypes
        public uint ulTotalLength;
        public uint ulStart;
        public uint ulLength;
        public int fDiskFile;        // BOOL
        public int fRemoveObject;    // BOOL
    }

    // NkMAIDImageInfo
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NkMAIDImageInfo
    {
        public NkMAIDDataInfo baseInfo;
        public NkMAIDSize szTotalPixels;
        public uint ulColorSpace;
        public NkMAIDRect rData;
        public uint ulRowBytes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] wBits;
        public ushort wPlane;
        public int fRemoveObject; // BOOL
    }

    // RefObj - estrutura do sample para rastrear hierarquia
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct RefObj
    {
        public IntPtr pObject;         // LPNkMAIDObject
        public int lMyID;
        public IntPtr pRefParent;      // LPRefObj
        public uint ulChildCount;
        public IntPtr pRefChildArray;  // LPRefObj array
        public uint ulCapCount;
        public IntPtr pCapArray;       // LPNkMAIDCapInfo
    }

    // RefCompletionProc - conforme SDK sample D7200 (count inline, não ponteiro)
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct RefCompletionProc
    {
        public uint ulCount;     // counter INLINE (ULONG, conforme SDK sample)
        public int nResult;      // NKERROR
        public IntPtr pRef;      // extra reference
    }

    // RefDataProc
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct RefDataProc
    {
        public IntPtr pBuffer;
        public uint ulOffset;
        public uint ulTotalLines;
        public int lID;
    }
}
