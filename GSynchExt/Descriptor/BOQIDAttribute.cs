using PX.Data;
using PX.Objects.GL;

namespace GSynchExt.Descriptor
{
    /// <summary>
    /// Standard BOQ ID field attribute
    /// The BOQ ID represent the project ID
    /// </summary>
    [PXDBInt()]
    [PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.SelectorVisible)]
    public class BOQIDAttribute : AcctSubAttribute
    {

    }
}
