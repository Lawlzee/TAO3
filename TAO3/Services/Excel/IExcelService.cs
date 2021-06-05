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

        private Application? _application = null;
        internal Application Application => _application ??= GetOrOpenExcel();
        public dynamic Instance => Application;

        public IReadOnlyList<ExcelWorkbook> Workbooks => Application.Workbooks
            .Cast<Workbook>()
            .Select(x => new ExcelWorkbook(TypeGenerator, x))
            .ToList();

        public ExcelService(CSharpKernel kernel)
        {
            TypeGenerator = new ExcelTypeSafeGenerator(kernel, this);
        }

        public ExcelWorkbook Open(string path, bool refreshTypes = true)
        {
            ExcelWorkbook workbook = new ExcelWorkbook(TypeGenerator, Application.Workbooks.Open(path));
            if (refreshTypes)
            {
                TypeGenerator.ScheduleRefreshGeneration();
            }
            return workbook;
        }

        public void RefreshTypes()
        {
            TypeGenerator.ScheduleRefreshGeneration();
        }

        private Application GetOrOpenExcel()
        {
            Application application;

            try
            {
                application = (Application)GetActiveObject("Excel.Application");
            }
            catch (Exception)//Excel not open
            {
                application = new Application();
            }

            RegistedRefreshEvents(application);
            return application;
        }

        private void RegistedRefreshEvents(Application application)
        {
            AppEvents_Event evnts = application;
            evnts.NewWorkbook += w =>
            {
                RegisterWorkbookEvents(w);
                TypeGenerator.ScheduleRefreshGeneration();
            };

            evnts.WorkbookOpen += w =>
            {
                RegisterWorkbookEvents(w);
                TypeGenerator.ScheduleRefreshGeneration();
            };

            evnts.WorkbookBeforeClose += (Workbook w, ref bool cancel) =>
            {
                TypeGenerator.ScheduleRefreshGeneration();
            };

            foreach (Workbook workbook in application.Workbooks)
            {
                RegisterWorkbookEvents(workbook);
            }

            void RegisterWorkbookEvents(Workbook workbook)
            {
                workbook.NewSheet += sheet => TypeGenerator.ScheduleRefreshGeneration();
                workbook.SheetBeforeDelete += sheet => TypeGenerator.ScheduleRefreshGeneration();
            }
        }

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

        //https://stackoverflow.com/questions/158706/how-do-i-properly-clean-up-excel-interop-objects
        public void Dispose()
        {
            //todo: Close excel application?

            _application = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
