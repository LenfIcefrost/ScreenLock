using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LenfLock.Question {
    public class MathQuestion : IBasicQuestion {
        public int minNum { get; set; }
        public int maxNum { get; set; }
        public int plusCount { get; set; }
        public int minusCount { get; set; }
        public int mutlyCount { get; set; }
        public MathQuestion() {
            minNum = 4;
            maxNum = 9;
            plusCount = 2;
            minusCount = 0;
            mutlyCount = 0;
        }
        public QuestionPack GenerateQuestionAns() {
            Random r = new Random();
            short count = (short)(plusCount + minusCount + mutlyCount + 1);
            var num = Enumerable.Range(0, count).Select(x => r.Next(maxNum - minNum + 1) + minNum).ToList();
            var op = Enumerable.Repeat('+', plusCount).Concat(Enumerable.Repeat('-', minusCount)).Concat(Enumerable.Repeat('*', mutlyCount)).OrderBy(x => Guid.NewGuid()).ToList();
            string q = "";
            List<int> ansNum = new List<int>() { num[0] };
            for (int i = 0; i < count - 1; i++) {
                var la = ansNum.Last();
                switch (op[i]) {
                    case '*':
                        ansNum.Add(la * num[i + 1]);
                        ansNum.Remove(la);
                        break;
                    case '-':
                        ansNum.Add(-num[i + 1]);
                        break;
                    default:
                        ansNum.Add(num[i + 1]);
                        break;
                }
                q += $"{num[i]} {op[i]} ";
            }
            q += num[count - 1];
            string ans = ansNum.Sum().ToString();
            return new QuestionPack(q, ans);
        }
    }
}
