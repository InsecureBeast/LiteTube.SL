using Google.Apis.YouTube.v3;

namespace LiteTube.Core.DataClasses
{
    public enum RatingEnum
    {
        Like,
        Dislike,
        None
    }

    static class RatingEnumEx
    {
        public static VideosResource.RateRequest.RatingEnum GetYTRating(this RatingEnum ratingEnum)
        {
            if (ratingEnum == RatingEnum.Like)
                return VideosResource.RateRequest.RatingEnum.Like;
            if (ratingEnum == RatingEnum.Dislike)
                return VideosResource.RateRequest.RatingEnum.Dislike;

            return VideosResource.RateRequest.RatingEnum.None;
        }

        public static RatingEnum GetRating(this string ytEnum)
        {
            if (ytEnum == "like")
                return RatingEnum.Like;
            if (ytEnum == "dislike")
                return RatingEnum.Dislike;
            return RatingEnum.None;
        }
    }
}