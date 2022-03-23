using API.Helpers.Enums;

namespace API.Extensions;

public static class SexualityExtension
{
    public static string AttractedTo(this Sexuality sexuality, string gender)
    {
        switch (sexuality)
        {
            case Sexuality.Straight:
                return gender switch
                {
                    "male" => "female",
                    "female" => "male",
                    _ => "all"
                };
            case Sexuality.Gay:
                return gender switch
                {
                    "male" => "male",
                    "female" => "female",
                    _ => "all"
                };
            case Sexuality.Ace: return "all";
            case Sexuality.Bi: return "all";
            default: return "all";
        }
    }
}