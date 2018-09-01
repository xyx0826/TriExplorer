using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TriExplorer.Types
{
    /// <summary>
    /// A node in TreeView.
    /// </summary>
    public abstract class SharedCacheNode
    {
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// A file node in TreeView.
    /// </summary>
    public class SharedCacheFile : SharedCacheNode
    {
        public SharedCacheEntry Info { get; set; }

        public override string ToString()
        {
            return DisplayName + " (File)";
        }

        public Style TypeIcon
        {
            get
            {
                return NodeCategories.Instance
                    .FindIcon(Info.ResName.Split('.').Last());
            }
        }

        public string TypeDesc
        {
            get
            {
                return NodeCategories.Instance
                    .FindDesc(Info.ResName.Split('.').Last());
            }
        }
    }

    /// <summary>
    /// A directory node in TreeView.
    /// </summary>
    public class SharedCacheDirectory : SharedCacheNode
    {
        public List<SharedCacheNode> Files { get; set; }

        public SharedCacheDirectory() { Files = new List<SharedCacheNode>(); }

        public override string ToString()
        {
            return DisplayName + " (Directory)";
        }

        public Style TypeIcon
        {
            get
            {
                return NodeCategories.Instance
                  .FindIcon("FOLDER");
            }
        }
    }


}
