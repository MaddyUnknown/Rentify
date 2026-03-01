using Rentify.Core.Enums;

namespace Rentify.API.Enums
{
    // Hack to accept snake case enum from query parameters
    public enum MediaFileVariantDtoEnum
    {
        thumbnail = 1,
        cover_pic = 2,
        profile_pic = 3
    }

    public static class MediaFileVariantDtoEnumConverter
    {
        public static MediaFileVariantEnum ToMediaFilVariantEnum(MediaFileVariantDtoEnum variantEnum)
        {
            switch(variantEnum)
            {
                case MediaFileVariantDtoEnum.thumbnail: return MediaFileVariantEnum.Thumbnail;
                case MediaFileVariantDtoEnum.cover_pic: return MediaFileVariantEnum.CoverPic;
                case MediaFileVariantDtoEnum.profile_pic: return MediaFileVariantEnum.ProfilePic;
                default: return MediaFileVariantEnum.None;
            }
        }
    }
}
