using System.Reflection;
using UnityEngine;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Math", "Derivative", "DDXY")]
    public class DDXYNode : CodeFunctionNode
    {
        public DDXYNode()
        {
            name = "DDXY";
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("Unity_DDXY", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string Unity_DDXY(
            [Slot(0, Binding.None)] DynamicDimensionVector In,
            [Slot(1, Binding.None)] out DynamicDimensionVector Out)
        {
            return
                @"
{
    Out = abs(ddx(In)) + abs(ddy(In));
}
";
        }
    }
}
