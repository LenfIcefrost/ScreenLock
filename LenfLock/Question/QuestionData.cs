using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using LenfLock.Question;
using LenfLock.Communication;

namespace LenfLock {
    public enum QuestionType {
        Math,
        English,
        Chinese
    }
    public class QuestionData {
        #region Interface
        static QuestionData _i = null;
        public static QuestionData instance { get { return _i ??= new QuestionData(); } }

        public QPersonal Pinstance = new QPersonal();
        public QSystem System = new QSystem();
        public QAudio Audio = new QAudio();
        public ChineseQuestion chineseQuestion = new ChineseQuestion();
        public MathQuestion mathQuestion = new MathQuestion();
        public EnglishQuestion englishQuestion = new EnglishQuestion();
        #endregion
        #region SaveLoad
        static string Directoryname = $@".\LenfLock";
        static string Filename = "saveData.txt";

        public static void Save() {
            var question = JsonConvert.SerializeObject(instance);
            Directory.CreateDirectory(Directoryname);
            var filestream = File.Create($@"{Directoryname}\{Filename}");
            filestream.Close();
            File.WriteAllText($@"{Directoryname}\{Filename}", question);
        }
        public static void Load() {
            if(File.Exists($@"{Directoryname}\{Filename}")) {
                _i = JsonConvert.DeserializeObject<QuestionData>(File.ReadAllText($@"{Directoryname}\{Filename}"), new JsonSerializerSettings() {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });
            }
        }
        #endregion
        #region Person
        public class QPersonal {
            public string password = "123";
        }
        #endregion
        #region System
        public class QSystem {
            public int TimeForRecall { get; set; }
            public QuestionType QuestionType { get; set; }
            public QSystem() {
                this.TimeForRecall = 30;
            }
        }
        #endregion
        #region Audio
        public class QAudio {
            public bool Active { get; set; }
            public int Filter { get; set; }
            public QAudio() {
                this.Active = false;
                this.Filter = 0;
            }
        }
        #endregion

        public QuestionPack Generate() {
            switch (this.System.QuestionType) {
                case QuestionType.Math:
                    return this.mathQuestion.GenerateQuestionAns();
                case QuestionType.English:
                    return this.englishQuestion.GenerateQuestionAns();
                case QuestionType.Chinese:
                    return this.chineseQuestion.GenerateQuestionAns();
            }
            return null;
        }
    }
}
