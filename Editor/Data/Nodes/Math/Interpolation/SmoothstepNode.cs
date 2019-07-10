using System.Reflection;

namespace UnityEditor.ShaderGraph
{
    [Title("Math", "Interpolation", "Smoothstep")]
    class SmoothstepNode : CodeFunctionNode
    {
        public SmoothstepNode()
        {
            name = "Smoothstep";
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("Unity_Smoothstep", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string Unity_Smoothstep(
            [Slot(0, Binding.None, 0, 0, 0, 0)] DynamicDimensionVector A,
            [Slot(1, Binding.None, 1, 1, 1, 1)] DynamicDimensionVector B,
            [Slot(2, Binding.None, 0, 0, 0, 0)] DynamicDimensionVector T,
            [Slot(3, Binding.None)] out DynamicDimensionVector Out)
        {
            return
                @"
{
    Out = smoothstep(A, B, T);
}";
        }
    }
}
