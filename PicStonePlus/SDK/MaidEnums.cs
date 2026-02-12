namespace PicStonePlus.SDK
{
    public enum eNkMAIDResult : int
    {
        kNkMAIDResult_NotSupported = -127,
        kNkMAIDResult_UnexpectedDataType = -126,
        kNkMAIDResult_ValueOutOfBounds = -125,
        kNkMAIDResult_BufferSize = -124,
        kNkMAIDResult_Aborted = -123,
        kNkMAIDResult_NoMedia = -122,
        kNkMAIDResult_NoEventProc = -121,
        kNkMAIDResult_NoDataProc = -120,
        kNkMAIDResult_ZombieObject = -119,
        kNkMAIDResult_OutOfMemory = -118,
        kNkMAIDResult_UnexpectedError = -117,
        kNkMAIDResult_HardwareError = -116,
        kNkMAIDResult_MissingComponent = -115,
        kNkMAIDResult_NoError = 0,
        kNkMAIDResult_Pending = 1,
        kNkMAIDResult_OrphanedChildren = 2,
        kNkMAIDResult_VendorBase = 127,
        // Vendor-specific (D1/DX2)
        kNkMAIDResult_ApertureFEE = 128,
        kNkMAIDResult_BufferNotReady = 129,
        kNkMAIDResult_NormalTTL = 130,
        kNkMAIDResult_MediaFull = 131,
        kNkMAIDResult_InvalidMedia = 132,
        kNkMAIDResult_EraseFailure = 133,
        kNkMAIDResult_CameraNotFound = 134,
        kNkMAIDResult_BatteryDontWork = 135,
        kNkMAIDResult_ShutterBulb = 136,
        kNkMAIDResult_OutOfFocus = 137,
        kNkMAIDResult_Protected = 138,
        kNkMAIDResult_FileExists = 139,
        kNkMAIDResult_SharingViolation = 140,
        kNkMAIDResult_DataTransFailure = 141,
        kNkMAIDResult_SessionFailure = 142,
        kNkMAIDResult_FileRemoved = 143,
        kNkMAIDResult_BusReset = 144,
        kNkMAIDResult_NonCPULens = 145,
        kNkMAIDResult_ReleaseButtonPressed = 146,
        kNkMAIDResult_BatteryExhausted = 147,
        kNkMAIDResult_CaptureFailure = 148,
        kNkMAIDResult_InvalidString = 149,
        kNkMAIDResult_NotInitialized = 150,
        kNkMAIDResult_CaptureDisable = 151,
        kNkMAIDResult_DeviceBusy = 152,
        kNkMAIDResult_CaptureDustFailure = 153,
        kNkMAIDResult_NotLiveView = 159,
        kNkMAIDResult_MFDriveEnd = 160,
        kNkMAIDResult_High_Temperature = 170,
    }

    public enum eNkMAIDCommand : uint
    {
        kNkMAIDCommand_Async = 0,
        kNkMAIDCommand_Open,
        kNkMAIDCommand_Close,
        kNkMAIDCommand_GetCapCount,
        kNkMAIDCommand_GetCapInfo,
        kNkMAIDCommand_CapStart,
        kNkMAIDCommand_CapSet,
        kNkMAIDCommand_CapGet,
        kNkMAIDCommand_CapGetDefault,
        kNkMAIDCommand_CapGetArray,
        kNkMAIDCommand_Mark,
        kNkMAIDCommand_AbortToMark,
        kNkMAIDCommand_Abort,
        kNkMAIDCommand_EnumChildren,
        kNkMAIDCommand_GetParent,
        kNkMAIDCommand_ResetToDefault,
    }

    public enum eNkMAIDDataType : uint
    {
        kNkMAIDDataType_Null = 0,
        kNkMAIDDataType_Boolean,
        kNkMAIDDataType_Integer,
        kNkMAIDDataType_Unsigned,
        kNkMAIDDataType_BooleanPtr,
        kNkMAIDDataType_IntegerPtr,
        kNkMAIDDataType_UnsignedPtr,
        kNkMAIDDataType_FloatPtr,
        kNkMAIDDataType_PointPtr,
        kNkMAIDDataType_SizePtr,
        kNkMAIDDataType_RectPtr,
        kNkMAIDDataType_StringPtr,
        kNkMAIDDataType_DateTimePtr,
        kNkMAIDDataType_CallbackPtr,
        kNkMAIDDataType_RangePtr,
        kNkMAIDDataType_ArrayPtr,
        kNkMAIDDataType_EnumPtr,
        kNkMAIDDataType_ObjectPtr,
        kNkMAIDDataType_CapInfoPtr,
        kNkMAIDDataType_GenericPtr,
    }

    public enum eNkMAIDCapType : uint
    {
        kNkMAIDCapType_Process = 0,
        kNkMAIDCapType_Boolean,
        kNkMAIDCapType_Integer,
        kNkMAIDCapType_Unsigned,
        kNkMAIDCapType_Float,
        kNkMAIDCapType_Point,
        kNkMAIDCapType_Size,
        kNkMAIDCapType_Rect,
        kNkMAIDCapType_String,
        kNkMAIDCapType_DateTime,
        kNkMAIDCapType_Callback,
        kNkMAIDCapType_Array,
        kNkMAIDCapType_Enum,
        kNkMAIDCapType_Range,
        kNkMAIDCapType_Generic,
        kNkMAIDCapType_BoolDefault,
    }

    public enum eNkMAIDArrayType : uint
    {
        kNkMAIDArrayType_Boolean = 0,
        kNkMAIDArrayType_Integer,
        kNkMAIDArrayType_Unsigned,
        kNkMAIDArrayType_Float,
        kNkMAIDArrayType_Point,
        kNkMAIDArrayType_Size,
        kNkMAIDArrayType_Rect,
        kNkMAIDArrayType_PackedString,
        kNkMAIDArrayType_String,
        kNkMAIDArrayType_DateTime,
    }

    [System.Flags]
    public enum eNkMAIDCapOperation : uint
    {
        kNkMAIDCapOperation_Start = 0x0001,
        kNkMAIDCapOperation_Get = 0x0002,
        kNkMAIDCapOperation_Set = 0x0004,
        kNkMAIDCapOperation_GetArray = 0x0008,
        kNkMAIDCapOperation_GetDefault = 0x0010,
    }

    [System.Flags]
    public enum eNkMAIDCapVisibility : uint
    {
        kNkMAIDCapVisibility_Hidden = 0x0001,
        kNkMAIDCapVisibility_Advanced = 0x0002,
        kNkMAIDCapVisibility_Vendor = 0x0004,
        kNkMAIDCapVisibility_Group = 0x0008,
        kNkMAIDCapVisibility_GroupMember = 0x0010,
        kNkMAIDCapVisibility_Invalid = 0x0020,
    }

    public enum eNkMAIDObjectType : uint
    {
        kNkMAIDObjectType_Module = 1,
        kNkMAIDObjectType_Source,
        kNkMAIDObjectType_Item,
        kNkMAIDObjectType_DataObj,
    }

    public enum eNkMAIDEvent : uint
    {
        kNkMAIDEvent_AddChild = 0,
        kNkMAIDEvent_RemoveChild,
        kNkMAIDEvent_WarmingUp,
        kNkMAIDEvent_WarmedUp,
        kNkMAIDEvent_CapChange,
        kNkMAIDEvent_OrphanedChildren,
        kNkMAIDEvent_CapChangeValueOnly,
        // Vendor events (DX2 origin = kNkMAIDEvent_CapChangeValueOnly + 0x100 = 0x106)
        kNkMAIDEvent_AddPreviewImage = 0x107,
        kNkMAIDEvent_CaptureComplete = 0x108,
        kNkMAIDEvent_AddChildInCard = 0x109,
        kNkMAIDEvent_RecordingInterrupted = 0x10A,
        kNkMAIDEvent_CapChangeOperationOnly = 0x10B,
        kNkMAIDEvent_1stCaptureComplete = 0x10C,
        kNkMAIDEvent_MirrorUpCancelComplete = 0x10D,
        kNkMAIDEvent_MovieRecordComplete = 0x115,
    }

    [System.Flags]
    public enum eNkMAIDDataObjType : uint
    {
        kNkMAIDDataObjType_Image = 0x00000001,
        kNkMAIDDataObjType_Sound = 0x00000002,
        kNkMAIDDataObjType_Video = 0x00000004,
        kNkMAIDDataObjType_Thumbnail = 0x00000008,
        kNkMAIDDataObjType_File = 0x00000010,
    }

    public enum eNkMAIDFileDataType : uint
    {
        kNkMAIDFileDataType_NotSpecified = 0,
        kNkMAIDFileDataType_JPEG,
        kNkMAIDFileDataType_TIFF,
        kNkMAIDFileDataType_FlashPix,
        kNkMAIDFileDataType_NIF,
        kNkMAIDFileDataType_QuickTime,
        kNkMAIDFileDataType_UserType = 0x100,
        kNkMAIDFileDataType_NDF = 0x101,
    }

    // Capabilities padrão MAID3
    public enum eNkMAIDCapability : uint
    {
        kNkMAIDCapability_AsyncRate = 1,
        kNkMAIDCapability_ProgressProc = 2,
        kNkMAIDCapability_EventProc = 3,
        kNkMAIDCapability_DataProc = 4,
        kNkMAIDCapability_UIRequestProc = 5,
        kNkMAIDCapability_IsAlive = 6,
        kNkMAIDCapability_Children = 7,
        kNkMAIDCapability_State = 8,
        kNkMAIDCapability_Name = 9,
        kNkMAIDCapability_Description = 10,
        kNkMAIDCapability_Interface = 11,
        kNkMAIDCapability_DataTypes = 12,
        kNkMAIDCapability_DateTime = 13,
        kNkMAIDCapability_StoredBytes = 14,
        kNkMAIDCapability_Eject = 15,
        kNkMAIDCapability_Feed = 16,
        kNkMAIDCapability_Capture = 17,
        kNkMAIDCapability_MediaPresent = 18,
        kNkMAIDCapability_Mode = 19,
        kNkMAIDCapability_Acquire = 20,
        kNkMAIDCapability_AutoFocus = 29,
        kNkMAIDCapability_Focus = 32,
        kNkMAIDCapability_Resolution = 34,
        kNkMAIDCapability_ColorSpace = 37,
        kNkMAIDCapability_Pixels = 43,
        kNkMAIDCapability_Firmware = 45,
        kNkMAIDCapability_BatteryLevel = 49,
        kNkMAIDCapability_FreeBytes = 50,
        kNkMAIDCapability_FreeItems = 51,
        kNkMAIDCapability_Remove = 52,
        kNkMAIDCapability_FlashMode = 53,
        kNkMAIDCapability_ModuleType = 54,
        kNkMAIDCapability_Version = 58,
        kNkMAIDCapability_TotalBytes = 60,
        kNkMAIDCapability_VendorBase = 0x8000,

        // Vendor capabilities (kNkMAIDCapability_VendorBaseDX2 = 0x8100)
        kNkMAIDCapability_ModuleMode = 0x8101,
        kNkMAIDCapability_CurrentDirectory = 0x8102,
        kNkMAIDCapability_FormatStorage = 0x8103,
        kNkMAIDCapability_PreCapture = 0x8104,
        kNkMAIDCapability_LockFocus = 0x8105,
        kNkMAIDCapability_LockExposure = 0x8106,
        kNkMAIDCapability_CFStatus = 0x8108,
        kNkMAIDCapability_FlashStatus = 0x810a,
        kNkMAIDCapability_ExposureStatus = 0x810b,
        kNkMAIDCapability_FileType = 0x810f,
        kNkMAIDCapability_CompressionLevel = 0x8110,
        kNkMAIDCapability_ExposureMode = 0x8111,
        kNkMAIDCapability_ShutterSpeed = 0x8112,
        kNkMAIDCapability_Aperture = 0x8113,
        kNkMAIDCapability_FlexibleProgram = 0x8114,
        kNkMAIDCapability_ExposureComp = 0x8115,
        kNkMAIDCapability_MeteringMode = 0x8116,
        kNkMAIDCapability_Sensitivity = 0x8117,
        kNkMAIDCapability_WBMode = 0x8118,
        kNkMAIDCapability_WBTuneAuto = 0x8119,
        kNkMAIDCapability_WBTuneIncandescent = 0x811a,
        kNkMAIDCapability_WBTuneFluorescent = 0x811b,
        kNkMAIDCapability_WBTuneSunny = 0x811c,
        kNkMAIDCapability_WBTuneFlash = 0x811d,
        kNkMAIDCapability_WBTuneShade = 0x811e,
        kNkMAIDCapability_WBTuneCloudy = 0x811f,
        kNkMAIDCapability_FocusMode = 0x8120,
        kNkMAIDCapability_FocusAreaMode = 0x8121,
        kNkMAIDCapability_AFsPriority = 0x812A,            // VBD1 + 0x2A
        kNkMAIDCapability_AFcPriority = 0x812B,            // VBD1 + 0x2B
        kNkMAIDCapability_FocusPreferredArea = 0x8122,
        kNkMAIDCapability_FocalLength = 0x8123,
        kNkMAIDCapability_ClockDateTime = 0x8124,
        kNkMAIDCapability_WBTuneColorTemp = 0x818C,        // VBD1 + 0x8C (D7100: Enum)
        kNkMAIDCapability_ShootingMode = 0x818d,
        kNkMAIDCapability_ShootingBankName = 0x8190,
        kNkMAIDCapability_WBPresetNumber = 0x8151,
        kNkMAIDCapability_WBPresetData = 0x81ad,
        kNkMAIDCapability_LockCamera = 0x8141,
        kNkMAIDCapability_LensInfo = 0x8144,
        kNkMAIDCapability_UserComment = 0x8148,
        kNkMAIDCapability_EnableComment = 0x8176,
        kNkMAIDCapability_AfSubLight = 0x816F,             // VBD1 + 0x6F
        kNkMAIDCapability_IsoControl = 0x816c,
        kNkMAIDCapability_NoiseReduction = 0x816d,
        kNkMAIDCapability_ImageSize = 0x8157,
        kNkMAIDCapability_JpegCompressionPolicy = 0x81D1,  // VBD1 + 0xD1
        kNkMAIDCapability_ImageColorSpace = 0x81db,
        kNkMAIDCapability_NoiseReductionHighISO = 0x81d8,
        kNkMAIDCapability_CameraType = 0x81d7,
        kNkMAIDCapability_DeviceNameList = 0x81da,
        // Capabilities com offset >= 0x100 de VendorBaseD1 (0x8100)
        // Corrigido: VendorBaseD1 + offset = 0x8100 + offset (conforme PicStone/SDK D7200)
        kNkMAIDCapability_AFCapture = 0x8225,           // VBD1 + 0x125
        kNkMAIDCapability_PictureControl = 0x822E,      // VBD1 + 0x12E
        kNkMAIDCapability_Active_D_Lighting = 0x8232,   // VBD1 + 0x132
        kNkMAIDCapability_LiveViewStatus = 0x823E,      // VBD1 + 0x13E
        kNkMAIDCapability_LiveViewImageZoomRate = 0x823F,// VBD1 + 0x13F
        kNkMAIDCapability_DeleteDramImage = 0x8243,     // VBD1 + 0x143
        kNkMAIDCapability_GetLiveViewImage = 0x8247,    // VBD1 + 0x147
        kNkMAIDCapability_CompressRAWEx = 0x824B,       // VBD1 + 0x14B
        kNkMAIDCapability_WBFluorescentType = 0x824D,   // VBD1 + 0x14D
        kNkMAIDCapability_PictureControlData = 0x8258,  // VBD1 + 0x158
        kNkMAIDCapability_GetPicCtrlInfo = 0x8259,      // VBD1 + 0x159
        kNkMAIDCapability_DeleteCustomPictureControl = 0x825A, // VBD1 + 0x15A
        kNkMAIDCapability_LiveViewProhibit = 0x825E,    // VBD1 + 0x15E
        kNkMAIDCapability_AutoDistortion = 0x8282,        // VBD1 + 0x182
        kNkMAIDCapability_VignetteControl = 0x826C,     // VBD1 + 0x16C
        kNkMAIDCapability_MovieScreenSize = 0x8272,     // VBD1 + 0x172
        kNkMAIDCapability_SaveMedia = 0x8305,           // VBD1 + 0x205
        kNkMAIDCapability_WBTuneColorTempEx = 0x8325,   // VBD1 + 0x225 (D7200/D7500: Range)
        kNkMAIDCapability_GetVideoImage = 0x8317,       // VBD1 + 0x217
        kNkMAIDCapability_TerminateCapture = 0x8318,    // VBD1 + 0x218
        kNkMAIDCapability_HDRMode = 0x8320,             // VBD1 + 0x220
        kNkMAIDCapability_HDRExposure = 0x8321,         // VBD1 + 0x221
        kNkMAIDCapability_HDRSmoothing = 0x8322,        // VBD1 + 0x222
        kNkMAIDCapability_MovieImageQuality = 0x8331,   // VBD1 + 0x231
        kNkMAIDCapability_MovieShutterSpeed = 0x8336,   // VBD1 + 0x236
        kNkMAIDCapability_MovieAperture = 0x8337,       // VBD1 + 0x237
        kNkMAIDCapability_MovieSensitivity = 0x8338,    // VBD1 + 0x238
        kNkMAIDCapability_MovieExposureComp = 0x8339,   // VBD1 + 0x239
        kNkMAIDCapability_LiveViewImageSize = 0x8353,   // VBD1 + 0x253
        kNkMAIDCapability_PictureControlDataEx = 0x8459,// VBD1 + 0x359
        kNkMAIDCapability_MovieWBMode = 0x8460,         // VBD1 + 0x360
        // Capabilities que não mudaram (offset < 0x100)
        kNkMAIDCapability_AFMode = 0x81c3,              // VBD1 + 0xC3
        kNkMAIDCapability_ISOAutoHiLimit = 0x81f2,      // VBD1 + 0xF2
        kNkMAIDCapability_ISOAutoShutterTime = 0x81c4,  // VBD1 + 0xC4
        kNkMAIDCapability_CompressRAW = 0x8158,         // VBD1 + 0x58
        // CaptureAsync/DeviceReady (offsets grandes, valores do SDK D7200)
        kNkMAIDCapability_CaptureAsync = 0x83d8,        // VBD1 + 0x2D8
        kNkMAIDCapability_AFCaptureAsync = 0x83d9,      // VBD1 + 0x2D9
        kNkMAIDCapability_DeviceReady = 0x83da,         // VBD1 + 0x2DA
        kNkMAIDCapability_AngleLevel = 0x8171,
        kNkMAIDCapability_FlashISOAutoHighLimit = 0x83c1,
        kNkMAIDCapability_MirrorUpStatus = 0x83f0,
        kNkMAIDCapability_MirrorUpCancel = 0x83ef,
        kNkMAIDCapability_MirrorUpReleaseShootingCount = 0x83f1,
        kNkMAIDCapability_MovieActive_D_Lighting = 0x83f2,
        kNkMAIDCapability_FlickerReductionSetting = 0x83f3,
        kNkMAIDCapability_MovieFileType = 0x83f6,
        kNkMAIDCapability_SaveCameraSetting = 0x83f7,
        kNkMAIDCapability_RawJpegImageStatus = 0x81b5,
        kNkMAIDCapability_MovRecInCardStatus = 0x8198,
        kNkMAIDCapability_MovRecInCardProhibit = 0x8199,
        kNkMAIDCapability_MovieMeteringMode = 0x8385,
        kNkMAIDCapability_ElectronicVR = 0x8398,
        kNkMAIDCapability_CaptureLV = 0x8399,
        kNkMAIDCapability_AFCaptureLV = 0x839a,
        kNkMAIDCapability_DeviceReadyLV = 0x839b,
        kNkMAIDCapability_MatrixMetering = 0x83d7,
        kNkMAIDCapability_ExpBaseHighlight = 0x8357,
        kNkMAIDCapability_ElectronicFrontCurtainShutter = 0x8358,
        kNkMAIDCapability_EVInterval = 0x8100,
        kNkMAIDCapability_BracketingVary = 0x8127,
        kNkMAIDCapability_EnableBracketing = 0x81af,
        kNkMAIDCapability_BracketingType = 0x81b0,
        kNkMAIDCapability_AEBracketingStep = 0x81b1,
        kNkMAIDCapability_RemainContinuousShooting = 0x818f,
        kNkMAIDCapability_PossibleToShoot = 0x8178,
    }

    // ModuleMode
    public enum eNkMAIDModuleMode : uint
    {
        kNkMAIDModuleMode_Browser = 0,
        kNkMAIDModuleMode_Controller = 1,
    }

    // ExposureMode
    public enum eNkMAIDExposureMode : uint
    {
        kNkMAIDExposureMode_Program = 0,
        kNkMAIDExposureMode_AperturePriority,
        kNkMAIDExposureMode_SpeedPriority,
        kNkMAIDExposureMode_Manual,
        kNkMAIDExposureMode_Disable,
        kNkMAIDExposureMode_Auto,
        kNkMAIDExposureMode_Portrait,
        kNkMAIDExposureMode_Landscape,
        kNkMAIDExposureMode_Closeup,
        kNkMAIDExposureMode_Sports,
        kNkMAIDExposureMode_NightPortrait,
        kNkMAIDExposureMode_NightView,
        kNkMAIDExposureMode_Child,
        kNkMAIDExposureMode_FlashOff,
        kNkMAIDExposureMode_Scene,
        kNkMAIDExposureMode_UserMode1,
        kNkMAIDExposureMode_UserMode2,
        kNkMAIDExposureMode_Effects,
    }

    // MeteringMode
    public enum eNkMAIDMeteringMode : uint
    {
        kNkMAIDMeteringMode_Matrix = 0,
        kNkMAIDMeteringMode_CenterWeighted,
        kNkMAIDMeteringMode_Spot,
        kNkMAIDMeteringMode_AfSpot,
        kNkMAIDMeteringMode_HighLight,
    }

    // FocusMode
    public enum eNkMAIDFocusMode : uint
    {
        kNkMAIDFocusMode_MF = 0,
        kNkMAIDFocusMode_AFs = 1,
        kNkMAIDFocusMode_AFc = 2,
        kNkMAIDFocusMode_AFa = 3,
        kNkMAIDFocusMode_AFf = 4,
    }

    // PictureControl
    public enum eNkMAIDPictureControl : uint
    {
        kNkMAIDPictureControl_Undefined = 0,
        kNkMAIDPictureControl_Standard = 1,
        kNkMAIDPictureControl_Neutral = 2,
        kNkMAIDPictureControl_Vivid = 3,
        kNkMAIDPictureControl_Monochrome = 4,
        kNkMAIDPictureControl_Portrait = 5,
        kNkMAIDPictureControl_Landscape = 6,
        kNkMAIDPictureControl_Flat = 7,
        kNkMAIDPictureControl_Auto = 8,
        kNkMAIDPictureControl_Custom1 = 201,
        kNkMAIDPictureControl_Custom2 = 202,
        kNkMAIDPictureControl_Custom3 = 203,
        kNkMAIDPictureControl_Custom4 = 204,
        kNkMAIDPictureControl_Custom5 = 205,
        kNkMAIDPictureControl_Custom6 = 206,
        kNkMAIDPictureControl_Custom7 = 207,
        kNkMAIDPictureControl_Custom8 = 208,
        kNkMAIDPictureControl_Custom9 = 209,
    }

    // LiveViewStatus
    public enum eNkMAIDLiveViewStatus : uint
    {
        kNkMAIDLiveViewStatus_OFF = 0,
        kNkMAIDLiveViewStatus_ON = 1,
    }

    // SaveMedia
    public enum eNkMAIDSaveMedia : uint
    {
        kNkMAIDSaveMedia_Card = 0,
        kNkMAIDSaveMedia_SDRAM = 1,
    }

    // JpegCompressionPolicy
    public enum eNkMAIDJpegCompressionPolicy : uint
    {
        kNkMAIDJpegCompressionPolicy_Size = 0,
        kNkMAIDJpegCompressionPolicy_Quality = 1,
    }

    // ImageColorSpace
    public enum eNkMAIDImageColorSpace : uint
    {
        kNkMAIDImageColorSpace_sRGB = 0,
        kNkMAIDImageColorSpace_AdobeRGB = 1,
    }

    // AutoDistortion
    public enum eNkMAIDAutoDistortion : uint
    {
        kNkMAIDAutoDistortion_Off = 0,
        kNkMAIDAutoDistortion_On = 1,
    }

    // VignetteControl
    public enum eNkMAIDVignetteControl : uint
    {
        kNkMAIDVignetteControl_High = 0,
        kNkMAIDVignetteControl_Normal = 1,
        kNkMAIDVignetteControl_Low = 2,
        kNkMAIDVignetteControl_Off = 3,
    }

    // FlashMode extension (inclui Off)
    public enum eNkMAIDFlashModeEx : uint
    {
        kNkMAIDFlashMode_FrontCurtain = 0,
        kNkMAIDFlashMode_RearCurtain,
        kNkMAIDFlashMode_SlowSync,
        kNkMAIDFlashMode_RedEyeReduction,
        kNkMAIDFlashMode_SlowSyncRedEyeReduction,
        kNkMAIDFlashMode_SlowSyncRearCurtain,
        kNkMAIDFlashMode_Off = 8,
    }
}
