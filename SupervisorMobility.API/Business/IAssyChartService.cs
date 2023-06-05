using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.GuidesDtos;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.AttendanceDtos;

namespace SupervisorMobility.API.Business
{
    public interface IAssyChartService
    {
        #region SupportDocumentTypes
        Task<IEnumerable<SupportDocumentType>> FetchSupportDocumentTypesAsync();
        Task<SupportDocumentType?> FetchSupportDocumentTypeAsync(int supportDocumentTypeId);
        Task<SupportDocumentType> CreateSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task RemoveSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task UpdateSupportDocumentTypeAsync(SupportDocumentTypeForUpdateDto supportDocumentTypeForUpdate, SupportDocumentType supportDocumentType);
        #endregion
        #region Operations
        Task<IEnumerable<Operation>> FetchOperationsAsync(int distributionId);
        Task<Operation?> FetchOperationAsync(int distributionId, int operationId);
        Task<Operation> CreateOperationAsync(int areaId, int distributionId, Operation operation);
        Task UpdateOperationAsync(OperationForUpdateDto operationForUpdate, Operation operation);
        Task RemoveOperationAsync(Operation operation);
        #endregion


        #region Product
        Task<bool> CheckProductExistance(int productId);
        Task<IEnumerable<Product>> FetchProductsAsync();
        Task<Product?> FetchProductAsync(int productId, bool collections = false);
        Task<Product> CreateProductAsync(ProductForCreationDto product);
        Task UpdateProductAsync(ProductForUpdateDto productForUpdate, Product product);
        Task RemoveProductAsync(Product product);
        #endregion
        #region Plant
        Task<IEnumerable<Plant>> FetchPlantsAsync();
        Task<Plant?> FetchPlantAsync(int plantId, bool includeAreas = false);
        Task<Plant> CreatePlantAsync(PlantForCreationDto plant);
        Task UpdatePlantAsync(PlantForUpdateDto plantForUpdate, Plant plant);
        Task RemovePlantAsync(Plant plant);
        Task<bool> CheckPlantExistance(int plantId);
        #endregion
        #region Area
        Task<bool> CheckAreaExistance(int areaId);
        #endregion
        #region Distribution
        Task<bool> CheckDistributionExistance(int distributionId);
        #endregion
        

        #region AssyChart
        Task<AssyChart> CreateAssyChartAsync(AssyChartForCreation assyChart);
        Task UpdateAssyChartAsync(AssyChartForUpdateDto assyChartForUpdate, AssyChart assyChart);
        Task RemoveAssyChartAsync(AssyChart assyChart);
        #endregion
        #region Users
        Task<User?> FetchUserAsync(int userId);
        Task<User?> FetchUserWhitObjectIdAsync(string objectId);
        Task<User?> FetchUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsers();
        Task<IEnumerable<User>> GetAllUsersWhitPlantAreaAndGroupAsync();
        Task<User> CreateUserAsync(UsersForCreation user);
        Task UpdateUserAsync(UsersForUpdateDto userForUpdate, int UserId);
        Task RemoveUserAsync(User user);
        #endregion
        #region Files
        Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile);
        Task<FileUpload?> FetchFileAsync(int fileId);

        Task RemoveFileAsync(FileUpload fileUpload);
        #endregion
        #region guides
        Task<Guides?> FetchGuideAsync(int guideId, bool includeFile = false);

        Task<Guides> CreateGuideAsync(GuideForCreationDto guide);
        Task UpdateGuideAsync(GuideForUpdateDto guideForUpdate, Guides guide);
        Task RemoveGuideAsync(Guides guide);
        #endregion
        #region HistoryJobObservation
        Task<JobObservationVersion?> FetchHistoryJobObservationAsync(int JobObservationHistoryId);
        Task<JobObservationVersion> CreateHistoryJobObservationAsync(JobObservation jobObservation);
        Task UpdateHistoryJobObservationAsync(JobObservationVersion userForUpdate, JobObservationVersion user);
        Task<bool> RemoveHistoryJobObservationAsync(JobObservationVersion HistoryVersion, JobObservation jobObservation);
        #endregion

        #region Notification
        Task<Notification?> FetchNotificationAsync(int NotificationId);
        Task<Notification> CreateNotificationAsync(NotificationToCreateDto notify);
        Task<IEnumerable<Notification>> GetNotifications();
        Task<IEnumerable<Notification>> GetNotificationsFromUser(int iduser);
        Task<bool> UpdateNotificationAsync(NotificationForUpdateDto ForUpdate, Notification notify);
        Task<bool> RemoveNotificationAsync(Notification NotificationId);
        #endregion
        #region attendance
        Task<Attendance> FetchAttendanceByIdAsync(int AttendanceId);
  
        Task<Attendance> CreateAttendanceAsync(AttendanceForCreationDto Attendance);
        Task<bool> UpdateAttendanceAsync(AttendanceForUpdateDto ForUpdate, Attendance Attendance);
        Task<IEnumerable<Attendance>> GetAllAttendanceAsync();
        Task<IEnumerable<Attendance>> GetAllAttendanceOfSupervisorAsync(int idsupervisor);
        #endregion

        #region UserNotFound 
        Task<UserNotFound?> FetchUserNotFoundAsync(int userNotFoundId);
        Task<UserNotFound> CreateUserNotFoundAsync(UserNotFoundForCreation userNotFound);
        Task UpdateUserNotFoundAsync(UserNotFoundForUpdateDto userNotFoundForUpdate, int userNotFoundId);
        #endregion
    }
}
