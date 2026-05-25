using AutoMapper;
using Moq;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.HRITests.HRIRevisionItemsTest
{
    public class MockMapperCreation
    {
        public Mock<IMapper> GetMockMapper()
        {
            var mockMapper = new Mock<IMapper>();
            // Configuración del mapper para CreateHRIRevisionItemDto a HRIRevisionItems
            mockMapper.Setup(m => m.Map<SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionItems>(It.IsAny<SupervisorMobility.API.Models.HRIRevisionItemsDtos.CreateHRIRevisionItemDto>()))
                .Returns<SupervisorMobility.API.Models.HRIRevisionItemsDtos.CreateHRIRevisionItemDto>(dto => new SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionItems
                {
                    ItemId = new Random().Next(1, 1000), // Genera un ID aleatorio para simular la base de datos
                    HriId = dto.HriId,
                    ItemNumber = dto.ItemNumber,
                    RevisionPoint = dto.RevisionPoint,
                    RevisionMethodId = dto.RevisionMethodId,
                    VeredictId = dto.VeredictId,
                    FrequencyId = dto.FrequencyId,
                    IsActive = true,
                    CreationDate = DateTime.UtcNow
                });
            // Configuración del mapper para HRIRevisionItems a GetHRIRevisionItemDto
            mockMapper.Setup(m => m.Map<GetHRIRevisionItemDto>(It.IsAny<SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionItems>()))
                .Returns<SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionItems>(entity => new SupervisorMobility.API.Models.HRIRevisionItemsDtos.GetHRIRevisionItemDto
                {
                    ItemId = entity.ItemId,
                    HriId = entity.HriId,
                    ItemNumber = entity.ItemNumber,
                    RevisionPoint = entity.RevisionPoint,
                    RevisionMethodId = entity.RevisionMethodId,
                    VeredictId = entity.VeredictId,
                    FrequencyId = entity.FrequencyId,
                    IsActive = entity.IsActive,
                });

            //configuracion del mapper de Frequencies a GetFrequencyDto
            mockMapper.Setup(m=>m.Map<GetFrequencyDto>(It.IsAny<Frequency>()))
                .Returns<Frequency>(entity => new GetFrequencyDto
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                });

            //configuracion del mapper CreateFrequencyDto a Frequency
            mockMapper.Setup(m => m.Map<Frequency>(It.IsAny<CreateFrequencyDto>()))
                .Returns<CreateFrequencyDto>(dto => new Frequency
                {
                    Id = 1, // Genera un ID aleatorio para simular la base de datos
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = true,
                });

            //configuracion del mapper UpdateFrequencyDto a Frequency
            mockMapper.Setup(m => m.Map<Frequency>(It.IsAny<UpdateFrequencyDto>()))
                .Returns<UpdateFrequencyDto>(dto => new Frequency
                {
                    
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = true,
                });

            //configuracion del mapper de Veredict a GetVeredictDto
            mockMapper.Setup(m => m.Map<GetVeredictDto>(It.IsAny<Veredict>()))
                .Returns<Veredict>(entity => new GetVeredictDto
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                });
            //configuracion del mapper CreateVeredictDto a Veredict
            mockMapper.Setup(m => m.Map<Veredict>(It.IsAny<CreateVeredictDto>()))
                .Returns<CreateVeredictDto>(dto => new Veredict
                {
                    Id = 1, // Genera un ID aleatorio para simular la base de datos
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = true,
                });
            //configuracion del mapper UpdateVeredictDto a Veredict
            mockMapper.Setup(m => m.Map<Veredict>(It.IsAny<UpdateVeredictDto>()))
                .Returns<UpdateVeredictDto>(dto => new Veredict
                {
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = true,
                });

            //configuracion del mapper de RevisionMethod a GetRevisionMethodDto
            mockMapper.Setup(m => m.Map<GetRevisionMethodDto>(It.IsAny<RevisionMethod>()))
                .Returns<RevisionMethod>(entity => new GetRevisionMethodDto
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                });
            //configuracion del mapper CreateRevisionMethodDto a RevisionMethod
            mockMapper.Setup(m => m.Map<RevisionMethod>(It.IsAny<CreateRevisionMethodDto>()))
                .Returns<CreateRevisionMethodDto>(dto => new RevisionMethod
                {
                    Id = 1, // Genera un ID aleatorio para simular la base de datos
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = true,
                });
            //configuracion del mapper UpdateRevisionMethodDto a RevisionMethod
            mockMapper.Setup(m => m.Map<RevisionMethod>(It.IsAny<UpdateRevisionMethodDto>()))
                .Returns<UpdateRevisionMethodDto>(dto => new RevisionMethod
                {
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = true,
                });

            //configuracion del mapper de RevisionCycle a GetRevisionCycleDto
            mockMapper.Setup(m => m.Map<GetRevisionCyclesDto>(It.IsAny<RevisionCycles>()))
                .Returns<RevisionCycles>(entity => new GetRevisionCyclesDto
                {
                    RevisionCycleId = entity.RevisionCycleId,
                    Cycle = entity.Cycle,
                    IsActive = entity.IsActive,
                    HRIRevisionItemsId = entity.HRIRevisionItemsId
                });

            //configuracion del mapper CreateRevisionCycleDto a RevisionCycle
            mockMapper.Setup(m => m.Map<RevisionCycles>(It.IsAny<CreateRevisionCyclesDto>()))
                .Returns<CreateRevisionCyclesDto>(dto => new RevisionCycles
                {
                    RevisionCycleId = 1, // Genera un ID aleatorio para simular la base de datos
                    Cycle = dto.Cycle,
                    IsActive = true
                   
                });

            //configuracion del mapper HRIRevisionItems a GetHRIRevisionItemDto
            mockMapper.Setup(m => m.Map<GetHRIRevisionItemDto>(It.IsAny<HRIRevisionItems>()))
                .Returns<HRIRevisionItems>(entity => new GetHRIRevisionItemDto
                {
                    ItemId = entity.ItemId,
                    HriId = entity.HriId,
                    ItemNumber = entity.ItemNumber,
                    RevisionPoint = entity.RevisionPoint,
                    RevisionMethodId = entity.RevisionMethodId,
                    VeredictId = entity.VeredictId,
                    FrequencyId = entity.FrequencyId,
                    IsActive = entity.IsActive,
                });

            //configuracion del mapper de CreateHRIRevisionItemDto a HRIRevisionItems
            mockMapper.Setup(m => m.Map<HRIRevisionItems>(It.IsAny<CreateHRIRevisionItemDto>()))
                .Returns<CreateHRIRevisionItemDto>(dto => new HRIRevisionItems
                {
                    ItemId = 1, // Genera un ID aleatorio para simular la base de datos
                    HriId = dto.HriId,
                    ItemNumber = dto.ItemNumber,
                    RevisionPoint = dto.RevisionPoint,
                    RevisionMethodId = dto.RevisionMethodId,
                    VeredictId = dto.VeredictId,
                    FrequencyId = dto.FrequencyId,
                    IsActive = true,
                    CreationDate = DateTime.UtcNow
                });


            return mockMapper;
        }
    }
}
