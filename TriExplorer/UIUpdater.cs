using System;
using System.IO;
using System.Threading.Tasks;
using TriExplorer.Properties;
using TriExplorer.Types;
using TriExplorer.Utils;

namespace TriExplorer
{
    static class UIUpdater
    {
        public async static Task UpdateUiForFile(SharedCacheFile file)
        {
            UIStrings.GetInstance().IsCurrentFileValid = null;
            UIStrings.GetInstance().IsCurrentNodeFile = true;
            // Use predefined description, or system description if file is unknown
            var systemDesc = FileHelper.GetFileTypeDescription(file.DisplayName);
            var builtInDesc = file.TypeDesc;
            UIStrings.GetInstance().CurrentNodeType = (String.IsNullOrEmpty(builtInDesc) ? systemDesc : builtInDesc);
            
            UIStrings.GetInstance().CurrentFileSize = file.Info.RawSize.ToString();
            UIStrings.GetInstance().CurrentFileCompressedSize = file.Info.CompressedSize.ToString();
            
            UIStrings.GetInstance().CurrentFileName = file.Info.FilePath;
            UIStrings.GetInstance().CurrentFileHash = file.Info.Md5;

            var fileLocation = Path.Combine(Settings.Default.SCPath, "ResFiles",
                    file.Info.FilePath).Replace('/', '\\');
            UIStrings.GetInstance().IsCurrentFileValid = await FileHelper.CompareMd5(fileLocation, file.Info.Md5);
        }

        public static void UpdateUiForDir(SharedCacheDirectory dir)
        {
            UIStrings.GetInstance().IsCurrentFileValid = null;
            UIStrings.GetInstance().IsCurrentNodeFile = false;
            UIStrings.GetInstance().CurrentNodeType = "Shared Resource Path";
        }
    }
}
