using System;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.API.Libraries
{
    internal class StringAPI : AEAPI
    {
        public LibraryMethod[] GetMethods()
        {
            return new LibraryMethod[]
            {
                new LibraryMethod("concatenate", (Func<string, object, string>)Concatenate, VariableType.STRING, VariableType.STRING, VariableType.STRING),
                new LibraryMethod("concatenate", (Func<string, object, string>)Concatenate, VariableType.STRING, VariableType.STRING, VariableType.INT),
                new LibraryMethod("concatenate", (Func<string, object, string>)Concatenate, VariableType.STRING, VariableType.STRING, VariableType.FLOAT),
                new LibraryMethod("concatenate", (Func<string, object, string>)Concatenate, VariableType.STRING, VariableType.STRING, VariableType.BOOL),

                new LibraryMethod("substring", (Func<string, int, string>)Substring, VariableType.STRING, VariableType.STRING, VariableType.INT),
                new LibraryMethod("substring", (Func<string, int, int, string>)Substring, VariableType.STRING, VariableType.STRING, VariableType.INT, VariableType.INT),
                
                new LibraryMethod("tostring", (Func<object, string>)ToString, VariableType.STRING, VariableType.STRING),
                new LibraryMethod("tostring", (Func<object, string>)ToString, VariableType.STRING, VariableType.INT),
                new LibraryMethod("tostring", (Func<object, string>)ToString, VariableType.STRING, VariableType.FLOAT),
                new LibraryMethod("tostring", (Func<object, string>)ToString, VariableType.STRING, VariableType.BOOL),
                new LibraryMethod("tostring", (Func<object[], string>)ToString, VariableType.STRING, VariableType.ARRAY),
            };
        }

        private string ToString(object[] arr)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                str.Append($"{arr[i].ToString()}, ");
            }
            return str.Remove(str.Length - 3, 2).ToString();
        }

        private string ToString(object obj)
        {
            return obj.ToString();
        }

        private string Substring(string str, int start)
        {
            return str.Substring(start);
        }

        private string Substring(string str, int start, int length)
        {
            return str.Substring(start, length);
        }

        private string Concatenate(string str, object obj)
        {
            return str + obj.ToString();
        }

    }
}
