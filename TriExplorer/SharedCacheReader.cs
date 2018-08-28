﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TriExplorer.Properties;

namespace TriExplorer
{
    class SharedCacheReader
    {
        const string _scRegPath = "HKEY_CURRENT_USER\\Software\\CCP\\EVEONLINE";
        const string _scRegKey = "CACHEFOLDER";

        /// <summary>
        /// Validates existence of ResFiles folder from registry or given SharedCache path.
        /// </summary>
        /// <param name="path">Optional SharedCache path to be validated.</param>
        /// <returns>True if path is valid, or false if not.</returns>
        public static bool ValidateSCPath(string path = "")
        {
            if (String.IsNullOrEmpty(path) && 
                String.IsNullOrEmpty(Settings.Default.SCPath)) // path unspecified, reading registry
                path = (string) Registry.GetValue(_scRegPath, _scRegKey, "");

            // Save the path if valid
            var result = Directory.Exists(Path.Combine(path, "ResFiles"));
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

    /// <summary>
    /// Holds the raw data of a shared cache entry.
    /// </summary>
    public class SharedCacheEntry
    {
        public SharedCacheEntry() { }
        /// <summary>
        /// Constructs a SharedCacheEntry from a line of shared cache index.
        /// </summary>
        /// <param name="scEntry">Shared cache index to be deserialized.</param>
        public SharedCacheEntry(string scEntry)
        {
            var data = scEntry.Split(',');
            ResPath = data[0].Split('/');
            FilePath = data[1];
            Md5 = data[2];
            RawSize = Int32.Parse(data[3]);
            CompressedSize = (data.Length > 4 ? Int32.Parse(data[4]) : 0);

            CurrentDepth = 0;
            ResPathDepth = ResPath.Length;
        }
        
        // Actual path in file system
        public string FilePath { get; set; }
        public string Md5 { get; set; }
        public int RawSize { get; set; }
        public int CompressedSize { get; set; }

        public string ResName
        {
            get { return ResPath.Last(); }
        }

        private string[] ResPath;
        
        private int CurrentDepth;

        private int ResPathDepth;

        /// <summary>
        /// Reads the next path segment of this shared cache entry. Returns null if the end is reached.
        /// </summary>
        public string NextPath
        {
            get
            {
                if (CurrentDepth == ResPathDepth) return null;
                return ResPath[CurrentDepth++];
            }
        }

        public bool IsPathTraversed
        {
            get { return (CurrentDepth == ResPathDepth); }
        }
    }
}