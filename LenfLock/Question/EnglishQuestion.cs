using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LenfLock.Question {
    public class EnglishDictionary {
        public List<string> English { get; set; }
        public List<string> Chinese { get; set; }
        public EnglishDictionary() {
            this.English = new List<string>();
            this.Chinese = new List<string>();
        }
        public EnglishDictionary(List<string> English, List<string> Chinese) {
            this.English = English;
            this.Chinese = Chinese;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is EnglishDictionary)) return false;
            EnglishDictionary other = (EnglishDictionary)obj;
            if(this.English != other.English) return false;
            if(this.Chinese != other.Chinese) return false;
            return true;
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
    public class EnglishQuestion : IBasicQuestion {
        public List<EnglishDictionary> englishDictionaries { get; set; }
        public EnglishQuestion() {
            this.englishDictionaries = new List<EnglishDictionary>() {
                new EnglishDictionary(new List<string>{"sleeping"}, new List<string>{"睡覺"})
            };
        }
        public QuestionPack GenerateQuestionAns() {
            Random rnd = new Random();
            var question = this.englishDictionaries[rnd.Next(this.englishDictionaries.Count)];
            return new QuestionPack(string.Join(", ", question.Chinese), question.English);
        }
    }
}
