// Guids.cs
// MUST match guids.h
using System;

namespace bukachacha.vsplot
{
    static class GuidList
    {
        public const string guidvsplotPkgString = "8672e1d4-4dcd-4935-b491-478fe1a95b5c";
        public const string guidvsplotCmdSetString = "f31834f3-6862-44ec-bb10-49f62ba8d59d";
        public const string guidToolWindowPersistanceString = "becf3777-4750-4ab5-b11f-8a18fa7b27f5";

        //public const string guidVSDebugGroupString = "8cc62443-0c88-4f48-b02e-7724f37cff5a";

        public static readonly Guid guidvsplotPkg = new Guid(guidvsplotPkgString);
        public static readonly Guid guidvsplotCmdSet = new Guid(guidvsplotCmdSetString);
        public static readonly Guid guidToolWindowPersistance = new Guid(guidToolWindowPersistanceString);
       // public static readonly Guid guidVSDebugGroup = new Guid(guidVSDebugGroupString);
    };
}