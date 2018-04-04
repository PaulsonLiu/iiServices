using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iiService.Models
{
    public class TreeModel
    {
        public string State { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Text { get; set; }
        public int? Sort { get; set; }
        public int Childcount { get; set; }
        public bool Handlered { get; set; }
        public List<TreeModel> Children { get; set; }

        public TreeModel()
        {
            this.Children = new List<TreeModel>();
        }
    }

    /// <summary>
    ///级联结构显示
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeModel<T>
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Text { get; set; }
        public bool Handled { get; set; }
        public int? Sort { get; set; }
        public T Model { get; set; }
        public List<TreeModel<T>> Children { get; set; }
        public TreeModel()
        {
            this.Children = new List<TreeModel<T>>();
        }
    }
}
