using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.HeadCount;
using SupervisorMobility.API.Models.PATDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using SupervisorMobility.API.Models.Users;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.Services
{
    public interface ISupervisorMobilityRepository
    {
        #region ChecklistCategoryOperations

        Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesAsync(bool includeChecklistQuestion = false);
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
        Task<Plant?> GetPlantOnlyIdAsync(int PlantId);

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
        
        Task<Area?> GetAreaOnlyIdAsync(int areaId);
        Task<Area?> GetAreaForPlantByCodeAndDescriptionAsync(int plantId,
            string code, string description);
        Task<bool> AreaExistAsync(int areaId);
        Task<bool> AreaExistByCodeAndDescriptionInPlantAsync(string code, string description, int plantId);
        Task AddAreaForPlantAsync(int plantId, Area area);
        Task<AsyncVoidMethodBuilder> AddArea(Area area);
        void DeleteArea(Area area);
        #endregion
        #region DistributionOperations
        Task<IEnumerable<Distribution>> GetDistributionsForAreaAsync(int areaId, bool includecollections = false);
        Task<IEnumerable<Distribution>> GetAllDistributions();

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

        Task<AsyncVoidMethodBuilder> RemoveAllOperations();

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
        Task<Product?> GetProductByCodeAsync(string code);
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
        Task<AssyChart?> GetAssyChartAdvanceAsync( int PlantId, int AreaId, int DistributionId, int OperationId);
        Task<AssyChart?> GetAssyChartForJobObservationAsync(int PlantId, int AreaId, int DistributionId);
        Task<AssyChart?> GetAssyChartAdvanceByProductAsync(int plantId, int areaId, int distributionId, int ProductId);
        Task<AssyChart?> GetAssyChartAdvanceByOperationAndProductAsync(int plantId, int areaId, int distributionId, int operationId, int ProductId);
        Task<IEnumerable<AssyChart>> GetAllAssyChartsByPlantAsync(int plantId);
        Task<IEnumerable<AssyChart>> GetAllAssyChartsByAreaAsync(int plantId, int areaId);
        Task<IEnumerable<AssyChart>> GetAllAssyChartsByDistributionAsync(int plantId, int areaId, int distributionId);
        Task<bool> AssyChartExistAsync(int assychartID);
        Task<bool> AssyChartExistAdvanceAsync(int PlantId, int AreaId, int DistributionId, int OperationId);
        void AddAssyChartAsync(AssyChart assychart);
        void DeleteAssyChartAsync(AssyChart assyChart);

        #endregion
        #region RouteProductAssychart

        Task<SOSCodePath?> GetCodePathItemAsync(int RouteId);
        Task<SOSCodePath?> TryFindCodePathItemAsync(int assychartId, string code);

        Task<IEnumerable<SOSCodePath>> GetAllCodePathsAsync();
        Task AssyChartRemoveAllCodePaths(AssyChart AssyChart);

        Task AssychartCreateCodePath(SOSCodePath RouteAssychart);
        void AssychartAddCodePath(AssyChart Master, SOSCodePath Slave);
        #endregion
        #region Users
        Task<IEnumerable<User>> GetAllUsersAsync(bool includeCollections = false, bool includeSubordinates = false);
        Task<IEnumerable<User>> GetAllSubordinatesAsync(int superiorId);
        Task<IEnumerable<User>> GetAllUserByTypeAsync(int typeUser, bool includeCollections = false, bool includeSubordinates = false);
        Task<IEnumerable<User>> GetAllUserByTypeInPlantAreaAsync(int plantId, int areaId, int typeUser, bool includeCollections = false, bool includeSubordinates = false);
        Task<IEnumerable<User>> GetAllUserByTypeInPlantAsync(int plantId, int typeUser, bool includeCollections = false, bool includeSubordinates = false);
        Task<IEnumerable<User>> GetAllUsersWhitPlantAreaAndGroupAsync();
        Task<User?> GetUserAsync(int userId, bool collection = false);
        Task<User?> GetUserByObjectIdAsync(string objectId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByPayrollAsync(int payroll);
        Task<User?> GetUserByPayrollAndMoreAsync(int payroll, int plantid, int areaid, int groupid);
        Task<bool> UserExistAsync(int userId);
        Task<bool> UserExistByPayrollAsync(int payroll);
        Task<bool> UserExistByEmailAsync(string email);
        Task<bool> UserExistByObjectIdAsync(string ObjectId);
        Task<bool> UserExistAdvanceAsync(string nombre, int nomina, int plantid, int areaid, int grupoid);
        Task<AsyncVoidMethodBuilder> UserAddSubordinated(User Master, User Slave);
        Task<AsyncVoidMethodBuilder> UserRemoveSubordinated(User Master, User Slave);
        Task<AsyncVoidMethodBuilder> UserUpdateAllSubordinated(User Master);
        Task<AsyncVoidMethodBuilder> UserRemoveAllSubordinated(User Master);
        Task RemoveAllAreasFromUser(User user);
        Task<AsyncVoidMethodBuilder> UserRemoveAllAreas(User Master);
        Task<AsyncVoidMethodBuilder> UserAddArea(User Master, Area Slave);
       

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
        Task<IEnumerable<JobObservation>> GetJobObservationsByFiltersAsync(DateTime startDate, DateTime endDate, int plantId, int areaId, int distributionId, int operationId, int supervisorId, int status);
        Task<JobObservation?> GetJobObservationAsync(int jobObservationId, bool includeLup);
        Task<int> AddJobObservation(JobObservation jobObservation);
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
        Task<Lup?> GetLupAsync(int lupId, bool includeFile = false);
        Task<IEnumerable<Lup>> GetLupsByFiltersAsync(DateTime startDate, DateTime endDate, int plantId, int areaId, int distributionId, int operationId, int supervisorId, int status);
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
        #region ILU
        Task<ILULevel?> GetILULevel(int idILU);
        Task<IEnumerable<ILULevel>> GetAllILULevel();
        Task<int> AddILU(ILULevel lU);
        Task<int> UpdateILU(ILULevel iluforUpdate, ILULevel iluEntity);
        Task RemoveILU(ILULevel lU);

        #endregion
        #region ILURegister
        Task<ILURegister> GetILURegister(int idILUR);
        Task<int> AddILURegister(ILURegister iLURegister);
        Task<int> AddILURegToUser(ILURegister iLURegister, User Master);
        Task<int> UpdateILURegister(ILURegister iluRforUpdate, ILURegister iluREntity);
        Task<int> RemoveILURegister(ILURegister id);

        #endregion
        #region PAT
        Task<PAT?> GetPat(int patId);
        Task<int> AddPat(PAT patForAdd);
        Task<PAT?> GetPatForYearOfSV(int sv, int Year);
        Task<int> UpdatePAT(PATForUpdateDto patForUpdate, PAT PatEntity);
        Task<IEnumerable<PAT>> GetAllPATs();
        Task<IEnumerable<PAT>> GetAllPATsOfSv(int svId);
        Task<IEnumerable<PAT>> GetAllPATsofSSV(int ssvID);

        #endregion
        #region Logger

        #endregion
        #region UserNotFound

        Task<IEnumerable<UserNotFound>> GetAllUsersNotFoundAsync();
        Task<UserNotFound?> GetUserNotFoundAsync(int userNotFoundId);

        Task UpdateUserNotFound(UserNotFoundForUpdateDto userNotFound, int userNotFoundId);

        Task AddUserNotFoundAsync(UserNotFound userNotFound);
        #endregion
        #region SOS_Reviews
        //startRegion
        Task<int> AddSOSReview(SOSReviewProgram SOSEntity);
        Task<IEnumerable<SOSReviewProgram>> GetAllSOSReviews();
        Task<SOSReviewProgram?> GetSOSasync(int sosId);
        Task<int> UpdateSOSReview(SOSReviewForUpdateDto SOSForUpdate, SOSReviewProgram SOSEntity);
        Task<int> DeleteSOSReview(SOSReviewProgram SOSEntity);
        
        // add Supervisor Responsable
        void SOSReviewAddUser(SOSReviewProgram Master, User Slave);
        void SOSReviewRemoveUser(SOSReviewProgram Master, User Slave);
        //EndRegion
        #endregion 
        #region SOS_RegOperationJobObservartion
        //startRegion
        Task<int> AddSOSReviewRegister(SOSRegisterJobObservation RegEntity);
        Task<IEnumerable<SOSRegisterJobObservation>> GetAllSOSReviewsRegisters(int SOSReviewProgramId);
        Task<IEnumerable<SOSRegisterJobObservation>> GetAllSOSReviewsRegistersByDistribution(int SOSReviewProgramId, int distributionid);
        Task<SOSRegisterJobObservation?> GetSOSReviewRegister(int SosId);
        Task<int> UpdateRegisterJobObservation(SOSReviewsRegisterForUpdateDto SOSForUpdate, SOSRegisterJobObservation SOSEntity);

        //EndRegion
        #endregion
        #region SOS_RegUserOperation
        //startRegion
        Task<int> AddSOSRegUserOperation(SOSRegUserOperation RegEntity);
        Task<SOSRegUserOperation?> GetSOSRegUserOperation(int SosId);
        Task<IEnumerable<SOSRegUserOperation>> GetAllSOSRegUserOperations(int SosId);
        Task<int> UpdateRegUserOperation(SOSRegUserOperationForUpdateDto SOSForUpdate, SOSRegUserOperation SOSEntity);

        //EndRegion

        #endregion
        #region headcount
        Task<IEnumerable<HeadCount>> GetAllHeadCountsDataAsync();
        Task<HeadCount?> GetHeadCountByIdAsync(int HeadId);
        Task<AsyncVoidMethodBuilder> RemoveAllHeadCountRegisters();
        Task AddHeadCoutAsync(HeadCount user);


        //Task<int> AddHeadCountProcess(HeadCountProcess headCountProcess);
        //Task<HeadCountProcess?> GetHeadCountProcessById(int id);
        //Task<IEnumerable<HeadCountProcess>> GetAllHeadCountProcess();
        //Task<int> UpdateHeadCountProcess(HeadCountProcessCreateUpdateDto headCountProcess, HeadCountProcess entity);
        //Task<int> DeleteHeadCountProcess(HeadCountProcess headCountProcess);
        #endregion
        #region DepartmentOperations
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<Department?> GetDepartmentAsync(int departmentId);
        Task<bool> DepartmentExistAsync(int departmentId);

        void AddDepartment(Department department);
        void DeleteDepartment(Department department);
        #endregion
        #region PillarOperations
        Task<IEnumerable<Pillar>> GetPillarsAsync();
        Task<Pillar?> GetPillarAsync(int pillarId);
        Task<bool> PillarExistAsync(int pillarId);

        void AddPillar(Pillar pillar);
        void DeletePillar(Pillar pillar);
        #endregion
        #region ChecklistAnswersOperations
        Task<IEnumerable<ChecklistAnswer>> GetAllChecklistAnswerAsync();
        Task<IEnumerable<ChecklistAnswer>> GetAllChecklistAnswersByJobObservationIdAsync(int jobObservationId);
        Task<ChecklistAnswer?> GetChecklistAnswerAsync(int guideId);
        void AddChecklistAnswer(ChecklistAnswer checklistAnswer);
        void DeleteChecklistAnswer(ChecklistAnswer checklistAnswer);
        Task<bool> ChecklistAnswerExistAsync(int checklistAnswerId);

        #endregion
        #region CommonOperations

        Task<bool> SaveChangesAsync();
        #endregion

    }
}
