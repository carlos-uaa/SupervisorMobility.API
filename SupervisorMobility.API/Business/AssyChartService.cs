using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.AttendanceDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.GuidesDtos;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Business
{
    public class AssyChartService : IAssyChartService
    {
        private readonly ISupervisorMobilityRepository _repository;
        private readonly IMapper _mapper;

        public AssyChartService(ISupervisorMobilityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #region Product
        public async Task<bool> CheckProductExistance(int productId)
        {
            return await _repository.ProductExistAsync(productId);
        }
        #endregion
        #region Area

        public async Task<bool> CheckAreaExistance(int areaId)
        {
            return await _repository.AreaExistAsync(areaId);
        }
        #endregion
        #region Distribution
        public async Task<bool> CheckDistributionExistance(int distributionId)
        {
            return await _repository.DistributionExistsAsync(distributionId);
        }
        #endregion
        #region Operation
        public async Task<Operation> CreateOperationAsync(int areaId, int distributionId, Operation operation)
        {
            await _repository.AddOperationForDistributionAsync(areaId, distributionId, operation);
            await _repository.SaveChangesAsync();
            return operation;

        }
        public async Task<Operation?> FetchOperationAsync(int distributionId, int operationId)
        {
            return await _repository.GetOperationForDistributionAsync(distributionId, operationId);
        }

        public async Task<IEnumerable<Operation>> FetchOperationsAsync(int distributionId)
        {
            return await _repository.GetOperationsForDistributionAsync(distributionId);
        }

        public async Task RemoveOperationAsync(Operation operation)
        {
            _repository.DeleteOperation(operation);
            await _repository.SaveChangesAsync();
        }
        public async Task UpdateOperationAsync(
            OperationForUpdateDto operationForUpdate,
            Operation operation)
        {
            _mapper.Map(operationForUpdate, operation);
            await _repository.SaveChangesAsync();
        }


        #endregion
        #region Product
        public async Task<Product> CreateProductAsync(ProductForCreationDto product)
        {
            var productEntity = _mapper.Map<Product>(product);
            _repository.AddProduct(productEntity);
            await _repository.SaveChangesAsync();
            return productEntity;

        }

        public async Task<Product?> FetchProductAsync(int productId, bool collection = false)
        {

            return await _repository.GetProductAsync(productId, collection);


        }




        public async Task<IEnumerable<Product>> FetchProductsAsync()
        {
            return await _repository.GetProductsAsync();
        }




        public async Task RemoveProductAsync(Product product)
        {
            _repository.DeleteProduct(product);
            await _repository.SaveChangesAsync();
        }


        public async Task UpdateProductAsync(ProductForUpdateDto productForUpdate, Product product)
        {
            _mapper.Map(productForUpdate, product);
            await _repository.SaveChangesAsync();
        }


        #endregion
        #region Plant
        public async Task<IEnumerable<Plant>> FetchPlantsAsync()
        {
            return await _repository.GetPlantsAsync();
        }



        public async Task<Plant?> FetchPlantAsync(int plantId, bool includeAreas = false)
        {
            return await _repository.GetPlantAsync(plantId, includeAreas);
        }
        public async Task<Plant> CreatePlantAsync(PlantForCreationDto plant)
        {
            var finalPlant = _mapper.Map<Entities.Plant>(plant);
            _repository.AddPlant(finalPlant);
            await _repository.SaveChangesAsync();

            return finalPlant;
        }
        public async Task UpdatePlantAsync(PlantForUpdateDto plantForUpdate, Plant plant)
        {
            _mapper.Map(plantForUpdate, plant);
            await _repository.SaveChangesAsync();
        }
        public async Task RemovePlantAsync(Plant plant)
        {
            _repository.DeletePlant(plant);
            await _repository.SaveChangesAsync();
        }
        public async Task<bool> CheckPlantExistance(int plantId)
        {
            return await _repository.PlantExistAsync(plantId);
        }
        #endregion
        #region SupportDocumentTypes
        public async Task<SupportDocumentType> CreateSupportDocumentTypeAsync(SupportDocumentType supportDocumentType)
        {
            _repository.AddSupportDocumentType(supportDocumentType);
            await _repository.SaveChangesAsync();
            return supportDocumentType;
        }
        public async Task<SupportDocumentType?> FetchSupportDocumentTypeAsync(int supportDocumentTypeId)
        {
            return await _repository
                .GetSupportDocumentTypeAsync(supportDocumentTypeId);
        }
        public async Task<IEnumerable<SupportDocumentType>> FetchSupportDocumentTypesAsync()
        {
            return await _repository.GetSupportDocumentTypesAsync();
        }
        public async Task RemoveSupportDocumentTypeAsync(SupportDocumentType supportDocumentType)
        {
            _repository.DeleteSupportDocumentType(supportDocumentType);
            await _repository.SaveChangesAsync();
        }
        public async Task UpdateSupportDocumentTypeAsync(
            SupportDocumentTypeForUpdateDto supportDocumentTypeForUpdate,
            SupportDocumentType supportDocumentType)
        {
            _mapper.Map(supportDocumentTypeForUpdate, supportDocumentType);
            await _repository.SaveChangesAsync();
        }
        #endregion
        #region AssyChart
        public async Task<AssyChart> CreateAssyChartAsync(AssyChartForCreation assyChart)
        {
            var finalasssychart = _mapper.Map<AssyChart>(assyChart);
            _repository.AddAssyChartAsync(finalasssychart);
            await _repository.SaveChangesAsync();
            return finalasssychart;
        }

        public async Task UpdateAssyChartAsync(AssyChartForUpdateDto assyChartUpdate, AssyChart assyChart)
        {
            _mapper.Map(assyChartUpdate, assyChart);
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveAssyChartAsync(AssyChart assyChart)
        {
            _repository.DeleteAssyChartAsync(assyChart);
            await _repository.SaveChangesAsync();
        }

        #endregion
        #region User

        public async Task<User?> FetchUserAsync(int userId)
        {
            return await _repository.GetUserAsync(userId);
        }
        public async Task<User?> FetchUserWhitObjectIdAsync(string objectId)
        {
            return await _repository.GetUserByObjectIdAsync(objectId);
        }

        public async Task<User?> FetchUserByEmailAsync(string email)
        {
            return await _repository.GetUserByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _repository.GetAllUsersAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersWhitPlantAreaAndGroupAsync()
        {
            return await _repository.GetAllUsersWhitPlantAreaAndGroupAsync();
        }
        public async Task<User> CreateUserAsync(UsersForCreation newuser)
        {
            var finaluser = _mapper.Map<User>(newuser);
            await _repository.AddUserAsync(finaluser);
            return finaluser;
        }

        public async Task UpdateUserAsync(UsersForUpdateDto userForUpdate, int UserId)
        {
            await _repository.UpdateUser(userForUpdate, UserId);
        }

        public async Task RemoveUserAsync(User user)
        {
            _repository.DeleteUserAsync(user);
            await _repository.SaveChangesAsync();
        }

        #endregion
        #region File
        public async Task<FileUpload> CreateFileAsync(FileUploadForCreationDto newFile)
        {
            var finalNewFile = _mapper.Map<FileUpload>(newFile);
            _repository.AddUploadFile(finalNewFile);
            await _repository.SaveChangesAsync();
            return finalNewFile;
        }
        public async Task<FileUpload> CreateLupFileAsync(FileUploadForCreationDto newFile, int lupId)
        {
            var finalNewFile = _mapper.Map<FileUpload>(newFile);
            _repository.AddUploadFile(finalNewFile);
            await _repository.SaveChangesAsync();
            return finalNewFile;
        }

        public async Task<FileUpload?> FetchFileAsync(int fileid)
        {
            return await _repository.GetFileUploadAsync(fileid);
        }
        public async Task RemoveFileAsync(FileUpload fileUpload)
        {
            _repository.DeleteUploadFile(fileUpload);
            await _repository.SaveChangesAsync();
        }
        #endregion
        #region Guide
        public async Task<Guides?> FetchGuideAsync(int guideId, bool includeFile = false)
        {
            return await _repository.GetGuideAsync(guideId, includeFile);
        }
        public async Task<Guides> CreateGuideAsync(GuideForCreationDto newguide)
        {
            var finalNewGuide = _mapper.Map<Guides>(newguide);
            _repository.AddGuide(finalNewGuide);


            //var FileGuide


            await _repository.SaveChangesAsync();
            return finalNewGuide;
        }
        public async Task UpdateGuideAsync(GuideForUpdateDto GuideToUpdate, Guides guide)
        {
            _mapper.Map(GuideToUpdate, guide);
            await _repository.SaveChangesAsync();
        }
        public async Task RemoveGuideAsync(Guides guide)
        {
            _repository.DeleteGuide(guide);
            await _repository.SaveChangesAsync();
        }
        #endregion
        #region HistoyJobObservation
        public async Task<JobObservationVersion?> FetchHistoryJobObservationAsync(int HistoyJobObservationId)
        {
            return await _repository.GetHistoryJobObservationAsync(HistoyJobObservationId);
        }
        public async Task<JobObservationVersion> CreateHistoryJobObservationAsync(Entities.JobObservation jobObservationEntity)
        {
            var HistoryToAdd = _mapper.Map<JobObservationVersion>(jobObservationEntity);

            _repository.AddHistoyJobObservationAsync(HistoryToAdd);
            await _repository.SaveChangesAsync();
            return HistoryToAdd;
        }


        public Task UpdateHistoryJobObservationAsync(JobObservationVersion ForUpdate, JobObservationVersion JobObservation)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveHistoryJobObservationAsync(JobObservationVersion HistoryVersion, Entities.JobObservation jobObservation)
        {
            bool state = await _repository.DeleteHistoyFromJobObservationAsync(HistoryVersion, jobObservation);

            return state;
        }
        #endregion
        #region Notification
        public async Task<Notification?> FetchNotificationAsync(int notifyId)
        {
            return await _repository.GetNotificationAsync(notifyId);

        }
        public async Task<Notification> CreateNotificationAsync(NotificationToCreateDto Notification)
        {
            var NotifyToAdd = _mapper.Map<Notification>(Notification);

            _repository.AddNotificationAsync(NotifyToAdd);
            await _repository.SaveChangesAsync();
            return NotifyToAdd;
        }

        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            return await _repository.GetAllNotificationsAsync();
        }
        public async Task<IEnumerable<Notification>> GetNotificationsFromUser(int iduser)
        {
            return await _repository.GetAllNotificationsFromUserAsync(iduser);
        }
        public async Task<bool> UpdateNotificationAsync(NotificationForUpdateDto ForUpdate, Notification notifyEntity)
        {
            _mapper.Map(ForUpdate, notifyEntity);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveNotificationAsync(Notification notificationEntity)
        {
            _repository.DeleteNotificationAsync(notificationEntity);
            await _repository.SaveChangesAsync();
            return true;
        }
        #endregion
        #region Attendance
        public async Task<Attendance> FetchAttendanceByIdAsync(int AttendanceId)
        {
            return await _repository.GetAttendanceById(AttendanceId);
        }

        public async Task<Attendance> CreateAttendanceAsync(AttendanceForCreationDto AttendanceForCreate)
        {
            var AttendanceToAdd = _mapper.Map<Attendance>(AttendanceForCreate);

            _repository.AddAttendance(AttendanceToAdd);
            await _repository.SaveChangesAsync();
            return AttendanceToAdd;
        }

        public async Task<bool> UpdateAttendanceAsync(AttendanceForUpdateDto ForUpdate, Attendance attendance)
        {
            _mapper.Map(ForUpdate, attendance);
            await _repository.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<Attendance>> GetAllAttendanceAsync()
        {
            return await _repository.GetAllAttendance();
        }
        public async Task<IEnumerable<Attendance>> GetAllAttendanceOfSupervisorAsync(int idsupervisor)
        {
            return await _repository.GetAllAttendanceOfSupervisor(idsupervisor);
        }


        #endregion

        #region UserNotFound

        public async Task<UserNotFound?> FetchUserNotFoundAsync(int userNotFoundId)
        {
            return await _repository.GetUserNotFoundAsync(userNotFoundId);
        }
        public async Task<UserNotFound> CreateUserNotFoundAsync(UserNotFoundForCreation newuser)
        {
            var finaluser = _mapper.Map<UserNotFound>(newuser);
            await _repository.AddUserNotFoundAsync(finaluser);
            return finaluser;
        }

        public async Task UpdateUserNotFoundAsync(UserNotFoundForUpdateDto userNotFoundForUpdate, int userNotFoundId)
        {
            await _repository.UpdateUserNotFound(userNotFoundForUpdate, userNotFoundId);
        }

        #endregion

        #region Pat

        public async Task<PAT?> FetchPatAsync(int plantId)
        {
            return await _repository.GetPat(plantId);
        }

        #endregion
    }
}
