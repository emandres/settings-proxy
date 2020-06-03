using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SettingsProxyApp
{
    public class SettingsProxy: DispatchProxy
    {
        private Dictionary<string, string> settingNames;

        public static TInterface Create<TInterface>(Dictionary<string, string> settingNames)
        {
            var result = DispatchProxy.Create<TInterface, SettingsProxy>();
            if (result is SettingsProxy proxy)
            {
                Console.WriteLine("setting the dictionary");
                proxy.settingNames = settingNames;
            }
            return result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            EnsureSettingNameCache(targetMethod.DeclaringType);
            return settingNames[targetMethod.Name];
        }

        private void EnsureSettingNameCache(Type type)
        {
            if (settingNames == null)
            {
                settingNames = new Dictionary<string, string>();
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var getterName = property.GetGetMethod(false).Name;
                if (settingNames.ContainsKey(getterName))
                {
                    continue;
                }

                if (property.GetCustomAttribute<SettingNameAttribute>() is SettingNameAttribute attr)
                {
                    settingNames[getterName] = attr.Name;
                }
                else
                {
                    settingNames[getterName] = Regex.Replace(getterName, "^get_", "");
                }
            }
        }
    }
}