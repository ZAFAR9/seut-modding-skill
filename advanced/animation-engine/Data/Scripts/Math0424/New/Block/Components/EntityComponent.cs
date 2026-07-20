using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.Block.Components
{
    internal abstract class EntityComponent
    {
        public abstract object ExecuteInstruction(string name, object[] args);
        public abstract object GetProperty(string name);
        public abstract object SetProperty(string name, object value);
    }
}
