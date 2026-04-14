using CodeAssistant.Settings;

using System.Xml;
using System.Xml.Linq;

namespace CodeAssistant.Helpers;

internal static class StandardXmlFormatter
{
    public static string Format(string xml, CSProjFormatSettings settings)
    {
        var doc = XDocument.Parse(xml, LoadOptions.None);

        var xmlSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = settings.IndentChars,
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace,
            OmitXmlDeclaration = true
        };

        using var sw = new StringWriter();
        using var writer = XmlWriter.Create(sw, xmlSettings);

        doc.Save(writer);
        writer.Flush();

        return sw.ToString();
    }
}
