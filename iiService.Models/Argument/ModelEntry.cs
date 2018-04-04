using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace iiService.Models
{
    /// <summary>
    /// 数据模型类(通用数据操作模型)
    /// </summary>
    [DataContract]
    public class ModelEntry
    {
        /// <summary>
        /// 当前的对象名称
        /// </summary>
        [DataMember]
        public string ObjectName { get; set; }
        /// <summary>
        /// 模型名称(模型的类名)
        /// 也可以是对象主模型的名称
        /// </summary>
        [DataMember]
        public string ModelName { get; set; }
        /// <summary>
        /// 模型的状态
        /// </summary>
        [DataMember]
        public ModelState ModelState { get; set; }
        /// <summary>
        /// 模型的关键字值(用于删除操作)
        /// </summary>
        [DataMember]
        public string ModelPK { get; set; }
        /// <summary>
        /// 当前的值
        /// </summary>
        [DataMember]
        public ModelPropertyValues CurrentValues { get; set; }
        /// <summary>
        /// 原始的值
        /// </summary>
        [DataMember]
        public ModelPropertyValues OriginalValues { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelEntry()
        {
            this.CurrentValues = new ModelPropertyValues();
            this.OriginalValues = new ModelPropertyValues();
        }
        /// <summary>
        /// 设置一个值
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">字段值</param>
        /// <param name="dispalyValue">字段的参照值</param>
        public virtual void SetValue(string name, string value, string dispalyValue)
        {
            if (this.CurrentValues.PropertyValues.ContainsKey(name))
            {
                this.CurrentValues.PropertyValues[name] = value;
            }
            else
            {
                this.CurrentValues.PropertyValues.Add(name, value);
            }
            if (dispalyValue != null)
            {
                if (this.CurrentValues.PropertyValues.ContainsKey(name + "_DISP"))
                {
                    this.CurrentValues.PropertyValues[name + "_DISP"] = dispalyValue;
                }
                else
                {
                    this.CurrentValues.PropertyValues.Add(name + "_DISP", dispalyValue);
                }
            }
        }
        /// <summary>
        /// 按照索引访问
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="defalultValue">默认值</param>
        /// <returns></returns>
        public virtual FieldValue this[string name, string defalultValue = null]
        {
            get
            {
                FieldValue theValue = new FieldValue();
                if (this.CurrentValues != null && name != null)
                {
                    if (this.CurrentValues.PropertyValues.ContainsKey(name))
                    {
                        if (this.CurrentValues.PropertyValues.ContainsKey(name + "_DISP"))
                        {
                            theValue.Value = this.CurrentValues.PropertyValues[name];
                            theValue.DisplayValue = this.CurrentValues.PropertyValues[name + "_DISP"];
                        }
                        else
                        {
                            theValue.Value = this.CurrentValues.PropertyValues[name];
                            theValue.DisplayValue = this.CurrentValues.PropertyValues[name];

                        }
                    }
                }
                if (string.IsNullOrEmpty(theValue.DisplayValue))
                {
                    theValue.DisplayValue = defalultValue;
                }
                if (string.IsNullOrEmpty(theValue.Value))
                {
                    theValue.Value = defalultValue;
                }
                return theValue;
            }
             set { }
        }


    }
}
