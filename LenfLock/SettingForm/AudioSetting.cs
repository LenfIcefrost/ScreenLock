using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LenfLock.SettingForm {
    public partial class AudioSetting : Form {
        public AudioSetting() {
            InitializeComponent();
            this.checkBox1.Checked = QuestionData.instance.Audio.Active;
            this.trackBar1.Value = QuestionData.instance.Audio.Filter;
            this.label3.Text = "db : " + Convert.ToString(QuestionData.instance.Audio.Filter);

            this.trackBar1.ValueChanged += (x, s) => {
                this.label3.Text = "db : " + Convert.ToString((x as TrackBar).Value);
            };
            this.button1.Click += (x, s) => {
                QuestionData.instance.Audio.Active = this.checkBox1.Checked;
                QuestionData.instance.Audio.Filter = this.trackBar1.Value;
                try {
                    QuestionData.Save();
                    MessageBox.Show("儲存成功");
                } catch (Exception err) {
                    MessageBox.Show(err.Message);
                }
                MainInterface.instance.add(new Setting());
            };
        }
    }
}
