using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Services
{
    internal interface IExcelService
    {
        
    }

    public class ExcelService
    {
        public dynamic GetOrOpenExcel2()
        {
            try
            {
                return GetActiveObject("Excel.Application");
            }
            catch (Exception)//Excel not open
            {
                return new Application().Workbooks;
            }
        }

        public Application GetOrOpenExcel()
        {
            try
            {
                return (Application)GetActiveObject("Excel.Application");
            }
            catch (Exception)//Excel not open
            {
                return new Application();
            }
            Application application;
            Worksheet worksheet = (Worksheet)application.Workbooks[""].Sheets[0];
            worksheet.UsedRange.Table(null, null);
        }

        public Workbook OpenWorkbook(Application application, string name)
        {
            return application.Workbooks[name];
        }

        /*public string[] T(Application application)
        {
            Worksheet worksheet = (Worksheet)application.Workbooks[""].Sheets[0];
            worksheet.UsedRange.Table(null, null);
        }*/

        //https://stackoverflow.com/questions/58010510/no-definition-found-for-getactiveobject-from-system-runtime-interopservices-mars
        [System.Security.SecurityCritical]  // auto-generated_required
        private object GetActiveObject(string progID)
        {
            Guid clsid;

            // Call CLSIDFromProgIDEx first then fall back on CLSIDFromProgID if
            // CLSIDFromProgIDEx doesn't exist.
            try
            {
                CLSIDFromProgIDEx(progID, out clsid);
            }
            catch (Exception)
            {
                CLSIDFromProgID(progID, out clsid);
            }

            GetActiveObject(ref clsid, IntPtr.Zero, out object obj);
            return obj;
        }

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport("ole32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport("ole32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLEAUT32, PreserveSig = false)]
        [DllImport("oleaut32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void GetActiveObject(ref Guid rclsid, IntPtr reserved, [MarshalAs(UnmanagedType.Interface)] out object ppunk);
    }
}
