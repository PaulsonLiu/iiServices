using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace iiService.Models
{
    [DataContract]
    public class ModelPropertyValues
    {
        [DataMember]
        public Dictionary<string, string> PropertyValues { get; set; }
        [DataMember]
        public Type ModelType { get; set; }

        public ModelPropertyValues()
        {
            this.PropertyValues = new Dictionary<string, string>();
        }

        public Type GetPropertyType(string name)
        {
            return ModelType.GetProperty(name.Trim()).PropertyType;

        }
        public string this[string key]
        {
            get
            {
                if (this.PropertyValues.ContainsKey(key))
                {
                    return this.PropertyValues[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (this.PropertyValues.ContainsKey(key))
                {
                    this.PropertyValues[key] = value;
                }
                else
                {
                    this.PropertyValues.Add(key, value);
                }

            }
        }
    }

    [DataContract]
    public class ModelPropertyValue
    {
        [DataMember]
        public string PropertyName { get; set; }
        [DataMember]
        public string PropertyValue { get; set; }
        [DataMember]
        public Type PropertyType { get; set; }
    }
}
