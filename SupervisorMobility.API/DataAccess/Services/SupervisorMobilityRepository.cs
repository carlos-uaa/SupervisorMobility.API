
using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.PATDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using SupervisorMobility.API.Models.Users;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.Services
{
    public class SupervisorMobilityRepository : ISupervisorMobilityRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;


        public SupervisorMobilityRepository(SupervisorMobilityContext context, IMapper mapper)
        {
            _mapper = mapper;
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
            checklistCategory.IsActive = false;
            _context.SaveChanges();
            //_context.ChecklistCategories.Remove(checklistCategory);
        }

        public async Task<IEnumerable<ChecklistCategory>> GetChecklistCategoriesAsync()
        {
            return await _context.ChecklistCategories.Where(u => u.IsActive == true)
                .OrderBy(c => c.Sequence).ToListAsync();
        }

        public async Task<ChecklistCategory?> GetChecklistCategoryAsync(int categoryId, bool includeChecklistQuestion = false)
        {
            if (includeChecklistQuestion)
            {
                return await _context.ChecklistCategories.Include(cq => cq.ChecklistQuestions)
                    .Where(c => c.ChecklistCategoryId == categoryId ).FirstOrDefaultAsync();
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
                            && c.ChecklistCategoryId != categoryId 
                            && c.IsActive == true)
                        .OrderBy(c => c.Sequence).ToListAsync();
        }
        #endregion
        #region JobObservationTypesOperations
        public async Task<IEnumerable<JobObservationType>> GetJobObservationTypesAsync()
        {
            return await _context.JobObservationTypes.Where(u => u.IsActive == true)
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
                .Where(c => c.JobObservationTypeId == id ).FirstOrDefaultAsync();
        }

        public void AddJobObservationType(JobObservationType jobObservationType)
        {
            _context.JobObservationTypes.Add(jobObservationType);
        }

        public void DeleteJobObservationType(JobObservationType jobObservationType)
        {
            //_context.JobObservationTypes.Remove(jobObservationType);
            jobObservationType.IsActive = false;
            _context.SaveChanges();
        }

        public async Task<bool> JobObservationTypeExistAsync(int jobObservationTypeId)
        {
            return await _context.JobObservationTypes.AnyAsync(c => c.JobObservationTypeId == jobObservationTypeId);
        }
        #endregion
        #region GroupOperations
        public async Task<IEnumerable<Entities.Group>> GetGroupsAsync()
        {
            return await _context.Groups.Where(u => u.IsActive == true)
                .OrderBy(c => c.GroupId).ToListAsync();
        }

        public async Task<Entities.Group?> GetGroupAsync(int groupId)
        {
            return await _context.Groups
                .Where(c => c.GroupId == groupId).FirstOrDefaultAsync();
        }

        public async Task<bool> GroupExistAsync(int groupId)
        {
            return await _context.Groups.AnyAsync(p => p.GroupId == groupId);
        }


        public void AddGroup(Entities.Group group)
        {
            _context.Groups.Add(group);
        }

        public void DeleteGroup(Entities.Group group)
        {
            //_context.Groups.Remove(group);
            group.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region PlantOperations
        public async Task<IEnumerable<Plant>> GetPlantsAsync()
        {
            return await _context.Plants.Where(u => u.IsActive == true)
                .OrderBy(c => c.PlantId).ToListAsync();
        }

        public async Task<Plant?> GetPlantAsync(int plantId, bool includeAreas = false)
        {
            if (includeAreas)
            {
                return await _context.Plants.Include(p => p.Areas)
                    .Where(p => p.PlantId == plantId ).FirstOrDefaultAsync();
            }

            return await _context.Plants
                .Where(p => p.PlantId == plantId ).FirstOrDefaultAsync();
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
            //_context.Plants.Remove(plant);
            plant.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region AreaOperations
        public async Task<IEnumerable<Area>> GetAreasForPlantAsync(int plantId, bool includeCollections = false)
        {

            if (includeCollections)
            {
                return await _context.Areas.Include(a => a.Distributions)
              .Where(a => a.PlantId == plantId && a.IsActive == true).ToListAsync();
            }

            return await _context.Areas
                .Where(a => a.PlantId == plantId && a.IsActive == true).ToListAsync();
        }
        public async Task<Area?> GetAreaForPlantAsync(int plantId,
            int areaId, bool includeOperations = false)
        {
            if (includeOperations)
            {
                return await _context.Areas.Include(a => a.Distributions)
                .Where(a => a.PlantId == plantId && a.AreaId == areaId )
                .FirstOrDefaultAsync();
            }
            return await _context.Areas
                .Where(a => a.PlantId == plantId && a.AreaId == areaId )
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
        public async Task<AsyncVoidMethodBuilder> AddArea(Area area)
        {
            var resp = new AsyncVoidMethodBuilder();
            await _context.Areas.AddAsync(area);
            await _context.SaveChangesAsync();
            return resp;
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
            //_context.Areas.Remove(area);
            area.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region DistributionOperations

        public async Task<IEnumerable<Distribution>> GetDistributionsForAreaAsync(int areaId, bool includecollections = false)
        {

            if (includecollections)
            {
                return await _context.Distributions.Include(o => o.Operations).Include(p => p.Products)
                     .Where(o => o.AreaId == areaId && o.IsActive == true)
                    .ToListAsync();
            }

            return await _context.Distributions
                .Where(o => o.AreaId == areaId && o.IsActive == true).ToListAsync();
        }
        public async Task<Distribution?> GetDistributionForAreaAsync(int areaId, int distributionId, bool includeCollections = false)
        {
            if (includeCollections)
            {
                return await _context.Distributions.Include(o => o.Operations).Include(p => p.Products)
                     .Where(o => o.AreaId == areaId && o.DistributionId == distributionId )
                    .FirstOrDefaultAsync();
            }


            return await _context.Distributions
                .Where(o => o.AreaId == areaId && o.DistributionId == distributionId )
                .FirstOrDefaultAsync();
        }
        public async Task<Distribution?> GetDistributionOnlyIdAsync(int distributionId, bool includeCollections = false)
        {
            if (includeCollections)
            {
                return await _context.Distributions.Include(o => o.Operations).Include(p => p.Products)
                     .Where(o => o.DistributionId == distributionId)
                    .FirstOrDefaultAsync();
            }


            return await _context.Distributions
                .Where(o => o.DistributionId == distributionId)
                .FirstOrDefaultAsync();
        }
        public async Task<Distribution?> GetDistributionForAreaByCodeAndDescriptionAsync(int areaId, string code, string description)
        {
            return await _context.Distributions
                .Where(o => o.AreaId == areaId && o.Code == code && o.Description == description)
                .FirstOrDefaultAsync();
        }
        // public async Task AddProductForDistributionAsync(int areaId, int distributionId, Product product)
        //{
        //usar metodo de include distribution un producs

        //var distribution = await GetDistributionForAreaAsync(areaId, distributionId, true);
        //if (distribution != null)
        //{
        //    if (distribution.Products != null)
        //    {
        //        distribution.Products.Add(product);
        //    }
        //    else {
        //        distribution.Products = new List<Product>();
        //        distribution.Products.Add(product);
        //    }

        //}
        // }

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
            //_context.Distributions.Remove(distribution);
            distribution.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region OperationOperations
        public async Task<IEnumerable<Operation>> GetOperationsForDistributionAsync(int distributionId)
        {
            return await _context.Operations
                .Where(o => o.DistributionId == distributionId && o.IsActive == true).ToListAsync();
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
            //_context.Operations.Remove(operation);
            operation.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region QuestionTypeOperations

        public async Task<IEnumerable<QuestionType>> GetQuestionTypesAsync()
        {
            return await _context.QuestionTypes.Where(u => u.IsActive == true).OrderBy(q => q.QuestionTypeId).ToListAsync();
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
                .Where(cq => cq.ChecklistCategoryId == categoryId && cq.IsActive == true)
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
            //_context.ChecklistQuestions.Remove(checklistQuestion);
            checklistQuestion.IsActive = false;
            _context.SaveChanges();
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
                            && c.QuestionID != checklistQuestionId && c.IsActive == true)
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
            return await _context.SupportDocumentTypes.Where(u => u.IsActive == true)
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
            //_context.SupportDocumentTypes.Remove(supportDocumentType);
            supportDocumentType.IsActive = false;
            _context.SaveChanges();
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
            if (collection)
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

        public async Task RemoveDistributionForProductAsync(int productId, int distributionID)
        {
            var product = await GetProductAsync(productId, true);
            if (product != null)
            {
                if (product.Distributions != null)
                {
                    //Remove product
                    product.Distributions.Remove(item: product.Distributions.ToList().Find(d => d.DistributionId == distributionID));
                }
            }
        }

        public async Task RemoveProductForDistributionAsync(int productId, int distributionID)
        {
            var product = await GetProductAsync(productId, true);
            if (product != null)
            {
                if (product.Distributions != null)
                {
                    //Remove product
                    product.Distributions.Remove(item: product.Distributions.ToList().Find(d => d.DistributionId == distributionID));
                }
            }
        }
        public async Task AddDistributionForProductAsync(int productId, Distribution distribution)
        {
            var product = await GetProductAsync(productId, true);
            Debug.WriteLine("GET product");

            if (product != null)
            {
                if (product.Distributions != null)
                {
                    product.Distributions.Add(distribution);

                }
                else
                {
                    product.Distributions = new List<Distribution>();
                    product.Distributions.Add(distribution);

                }


            }
        }

        public void DeleteProduct(Product product)
        {
            //_context.Products.Remove(product);
            product.IsActive = false;
            _context.SaveChanges();
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
                .Include(pr => pr.Product).Where(u => u.IsActive == true)
                 .OrderBy(c => c.AssyChardId).ToListAsync();
        }
        public async Task<AssyChart?> GetAssyChartAsync(int asssychartId)
        {
            return await _context.AssyCharts.Include(o => o.Operation)
                 .Where(p => p.AssyChardId == asssychartId).FirstOrDefaultAsync();
        }
        public async Task<AssyChart?> GetAssyChartForJobObservationAsync(int PlantId, int AreaId, int DistributionId, int OperationId)
        {
            return await _context.AssyCharts
            .Where(p => p.PlantId == PlantId && p.AreaId == AreaId && p.DistributionId == DistributionId && p.OperationId == OperationId).FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<AssyChart>> GetAssyChartByPlantAsync(int plantId)
        {
            return await _context.AssyCharts.Where(plant => plant.PlantId == plantId)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.Product).Where(u => u.IsActive == true)
                .OrderBy(c => c.AssyChardId).ToListAsync();
        }

        public async Task<IEnumerable<AssyChart>> GetAssyChartByAreaAsync(int plantId, int areaId)
        {
            return await _context.AssyCharts.Where(a => a.PlantId == plantId && a.AreaId == areaId)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.Product).Where(u => u.IsActive == true)
                .OrderBy(c => c.AssyChardId).ToListAsync();
        }

        public async Task<IEnumerable<AssyChart>> GetAssyChartByDistributionAsync(int plantId, int areaId, int distributionId)
        {
            return await _context.AssyCharts.Where(a => a.PlantId == plantId && a.AreaId == areaId && a.DistributionId == distributionId)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.Product).Where(u => u.IsActive == true)
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
            //_context.AssyCharts.Remove(assyChart);
            assyChart.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region HistoryJobObservation
        public async Task<JobObservationVersion?> GetHistoryJobObservationAsync(int HistoryJobObservationId)
        {
            return await _context.JobObservationHistory
                .Include(l => l.Lup)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(s => s.Supervisor)
                .Include(o => o.Operator)
                .Where(H => H.JobObservationVersionId == HistoryJobObservationId).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<JobObservationVersion>> GetAllHistoryJobObservationAsync(int jobObservationId)
        {
            return await _context.JobObservationHistory
                 .Include(a => a.Area)
                    .Include(p => p.Plant)
                    .Include(d => d.Distribution)
                    .Include(o => o.Operation)
                    .Include(l => l.Lup)
                    .Include(s => s.Supervisor)
                    .Include(o => o.Operator)
                    .Where(h => h.JobObservationId == jobObservationId && h.IsActive == true)
                 .OrderBy(c => c.JobObservationVersionId).ToListAsync();
        }


        public void AddHistoyJobObservationAsync(JobObservationVersion jobObservationHistory)
        {
            _context.JobObservationHistory.Add(jobObservationHistory);
        }

        public void DeleteHistoyJobObservationAsync(JobObservationVersion jobObservationHistory)
        {
            //_context.JobObservationHistory.Remove(jobObservationHistory);
            jobObservationHistory.IsActive = false;
            _context.SaveChanges();
        }

        public async Task<bool> DeleteHistoyFromJobObservationAsync(JobObservationVersion HistoryVersion, JobObservation jobObservation)
        {

            if (jobObservation != null)
            {
                if (jobObservation.History != null)
                {
                    jobObservation.History.Remove(HistoryVersion);
                }
                return !(jobObservation.History.Contains(HistoryVersion));
            }

            return false;
        }
        public async Task<bool> AddHistoyToJobObservationAsync(JobObservationVersion HistoryVersion, JobObservation jobObservation)
        {

            if (jobObservation != null)
            {
                if (jobObservation.History != null)
                {
                    jobObservation.History.Add(HistoryVersion);

                }
                else
                {
                    jobObservation.History = new List<JobObservationVersion>();
                    jobObservation.History.Add(HistoryVersion);
                }

                return jobObservation.History.Contains(HistoryVersion);
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region Users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(ss => ss.Subordinates)
                .Include(aa => aa.Areas)
                .Include(ILU => ILU.ILURegisers).Where(u => u.IsActive == true)
                 .OrderBy(c => c.UserId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUserByTypeAsync(int typeUser)
        {
            return await _context.Users
                .Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(ss => ss.Subordinates)
                .Include(aa => aa.Areas)
                .Include(ILU => ILU.ILURegisers)
                .Where(u => u.UserType == typeUser).Where(u => u.IsActive == true)
                 .OrderBy(c => c.UserId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllSubordinatesAsync(int superiorid)
        {
            return await _context.Users
                .Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(ss => ss.Subordinates)
                .Include(aa => aa.Areas)
                .Include(ILU => ILU.ILURegisers)
                .Where(u =>  u.SuperiorId == superiorid && u.IsActive==true)
                 .OrderBy(c => c.UserId).ToListAsync();
        }
        public async Task<IEnumerable<User>> GetAllUsersWhitPlantAreaAndGroupAsync()
        {
            return await _context.Users
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(g => g.Group)
                .Include(o => o.Distribution).Where(u => u.IsActive == true)
                 .OrderBy(c => c.UserId).ToListAsync();
        }

        public async Task<User?> GetUserAsync(int userId, bool collection = false)
        {
            if (collection)
            {
                return await _context.Users.Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(ss => ss.Subordinates)
                .Include(ILU => ILU.ILURegisers)
                .Include(aa => aa.Areas)
                .Where(p => p.UserId == userId).FirstOrDefaultAsync();
            }
            return await _context.Users.Where(p => p.UserId == userId).FirstOrDefaultAsync();
        }
        public async Task<User?> GetUserByObjectIdAsync(string objectId)
        {
            return await _context.Users.Include(a => a.Area)
           .Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(ss => ss.Subordinates)
                .Include(ILU => ILU.ILURegisers)
                .Include(aa => aa.Areas)
            .Where(p => p.ObjectId!.ToLower() == objectId.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Include(a => a.Area)
           .Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(ss => ss.Subordinates)
                .Include(ILU => ILU.ILURegisers)
                .Include(aa => aa.Areas)
            .Where(p => p.Email == email).FirstOrDefaultAsync();
        }
        public async Task<User?> GetUserByPayrollAsync(int payroll)
        {
            return await _context.Users.Include(a => a.Area)
           .Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(ss => ss.Subordinates)
                .Include(aa => aa.Areas)
                .Include(ILU => ILU.ILURegisers)
            .Where(p => p.Payroll == payroll).FirstOrDefaultAsync();
        }


        public async Task<User?> GetUserByPayrollAndMoreAsync(int payroll, int plantid, int areaid, int groupid)
        {
            return await _context.Users.Where(p => p.Payroll == payroll && p.PlantId == plantid && p.AreaId == areaid && p.GroupId == groupid).FirstOrDefaultAsync();
        }


        public async Task<bool> UserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(p => p.UserId == userId);
        }
        public async Task<bool> UserExistByPayrollAsync(int payroll)
        {
            return await _context.Users.AnyAsync(p => p.Payroll == payroll);
        }
        public async Task<bool> UserExistByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(p => p.Email == email);
        }

        public async Task<bool> UserExistAdvanceAsync(string nombre, int nomina, int plantid, int areaid, int grupoid)
        {
            return await _context.Users.AnyAsync(p => p.Name == nombre && p.Payroll == nomina && p.PlantId == plantid && p.AreaId == areaid && p.GroupId == grupoid);
        }

        public async Task UpdateUser(UsersForUpdateDto user, int userId)
        {
            var entityUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            _mapper.Map(user, entityUser);

            _context.SaveChanges();
        }
        public async void UserAddSubordinated(User Master, User Slave)
        {

            if (Master.Subordinates != null)
            {
                Slave.SuperiorId = Master.UserId;
                Master.Subordinates.Add(Slave);
            }
            else
            {
                Master.Subordinates = new List<User>();
                Slave.SuperiorId = Master.UserId;
                Master.Subordinates.Add(Slave);
            }
            _context.SaveChanges();

        }

        public async void UserRemoveSubordinated(User Master, User Slave)
        {
            Master.Subordinates?.Remove(Slave);
            _context.SaveChanges();
        }
        public async Task<AsyncVoidMethodBuilder> UserRemoveAllSubordinated(User Master)
        {

            var UsersList = await _context.Users.Where(u => u.SuperiorId == Master.UserId)
                 .OrderBy(c => c.UserId).ToListAsync();

            if (UsersList?.Count > 0)
            {
                foreach (User sub in UsersList)
                {
                    sub.SuperiorId = null;
                }

                Master.Subordinates?.Clear();
                _context.SaveChanges();
            }

            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> UserRemoveAllAreas(User Master)
        {
            Master.Areas?.Clear();
            // Eliminar todas las entradas relacionadas en la tabla UserAreas para el usuario especificado
            string sqlQuery = "DELETE FROM UserAreas WHERE UserId = @userId";

            int executeCount = _context.Database.ExecuteSqlRaw(sqlQuery,
                    new SqlParameter("@userId", Master.UserId));

            Debug.WriteLine($"Este es executeCount: {executeCount}");

            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();

        }

        public void UserAddArea(User Master, Area Slave)
        {
            if (Master.Areas != null)
            {
                Master.Areas.Add(Slave);
            }
            else
            {
                Master.Areas = new List<Area>();
                Master.Areas.Add(Slave);
            }
            _context.SaveChanges();
        }


        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public void DeleteUserAsync(User user)
        {
            //_context.Users.Remove(user);
            user.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region File
        public void AddUploadFile(FileUpload fileUplaod)
        {
            _context.Files.Add(fileUplaod);
        }

        public async Task<FileUpload?> GetFileUploadAsync(int fileid)
        {

            return await _context.Files
                .Where(p => p.FileUploadId == fileid).FirstOrDefaultAsync();
        }

        public void DeleteUploadFile(FileUpload fileUplaod)
        {
            _context.Files.Remove(fileUplaod);
            //fileUplaod.IsActive = false;
            //_context.SaveChanges();
        }
        #endregion
        #region Guide

        public async Task<Guides?> GetGuideAsync(int guideId, bool includeFile = false)
        {
            if (includeFile)
            {
                return await _context.Guides.Include(p => p.FileUpload)
                    .Where(p => p.GuideId == guideId).FirstOrDefaultAsync();
            }

            return await _context.Guides
                .Where(p => p.GuideId == guideId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Guides>> GetAllGuides(bool includeFile = false)
        {
            if (includeFile)
            {
                return await _context.Guides.Include(p => p.FileUpload).Where(u => u.IsActive == true).OrderBy(g => g.GuideId).ToListAsync();
            }

            return await _context.Guides.OrderBy(g => g.GuideId).Where(u => u.IsActive == true).ToListAsync();
        }

        public void AddGuide(Guides guide)
        {
            _context.Guides.Add(guide);
        }

        public void DeleteGuide(Guides guide)
        {
            //_context.Guides.Remove(guide);
            guide.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region JobObservationOperations

        public async Task<IEnumerable<JobObservation>> GetAllJobObservationsAsync(bool includeLup)
        {

            if (includeLup)
            {
                return await _context.JobObservations
                    .Include(a => a.Area)
                    .Include(p => p.Plant)
                    .Include(d => d.Distribution)
                    .Include(o => o.Operation)
                    .Include(l => l.Lup)
                    .Include(s => s.Supervisor)
                    .Include(o => o.Operator).Where(u => u.IsActive == true)
                     .OrderBy(c => c.JobObservationId).ToListAsync();
            }

            return await _context.JobObservations
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(s => s.Supervisor)
                .Include(o => o.Operator).Where(u => u.IsActive == true)
                 .OrderBy(c => c.JobObservationId).ToListAsync();

        }



        public async Task<JobObservation?> GetJobObservationAsync(int jobObservationId, bool includeLup)
        {
            if (includeLup)
            {
                return await _context.JobObservations
                    .Include(l => l.Lup)
                    .Include(s => s.Supervisor)
                    .Include(o => o.Operator)
                    .Include(h => h.History)
                     .Where(p => p.JobObservationId == jobObservationId).FirstOrDefaultAsync();
            }
            //return whit info
            return await _context.JobObservations
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(s => s.Supervisor)
                .Include(o => o.Operator)
                 .Where(p => p.JobObservationId == jobObservationId).FirstOrDefaultAsync();
        }

        public void AddJobObservation(JobObservation jobObservation)
        {
            _context.JobObservations.Add(jobObservation);
        }

        public void DeleteJobObservation(JobObservation jobObservation)
        {
            //_context.JobObservations.Remove(jobObservation);
            jobObservation.IsActive = false;
            _context.SaveChanges();
        }
        public async Task<bool> JobObservationExistAsync(int jobObservationId)
        {
            return await _context.JobObservations.AnyAsync(j => j.JobObservationId == jobObservationId);
        }

        #endregion
        #region GlosaryOperations

        public async Task<IEnumerable<Glosary>> GetGlosaryAsync()
        {
            return await _context.Glosary.Where(u => u.IsActive == true)
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
            //_context.Glosary.Remove(glosaryWord);
            glosaryWord.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region LupOperations
        public async Task<Lup?> GetLupAsync(int lupId, bool includeFile = false)
        {
            if (includeFile)
            {
                return await _context.Lup.Include(l => l.Evidences)
                    .Where(e => e.LupId == lupId).FirstOrDefaultAsync();
            }
            return await _context.Lup
                 .Where(x => x.LupId == lupId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Lup>> GetAllLupAsync()
        {
            return await _context.Lup.Where(u => u.IsActive == true)
                 .OrderBy(c => c.LupId).ToListAsync();

        }


        public void AddLup(Lup lup)
        {
            _context.Lup.Add(lup);
        }

        public void DeleteLup(Lup lup)
        {
            //_context.Lup.Remove(lup);
            lup.IsActive = false;
            _context.SaveChanges();
        }

        public async Task<bool> LupExistAsync(int lupId)
        {
            return await _context.Lup.AnyAsync(l => l.LupId == lupId);
        }

        public async Task AddEvidenceForLupAsync(int lupId, FileUpload evidence)
        {
            var lup = await GetLupAsync(lupId, true);

            if (lup != null)
            {

                if (lup.Evidences != null)
                {
                    lup.Evidences.Add(evidence);
                }
                else
                {
                    lup.Evidences = new List<FileUpload>
                    {
                        evidence
                    };

                }


            }

        }
        public async Task RemoveEvidenceForLupAsync(int lupId, int fileUploadId)
        {
            var lup = await GetLupAsync(lupId, true);
            if (lup != null)
            {
                if (lup.Evidences != null)
                {
                    //Remove evidence
                    lup.Evidences.Remove(item: lup.Evidences.ToList().Find(e => e.FileUploadId == fileUploadId));
                }
            }
        }
        #endregion
        #region Notification
        public async Task<Notification?> GetNotificationAsync(int notifyID)
        {
            return await _context.Notifications.Include(n => n.User).Where(n => n.NotificationID == notifyID).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.Include(n => n.User)
                .Where(n => n.IsActive == true)
                 .OrderBy(c => c.NotificationID).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsFromUserAsync(int id)
        {
            return await _context.Notifications.Include(n => n.User)
                .Where(n => n.UserId == id && EF.Functions.DateDiffDay(DateTime.Now, n.EntryDate) <= 3 && EF.Functions.DateDiffMonth(DateTime.Now, n.EntryDate) == 0)
                 .OrderBy(c => c.NotificationID).ToListAsync();
        }



        public void AddNotificationAsync(Notification notify)
        {
            _context.Notifications.Add(notify);
        }

        public void DeleteNotificationAsync(Notification notify)
        {
            notify.IsActive = false;
            _context.SaveChanges();
            //_context.Notifications.Remove(notify);
        }
        #endregion
        #region Attendance
        public async Task<Attendance> GetAttendanceById(int AttendanceId)
        {
            return await _context.Attendances
                .Include(a => a.User)
                .Include(g => g.Superior)
                .Include(c => c.currentdistribution)
                  .Where(p => p.AttendanceId == AttendanceId).FirstOrDefaultAsync();
        }
        public void AddAttendance(Attendance Attendance)
        {
            _context.Attendances.Add(Attendance);
        }
        public async Task<IEnumerable<Attendance>> GetAllAttendance()
        {
            return await _context.Attendances
                .Include(a => a.User)
                .Include(g => g.Superior)
                .Include(c => c.currentdistribution)
               .OrderBy(c => c.AttendanceId).ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendanceOfSupervisor(int idsuperior)
        {
            return await _context.Attendances
                .Include(a => a.User)
                .Include(g => g.Superior)
                .Include(c => c.currentdistribution)
                .Include(s => s.Superior.Plant)
                .Include(s => s.Superior.Area)
                .Include(s => s.Superior.Distribution)
                .Include(s => s.Superior.Group)
                .Include(s => s.User.Plant)
                .Include(s => s.User.Area)
                .Include(s => s.User.Distribution)
                .Include(s => s.User.Group)
                .Where(o => o.SuperiorId == idsuperior)
               .OrderBy(c => c.AttendanceId).ToListAsync();
        }

        #endregion

        #region ILU
        public async Task<ILULevel?> GetILULevel(int idILU)
        {
            return await _context.ILULevels
            .Where(p => p.ILULevelId == idILU).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ILULevel>> GetAllILULevel()
        {
            return await _context.ILULevels.Where(u => u.isActive == true)
                    .OrderBy(c => c.ILULevelId).ToListAsync();
        }
        public async Task<int> AddILU(ILULevel lU)
        {
            _context.ILULevels.Add(lU);

            return _context.SaveChanges();
        }
        public async Task<int> UpdateILU(ILULevel iluforUpdate, ILULevel iluEntity)
        {

            _mapper.Map(iluforUpdate, iluEntity);

            return _context.SaveChanges();

        }
        public async Task RemoveILU(ILULevel lU)
        {
            var ilu = await _context.ILULevels.Where(i => i.ILULevelId == lU.ILULevelId).FirstOrDefaultAsync();
            ilu.isActive = false;
            _context.SaveChanges();
        }

        #endregion
        #region ILURegister
        public async Task<ILURegister?> GetILURegister(int idILUR)
        {
            return await _context.ILURegisters
               .Where(p => p.ILURegisterid == idILUR).FirstOrDefaultAsync();
        }
        public async Task<int> AddILURegister(ILURegister iLURegister)
        {
            _context.ILURegisters.Add(iLURegister);

            return _context.SaveChanges();
        }
        public async Task<int> AddILURegToUser(ILURegister iLURegister, User Master)
        {
            Master.ILURegisers?.Add(iLURegister);

            return _context.SaveChanges();
        }
        public async Task<int> UpdateILURegister(ILURegister iluRforUpdate, ILURegister iluREntity)
        {
            _mapper.Map(iluRforUpdate, iluREntity);

            return _context.SaveChanges();
        }
        public async Task<int> RemoveILURegister(ILURegister ILUReg)
        {
            var entity = await _context.ILURegisters.Where(u => u.ILURegisterid == ILUReg.ILURegisterid).FirstOrDefaultAsync();

            entity.isActive = false;

            return _context.SaveChanges();
        }

        #endregion
        #region PAT
        public async Task<int> AddPat(PAT patForAdd)
        {
            _context.PATs.Add(patForAdd);
            return _context.SaveChanges();
        }

        public async Task<PAT?> GetPat(int patId)
        {
            return await _context.PATs
                   .Include(p => p.Plant)
                   .Include(a => a.Area)
                   .Include(d => d.Distribution)
                   .Include(sv => sv.Supervisor)
                   .Include(ssv => ssv.SSVresponsible)
                   .Where(p => p.PATid == patId).FirstOrDefaultAsync();
        }
        public async Task<PAT?> GetPatForYearOfSV(int sv, int Year)
        {
            return await _context.PATs.Where(p => p.SupervisorId == sv && p.AplicationYear == Year).FirstOrDefaultAsync();
        }
        public async Task<int> UpdatePAT(PATForUpdateDto patForUpdate, PAT PatEntity)
        {

            _mapper.Map(patForUpdate, PatEntity);

            return _context.SaveChanges();
        }
        public async Task<IEnumerable<PAT>> GetAllPATs()
        {
            return await _context.PATs
                   .Include(p => p.Plant)
                   .Include(a => a.Area)
                   .Include(d => d.Distribution)
                   .Include(sv => sv.Supervisor)
                   .Include(ssv => ssv.SSVresponsible).Where(u => u.IsActive == true)
                    .OrderBy(c => c.PATid).ToListAsync();
        }
        public async Task<IEnumerable<PAT>> GetAllPATsOfSv(int svId)
        {
            return await _context.PATs
                    .Include(p => p.Plant)
                    .Include(a => a.Area)
                    .Include(d => d.Distribution)
                    .Include(sv => sv.Supervisor)
                    .Include(ssv => ssv.SSVresponsible)
                    .Where(p => p.SupervisorId == svId && p.IsActive ==true)
                    .OrderBy(c => c.PATid).ToListAsync();
        }
        public async Task<IEnumerable<PAT>> GetAllPATsofSSV(int ssvID)
        {
            return await _context.PATs
                           .Include(p => p.Plant)
                   .Include(a => a.Area)
                   .Include(d => d.Distribution)
                   .Include(sv => sv.Supervisor)
                   .Include(ssv => ssv.SSVresponsible)
                           .Where(p => p.SSVresponsibleID == ssvID && p.IsActive ==true )
                            .OrderBy(c => c.PATid).ToListAsync();
        }
        #endregion

        #region UserNotFound
        public async Task<IEnumerable<UserNotFound>> GetAllUsersNotFoundAsync()
        {
            return await _context.UsersNotFound.Where(u => u.IsActive == true).ToListAsync();
        }
        public async Task<UserNotFound?> GetUserNotFoundAsync(int userNotFoundId)
        {
            return await _context.UsersNotFound.Where(u => u.UserNotFoundId == userNotFoundId).FirstOrDefaultAsync();
        }

        public async Task UpdateUserNotFound(UserNotFoundForUpdateDto userNotFound, int userNotFoundId)
        {
            var entityUserNotFound = await _context.UsersNotFound.FirstOrDefaultAsync(u => u.UserNotFoundId == userNotFoundId);

            _mapper.Map(userNotFound, entityUserNotFound);

            _context.SaveChanges();
        }

        public async Task AddUserNotFoundAsync(UserNotFound userNotFound)
        {
            _context.UsersNotFound.Add(userNotFound);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region SOS_Reviews

        public async Task<IEnumerable<SOSReviewProgram>> GetAllSOSReviews()
        {
            return await _context.SOSReviews
                   .Include(p => p.Plant)
                   .Include(a => a.Area)
                   .Include(UA => UA.UserA)
                   .Include(UB => UB.UserA)
                   .Include(UC => UC.UserA)
                   .Where(u => u.IsActive == true)
                    .OrderBy(c => c.SOSid).ToListAsync();

        }

       public async Task<SOSReviewProgram?> GetSOSasync(int sosId)
        {
            return await _context.SOSReviews
                   .Include(p => p.Plant)
                   .Include(a => a.Area)
                   .Include(UA => UA.UserA)
                   .Include(UB => UB.UserA)
                   .Include(UC => UC.UserA)
                   .Where(p => p.SOSid == sosId).FirstOrDefaultAsync();
        }
        public async Task<int> AddSOSReview(SOSReviewProgram SOSEntity)
        {
            _context.SOSReviews.Add(SOSEntity);
            return _context.SaveChanges();
        }
        public async Task<int> DeleteSOSReview(SOSReviewProgram SOSEntity)
        {
            SOSEntity.IsActive = false;
            return _context.SaveChanges();
        }
        public async Task<int> UpdateSOSReview(SOSReviewForUpdateDto SOSForUpdate, SOSReviewProgram SOSEntity)
        {
            _mapper.Map(SOSForUpdate, SOSEntity);

            return _context.SaveChanges();
        }
        #endregion

        #region CommonOperations
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
        #endregion 
    }
}
