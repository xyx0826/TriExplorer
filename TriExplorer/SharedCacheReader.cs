using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TriExplorer.Types;
using TriExplorer.Properties;

namespace TriExplorer
{
    class SharedCacheReader
    {
        const string _scRegPath = "HKEY_CURRENT_USER\\Software\\CCP\\EVEONLINE";
        const string _scRegKey = "CACHEFOLDER";

        /// <summary>
        /// Validates existence of ResFiles folder from registry or given SharedCache path.
        /// If a path is validated, it is saved to settings.
        /// </summary>
        /// <param name="path">Optional SharedCache path to be validated. 
        /// If none given, registry value is used.</param>
        /// <returns>True if path is valid, or false if not.</returns>
        public static bool ValidateSCPath(string path = "")
        {
            // Not specifying path, can be first run or using settings
            if (String.IsNullOrEmpty(path))
            {
                path = (string)Registry.GetValue(_scRegPath, _scRegKey, "");
                // Setting is fresh, use setting value
                if (!String.IsNullOrEmpty(Settings.Default.SCPath))
                    path = Settings.Default.SCPath;
            }

            // Save the path if valid
            var result = Directory.Exists(Path.Combine(path, "ResFiles"));
            Settings.Default.IsSCPathValid = result;
            if (result) Settings.Default.SCPath = path;
            return result;
        }

        /// <summary>
        /// Reads SharedCache index of the given server and returns a list of indexed files.
        /// </summary>
        /// <param name="path">Validated SharedCache path.</param>
        /// <param name="server">Server to read index from, Tranquility by default.</param>
        /// <returns>A list of files contained in the index.</returns>
        public async static Task<List<SharedCacheEntry>> ReadSCIndex(string path, string server = "tq")
        {
            string scIndexFile = Path.Combine(path, server + "\\resfileindex.txt");

            string[] scIndex;
            try
            {
                using (var reader = File.OpenText(scIndexFile))
                {
                    var fileText = await reader.ReadToEndAsync();
                    scIndex = fileText.Split
                        (new[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
            catch (FileNotFoundException e) { throw e; }

            // Put deserialization on async
            var scEntries = new List<SharedCacheEntry>(scIndex.Length);
            return await Task.Run(() =>
             {
                 foreach (string entry in scIndex)
                     if (!String.IsNullOrEmpty(entry)) scEntries.Add(new SharedCacheEntry(entry));
                 return scEntries;
             });
        }
    }
}
