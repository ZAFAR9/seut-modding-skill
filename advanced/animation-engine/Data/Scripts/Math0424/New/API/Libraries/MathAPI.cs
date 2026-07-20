using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.API.Libraries
{
    internal class MathAPI : AEAPI
    {
        public LibraryMethod[] GetMethods()
        {
            return new LibraryMethod[] {
                new LibraryMethod("floor", (Func<float, int>)Floor, VariableType.INT, VariableType.FLOAT),
                new LibraryMethod("ceil", (Func<float, int>)Ceil, VariableType.INT, VariableType.FLOAT),
                new LibraryMethod("round", (Func<float, int>)Round, VariableType.INT, VariableType.FLOAT),

                new LibraryMethod("abs", (Func<float, float>)Abs, VariableType.FLOAT, VariableType.FLOAT),
                new LibraryMethod("abs", (Func<int, int>)Abs, VariableType.INT, VariableType.INT),

                new LibraryMethod("min", (Func<float, float, float>)Min, VariableType.FLOAT, VariableType.FLOAT, VariableType.FLOAT),
                new LibraryMethod("max", (Func<float, float, float>)Max, VariableType.FLOAT, VariableType.FLOAT, VariableType.FLOAT),

                new LibraryMethod("min", (Func<int, int, int>)Min, VariableType.INT, VariableType.INT, VariableType.INT),
                new LibraryMethod("max", (Func<int, int, int>)Max, VariableType.INT, VariableType.INT, VariableType.INT),

                new LibraryMethod("sin", (Func<float, float>)Sin, VariableType.FLOAT, VariableType.FLOAT),
            };
        }

        private int Floor(float val)
        {
            return (int)Math.Floor(val);
        }

        private int Ceil(float val)
        {
            return (int)Math.Ceiling(val);
        }

        private int Round(float val)
        {
            return (int)Math.Round(val);
        }

        private float Abs(float val)
        {
            return Math.Abs(val);
        }

        private int Abs(int val)
        {
            return Math.Abs(val);
        }

        private int Min(int valA, int valB)
        {
            return Math.Min(valA, valB);
        }

        private int Max(int valA, int valB)
        {
            return Math.Max(valA, valB);
        }

        private float Min(float valA, float valB)
        {
            return Math.Min(valA, valB);
        }

        private float Max(float valA, float valB)
        {
            return Math.Max(valA, valB);
        }

        private float Sin(float val)
        {
            return (float)Math.Sin(val);
        }
    }
}
