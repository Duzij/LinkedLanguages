// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var path = @"F:\DEV\LinkedLanguages\EtyTreeLocal.Fixes\enwkt-20180301.etymology.bad.ttl";

File.WriteAllLines(
        @"F:\DEV\LinkedLanguages\EtyTreeLocal.Fixes\enwkt-20180301.etymology.ttl",
        File.ReadAllLines(path).Select(line => changeLine(line)
            )
        , Encoding.UTF8);

static string changeLine(string line)
{
    var pattern = @"/wiki/.+ ";
    if (Regex.IsMatch(line, pattern))
    {
        var replace = Regex.Replace(line, pattern, replaceSpacesWithUnderscore);
        return replace;
    }

    return line;
}

static string replaceSpacesWithUnderscore(Match match)
{
    return match.Groups[0].Value.Trim() + "_";
}