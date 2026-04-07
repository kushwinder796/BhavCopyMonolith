using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bhav.Application.Services
{
    public class BhavCopyParserService
    {
        public async Task<List<Dictionary<string, object>>> ParseZipAsync(byte[] zipBytes)
        {
            var result = new List<Dictionary<string, object>>();

            using var ms = new MemoryStream(zipBytes);
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                // Skip directories
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                using var stream = entry.Open();
                using var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream);

                var bytes = memStream.ToArray();

                // Parse the extracted file as CSV
                var parsed = await ParseCsvAsync(bytes);
                result.AddRange(parsed);
            }

            return result;
        }

        internal async Task<List<Dictionary<string, object>>> ParseCsvAsync(byte[] bytes)
        {
            var result = new List<Dictionary<string, object>>();

           
            using var stream = new MemoryStream(bytes);
            using var reader = new StreamReader(stream, Encoding.UTF8, true);

            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
                return result;

            // Clean and split headers - handle quoted fields
            var headers = ParseCsvLine(headerLine);

            if (headers.Length == 0)
                return result;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Skip lines that are just separators or comments
                if (line.StartsWith("*") || line.StartsWith("#") || line.StartsWith("-"))
                    continue;

                var values = ParseCsvLine(line);

               
                if (values.Length == 0)
                    continue;

                var dict = new Dictionary<string, object>();

                for (int i = 0; i < headers.Length && i < values.Length; i++)
                {
                    dict[headers[i].Trim()] = values[i].Trim();
                }

                // Only add if we have data
                if (dict.Count > 0)
                    result.Add(dict);
            }

            return result;
        }

        //  create Helper method 
        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());

            return result.ToArray();
        }

        public async Task<List<Dictionary<string, object>>> ParseDelimitedAsync(byte[] bytes, char delimiter)
        {
            var result = new List<Dictionary<string, object>>();
            using var stream = new MemoryStream(bytes);
            using var reader = new StreamReader(stream, Encoding.UTF8, true);

            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
                return result;

            var headers = ParseDelimitedLine(headerLine, delimiter);
            if (headers.Length == 0)
                return result;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("*") || line.StartsWith("#") || line.StartsWith("-")) continue;

                var values = ParseDelimitedLine(line, delimiter);
                if (values.Length == 0) continue;

                var dict = new Dictionary<string, object>();
                for (int i = 0; i < headers.Length && i < values.Length; i++)
                {
                    dict[headers[i].Trim()] = values[i].Trim();
                }

                if (dict.Count > 0)
                    result.Add(dict);
            }

            return result;
        }

        private string[] ParseDelimitedLine(string line, char delimiter)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == delimiter && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}