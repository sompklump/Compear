using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comparer.Templates
{
    public partial class InputBox : Form
    {
        public InputBoxResponse Response = new InputBoxResponse();
        public InputBox()
        {
            InitializeComponent();
        }
        public InputBoxResponse ShowDialog(string title, string header, string inputText = null)
        {
            this.Text = title;
            header_lbl.Text = header;
            inputBox_input.Text = null;
            this.ShowDialog();
            return Response;
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            Response.state = InputBoxState.Ok;
            Response.input = inputBox_input.Text;
            this.Close();
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            Response.state = InputBoxState.Cancel;
            this.Close();
        }
    }
    public class InputBoxResponse
    {
        public string input = null;
        public InputBoxState state;
    }
    public enum InputBoxState
    {
        Ok,
        Cancel
    }
}
