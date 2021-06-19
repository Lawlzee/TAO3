using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TAO3.Excel.Generation;
using TAO3.Excel.Generation.Auto;

namespace TAO3.Excel
{
    public interface IExcelService : IDisposable
    {
        public dynamic Instance { get; }
        IReadOnlyList<ExcelWorkbook> Workbooks { get; }
        ExcelWorkbook Open(string path, bool refreshTypes = true);
        void RefreshTypes();
    }

    internal class ExcelService : IExcelService
    {
        internal ExcelTypeSafeGenerator TypeGenerator { get; }
        private readonly AutoExcelTypeProvider _autoExcelTypeProvider;
        
        private Application? _application = null;
        internal Application Application => _application ??= GetExcel();
        public dynamic Instance => Application;

        public IReadOnlyList<ExcelWorkbook> Workbooks => Application.Workbooks
            .Cast<Workbook>()
            .Select(x => new ExcelWorkbook(TypeGenerator, x))
            .ToList();

        public ExcelService(CSharpKernel kernel)
        {
            TypeGenerator = new ExcelTypeSafeGenerator(kernel, this);
            _autoExcelTypeProvider = new AutoExcelTypeProvider(TypeGenerator);
        }

        public ExcelWorkbook Open(string path, bool refreshTypes = true)
        {
            _application ??= GetOrOpenExcel();

            return TypeGenerator.ScheduleRefreshGenerationAfter(refreshTypes, () => new ExcelWorkbook(TypeGenerator, Application.Workbooks.Open(path)));
        }

        public void RefreshTypes()
        {
            TypeGenerator.ScheduleRefreshGeneration();
        }

        private Application GetExcel()
        {
            try
            {
                Application application = (Application)GetActiveObject("Excel.Application");
                _autoExcelTypeProvider.Initialize(application);
                return application;
            }
            catch
            {
                throw new Exception("Excel is closed");
            }   
        }

        private Application GetOrOpenExcel()
        {
            Application application = null!;
            try
            {
                application = (Application)GetActiveObject("Excel.Application");
            }
            catch
            {
                application = new Application()
                {
                    Visible = true
                };
            }
            _autoExcelTypeProvider.Initialize(application);
            return application;
        }

        //https://stackoverflow.com/questions/58010510/no-definition-found-for-getactiveobject-from-system-runtime-interopservices-mars
        [SecurityCritical]  // auto-generated_required
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
        [SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport("ole32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLEAUT32, PreserveSig = false)]
        [DllImport("oleaut32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]  // auto-generated
        private static extern void GetActiveObject(ref Guid rclsid, IntPtr reserved, [MarshalAs(UnmanagedType.Interface)] out object ppunk);

        //https://stackoverflow.com/questions/158706/how-do-i-properly-clean-up-excel-interop-objects
        public void Dispose()
        {
            //todo: Close excel application?
            _autoExcelTypeProvider.Dispose();
            _application = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
