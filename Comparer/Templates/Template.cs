using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Comparer.Templates
{
    public class Template
    {
        public static Template MakeTemplate(Panel profile)
        {
            Template template = new Template();
            string profileName = profile.Name;
            foreach(Panel p in profile.Controls.OfType<Panel>())
            {
                string[] contNameSplit = p.Name.Split('-');
                string contentName = (string)contNameSplit.GetValue(1);
                foreach(TextBox v in p.Controls.OfType<TextBox>())
                {
                    if ((string)v.Name.Split('-').GetValue(v.Name.Split('-').Length - 1) != "tb") continue;
                    template.AddContent(contentName, v.Text);
                    break;
                }
            }
            template.TemplateName = profileName;
            return template;
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
