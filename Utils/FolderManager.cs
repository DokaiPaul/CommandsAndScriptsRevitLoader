using System.IO;

namespace R2022.Utils
{
    public static class FolderManager
    {
        
        public static void DeleteAllFilesInFolder(string folderPath)
        {
            var di = new DirectoryInfo(folderPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
        
        public static void TryDeleteAllFilesInFolder(string folderPath)
        {
            var di = new DirectoryInfo(folderPath);
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}