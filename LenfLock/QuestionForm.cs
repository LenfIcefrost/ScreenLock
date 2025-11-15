using LenfLock.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LenfLock {
    public partial class QuestionForm : Form {
        QuestionPack pack;
        public QuestionForm(bool freeze = false) {
            InitializeComponent();

            textBox2.TextChanged += (x, e) => {
                if(textBox2.Text == QuestionData.instance.Pinstance.password)
                    MainInterface.instance.add(new Setting());
            };

            start();

            button1.Click += (x, e) => {
                if(pack.Check(textBox1.Text.Trim(new char[] { '\n', '\r', ' ' }))) {
                    textBox1.Text = "";
                    MainInterface.instance.hide();
                }
            };
            textBox1.KeyDown += (x, e) => {
                if(e.KeyCode == Keys.Enter && pack.Check(textBox1.Text.Trim(new char[] { '\n', '\r', ' ' }))) {
                    textBox1.Text = "";
                    MainInterface.instance.hide();
                }
            };
            button1.Visible = !freeze;
            textBox1.Visible = !freeze;
            textBox2.Visible = !freeze;
            label1.Text = freeze ? "This device has already been frozen, \nplease check with admin." : label1.Text;
        }
        public void start() {
            this.pack = QuestionData.instance.Generate();
            label1.Text = $"Question：\n{pack.Question}";
        }
    }
}
