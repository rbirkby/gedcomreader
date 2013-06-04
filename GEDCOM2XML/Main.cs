// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.IO;
using System.Linq;
using System.Xml;
using NDesk.Options;    

namespace ThunderMain.Gedcom2Xml
{
    /// <summary>
    /// Outputs the given GEDCOM file in XML.
    /// </summary>
    class Program
    {
        private static void PrintHelp()
        {
            Console.WriteLine("GEDCOM2XML");
            Console.WriteLine("Usage: GEDCOM2XML [/s] <filename.ged> ");
            Console.WriteLine("/s - Suppress all output");
            Console.WriteLine("If filename ends with .xml then it is treated as an XML document.");
        }

        static XmlReader GetReader(string uri)
        {
            if (uri.EndsWith(".xml"))
            {
                TextReader textReader = new StreamReader(uri);
                return new XmlTextReader(textReader);
            }

            Stream stream = new FileStream(uri, FileMode.Open, FileAccess.Read);
            return new GedcomReader.GedcomReader(stream);
        }

        static void Main(string[] args)
        {
            bool showHelp = false;
            bool suppressOutput = false;

            var options = new OptionSet {
                { "s", v => suppressOutput = true },
                { "h|?|help", v => showHelp = true }
            };

            string filename = options.Parse(args).FirstOrDefault() ?? string.Empty;

            if (args.Length == 0 || showHelp)
            {
                PrintHelp();
                return;
            }

            if(!File.Exists(filename))
            {
                Console.Error.WriteLine("File {0} does not exist", filename);
                return;
            }

            XmlReader reader;
            TextWriter outputStream;

            if (suppressOutput)
            {
                // Suppress output
                reader = GetReader(filename);
                outputStream = TextWriter.Null;
            }
            else
            {
                reader = GetReader(filename);

                // Console.Out is a StreamWriter which flushes it's
                // buffer after every call to write. This is due to
                // setting AutoFlush to true. (according to the docs)

                // 256 should be the default buffer size
                Console.OpenStandardOutput(256);

                outputStream = Console.Out;
            }

            var writer = new XmlTextWriter(outputStream) {Formatting = Formatting.Indented};
            writer.WriteNode(reader, false);
        }
    }
}