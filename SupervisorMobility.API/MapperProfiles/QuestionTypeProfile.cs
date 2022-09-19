using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class QuestionTypeProfile : Profile
    {
        public QuestionTypeProfile()
        {
            CreateMap<Entities.QuestionType, Models.QuestionTypeDtos.QuestionTypeWithoutChecklistDto>();
            CreateMap<Entities.QuestionType, Models.QuestionTypeDtos.QuestionTypeWithJustChecklistQuestionsDto>();
        }
    }
}
