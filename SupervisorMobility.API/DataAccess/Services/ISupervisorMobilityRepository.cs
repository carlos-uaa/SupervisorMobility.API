using Microsoft.EntityFrameworkCore.ChangeTracking;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;

namespace SupervisorMobility.API.Services
{
    public interface ISupervisorMobilityRepository
    {
        #region ChecklistCategoryOperations

        Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesAsync();
        Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesForUpdateSequenceAsync(
            int currentSequence, int oldSequence, int categoryId);
        Task<ChecklistCategory?> GetChecklistCategoryAsync(
            int categoryId, bool includeChecklistQuestion = false);
        Task<bool> ChecklistCategoryExistAsync(int categoryId);
        Task<int> GetChecklistCategoriesMaxSequenceAsync();
        void AddChecklistCategory(ChecklistCategory checklistCategory);
        void DeleteChecklistCategory(ChecklistCategory checklistCategory);
        #endregion
        #region JobObservationTypesOperations
        Task<IEnumerable<JobObservationType>> GetJobObservationTypesAsync();
        Task<JobObservationType?> GetJobObservationTypeAsync(int id, bool includeConfigs = false);
        void AddJobObservationType (JobObservationType jobObservationType);
        void DeleteJobObservationType (JobObservationType jobObservationType);
        Task<bool> JobObservationTypeExistAsync(int jobObservationTypeId);
        #endregion
        #region GroupOperations
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task<Group?> GetGroupAsync(int groupId);
        Task<bool> GroupExistAsync(int groupId);

        void AddGroup(Group group);
        void DeleteGroup(Group group);
        #endregion
        #region PlantOperations
        Task<IEnumerable<Plant>> GetPlantsAsync();
        Task<Plant?> GetPlantAsync(int plantId, bool includeAreas = false);
        Task<Plant?> GetPlantByCodeAndDescriptionAsync(string code, string description);
        Task<bool> PlantExistAsync(int plantId);
        Task<bool> PlantExistByCodeAndDescriptionAsync(string code, string description);
        void AddPlant(Plant plant);
        void DeletePlant(Plant plant);
        #endregion
        #region AreaOperations
        Task<IEnumerable<Area>> GetAreasForPlantAsync(int plantId);
        Task<Area?> GetAreaForPlantAsync(int plantId,
            int areaId, bool includeOperations = false);
        Task<Area?> GetAreaForPlantByCodeAndDescriptionAsync(int plantId,
            string code, string description);
        Task<bool> AreaExistAsync(int areaId);
        Task<bool> AreaExistByCodeAndDescriptionInPlantAsync(string code, string description, int plantId);
        Task AddAreaForPlantAsync(int plantId, Area area);
        void DeleteArea(Area area);
        #endregion
        #region DistributionOperations
        Task<IEnumerable<Distribution>> GetDistributionsForAreaAsync(int areaId);
        
        Task<Distribution?> GetDistributionForAreaAsync(int areaId, int distributionId);
        Task<Distribution?> GetDistributionForAreaByCodeAndDescriptionAsync(int areaId, string code, string description);
      
        Task AddDistributionForPlantAsync(int plantId, int areaId, Distribution distribution);
        Task<bool> DistributionExistsAsync(int distributionId);
        Task<bool> DistributionExistsByCodeandDescriptionInAreaAsync(int areaId, string code, string description);
        void DeleteDistribution (Distribution distribution);
        #endregion
        #region OperationOperations
        Task<IEnumerable<Operation>> GetOperationsForDistributionAsync(int distributionId);
        
        Task<bool> OperationExistsAsync(int operationId);
        Task<bool> OperationExistsByCodeAndDescriptionInDistributionAsync(int distributionId, string code, string description);

        Task<Operation?> GetOperationForDistributionAsync(int distributionId, int operationId);
        Task<Operation?> GetOperationForDistributionByCodeAndDescriptionAsync(int distributionId, string opcode, string opdescription);

        Task AddOperationForDistributionAsync(int areaId, int distributionId, Operation operation);
        void DeleteOperation(Operation operation);
        #endregion
        #region QuestionTypeOperations
        Task<IEnumerable<QuestionType>> GetQuestionTypesAsync();
        Task<QuestionType?> GetQuestionTypeAsync(int id, bool includeChecklistQuestions = false);
        #endregion
        #region ChecklistQuestionOperations

        Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForCategoryAsync(int categoryId);
        Task<ChecklistQuestion?> GetChecklistQuestionForCategoryAsync(int categoryId,
            int checklistQuestionId);
        Task AddChecklistQuestionForCategoryAsync(int categoryId, ChecklistQuestion checklistQuestion);
        Task<int> GetChecklistQuestionMaxCategorySequenceAsync(int categoryId);
        Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForUpdateSequenceAsync(
                int currentSequence, int oldSequence, int categoryId, int checklistQuestionId);
        void DeleteChecklistQuestions(ChecklistQuestion checklistQuestion);
        #endregion
        #region JobObservationConfigOperations
        Task<IEnumerable<JobObservationConfig>> GetJobOperationConfigsForJobOperationTypeAsync(int jobObservationTypeId);
        Task<JobObservationConfig?> GetJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId,
            int jobObservationConfigId);
        Task AddJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId, JobObservationConfig jobObservationConfig);
        void DeleteJobOperationConfig(JobObservationConfig jobObservationConfig);
        #endregion
        #region SupportDocumentTypeOperations
        Task<IEnumerable<SupportDocumentType>> GetSupportDocumentTypesAsync();
        Task<SupportDocumentType?> GetSupportDocumentTypeAsync(int supportDocumentTypeId);
        Task<bool> SupportDocumentTypeExistAsync(int supportDocumentTypeId);
        void AddSupportDocumentType(SupportDocumentType supportDocumentType);
        void DeleteSupportDocumentType(SupportDocumentType supportDocumentType);
        #endregion
        #region ProductOperations
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductAsync(int productId);
        Task<Product?> GetProductByCodeAndDescriptionAsync(string code, string description);
        Task<bool> ProductExistAsync(int productId);
        Task<bool> ProductExistByCodeAndDescriptionAsync(string code, string description);
        void AddProduct(Product product);
        void DeleteProduct(Product product);
        #endregion
        #region AssyChart
        Task<IEnumerable<AssyChart>> GetAllAssyChartsAsync();
        Task<AssyChart?> GetAssyChartAsync(int asssychartId);
        Task<AssyChart?> GetAssyChartAdvanceAsync(string GOS, string CCP, string HOE, int PlantId, int AreaId, int DistributionId, int OperationId, int Productid);
        Task<IEnumerable<AssyChart>> GetAssyChartByPlantAsync(int plantId);
        Task<bool> AssyChartExistAsync(int assychartID);
        Task<bool> AssyChartExistAdvanceAsync(string GOS, string CCP, string HOE, int PlantId, int AreaId, int DistributionId, int OperationId, int Productid);
        void AddAssyChartAsync(AssyChart assychart);
        void DeleteAssyChartAsync(AssyChart assyChart);

        #endregion

        #region Users
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetAllUsersByPlantAreaAndGroupAsync(int plantaId, int areaId, int grupId);
        Task<User?> GetUserAsync(int userId);
        Task<User?> GetUserByNominaAsync(int nomina);
        Task<bool> UserExistAsync(int userId);
        Task<bool> UserExistAdvanceAsync(string nombre, int nomina, Plant plantid, Area areaid, Group grupoid);
        void AddUserAsync(User user);
        void DeleteUserAsync(User user);

        #endregion

        #region CommonOperations

        Task<bool> SaveChangesAsync();

        #endregion

        #region ProductDistributions
        Task<IEnumerable<ProductDistribution>> GetDistributionsForProductAsync(int productId);
        Task<ProductDistribution?> GetDistributionForProductAsync(int productId, int distributionId);
        Task AddDistributionForProductAsync(int plantId, ProductDistribution distribution);

        void DeleteProductDistribution(ProductDistribution productDistribution);
        #endregion
        #region ProductOperationsOperations
        Task<IEnumerable<ProductOperation>> GetProductOperationsForDistributionAsync(int productDistributionId);
        Task<ProductOperation?> GetProductOperationForDistributionAsync(int productDistributionId, int operationId);
        Task AddProductOperationForDistributionAsync(int productId, int productDistributionId, ProductOperation productOperation);
        void DeleteProductOperation(ProductOperation productOperation);
        Task<bool> ProductDistributionExistsAsync(int distributionId);

        #endregion

        #region JobObservationOperations

        Task<IEnumerable<JobObservation>> GetAllJobObservationsAsync();

        Task<JobObservation?> GetJobObservationAsync(int jobObservationId);

        void AddJobObservation(JobObservation jobObservation);

        void DeleteJobObservation(JobObservation jobObservation);


        #endregion

        #region GlosaryOperations

        Task<IEnumerable<Glosary>> GetGlosaryAsync();
        Task<Glosary?> GetGlosaryWordAsync(int glosaryWordId);
        void AddGlosaryWord(Glosary glosaryWord);
        void DeleteGlosaryWord(Glosary glossaryWord);
        #endregion

    }
}
