using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriExplorer.Types;

namespace TriExplorer
{
    class SharedCacheParser
    {
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
