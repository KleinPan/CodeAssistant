using System.Text;
using System.Xml.Linq;

namespace CodeAssistant;

public static class CSprojFormatter
{
    public static string Format(string xml, XmlFormatSettings settings)
    {
        var doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
        var output = new StringBuilder();
        FormatNode(doc.Root!, 0, settings, output);
        return output.ToString().TrimStart();
    }

    private static void FormatNode(XElement element, int depth, XmlFormatSettings settings, StringBuilder output)
    {
        //缩进
        var indent = new string(' ', depth * settings.IndentChars.Length);

        // 开始标签
        output.Append($"{indent}<{element.Name}");

        // 处理属性
        // <PackageReference Include="Newtonsoft.Json" Version="13.0.3"/>
        // Include="Newtonsoft.Json"叫做属性
        if (element.HasAttributes)
        {
            FormatAttributes(element, depth, settings, output);
        }

        // 处理子内容
        if (!element.IsEmpty && element.Nodes().Any())
        {
            FormatContent(element, depth, settings, output);
        }
        else
        {
            output.Append("/>");
        }

        // 当前元素格式化完换行
        AddElementNewLine(element, settings, output);
    }

    private static void FormatAttributes(XElement element, int depth, XmlFormatSettings settings, StringBuilder output)
    {
        var attributes = element.Attributes().ToList();

        for (int i = 0; i < attributes.Count; i++)
        {
            //属性值
            var attr = attributes[i];
            //var
            //多属性不用换行
            string attrIndent = "";
            if (settings.NewLineOnAttributes)
            {
                attrIndent = i == 0 ? " " : $"{Environment.NewLine}{new string(' ', depth * settings.IndentChars.Length + 2)}";
            }
            else
            {
                attrIndent = " ";
            }

            output.Append($"{attrIndent}{attr.Name.LocalName}=\"{attr.Value}\"");
        }
    }

    private static void FormatContent(XElement element, int depth, XmlFormatSettings settings, StringBuilder output)
    {
        output.Append('>');

        var childIndent = new string(' ', (depth + 1) * settings.IndentChars.Length);
        var isSimpleContent = element.Nodes().All(n => n is XText);

        // 处理子节点
        foreach (var node in element.Nodes())
        {
            if (node is XElement child)
            {
                FormatNode(child, depth + 1, settings, output);
            }
            else if (node is XText text)
            {
                output.Append(isSimpleContent
                    ? text.Value.Trim()
                    : $"{Environment.NewLine}{text.Value.Trim().Replace(Environment.NewLine, "")}");
            }
        }

        // 闭合标签
        if (!isSimpleContent)
        {
            output.Append(new string(' ', depth * settings.IndentChars.Length));
        }

        output.Append($"</{element.Name}>");
    }

    private static void AddElementNewLine(XElement element, XmlFormatSettings settings, StringBuilder output)
    {
        if (settings.AddEmptyLineBetweenGroups && settings.ElementsWithNewLine.Contains(element.Name.LocalName))
        {
            if (element.NextNode!.NextNode == null)//最后一个分组不换行
            {
                return;
            }
            output.AppendLine();
        }
    }
}

/// <summary>基础格式</summary>
public class XmlFormatSettings
{
    /// <summary>是否保留元素Value文本中的换行</summary>
    public bool PreserveElementValueNewLines { get; set; } = false;

    /// <summary>缩进字符</summary>
    public string IndentChars { get; set; } = "  ";

    /// <summary>属性换行</summary>
    public bool NewLineOnAttributes { get; set; } = false;

    /// <summary> 在<ItemGroup>等元素间添加空行 </summary>
    public bool AddEmptyLineBetweenGroups { get; set; } = true;

    /// <summary>展开空元素（如 <ItemGroup/> → <ItemGroup></ItemGroup>）</summary>
    public bool ExpandEmptyElements { get; set; } = false;

    /// <summary>特定元素处理</summary>
    public HashSet<string> ElementsWithAttributeAlignment { get; } = new()
    {
        "PackageReference", "ProjectReference", "Reference"
    };

    /// <summary>特定元素处理</summary>
    public HashSet<string> ElementsWithNewLine { get; } = new()
    {
        "ItemGroup", "PropertyGroup"
    };
}