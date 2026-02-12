namespace PicStonePlus.SDK
{
    public static class MaidConstants
    {
        public const uint kNkMAIDCapability_VendorBase = 0x8000;
        public const uint kNkMAIDCapability_VendorBaseDX2 = kNkMAIDCapability_VendorBase + 0x100;
        public const int kNkMAIDResult_DX2Origin = 127 + 129; // kNkMAIDResult_VendorBase + 129
        public const int kNkMAIDEvent_DX2Origin = 6 + 0x100; // kNkMAIDEvent_CapChangeValueOnly + 0x100

        // Live View header size
        public const int LiveViewHeaderSize = 384;

        // Tamanho máximo de descrição de capability
        public const int MaxDescriptionLength = 256;

        // Tamanho máximo de string
        public const int MaxStringLength = 256;
    }
}
