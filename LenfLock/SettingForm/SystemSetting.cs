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
    public partial class SystemSetting : Form {
        public SystemSetting() {
            InitializeComponent();
            ErrorProvider error = new ErrorProvider();
            textBox1.DataBindings.Add(new Binding("Text", QuestionData.instance.System, "TimeForRecall", true));
            textBox1.Validating += (x, e) => {
                error.SetError((x as TextBox), int.TryParse((x as TextBox).Text, out _) ? "" : "The value need to be a number");
            };
            this.comboBox1.DataSource = Enum.GetValues(typeof(QuestionType));
            this.comboBox1.DataBindings.Add(new Binding("SelectedValue", QuestionData.instance.System, "QuestionType", true, DataSourceUpdateMode.OnPropertyChanged));
            button1.Click += (x, e) => {
                if (error.GetError(textBox1) != "") {
                    MessageBox.Show("請確認輸入是否正確");
                    return;
                }
                try {
                    QuestionData.Save();
                    MessageBox.Show("儲存成功");
                } catch(Exception err) {
                    MessageBox.Show(err.Message);
                }
                MainInterface.instance.add(new Setting());
            };
        }
    }
}
