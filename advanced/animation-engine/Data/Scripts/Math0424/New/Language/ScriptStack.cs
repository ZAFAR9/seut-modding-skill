using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.Language
{
    internal class ScriptStack
    {
        string stackName;
        int stackDepth;
        ScriptStack child;
        ScriptStack topStack;
        Dictionary<string, Lexer.LexerTokenValue> variables = new Dictionary<string, Lexer.LexerTokenValue>();
        Dictionary<string, Lexer.LexerTokenValue[]> functions = new Dictionary<string, Lexer.LexerTokenValue[]>();

        public ScriptStack()
        {
            topStack = this;
            stackName = "root";
        }



        public bool HasVariable(string name)
        {
            name = name.ToLower();

            if (variables.ContainsKey(name))
                return true;

            if (child != null)
                return child.HasVariable(name);

            return false;
        }

        public void AddVariable(string name, Lexer.LexerTokenValue type)
        {
            topStack.variables.Add(name.ToLower(), type);
        }

        public bool HasFunction(string name, params Lexer.LexerTokenValue[] args)
        {
            name = name.ToLower();

            if (functions.ContainsKey(name))
            {
                if (args.Length != functions[name].Length)
                    return false;

                for(int i = 0; i < args.Length; i++)
                    if (args[i] != functions[name][i])
                        return false;
                
                return true;
            }

            if (child != null)
                return child.HasVariable(name);

            return false;
        }

        public void AddFunction(string name, params Lexer.LexerTokenValue[] args)
        {
            functions.Add(name.ToLower(), args);
        }



        public void ThrowException(string reason)
        {
            throw new Exception(reason, GenerateEx());
        }

        private Exception GenerateEx()
        {
            if (child != null)
            {
                return new Exception($"Stack - {stackName}", child.GenerateEx());
            }
            return new Exception($"Stack - {stackName}");
        }

        public void PopStack()
        {
            stackDepth--;
            if (stackDepth < 0)
                throw new Exception("Stack underflowed!");
            topStack = topStack.child;
        }

        public void NewStack(string name)
        {
            stackDepth++;
            topStack = new ScriptStack();
            topStack.stackName = name;
            topStack.child = this;
        }
    }
}
