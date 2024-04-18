using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LenfLock.Question {
    public enum QuestionPackType {
        BlankFill,
        MultiChoice,
        MultiChoiseAns,
        Error
    }
    public class QuestionPack {
        public QuestionPackType QuestionPackType { get; set; }
        public string Question { get; set; }
        public List<string> AnswerList { get; set; }
        public List<string> WrongAnsList { get; set; }
        public QuestionPack()
            : this("", new List<string>(), new List<string>(), QuestionPackType.Error) { }
        public QuestionPack(string question, string answer)
            : this(question, new List<string>() { answer }) { }
        public QuestionPack(string question, List<string> answerList)
            : this(question, answerList, new List<string>(), QuestionPackType.BlankFill) { }
        public QuestionPack(string question, string answer, List<string> wrongAnsList)
            : this(question, new List<string>() { answer }, wrongAnsList, QuestionPackType.MultiChoice) { }
        public QuestionPack(string question, List<string> answerList, List<string> wrongAnsList)
            : this(question, answerList, wrongAnsList, QuestionPackType.MultiChoiseAns) { }
        public QuestionPack(string question, List<string> answerList, List<string> wrongAnsList, QuestionPackType questionPackType) {
            this.Question = question;
            this.AnswerList = answerList;
            this.WrongAnsList = wrongAnsList;
            this.QuestionPackType = questionPackType;
        }
        public bool Check(string ans) {
            return this.Check(new List<string>() { ans });
        }
        public bool Check(List<string> ans) {
            return ans.TrueForAll(ans => this.AnswerList.Contains(ans));
        }
    }
    public interface IBasicQuestion {
        public abstract QuestionPack GenerateQuestionAns();
    }
}
