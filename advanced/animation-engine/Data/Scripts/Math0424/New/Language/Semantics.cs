using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnimationEngine.Data.Scripts.Math0424.New.Language
{
    internal class Semantics
    {
        public static void Analyze(AST ast)
        {
            ScriptStack stack = new ScriptStack();
            foreach(var x in ast.Root.Children)
            {
                switch(x.Type)
                {
                    case Lexer.LexerTokenValue.USING:
                        VisitUsing(stack, x);
                        break;
                    case Lexer.LexerTokenValue.VARIABLE:
                    case Lexer.LexerTokenValue.UNKNOWN_VARIABLE:
                        VisitGlobalVariable(stack, x);
                        break;
                    case Lexer.LexerTokenValue.FUNC:
                        VisitFunction(stack, x);
                        break;
                    case Lexer.LexerTokenValue.STRUCT:
                        VisitStruct(stack, x);
                        break;
                }
            }
        }

        private static void VisitStruct(ScriptStack stack, ASTNode node)
        {

        }

        private static void VisitUsing(ScriptStack stack, ASTNode node)
        {
            
        }

        private static void VisitGlobalVariable(ScriptStack stack, ASTNode node)
        {
            if (stack.HasVariable((string)node.Value))
                stack.ThrowException($"Duplicate global variable at {node}");

            if (node.Type == Lexer.LexerTokenValue.UNKNOWN_VARIABLE)
            {
                var nonHeader = FirstNonHeaderChild(node);
                if (!nonHeader.HasValue || !nonHeader.Value.Type.IsLiteralVariable())
                    stack.ThrowException($"Global variables must be literals {node}");

                stack.AddVariable((string)node.Value, nonHeader.Value.Type);
            }
            else
            {
                var nonHeader = node.Children.Last();
                if (!nonHeader.Type.IsLiteralVariable())
                    stack.ThrowException($"Global variables must be literals {node}");

                stack.AddVariable((string)node.Value, nonHeader.Type);
            }
        }

        private static void VisitFunction(ScriptStack stack, ASTNode node)
        {
            //stack.AddFunction((string)node.Value,);
        }

        private static ASTNode? FirstChild(ASTNode node, Lexer.LexerTokenValue value)
        {
            foreach (var x in node.Children)
                if (x.Type == value)
                    return x;
            return null;
        }

        private static ASTNode? FirstNonHeaderChild(ASTNode node)
        {
            foreach (var x in node.Children)
                if (x.Type != Lexer.LexerTokenValue.HEADER)
                    return x;
            return null;
        }

    }
}
