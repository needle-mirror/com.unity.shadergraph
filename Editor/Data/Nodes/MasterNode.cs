using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.Graphing.Util;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    public abstract class MasterNode<T> : AbstractMaterialNode, IMasterNode where T : class, ISubShader
    {
        [NonSerialized]
        List<T> m_SubShaders = new List<T>();

        [SerializeField]
        List<SerializationHelper.JSONSerializedElement> m_SerializableSubShaders = new List<SerializationHelper.JSONSerializedElement>();

        public override bool hasPreview
        {
            get { return true; }
        }

        public override bool allowedInSubGraph
        {
            get { return false; }
        }

        public override PreviewMode previewMode
        {
            get { return PreviewMode.Preview3D; }
        }

        public Type supportedSubshaderType
        {
            get { return typeof(T); }
        }

        public IEnumerable<T> subShaders
        {
            get { return m_SubShaders; }
        }

        public void AddSubShader(T subshader)
        {
            if (m_SubShaders.Contains(subshader))
                return;

            m_SubShaders.Add(subshader);
            Dirty(ModificationScope.Graph);
        }

        public void RemoveSubShader(T subshader)
        {
            m_SubShaders.RemoveAll(x => x == subshader);
            Dirty(ModificationScope.Graph);
        }

        public string GetShader(GenerationMode mode, string outputName, out List<PropertyCollector.TextureInfo> configuredTextures)
        {
            var activeNodeList = ListPool<INode>.Get();
            NodeUtils.DepthFirstCollectNodesFromNode(activeNodeList, this);

            var shaderProperties = new PropertyCollector();

            var abstractMaterialGraph = owner as AbstractMaterialGraph;
            if (abstractMaterialGraph != null)
                abstractMaterialGraph.CollectShaderProperties(shaderProperties, mode);

            foreach (var activeNode in activeNodeList.OfType<AbstractMaterialNode>())
                activeNode.CollectShaderProperties(shaderProperties, mode);

            var finalShader = new ShaderGenerator();
            finalShader.AddShaderChunk(string.Format(@"Shader ""{0}""", outputName), false);
            finalShader.AddShaderChunk("{", false);
            finalShader.Indent();

            finalShader.AddShaderChunk("Properties", false);
            finalShader.AddShaderChunk("{", false);
            finalShader.Indent();
            finalShader.AddShaderChunk(shaderProperties.GetPropertiesBlock(2), false);
            finalShader.Deindent();
            finalShader.AddShaderChunk("}", false);

            foreach (var subShader in m_SubShaders)
                finalShader.AddShaderChunk(subShader.GetSubshader(this, mode), true);

            finalShader.AddShaderChunk(@"FallBack ""Hidden/InternalErrorShader""", false);
            finalShader.Deindent();
            finalShader.AddShaderChunk("}", false);

            configuredTextures = shaderProperties.GetConfiguredTexutres();
            return finalShader.GetShaderString(0);
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            m_SerializableSubShaders = SerializationHelper.Serialize<T>(m_SubShaders);
        }

        public override void OnAfterDeserialize()
        {
            m_SubShaders = SerializationHelper.Deserialize<T>(m_SerializableSubShaders, GraphUtil.GetLegacyTypeRemapping());
            m_SerializableSubShaders = null;
            base.OnAfterDeserialize();

        }

        public override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    var isValid = !type.IsAbstract && type.IsPublic && !type.IsGenericType && type.IsClass && typeof(T).IsAssignableFrom(type);
                    if (isValid && !subShaders.Any(s => s.GetType() == type))
                    {
                        try
                        {
                            var subShader = (T)Activator.CreateInstance(type);
                            AddSubShader(subShader);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }
    }
}
