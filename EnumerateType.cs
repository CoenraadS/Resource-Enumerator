using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Resource_Enumerator
{
    public static class EnumerateType
    {
        private static List<string> resourceNames;

        private delegate bool EnumResNameDelegate(
       IntPtr hModule,
       IntPtr lpszType,
       IntPtr lpszName,
       IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "EnumResourceNamesW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool EnumResourceNamesWithID(IntPtr hModule, uint lpszType, EnumResNameDelegate lpEnumFunc, IntPtr lParam);

        public static List<string> getResourceList(IntPtr dataFilePointer, Resource type)
        {
            resourceNames = new List<string>();
            EnumResourceNamesWithID(dataFilePointer, (uint)type, new EnumResNameDelegate(EnumRes), IntPtr.Zero);
            
            return resourceNames;
        }

        #region Enum Functions

        private static bool IS_INTRESOURCE(IntPtr value)
        {
            if (((uint)value) > ushort.MaxValue)
                return false;
            return true;
        }

        private static string GET_RESOURCE_NAME(IntPtr value)
        {
            if (IS_INTRESOURCE(value) == true)
                return value.ToString();
            return Marshal.PtrToStringUni((IntPtr)value);
        }

        private static bool EnumRes(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam)
        {
            resourceNames.Add(GET_RESOURCE_NAME(lpszName));
            return true;
        }

        #endregion
    }
}
