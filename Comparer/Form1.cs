using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using HakuniAssets.IO;
using Comparer.Content;

namespace Comparer
{
    public partial class Form1 : Form
    {
        List<Template> Templates = new List<Template>();
        Dictionary<string, List<string>> ProfileSpecContNames = new Dictionary<string, List<string>>();
        string TemplatesPath = $@"{Path.GetDirectoryName(Application.ExecutablePath)}/Templates.json";
        int TemplateCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataFlowPanel.AutoSize = true;
            dataFlowPanel.AutoScroll = true;
            dataFlowPanel.Dock = DockStyle.Fill;
            SaveTemplates();
            foreach (Template tmp in LoadTemplates())
            {
                ToolStripButton button = new ToolStripButton();
                button.Text = tmp.TemplateName;
                button.Width = tmp.TemplateName.Length*5;
                button.Name = tmp.TemplateName;
                button.Click += TemplateButton_click;
                templates_toolTipDrop.DropDownItems.Add(button);
            }
        }
        public void TemplateButton_click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            NewTemplate(btn.Name);
        }

        public void NewTemplate(string tmpName)
        {
            Template template = new Template();
            foreach(Template tmp in Templates)
            {
                if (tmp.TemplateName == tmpName)
                {
                    template = tmp;
                    break;
                }
            }
            if (template == default(Template))
                return;

            Panel p = new Panel();
            p.Name = $"template_{TemplateCount}";
            dataFlowPanel.Controls.Add(p);
            Button newCont_btn = new Button();
            newCont_btn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            newCont_btn.Name = $"{p.Name}-newCont";
            newCont_btn.Click += NewContent_click;
            newCont_btn.Text = "+";
            newCont_btn.Font = new Font(newCont_btn.Font.FontFamily, 10);
            newCont_btn.AutoSize = true;
            newCont_btn.Size = new Size(12, 12);
            Button makeTemplate_btn = new Button();
            makeTemplate_btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            makeTemplate_btn.Name = $"{p.Name}-mkTemp";
            makeTemplate_btn.Click += Template.MakeTemplate_click;
            makeTemplate_btn.Text = "⎙";
            makeTemplate_btn.Font = new Font(newCont_btn.Font.FontFamily, 10);
            makeTemplate_btn.AutoSize = true;
            makeTemplate_btn.Size = new Size(12,12);

            foreach (TemplateContent cont in template.Content)
            {
                string currContName = SetContentName(p.Name, cont.Name);

                Panel contPanel = new Panel();
                contPanel.Name = $"{p.Name}-{currContName}-contPanel";
                contPanel.AutoSize = false;
                contPanel.Parent = p;

                Label label = new Label();
                label.Name = $"{p.Name}-{currContName}-label";
                label.DoubleClick += ChangeLabelName_click;

                TextBox labelNameChange_tb = new TextBox();
                labelNameChange_tb.KeyDown += ChangeLabelName_keyDown;
                labelNameChange_tb.Name = $"{p.Name}-{currContName}-labelNameChangeTb";
                labelNameChange_tb.Visible = false;

                TextBox tb = new TextBox();
                tb.Name = $"{p.Name}-{currContName}-tb";

                Button removeCont_btn = new Button();
                removeCont_btn.Name = $"{p.Name}-{currContName}-remBtn";
                removeCont_btn.Size = new Size(23, 23);
                removeCont_btn.Text = "X";
                removeCont_btn.Click += RemoveContet_click;
                removeCont_btn.TextAlign = ContentAlignment.MiddleCenter;

                label.Parent = tb.Parent = removeCont_btn.Parent = p;
                label.Text = cont.Name;
                tb.Text = cont.Value.ToString();

                label.Parent = labelNameChange_tb.Parent = tb.Parent = removeCont_btn.Parent = contPanel;
                contPanel.Size = new Size(tb.Width + removeCont_btn.Width + label.Width, tb.Height +2);
            }
            Button remove_btn = new Button();
            remove_btn.Name = $"{p.Name}-remProBtn";
            remove_btn.Text = "Remove Profile";
            remove_btn.AutoSize = true;
            remove_btn.Parent = newCont_btn.Parent = makeTemplate_btn.Parent = p;
            p.AutoSize = true;
            p.BackColor = Color.Azure;
            TemplateCount++;

            ContentUI.RearangeContent(p);
        }

        public void ChangeLabelName_click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            string[] nameSplit = lbl.Name.Split('-');
            string lblText = lbl.Text;
            string profileName = (string)nameSplit.GetValue(0);
            string contentName = (string)nameSplit.GetValue(1);
            foreach(Panel p in dataFlowPanel.Controls.OfType<Panel>())
            {
                if (p.Name != profileName) continue;
                foreach(Panel contPanel in p.Controls.OfType<Panel>())
                {
                    if ((string)contPanel.Name.Split('-').GetValue(1) != contentName) continue;
                    foreach(Control c in contPanel.Controls)
                    {
                        string eleName = (string)c.Name.Split('-').GetValue(2);
                        if (eleName == "label") c.Visible = false;
                        if (eleName == "labelNameChangeTb")
                        {
                            c.Text = lblText;
                            c.Visible = true;
                        }
                    }
                }
            }
        }
        public void ChangeLabelName_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            TextBox tb = (TextBox)sender;
            string[] nameSplit = tb.Name.Split('-');
            string tbText = tb.Text;
            string profileName = (string)nameSplit.GetValue(0);
            string contentName = (string)nameSplit.GetValue(1);
            foreach (Panel p in dataFlowPanel.Controls.OfType<Panel>())
            {
                if (p.Name != profileName) continue;
                foreach (Panel contPanel in p.Controls.OfType<Panel>())
                {
                    if ((string)contPanel.Name.Split('-').GetValue(1) != contentName) continue;
                    contPanel.Name = $"{profileName}-{SetContentName(profileName, tbText, contentName)}";
                    foreach (Control c in contPanel.Controls)
                    {
                        string eleName = (string)c.Name.Split('-').GetValue(2);
                        if (eleName == "label")
                        {
                            c.Text = tbText;
                            c.Visible = true;
                        }
                        if (eleName == "labelNameChangeTb") c.Visible = false;
                    }
                }
            }
        }

        public void NewContent_click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string panelName = (string)button.Name.Split('-').GetValue(0);
            foreach (Panel p in dataFlowPanel.Controls.OfType<Panel>())
            {
                if (p.Name == panelName)
                {
                    Panel contPanel = new Panel();
                    Label lbl = new Label();
                    TextBox lblNameChangeTb = new TextBox();
                    TextBox tb = new TextBox();
                    Button btn = new Button();
                    lbl.Parent = lblNameChangeTb.Parent = tb.Parent = btn.Parent = p;

                    string contName = SetContentName(panelName, string.Empty);

                    ContentUI.RearangeContent(p);
                    return;
                }
            }
        }
        public void RemoveContet_click(object sender, EventArgs e)
        {
            List<string> contNames;
            Button button = (Button)sender;
            string[] nameSplit = button.Name.Split('-');
            string profileName = (string)nameSplit.GetValue(0);
            string contentName = (string)nameSplit.GetValue(1);

            // Remove from content list
            if (!ProfileSpecContNames.TryGetValue(profileName, out contNames))
                contNames = new List<string>();

            if (contNames.Contains(contentName))
                contNames.Remove(contentName);

            // Search for the profile
            foreach (Panel p in dataFlowPanel.Controls.OfType<Panel>())
            {
                if (p.Name != profileName) continue;
                // Search for the specific content
                foreach (Panel contPanel in p.Controls.OfType<Panel>())
                {
                    if ((string)contPanel.Name.Split('-').GetValue(1) == contentName) continue;
                    foreach (Control c in contPanel.Controls)
                    {
                        contPanel.Controls.Remove(c);
                        c.Dispose();
                    }
                    p.Controls.Remove(contPanel);
                    ContentUI.RearangeContent(p);
                    return;
                }
            }
        }

        public string SetContentName(string profileName, string contName, string oldName = null)
        {
            List<string> contNames;
            if (!ProfileSpecContNames.TryGetValue(profileName, out contNames))
                contNames = new List<string>();

            contName = contName.Replace(" ", string.Empty);
            int currNameCount = 0;
            if (contNames.Contains(contName))
            {
                List<string> reversedContNames = contNames;
                reversedContNames.Reverse();
                foreach (string s in reversedContNames)
                {
                    string[] split = s.Split('_');
                    if ((string)split.GetValue(0) == contName)
                    {
                        currNameCount = (int)split.GetValue(split.Length - 1) + 1;
                        break;
                    }
                }
            }
            string newName = $"{contName}_{currNameCount}";
            if (!string.IsNullOrWhiteSpace(oldName))
                contNames.Remove(oldName);
            contNames.Add(newName);

            // Rename all the elements
            if (!string.IsNullOrWhiteSpace(oldName))
            {
                foreach (Panel p in dataFlowPanel.Controls.OfType<Panel>())
                {
                    if (p.Name != profileName) continue;
                    foreach (Panel contPanel in p.Controls.OfType<Panel>())
                    {
                        if ((string)contPanel.Name.Split('-').GetValue(1) != oldName) continue;
                        MessageBox.Show($"Contpanel : {contPanel.Name}");
                        foreach (Control c in contPanel.Controls)
                        {
                            MessageBox.Show($"Old Name: {c.Name}");
                            string[] nameSplit = c.Name.Split('-');
                            nameSplit[1] = newName;
                            c.Name = String.Join("-", nameSplit);
                            MessageBox.Show($"New Name: {c.Name}");
                        }
                        string[] contPNameSplit = contPanel.Name.Split('-');
                        contPNameSplit[1] = newName;
                        contPanel.Name = string.Join("-", contPNameSplit);
                        MessageBox.Show(contPanel.Name);
                    }
                }
            }

            if (ProfileSpecContNames.ContainsKey(profileName))
                ProfileSpecContNames[profileName] = contNames;
            else
                ProfileSpecContNames.Add(profileName, contNames);
            return newName;
        }

        List<Template> LoadTemplates()
        {
            Templates = new List<Template>();

            if (!File.Exists(TemplatesPath))
                return Templates;

            using (StreamReader file = File.OpenText(TemplatesPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                Templates = (List<Template>)serializer.Deserialize(file, typeof(List<Template>));
            }
            return Templates;
        }
        bool SaveTemplates()
        {
            List<Template> Templates = new List<Template>();
            if (!File.Exists(TemplatesPath))
                File.CreateText(TemplatesPath);
            Template template = new Template("Car Bass Box");
            template.AddContent("Name", string.Empty);
            template.AddContent("Comfort", 7);
            template.AddContent("Quality", 9);
            template.AddContent("Feel", 5);
            Templates.Add(template);

            JSON.SaveJsonFile(TemplatesPath, Templates);
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveTemplates();
        }
    }
}
