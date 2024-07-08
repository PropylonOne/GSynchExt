using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt.Descriptor
{
    public static class SSSurveyListAttributes
    {

        public const string SSSBuildingCondition1 = "BC1";
        public const string SSSBuildingCondition2 = "BC2";
        public const string SSSBuildingCondition3 = "BC3";
        public const string SSSRoofMaterial1 = "RM1";
        public const string SSSRoofMaterial2 = "RM2";
        public const string SSSRoofMaterial3 = "RM3";
        public const string SSSRoofMaterial4 = "RM4";
        public const string SSSRoofCondition1 = "RC1";
        public const string SSSRoofCondition2 = "RC2";
        public const string SSSRoofCondition3 = "RC3";
        public const string SSSRoofCondition4 = "RC4";
        public const string SSSRepairPercentage1 = "RP1";
        public const string SSSRepairPercentage2 = "RP2";
        public const string SSSRepairPercentage3 = "RP3";
        public const string SSSOrientation1 = "O1";
        public const string SSSOrientation2 = "O2";
        public const string SSSOrientation3 = "O3";
        public const string SSSOrientation4 = "O4";

        public class SSSurveyBuildingConditionAttribute : PXStringListAttribute
        {
            public SSSurveyBuildingConditionAttribute() : base(GetSSSBuildingConditionAttribute)
            {

            }

            public static (string, string)[] GetSSSBuildingConditionAttribute => new[] {
                (SSSBuildingCondition1, Messages.SSSBuildingCondition1),
                (SSSBuildingCondition2, Messages.SSSBuildingCondition2),
                (SSSBuildingCondition3, Messages.SSSBuildingCondition3)
            };
        }

        public class SSSurveyrRoofConditionAttribute : PXStringListAttribute
        {
            public SSSurveyrRoofConditionAttribute() : base(GetSSSSRoofConditionAttribute)
            {

            }

            public static (string, string)[] GetSSSSRoofConditionAttribute => new[] {
                (SSSRoofCondition1, Messages.SSSRoofCondition1),
                (SSSRoofCondition2, Messages.SSSRoofCondition2),
                (SSSRoofCondition3, Messages.SSSRoofCondition3),
                (SSSRoofCondition4, Messages.SSSRoofCondition4)
            };
        }

        public class SSSurveyrRoofMaterialAttribute : PXStringListAttribute
        {
            public SSSurveyrRoofMaterialAttribute() : base(GetSSSSRoofMaterialAttribute)
            {

            }

            public static (string, string)[] GetSSSSRoofMaterialAttribute => new[] {
                (SSSRoofMaterial1, Messages.SSSRoofMaterial1),
                (SSSRoofMaterial2, Messages.SSSRoofMaterial2),
                (SSSRoofMaterial3, Messages.SSSRoofMaterial3),
                (SSSRoofMaterial4, Messages.SSSRoofMaterial4)
            };
        }

        public class SSSurveyrRepairPercentageAttribute : PXStringListAttribute
        {
            public SSSurveyrRepairPercentageAttribute() : base(GetSSSSRepairPercentageAttribute)
            {

            }

            public static (string, string)[] GetSSSSRepairPercentageAttribute => new[] {
                (SSSRepairPercentage1, Messages.SSSRepairPercentage1),
                (SSSRepairPercentage2, Messages.SSSRepairPercentage2),
                (SSSRepairPercentage3, Messages.SSSRepairPercentage3)
            };
        }

        public class SSSurveyrOrientationAttribute : PXStringListAttribute
        {
            public SSSurveyrOrientationAttribute() : base(GetSSSSOrientationAttribute)
            {

            }

            public static (string, string)[] GetSSSSOrientationAttribute => new[] {
                (SSSOrientation1, Messages.SSSOrientation1),
                (SSSOrientation2, Messages.SSSOrientation2),
                (SSSOrientation3, Messages.SSSOrientation3),
                (SSSOrientation4, Messages.SSSOrientation4)
            };
        }


    }
}
