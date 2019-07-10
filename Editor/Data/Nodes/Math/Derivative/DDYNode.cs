using System.Reflection;
using UnityEngine;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Math", "Derivative", "DDY")]
    public class DDYNode : CodeFunctionNode
    {
        public DDYNode()
        {
            name = "DDY";
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("Unity_DDY", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string Unity_DDY(
            [Slot(0, Binding.None)] DynamicDimensionVector In,
            [Slot(1, Binding.None)] out DynamicDimensionVector Out)
        {
            return
                @"
{
    Out = ddy(In);
}
";
        }
    }
}
