using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.CoreLib.Services
{
    public class FileFolderNamingHelper
    {
        // Method to create the slate file name based on the provided frame number and version
        public static string CreateSlateFileName(string shotName, int frameNumber, string version)
        {
            // Adjust the frame number to be one less than the provided value
            int slateFrameNumber = frameNumber - 1;

            // Construct the slate file name
            return $"{shotName}.v{version}.{slateFrameNumber:D4}.exr";
        }

        // Method to create the frame file name based on the provided shot name, version, and frame number
        public static string CreateFrameFileName(string shotName, int frameNumber, string version, string fileExtension)
        {
            // Construct the frame file name
            return $"{shotName}.v{version}.{frameNumber:D4}.{fileExtension}";
        }

        // Method to create the folder name for shot versions based on the provided parameters
        public static string CreateShotVersionFolderName(string projectPrefix, int id1, int id2, int id3, string version)
        {
            // Construct the shot version folder name
            return $"{projectPrefix}_{id1:D3}_{id2:D3}_{id3:D3}_{version}";
        }

        // Method to get the main working directory
        public static string GetMainWorkingDirectory(string rootDirectory)
        {
            return Path.Combine(rootDirectory, "__transcoder__");
        }

        // Method to get the renders directory within a shot version
        public static string GetRendersDirectory(string mainWorkingDirectory, string shotVersionFolder)
        {
            return Path.Combine(mainWorkingDirectory, shotVersionFolder, "Renders");
        }
    }

    class Program
    {
        static void Main()
        {
            // Example usage:
            string mainWorkingDirectory = FileFolderNamingHelper.GetMainWorkingDirectory("D:\\Projects\\VAS");
            string shotVersionFolder = FileFolderNamingHelper.CreateShotVersionFolderName("IFTC", 107, 349, 10, "v001");
            string rendersDirectory = FileFolderNamingHelper.GetRendersDirectory(mainWorkingDirectory, shotVersionFolder);

            Console.WriteLine($"Main Working Directory: {mainWorkingDirectory}");
            Console.WriteLine($"Shot Version Folder: {shotVersionFolder}");
            Console.WriteLine($"Renders Directory: {rendersDirectory}");
        }
    }
}
