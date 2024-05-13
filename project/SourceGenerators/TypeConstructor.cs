﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGenerators;

public class TypeConstructor(string Name, string Namespace, params string[] Usings) {
    private readonly List<string> _fields = [];
    private readonly Dictionary<string, string> _methods = [];

    public void Clear() {
        _fields.Clear();
        _methods.Clear();
    }

    public void AddField(string name, string type, string initializer = null, bool instanced = true) {
        string fieldDefinition = "public ";
        if (!instanced) {
            fieldDefinition += "static ";
        }
        fieldDefinition += $"{type} {name}";
        if (initializer != null) {
            fieldDefinition += $" = new {type}({initializer});";
        }
        else {
            fieldDefinition += ";";
        }

        _fields.Add(fieldDefinition);
    }
    public void AddMethod(string Define, string Code) {
        if (_methods.TryGetValue(Define, out string existingCode)) {
            Code = $"{existingCode}\n{Code}";
        }

        _methods[Define] = Code; 
    }

    public void AddSource(in GeneratorExecutionContext context) {
        StringBuilder stringBuilder = new StringBuilder();

        string tabs = "";

        try {
            // Add using statements.
            AppendLine("using System.Runtime.CompilerServices;");
            AppendLine("using Terraria.ModLoader;");

            if (Usings != null && Usings.Length > 0) {
                foreach (string s in Usings) {
                    AppendLine($"using {s};");
                }
            }

            AppendLine();

            // Add namespace
            AppendLine($"namespace {Namespace};");
            AppendLine();

            // Add class definition
            AppendLine($"public partial class {Name} {{");

            TabFor();

            foreach (string field in _fields) {
                AppendLine($"[CompilerGenerated]");
                AppendLine(field);
            }

            foreach (KeyValuePair<string, string> method in _methods) {
                string def = method.Key;
                string code = method.Value;
                AppendLine();

                AppendLine($"[CompilerGenerated]");
                AppendLine($"private void {method.Key} {{");
                TabFor();
                AppendLine(code);
                TabBack();
                AppendLine($"}}");
            }

            TabBack();

            // End class definition
            Append("}");
        }
        catch (Exception ex) {
            stringBuilder.AppendLine(ex.ToString());

            stringBuilder.Append("*/");
            stringBuilder.Insert(0, "/*");
        }

        context.AddSource($"{Name}.cs", stringBuilder.ToString());

        void TabFor() {
            tabs += Utilities.TabForward;
        }

        void TabBack() {
            tabs = tabs.Substring(0, tabs.Length - Utilities.TabForward.Length);
        }

        void Append(string text) {
            stringBuilder.Append($"{tabs}{text.Replace("\n", $"\n{tabs}")}");
        }
        void AppendLine(string text = "") {
            stringBuilder.AppendLine($"{tabs}{text.Replace("\n", $"\n{tabs}")}");
        }
    }
}
