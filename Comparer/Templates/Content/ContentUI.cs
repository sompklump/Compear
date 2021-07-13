using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Comparer.Templates.Content
{
    public static class ContentUI
    {
        public static void RearangeContent(Panel p)
        {
            int lastY = 30;
            foreach (Control c in p.Controls)
            {
                c.Location = new Point(0, lastY);
                string[] nameSplit = c.Name.Split('-');
                switch ((string)nameSplit.GetValue(nameSplit.Length - 1))
                {
                    // New Content button
                    case "newCont":
                        c.Location = new Point(0, 0);
                        break;
                    case "mkTemp":
                        c.Location = new Point(25, 0);
                        break;
                    // Content panel
                    case "contPanel":
                        foreach (Control c1 in c.Controls)
                        {
                            string[] c1NameSplit = c1.Name.Split('-');
                            if ((string)c1NameSplit.GetValue(c1NameSplit.Length - 1) == "label")
                                c1.Location = new Point(0, c.Height / 2 - 5);
                            if((string)c1NameSplit.GetValue(c1NameSplit.Length - 1) == "labelNameChangeTb")
                                c1.Location = new Point(0, 0);
                            if ((string)c1NameSplit.GetValue(c1NameSplit.Length - 1) == "tb")
                                c1.Location = new Point(80, 0);
                            if ((string)c1NameSplit.GetValue(c1NameSplit.Length - 1) == "remBtn")
                                c1.Location = new Point(193, 0);
                        }
                        lastY += 32;
                        break;
                    default:
                        break;
                }
            }
            foreach (Control c in p.Controls)
            {
                string[] nameSplit = c.Name.Split('-');
                if ((string)nameSplit.GetValue(nameSplit.Length - 1) == "remProBtn")
                {
                    c.Location = new Point(0, lastY);
                    return;
                }
            }
            p.Update();
            p.Refresh();
        }
    }
}
