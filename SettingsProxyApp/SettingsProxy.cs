using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace SettingsProxyApp
{
    public class SettingsProxy: DispatchProxy
    {
        private Dictionary<string, SettingInfo> settingInfos;
        private IConfigurationRoot configurationRoot;

        private class SettingInfo
        {
            public string Name { get; set; }
            public Type Type { get; set; }
        }

        public static TInterface Create<TInterface>(Dictionary<string, string> settingNames, IConfigurationRoot configurationRoot)
        {
            var result = DispatchProxy.Create<TInterface, SettingsProxy>();
            if (result is SettingsProxy proxy)
            {
                Console.WriteLine("setting the dictionary");
                proxy.OverrideSettingNames(settingNames);
                proxy.configurationRoot = configurationRoot;
            }
            
            return result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            EnsureSettingNameCache(targetMethod.DeclaringType);
            var settingInfo = settingInfos[targetMethod.Name];
            return configurationRoot.GetValue(settingInfo.Type, settingInfo.Name);
        }

        private void OverrideSettingNames(Dictionary<string, string> settingNames)
        {
            settingInfos = settingNames.ToDictionary(x => x.Key, x => new SettingInfo {Name = x.Value});
        }

        private void EnsureSettingNameCache(Type type)
        {
            if (settingInfos == null)
            {
                settingInfos = new Dictionary<string, SettingInfo>();
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var getter = property.GetGetMethod(false);
                var getterName = getter.Name;
                if (!settingInfos.ContainsKey(getterName))
                {
                    settingInfos[getterName] = new SettingInfo();
                }

                settingInfos[getterName].Type = getter.ReturnType;
                if (property.GetCustomAttribute<SettingNameAttribute>() is SettingNameAttribute attr)
                {
                    settingInfos[getterName].Name = attr.Name;
                }
                else
                {
                    settingInfos[getterName].Name = Regex.Replace(getterName, "^get_", "");
                }
            }
        }
    }
}
