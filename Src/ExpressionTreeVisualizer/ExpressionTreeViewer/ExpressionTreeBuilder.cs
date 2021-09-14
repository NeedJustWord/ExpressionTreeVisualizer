using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTreeViewer
{
    public static class ExpressionTreeBuilder
    {
        public static ExpressionTreeNode GetExpressionTreeNode(Expression expression, string prefix = null)
        {
            ExpressionTreeNode node = null;
            if (expression is BinaryExpression)
            {
                var expr = expression as BinaryExpression;
                node = new ExpressionTreeNode(string.Format("BinaryExpression: [{0}-{1}]", expr.NodeType, GetTypeName(expr.Type)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Left, "Left"));
                node.Nodes.Add(GetExpressionTreeNode(expr.Right, "Right"));
            }
            else if (expression is BlockExpression)
            {
                var expr = expression as BlockExpression;
                node = new ExpressionTreeNode(string.Format("BlockExpression Expressions:"));
                node.Nodes.AddRange(expr.Expressions.Select(a => GetExpressionTreeNode(a)));
            }
            else if (expression is ConditionalExpression)
            {
                var expr = expression as ConditionalExpression;
                node = new ExpressionTreeNode(string.Format("ConditionalExpression: [{0}]", GetTypeName(expr.Type)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Test, "Test"));
                node.Nodes.Add(GetExpressionTreeNode(expr.IfTrue, "IfTrue"));
                node.Nodes.Add(GetExpressionTreeNode(expr.IfFalse, "IfFalse"));
            }
            else if (expression is ConstantExpression)
            {
                var expr = expression as ConstantExpression;
                var valueStr = expr.Value.AsString();
                node = new ExpressionTreeNode(string.Format("ConstantExpression [{0}]: {1}", GetTypeName(expr.Type), valueStr));
                node.ExpressionString = valueStr;
            }
            else if (expression is DebugInfoExpression)
            {
                var expr = expression as DebugInfoExpression;
                node = new ExpressionTreeNode("DebugInfoExpression:");
                var document = expr.Document;
                node.Nodes.Add(new ExpressionTreeNode("FileName: " + document.FileName));
                node.Nodes.Add(new ExpressionTreeNode("Language: " + document.Language));
                node.Nodes.Add(new ExpressionTreeNode("LanguageVendor: " + document.LanguageVendor));
                node.Nodes.Add(new ExpressionTreeNode("DocumentType: " + document.DocumentType));
                node.Nodes.Add(new ExpressionTreeNode("StartLine: " + expr.StartLine));
                node.Nodes.Add(new ExpressionTreeNode("StartColumn: " + expr.StartColumn));
                node.Nodes.Add(new ExpressionTreeNode("EndLine: " + expr.EndLine));
                node.Nodes.Add(new ExpressionTreeNode("EndColumn: " + expr.EndColumn));
            }
            else if (expression is DefaultExpression)
            {
                var expr = expression as DefaultExpression;
                node = new ExpressionTreeNode(string.Format("DefaultExpression: [{0}-{1}]", GetType(expr.Type), GetTypeName(expr.Type)));
            }
            else if (expression is DynamicExpression)
            {
                var expr = expression as DynamicExpression;
                node = new ExpressionTreeNode(string.Format("DynamicExpression [{0}] Arguments:", GetTypeName(expr.DelegateType)));
                node.Nodes.AddRange(expr.Arguments.Select(a => GetExpressionTreeNode(a)));
            }
            else if (expression is GotoExpression)
            {
                var expr = expression as GotoExpression;
                node = new ExpressionTreeNode(string.Format("GotoExpression: [{0}]", GetTypeName(expr.Type)));
                node.Nodes.Add(new ExpressionTreeNode("Kind => " + expr.Kind));
                node.Nodes.Add(new ExpressionTreeNode("Target => " + expr.Target));
                node.Nodes.Add(CheckNull(expr.Value, "Value"));
            }
            else if (expression is IndexExpression)
            {
                var expr = expression as IndexExpression;
                node = new ExpressionTreeNode(string.Format("IndexExpression [{0}] Arguments:", expr.Indexer?.Name));
                node.Nodes.AddRange(expr.Arguments.Select(a => GetExpressionTreeNode(a)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Object, "Object"));
            }
            else if (expression is InvocationExpression)
            {
                var expr = expression as InvocationExpression;
                node = new ExpressionTreeNode(string.Format("InvocationExpression [{0}] Arguments:", expr.Expression));
                node.Nodes.AddRange(expr.Arguments.Select(a => GetExpressionTreeNode(a)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Expression, "Expression"));
            }
            else if (expression is LabelExpression)
            {
                var expr = expression as LabelExpression;
                node = new ExpressionTreeNode(string.Format("LabelExpression [{0}]", GetTypeName(expr.Type)));
                node.Nodes.Add(new ExpressionTreeNode("Target => " + expr.Target));
                node.Nodes.Add(CheckNull(expr.DefaultValue, "DefaultValue"));
            }
            else if (expression is LambdaExpression)
            {
                var expr = expression as LambdaExpression;
                node = new ExpressionTreeNode(string.Format("LambdaExpression [{0}] Parameters:", GetTypeName(expr.ReturnType)));
                node.Nodes.AddRange(expr.Parameters.Select(a => GetExpressionTreeNode(a)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Body, "Body"));
            }
            else if (expression is ListInitExpression)
            {
                var expr = expression as ListInitExpression;
                node = new ExpressionTreeNode("ListInitExpression");
                node.Nodes.Add(GetExpressionTreeNode(expr.NewExpression));
            }
            else if (expression is LoopExpression)
            {
                var expr = expression as LoopExpression;
                node = new ExpressionTreeNode("LoopExpression");
                node.Nodes.Add(GetExpressionTreeNode(expr.Body, "Body"));
            }
            else if (expression is MemberExpression)
            {
                var expr = expression as MemberExpression;
                node = new ExpressionTreeNode(string.Format("MemberExpression [{0}]: {1}", GetTypeName(expr.Type), expr.Member.Name));
            }
            else if (expression is MemberInitExpression)
            {
                var expr = expression as MemberInitExpression;
                node = new ExpressionTreeNode(string.Format("MemberInitExpression [{0}]:", GetTypeName(expr.NewExpression.Type)));
                expr.Bindings.ToList().ForEach(b => node.Nodes.Add(new ExpressionTreeNode(b.ToString())));
            }
            else if (expression is MethodCallExpression)
            {
                var expr = expression as MethodCallExpression;
                node = new ExpressionTreeNode(string.Format("MethodCallExpression [{0}] Arguments:", expr.Method.Name));
                expr.Arguments.ToList().ForEach(a => node.Nodes.Add(GetExpressionTreeNode(a)));
            }
            else if (expression is NewArrayExpression)
            {
                var expr = expression as NewArrayExpression;
                node = new ExpressionTreeNode(string.Format("NewArrayExpression [{0}]", GetTypeName(expr.Type)));
                expr.Expressions.ToList().ForEach(e => node.Nodes.Add(GetExpressionTreeNode(e)));
            }
            else if (expression is NewExpression)
            {
                var expr = expression as NewExpression;
                node = new ExpressionTreeNode(string.Format("NewExpression Arguments:"));
                for (int i = 0; i < expr.Arguments.Count; i++)
                    node.Nodes.Add(GetExpressionTreeNode(expr.Arguments[i], expr.Members?[i].Name));
            }
            else if (expression is ParameterExpression)
            {
                var expr = expression as ParameterExpression;
                node = new ExpressionTreeNode(string.Format("ParameterExpression [{0}]: {1}", GetTypeName(expr.Type), expr.Name));
            }
            else if (expression is RuntimeVariablesExpression)
            {
                var expr = expression as RuntimeVariablesExpression;
                node = new ExpressionTreeNode(string.Format("RuntimeVariablesExpression [{0}]", GetTypeName(expr.Type)));
                node.Nodes.AddRange(expr.Variables.Select(e => GetExpressionTreeNode(e)));
            }
            else if (expression is SwitchExpression)
            {
                var expr = expression as SwitchExpression;
                node = new ExpressionTreeNode(string.Format("SwitchExpression [{0}]", GetTypeName(expr.Type)));
                node.Nodes.AddRange(expr.Cases.Select((c, i) =>
                {
                    var n = new ExpressionTreeNode("Case");
                    n.Nodes.AddRange(c.TestValues.Select(t => GetExpressionTreeNode(t, "Value")));
                    n.Nodes.Add(GetExpressionTreeNode(c.Body, "Body"));
                    return n;
                }));
            }
            else if (expression is TryExpression)
            {
                var expr = expression as TryExpression;
                node = new ExpressionTreeNode(string.Format("TryExpression [{0}]", GetTypeName(expr.Type)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Body, "Try"));
                node.Nodes.AddRange(expr.Handlers.Select((c, i) =>
                {
                    var n = new ExpressionTreeNode("Catch");
                    n.Nodes.Add(GetExpressionTreeNode(c.Filter, "Filter"));
                    n.Nodes.Add(GetExpressionTreeNode(c.Body, "Body"));
                    return n;
                }));
                node.Nodes.Add(GetExpressionTreeNode(expr.Finally, "Finally"));
            }
            else if (expression is TypeBinaryExpression)
            {
                var expr = expression as TypeBinaryExpression;
                node = new ExpressionTreeNode(string.Format("TypeBinaryExpression [{0}] Operand:", GetTypeName(expr.TypeOperand)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Expression));
            }
            else if (expression is UnaryExpression)
            {
                var expr = expression as UnaryExpression;
                node = new ExpressionTreeNode(string.Format("UnaryExpression [{0}-{1}] Operand:", expr.NodeType, GetTypeName(expr.Type)));
                node.Nodes.Add(GetExpressionTreeNode(expr.Operand));
            }
            if (node == null)
                node = new ExpressionTreeNode(string.Format("Unknown Node [{0}-{1}]: {2}", GetTypeName(expression.GetType()), expression.NodeType, expression));
            if (prefix != null)
                node.Text = string.Format("{0} => {1}", prefix, node.Text);
            if (node.ExpressionString == null)
                node.ExpressionString = expression.ToString();
            return node;
        }

        private static string GetTypeName(Type type)
        {
            if (type == typeof(byte)) return "byte";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(short)) return "short";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(int)) return "int";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(long)) return "long";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(char)) return "char";
            if (type == typeof(string)) return "string";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(object)) return "object";
            if (type == typeof(void)) return "void";
            if (type.IsGenericType)
            {
                if (type.IsGenericTypeDefinition)
                {
                    var index = type.Name.IndexOf('`');
                    return index == -1 ? type.Name : type.Name.Substring(0, index);
                }

                var genTypeName = GetTypeName(type.GetGenericTypeDefinition());
                var genArgs = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
                return genTypeName + "<" + genArgs + ">";
            }
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                while (elementType.IsArray)
                {
                    elementType = elementType.GetElementType();
                }
                var elementTypeName = GetTypeName(elementType);

                var ranks = type.Name.Substring(type.Name.IndexOf('['));
                if (type.Name.Contains("]["))
                {
                    ranks = string.Join("", ranks.Replace("][", "]|[").Split('|').Reverse());
                }

                return elementTypeName + ranks;
            }
            return type.Name;
        }

        private static string GetType(Type type)
        {
            return type.IsSubclassOf(typeof(Delegate)) ? "delegate" :
                type.IsClass ? "class" :
                type.IsValueType ? "struct" :
                type.IsInterface ? "interface" : "";
        }

        private static ExpressionTreeNode CheckNull(Expression expression, string name)
        {
            ExpressionTreeNode node;
            if (expression == null)
            {
                node = new ExpressionTreeNode($"{name} => null");
            }
            else
            {
                node = GetExpressionTreeNode(expression, name);
            }
            return node;
        }
    }

    [Serializable]
    public class ExpressionTreeNode
    {
        public ExpressionTreeNode(string text)
        {
            Text = text;
            Nodes = new List<ExpressionTreeNode>();
        }

        public string Text { get; set; }
        public List<ExpressionTreeNode> Nodes { get; set; }
        public string ExpressionString { get; set; }
    }
}