using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.AttendanceDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.Users;
using System;

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
        void AddJobObservationType(JobObservationType jobObservationType);
        void DeleteJobObservationType(JobObservationType jobObservationType);
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
        Task<IEnumerable<Area>> GetAreasForPlantAsync(int plantId, bool includeCollections = false);
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
        Task<IEnumerable<Distribution>> GetDistributionsForAreaAsync(int areaId, bool includecollections = false);

        Task<Distribution?> GetDistributionForAreaAsync(int areaId, int distributionId, bool includeCollections = false);
        Task<Distribution?> GetDistributionOnlyIdAsync(int distributionId, bool includeCollections = false);
        Task<Distribution?> GetDistributionForAreaByCodeAndDescriptionAsync(int areaId, string code, string description);

        Task AddDistributionForPlantAsync(int plantId, int areaId, Distribution distribution);
        //Task AddProductForDistributionAsync(int areaId, int distributionId, Product product);
        Task<bool> DistributionExistsAsync(int distributionId);
        Task<bool> DistributionExistsByCodeandDescriptionInAreaAsync(int areaId, string code, string description);
        void DeleteDistribution(Distribution distribution);
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
        Task<Product?> GetProductAsync(int productId, bool collection = false);
        Task<Product?> GetProductByCodeAndDescriptionAsync(string code, string description);
        Task<bool> ProductExistAsync(int productId);
        Task<bool> ProductExistByCodeAndDescriptionAsync(string code, string description);
        void AddProduct(Product product);
        Task RemoveDistributionForProductAsync(int productId, int distributionID);
        Task RemoveProductForDistributionAsync(int productId, int distributionID);
        Task AddDistributionForProductAsync(int productId, Distribution distribution);
        void DeleteProduct(Product product);
        #endregion
        #region AssyChart
        Task<IEnumerable<AssyChart>> GetAllAssyChartsAsync();
        Task<AssyChart?> GetAssyChartAsync(int asssychartId);
        Task<AssyChart?> GetAssyChartForJobObservationAsync(int PlantId, int AreaId, int DistributionId, int OperationId);
        Task<AssyChart?> GetAssyChartAdvanceAsync(string GOS, string CCP, string HOE, int PlantId, int AreaId, int DistributionId, int OperationId, int Productid);
        Task<IEnumerable<AssyChart>> GetAssyChartByPlantAsync(int plantId);
        Task<IEnumerable<AssyChart>> GetAssyChartByAreaAsync(int plantId, int areaId);
        Task<IEnumerable<AssyChart>> GetAssyChartByDistributionAsync(int plantId, int areaId, int distributionId);
        Task<bool> AssyChartExistAsync(int assychartID);
        Task<bool> AssyChartExistAdvanceAsync(string GOS, string CCP, string HOE, int PlantId, int AreaId, int DistributionId, int OperationId, int Productid);
        void AddAssyChartAsync(AssyChart assychart);
        void DeleteAssyChartAsync(AssyChart assyChart);

        #endregion
        #region Users
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetAllUsersWhitPlantAreaAndGroupAsync();
        Task<User?> GetUserAsync(int userId, bool collection = false);
        Task<User?> GetUserByObjectIdAsync(string objectId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByPayrollAndMoreAsync(int payroll, int plantid, int areaid, int groupid);
        Task<bool> UserExistAsync(int userId);
        Task<bool> UserExistAdvanceAsync(string nombre, int nomina, int plantid, int areaid, int grupoid);
        void UserAddSubordinated(User Master, User Slave);
        void UserAddArea(User Master, Area Slave);

        Task UpdateUser(UsersForUpdateDto user, int userId);

        Task AddUserAsync(User user);
        void DeleteUserAsync(User user);
        #endregion
        #region HistoyJobObservation
        Task<JobObservationVersion?> GetHistoryJobObservationAsync(int HistoryJobObservationId);
        Task<IEnumerable<JobObservationVersion>> GetAllHistoryJobObservationAsync(int jobObservationId);
        void AddHistoyJobObservationAsync(JobObservationVersion jobObservationHistory);
        void DeleteHistoyJobObservationAsync(JobObservationVersion HistoryVersion);
        Task<bool> DeleteHistoyFromJobObservationAsync(JobObservationVersion HistoryVersion, JobObservation jobObservation);
        Task<bool> AddHistoyToJobObservationAsync(JobObservationVersion HistoryVersion, JobObservation jobObservation);

        #endregion
        #region Notification
        Task<Notification?> GetNotificationAsync(int notificationID);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task<IEnumerable<Notification>> GetAllNotificationsFromUserAsync(int id);
        void AddNotificationAsync(Notification notify);
        void DeleteNotificationAsync(Notification notify);
        #endregion
        #region File
        void AddUploadFile(FileUpload fileUpload);

        Task<FileUpload?> GetFileUploadAsync(int fileid);
        void DeleteUploadFile(FileUpload fileUpload);


        #endregion
        #region Guide

        Task<Guides?> GetGuideAsync(int guideId, bool includeFile = false);

        Task<IEnumerable<Guides>> GetAllGuides(bool includeFile = false);
        void AddGuide(Guides guide);
        void DeleteGuide(Guides guide);
        #endregion
        #region JobObservationOperations

        Task<IEnumerable<JobObservation>> GetAllJobObservationsAsync(bool includeLup);
        Task<JobObservation?> GetJobObservationAsync(int jobObservationId, bool includeLup);
        void AddJobObservation(JobObservation jobObservation);
        void DeleteJobObservation(JobObservation jobObservation);
        Task<bool> JobObservationExistAsync(int jobObservationId);

        #endregion
        #region GlosaryOperations

        Task<IEnumerable<Glosary>> GetGlosaryAsync();
        Task<Glosary?> GetGlosaryWordAsync(int glosaryWordId);
        void AddGlosaryWord(Glosary glosaryWord);
        void DeleteGlosaryWord(Glosary glossaryWord);
        #endregion
        #region LupOperations
        Task<IEnumerable<Lup>> GetAllLupAsync();
        Task<Lup?> GetLupAsync(int guideId, bool includeFile = false);
        void AddLup(Lup lup);
        void DeleteLup(Lup lup);
        Task<bool> LupExistAsync(int lupId);
        Task AddEvidenceForLupAsync(int lupId, FileUpload evidence);
        Task RemoveEvidenceForLupAsync(int lupId, int fileUploadId);


        #endregion
        #region Attendance
        Task<Attendance> GetAttendanceById(int AttendanceId);
      
        void AddAttendance(Attendance Attendance);
        Task<IEnumerable<Attendance>> GetAllAttendance();
        Task<IEnumerable<Attendance>> GetAllAttendanceOfSupervisor(int idsupervisor);
        #endregion
        #region CommonOperations

        Task<bool> SaveChangesAsync();
        #endregion

    }
}
