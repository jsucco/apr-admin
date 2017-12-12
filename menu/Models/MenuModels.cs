using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace menu.Models
{
    public class ButtonModel
    {
        public string text { get; set; }
        public string cssClass { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string AppUrl { get; set; }
        public int GridRow { get; set; }
        public int GridCol { get; set; }
        public int ColSpan { get; set; }
        public string RowSpan { get; set; }
        public string section { get; set; }
        public OptionModel[] Dropdowns { get; set; }
    }
    public class OptionModel
    {
        public string text { get; set; }
        public string name { get; set; }
        public string Id { get; set; }
        public string AppUrl { get; set; }
        public string cssClass { get; set; }
    }
}