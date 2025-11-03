using System.Text;

namespace Robin.Generators.Accessor
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIndented(
            this StringBuilder sb,
            int indentLevel,
            string text = "",
            string indentString = "    ")
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            if (indentLevel < 0)
                indentLevel = 0;

            // Apply indentation before appending text
            for (int i = 0; i < indentLevel; i++)
                sb.Append(indentString);

            sb.AppendLine(text);
            return sb;
        }

        public static StringBuilder AppendLinesIndented(
             this StringBuilder sb,
             int indentLevel,
             IEnumerable<string> lines,
             string indentString = "    ")
        {
            foreach (var line in lines)
                sb.AppendLineIndented(indentLevel, line, indentString);

            return sb;
        }
    }
}