using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SettingsProxyApp
{
    public class SettingsProxyBuilder<TProxy>
    {
        private readonly Dictionary<string, string> settingNames;

        public SettingsProxyBuilder()
        {
            settingNames = new Dictionary<string, string>();
        }

        public SettingsProxyBuilder<TProxy> With(Expression<Func<TProxy, object>> propExpr, string name)
        {
            var propertyInfo = (propExpr.Body as MemberExpression)?.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Expression must reference a public property", nameof(propExpr));
            }
            settingNames[propertyInfo.GetGetMethod(false).Name] = name;
            return this;
        }

        public TProxy Build()
        {
            return SettingsProxy.Create<TProxy>(settingNames);
        }
    }
}