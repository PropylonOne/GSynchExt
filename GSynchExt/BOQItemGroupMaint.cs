using System;
using PX.Data;

namespace GSynchExt
{
    public class BOQItemGroupMaint : PXGraph<BOQItemGroupMaint, BOQGroup>
    {

        public PXSelect<BOQGroup> MasterView;
        public PXSelect<BOQGroupItems,
             Where<BOQGroupItems.groupID, Equal<Current<BOQGroupItems.groupID>>>> DetailsView;

    }
}