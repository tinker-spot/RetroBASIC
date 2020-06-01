using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC
{
    static public class ErrorMessages
    {
        static public string nextWithOutFor = "NEXT WITHOUT FOR";
        static public string undefinedStatement = "UNDEF'D STATEMENT";
        static public string syntax = "SYNTAX";
        static public string returnWithoutGosub = "RETURN WITHOUT GOSUB";
        static public string outOfData = "OUT OF DATA";
        static public string illegalQuantity = "ILLEGAL QUANTITY";
        static public string overflow = "OVERFLOW";
        static public string outOfMemory = "OUT OF MEMORY";
        static public string badSubscript = "BAD SUBSCRIPT";
        static public string redimdArray = "REDIM'D ARRAY";
        static public string divisionByZero = "DIVISION BY ZERO";
        static public string illegalDirect = "ILLEGAL DIRECT";
        static public string typeMismatch = "TYPE MISMATCH";
        static public string stringTooLong = "STRING TOO LONG";
        static public string fileData = "FILE DATA";
        static public string formulaTooComplex = "FORMULA TOO COMPLEX";
        static public string cantContinue = "CAN'T CONTINUE";
        static public string extraIgnored = "EXTRA IGNORED";
        static public string redoFromStart = "REDO FROM START";
        static public string error = " ERROR";
        static public string inMsg = " IN ";
        static public string errorStart = "?";
    }
}
