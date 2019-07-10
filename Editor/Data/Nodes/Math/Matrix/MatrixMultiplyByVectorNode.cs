using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Math", "Matrix", "MultiplyMatrixByVector")]
    public class MatrixMultiplyByVectorNode : AbstractMaterialNode, IGeneratesBodyCode, IGeneratesFunction
    {
        protected const string kInputSlot1ShaderName = "Input1";
        protected const string kInputSlot2ShaderName = "Input2";
        protected const string kOutputSlotShaderName = "Output";

        public const int InputSlot1Id = 0;
        public const int InputSlot2Id = 1;
        public const int OutputSlotId = 2;

        public override bool hasPreview
        {
            get { return false; }
        }

        public MatrixMultiplyByVectorNode()
        {
            name = "MultiplyMatrixByVector";
            UpdateNodeAfterDeserialization();
        }

        protected string GetFunctionName()
        {
            return "unity_matrix_multiplybyvector_" + precision;
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(GetInputSlot1());
            AddSlot(GetInputSlot2());
            AddSlot(GetOutputSlot());
            RemoveSlotsNameNotMatching(validSlots);
        }

        protected int[] validSlots
        {
            get { return new[] { InputSlot1Id, InputSlot2Id, OutputSlotId }; }
        }

        protected MaterialSlot GetInputSlot1()
        {
            return new Matrix4MaterialSlot(InputSlot1Id, GetInputSlot1Name(), kInputSlot1ShaderName, SlotType.Input);
        }

        protected MaterialSlot GetInputSlot2()
        {
            return new Vector4MaterialSlot(InputSlot2Id, GetInputSlot2Name(), kInputSlot2ShaderName, SlotType.Input, Vector4.zero);
        }

        protected MaterialSlot GetOutputSlot()
        {
            return new Vector4MaterialSlot(OutputSlotId, GetOutputSlotName(), kOutputSlotShaderName, SlotType.Output, Vector4.zero);
        }

        protected virtual string GetInputSlot1Name()
        {
            return "Input1";
        }

        protected virtual string GetInputSlot2Name()
        {
            return "Input2";
        }

        protected string GetOutputSlotName()
        {
            return "Output";
        }

        protected string GetFunctionPrototype(string arg1Name, string arg2Name)
        {
            return string.Format("inline {0} {1} ({2} {3}, {4} {5})", NodeUtils.ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(InputSlot2Id).concreteValueType), GetFunctionName(), NodeUtils.ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(InputSlot1Id).concreteValueType), arg1Name, NodeUtils.ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(InputSlot2Id).concreteValueType), arg2Name);
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            NodeUtils.SlotConfigurationExceptionIfBadConfiguration(this, new[] { InputSlot1Id, InputSlot2Id }, new[] { OutputSlotId });
            string input1Value = GetSlotValue(InputSlot1Id, generationMode);
            string input2Value = GetSlotValue(InputSlot2Id, generationMode);
            visitor.AddShaderChunk(string.Format("{0} {1} = {2};", NodeUtils.ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(InputSlot2Id).concreteValueType), GetVariableNameForSlot(OutputSlotId), GetFunctionCallBody(input1Value, input2Value)), true);
        }

        protected string GetFunctionCallBody(string input1Value, string input2Value)
        {
            return string.Format("{0} ({1}, {2})", GetFunctionName(), input1Value, input2Value);
        }

        public void GenerateNodeFunction(FunctionRegistry registry, GenerationMode generationMode)
        {
            registry.ProvideFunction(GetFunctionName(), s =>
            {
                s.AppendLine(GetFunctionPrototype("arg1", "arg2"));
                using (s.BlockScope())
                {
                    s.AppendLine("return mul(arg1, arg2);");
                }
            });
        }
    }
}
