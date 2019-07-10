using System;
using System.Linq;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Property")]
    public class PropertyNode : AbstractMaterialNode, IGeneratesBodyCode, IOnAssetEnabled
    {
        private Guid m_PropertyGuid;

        [SerializeField]
        private string m_PropertyGuidSerialized;

        public const int OutputSlotId = 0;

        public PropertyNode()
        {
            name = "Property";
            UpdateNodeAfterDeserialization();
        }

        private void UpdateNode()
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property is FloatShaderProperty)
            {
                AddSlot(new Vector1MaterialSlot(OutputSlotId, "Out", "Out", SlotType.Output, 0));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Vector2ShaderProperty)
            {
                AddSlot(new Vector2MaterialSlot(OutputSlotId, "Out", "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Vector3ShaderProperty)
            {
                AddSlot(new Vector3MaterialSlot(OutputSlotId, "Out", "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Vector4ShaderProperty)
            {
                AddSlot(new Vector4MaterialSlot(OutputSlotId, "Out", "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is ColorShaderProperty)
            {
                AddSlot(new Vector4MaterialSlot(OutputSlotId, "Out", "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is TextureShaderProperty)
            {
                AddSlot(new Texture2DMaterialSlot(OutputSlotId, "Out", "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is CubemapShaderProperty)
            {
                AddSlot(new CubemapMaterialSlot(OutputSlotId, "Out", "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property is FloatShaderProperty)
            {
                var result = string.Format("{0} {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            else if (property is Vector2ShaderProperty)
            {
                var result = string.Format("{0}2 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            else if (property is Vector3ShaderProperty)
            {
                var result = string.Format("{0}3 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            else if (property is Vector4ShaderProperty)
            {
                var result = string.Format("{0}4 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            else if (property is ColorShaderProperty)
            {
                var result = string.Format("{0}4 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
        }

        [PropertyControl]
        public Guid propertyGuid
        {
            get { return m_PropertyGuid; }
            set
            {
                if (m_PropertyGuid == value)
                    return;

                var graph = owner as AbstractMaterialGraph;
                var property = graph.properties.FirstOrDefault(x => x.guid == value);
                if (property == null)
                    return;
                m_PropertyGuid = value;

                UpdateNode();

                if (onModified != null)
                {
                    onModified(this, ModificationScope.Topological);
                }
            }
        }

        public override string GetVariableNameForSlot(int slotId)
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);

            if (!(property is TextureShaderProperty) && !(property is CubemapShaderProperty))
                return base.GetVariableNameForSlot(slotId);

            return property.referenceName;
        }

        protected override bool CalculateNodeHasError()
        {
            var graph = owner as AbstractMaterialGraph;

            if (!graph.properties.Any(x => x.guid == propertyGuid))
                return true;

            return false;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            m_PropertyGuidSerialized = m_PropertyGuid.ToString();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (!string.IsNullOrEmpty(m_PropertyGuidSerialized))
                m_PropertyGuid = new Guid(m_PropertyGuidSerialized);
        }

        public void OnEnable()
        {
            UpdateNode();
        }
    }
}
