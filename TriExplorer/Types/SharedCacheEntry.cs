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
        /// <summary>
        /// Actual file path (on disk).
        /// </summary>
        public string FilePath;
        /// <summary>
        /// Expected file raw size.
        /// </summary>
        public int RawSize;
        /// <summary>
        /// Expected file compressed size. 0 if unspecified.
        /// </summary>
        public int CompressedSize;
        /// <summary>
        /// Expected file checksum.
        /// </summary>
        public string Md5;

        /// <summary>
        /// File resource path, sliced up.
        /// </summary>
        private string[] ResPath;
        /// <summary>
        /// File resource path depth.
        /// </summary>
        private int ResPathDepth;
        /// <summary>
        /// (For parsing) current traversed depth.
        /// </summary>
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
