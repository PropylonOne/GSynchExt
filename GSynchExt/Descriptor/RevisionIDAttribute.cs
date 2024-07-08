using PX.Data;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSynchExt.Descriptor
{
    /// <summary>
    /// Revision field attribute
    /// (Length 10)
    /// </summary>
    [PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCC")]
    [PXUIField(DisplayName = "BoQ Revision")]
    public class RevisionIDFieldAttribute : AcctSubAttribute
    {
    }
}
