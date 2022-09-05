using LinkedLanguages.BL;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace LinkedLanguages.CodeGenerator
{
    [Generator]
    public class LangaugesEnumSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder(@"
                using System;
                using System.ComponentModel.DataAnnotations;
                namespace LinkedLanguages.CodeGenerator
                {
                    public enum Language
                    {
                ");

            var stream = typeof(LanguageFacade).GetTypeInfo().Assembly.GetManifestResourceStream("LinkedLanguages.Resources.ISO-639-2_utf-8.txt");
            var langList = new List<Tuple<string, string>>() { };
            if (stream is not null)
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        var lineSplit = line.Split('|');
                        langList.Add(new Tuple<string, string>($@"[Display(Name=""{lineSplit[3]}"")]", $"{lineSplit[0]}"));

                        if (!string.IsNullOrEmpty(lineSplit[1]))
                        {
                            langList.Add(new Tuple<string, string>($@"[Display(Name=""{lineSplit[3]}"")]", $"{lineSplit[1]}"));
                        }

                        if (!string.IsNullOrEmpty(lineSplit[2]))
                        {
                            langList.Add(new Tuple<string, string>($@"[Display(Name=""{lineSplit[3]}"")]", $"{lineSplit[2]}"));
                        }
                    }
                }
            }

            for (int i = 0; i < langList.Count; i++)
            {
                var lang = langList[i];
                sourceBuilder.AppendLine(lang.Item1);
                sourceBuilder.AppendLine("@" + lang.Item2.Replace("-", "_"));

                if (i != langList.Count - 1)
                {
                    sourceBuilder.AppendLine(",");
                }
            }

            sourceBuilder.AppendLine(@"}}");
            // inject the created source into the users compilation
            context.AddSource("LangaugesEnum", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

    }
}