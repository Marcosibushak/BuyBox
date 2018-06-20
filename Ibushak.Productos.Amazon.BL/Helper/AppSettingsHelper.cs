using System.Configuration;

namespace Ibushak.Productos.Amazon.BL.Helper
{
    public static class AppSettingsHelper
    {
        public static string LoadAppSetting(string name)
        {
            var reader = new AppSettingsReader();
            return (string)reader.GetValue(name, typeof(string));
        }
    }
}