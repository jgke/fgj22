using Microsoft.Xna.Framework;
using Nez;
using System;
using Serilog.Events;
using System.Reflection;
using System.Collections.Generic;
using Serilog;
using Serilog.Core;

namespace Fgj22.App
{
    public interface ILoggable
    {
        Dictionary<string, object> GetFields()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            Type ty = this.GetType();
            FieldInfo[] fields = ty.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var att = (Loggable)Attribute.GetCustomAttribute(fields[i], typeof(Loggable));
                if (att != null)
                {
                    res.Add(fields[i].Name, fields[i].GetValue(this));
                }
            }
            return res;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class Loggable : Attribute { }

    public class LoggableDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            if (value is ILoggable response)
            {
                Type ty = response.GetType();
                result = propertyValueFactory.CreatePropertyValue(new Dictionary<string, Dictionary<string, object>> { { ty.Name, response.GetFields() } });
                return true;
            }

            result = null;
            return false;
        }
    }
}
