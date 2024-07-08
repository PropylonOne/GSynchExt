#region Assembly PX.Objects, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// C:\Program Files\Acumatica ERP\GAIA\Bin\PX.Objects.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;

namespace GSynchExt
{
    [PXDBString(9, InputMask = ">CCCCCCCCC", IsUnicode = true)]
    [PXUIField(DisplayName = "Site ID", Visibility = PXUIVisibility.SelectorVisible)]
    public sealed class SiteRawAttribute : AcctSubAttribute
    {
        public const string DimensionName = "SOLARSITE";

        private System.Type _whereType;

        public SiteRawAttribute()
        {
            System.Type typeFromHandle = typeof(Search<SolarSite.solarSiteCD>);
            PXDimensionSelectorAttribute item = new PXDimensionSelectorAttribute("SOLARSITE", typeFromHandle, typeof(SolarSite.solarSiteCD), typeof(SolarSite.siteStatus))
            {
                DescriptionField = typeof(SolarSite.siteName),
                CacheGlobal = true
            };
            _Attributes.Add(item);
            _SelAttrIndex = _Attributes.Count - 1;
        }

        public SiteRawAttribute(System.Type WhereType)
            : this()
        {
            if (WhereType != null)
            {
                _whereType = WhereType;
                System.Type type = BqlCommand.Compose(typeof(Search<,>), typeof(SolarSite.solarSiteCD), typeof(And<>), _whereType);
                PXDimensionSelectorAttribute value = new PXDimensionSelectorAttribute("SOLARSITE", type, typeof(SolarSite.solarSiteCD), typeof(SolarSite.siteStatus))
                {
                    DescriptionField = typeof(SolarSite.siteName),
                    CacheGlobal = true
                };
                _Attributes[_SelAttrIndex] = value;
            }
        }
    }
}

