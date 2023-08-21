using DotNetCommon.Extensions;
using System.Runtime.Serialization;

namespace DotNetCommon.Bases
{
    public class ValidatingObject
    {//Not complete. Can implement as needed.
        public bool IsValid() => this.RequiredAttributesSatisfied();

        public void AssertValid() { if (!this.IsValid()) throw new InvalidDataContractException(this.GetType().Name + " object does not have all required attributes"); }
    }
}
