using System;

namespace SettingsProxyApp
{
    public class SettingNameAttribute: Attribute
    {
        public string Name { get; }

        public SettingNameAttribute(string name)
        {
            Name = name;
        }
    }
}