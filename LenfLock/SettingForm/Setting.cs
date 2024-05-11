using LenfLock.SettingForm;
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
    public partial class Setting : Form {
        public Setting() {
            InitializeComponent();
            button1.Click += (x, e) => {
                MainInterface.instance.add(new QuestionForm());
            };
        }

        private void 基本設定ToolStripMenuItem_Click(object sender, EventArgs e) {
            add(new PersonalSetting());
        }
        private void 系統設定ToolStripMenuItem_Click(object sender, EventArgs e) {
            add(new SystemSetting());
        }

        private void 數學設定ToolStripMenuItem_Click(object sender, EventArgs e) {
            add(new MathSetting());
        }

        private void 英文設定ToolStripMenuItem_Click(object sender, EventArgs e) {
            add(new EnglishSetting());
        }
        public void add(Form form) {
            form.TopLevel = false;
            panel1.Controls.Clear();
            panel1.Controls.Add(form);
            updateFormSize(form);
            form.Show();
        }
        public void updateFormSize(Form form) {
            this.Size = new Size(form.Size.Width, form.Size.Height + this.menuStrip1.Height);
            MainInterface.instance.updateFormSize(this);
        }
    }
}
