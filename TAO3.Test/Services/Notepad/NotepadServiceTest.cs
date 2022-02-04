using Microsoft.VisualStudio.TestTools.UnitTesting;
using TAO3.Notepad;

namespace TAO3.Test.Services.Notepad;

[TestClass]
public class NotepadServiceTest
{
    //[TestMethod]
    public void Test()
    {
        INotepadService notepadService = new NotepadService();
        string[] files = notepadService.Tabs;
    }
}
