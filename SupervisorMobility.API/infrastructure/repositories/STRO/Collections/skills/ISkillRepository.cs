using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;

namespace SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Skills
{
    public interface ISkillRepository
    {
        Task<Skill> GetSkill(int Id);
        Task<List<Skill>> GetAllSkill();
        Task<Skill> CreateSkill(Skill createSkillDto);
    }
}