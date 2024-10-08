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
    }
}