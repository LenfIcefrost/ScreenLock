using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LenfLock.SettingForm {
    public partial class EnglishSetting : Form {
        public EnglishSetting() {
            InitializeComponent();

            this.dataGridView1.Rows.Clear();

            foreach(var question in QuestionData.instance.englishQuestion.englishDictionaries
                .Select(x => new { Chinese = String.Join(",", x.Chinese), English = String.Join(",", x.English) }).ToList()) {
                this.dataGridView1.Rows.Add(question.GetType().GetProperties().Select(x => x.GetValue(question)).ToArray());
            }

            button1.Click += (x, e) => {
                try {
                    List<Question.EnglishDictionary> englishDictionaries = new List<Question.EnglishDictionary>();
                    foreach(DataGridViewRow question in dataGridView1.Rows) {
                        if (question.Index == dataGridView1.Rows.Count - 1) continue;
                        englishDictionaries.Add(new Question.EnglishDictionary() {
                            Chinese = question.Cells[0].Value.ToString().Split(',').ToList(),
                            English = question.Cells[1].Value.ToString().Split(',').ToList()
                        });
                    }
                    QuestionData.instance.englishQuestion.englishDictionaries = englishDictionaries;
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
