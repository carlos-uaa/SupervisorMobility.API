
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.DataAccess.Services.OrderingServices;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.HCIDtos;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.JobPaginationDtos;
using SupervisorMobility.API.Models.KaizenDtos;
using SupervisorMobility.API.Models.KaizenTransactionDtos;
using SupervisorMobility.API.Models.PATDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using SupervisorMobility.API.Models.Users;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.API.Services
{
    public class SupervisorMobilityRepository : ISupervisorMobilityRepository
    {
        private readonly SupervisorMobilityContext _context;
        private readonly IMapper _mapper;
        private readonly IOrderingService orderingService;


        public SupervisorMobilityRepository(SupervisorMobilityContext context, IMapper mapper, IOrderingService orderingService)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.orderingService = orderingService;
        }

        #region JobCategoryStructureOperations

        public void AddChecklistCategory(JobCategoryStructure checklistCategory)
        {
            _context.JobCategoryStructures.Add(checklistCategory);
        }

        public async Task<bool> ChecklistCategoryExistAsync(int checklistCategoryId)
        {
            return await _context.JobCategoryStructures.AnyAsync(c => c.JobCategoryStructureId == checklistCategoryId);
        }

        public void DeleteChecklistCategory(JobCategoryStructure checklistCategory)
        {
            checklistCategory.IsActive = false;
            _context.SaveChanges();
            //_context.JobCategoryStructures.Remove(checklistCategory);
        }

        public async Task<IEnumerable<JobCategoryStructure>> GetChecklistCategoriesAsync(bool includeChecklistQuestion = false)
        {
            if (includeChecklistQuestion)
            {
                return await _context.JobCategoryStructures.Include(cq => cq.ChecklistQuestions.Where(cq => cq.IsActive == true).OrderBy(c => c.CategorySequence)).ThenInclude(p => p.Pillars)
                    .Where(u => u.IsActive == true).OrderBy(c => c.Sequence).ToListAsync();
            }
            return await _context.JobCategoryStructures.Where(u => u.IsActive == true)
                .OrderBy(c => c.Sequence).ToListAsync();
        }
        public async Task<IEnumerable<JobCategoryStructure>> GetAllChecklistCategoriesAsync(bool includeChecklistQuestion = false)
        {
            if (includeChecklistQuestion)
            {
                return await _context.JobCategoryStructures.Include(cq => cq.ChecklistQuestions.Where(cq => cq.IsActive == true).OrderBy(c => c.CategorySequence)).ThenInclude(p => p.Pillars)
                    .OrderBy(c => c.Sequence).ToListAsync();
            }
            return await _context.JobCategoryStructures
                .OrderBy(c => c.Sequence).ToListAsync();
        }

        public async Task<JobCategoryStructure?> GetChecklistCategoryAsync(int categoryId, bool includeChecklistQuestion = false)
        {
            if (includeChecklistQuestion)
            {
                return await _context.JobCategoryStructures.Include(cq => cq.ChecklistQuestions.Where(cq => cq.IsActive == true).OrderBy(c => c.CategorySequence)).ThenInclude(p => p.Pillars)
                    .Where(c => c.JobCategoryStructureId == categoryId).FirstOrDefaultAsync();
            }

            return await _context.JobCategoryStructures
                .Where(c => c.JobCategoryStructureId == categoryId).FirstOrDefaultAsync();
        }

        public async Task<int> GetChecklistCategoriesMaxSequenceAsync()
        {
            return await _context.JobCategoryStructures.MaxAsync(cc => cc.Sequence) + 1;
        }

        public async Task<IEnumerable<JobCategoryStructure>> GetChecklistCategoriesForUpdateSequenceAsync(
            int currentSequence, int oldSequence, int categoryId)
        {
            int lowerValue = currentSequence < oldSequence ? currentSequence : oldSequence;
            int upperValue = currentSequence > oldSequence ? currentSequence : oldSequence;

            return await _context.JobCategoryStructures
                        .Where(c => c.Sequence >= lowerValue
                            && c.Sequence <= upperValue
                            && c.JobCategoryStructureId != categoryId
                            && c.IsActive == true)
                        .OrderBy(c => c.Sequence).ToListAsync();
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
        public async Task<Plant?> GetPlantOnlyIdAsync(int plantid)
        {
            return await _context.Plants
                           .Where(a => a.PlantId == plantid)
                           .FirstOrDefaultAsync();
        }
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

        public async Task<Area?> GetAreaOnlyIdAsync(int areaId)
        {
            return await _context.Areas
                           .Where(a => a.AreaId == areaId)
                           .FirstOrDefaultAsync();
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

        public async Task<IEnumerable<Distribution>> GetAllDistributions()
        {

            return await _context.Distributions.Include(p => p.Products)
                 .Where(o => o.IsActive == true)
                .ToListAsync();
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

        public async Task<AsyncVoidMethodBuilder> RemoveAllOperations()
        {
            _context.Operations.RemoveRange(_context.Operations);
            await _context.SaveChangesAsync();

            return new AsyncVoidMethodBuilder();
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
            return await _context.ChecklistQuestions.Include(p => p.Pillars)
                .Where(cq => cq.JobCategoryStructureId == categoryId && cq.IsActive == true)
                .OrderBy(cq => cq.CategorySequence).ToListAsync();
        }
        public async Task<ChecklistQuestion?> GetChecklistQuestionForCategoryAsync(int categoryId,
            int questionId)
        {
            return await _context.ChecklistQuestions.Include(p => p.Pillars)
                .Where(cq => cq.JobCategoryStructureId == categoryId && cq.QuestionID == questionId)
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
                .Where(cq => cq.JobCategoryStructureId == categoryId)
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

            return await _context.ChecklistQuestions.Include(p => p.Pillars)
                        .Where(c => c.JobCategoryStructureId == categoryId
                            && c.CategorySequence >= lowerValue
                            && c.CategorySequence <= upperValue
                            && c.QuestionID != checklistQuestionId && c.IsActive == true)
                        .OrderBy(c => c.CategorySequence).ToListAsync();
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
            return await _context.Products.Where(p => p.IsActive == true)
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
        public async Task<Product?> GetProductByCodeAsync(string code)
        {
            return await _context.Products
                .Where(p => p.Code == code).FirstOrDefaultAsync();
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
                .Include(pr => pr.RoutesProductsAssyChart)
                .ThenInclude(r => r.Product)
                .Where(u => u.IsActive == true)
                 .OrderBy(c => c.AssyChardId).ToListAsync();
        }
        public async Task<AssyChart?> GetAssyChartAsync(int asssychartId)
        {
            return await _context.AssyCharts
                .Include(o => o.Plant)
                .Include(o => o.Area)
                .Include(o => o.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.RoutesProductsAssyChart)
                .ThenInclude(r => r.Product)
                 .Where(p => p.AssyChardId == asssychartId).FirstOrDefaultAsync();
        }


        public async Task<AssyChart?> GetAssyChartForJobObservationAsync(int PlantId, int AreaId, int DistributionId)
        {
            return await _context.AssyCharts.Include(pr => pr.RoutesProductsAssyChart)
                .ThenInclude(r => r.Product)
            .Where(p => p.PlantId == PlantId && p.AreaId == AreaId && p.DistributionId == DistributionId).FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<AssyChart>> GetAllAssyChartsByPlantAsync(int plantId)
        {
            return await _context.AssyCharts.Where(plant => plant.PlantId == plantId)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.RoutesProductsAssyChart)
                .ThenInclude(r => r.Product).Where(u => u.IsActive == true)
                .OrderBy(c => c.AssyChardId).ToListAsync();
        }

        public async Task<IEnumerable<AssyChart>> GetAllAssyChartsByAreaAsync(int plantId, int areaId)
        {
            return await _context.AssyCharts.Where(a => a.PlantId == plantId && a.AreaId == areaId)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.RoutesProductsAssyChart)
                .ThenInclude(r => r.Product).Where(u => u.IsActive == true)
                .OrderBy(c => c.AssyChardId).ToListAsync();
        }

        public async Task<IEnumerable<AssyChart>> GetAllAssyChartsByDistributionAsync(int plantId, int areaId, int distributionId)
        {
            return await _context.AssyCharts.Where(a => a.PlantId == plantId && a.AreaId == areaId && a.DistributionId == distributionId)
                .Include(a => a.Area)
                .Include(p => p.Plant)
                .Include(d => d.Distribution)
                .Include(o => o.Operation)
                .Include(pr => pr.RoutesProductsAssyChart).Where(u => u.IsActive == true)
                .OrderBy(c => c.AssyChardId).ToListAsync();
        }




        public async Task<AssyChart?> GetAssyChartAdvanceAsync(int PlantId, int AreaId, int DistributionId, int OperationId)
        {
            //return whit info
            return await _context.AssyCharts.Where(p => p.PlantId == PlantId && p.AreaId == AreaId && p.DistributionId == DistributionId && p.OperationId == OperationId).FirstOrDefaultAsync();
        }


        public async Task<AssyChart?> GetAssyChartAdvanceByOperationAndProductAsync(int plantId, int areaId, int distributionId, int operationId, int ProductId)
        {
            return await _context.AssyCharts.Where(p => p.PlantId == plantId && p.AreaId == areaId && p.DistributionId == distributionId && p.OperationId == operationId).Include(a => a.RoutesProductsAssyChart).ThenInclude(p => p.ProductId == ProductId).FirstOrDefaultAsync();

        }
        public async Task<AssyChart?> GetAssyChartAdvanceByProductAsync(int plantId, int areaId, int distributionId, int ProductId)
        {
            return await _context.AssyCharts.Where(p => p.PlantId == plantId && p.AreaId == areaId && p.DistributionId == distributionId)
                .Include(a => a.RoutesProductsAssyChart).ThenInclude(r => r.ProductId == ProductId)
                .FirstOrDefaultAsync();
        }


        public async Task<bool> AssyChartExistAsync(int assychartID)
        {
            return await _context.AssyCharts.AnyAsync(p => p.AssyChardId == assychartID);
        }
        public async Task<bool> AssyChartExistAdvanceAsync(int PlantId, int AreaId, int DistributionId, int OperationId)
        {
            return await _context.AssyCharts.AnyAsync(p => p.PlantId == PlantId && p.AreaId == AreaId && p.DistributionId == DistributionId && p.OperationId == OperationId);
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
        public async Task<IEnumerable<User>> GetAllUsersAsync(bool includeCollections = false, bool includeSubordinates = false, bool includeLeadershipRecord = false)
        {
            var query = _context.Users.Where(u => u.IsActive == true);

            if (includeCollections)
            {
                query = query
                    .Include(p => p.Plant)
                    .Include(a => a.Area)
                    .Include(d => d.Distribution)
                    .Include(g => g.Group)
                    .Include(s => s.Superior)
                    .Include(aa => aa.Areas)
                    .Include(ILU => ILU.ILURegisers);
            }

            if (includeSubordinates)
            {
                query = query.Include(s => s.Subordinates);
            }

            if (includeLeadershipRecord)
            {
                query = query.Include(s => s.LeadershipRecords);
            }

            return await query.OrderBy(c => c.UserId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUserByTypeAsync(int typeUser, bool includeCollections = false, bool includeSubordinates = false, bool includeLeadershipRecord = false)
        {
            var query = _context.Users.Where(u => u.UserType == typeUser).Where(u => u.IsActive == true);

            if (includeCollections)
            {
                query = query.Include(p => p.Plant)
                .Include(a => a.Area)
                .Include(d => d.Distribution)
                .Include(g => g.Group)
                .Include(s => s.Superior)
                .Include(aa => aa.Areas)
                .Include(ILU => ILU.ILURegisers);
            }

            if (includeSubordinates)
            {
                query = query.Include(ss => ss.Subordinates);
            }

            if (includeLeadershipRecord)
            {
                query = query.Include(ss => ss.LeadershipRecords);
            }

            return await query.OrderBy(c => c.UserId).ToListAsync();

        }

        public async Task<IEnumerable<User>> GetAllUserByTypeInPlantAreaAsync(int plantId, int areaId, int typeUser, bool includeCollections = false, bool includeSubordinates = false, bool includeLeadershipRecord = false)
        {
            var query = _context.Users.Where(u => u.IsActive == true && u.UserType == typeUser && u.PlantId == plantId && u.AreaId == areaId);

            if (includeCollections)
            {
                query = query.Include(p => p.Plant)
                    .Include(a => a.Area)
                    .Include(d => d.Distribution)
                    .Include(g => g.Group)
                    .Include(s => s.Superior)
                    .Include(aa => aa.Areas)
                    .Include(ILU => ILU.ILURegisers);
            }
            if (includeSubordinates)
            {
                query = query.Include(ss => ss.Subordinates);
            }
            if (includeLeadershipRecord)
            {
                query = query.Include(ss => ss.LeadershipRecords);
            }
            return await query.OrderBy(c => c.UserId).ToListAsync();

        }

        public async Task<IEnumerable<User>> GetAllUserByTypeInPlantAsync(int plantId, int typeUser, bool includeCollections = false, bool includeSubordinates = false, bool includeLeadershipRecord = false)
        {
            var query = _context.Users.Where(u => u.UserType == typeUser && u.PlantId == plantId && u.IsActive == true);

            if (includeCollections)
            {
                query = query.Include(p => p.Plant)
                    .Include(a => a.Area)
                    .Include(d => d.Distribution)
                    .Include(g => g.Group)
                    .Include(s => s.Superior)
                    .Include(aa => aa.Areas)
                    .Include(ILU => ILU.ILURegisers);
            }
            if (includeSubordinates)
            {
                query = query
                    .Include(ss => ss.Subordinates);
            } if (includeLeadershipRecord)
            {
                query = query.Include(u => u.LeadershipRecords);
            }

            return await query.OrderBy(c => c.UserId).ToListAsync();


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
                .Where(u => u.SuperiorId == superiorid && u.IsActive == true)
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
                .Include(s => s.Superior.Areas)
                .Include(lr => lr.LeadershipRecords)
                .Include(ss => ss.Subordinates)
                    .ThenInclude(sub => sub.Area)
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
                .Include(lr => lr.LeadershipRecords)
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
                .Include(lr => lr.LeadershipRecords)
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
                .Include(lr => lr.LeadershipRecords)
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
        public async Task<bool> UserExistByObjectIdAsync(string ObjectId)
        {
            return await _context.Users.AnyAsync(p => p.ObjectId == ObjectId);
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
        public async Task<AsyncVoidMethodBuilder> UserAddSubordinated(User Master, User Slave)
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
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> UserRemoveSubordinated(User Master, User Slave)
        {
            Master.Subordinates?.Remove(Slave);
            _context.SaveChanges();
            return new AsyncVoidMethodBuilder();
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

        public async Task RemoveAllAreasFromUser(User user)
        {
            var userWithAreas = await _context.Users.Include(u => u.Areas).FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (userWithAreas != null)
            {
                userWithAreas.Areas?.Clear();

                _context.SaveChanges();
            }
        }
        public async Task<AsyncVoidMethodBuilder> UserUpdateAllSubordinated(User Master)
        {

            var UsersList = await _context.Users.Where(u => u.SuperiorId == Master.UserId)
                 .OrderBy(c => c.UserId).ToListAsync();

            if (UsersList?.Count > 0)
            {
                foreach (User sub in UsersList)
                {
                    switch (sub.UserType)
                    {
                        case 2:
                            sub.PlantId = Master.PlantId;
                            await UserUpdateAllSubordinated(sub);
                            break;
                        case 3:
                            sub.PlantId = Master.PlantId;
                            sub.GroupId = Master.GroupId;
                            await UserUpdateAllSubordinated(sub);
                            break;
                        case 4:
                            sub.PlantId = Master.PlantId;
                            sub.AreaId = Master.AreaId;
                            sub.GroupId = Master.GroupId;
                            break;
                    }
                }

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

        public async Task<AsyncVoidMethodBuilder> UserAddArea(User Master, Area Slave)
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

            return new AsyncVoidMethodBuilder();

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
        #region RouteAssychart
        public async Task<SOSCodePath?> GetCodePathItemAsync(int RouteId)
        {
            return await _context.CodePaths
                .Include(p => p.Product)
                .Include(p => p.Distribution)
                .Include(p => p.AssyChart)
                .Include(a => a.AssyChart.Plant)
                .Include(a => a.AssyChart.Area)
                .Include(a => a.AssyChart.Distribution)
                .Where(p => p.SOSCodePathId == RouteId).FirstOrDefaultAsync();
        }

        public async Task<SOSCodePath?> TryFindCodePathItemAsync(int assychartId, string code)
        {
            return await _context.CodePaths
                .Where(p => p.AssyChardId == assychartId && p.Code == code).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SOSCodePath>> GetAllCodePathsAsync()
        {
            return await _context.CodePaths
                .Include(p => p.Distribution)
                 .Include(cp => cp.AssyChart.Plant)
                 .Include(cp => cp.AssyChart.Area)
                 .Include(a => a.Product)
                .Where(u => u.IsActive == true)
                 .OrderBy(c => c.SOSCodePathId).ToListAsync();
        }

        public async Task AssyChartRemoveAllCodePaths(AssyChart AssyChart)
        {
            var AssychartEntity = await _context.AssyCharts.Include(u => u.RoutesProductsAssyChart).FirstOrDefaultAsync(u => u.AssyChardId == AssyChart.AssyChardId);

            if (AssychartEntity != null)
            {
                AssychartEntity.RoutesProductsAssyChart?.Clear();

                await _context.SaveChangesAsync();
            }
        }
        public async Task AssychartCreateCodePath(SOSCodePath RouteAssychart)
        {
            _context.CodePaths.Add(RouteAssychart);
            await _context.SaveChangesAsync();
        }

        public void AssychartAddCodePath(AssyChart Master, SOSCodePath Slave)
        {
            if (Master.RoutesProductsAssyChart != null)
            {
                Master.RoutesProductsAssyChart.Add(Slave);
            }
            else
            {
                Master.RoutesProductsAssyChart = new List<SOSCodePath>();
                Master.RoutesProductsAssyChart.Add(Slave);
            }
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

        public async Task<JOPaginationDto> GetJobObservationsByFiltersAsync(DateTime startDate, DateTime endDate, int jobObsId, int plantId, int areaId, int distributionId, int operationId, int supervisorId, int status, int userId, int typeId, string searchString, int page = 1, int entries = 10, int? sortO = 2, string? sortL = "")
        {
            Expression<Func<JobObservation, object>>? keySelectorExp = orderingService.BuildJOKeySelector<JobObservation>(sortL);

            var query = _context.JobObservations
                //.Include(a => a.Area)
                //.Include(p => p.Plant)
                //.Include(d => d.Distribution)
                //.Include(l => l.Lup.Where(lup => lup.IsActive == true))
                //.Include(o => o.Operation)
                //.Include(s => s.Supervisor)
                //.Include(o => o.Operator).Where(u => u.IsActive == true)
                .Where(u => u.IsActive == true);
            IQueryable < Distribution >? dtquery = null;
            IQueryable < Operation >? opquery = null;
            IQueryable < User >? oquery = null;

            query = query.Include(a => a.Area)
                             .Include(p => p.Plant)
                             .Include(d => d.Distribution)
                             .Include(o => o.Operation);
            query = query.Include(s => s.Supervisor)
                             .Include(o => o.Operator);

            if (userId != default)
            {
                var user = await _context.Users.Include(u => u.Subordinates).Where(p => p.UserId == userId).FirstOrDefaultAsync();
                switch (user.UserType)
                {
                    case 2:
                        query = query.Where(j => user.Subordinates.Any() && user.Subordinates.Select(subordinate => subordinate.UserId).Contains((int)j.SupervisorId));
                        break;
                    case 3:
                        query = query.Where(j => j.SupervisorId == userId);
                        break;
                    case 5:
                        query = query.Where(j => user.Subordinates.Any() && user.Subordinates.Any(sub => sub.SuperiorId == j.Supervisor.SuperiorId));
                        break;
                }
            }

            if (plantId != default(int))
            {
                query = query.Where(j => j.PlantId == plantId);
            }
            if (areaId != default(int))
            {
                query = query.Where(j => j.AreaId == areaId);
                dtquery = _context.Distributions.Where(p=>p.AreaId == areaId && p.IsActive==true);
                oquery = _context.Users.Where(u => u.IsActive == true && u.UserType == 4 && u.PlantId == plantId && u.AreaId == areaId);
            }

            if (distributionId != default(int))
            {
                query = query.Where(j => j.DistributionId == distributionId);
                opquery = _context.Operations.Where(p=>p.DistributionId == distributionId && p.IsActive == true);
            }
            if (operationId != default(int))
            {
                query = query.Where(j => j.OperationId == operationId);
            }

            if (supervisorId != default(int))
            {
                query = query.Where(j => j.SupervisorId == supervisorId);
            }

            if (startDate != default(DateTime))
            {
                query = query.Where(j => j.StartDate.HasValue && j.StartDate.Value.Date >= startDate.Date || (j.StartDate.HasValue && j.StartDate.Value.Date <= startDate.Date && (j.EndDate.HasValue && j.EndDate.Value.Date >= startDate.Date)));
            }

            if (endDate != default(DateTime))
            {
                query = query.Where(j => j.StartDate.HasValue && j.StartDate.Value.Date <= endDate.Date);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.JobObservationId.ToString().ToLower().Contains(searchString.ToLower()) ||
                    p.Distribution.Description.ToLower().Contains(searchString.ToLower()) ||
                    p.Operation.Description.ToLower().Contains(searchString.ToLower()) ||
                    p.StartDate.ToString().ToLower().Contains(searchString.ToLower()) ||
                    p.Operator.Name.ToLower().Contains(searchString.ToLower()) ||
                    p.Supervisor.Name.ToLower().Contains(searchString.ToLower())
                );
            }

            var queryWoutStatus = query;

            if (status != default(int))
            {
                query = query.Where(j => j.Status == status);
            }
            //else
            //{
            //    query = query.Where(j => j.Status != 7);
            //}

            if (typeId != default)
            {
                query = query.Where(j => j.Type == typeId);
            }
            else
            {
                query = query.Where(j => j.Type != 5);
            }

            if (sortL != "id_field" && sortL != "")
            {
                query = query.OrderByDynamic(keySelectorExp, sortO).ThenByDescending(p => p.JobObservationId);
            }
            else
            {
                query = query.OrderByDynamic(keySelectorExp, sortO);
            }

            //previously returned query.OrderBy(c => c.StartDate)
            int count = query.Count();
            var list = await query.Skip((page - 1) * entries)
                        .Take(entries).ToListAsync();

            if (jobObsId != default)
            {
                list.Clear();
                list.Add(await _context.JobObservations.Include(a => a.Area)
                             .Include(p => p.Plant)
                             .Include(d => d.Distribution)
                             .Include(o => o.Operation)
                             .Include(s => s.Supervisor)
                                 .Include(o => o.Operator)
                    .FirstOrDefaultAsync(p=>p.JobObservationId == jobObsId));
            }

            JOCountPaginationDto counts = new();

            counts.DistributionCount = new();
            if (dtquery != null)
            {
                var dist = await dtquery.ToListAsync();
                foreach(var item in dist)
                {
                    JOCount jOCount = new JOCount { id = item.DistributionId, count = query.Count(j=>j.DistributionId == item.DistributionId) };
                    counts.DistributionCount.Add(jOCount);
                }
            }

            counts.OperationCount = new();
            if (opquery != null)
            {
                var oper = await opquery.ToListAsync();
                foreach (var item in oper)
                {
                    JOCount jOCount = new JOCount { id = item.OperationId, count = query.Count(j => j.OperationId == item.OperationId) };
                    counts.OperationCount.Add(jOCount);
                }
            }

            counts.OperatorCount = new();
            if (oquery != null)
            {
                var o = await oquery.ToListAsync();
                foreach (var item in o)
                {
                    JOCount jOCount = new JOCount { id = item.UserId, count = query.Count(j => j.OperatorId == item.UserId) };
                    counts.OperatorCount.Add(jOCount);
                }
            }

            counts.StatusCount = new();
            foreach (var sts in new[] { 1, 2, 3, 4, 5, 6, 7 })
            {
                JOCount jOCount = new JOCount { id = sts, count = queryWoutStatus.Count(j=>j.Status == sts) };
                counts.StatusCount.Add(jOCount);
            }

            JOCount typeCount = new JOCount { id=44, count = query.Count(j=>j.Type == 4) };
            counts.StatusCount.Add(typeCount);

            JOPaginationDto response = new JOPaginationDto
            {
                JobObservations = _mapper.Map<IEnumerable<JobObservationDto>>(list),
                Total = count,
                CountPagination = counts
            };

            return response;

        }


        public async Task<JobObservation?> FindNextYearJobObservation(int plantId, int areaId, int DistributionId, int operationId, int supervisorId, int year)
        {

            var query = _context.JobObservations
                .Where(u => u.IsActive == true && u.Type == 5);

            if (plantId != default(int))
            {
                query = query.Where(j => j.PlantId == plantId);

            }
            if (areaId != default(int))
            {
                query = query.Where(j => j.AreaId == areaId);
            }

            if (DistributionId != default(int))
            {
                query = query.Where(j => j.DistributionId == DistributionId);

            }

            if (supervisorId != default(int))
            {
                query = query.Where(j => j.SupervisorId == supervisorId);
            }


            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<JobObservation>> GetAllJobObservationsAsync(bool includeTree = false, bool includePeople = false, bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false, int idPlant = 0, int idArea = 0, bool ForSosProgram = false, int year = 0, int month = 0, int SOSAnualId = 0, int idUser = 0)
        {

            var query = _context.JobObservations.Where(j => j.IsActive == true && j.Type != 5);

            if (includeTree)
            {
                query = query.Include(a => a.Area)
                             .Include(p => p.Plant)
                             .Include(d => d.Distribution)
                             .Include(o => o.Operation);
            }

            if (includePeople)
            {
                query = query.Include(s => s.Supervisor)
                             .Include(o => o.Operator);
            }

            if (includeLup)
            {
                query = query.Include(l => l.Lup.Where(lup => lup.IsActive == true))
                        .ThenInclude(lup => lup.Evidences)
                    .Include(l => l.Lup.Where(lup => lup.IsActive == true))
                        .ThenInclude(lup => lup.Department)
                    .Where(d => d.IsActive == true);

            }

            if (includeHistory)
            {
                query = query.Include(h => h.History);
            }

            if (includeCkAnswers)
            {
                query = query.Include(c => c.checklistAnswers);
            }

            if (idPlant != 0)
            {
                query = query.Where(p => p.PlantId == idPlant);
            }

            if (idArea != 0)
            {
                query = query.Where(p => p.AreaId == idArea);
            }

            if (ForSosProgram)
            {
                query = query.Where(d => d.Type == 3);
            }

            if (year != 0)
            {
                query = query.Where(d => d.StartDate.Value.Year == year || d.EndDate.Value.Year == year);
            }

            if (month != 0)
            {
                query = query.Where(d => d.StartDate.Value.Month == month || d.EndDate.Value.Month == month);
            }


            if (SOSAnualId != 0)
            {
                //Jobs que sean regulares (Externas al SOS ID)
                query = query.Where(d => d.Type != 3);
                SOSReviewProgram? sos = _context.SOSReviews.Include(r => r.Suggestions).Include(s => s.Supervisors).Where(u => u.SOSid == SOSAnualId).FirstOrDefault();
                //Jobs que pertenezcan a los SV que partician en la SOS 
                if (sos != null && sos.Supervisors?.Count > 0)
                {
                    List<int> supervisorIds = sos.Supervisors.Select(s => s.UserId).ToList();

                    query = query.Where(j => supervisorIds.Contains((int)j.SupervisorId));

                }
                else
                {
                    return new List<JobObservation>();
                }
            }

            if (idUser != 0)
            {
                //aqui traemos al user para verificar el tipo
                User? _user = await _context.Users.Include(u => u.Subordinates).Where(p => p.UserId == idUser).FirstOrDefaultAsync();

                if (_user.UserType == 2)
                {
                    //List<int> subordinateIds = _user.Subordinates?.Select(subordinate => subordinate.UserId).ToList();

                    //if(subordinateIds.Count > 0)
                    //    query = query.Where(j => subordinateIds.Contains((int)j.SupervisorId));


                    query = query.Where(j => _user.Subordinates.Any() && _user.Subordinates.Select(subordinate => subordinate.UserId).Contains((int)j.SupervisorId));
                }
                else if (_user.UserType == 3)
                {
                    query = query.Where(j => j.SupervisorId == idUser);
                }

            }

            return await query.OrderBy(c => c.JobObservationId).ToListAsync();

        }
        

         public async Task<IEnumerable<JobObservation>> GetAllNextYearJobsObservations(int plantId, int areaId, int year)
        {

            var query = _context.JobObservations.Where(j => j.IsActive == true && j.Type == 5);

            
            if (plantId != 0)
            {
                query = query.Where(p => p.PlantId == plantId);
            }

            if (areaId != 0)
            {
                query = query.Where(p => p.AreaId == areaId);
            }

            if (year != 0)
            {
                query = query.Where(d => d.StartDate.Value.Year == year || d.EndDate.Value.Year == year);
            }

            return await query.OrderBy(c => c.JobObservationId).ToListAsync();

        }



        public async Task<JobObservation?> GetJobObservationAsync(int jobObservationId, bool includeTree = false, bool includePeople = false, bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false)
        {
            var query = _context.JobObservations.Where(p => p.JobObservationId == jobObservationId);

            if (includeTree)
            {
                query = query.Include(a => a.Area)
                             .Include(p => p.Plant)
                             .Include(d => d.Distribution)
                             .Include(o => o.Operation)
                             .Include(s => s.SignatureImage);
            }

            if (includePeople)
            {
                query = query.Include(s => s.Supervisor)
                             .Include(o => o.Operator).ThenInclude(o => o.ILURegisers);
            }

            if (includeLup)
            {
                query = query.Include(l => l.Lup.Where(lup => lup.IsActive == true)).ThenInclude(d => d.Department).Where(d => d.IsActive == true);
            }

            if (includeHistory)
            {
                query = query.Include(h => h.History);
            }

            if (includeCkAnswers)
            {
                query = query.Include(c => c.checklistAnswers).ThenInclude(ck => ck.Evidences);
            }

            return await query.FirstOrDefaultAsync();

        }

        public async Task<int> AddJobObservation(JobObservation jobObservation)
        {
            _context.JobObservations.Add(jobObservation);
            return _context.SaveChanges();
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

        public async Task AddOperatorSignatureForJobObservationAsync(int jobObservationId, FileUpload evidence)
        {
            var jobObservation = await GetJobObservationAsync(jobObservationId, true);

            if (jobObservation != null)
            {
                jobObservation.SignatureImage = evidence;

            }

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
                return await _context.Lup
                    .Include(l => l.Evidences)
                    .Include(j => j.JobObservation)
                    .Include(d => d.Department)
                    .Where(e => e.LupId == lupId).FirstOrDefaultAsync();
            }
            return await _context.Lup
                 .Where(x => x.LupId == lupId).Include(d => d.Department).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Lup>> GetAllLupAsync()
        {
            return await _context.Lup.Where(u => u.IsActive == true)
                .Include(j => j.JobObservation)
                .Include(f => f.Evidences)
                .Include(d => d.Department)
                 .OrderBy(c => c.LupId).ToListAsync();

        }

        public async Task<IEnumerable<Lup>> GetAllLupInsidences(int QuestionId, int supervisorId, int distributionId)
        {
            var query = _context.JobObservations
                .Where(j => j.IsActive == true);

            if (supervisorId != default(int))
            {
                query = query.Where(j => j.SupervisorId == supervisorId);
            }

            if (distributionId != default(int))
            {
                query = query.Where(j => j.DistributionId == distributionId);
            }



            var lups = await query
                .SelectMany(j => j.Lup.Where(lup => lup.IsActive == true))
                .ToListAsync();

            return lups.Where(u => (u.Status == 1 || u.Status == 2) && u.CreatedDate > DateTime.Now.AddDays(-14) && u.ChecklistQuestionId == QuestionId && u.EndDate == null )
                 .OrderBy(c => c.LupId);

                //.Include(j => j.JobObservation)
                //.Include(f => f.Evidences)
                //.Include(d => d.Department)
        }

        [HttpGet("lups")]
        public async Task<IEnumerable<Lup>> GetLupsByFiltersAsync(DateTime startDate, DateTime endDate, int plantId, int areaId, int distributionId, int operationId, int supervisorId, int status)
        {
            var query = _context.JobObservations
                .Where(j => j.IsActive == true);

            if (plantId != default(int))
            {
                query = query.Where(j => j.PlantId == plantId);

            }
            if (areaId != default(int))
            {
                query = query.Where(j => j.AreaId == areaId);
            }

            if (distributionId != default(int))
            {
                query = query.Where(j => j.DistributionId == distributionId);

            }
            if (operationId != default(int))
            {
                query = query.Where(j => j.OperationId == operationId);
            }

            if (supervisorId != default(int))
            {
                query = query.Where(j => j.SupervisorId == supervisorId);
            }

            if (endDate != default(DateTime))
            {
                query = query.Where(j => j.StartDate.HasValue && j.StartDate.Value.Date <= endDate.Date);
            }
            var lups = await query
                .SelectMany(j => j.Lup.Where(lup => lup.IsActive == true))
                .ToListAsync();

            if (startDate != default(DateTime))
            {
                lups = lups
                    .Where(lup => lup.CreatedDate.HasValue && lup.CreatedDate.Value.Date >= startDate.Date ||
                                   (lup.CreatedDate.HasValue && lup.CreatedDate.Value.Date <= startDate.Date &&
                                    (lup.EndDate.HasValue && lup.EndDate.Value.Date >= startDate.Date)))
                    .ToList();
            }

            if (endDate != default(DateTime))
            {
                lups = lups
                    .Where(lup => lup.CreatedDate.HasValue && lup.CreatedDate.Value.Date <= endDate.Date)
                    .ToList();
            }

            if (status != default(int))
            {
                lups = lups
                    .Where(lup => lup.Status == status)
                    .ToList();
            }

            return lups;
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
                 .OrderByDescending(c => c.NotificationID).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsFromUserAsync(int id)
        {
            return await _context.Notifications.Include(n => n.User)
                .Where(n => n.UserId == id && EF.Functions.DateDiffDay(DateTime.Now, n.EntryDate) <= 3 && EF.Functions.DateDiffMonth(DateTime.Now, n.EntryDate) == 0)
                 .OrderByDescending(c => c.NotificationID).ToListAsync();
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
                   .Include(sv => sv.Supervisor)
                   .Include(ssv => ssv.SSVresponsible)
                   .Include(lr => lr.LeadershipRecords)
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
                   .Include(sv => sv.Supervisor)
                   .Include(ssv => ssv.SSVresponsible).Where(u => u.IsActive == true)
                    .OrderBy(c => c.PATid).ToListAsync();
        }
        public async Task<IEnumerable<PAT>> GetAllPATsOfSv(int svId)
        {
            return await _context.PATs
                    .Include(p => p.Plant)
                    .Include(a => a.Area)
                    .Include(sv => sv.Supervisor)
                    .Include(ssv => ssv.SSVresponsible)
                    .Where(p => p.SupervisorId == svId && p.IsActive == true)
                    .OrderBy(c => c.PATid).ToListAsync();
        }
        public async Task<IEnumerable<PAT>> GetAllPATsofSSV(int ssvID)
        {
            return await _context.PATs
                           .Include(p => p.Plant)
                   .Include(a => a.Area)
                   .Include(sv => sv.Supervisor)
                   .Include(ssv => ssv.SSVresponsible)
                           .Where(p => p.SSVresponsibleID == ssvID && p.IsActive == true)
                            .OrderBy(c => c.PATid).ToListAsync();
        }
        #endregion
        #region LeadershipRecords

        public async Task<int> AddLeadershipRecordToPAT(PAT entity, LeadershipRecord leadershipRecordsForCreation)
        {
            if (entity.LeadershipRecords != null)
            {
                entity.LeadershipRecords.Add(leadershipRecordsForCreation);
            }
            else
            {
                entity.LeadershipRecords = new List<LeadershipRecord>();
                entity.LeadershipRecords.Add(leadershipRecordsForCreation);
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateLeadershipRecordToPAT(PAT entity, LeadershipRecordsForUpdateDto leadershipRecordsForUpdate)
        {
            LeadershipRecord recordEntity = new LeadershipRecord();

            recordEntity = entity.LeadershipRecords.ToList().Find(r => r.LeadershipRecordsid == leadershipRecordsForUpdate.LeadershipRecordsid);
            _mapper.Map(leadershipRecordsForUpdate, recordEntity);

            return await _context.SaveChangesAsync();
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

        public async Task<IEnumerable<SOSReviewProgram>> GetAllSOSReviews(bool includeNavigation = false, bool includeUsers = false, bool includeSuggestions = false)
        {
            var query = _context.SOSReviews.Where(u => u.IsActive == true);

            if (includeNavigation)
            {
                query = query.Include(p => p.Plant)
                    .Include(a => a.Area);
            }

            if (includeSuggestions) {
                query = query.Include(r => r.Suggestions);
            }

            if (includeUsers) {
                query = query.Include(s => s.Supervisors);
            }

            return await query.OrderBy(c => c.SOSid).ToListAsync();
        }

        public async Task<SOSReviewProgram?> GetSOSasync(int sosId, bool includeNavigation = false, bool includeUsers = false, bool includeSuggestions = false)
        {
            var query = _context.SOSReviews.Where(p => p.SOSid == sosId);

            if (includeNavigation)
            {
                query = query.Include(p => p.Plant)
                    .Include(a => a.Area);
            }

            if (includeSuggestions)
            {
                query = query.Include(r => r.Suggestions);
            }

            if (includeUsers)
            {
                query = query.Include(s => s.Supervisors);
            }


            return await query.FirstOrDefaultAsync();
        }

        public async Task<SOSReviewProgram?> FindSOSasync(int plantId, int areaId, int year, bool includeNavigation = false, bool includeUsers = false, bool includeSuggestions = false)
        {
            var query = _context.SOSReviews.Where(p => p.PlantId == plantId && p.AreaId == areaId && p.AplicationYear == year);

            if (includeNavigation)
            {
                query = query.Include(p => p.Plant)
                    .Include(a => a.Area);
            }

            if (includeSuggestions)
            {
                query = query.Include(r => r.Suggestions);
            }

            if (includeUsers)
            {
                query = query.Include(s => s.Supervisors);
            }


            return await query.FirstOrDefaultAsync();
        }

        public async Task<SOSReviewProgram?> FindSOSSupervisor(int plantId, int areaId, int year, int SV_id)
        {
            var query = _context.SOSReviews.Where(p => p.PlantId == plantId && p.AreaId == areaId && p.AplicationYear == year);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> AddSOSReview(SOSReviewProgram SOSEntity)
        {
            _context.SOSReviews.Add(SOSEntity);
            return _context.SaveChanges();
        }

        public async void SOSReviewAddUser(SOSReviewProgram Master, User Slave)
        {
            Master.Supervisors?.Add(Slave);
            _context.SaveChanges();
        }

        public async void SOSReviewRemoveUser(SOSReviewProgram Master, User Slave)
        {
            Master.Supervisors?.Remove(Slave);
            _context.SaveChanges();
        }

        public async void SOSReviewAddDistSuggestion(SOSReviewProgram Master, SOSReviewDistSuggestion Slave)
        {
            Master.Suggestions?.Add(Slave);
            _context.SaveChanges();
        }

        public async Task<int> CreateSOSReviewDistSuggestion(SOSReviewDistSuggestion RegEntity)
        {
            await _context.SOSSuggestionsDistribution.AddAsync(RegEntity);
            return _context.SaveChanges();
        }

        public async Task<SOSReviewDistSuggestion?> GetDistSuggestion(int sosId, int dist_id)
        {
            return await _context.SOSSuggestionsDistribution
                            .Where(p => p.SOSReviewProgramid == sosId && p.DistributionId == dist_id).FirstOrDefaultAsync();
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
        #region SOS_RegOperationJobObservartion
        public async Task<int> AddSOSReviewRegister(SOSRegisterJobObservation RegEntity)
        {
            _context.SOSRegisters.Add(RegEntity);
            return _context.SaveChanges();
        }
        public async Task<IEnumerable<SOSRegisterJobObservation>> GetAllSOSReviewsRegisters(int SOSReviewProgramId)
        {
            return await _context.SOSRegisters
                   .Include(j => j.JobObservation).ThenInclude(jj => jj.Distribution)
                   .Include(d => d.Operation)
                   .Include(s => s.SOSReviewProgram)
                   .Where(u => u.SOSReviewProgramid == SOSReviewProgramId)
                    .OrderBy(c => c.SOSRegisterJobid).ToListAsync();

        }
        public async Task<SOSRegisterJobObservation?> GetSOSReviewRegister(int SosId)
        {
            return await _context.SOSRegisters
                   .Include(j => j.JobObservation)
                   .Include(d => d.Operation)
                   .Include(s => s.SOSReviewProgram)
                   .Where(p => p.SOSRegisterJobid == SosId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateRegisterJobObservation(SOSReviewsRegisterForUpdateDto SOSForUpdate, SOSRegisterJobObservation SOSEntity)
        {
            _mapper.Map(SOSForUpdate, SOSEntity);

            return _context.SaveChanges();
        }
        public Task<IEnumerable<SOSRegisterJobObservation>> GetAllSOSReviewsRegistersByDistribution(int SOSReviewProgramId, int distributionid)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region SOS_RegUserOperation
        public async Task<int> AddSOSRegUserOperation(SOSRegUserOperation RegEntity)
        {
            _context.SOSRegsUserOperation.Add(RegEntity);
            return _context.SaveChanges();
        }


        public async Task<SOSRegUserOperation?> GetSOSRegUserOperation(int SosId)
        {
            return await _context.SOSRegsUserOperation
                   .Include(d => d.Operation)
                   .Include(U => U.Supervisor)
                   .Include(s => s.SOSReviewProgram)
                   .Where(p => p.SOSRegUserOperationId == SosId).FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<SOSRegUserOperation>> GetAllSOSRegUserOperations(int SosId)
        {
            return await _context.SOSRegsUserOperation
                   .Include(d => d.Operation)
                   .Include(U => U.Supervisor)
                   .Include(s => s.SOSReviewProgram)
                   .Where(u => u.SOSReviewProgramid == SosId)
                    .OrderBy(c => c.SOSRegUserOperationId).ToListAsync();

        }
        public async Task<int> UpdateRegUserOperation(SOSRegUserOperationForUpdateDto SOSForUpdate, SOSRegUserOperation SOSEntity)
        {
            _mapper.Map(SOSForUpdate, SOSEntity);

            return _context.SaveChanges();
        }
        #endregion
        #region HeadCount
        public async Task<IEnumerable<HeadCount>> GetAllHeadCountsDataAsync()
        {
            return await _context.headCounts.ToListAsync();
        }

        public async Task<AsyncVoidMethodBuilder> RemoveAllHeadCountRegisters()
        {
            _context.headCounts.RemoveRange(_context.headCounts);
            await _context.SaveChangesAsync();

            return new AsyncVoidMethodBuilder();
        }

        public async Task AddHeadCoutAsync(HeadCount user)
        {
            _context.headCounts.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<HeadCount?> GetHeadCountByIdAsync(int HeadId)
        {
            return await _context.headCounts.Where(a => a.HeadCountId == HeadId).FirstOrDefaultAsync();
        }

        #endregion
        #region HeadCountProcess
        //public async Task<int> AddHeadCountProcess(HeadCountProcess headCountProcess)
        //{
        //    _context.headCountsProcess.Add(headCountProcess);
        //    return await _context.SaveChangesAsync();
        //}
        //public async Task<HeadCountProcess?> GetHeadCountProcessById(int id)
        //{
        //    return await _context.headCountsProcess.Where(a => a.HeadCountProcessId == id).FirstOrDefaultAsync();
        //}

        //public async Task<IEnumerable<HeadCountProcess>> GetAllHeadCountProcess()
        //{
        //    return _context.headCountsProcess.ToList();
        //}
        //public async Task<int> UpdateHeadCountProcess(HeadCountProcessCreateUpdateDto headCountProcess, HeadCountProcess entity)
        //{
        //    _mapper.Map(headCountProcess, entity);
        //    return await _context.SaveChangesAsync();
        //}
        //public async Task<int> DeleteHeadCountProcess(HeadCountProcess headCountProcess)
        //{
        //    _context.Remove(headCountProcess);
        //    return await _context.SaveChangesAsync();
        //}


        #endregion
        #region DepartmentOperations
        public async Task<IEnumerable<Entities.Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.Where(d => d.IsActive == true)
                .OrderBy(d => d.DepartmentId).ToListAsync();
        }

        public async Task<Entities.Department?> GetDepartmentAsync(int departmentId)
        {
            return await _context.Departments
                .Where(c => c.DepartmentId == departmentId).FirstOrDefaultAsync();
        }

        public async Task<bool> DepartmentExistAsync(int departmentId)
        {
            return await _context.Departments.AnyAsync(p => p.DepartmentId == departmentId);
        }


        public void AddDepartment(Entities.Department department)
        {
            _context.Departments.Add(department);
        }

        public void DeleteDepartment(Entities.Department department)
        {
            //_context.Groups.Remove(group);
            department.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region StationOperations
        public async Task<IEnumerable<Entities.Station>> GetStationsAsync()
        {
            return await _context.Stations.Where(d => d.IsActive == true)
                .OrderBy(d => d.StationId).ToListAsync();
        }

        public async Task<Entities.Station?> GetStationAsync(int StationId)
        {
            return await _context.Stations
                .Where(c => c.StationId == StationId).FirstOrDefaultAsync();
        }

        public async Task<bool> StationExistAsync(int StationId)
        {
            return await _context.Stations.AnyAsync(p => p.StationId == StationId);
        }


        public void AddStation(Entities.Station Station)
        {
            _context.Stations.Add(Station);
        }

        public void DeleteStation(Entities.Station Station)
        {
            //_context.Groups.Remove(group);
            Station.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region PillarOperations
        public async Task<IEnumerable<Pillar>> GetPillarsAsync()
        {
            return await _context.Pillars.Where(u => u.IsActive == true)
                .OrderBy(c => c.PillarId).ToListAsync();
        }

        public async Task<Pillar?> GetPillarAsync(int pillarId)
        {
            return await _context.Pillars
                .Where(c => c.PillarId == pillarId).FirstOrDefaultAsync();
        }

        public async Task<List<Pillar>?> GetPillarsFromList(List<int>? pillarIds)
        {
            return pillarIds != null && pillarIds.Any() ? await _context.Pillars
                .Where(p => pillarIds.Contains(p.PillarId)).ToListAsync() : null;
        }

        public async Task<bool> PillarExistAsync(int pillarId)
        {
            return await _context.Pillars.AnyAsync(p => p.PillarId == pillarId);
        }


        public void AddPillar(Pillar pillar)
        {
            _context.Pillars.Add(pillar);
        }

        public void DeletePillar(Entities.Pillar pillar)
        {
            //_context.Pillars.Remove(pillar);
            pillar.IsActive = false;
            _context.SaveChanges();
        }
        #endregion
        #region ChecklistAnswersOperations
        public async Task<ChecklistAnswer?> GetChecklistAnswerAsync(int checklistAnswerId)
        {
            return await _context.ChecklistAnswers.Include(a => a.Evidences)
                 .Where(x => x.AnswerId == checklistAnswerId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ChecklistAnswer>> GetAllChecklistAnswerAsync()
        {
            return await _context.ChecklistAnswers.Include(a => a.Evidences)
                 .OrderBy(c => c.AnswerId).ToListAsync();

        }
        public async Task<IEnumerable<ChecklistAnswer>> GetAllChecklistAnswersByJobObservationIdAsync(int jobObservationId)
        {
            return await _context.ChecklistAnswers.Include(a => a.Evidences)
                .Where(c => c.JobObservationId == jobObservationId)
                 .OrderBy(c => c.AnswerId).ToListAsync();

        }
        public void AddChecklistAnswer(ChecklistAnswer checklistAnswer)
        {
            _context.ChecklistAnswers.Add(checklistAnswer);
        }
        public async Task AddEvidenceForCkAnswerAsync(int answerId, FileUpload evidence)
        {
            var answer = await GetChecklistAnswerAsync(answerId);

            if (answer != null)
            {
                if (answer.Evidences != null)
                {
                    answer.Evidences.Add(evidence);
                }
                else
                {
                    answer.Evidences = new List<FileUpload>
                    {
                        evidence
                    };
                }
            }
        }

        public void DeleteChecklistAnswer(ChecklistAnswer checklistAnswer)
        {
            checklistAnswer.Answer = null;
            _context.SaveChanges();
        }

        public async Task<bool> ChecklistAnswerExistAsync(int checklistAnswerId)
        {
            return await _context.ChecklistAnswers.AnyAsync(l => l.AnswerId == checklistAnswerId);
        }

        #endregion
        #region CommonOperations
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }


        #endregion
        #region Kaizen
        public async Task<Kaizen?> GetKaizen(int KaizenId, bool includeNavigation = false, bool includePeople = false, bool includeEvidences = false, bool includeTransactions = false)
        {
            var query = _context.Kaizens.Where(k => k.IsActive == true && k.KaizenId == KaizenId);

            if (includeNavigation)
            {
                query = query
                 .Include(p => p.Plant)
                 .Include(p => p.Pillar)
                 .Include(a => a.Area);
            }

            if (includePeople)
            {
                query = query
                 .Include(s => s.Supervisor)
                 .Include(ssv => ssv.SeniorSupervisor)
                 .Include(up => up.Proposed);
            }

            if (includeEvidences)
            {
                query = query
                 .Include(ae => ae.PreviousEvidences)
                 .Include(be => be.ThenEvidences);

            }

            if (includeTransactions)
            {
                query = query
                 .Include(t => t.Transactions);
            }

            return await query.FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<Kaizen>> GetAllKaizens(bool includeNavigation = false, bool includePeople = false, bool includeEvidences = false, bool includeTransactions = false)
        {
            var query = _context.Kaizens.Where(u => u.IsActive == true);

            if (includeNavigation)
            {
                query = query
                 .Include(p => p.Plant)
                 .Include(p => p.Pillar)
                 .Include(a => a.Area);
            }

            if (includePeople)
            {
                query = query
                 .Include(s => s.Supervisor)
                 .Include(ssv => ssv.SeniorSupervisor)
                 .Include(up => up.Proposed);
            }

            if (includeEvidences)
            {
                query = query
                 .Include(ae => ae.PreviousEvidences)
                 .Include(be => be.ThenEvidences);

            }

            if (includeTransactions)
            {
                query = query
                 .Include(t => t.Transactions);
            }

            query = query.OrderBy(c => c.KaizenId);

            return query.ToList();
        }
        public async Task<int> AddKaizen(Kaizen KaizenForAdd)
        {
            _context.Kaizens.Add(KaizenForAdd);
            return _context.SaveChanges();
        }
        public async Task<int> UpdateKaizen(UpdateKaizenDto KaizenForUpdate, Kaizen KaizenEntity)
        {
            
            _mapper.Map(KaizenForUpdate, KaizenEntity);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveKaizen(Kaizen KaizenForAdd)
        {
            KaizenForAdd.IsActive = false;
            return _context.SaveChanges();
        }

        public async Task AddPreviousEvidenceForKaizen(int kaizenId, FileUpload evidence)
        {
            var kaizen = await GetKaizen(kaizenId, true);

            if (kaizen != null)
            {

                if (kaizen.PreviousEvidences != null)
                {
                    kaizen.PreviousEvidences.Add(evidence);
                }
                else
                {
                    kaizen.PreviousEvidences = new List<FileUpload>
                    {
                        evidence
                    };

                }


            }

        }

        public async Task AddThenEvidenceForKaizen(int kaizenId, FileUpload evidence)
        {
            var kaizen = await GetKaizen(kaizenId, true);

            if (kaizen != null)
            {

                if (kaizen.ThenEvidences != null)
                {
                    kaizen.ThenEvidences.Add(evidence);
                }
                else
                {
                    kaizen.ThenEvidences = new List<FileUpload>
                    {
                        evidence
                    };

                }


            }

        }

        public async Task RemoveEvidenceForKaizenAsync(int kaizenId, int fileUploadId, bool isPreviousEvidence)
        {
            var kaizen = await GetKaizen(kaizenId, true, true, true, false);
            if (kaizen != null)
            {
                if (isPreviousEvidence)
                {
                    if (kaizen.PreviousEvidences != null)
                    {
                        kaizen.PreviousEvidences.Remove(item: kaizen.PreviousEvidences.ToList().Find(e => e.FileUploadId == fileUploadId));
                    }

                }
                else
                {
                    if (kaizen.ThenEvidences != null)
                    {
                        kaizen.ThenEvidences.Remove(item: kaizen.ThenEvidences.ToList().Find(e => e.FileUploadId == fileUploadId));
                    }
                }
            }
        }
        #endregion
        #region HCI
        public async Task<HCI?> GetHCI(int HCIId, bool includeNavigation = false, bool includePeople = false, bool includeCommentaries = false, bool includeTransactions = false)
        {
            var query = _context.HCIs.Where(k => k.IsActive == true && k.HCIId == HCIId);

            if (includeNavigation)
            {
                query = query.Include(p => p.CareerPaths).Include(p => p.Categories).Include(p => p.Commentaries).Include(p => p.ILUs).Include(p => p.Transactions);
            }

            if (includePeople)
            {
                query = query
                 .Include(t => t.User).ThenInclude(u => u.Plant);

                query = query
                 .Include(t => t.User).ThenInclude(u => u.Area);
            }

            //if (includeTransactions)
            //{
            //    query = query
            //     .Include(t => t.Transactions);
            //}

            return await query.FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<HCI>> GetAllHCIs(bool includeNavigation = false, bool includePeople = false, bool includeCommentaries = false, bool includeTransactions = false)
        {
            var query = _context.HCIs.Where(u => u.IsActive == true);

            if (includePeople)
            {
                query = query.Include(t => t.User);
            }

            if (includeCommentaries)
            {
                query = query.Include(c => c.Commentaries);
            }

            if (includeNavigation)
            {
                query = query
                 .Include(t => t.User).ThenInclude(u => u.Plant);

                query = query
                 .Include(t => t.User).ThenInclude(u => u.Area);
                
                query = query
                 .Include(t => t.User).ThenInclude(u => u.ILURegisers);
            }


            if (includeTransactions)
            {
                query = query
                 .Include(t => t.Transactions);
            }

            query = query.OrderBy(c => c.HCIId);

            return await query.ToListAsync();
        }
        public async Task<int> AddHCI(HCI HCIForAdd)
        {
            //HCIForAdd.User = _context.Users.FirstOrDefault(p=>p.UserId == HCIForAdd.UserId);
            _context.HCIs.Add(HCIForAdd);
            return _context.SaveChanges();
        }
        public async Task<int> UpdateHCI(UpdateHCIDto HCIForUpdate, HCI HCIEntity)
        {

            _mapper.Map(HCIForUpdate, HCIEntity);

            return _context.SaveChanges();
        }

        public async Task<int> RemoveHCI(HCI HCIForAdd)
        {
            HCIForAdd.IsActive = false;
            return _context.SaveChanges();
        }


        public async  Task<IEnumerable<User>> GetUsersWithoutHci()
        {
            var usuariosSinHCI = _context.Users.Where(u => !_context.HCIs.Any(hci => hci.UserId == u.UserId));
            return await usuariosSinHCI.ToListAsync();
        }

        public async Task<IEnumerable<HCICategory>> GetHCICategories()
        {
            return _context.HCICategories.OrderBy(p => p.ChosenCategoryDepartmentId).ToList();
        }
        #endregion

        #region HCI ILU
        public async Task<int> AddHciIluReg(HCIILU registry)
        {
            _context.HCIILUs.Add(registry);
            return _context.SaveChanges();
        }
        #endregion



    }
}
