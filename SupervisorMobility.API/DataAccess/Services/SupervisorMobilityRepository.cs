using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;

namespace SupervisorMobility.API.Services
{
    public class SupervisorMobilityRepository : ISupervisorMobilityRepository
    {
        private readonly SupervisorMobilityContext _context;

        public SupervisorMobilityRepository(SupervisorMobilityContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region ChecklistCategoryOperations

        public void AddChecklistCategory(ChecklistCategory checklistCategory)
        {
            _context.ChecklistCategories.Add(checklistCategory);
        }

        public async Task<bool> ChecklistCategoryExistAsync(int checklistCategoryId)
        {
            return await _context.ChecklistCategories.AnyAsync(c => c.ChecklistCategoryId == checklistCategoryId);
        }

        public void DeleteChecklistCategory(ChecklistCategory checklistCategory)
        {
            _context.ChecklistCategories.Remove(checklistCategory);
        }

        public async Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesAsync()
        {
            return await _context.ChecklistCategories
                .OrderBy(c => c.Sequence).ToListAsync();
        }

        public async Task<ChecklistCategory?> GetChecklistCategoryAsync(int categoryId, bool includeChecklistQuestion = false)
        {
            if (includeChecklistQuestion)
            {
                return await _context.ChecklistCategories.Include(cq => cq.ChecklistQuestions)
                    .Where(c => c.ChecklistCategoryId == categoryId).FirstOrDefaultAsync();
            }

            return await _context.ChecklistCategories
                .Where(c => c.ChecklistCategoryId == categoryId).FirstOrDefaultAsync();
        }

        public async Task<int> GetChecklistCategoriesMaxSequenceAsync()
        {
            return await _context.ChecklistCategories.MaxAsync(cc => cc.Sequence) + 1;
        }

        public async Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesForUpdateSequenceAsync(
            int currentSequence, int oldSequence, int categoryId)
        {
            int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
            int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

            return await _context.ChecklistCategories
                        .Where(c => c.Sequence >= lowerValue
                            && c.Sequence <= upperValue
                            && c.ChecklistCategoryId != categoryId)
                        .OrderBy(c => c.Sequence).ToListAsync();
        }
        #endregion
        #region JobObservationTypesOperations
        public async Task<IEnumerable<JobObservationType>> GetJobObservationTypesAsync()
        {
            return await _context.JobObservationTypes
                .OrderBy(c => c.JobObservationTypeId).ToListAsync();
        }

        public async Task<JobObservationType?> GetJobObservationTypeAsync(int id, bool includeConfigs = false)
        {
            if (includeConfigs)
            {
                return await _context.JobObservationTypes.Include(jot => jot.JobObservationConfigs)
                    .Where(c => c.JobObservationTypeId == id).FirstOrDefaultAsync();
            }

            return await _context.JobObservationTypes
                .Where(c => c.JobObservationTypeId == id).FirstOrDefaultAsync();
        }

        public void AddJobObservationType(JobObservationType jobObservationType)
        {
            _context.JobObservationTypes.Add(jobObservationType);
        }

        public void DeleteJobObservationType(JobObservationType jobObservationType)
        {
            _context.JobObservationTypes.Remove(jobObservationType);
        }

        public async Task<bool> JobObservationTypeExistAsync(int jobObservationTypeId)
        {
            return await _context.JobObservationTypes.AnyAsync(c => c.JobObservationTypeId == jobObservationTypeId);
        }
        #endregion
        #region GroupOperations
        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            return await _context.Groups
                .OrderBy(c => c.GroupId).ToListAsync();
        }

        public async Task<Group?> GetGroupAsync(int groupId)
        {
            return await _context.Groups
                .Where(c => c.GroupId == groupId).FirstOrDefaultAsync();
        }

        public async Task<bool> GroupExistAsync(int groupId)
        {
            return await _context.Groups.AnyAsync(p => p.GroupId == groupId);
        }


        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void DeleteGroup(Group group)
        {
            _context.Groups.Remove(group);
        }
        #endregion
        #region PlantOperations
        public async Task<IEnumerable<Plant>> GetPlantsAsync()
        {
            return await _context.Plants
                .OrderBy(c => c.PlantId).ToListAsync();
        }

        public async Task<Plant?> GetPlantAsync(int plantId, bool includeAreas = false)
        {
            if (includeAreas)
            {
                return await _context.Plants.Include(p => p.Areas)
                    .Where(p => p.PlantId == plantId).FirstOrDefaultAsync();
            }

            return await _context.Plants
                .Where(p => p.PlantId == plantId).FirstOrDefaultAsync();
        }
        public async Task<Plant?> GetPlantByCodeAndDescriptionAsync(string code, string description)
        {
            return await _context.Plants
                .Where(p => p.Code == code && p.Description == description).FirstOrDefaultAsync();
        }
        public async Task<bool> PlantExistAsync(int plantId)
        {
            return await _context.Plants.AnyAsync(p => p.PlantId == plantId);
        }
        public async Task<bool> PlantExistByCodeAndDescriptionAsync(string code, string description)
        {
            return await _context.Plants.AnyAsync(p => p.Code == code && p.Description == description);
        }

        public void AddPlant(Plant plant)
        {
            _context.Plants.Add(plant);
        }

        public void DeletePlant(Plant plant)
        {
            _context.Plants.Remove(plant);
        }
        #endregion
        #region AreaOperations
        public async Task<IEnumerable<Area>> GetAreasForPlantAsync(int plantId)
        {
            return await _context.Areas
                .Where(a => a.PlantId == plantId).ToListAsync();
        }
        public async Task<Area?> GetAreaForPlantAsync(int plantId,
            int areaId, bool includeOperations = false)
        {
            if (includeOperations)
            {
                return await _context.Areas.Include(a => a.Distributions)
                .Where(a => a.PlantId == plantId && a.AreaId == areaId)
                .FirstOrDefaultAsync();
            }
            return await _context.Areas
                .Where(a => a.PlantId == plantId && a.AreaId == areaId)
                .FirstOrDefaultAsync();
        }
        public async Task<Area?> GetAreaForPlantByCodeAndDescriptionAsync(int plantId,
            string code, string description)
        {

            return await _context.Areas
                .Where(a => a.PlantId == plantId && a.Code == code && a.Description == description)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> AreaExistAsync(int areaId)
        {
            return await _context.Areas.AnyAsync(p => p.AreaId == areaId);
        }


        public async Task<bool> AreaExistByCodeAndDescriptionInPlantAsync(string code, string description, int plantId)
        {
            return await _context.Areas.AnyAsync(a => a.PlantId == plantId && a.Code == code && a.Description == description);
        }

        public async Task AddAreaForPlantAsync(int plantId, Area area)
        {
            var plant = await GetPlantAsync(plantId);
            if (plant != null)
            {
                plant.Areas.Add(area);
            }
        }
        public void DeleteArea(Area area)
        {
            _context.Areas.Remove(area);
        }
        #endregion
        #region DistributionOperations

        public async Task<IEnumerable<Distribution>> GetDistributionsForAreaAsync(int areaId, bool includecollections = false)
        {

            if (includecollections)
            {
                return await _context.Distributions.Include(o => o.Operations).Include(p => p.Products)
                     .Where(o => o.AreaId == areaId)
                    .ToListAsync();
            }

            return await _context.Distributions
                .Where(o => o.AreaId == areaId).ToListAsync();
        }
        public async Task<Distribution?> GetDistributionForAreaAsync(int areaId, int distributionId, bool includeCollections = false)
        {
            if (includeCollections)
            {
                return await _context.Distributions.Include(o => o.Operations).Include(p => p.Products)
                     .Where(o => o.AreaId == areaId && o.DistributionId == distributionId)
                    .FirstOrDefaultAsync();
            }


            return await _context.Distributions
                .Where(o => o.AreaId == areaId && o.DistributionId == distributionId)
                .FirstOrDefaultAsync();
        }
        public async Task<Distribution?> GetDistributionForAreaByCodeAndDescriptionAsync(int areaId, string code, string description)
        {
            return await _context.Distributions
                .Where(o => o.AreaId == areaId && o.Code == code && o.Description == description)
                .FirstOrDefaultAsync();
        }

        public async Task AddDistributionForPlantAsync(int plantId, int areaId, Distribution distribution)
        {
            var area = await GetAreaForPlantAsync(plantId, areaId);
            if (area != null)
            {
                area.Distributions.Add(distribution);
            }
        }
        public async Task<bool> DistributionExistsAsync(int distributionId)
        {
            return await _context.Distributions.AnyAsync(p => p.DistributionId == distributionId);
        }
        public async Task<bool> DistributionExistsByCodeandDescriptionInAreaAsync(int areaid, string code, string description)
        {
            return await _context.Distributions.AnyAsync(p => p.AreaId == areaid && p.Code == code && p.Description == description);
        }
        public void DeleteDistribution(Distribution distribution)
        {
            _context.Distributions.Remove(distribution);
        }
        #endregion
        #region OperationOperations
        public async Task<IEnumerable<Operation>> GetOperationsForDistributionAsync(int distributionId)
        {
            return await _context.Operations
                .Where(o => o.DistributionId == distributionId).ToListAsync();
        }
        public async Task<Operation?> GetOperationForDistributionAsync(int distributionId, int operationId)
        {
            return await _context.Operations
                .Where(o => o.DistributionId == distributionId && o.OperationId == operationId)
                .FirstOrDefaultAsync();
        }
        public async Task<Operation?> GetOperationForDistributionByCodeAndDescriptionAsync(int distributionId, string opcode, string opdescription)
        {
            return await _context.Operations
                .Where(o => o.DistributionId == distributionId && o.Code == opcode && o.Description == opdescription)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> OperationExistsAsync(int operationId)
        {
            return await _context.Operations.AnyAsync(p => p.OperationId == operationId);
        }
        public async Task<bool> OperationExistsByCodeAndDescriptionInDistributionAsync(int distributionId, string code, string description)
        {
            return await _context.Operations.AnyAsync(p => p.DistributionId == distributionId && p.Code == code && p.Description == description);
        }
        public async Task AddOperationForDistributionAsync(int areaId, int distributionId, Operation operation)
        {
            var distribution = await GetDistributionForAreaAsync(areaId, distributionId);
            if (distribution != null)
            {
                distribution.Operations.Add(operation);
            }
        }
        public void DeleteOperation(Operation operation)
        {
            _context.Operations.Remove(operation);
        }
        #endregion
        #region QuestionTypeOperations

        public async Task<IEnumerable<QuestionType>> GetQuestionTypesAsync()
        {
            return await _context.QuestionTypes.OrderBy(q => q.QuestionTypeId).ToListAsync();
        }

        public async Task<QuestionType?> GetQuestionTypeAsync(int questionTypeId, bool includeChecklistQuestions = false)
        {
            if (includeChecklistQuestions)
            {
                return await _context.QuestionTypes.Include(cq => cq.ChecklistQuestions)
                    .Where(q => q.QuestionTypeId == questionTypeId).FirstOrDefaultAsync();
            }

            return await _context.QuestionTypes
                .Where(c => c.QuestionTypeId == questionTypeId).FirstOrDefaultAsync();
        }


        #endregion
        #region ChecklistQuestionOperations

        public async Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForCategoryAsync(int categoryId)
        {
            return await _context.ChecklistQuestions
                .Where(cq => cq.ChecklistCategoryId == categoryId)
                .OrderBy(cq => cq.CategorySequence).ToListAsync();
        }
        public async Task<ChecklistQuestion?> GetChecklistQuestionForCategoryAsync(int categoryId,
            int questionId)
        {
            return await _context.ChecklistQuestions
                .Where(cq => cq.ChecklistCategoryId == categoryId && cq.QuestionID == questionId)
                .FirstOrDefaultAsync();
        }
        public async Task AddChecklistQuestionForCategoryAsync(int categoryId, ChecklistQuestion checklistQuestion)
        {
            var checklistCategory = await GetChecklistCategoryAsync(categoryId, false);
            if (checklistCategory != null)
            {
                checklistCategory.ChecklistQuestions.Add(checklistQuestion);
            }
        }

        public async Task<int> GetChecklistQuestionMaxCategorySequenceAsync(int categoryId)
        {
            var sequence = await _context.ChecklistQuestions
                .Where(cq => cq.ChecklistCategoryId == categoryId)
                .MaxAsync(cq => (int?)cq.CategorySequence) ?? 0;
            return sequence + 1;
        }

        public void DeleteChecklistQuestions(ChecklistQuestion checklistQuestion)
        {
            _context.ChecklistQuestions.Remove(checklistQuestion);
        }

        public async Task<IEnumerable<ChecklistQuestion>> GetChecklistQuestionsForUpdateSequenceAsync(
                int currentSequence, int oldSequence, int categoryId, int checklistQuestionId)
        {
            int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
            int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

            return await _context.ChecklistQuestions
                        .Where(c => c.ChecklistCategoryId == categoryId
                            && c.CategorySequence >= lowerValue
                            && c.CategorySequence <= upperValue
                            && c.QuestionID != checklistQuestionId)
                        .OrderBy(c => c.CategorySequence).ToListAsync();
        }
        #endregion
        #region JobObservationConfigOperations
        public async Task<IEnumerable<JobObservationConfig>> GetJobOperationConfigsForJobOperationTypeAsync(int jobObservationTypeId)
        {
            return await _context.JobObservationConfigs
                .Where(joc => joc.JobObservationTypeId == jobObservationTypeId).ToListAsync();
        }
        public async Task<JobObservationConfig?> GetJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId,
            int jobObservationConfigId)
        {
            return await _context.JobObservationConfigs
                .Where(joc => joc.JobObservationTypeId == jobObservationTypeId
                           && joc.JobObservationConfigId == jobObservationConfigId)
                .FirstOrDefaultAsync();
        }
        public async Task AddJobOperationConfigForJobOperationTypeAsync(int jobObservationTypeId, JobObservationConfig jobObservationConfig)
        {
            var jobOperationType = await GetJobObservationTypeAsync(jobObservationTypeId);
            if (jobOperationType != null)
            {
                jobOperationType.JobObservationConfigs.Add(jobObservationConfig);
            }
        }
        public void DeleteJobOperationConfig(JobObservationConfig jobObservationConfig)
        {
            _context.JobObservationConfigs.Remove(jobObservationConfig);
        }
        #endregion
        #region SupportDocumentTypeOperations
        public async Task<IEnumerable<SupportDocumentType>> GetSupportDocumentTypesAsync()
        {
            return await _context.SupportDocumentTypes
                .OrderBy(c => c.SupportDocumentTypeId).ToListAsync();
        }

        public async Task<SupportDocumentType?> GetSupportDocumentTypeAsync(int supportDocumentTypeId)
        {
            return await _context.SupportDocumentTypes
                .Where(p => p.SupportDocumentTypeId == supportDocumentTypeId).FirstOrDefaultAsync();
        }
        public async Task<bool> SupportDocumentTypeExistAsync(int supportDocumentTypeId)
        {
            return await _context.SupportDocumentTypes.AnyAsync(p => p.SupportDocumentTypeId == supportDocumentTypeId);
        }

        public void AddSupportDocumentType(SupportDocumentType supportDocumentType)
        {
            _context.SupportDocumentTypes.Add(supportDocumentType);
        }

        public void DeleteSupportDocumentType(SupportDocumentType supportDocumentType)
        {
            _context.SupportDocumentTypes.Remove(supportDocumentType);
        }
        #endregion
        #region ProductOperations
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products
                .OrderBy(c => c.ProductId).ToListAsync();
        }

        public async Task<Product?> GetProductAsync(int productId, bool collection = false)
        {
            if(collection)
            {
                return await _context.Products.Include(d => d.Distributions).Where(p => p.ProductId == productId).FirstOrDefaultAsync();
            }

            return await _context.Products
                .Where(p => p.ProductId == productId).FirstOrDefaultAsync();
        }
        public async Task<Product?> GetProductByCodeAndDescriptionAsync(string code, string description)
        {
            return await _context.Products
                .Where(p => p.Code == code && p.Description == description).FirstOrDefaultAsync();
        }
        public async Task<bool> ProductExistAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == productId);
        }
        public async Task<bool> ProductExistByCodeAndDescriptionAsync(string code, string description)
        {
            return await _context.Products.AnyAsync(p => p.Code == code && p.Description == description);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }
        #endregion
        #region AssyChart
        public async Task<IEnumerable<AssyChart>> GetAllAssyChartsAsync()
        {
            return await _context.AssyCharts
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.Product)
                 .OrderBy(c => c.AssyChardId).ToListAsync();
        }
        public async Task<AssyChart?> GetAssyChartAsync(int asssychartId)
        {
            return await _context.AssyCharts.Include(o => o.Operation)
                 .Where(p => p.AssyChardId == asssychartId).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<AssyChart>> GetAssyChartByPlantAsync(int plantId)
        {
            return await _context.AssyCharts.Where(plant => plant.PlantId == plantId)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.Product)
                .OrderBy(c => c.AssyChardId).ToListAsync();
        }


        public async Task<AssyChart?> GetAssyChartAdvanceAsync(string GOS, string CCP, string HOE, int PlantId, int AreaId, int DistributionId, int OperationId, int Productid)
        {
            //return whit info
            return await _context.AssyCharts
                 .Where(p => p.GOS == GOS && p.CCP == CCP && p.HOE == HOE && p.PlantId == PlantId && p.AreaId == AreaId && p.DistributionId == DistributionId && p.OperationId == OperationId && p.ProductId == Productid).FirstOrDefaultAsync();
        }

        public async Task<bool> AssyChartExistAsync(int assychartID)
        {
            return await _context.AssyCharts.AnyAsync(p => p.AssyChardId == assychartID);
        }
        public async Task<bool> AssyChartExistAdvanceAsync(string GOS, string CCP, string HOE, int PlantId, int AreaId, int DistributionId, int OperationId, int Productid)
        {
            return await _context.AssyCharts.AnyAsync(p => p.GOS == GOS && p.CCP == CCP && p.HOE == HOE && p.PlantId == PlantId && p.AreaId == AreaId && p.DistributionId == DistributionId && p.OperationId == OperationId && p.ProductId == Productid);
        }

        public void AddAssyChartAsync(AssyChart assychart)
        {
            _context.AssyCharts.Add(assychart);
        }

        public void DeleteAssyChartAsync(AssyChart assyChart)
        {
            _context.AssyCharts.Remove(assyChart);
        }
        #endregion


        #region Users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                 .OrderBy(c => c.UserId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersWhitPlantAreaAndGroupAsync()
        {
            return await _context.Users
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(g => g.Group)
                 .OrderBy(c => c.UserId).ToListAsync();
        }

        public async Task<User?> GetUserAsync(int userId, bool collection = false)
        {
            if (collection)
            {
                return await _context.Users.Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(g => g.Group)
                .Where(p => p.UserId == userId).FirstOrDefaultAsync();
            }
            return await _context.Users.Where(p => p.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByNominaAsync(int nomina)
        {
            return await _context.Users.Where(p => p.Payroll == nomina).FirstOrDefaultAsync();

        }

        public async Task<bool> UserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(p => p.UserId == userId);
        }

        public async Task<bool> UserExistAdvanceAsync(string nombre, int nomina, int plantid, int areaid, int grupoid)
        {
            return await _context.Users.AnyAsync(p => p.Name == nombre && p.Payroll == nomina && p.PlantId == plantid && p.AreaId == areaid && p.GroupId == grupoid) ;

        }

        public void AddUserAsync(User user)
        {
            _context.Users.Add(user);

        }

        public void DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);

        }
        #endregion

        #region CommonOperations
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        #endregion

        #region JobObservationOperations

        public async Task<IEnumerable<JobObservation>> GetAllJobObservationsAsync()
        {
            return await _context.JobObservations
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                 .OrderBy(c => c.JobObservationId).ToListAsync();

        }

        public async Task<JobObservation?> GetJobObservationAsync(int jobObservationId)
        {
            //return whit info
            return await _context.JobObservations
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                 .Where(p => p.JobObservationId == jobObservationId).FirstOrDefaultAsync();
        }

        public void AddJobObservation(JobObservation jobObservation)
        {
            _context.JobObservations.Add(jobObservation);
        }

        public void DeleteJobObservation(JobObservation jobObservation)
        {
            _context.JobObservations.Remove(jobObservation);
        }

        #endregion

        #region GlosaryOperations

        public async Task<IEnumerable<Glosary>> GetGlosaryAsync()
        {
            return await _context.Glosary
                .OrderBy(c => c.GlosaryWordId).ToListAsync();
        }

        public async Task<Glosary?> GetGlosaryWordAsync(int glosaryWordId)
        {
            return await _context.Glosary
                .Where(c => c.GlosaryWordId == glosaryWordId).FirstOrDefaultAsync();
        }

        public void AddGlosaryWord(Glosary glosaryWord)
        {
            _context.Glosary.Add(glosaryWord);
        }

        public void DeleteGlosaryWord(Glosary glosaryWord)
        {
            _context.Glosary.Remove(glosaryWord);
        }
        #endregion

    }
}
