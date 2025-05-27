using System.Diagnostics;
using System.Reflection;
using MAUIEssentials.AppCode.Helpers;
using Newtonsoft.Json.Linq;

namespace MAUIEssentials
{
    public class AppSettingsManager
    {
        private static AppSettingsManager? _instance;
        private JObject _secrets;

        private const string Namespace = "MAUIEssentials";
        private const string FileName = "appsettings.json";

        // Creates the instance of the singleton
        private AppSettingsManager()
        {
            try
            {
                var a = Assembly.GetExecutingAssembly();

                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppSettingsManager)).Assembly;
                var stream = assembly.GetManifestResourceStream($"{Namespace}.{FileName}");
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        _secrets = JObject.Parse(json);

                        if (reader != null)
                        {
                            reader.Dispose();
                            stream.Dispose();
                        }
                    }
                }
                else
                {
                    _secrets = JObject.Parse("{}");
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static AppSettingsManager Settings
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSettingsManager();
                }

                return _instance;
            }
        }

        public string this[string name]
        {
            get
            {
                try
                {
                    var path = name.Split(':');

                    JToken? node = _secrets[path[0]];
                    for (int index = 1; index < path.Length; index++)
                    {
                        node = node?[path[index]];
                    }

                    return node?.ToString();
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Unable to retrieve secret '{name}'");
                    return string.Empty;
                }
            }
        }
    }
}
