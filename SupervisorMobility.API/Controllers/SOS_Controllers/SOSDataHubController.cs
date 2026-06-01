using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOS.ToolsUsedDtos;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/DataPool")]
    [ApiController]
    public class SOSDataHubController : ControllerBase
    {
        private readonly ISOS_ProcessRepository _AnalysisProcessRepository;
        private readonly IMapper _mapper;
        //private readonly SupervisorMobilityContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationService _notificationService;

        public SOSDataHubController(ISOS_ProcessRepository repository, IWebHostEnvironment env, IMapper mapper, INotificationService notificationService)
        {
            _AnalysisProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        [HttpPost]
        public async Task<ActionResult<SOSHubDto>> CreateSOSHub(SOSHubForCreateDto SOSHubForCreate)
        {
            List<Equipment> equipments = new List<Equipment>();
            List<Product> applyModels = new List<Product>();
            List<User> usersApproverOwners = new List<User>();
            List<User> usersReviewerEditors = new List<User>();

            if (SOSHubForCreate.SafetyEquipment != null)
                foreach (var equipment in SOSHubForCreate.SafetyEquipment)
                {
                    Equipment equipmentaux = await _AnalysisProcessRepository.GetEquipmentById(equipment.EquipmentId);
                    equipments.Add(equipmentaux);
                }

            if(SOSHubForCreate.ApproverOwners != null)
                foreach (var user in SOSHubForCreate.ApproverOwners)
                {
                    User useraux = await _AnalysisProcessRepository.GetUserById(user.UserId);
                    usersApproverOwners.Add(useraux);
                }

            if (SOSHubForCreate.ReviewerEditors != null)
                foreach (var user in SOSHubForCreate.ReviewerEditors)
                {
                    User useraux = await _AnalysisProcessRepository.GetUserById(user.UserId);
                    usersReviewerEditors.Add(useraux);
                }

            if (SOSHubForCreate.AppliedModels != null)
                foreach (var applymodel in SOSHubForCreate.AppliedModels)
                {
                    Product productaux = await _AnalysisProcessRepository.GetProductById(applymodel.ProductId);
                    applyModels.Add(productaux);
                }

            SOSHubForCreate.SafetyEquipment = null;
            SOSHubForCreate.ApproverOwners = null;
            SOSHubForCreate.ReviewerEditors = null;
            SOSHubForCreate.AppliedModels = null;

            SOSHub SOSEntity = new SOSHub();

            _mapper.Map(SOSHubForCreate, SOSEntity);

            SOSEntity.CreatedDate = DateTime.Now;
            if (SOSHubForCreate.CreatorId.HasValue)
            {
                SOSEntity.CreatorId = SOSHubForCreate.CreatorId;
            }

            // IF is draft auto set null ids
            if (SOSEntity.AreaId == 0) SOSEntity.AreaId = null;
            if (SOSEntity.DepartmentId == 0) SOSEntity.DepartmentId = null;
            if (SOSEntity.DistributionId == 0) SOSEntity.DistributionId = null;
            if (SOSEntity.PlantId == 0) SOSEntity.PlantId = null;
            if (SOSEntity.StationId == 0) SOSEntity.StationId = null;

            //se envia la distribucion id en null para que no genere conflicto al crear la sos hub
            if (SOSEntity.DistributionId != null) SOSEntity.DistributionId = null;

            SOSHub createdResult = await _AnalysisProcessRepository.CreateSOScollection(SOSEntity);

            if (equipments.Any())
            {
                foreach (Equipment equipment in equipments)
                {
                    await _AnalysisProcessRepository.AddEquipmentToSOSCollection(createdResult, equipment);
                }
            }

            if (usersApproverOwners.Any())
            {
                foreach (User userApprover in usersApproverOwners)
                {
                    await _AnalysisProcessRepository.AddApproverOwnersToSOSCollection(createdResult, userApprover);
                }
            }

            if (usersReviewerEditors.Any())
            {
                foreach (User userReviewer in usersReviewerEditors)
                {
                    await _AnalysisProcessRepository.AddReviewerEditorToSOSCollection(createdResult, userReviewer);
                }
            }

            if (applyModels.Any())
            {
                foreach (Product applymodel in applyModels)
                {
                    await _AnalysisProcessRepository.AddProductToSOSCollection(createdResult, applymodel);
                }
            }

            if (createdResult != null)
            {
                _notificationService.CreateNotificationAsync(new Models.NotificationDtos.NotificationToCreateDto
                {
                    MadeBy = "SM Mobility",
                    NotificationType = "New SOS Hub Created",
                    NotificationText = $"A new SOS Hub with ID {createdResult.SOSHubId} has been created.",
                    // UserId = SOSEntity.CreatorId ?? 0, // Assuming the creator should receive the notification
                    UserId = 1, // Assuming the creator should receive the notification
                    IsActive = true,
                    IsAccepted = true,
                    EntryDate = DateTime.Now
                });
                return Ok(_mapper.Map<SOSHubDto>(createdResult));
            }
            else
                return BadRequest();

        }

        //get
        [HttpGet("{id}", Name = "GetSOSHub")]
        public async Task<ActionResult<SOSHubDto>> GetSOHub(int id, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false, bool includeCollections = false, bool includePeopleCollections = false, bool includePats = false)
        {

            var SOSHub = await _AnalysisProcessRepository.GetSOSHub(id, includeAnalysesBkup: includeAnalysesBkup, includeSections: includeSections, includeImages: includeImages, includeVideos: includeVideos, includeCommentaries: includeCommentaries, includeTools: includeTools, includeEquipments: includeEquipments, includeMaterials: includeMaterials, includeInformation: includeInformation, includePeople: includePeople, includeDocuments: includeDocuments, includeModel: includeModel, includeCollections: includeCollections, includePeopleCollections: includePeopleCollections, includePats: includePats);
            if (SOSHub == null)
            {
                return NotFound("SOSHub not found!");
            }

            return Ok(_mapper.Map<SOSHubDto>(SOSHub));
        }

        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<SOSHubDto>>> GetAllSOSHub(bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeSOSDistribution = false, int userId = 0)
        {

            var CheckpointEntities = await _AnalysisProcessRepository.GetAllSOSHub(includeImages: includeImages, includeVideos: includeVideos, includeCommentaries: includeCommentaries, includeTools: includeTools, includeEquipments: includeEquipments, includeMaterials: includeMaterials, includeInformation: includeInformation, includePeople: includePeople, includeDocuments: includeDocuments, includeSOSDistribution: includeSOSDistribution, userId : userId);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Hub not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSHubDto>>(CheckpointEntities));
        }

        //Update
        [HttpPut("{SOSHubId}")]
        public async Task<ActionResult<SOSHubDto>> UpdateSOSHub(int SOSHubId, SOSHubForUpdateDto _SOSHubForUpdate)
        {
            SOSHub entitySOSHub = await _AnalysisProcessRepository.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true, true, true, true, includeDeleteds: true);

            List<Commentary> ProcessSheetCommentaries = new List<Commentary>();
            List<AnalysisBkup> AnalysisBkups = new List<AnalysisBkup>();
            List<Section> Sections = new List<Section>();
            List<ToolUsed> tools = new List<ToolUsed>();
            List<MaterialUsed> materials = new List<MaterialUsed>();
            List<Equipment> equipments = new List<Equipment>();
            List<CommonDirection> commons = new List<CommonDirection>();
            List<User> usersApproverOwners = new List<User>();
            List<User> usersReviewerEditors = new List<User>();
            List<Product> applyModels = new List<Product>();

            //Commmon direction 
            List<CommonDirectionDto> filteredCommonDirectionList = _SOSHubForUpdate.CommonDirection
           .Where(t => t.CommonDirectionId <= 0).ToList();

            if (filteredCommonDirectionList.Count > 0)
            {

                List<CommonDirection> commonDirectionUnlinked = await _AnalysisProcessRepository.GetAllCommonDirectionInactives();

                List<CommonDirection> existingList = entitySOSHub.CommonDirection.ToList();
                existingList = existingList.Union(commonDirectionUnlinked).ToList();

                List<CommonDirection> CDtoCreate = new List<CommonDirection>();


                // Remover nuevos commonDirection de la lista principal para evitar duplicados
                if (filteredCommonDirectionList.Any())
                {
                    _SOSHubForUpdate.CommonDirection.RemoveAll(t => t.CommonDirectionId == null || t.CommonDirectionId <= 0);

                    foreach (var commonDirection in filteredCommonDirectionList)
                    {
                        if (existingList.Any(p => p.DOC_ID == commonDirection.DOC_ID))
                        {
                            var element = existingList.First(p => p.DOC_ID == commonDirection.DOC_ID);
                            commonDirection.CommonDirectionId = element.CommonDirectionId;

                            _SOSHubForUpdate.CommonDirection.Add(commonDirection);
                        }
                        else
                        {
                            var element = _mapper.Map<CommonDirection>(commonDirection);
                            CDtoCreate.Add(element);
                        }
                    }

                    if (CDtoCreate.Count > 0)
                    {
                        var resultAddCD = await _AnalysisProcessRepository.AddRangeCommonDirection(CDtoCreate);

                        if (resultAddCD != null)
                        {
                            Debug.WriteLine("Common Direction añadidas con éxito");
                            commons.AddRange(resultAddCD);
                        }
                    }

                }
            }


            // Filtrar nuevos Comentarios
            List<UpdateCommentaryDto> filteredCommentaryList = _SOSHubForUpdate.ProcessSheetCommentary
                .Where(t => t.CommentaryId <= 0).ToList();

            // Remover nuevos Comentarios de la lista principal para evitar duplicados
            if (filteredCommentaryList.Any())
            {
                _SOSHubForUpdate.ProcessSheetCommentary.RemoveAll(t => t.CommentaryId == null || t.CommentaryId <= 0);

                // Mapear nuevas norms/standars
                List<Commentary> newCommentarys = _mapper.Map<List<Commentary>>(filteredCommentaryList);

                foreach (var newComentary in newCommentarys)
                {
                    newComentary.CommentaryId = 0;
                    newComentary.IsActive = true;
                }

                var resultAddCommentary = await _AnalysisProcessRepository.AddRangeCommentary(newCommentarys);

                if (resultAddCommentary != null)
                {
                    Debug.WriteLine("Commentarios añadidos con exitop");
                    ProcessSheetCommentaries.AddRange(resultAddCommentary);
                }
                else
                {
                    Debug.WriteLine("Error Commentarios añadidos");
                }
            }

            List<AnalysisBkupForUpdateDto> filteredAnalysisBkupList = _SOSHubForUpdate.AnalysesBkup
                .Where(t => t.AnalysisBkupId <= 0).ToList();

            // Remover nuevos AnalysisBkup de la lista principal para evitar duplicados
            if (filteredAnalysisBkupList.Any())
            {
                _SOSHubForUpdate.AnalysesBkup.RemoveAll(t => t.AnalysisBkupId == null || t.AnalysisBkupId <= 0);

                // Mapear nuevas AnalysisBkup
                List<AnalysisBkup> newAnalysisBkups = _mapper.Map<List<AnalysisBkup>>(filteredAnalysisBkupList);

                foreach (var newAnalysisBkup in newAnalysisBkups)
                {
                    newAnalysisBkup.AnalysisBkupId = 0;
                    newAnalysisBkup.IsActive = true;
                }

                var resultAddAnalysisBkup = await _AnalysisProcessRepository.AddRangeAnalysisBkup(newAnalysisBkups);

                if (resultAddAnalysisBkup != null)
                {
                    Debug.WriteLine("AnalysisBkup añadidos con exitop");
                    AnalysisBkups.AddRange(resultAddAnalysisBkup);
                }
                else
                {
                    Debug.WriteLine("Error AnalysisBkup añadidos");
                }

            }

            List<SectionForUpdateDto> filteredSectionList = _SOSHubForUpdate.Sections
              .Where(t => t.SectionId <= 0).ToList();

            // Remover nuevos Section de la lista principal para evitar duplicados
            if (filteredSectionList.Any())
            {
                _SOSHubForUpdate.Sections.RemoveAll(t => t.SectionId == null || t.SectionId <= 0);

                // Mapear nuevas sections
                List<Section> newSections = _mapper.Map<List<Section>>(filteredSectionList);

                foreach (var newSec in newSections)
                {
                    newSec.SectionId = 0;
                    newSec.IsActive = true;
                }

                var resultAddSections = await _AnalysisProcessRepository.AddRangeSections(newSections);

                if (resultAddSections != null)
                {
                    Debug.WriteLine("Sections añadidas con exito");
                    Sections.AddRange(resultAddSections);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                }

            }

            // Filtrar nuevos ToolUsed
            List<ToolUsedForUpdateDto> filteredToolsUsedList = _SOSHubForUpdate.ToolsUsed
                .Where(t => t.ToolUsedId <= 0).ToList();
            if (filteredToolsUsedList.Any())
            {
                _SOSHubForUpdate.ToolsUsed.RemoveAll(t => t.ToolUsedId == null || t.ToolUsedId <= 0);

                List<ToolUsed> newToolsUseds = new List<ToolUsed>();

                List<ToolUsed> existingToolList = entitySOSHub.ToolsUsed.ToList();

                foreach (var toolUsed in filteredToolsUsedList)
                {
                    if (existingToolList.Any(p => p.ToolId == toolUsed.ToolId))
                    {
                        var element = existingToolList.First(p => p.ToolId == toolUsed.ToolId);
                        toolUsed.ToolUsedId = element.ToolUsedId;

                        _SOSHubForUpdate.ToolsUsed.Add(toolUsed);
                    }
                    else
                    {
                        var element = _mapper.Map<ToolUsed>(toolUsed);
                        newToolsUseds.Add(element);
                    }
                }

                if (newToolsUseds.Count > 0)
                {

                    foreach (var newtool in newToolsUseds)
                    {
                        newtool.ToolUsedId = 0;
                        newtool.IsActive = true;
                    }

                    var resultAddToolsUsed = await _AnalysisProcessRepository.AddRangeToolsUsed(newToolsUseds);

                    if (resultAddToolsUsed != null)
                    {
                        Debug.WriteLine("Tools used añadidos con exito");
                        tools.AddRange(resultAddToolsUsed);
                    }
                    else
                    {
                        Debug.WriteLine("Error Tools used añadidos");
                    }
                }
            }


            // Filtrar nuevos MaterialUsed
            List<MaterialsUsedForUpdateDto> filteredMaterialsUsedList = _SOSHubForUpdate.MaterialsUsed
                .Where(t => t.MaterialUsedId <= 0).ToList();

            // Remover nuevos MaterialUsed de la lista principal para evitar duplicados
            if (filteredMaterialsUsedList.Any())
            {
                _SOSHubForUpdate.MaterialsUsed.RemoveAll(t => t.MaterialUsedId == null || t.MaterialUsedId <= 0);

                List<MaterialUsed> newMaterialsUseds = new List<MaterialUsed>();

                List<MaterialUsed> existingList = entitySOSHub.MaterialsUsed.ToList();

                foreach (var materialUsed in filteredMaterialsUsedList)
                {
                    if (existingList.Any(p => p.MaterialId == materialUsed.MaterialId))
                    {
                        var element = existingList.First(p => p.MaterialId == materialUsed.MaterialId);
                        materialUsed.MaterialUsedId = element.MaterialUsedId;

                        _SOSHubForUpdate.MaterialsUsed.Add(materialUsed);
                    }
                    else
                    {
                        var element = _mapper.Map<MaterialUsed>(materialUsed);
                        newMaterialsUseds.Add(element);
                    }
                }

                if (newMaterialsUseds.Count > 0)
                {

                    foreach (var newmaterial in newMaterialsUseds)
                    {
                        newmaterial.MaterialUsedId = 0;
                        newmaterial.IsActive = true;
                    }

                    var resultAddMaterialsUsed = await _AnalysisProcessRepository.AddRangeMaterialUsed(newMaterialsUseds);

                    if (resultAddMaterialsUsed != null)
                    {
                        Debug.WriteLine("Materials used añadidos con exitop");
                        materials.AddRange(resultAddMaterialsUsed);
                    }
                    else
                    {
                        Debug.WriteLine("Error Materials used añadidos");
                    }
                }

            }

            ProcessSheetCommentaries.AddRange(entitySOSHub.ProcessSheetCommentary?.Where(p => p.IsActive == false));
            AnalysisBkups.AddRange(entitySOSHub.AnalysesBkup?.Where(p => p.IsActive == false));
            Sections.AddRange(entitySOSHub.Sections?.Where(p => p.IsActive == false));
            commons.AddRange(entitySOSHub.CommonDirection?.Where(p => p.IsActive == false));

            var existingToolsIds = new HashSet<int>(_SOSHubForUpdate.ToolsUsed.Select(m => m.ToolUsedId));
            tools.AddRange(entitySOSHub.ToolsUsed.Where(p => p.IsActive == false && !existingToolsIds.Contains(p.ToolUsedId)));


            var existingMaterialIds = new HashSet<int>(_SOSHubForUpdate.MaterialsUsed.Select(m => m.MaterialUsedId));
            materials.AddRange(entitySOSHub.MaterialsUsed.Where(p => p.IsActive == false && !existingMaterialIds.Contains(p.MaterialUsedId)));

            //eliminar relaciones de entity bdd
            await _AnalysisProcessRepository.SOSDataRemoveAllProcessSheetCommentary(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllSections(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllCommonDirections(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllAnalysisBkups(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllToolsEquipmentMaterial(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllApproverOwners(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllReviewerEditors(entitySOSHub);
            await _AnalysisProcessRepository.SOSDataRemoveAllProducts(entitySOSHub);

            //almacenar y actualizar informacion de relaciones
            foreach (var commonD in _SOSHubForUpdate.CommonDirection)
            {
                var _commonDirection = await _AnalysisProcessRepository.UpdateCommonDirection(commonD);

                CommonDirection commonDirectionToAdd = await _AnalysisProcessRepository.GetCommonDirectionById(commonD.CommonDirectionId);
                commons.Add(commonDirectionToAdd);
            }

            foreach (var commentary in _SOSHubForUpdate.ProcessSheetCommentary)
            {
                var CommentaryUpdate = await _AnalysisProcessRepository.UpdateCommentary(commentary);

                Commentary CommentaryToAdd = await _AnalysisProcessRepository.GetCommentaryById(commentary.CommentaryId);
                ProcessSheetCommentaries.Add(CommentaryToAdd);
            }

            foreach (var analysisBkup in _SOSHubForUpdate.AnalysesBkup)
            {
                var analysisBkUpdate = await _AnalysisProcessRepository.UpdateAnalysisBkup(analysisBkup);

                AnalysisBkup analysisBkToAdd = await _AnalysisProcessRepository.GetAnalysisBkupId(analysisBkup.AnalysisBkupId);
                AnalysisBkups.Add(analysisBkToAdd);

            }

            foreach (var section in _SOSHubForUpdate.Sections)
            {
                var SecUpdate = await _AnalysisProcessRepository.UpdateSection(section);

                Section sectionToAdd = await _AnalysisProcessRepository.GetSectionById(section.SectionId);
                Sections.Add(sectionToAdd);
            }

            foreach (var material in _SOSHubForUpdate.MaterialsUsed)
            {
                var MatUsedUpdate = await _AnalysisProcessRepository.UpdateMaterialUsed(material);

                MaterialUsed mataux = await _AnalysisProcessRepository.GetMaterialUsedById(material.MaterialUsedId);
                materials.Add(mataux);
            }

            foreach (var tool in _SOSHubForUpdate.ToolsUsed)
            {
                var ToolUsedUpdate = await _AnalysisProcessRepository.UpdateToolUsed(tool);

                ToolUsed toolaux = await _AnalysisProcessRepository.GetToolUsedById(tool.ToolUsedId);
                tools.Add(toolaux);
            }

            foreach (var equipment in _SOSHubForUpdate.SafetyEquipment)
            {
                Equipment equipmentaux = await _AnalysisProcessRepository.GetEquipmentById(equipment.EquipmentId);
                equipments.Add(equipmentaux);
            }

            foreach (var user in _SOSHubForUpdate.ApproverOwners)
            {
                User useraux = await _AnalysisProcessRepository.GetUserById(user.UserId);
                usersApproverOwners.Add(useraux);
            }

            foreach (var user in _SOSHubForUpdate.ReviewerEditors)
            {
                User useraux = await _AnalysisProcessRepository.GetUserById(user.UserId);
                usersReviewerEditors.Add(useraux);
            }

            foreach (var model in _SOSHubForUpdate.AppliedModels)
            {
                Product modelaux = await _AnalysisProcessRepository.GetProductById(model.ProductId);
                applyModels.Add(modelaux);
            }

            _SOSHubForUpdate.ProcessSheetCommentary = null;
            _SOSHubForUpdate.AnalysesBkup = null;
            _SOSHubForUpdate.Sections = null;
            _SOSHubForUpdate.ToolsUsed = null;
            _SOSHubForUpdate.MaterialsUsed = null;
            _SOSHubForUpdate.SafetyEquipment = null;
            _SOSHubForUpdate.CommonDirection = null;
            _SOSHubForUpdate.ApproverOwners = null;
            _SOSHubForUpdate.ReviewerEditors = null;
            _SOSHubForUpdate.AppliedModels = null;

            //if (_SOSHubForUpdate.ApproverOwnerId <= 0)
            //{
            //    _SOSHubForUpdate.ApproverOwnerId = null;
            //}
            //if (_SOSHubForUpdate.ReviewerEditorId <= 0)
            //{
            //    _SOSHubForUpdate.ReviewerEditorId = null;
            //}
            //update base entity
            if (entitySOSHub.AreaId == 0) entitySOSHub.AreaId = null;
            if (entitySOSHub.DepartmentId == 0) entitySOSHub.DepartmentId = null;
            if (entitySOSHub.DistributionId == 0) entitySOSHub.DistributionId = null;
            if (_SOSHubForUpdate.DistributionId == 0) _SOSHubForUpdate.DistributionId = null;
            if (entitySOSHub.PlantId == 0) entitySOSHub.PlantId = null;
            if (entitySOSHub.StationId == 0) entitySOSHub.StationId = null;
            //se envia la distribucion id en null para que no genere conflicto al crear la sos hub
            if (entitySOSHub.DistributionId != null) entitySOSHub.DistributionId = null;
            var result = await _AnalysisProcessRepository.UpdateSOSHub(_SOSHubForUpdate, entitySOSHub);

            //restore all relationships
            //ProcessSheetCommentaries
            if (ProcessSheetCommentaries.Any())
            {
                foreach (Commentary Comment in ProcessSheetCommentaries)
                {
                    _AnalysisProcessRepository.AddProcessSheetCommentaryToSOSCollection(entitySOSHub, Comment);
                }
            }
            //Sections
            if (Sections.Any())
            {
                foreach (Section sec in Sections)
                {
                    await _AnalysisProcessRepository.AddSectionSOSCollection(entitySOSHub, sec);
                }
            }
            //Analysis Backups
            if (AnalysisBkups.Any())
            {
                foreach (AnalysisBkup analysisBk in AnalysisBkups)
                {
                    await _AnalysisProcessRepository.AddAnaysisBkupToSOSCollection(entitySOSHub, analysisBk);
                }
            }
            //Tools
            if (tools.Any())
            {
                foreach (ToolUsed tool in tools)
                {
                    await _AnalysisProcessRepository.AddToolToSOSCollection(entitySOSHub, tool);
                }
            }
            //Materials
            if (materials.Any())
            {
                foreach (MaterialUsed material in materials)
                {
                    await _AnalysisProcessRepository.AddMaterialToSOSCollection(entitySOSHub, material);
                }
            }
            //Equipments
            if (equipments.Any())
            {
                foreach (Equipment equipment in equipments)
                {
                    await _AnalysisProcessRepository.AddEquipmentToSOSCollection(entitySOSHub, equipment);
                }
            }
            //Common Directions
            if (commons.Any())
            {
                await _AnalysisProcessRepository.AddCommonDirectionsToSOSCollection(entitySOSHub, commons);
            }
            //Common Directions
            if (commons.Any())
            {
                await _AnalysisProcessRepository.AddCommonDirectionsToSOSCollection(entitySOSHub, commons);
            }

            //Common usersApproverOwners

            if (usersApproverOwners.Any())
            {
                foreach (User userApprover in usersApproverOwners)
                {
                    await _AnalysisProcessRepository.AddApproverOwnersToSOSCollection(entitySOSHub, userApprover);
                }
            }
            //Common usersReviewerEditors
            if (usersReviewerEditors.Any())
            {
                foreach (User userReviewer in usersReviewerEditors)
                {
                    await _AnalysisProcessRepository.AddReviewerEditorToSOSCollection(entitySOSHub, userReviewer);
                }
            }
            //ApplyModels
            if (applyModels.Any())
            {
                foreach (Product model in applyModels)
                {
                    await _AnalysisProcessRepository.AddProductToSOSCollection(entitySOSHub, model);
                }
            }

            //await _AnalysisProcessRepository.AddHistoryToSOSCollection(entitySOSHub, sOSHubHistory);

            //await _AnalysisProcessRepository.SaveChangesAsync();

            if (result != null)
            {
                return Ok(entitySOSHub);
            }
            else
                return BadRequest();

        }

        static string CompareAndGenerateJson(SOSHubForUpdateDto obj1, SOSHubForUpdateDto obj2)
        {
            var compareLogic = new CompareLogic
            {
                Config = new ComparisonConfig
                {
                    CompareChildren = true,
                    MaxDifferences = int.MaxValue
                }
            };

            ComparisonResult result = compareLogic.Compare(obj1, obj2);

            var differencesList = new List<DifferenceDetail>();

            foreach (var difference in result.Differences)
            {
                var differenceDetail = new DifferenceDetail
                {
                    Property = difference.PropertyName,
                    Before = difference.Object1Value?.ToString(),
                    After = difference.Object2Value?.ToString()
                };
                differencesList.Add(differenceDetail);
            }

            ValueConverter<List<DifferenceDetail>, string> jsonListConverter = new ValueConverter<List<DifferenceDetail>, string>(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<DifferenceDetail>>(v)
                    );

            string jsonResult = (string)jsonListConverter.ConvertToProvider(differencesList);
            return jsonResult;

        }

        public class DifferenceDetail
        {
            public string Property { get; set; }
            public string Before { get; set; }
            public string After { get; set; }
        }

        [HttpDelete("{SOSHubId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSHubId)
        {
            var SOSHub = await _AnalysisProcessRepository.GetSOSHub(SOSHubId);

            var result = await _AnalysisProcessRepository.RemoveSOSHub(SOSHubId);

            if (result > 0)
                return Ok(SOSHub);
            else
                return BadRequest("something wrong");
        }

        #region UploadFiles

        [HttpPost("Image/{pool_id}")]
        public async Task<ActionResult<FileUpload>> UploadImage(int pool_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Images", trustedFileNameForStorage);
            // Asegurarse de que el directorio de destino exista
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }


            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;
            uploadResult.IsActive = true;

            var fileToReturn = await _AnalysisProcessRepository.CreateFileAsync(uploadResult);

            await _AnalysisProcessRepository.AddImageToSOSData(pool_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpPost("Video/{pool_id}")]
        public async Task<ActionResult<FileUpload>> UploadVideo(int pool_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Videos", trustedFileNameForStorage);
            // Asegurarse de que el directorio de destino exista
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }


            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;
            uploadResult.IsActive = true;

            var fileToReturn = await _AnalysisProcessRepository.CreateFileAsync(uploadResult);

            await _AnalysisProcessRepository.AddVideoToSOSData(pool_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpPost("CD/{pool_id}")]
        public async Task<ActionResult<FileUpload>> UploadCD(int pool_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\CommonDirection", trustedFileNameForStorage);
            // Asegurarse de que el directorio de destino exista
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }


            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;
            uploadResult.IsActive = true;

            var fileToReturn = await _AnalysisProcessRepository.CreateFileAsync(uploadResult);

            await _AnalysisProcessRepository.AddImageToSOSData(pool_id, fileToReturn);
            await _AnalysisProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Image/{fileid}")]
        public async Task<IActionResult> DownloadImage(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Images", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");
        }

        [HttpGet("Video/{fileid}")]
        public async Task<IActionResult> DownloadVideo(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\Videos", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");
        }

        [HttpGet("CD/{fileid}")]
        public async Task<IActionResult> DownloadCD(int fileid)
        {
            var FileInfo = await _AnalysisProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSData\\CommonDirection", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;


                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;


            }
            return NotFound("Error File download");

        }
        #endregion

        [HttpDelete("Image/{pool_id}/remove/{fileUploadId}")]
        public async Task<ActionResult<int>> RemoveImage(int pool_id, int fileUploadId)
        {
            var result = await _AnalysisProcessRepository.RemoveImageFromSOSData(pool_id, fileUploadId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }

        [HttpDelete("Video/{pool_id}/remove/{fileUploadId}")]
        public async Task<ActionResult<int>> RemoveVideo(int pool_id, int fileUploadId)
        {
            var result = await _AnalysisProcessRepository.RemoveVideoFromSOSData(pool_id, fileUploadId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }

        [HttpDelete("CD/{pool_id}/remove/{fileUploadId}")]
        public async Task<ActionResult<int>> RemoveCD(int pool_id, int fileUploadId)
        {
            var result = await _AnalysisProcessRepository.RemoveCDFromSOSData(pool_id, fileUploadId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }
        /// Subir y borrar documento common direction


        //History
        [HttpGet("{id}/History", Name = "GetSOSHubHistory")]

        public async Task<ActionResult<List<SOSHubDto>>> GetSOSHubHistory(int id, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {

            var SOSHubs = await _AnalysisProcessRepository.GetAllHistorySOSHub(id, includeAnalysesBkup, includeSections, includeImages, includeVideos, includeCommentaries, includeTools, includeEquipments, includeMaterials, includeInformation, includePeople, includeDocuments);
            if (SOSHubs == null)
            {
                return NotFound("SOSHub History not found!");
            }

            return Ok(_mapper.Map<List<SOSHubDto>>(SOSHubs));
        }
    }// End SOS Data pool controller
}//end namespace


