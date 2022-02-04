using Microsoft.Office.Interop.Excel;

namespace TAO3.Excel.Generation.Auto;

internal class AutoExcelTypeProvider : IDisposable
{
    private readonly ExcelTypeSafeGenerator _typeGenerator;
    private readonly List<IChangeAwareExcelObject> _changeAwareExcelObjects;

    public AutoExcelTypeProvider(ExcelTypeSafeGenerator typeGenerator)
    {
        _typeGenerator = typeGenerator;
        _changeAwareExcelObjects = new List<IChangeAwareExcelObject>();
    }

    public void Initialize(Application application)
    {
        _changeAwareExcelObjects.Add(new ExcelApplication(application));

        AppEvents_Event evnts = application;
        evnts.NewWorkbook += w =>
        {
            RegisterWorkbookEvents(w);
            _typeGenerator.ScheduleRefreshGeneration();
        };

        evnts.WorkbookOpen += w =>
        {
            RegisterWorkbookEvents(w);
            _typeGenerator.ScheduleRefreshGeneration();
        };

        evnts.WorkbookBeforeClose += (Workbook w, ref bool cancel) =>
        {
            _changeAwareExcelObjects.RemoveAll(x => x.COMObject == w);
            _typeGenerator.ScheduleRefreshGeneration();
        };

        evnts.WorkbookBeforeSave += (Workbook Wb, bool SaveAsUI, ref bool Cancel) => _typeGenerator.ScheduleRefreshGeneration();

        //evnts.WindowActivate += (_, _) => OnChange();
        evnts.SheetActivate += (_) => OnChange();
        evnts.WorkbookActivate += (_) => OnChange();
        evnts.AfterCalculate += () => OnChange();

        foreach (Workbook workbook in application.Workbooks)
        {
            RegisterWorkbookEvents(workbook);
            foreach (Worksheet worksheet in workbook.Worksheets)
            {
                _changeAwareExcelObjects.Add(new ExcelWorksheet(worksheet));
            }
        }

        void OnChange()
        {
            if (_typeGenerator.RefreshEnable && _changeAwareExcelObjects.Any(x => x.HasChanged()))
            {
                _typeGenerator.ScheduleRefreshGeneration();
            }
        }

        void RegisterWorkbookEvents(Workbook workbook)
        {
            _changeAwareExcelObjects.Add(new ExcelWorkbook(workbook));
            workbook.NewSheet += sheet =>
            {
                _changeAwareExcelObjects.Add(new ExcelWorksheet((Worksheet)sheet));
                _typeGenerator.ScheduleRefreshGeneration();
            };

            workbook.SheetBeforeDelete += sheet =>
            {
                _changeAwareExcelObjects.RemoveAll(x => x.COMObject == sheet);
                _typeGenerator.ScheduleRefreshGeneration();
            };
        }
    }

    public void Dispose()
    {
        _changeAwareExcelObjects.Clear();
    }

    private interface IChangeAwareExcelObject
    {
        object COMObject { get; }
        bool HasChanged();
    }

    private class ExcelApplication : IChangeAwareExcelObject
    {
        public object COMObject { get; }

        public ExcelApplication(Application application)
        {
            COMObject = application;
        }

        public bool HasChanged()
        {
            return false;
        }
    }

    private class ExcelWorkbook : IChangeAwareExcelObject
    {
        private readonly Workbook _workbook;
        private string _name;

        public ExcelWorkbook(Workbook workbook)
        {
            _workbook = workbook;
            _name = workbook.Name;
        }

        public object COMObject => _workbook;

        public bool HasChanged()
        {
            if (_workbook.Name != _name)
            {
                _name = _workbook.Name;
                return true;
            }

            return false;
        }
    }

    private class ExcelWorksheet : IChangeAwareExcelObject
    {
        private readonly Worksheet _worksheet;
        private string _name;
        private List<ExcelTable> _tables;

        public ExcelWorksheet(Worksheet worksheet)
        {
            _worksheet = worksheet;
            _name = worksheet.Name;
            _tables = worksheet.ListObjects
                .Cast<ListObject>()
                .Select(x => new ExcelTable(x))
                .ToList();
        }

        public object COMObject => _worksheet;

        public bool HasChanged()
        {
            if (_worksheet.ListObjects.Count != _tables.Count)
            {
                _tables = _worksheet.ListObjects
                    .Cast<ListObject>()
                    .Select(x => new ExcelTable(x))
                    .ToList();

                return true;
            }

            if (_worksheet.Name != _name)
            {
                _name = _worksheet.Name;
                return true;
            }

            return _tables.Any(x => x.HasChanged());
        }
    }

    private class ExcelTable : IChangeAwareExcelObject
    {
        private readonly ListObject _listObject;
        private string _name;
        private List<string> _columns;

        public ExcelTable(ListObject listObject)
        {
            _listObject = listObject;
            _name = listObject.Name;
            _columns = listObject.ListColumns
                .Cast<ListColumn>()
                .Select(x => x.Name)
                .ToList();
        }

        public object COMObject => _listObject;

        public bool HasChanged()
        {
            if (_listObject.Name != _name)
            {
                _name = _listObject.Name;
                return true;
            }

            List<string> currentColumns = _listObject.ListColumns
                .Cast<ListColumn>()
                .Select(x => x.Name)
                .ToList();

            if (!currentColumns.SequenceEqual(_columns))
            {
                _columns = currentColumns;
                return true;
            }

            return false;
        }
    }
}
