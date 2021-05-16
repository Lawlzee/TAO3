using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Services.Notepad;

namespace TAO3.Test.Services.Notepad
{
    [TestClass]
    public class NotepadServiceTest
    {
        [TestMethod]
        public void Test()
        {
            INotepadService notepadService = new NotepadService();
            string[] files = notepadService.Tabs;
        }
    }
}
