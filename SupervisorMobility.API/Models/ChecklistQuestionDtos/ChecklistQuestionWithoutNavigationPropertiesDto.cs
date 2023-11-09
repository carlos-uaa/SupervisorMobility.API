namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionWithoutNavigationPropertiesDto
    {
        public int QuestionID { get; set; }
        public string Prompt { get; set; }
        public int PillarId { get; set; }
        public string NotGood { get; set; }
        public int CategorySequence { get; set; }
        public bool? IsActive { get; set; }
        //FK
        public int ChecklistCategoryId { get; set; }
    }
}
