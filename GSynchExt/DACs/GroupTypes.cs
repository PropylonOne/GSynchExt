using System.Diagnostics.CodeAnalysis;
using PX.Data;
using PX.Data.BQL;

namespace GSynchExt
{
    public static class GroupTypes
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base( PXStringListAttribute.Pair("SSITE", "Solar Site"), PXStringListAttribute.Pair("SSURVEY", "Solar Site Survey"))
            {
            }


        }

        public class SSurveyType : BqlType<IBqlString, string>.Constant<SSurveyType>
        {
            public SSurveyType()
                : base("SSURVEY")
            {
            }
        }

        public class SSiteType : BqlType<IBqlString, string>.Constant<SSiteType>
        {

            public SSiteType()
              : base("SSITE")
            {
            }
        }

        public const string SolarSiteSurvey = "SSURVEY";
        public const string SolarSite = "SSITE";

    }
}
