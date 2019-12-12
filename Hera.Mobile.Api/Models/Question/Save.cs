using System.Collections.Generic;

namespace Hera.Mobile.Api.Models.Question
{
    public class SaveAnswerRequest : BaseRequest
    {
        public int MemberId { get; set; }
        public int CategoryId { get; set; }
        public int ChildId { get; set; }
        public List<SaveQuestionItem> QuestionList { get; set; }
    }

    public class SaveQuestionItem
    {
        public int QuestionId { get; set; }
        public List<SaveAnswerItem> AnswerList { get; set; }
    }

    public class SaveAnswerItem
    {
        public int AnswerId { get; set; }
        public string Answer { get; set; }
    }
}