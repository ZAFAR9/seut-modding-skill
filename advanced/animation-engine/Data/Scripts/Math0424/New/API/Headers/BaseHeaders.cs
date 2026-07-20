using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.API.Headers
{
    internal class BaseHeaders : AEHeader
    {
        public FunctionHeader[] GetHeaders()
        {
            return new FunctionHeader[]
            {
                new FunctionHeader("loop", (Action)Dummy, VariableType.INT),

                new FunctionHeader("functional", (Action)Dummy, VariableType.BOOL),
                new FunctionHeader("working", (Action)Dummy, VariableType.BOOL),
                new FunctionHeader("powered", (Action)Dummy, VariableType.BOOL),

                new FunctionHeader("terminalslider", (Action)Dummy, VariableType.STRING, VariableType.FLOAT, VariableType.FLOAT),
                new FunctionHeader("terminaltoggle", (Action)Dummy, VariableType.STRING, VariableType.BOOL),
                new FunctionHeader("terminalbutton", (Action)Dummy, VariableType.STRING),

                new FunctionHeader("sync", (Action)Dummy, VariableType.BOOL, VariableType.BOOL),
                new FunctionHeader("listener", (Action)Dummy, VariableType.STRING),
            };
        }

        private void Dummy()
        {

        }

    }
}
