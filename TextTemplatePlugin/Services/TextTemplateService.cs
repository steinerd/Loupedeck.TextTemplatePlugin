namespace Loupedeck.TextTemplatePlugin.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Security.Cryptography;
    using HandlebarsDotNet;

    public class TextTemplateService
    {
        internal static readonly ConcurrentDictionary<string, Template> Templates = new ConcurrentDictionary<string, Template>();
        private static string FULL_PATH { get => Path.Combine(TextTemplatePlugin.UserProfilePath, TextTemplatePlugin.DEFAULT_PATH); }

        private readonly TextTemplatePlugin Plugin;
        
        private static Regex SectionRegex(string section) =>
            new Regex($@"((,)(\s+?))?(---- \|{section} ----)\r?\n(?<content>.*?)\r?\n(---- /{section} ----)",
                RegexOptions.Singleline | RegexOptions.RightToLeft);
        private static readonly Regex TemplateRegex = SectionRegex("Template");
        private static readonly Regex PatternRecongizerRegex = new Regex(@"{{(.+)}}", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public TextTemplateService()
        {
            this.Plugin = TextTemplatePlugin.Instance;
            this.Init();
        }
        private void Init()
        {
            this._loadFromPath(FULL_PATH);
        }

        private void _loadFromPath(string path)
        {
            var templateFiles = System.IO.Directory.GetFiles(path, "*.tt");

            foreach (var filePath in templateFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileBody = File.ReadAllText(filePath, Encoding.UTF8);
                var template = TemplateRegex.Match(fileBody);
                var templateContent = template.Groups["content"].Value;

                var fields = PatternRecongizerRegex.Matches(fileBody)
                    .Cast<Group>()
                    .ToList();

                var fieldCollection = new Dictionary<string, Template.Field>(fields.Count);
                fields.ForEach(f =>
                {
                    var fieldName = (f as Match).Groups[1].Value;
                    var fieldReplacement = SectionRegex(fieldName).Match(fileBody)?.Groups["content"]?.Value;
                    Template.Field field = null;

                    if (fieldName.StartsWith("file"))
                    {
                        var fpSplit = fieldReplacement.Split('|');
                        var fprPath = fpSplit[0];
                        var fprRule = fpSplit[1];
                        var fieldFilePath = Environment.ExpandEnvironmentVariables(fprPath);
                        var fieldFileName = Path.GetFileNameWithoutExtension(fieldFilePath);
                        var fieldFileNameExt = Path.GetExtension(fieldFilePath);
                        field = new Template.Field(fieldName, fieldFilePath);

                        field.IsFileRef = true;

                        field.FileRefPath = fieldFilePath;
                        field.FileRefRule = fprRule;
                        field.FileRefType = fieldFileNameExt.Equals(".json", StringComparison.OrdinalIgnoreCase)
                            ? FieldFileRefType.Json
                            : fieldFileNameExt.Equals(".xml", StringComparison.OrdinalIgnoreCase) ? FieldFileRefType.Xml : FieldFileRefType.Text;

                    }
                    else
                    {
                        field = new Template.Field(fieldName, fieldReplacement);
                    }


                    fieldCollection.Add(fieldName, field);
                });

                Templates.TryAdd(fileName, new Template(fileName, templateContent, fieldCollection));

            }
        }

    }

    public enum FieldFileRefType
    {
        Text,
        Json,
        Xml
    }

    public class Template
    {
        public Dictionary<string, Field> Fields { get; private set; }

        public string Name { get; private set; }

        public string Content { get; private set; }

        public string FilePath { get; private set; }

        public Template(string name, string content, Dictionary<string, Field> fields)
        {
            this.Name = name;
            this.Content = content;
            this.Fields = fields;
        }

        public string Compile()
        {
            var comp = Handlebars.Compile(this.Content);

            return comp(this.Fields.ToDictionary(k => k.Key, v => v.Value.Value));
        }


        public class Field
        {
            public string Name { get; private set; }
            public string Value { get => !this.IsFileRef ? this._value : this._getValue(); }
            private readonly string _value;

            public bool IsFileRef { get; set; }

            public string FileRefPath { get; set; }
            public FieldFileRefType FileRefType { get; set; }
            public string FileRefRule { get; set; }

            public Field(string name, string path, string value = null)
            {
                this.Name = name;
                this._value = value;
                this.FileRefPath = path;
            }

            private string _getValue()
            {
                var value = string.Empty;
                if (this.FileRefType != FieldFileRefType.Text)
                {
                    var fileObject = this.FileRefType == FieldFileRefType.Xml
                        ? JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeXNode(XDocument.Load(this.FileRefPath)))
                        : JsonConvert.DeserializeObject<JObject>(File.ReadAllText(this.FileRefPath));

                    value = fileObject.SelectToken(this.FileRefRule).ToString();
                }
                else
                {
                    var dynamicRegex = new Regex(this.FileRefRule, RegexOptions.IgnoreCase);
                    value = dynamicRegex.Match(File.ReadAllText(this.FileRefPath)).Groups["replacement"].Value;
                }

                return value;
            }

        }
    }

}
