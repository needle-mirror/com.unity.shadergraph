using System;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Graphing;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.ShaderGraph.Drawing
{
    class BlackboardFieldPropertyView : VisualElement
    {
        readonly AbstractMaterialGraph m_Graph;

        public BlackboardFieldPropertyView(AbstractMaterialGraph graph, IShaderProperty property)
        {
            m_Graph = graph;
            if (property is Vector1ShaderProperty)
            {
                VisualElement floatRow = new VisualElement();
                VisualElement intRow = new VisualElement();
                VisualElement modeRow = new VisualElement();
                VisualElement minRow = new VisualElement();
                VisualElement maxRow = new VisualElement();
                FloatField floatField = null;

                var floatProperty = (Vector1ShaderProperty)property;

                if(floatProperty.floatType == FloatType.Integer)
                {
                    var field = new IntegerField { value = (int)floatProperty.value };
                    field.OnValueChanged(intEvt =>
                    {
                        floatProperty.value = (float)intEvt.newValue;
                        DirtyNodes();
                    });
                    intRow = AddRow("Default", field);
                }
                else
                {
                    floatField = new FloatField { value = floatProperty.value };
                    floatField.OnValueChanged(evt =>
                    {
                        floatProperty.value = (float)evt.newValue;
                        DirtyNodes();
                    });
                    floatRow = AddRow("Default", floatField);
                }

                var floatModeField = new EnumField((Enum)floatProperty.floatType);
                floatModeField.value = floatProperty.floatType;
                floatModeField.OnValueChanged(evt =>
                {
                    if(floatProperty.floatType == (FloatType)evt.newValue)
                        return;
                    floatProperty = (Vector1ShaderProperty)property;
                    floatProperty.floatType = (FloatType)evt.newValue;
                    switch(floatProperty.floatType)
                    {
                        case FloatType.Slider:
                            RemoveElements(new VisualElement[]{floatRow, intRow, modeRow, minRow, maxRow});
                            var field = new FloatField { value = Mathf.Max(Mathf.Min(floatProperty.value, floatProperty.rangeValues.y), floatProperty.rangeValues.x) };
                            floatProperty.value = (float)field.value;
                            field.OnValueChanged(defaultEvt =>
                            {
                                floatProperty.value = Mathf.Max(Mathf.Min((float)defaultEvt.newValue, floatProperty.rangeValues.y), floatProperty.rangeValues.x);
                                field.value = floatProperty.value;
                                DirtyNodes();
                            });
                            floatRow = AddRow("Default", field);
                            field.value = Mathf.Max(Mathf.Min(floatProperty.value, floatProperty.rangeValues.y), floatProperty.rangeValues.x);
                            modeRow = AddRow("Mode", floatModeField);
                            var minField = new FloatField { value = floatProperty.rangeValues.x };
                            minField.OnValueChanged(minEvt =>
                            {
                                floatProperty.rangeValues = new Vector2((float)minEvt.newValue, floatProperty.rangeValues.y);
                                floatProperty.value = Mathf.Max(Mathf.Min(floatProperty.value, floatProperty.rangeValues.y), floatProperty.rangeValues.x);
                                field.value = floatProperty.value;
                                DirtyNodes();
                            });
                            minRow = AddRow("Min", minField);
                            var maxField = new FloatField { value = floatProperty.rangeValues.y };
                            maxField.OnValueChanged(maxEvt =>
                            {
                                floatProperty.rangeValues = new Vector2(floatProperty.rangeValues.x, (float)maxEvt.newValue);
                                floatProperty.value = Mathf.Max(Mathf.Min(floatProperty.value, floatProperty.rangeValues.y), floatProperty.rangeValues.x);
                                field.value = floatProperty.value;
                                DirtyNodes();
                            });
                            maxRow = AddRow("Max", maxField);
                            break;
                        case FloatType.Integer:
                            RemoveElements(new VisualElement[]{floatRow, intRow, modeRow, minRow, maxRow});
                            var intField = new IntegerField { value = (int)floatProperty.value };
                            intField.OnValueChanged(intEvt =>
                            {
                                floatProperty.value = (float)intEvt.newValue;
                                DirtyNodes();
                            });
                            intRow = AddRow("Default", intField);
                            modeRow = AddRow("Mode", floatModeField);
                            break;
                        default:
                            RemoveElements(new VisualElement[]{floatRow, intRow, modeRow, minRow, maxRow});
                            field = new FloatField { value = floatProperty.value };
                            field.OnValueChanged(defaultEvt =>
                            {
                                floatProperty.value = (float)defaultEvt.newValue;
                                DirtyNodes();
                            });
                            floatRow = AddRow("Default", field);
                            modeRow = AddRow("Mode", floatModeField);
                            break;
                    }
                    DirtyNodes();
                });
                modeRow = AddRow("Mode", floatModeField);

                if(floatProperty.floatType == FloatType.Slider)
                {
                    var minField = new FloatField { value = floatProperty.rangeValues.x };
                    minField.OnValueChanged(minEvt =>
                    {
                        floatProperty.rangeValues = new Vector2((float)minEvt.newValue, floatProperty.rangeValues.y);
                        floatProperty.value = Mathf.Max(Mathf.Min(floatProperty.value, floatProperty.rangeValues.y), floatProperty.rangeValues.x);
                        floatField.value = floatProperty.value;
                        DirtyNodes();
                    });
                    minRow = AddRow("Min", minField);
                    var maxField = new FloatField { value = floatProperty.rangeValues.y };
                    maxField.OnValueChanged(maxEvt =>
                    {
                        floatProperty.rangeValues = new Vector2(floatProperty.rangeValues.x, (float)maxEvt.newValue);
                        floatProperty.value = Mathf.Max(Mathf.Min(floatProperty.value, floatProperty.rangeValues.y), floatProperty.rangeValues.x);
                        floatField.value = floatProperty.value;
                        DirtyNodes();
                    });
                    maxRow = AddRow("Max", maxField);
                }
            }
            else if (property is Vector2ShaderProperty)
            {
                var vectorProperty = (Vector2ShaderProperty)property;
                var field = new Vector2Field { value = vectorProperty.value };
                field.OnValueChanged(evt =>
                {
                    vectorProperty.value = evt.newValue;
                    DirtyNodes();
                });
                AddRow("Default", field);
            }
            else if (property is Vector3ShaderProperty)
            {
                var vectorProperty = (Vector3ShaderProperty)property;
                var field = new Vector3Field { value = vectorProperty.value };
                field.OnValueChanged(evt =>
                {
                    vectorProperty.value = evt.newValue;
                    DirtyNodes();
                });
                AddRow("Default", field);
            }
            else if (property is Vector4ShaderProperty)
            {
                var vectorProperty = (Vector4ShaderProperty)property;
                var field = new Vector4Field { value = vectorProperty.value };
                field.OnValueChanged(evt =>
                {
                    vectorProperty.value = evt.newValue;
                    DirtyNodes();
                });
                AddRow("Default", field);
            }
            else if (property is ColorShaderProperty)
            {
                var colorProperty = (ColorShaderProperty)property;
                var colorField = new ColorField { value = property.defaultValue, showEyeDropper = false, hdr = colorProperty.colorMode == ColorMode.HDR };
                colorField.OnValueChanged(evt =>
                {
                    colorProperty.value = evt.newValue;
                    DirtyNodes();
                });
                AddRow("Default", colorField);
                var colorModeField = new EnumField((Enum)colorProperty.colorMode);
                colorModeField.OnValueChanged(evt =>
                {
                    if(colorProperty.colorMode == (ColorMode)evt.newValue)
                        return;
                    colorProperty.colorMode = (ColorMode)evt.newValue;
                    colorField.hdr = colorProperty.colorMode == ColorMode.HDR;
                    colorField.DoRepaint();
                    DirtyNodes();
                });
                AddRow("Mode", colorModeField);
            }
            else if (property is TextureShaderProperty)
            {
                var textureProperty = (TextureShaderProperty)property;
                var field = new ObjectField { value = textureProperty.value.texture, objectType = typeof(Texture) };
                field.OnValueChanged(evt =>
                {
                    textureProperty.value.texture = (Texture)evt.newValue;
                    DirtyNodes();
                });
                AddRow("Default", field);
            }
            else if (property is CubemapShaderProperty)
            {
                var cubemapProperty = (CubemapShaderProperty)property;
                var field = new ObjectField { value = cubemapProperty.value.cubemap, objectType = typeof(Cubemap) };
                field.OnValueChanged(evt =>
                {
                    cubemapProperty.value.cubemap = (Cubemap)evt.newValue;
                    DirtyNodes();
                });
                AddRow("Default", field);
            }
            else if (property is BooleanShaderProperty)
            {
                var booleanProperty = (BooleanShaderProperty)property;
                Action onBooleanChanged = () => 
                { 
                    booleanProperty.value = !booleanProperty.value;
                    DirtyNodes();
                };
                var field = new Toggle(onBooleanChanged) { on = booleanProperty.value };
                AddRow("Default", field);
            }
//            AddRow("Type", new TextField());
//            AddRow("Exposed", new Toggle(null));
//            AddRow("Range", new Toggle(null));
//            AddRow("Default", new TextField());
//            AddRow("Tooltip", new TextField());

            AddToClassList("sgblackboardFieldPropertyView");
        }

        VisualElement AddRow(string labelText, VisualElement control)
        {
            VisualElement rowView = new VisualElement();

            rowView.AddToClassList("rowView");

            Label label = new Label(labelText);

            label.AddToClassList("rowViewLabel");
            rowView.Add(label);

            control.AddToClassList("rowViewControl");
            rowView.Add(control);

            Add(rowView);
            return rowView;
        }

        void RemoveElements(VisualElement[] elements)
        {
            for(int i = 0; i < elements.Length; i++)
            {
                if(elements[i].parent == this)
                    Remove(elements[i]);
            }
        }

        void DirtyNodes()
        {
            foreach (var node in m_Graph.GetNodes<PropertyNode>())
                node.Dirty(ModificationScope.Node);
        }
    }
}
