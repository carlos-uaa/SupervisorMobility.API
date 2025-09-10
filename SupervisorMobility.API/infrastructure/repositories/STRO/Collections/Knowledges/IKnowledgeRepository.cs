using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Collections.Knowledge.Dtos;

namespace SupervisorMobility.API.infrastructure.repositories.STRO.Collections.Knowledges
{
    public interface IKnowledgeRepository
    {
        Task<Knowledge> GetKnowledge(int Id);
        Task<List<Knowledge>> GetAllKnowledge();
        Task<Knowledge> CreateKnowledge(Knowledge createKnowledgeDto);
    }
}