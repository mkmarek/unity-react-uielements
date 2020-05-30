using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace DocumentationGenerator
{
    class Program
    {
        static void GetAllCSharpFiles(string sDir, List<string> contents)
        {
            foreach (string d in Directory.GetDirectories(sDir))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    if (f.EndsWith(".cs"))
                    {
                        contents.Add(File.ReadAllText(f));
                    }
                }
                GetAllCSharpFiles(d, contents);
            }
        }

        static void Main(string[] args)
        {
            var path = "C:\\Projects\\unity-react-uielements\\packages\\uielements.react";
            var cSharpFiles = new List<string>();
            var comments = new List<Comment>();

            GetAllCSharpFiles(path, cSharpFiles);

            var syntaxTrees = cSharpFiles.Select(e => CSharpSyntaxTree.ParseText(e));
            var walker = new ComponentDocumentationCSharpSyntaxWalker(comment =>
            {
                if (comment.Content.Contains("docGeneration"))
                {
                    comments.Add(comment);
                }
            });

            foreach (var syntaxTree in syntaxTrees)
            {
                walker.Visit(syntaxTree.GetRoot());
            }

            Console.WriteLine("Hello World!");
        }
    }
}
