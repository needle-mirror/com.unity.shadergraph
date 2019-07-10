using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEditor.ShaderGraph
{
    public class PropertyCollector
    {
        public struct TextureInfo
        {
            public string name;
            public int textureId;
            public bool modifiable;
        }

        private readonly List<IShaderProperty> m_Properties = new List<IShaderProperty>();

        public void AddShaderProperty(IShaderProperty chunk)
        {
            if (m_Properties.Any(x => x.referenceName == chunk.referenceName))
                return;
            m_Properties.Add(chunk);
        }

        public string GetPropertiesBlock(int baseIndentLevel)
        {
            var sb = new StringBuilder();
            foreach (var prop in m_Properties.Where(x => x.generatePropertyBlock))
            {
                for (var i = 0; i < baseIndentLevel; i++)
                    sb.Append("\t");
                sb.AppendLine(prop.GetPropertyBlockString());
            }
            return sb.ToString();
        }

        public string GetPropertiesDeclaration(int baseIndentLevel)
        {
            var sb = new StringBuilder();
            foreach (var prop in m_Properties)
            {
                for (var i = 0; i < baseIndentLevel; i++)
                    sb.Append("\t");
                sb.AppendLine(prop.GetPropertyDeclarationString());
            }
            return sb.ToString();
        }

        public List<TextureInfo> GetConfiguredTexutres()
        {
            var result = new List<TextureInfo>();

            foreach (var prop in m_Properties.OfType<TextureShaderProperty>())
            {
                if (prop.referenceName != null)
                {
                    var textureInfo = new TextureInfo
                    {
                        name = prop.referenceName,
                        textureId = prop.value.texture != null ? prop.value.texture.GetInstanceID() : 0,
                        modifiable = prop.modifiable
                    };
                    result.Add(textureInfo);
                }
            }

            foreach (var prop in m_Properties.OfType<CubemapShaderProperty>())
            {
                if (prop.referenceName != null)
                {
                    var textureInfo = new TextureInfo
                    {
                        name = prop.referenceName,
                        textureId = prop.value.cubemap != null ? prop.value.cubemap.GetInstanceID() : 0,
                        modifiable = prop.modifiable
                    };
                    result.Add(textureInfo);
                }
            }
            return result;
        }
    }
}
