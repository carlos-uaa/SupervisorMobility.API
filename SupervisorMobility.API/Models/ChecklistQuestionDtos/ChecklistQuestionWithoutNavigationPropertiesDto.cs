namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionWithoutNavigationPropertiesDto
    {
        public int QuestionID { get; set; }
        public string Prompt { get; set; }
        public List<int>? Pillars { get; set; }
        public string NotGood { get; set; }
        public int CategorySequence { get; set; }
        public bool? IsActive { get; set; }
        //FK
        public string PromptEN { get; set; }
        public string NotGoodEN { get; set; }

        public int TypeId { get; set; }

        public List<string>? Options { get; set; }
        public List<string>? Actions { get; set; }

        public int JobCategoryStructureId { get; set; }
    }
}
