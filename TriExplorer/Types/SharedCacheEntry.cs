using System;
using System.Linq;

namespace TriExplorer.Types
{
    /// <summary>
    /// Holds the raw data of a shared cache entry.
    /// </summary>
    public class SharedCacheEntry
    {
        #region Properties
        // File (actual) path
        public string FilePath;
        // File expected raw size
        public int RawSize;
        // File expected compressed size (unspecified == 0)
        public int CompressedSize;
        // File expected checksum
        public string Md5;

        // Resource (pretty) path, sliced up
        private string[] ResPath;
        // Resource (pretty) path depth
        private int ResPathDepth;
        // (For parsing) current traversed depth
        private int CurrentDepth;
        #endregion

        #region Constructors
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
        #endregion

        #region Auxilary properties
        /// <summary>
        /// The resource name of this shared cache entry.
        /// </summary>
        public string ResName
        {
            get { return ResPath.Last(); }
        }

        /// <summary>
        /// The extension of resource name of this shared cache entry.
        /// </summary>
        public string ResExtension
        {
            get { return ResName.Split('.').Last(); }
        }

        /// <summary>
        /// The next path segment of this shared cache entry. 
        /// Returns null if the end is reached.
        /// </summary>
        public string NextPath
        {
            get
            {
                if (CurrentDepth == ResPathDepth) return null;
                return ResPath[CurrentDepth++];
            }
        }

        /// <summary>
        /// Whether the path of this shared cache entry 
        /// has been fully read.
        /// </summary>
        public bool IsPathTraversed
        {
            get { return (CurrentDepth == ResPathDepth); }
        }
        #endregion
    }
}
