using GreenDragonTranscoder.Hybrid.Properties;
 
namespace GreenDragonTranscoder.Hybrid.Helpers
{
    public class ResourceHelper
    {
        private static string _namespace = "GreenDragonTranscoder.Hybrid.Resources";

        public static Stream? GetFont(string fileName)
        {
            return GetResource("Fonts", fileName);
        }

        public static Stream? GetImage(string fileName)
        {
            return GetResource("Images", fileName);
        }

        public static Stream? GetRaw(string fileName)
        {
            return GetResource("Raw", fileName);
        }

        public static Stream? GetResource(string fileName)
        {
            var stream = Resources.ResourceManager.GetStream($"{_namespace}.{fileName}");
            return stream;
        } 

        public static Stream? GetResource(string folder, string fileName)
        {
            var stream = Resources.ResourceManager.GetStream($"{_namespace}.{folder}.{fileName}");
            return stream;
        }
    }
}
