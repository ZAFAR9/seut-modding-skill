using AnimationEngine.LanguageV2.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.API.Libraries
{
    internal class BaseAPI : AEAPI
    {
        public LibraryMethod[] GetMethods()
        {
            return new LibraryMethod[] {
                new LibraryMethod("log", (Action<object>)Log, VariableType.NONE, VariableType.STRING),
                new LibraryMethod("log", (Action<object>)Log, VariableType.NONE, VariableType.INT),
                new LibraryMethod("log", (Action<object>)Log, VariableType.NONE, VariableType.FLOAT),
                new LibraryMethod("log", (Action<object>)Log, VariableType.NONE, VariableType.BOOL),
                new LibraryMethod("log", (Action<object>)Log, VariableType.NONE, VariableType.POINTER),
                new LibraryMethod("log", (Action<object[]>)Log, VariableType.NONE, VariableType.ARRAY),

                new LibraryMethod("assert", (Action<int, int>)Assert, VariableType.NONE, VariableType.INT, VariableType.INT),
            };
        }

        public void Assert(int a, int b)
        {
            if (a != b)
                throw new Exception($"Assertion failure, {a} != {b}");
        }

        public void Log(object value)
        {
            Logging.Info(value);
        }

        public void Log(object[] value)
        {
            Logging.Info("ARRAY LOG");
            Logging.IncreaseIndent();
            foreach (var x in value)
                Logging.Info(x);
            Logging.DecreaseIndent();
        }

    }
}
