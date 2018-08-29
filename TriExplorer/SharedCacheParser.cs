using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TriExplorer
{
    class SharedCacheParser
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
                    switch (Info.ResName.Split('.').Last())
                    {
                        case "gr2":
                            return Application.Current.FindResource("ModelIcon") as Style;
                        case "dds":
                        case "png":
                            return Application.Current.FindResource("ImageIcon") as Style;
                        case "black":
                            return Application.Current.FindResource("SceneIcon") as Style;
                        case "sm_depth":
                        case "sm_hi":
                        case "sm_lo":
                            return Application.Current.FindResource("EffectIcon") as Style;
                        case "bnk":
                        case "wem":
                            return Application.Current.FindResource("AudioIcon") as Style;
                        case "xml":
                        case "yaml":
                            return Application.Current.FindResource("YamlIcon") as Style;
                        default:
                            return Application.Current.FindResource("MiscIcon") as Style;
                    }
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
                get { return Application.Current.FindResource("FolderIcon") as Style; }
            }
        }

        /// <summary>
        /// Takes a list of SharedCacheEntry 
        /// and recursively parse it into a tree 
        /// of SharedCacheNodes for displaying in 
        /// TreeView.
        /// </summary>
        /// <param name="entries">A list of SharedCacheEntry to be parsed.</param>
        /// <returns>A hierarchical list of SharedCacheNode.</returns>
        public async static Task<List<SharedCacheNode>> PopulateItemTree(List<SharedCacheEntry> entries)
        {
            return await await Task.Factory.StartNew(async () =>
              {
                  var nodes = new List<SharedCacheNode>();

                 // group up nodes by their directories
                 var tree =
                      from entry in entries
                      group entry by entry.NextPath into newTree
                      orderby newTree.Key
                      select newTree;

                  foreach (var branch in tree)   // every subnode is a group of files sharing a same path
                 {
                      if (branch.ElementAt(0).IsPathTraversed)
                      {
                          nodes.Add(new SharedCacheFile()    // path traversed - we've reached a file node
                         {
                              DisplayName = branch.Key,
                              Info = branch.ElementAt(0)
                          });
                          UIStrings.GetInstance().LoadingProgValue++;
                      }
                      else
                      {
                          nodes.Add(new SharedCacheDirectory()    // path to be traversed - we're still in directories
                         {
                              DisplayName = branch.Key,
                              Files = await PopulateItemTree(branch.ToList())   // recursively parse directory contents
                         });
                      }
                  }

                  return nodes;
              });
        }
    }
}
