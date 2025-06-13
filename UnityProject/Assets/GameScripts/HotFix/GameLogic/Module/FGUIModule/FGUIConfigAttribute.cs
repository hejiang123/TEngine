using System;

namespace GameLogic
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FGUIConfigAttribute : Attribute
    {
        public string PackageId { get; set; }
        public string[] DependentPackageIds { get; set; }
    }
}