using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.API
{

    public enum VariableType
    {
        NONE,
        STRING,
        BOOL,
        INT,
        FLOAT,
        ARRAY,
        POINTER,
    }

    public struct LibraryMethod
    {
        public LibraryMethod(string name, Delegate method, VariableType returnType, params VariableType[] args)
        {
            Name = name;
            Method = method;
            ReturnType = returnType;
            ArgTypes = args;
        }
        public string Name;
        public Delegate Method;
        public VariableType ReturnType;
        public VariableType[] ArgTypes;
    }

    public struct FunctionHeader
    {
        public FunctionHeader(string name, Delegate method, params VariableType[] args)
        {
            this.Name = name;
            this.Method = method;
            this.Args = args;
        }
        public string Name;
        public Delegate Method;
        public VariableType[] Args;
    }

}
