using System;
using System.Collections.Generic;

namespace Comparer.Content
{
    public class Template
    {
        public static void MakeTemplate_click(object sender, EventArgs e)
        {

        }

        public string TemplateName;
        public List<TemplateContent> Content = new List<TemplateContent>();

        public Template(string templateName = "Template")
        {
            TemplateName = templateName;
        }
        #region Methods
        public void AddContent(string name, object value)
        {
            TemplateContent tmpCont = new TemplateContent(name, value);
            Content.Add(tmpCont);
        }
        public void RemoveContent(string name)
        {
            foreach (TemplateContent cont in Content)
            {
                if (cont.Name.Equals(name))
                    Content.Remove(cont);
            }
        }
        #endregion
    }
    public partial class TemplateContent
    {
        public string Name;
        public object Value;
        public TemplateContent(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
