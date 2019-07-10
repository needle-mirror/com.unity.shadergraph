using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEngine.Experimental.UIElements;

[assembly: InternalsVisibleTo("com.unity.shadergraph.EditorTests")]

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    public abstract class MaterialSlot : ISlot
    {
        const string k_NotInit =  "Not Initilaized";

        [SerializeField]
        int m_Id;

        [SerializeField]
        string m_DisplayName = k_NotInit;

        [SerializeField]
        SlotType m_SlotType = SlotType.Input;

        [SerializeField]
        int m_Priority = int.MaxValue;

        [SerializeField]
        bool m_Hidden;

        [SerializeField]
        string m_ShaderOutputName;

        [SerializeField]
        ShaderStage m_ShaderStage;

        bool m_HasError;

        protected MaterialSlot() {}

        protected MaterialSlot(int slotId, string displayName, string shaderOutputName, SlotType slotType, ShaderStage shaderStage = ShaderStage.Dynamic, bool hidden = false)
        {
            m_Id = slotId;
            m_DisplayName = displayName;
            m_SlotType = slotType;
            m_Hidden = hidden;
            m_ShaderOutputName = shaderOutputName;
            this.shaderStage = shaderStage;
        }

        protected MaterialSlot(int slotId, string displayName, string shaderOutputName, SlotType slotType, int priority, ShaderStage shaderStage = ShaderStage.Dynamic, bool hidden = false)
        {
            m_Id = slotId;
            m_DisplayName = displayName;
            m_SlotType = slotType;
            m_Priority = priority;
            m_Hidden = hidden;
            m_ShaderOutputName = shaderOutputName;
            this.shaderStage = shaderStage;
        }

        public virtual VisualElement InstantiateControl()
        {
            return null;
        }

        static string ConcreteSlotValueTypeAsString(ConcreteSlotValueType type)
        {
            switch (type)
            {
                case ConcreteSlotValueType.Vector1:
                    return "(1)";
                case ConcreteSlotValueType.Vector2:
                    return "(2)";
                case ConcreteSlotValueType.Vector3:
                    return "(3)";
                case ConcreteSlotValueType.Vector4:
                    return "(4)";
                case ConcreteSlotValueType.Boolean:
                    return "(B)";
                case ConcreteSlotValueType.Matrix2:
                    return "(2x2)";
                case ConcreteSlotValueType.Matrix3:
                    return "(3x3)";
                case ConcreteSlotValueType.Matrix4:
                    return "(4x4)";
                case ConcreteSlotValueType.SamplerState:
                    return "(SS)";
                case ConcreteSlotValueType.Texture2D:
                    return "(T)";
                case ConcreteSlotValueType.Cubemap:
                    return "(C)";
                default:
                    return "(E)";
            }
        }

        public virtual string displayName
        {
            get { return m_DisplayName + ConcreteSlotValueTypeAsString(concreteValueType); }
            set { m_DisplayName = value; }
        }

        public string RawDisplayName()
        {
            return m_DisplayName;
        }

        public static MaterialSlot CreateMaterialSlot(SlotValueType type, int slotId, string displayName, string shaderOutputName, SlotType slotType, Vector4 defaultValue, ShaderStage shaderStage = ShaderStage.Dynamic, bool hidden = false)
        {
            switch (type)
            {
                case SlotValueType.SamplerState:
                    return new SamplerStateMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
                case SlotValueType.Matrix4:
                    return new Matrix4MaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
                case SlotValueType.Matrix3:
                    return new Matrix3MaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
                case SlotValueType.Matrix2:
                    return new Matrix2MaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
                case SlotValueType.Texture2D:
                    return slotType == SlotType.Input
                        ? new Texture2DInputMaterialSlot(slotId, displayName, shaderOutputName, shaderStage, hidden)
                        : new Texture2DMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
                case SlotValueType.Cubemap:
                    return slotType == SlotType.Input
                        ? new CubemapInputMaterialSlot(slotId, displayName, shaderOutputName, shaderStage, hidden)
                        : new CubemapMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
                case SlotValueType.Dynamic:
                    return new DynamicVectorMaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden);
                case SlotValueType.Vector4:
                    return new Vector4MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden);
                case SlotValueType.Vector3:
                    return new Vector3MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden);
                case SlotValueType.Vector2:
                    return new Vector2MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden);
                case SlotValueType.Vector1:
                    return new Vector1MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue.x, shaderStage, hidden);
                case SlotValueType.Boolean:
                    return new BooleanMaterialSlot(slotId, displayName, shaderOutputName, slotType, false, shaderStage, hidden);
            }

            throw new ArgumentOutOfRangeException("type", type, null);
        }

        public SlotReference slotReference
        {
            get { return new SlotReference(owner.guid, m_Id); }
        }

        public INode owner { get; set; }

        public bool hidden
        {
            get { return m_Hidden; }
            set { m_Hidden = value; }
        }

        public int id
        {
            get { return m_Id; }
        }

        public int priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }

        public bool isInputSlot
        {
            get { return m_SlotType == SlotType.Input; }
        }

        public bool isOutputSlot
        {
            get { return m_SlotType == SlotType.Output; }
        }

        public SlotType slotType
        {
            get { return m_SlotType; }
        }

        public bool isConnected
        {
            get
            {
                // node and graph respectivly
                if (owner == null || owner.owner == null)
                    return false;

                var graph = owner.owner;
                var edges = graph.GetEdges(slotReference);
                return edges.Any();
            }
        }

        public abstract SlotValueType valueType { get; }

        public abstract ConcreteSlotValueType concreteValueType { get; }

        public string shaderOutputName
        {
            get { return m_ShaderOutputName; }
            private set { m_ShaderOutputName = value; }
        }

        public ShaderStage shaderStage
        {
            get { return m_ShaderStage; }
            set { m_ShaderStage = value; }
        }

        public bool hasError
        {
            get { return m_HasError; }
            set { m_HasError = value; }
        }

        bool IsCompatibleWithInputSlotType(SlotValueType inputType)
        {
            switch (valueType)
            {
                case SlotValueType.SamplerState:
                    return inputType == SlotValueType.SamplerState;
                case SlotValueType.Matrix4:
                    return inputType == SlotValueType.Matrix4
                        || inputType == SlotValueType.Matrix3
                        || inputType == SlotValueType.Matrix2;
                case SlotValueType.Matrix3:
                    return inputType == SlotValueType.Matrix3
                        || inputType == SlotValueType.Matrix2;
                case SlotValueType.Matrix2:
                    return inputType == SlotValueType.Matrix2;
                case SlotValueType.Texture2D:
                    return inputType == SlotValueType.Texture2D;
                case SlotValueType.Cubemap:
                    return inputType == SlotValueType.Cubemap;
                case SlotValueType.Dynamic:
                case SlotValueType.Vector4:
                case SlotValueType.Vector3:
                case SlotValueType.Vector2:
                case SlotValueType.Vector1:
                    return inputType == SlotValueType.Vector4
                        || inputType == SlotValueType.Vector3
                        || inputType == SlotValueType.Vector2
                        || inputType == SlotValueType.Vector1
                        || inputType == SlotValueType.Dynamic;
                case SlotValueType.Boolean:
                    return inputType == SlotValueType.Boolean;
            }
            return false;
        }

        public bool IsCompatibleWith(MaterialSlot otherSlot)
        {
            return otherSlot != null
                && otherSlot.owner != owner
                && otherSlot.isInputSlot != isInputSlot
                && ((isInputSlot
                     ? otherSlot.IsCompatibleWithInputSlotType(valueType)
                     : IsCompatibleWithInputSlotType(otherSlot.valueType)));
        }

        public virtual string GetDefaultValue(GenerationMode generationMode)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            if (generationMode.IsPreview())
                return matOwner.GetVariableNameForSlot(id);

            return ConcreteSlotValueAsVariable(matOwner.precision);
        }

        protected virtual string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            return "error";
        }

        public abstract void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode);

        protected static PropertyType ConvertConcreteSlotValueTypeToPropertyType(ConcreteSlotValueType slotValue)
        {
            switch (slotValue)
            {
                case ConcreteSlotValueType.Texture2D:
                    return PropertyType.Texture;
                case ConcreteSlotValueType.Cubemap:
                    return PropertyType.Cubemap;
                case ConcreteSlotValueType.Boolean:
                    return PropertyType.Boolean;
                case ConcreteSlotValueType.Vector1:
                    return PropertyType.Vector1;
                case ConcreteSlotValueType.Vector2:
                    return PropertyType.Vector2;
                case ConcreteSlotValueType.Vector3:
                    return PropertyType.Vector3;
                case ConcreteSlotValueType.Vector4:
                    return PropertyType.Vector4;
                case ConcreteSlotValueType.Matrix2:
                    return PropertyType.Matrix2;
                case ConcreteSlotValueType.Matrix3:
                    return PropertyType.Matrix3;
                case ConcreteSlotValueType.Matrix4:
                    return PropertyType.Matrix4;
                case ConcreteSlotValueType.SamplerState:
                    return PropertyType.SamplerState;
                default:
                    return PropertyType.Vector4;
            }
        }

        public virtual PreviewProperty GetPreviewProperty(string name)
        {
            return default(PreviewProperty);
        }

        public abstract void CopyValuesFrom(MaterialSlot foundSlot);

        bool Equals(MaterialSlot other)
        {
            return m_Id == other.m_Id && owner.guid.Equals(other.owner.guid);
        }

        public bool Equals(ISlot other)
        {
            return Equals(other as object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MaterialSlot)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_Id * 397) ^ (owner != null ? owner.GetHashCode() : 0);
            }
        }
    }
}
