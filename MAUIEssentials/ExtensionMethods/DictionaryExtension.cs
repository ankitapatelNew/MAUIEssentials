namespace MAUIEssentials.ExtensionMethods
{
    public static  class DictionaryExtension
    {
        public static void AddOrUpdate(this Dictionary<string,object> dictionary,string key, object value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
