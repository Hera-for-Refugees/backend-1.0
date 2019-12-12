using System.Collections.Generic;

namespace Hera.Mobile.Api.Models.Question
{
    public class QuestionRequest : BaseRequest
    {
        public int MemberId { get; set; }
        public int CategoryId { get; set; }
        public int ChildId { get; set; }
    }

    public class QuestionResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public List<QuestionItem> QuestionList { get; set; }
    }

    public class QuestionItem
    {
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int AnswerTypeId { get; set; }
        public int AnswerDataTypeId { get; set; }
        public bool IsConnected { get; set; }
        public int ConnectedQuestionId { get; set; }
        public int ConnectedAnswerId { get; set; }
        public List<AnswerItem> AnswerList { get; set; }
        public string Answer { get; set; }
    }
    public class AnswerItem
    {
        public int AnswerId { get; set; }
        public int DataTypeId { get; set; }
        public string Answer { get; set; }
        public bool IsDefault { get; set; }
    }
}