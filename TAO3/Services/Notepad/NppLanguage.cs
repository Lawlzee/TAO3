using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Notepad
{
    //https://github.com/notepad-plus-plus/notepad-plus-plus/blob/master/PowerEditor/src/MISC/PluginsManager/Notepad_plus_msgs.h
    public enum NppLanguage
    {
        TEXT,
        PHP,
        C,
        CPP,
        CSHARP,
        OBJC,
        JAVA,
        RESOURCEFILE,
        HTML,
        XML,
        MAKEFILE,
        PASCAL,
        BATCH,
        INI,
        ASCII,
        //USER,
        ASP = 16,
        SQL,
        VB,
        // Don't use L_JS, use L_JAVASCRIPT instead
        //JS, 
        CSS = 20,
        PERL,
        PYTHON,
        LUA,
        TEX,
        FORTRAN,
        BASH,
        FLASH,
        NSIS,
        TCL,
        LISP,
        SCHEME,
        ASM,
        DIFF,
        PROPS,
        POSTSCRIPT,
        RUBY,
        SMALLTALK,
        VHDL,
        KIX,
        AUTOIT,
        CAML,
        ADA,
        VERILOG,
        MATLAB,
        HASKELL,
        INNO,
        //SEARCHRESULT,
        CMAKE = 48,
        YAML,
        COBOL,
        GUI4CLI,
        D,
        POWERSHELL,
        R,
        JSP,
        COFFEESCRIPT,
        JSON,
        JAVASCRIPT,
        FORTRAN_77,
        // The end of enumated language type, so it should be always at the end
        EXTERNAL
    }
}
