using System;
using System.Runtime.InteropServices;

namespace PicStonePlus.SDK
{
    // Entry point do módulo: NKERROR MAIDEntryPointProc(LPNkMAIDObject, ULONG, ULONG, ULONG, NKPARAM, LPNKFUNC, NKREF)
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int MAIDEntryPointProc(
        IntPtr pObject,         // LPNkMAIDObject
        uint ulCommand,         // eNkMAIDCommand
        uint ulParam,           // parameter for command
        uint ulDataType,        // eNkMAIDDataType
        IntPtr data,            // NKPARAM - pointer or value
        IntPtr pfnComplete,     // completion function pointer
        IntPtr refComplete      // reference for completion
    );

    // Completion callback: void MAIDCompletionProc(LPNkMAIDObject, ULONG, ULONG, ULONG, NKPARAM, NKREF, NKERROR)
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void MAIDCompletionProc(
        IntPtr pObject,
        uint ulCommand,
        uint ulParam,
        uint ulDataType,
        IntPtr data,
        IntPtr refComplete,
        int nResult
    );

    // Data callback: NKERROR MAIDDataProc(NKREF, LPVOID pDataInfo, LPVOID pData)
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int MAIDDataProc(
        IntPtr refClient,
        IntPtr pDataInfo,
        IntPtr pData
    );

    // Event callback: void MAIDEventProc(NKREF, ULONG ulEvent, NKPARAM data)
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void MAIDEventProc(
        IntPtr refClient,
        uint ulEvent,
        IntPtr data
    );

    // Progress callback: void MAIDProgressProc(ULONG, ULONG, NKREF, ULONG ulDone, ULONG ulTotal)
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void MAIDProgressProc(
        uint ulCommand,
        uint ulParam,
        IntPtr refProc,
        uint ulDone,
        uint ulTotal
    );

    // UI Request callback: ULONG MAIDUIRequestProc(NKREF, LPNkMAIDUIRequestInfo)
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate uint MAIDUIRequestProc(
        IntPtr refProc,
        IntPtr pUIRequest
    );
}
