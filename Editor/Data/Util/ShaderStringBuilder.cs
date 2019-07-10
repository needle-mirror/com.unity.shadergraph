using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace UnityEditor.ShaderGraph
{
    public class ShaderStringBuilder : IDisposable
    {
        enum ScopeType
        {
            Indent,
            Block
        }

        StringBuilder m_StringBuilder;
        Stack<ScopeType> m_ScopeStack;
        int m_IndentationLevel;
        const string k_IndentationString = "    ";

        public ShaderStringBuilder()
        {
            m_StringBuilder = new StringBuilder();
        }

        public void AppendNewLine()
        {
            m_StringBuilder.Append(Environment.NewLine);
        }

        public void AppendLine(string value)
        {
            AppendNewLine();
            AppendIndented(value);
        }

        [StringFormatMethod("formatStr")]
        public void AppendLineFormat(string format, params object[] args)
        {
            AppendNewLine();
            m_StringBuilder.AppendFormat(format, args);
        }

        public void Append(string value)
        {
            m_StringBuilder.Append(value);
        }

        public void AppendIndentation()
        {
            for (var i = 0; i < m_IndentationLevel; i++)
                m_StringBuilder.Append(k_IndentationString);
        }

        public void AppendIndented(string value)
        {
            AppendIndentation();
            m_StringBuilder.Append(value);
        }

        public IDisposable Indent()
        {
            m_IndentationLevel++;
            return this;
        }

        public void Dispose()
        {
            m_IndentationLevel--;
        }
    }
}
