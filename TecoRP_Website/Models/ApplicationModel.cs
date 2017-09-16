using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TecoRP_Website.Models
{
    public class ApplicationModel
    {
       public List<AnswerField> Answers { get; set; }
        public ApplicationModel(){ Answers = new List<AnswerField>(); }
    }
    public class ApplicationViewModel : ApplicationModel
    {
        public int ApplicationID { get; set; }
        public string Email { get; set; }
        public DateTime RegisterDate { get; set; }
        public string SocialClubName { get; set; }
        public bool? IsApproved { get; set; }
    }
    public class AnswerField
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public bool IsTextArea { get; set; }
        public string Selection_A { get; set; }
        public string Selection_B { get; set; }
        public string Selection_C { get; set; }
        public string AnswerText { get; set; }
        public int AnswerSelection { get; set; }
        public AnswerField()
        {

        }
        public AnswerField(int questionID,string questionText, bool isTextArea = true, string selection_A = "", string selection_B = "", string selection_C = "")
        {
            QuestionId = questionID; QuestionText = questionText; IsTextArea = isTextArea; Selection_A = selection_A; Selection_B = selection_B; Selection_C = selection_C;
        }
    }
}