using AutoMapper;
using Moq;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.HRITests.HRItemsTest;

namespace Tests.HRITests.HRIRevisionItemsTest
{
    public class HRIRevisionItemsInMemoryTest
    {
        [Test]
        public async Task GetAllFrecuencies()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;

            var repository = new HRIRevisionItemRepository(context, _service, _mapper);

            //agregamos una frecuencia a la base de datos
            await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });

            var allFrecuencies = await repository.GetAllFrequencies();

            //assert
            Assert.IsNotNull(allFrecuencies);
            Assert.That(allFrecuencies.Data!.Count, Is.EqualTo(1)); // Verifica que solo hay una frecuencia en la base de datos

        }

        [Test]
        public async Task GetFrecuencyById()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos una frecuencia a la base de datos
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var frequencyId = createdFrequency.Data!.Id;
            var retrievedFrequency = await repository.GetFrequencyById(frequencyId);
            //assert
            Assert.IsNotNull(retrievedFrequency);
            Assert.That(retrievedFrequency.Data!.Id, Is.EqualTo(frequencyId)); // Verifica que la frecuencia recuperada tiene el mismo ID que la creada

        }

        [Test]
        public async Task CreateFrecuency()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos una frecuencia a la base de datos
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            //assert
            Assert.IsNotNull(createdFrequency);
            Assert.That(createdFrequency.Data!.Code, Is.EqualTo("Frecuencia 1")); // Verifica que el código de la frecuencia creada es correcto
            Assert.That(createdFrequency.Data.Description, Is.EqualTo("Descripción de la frecuencia 1")); // Verifica que la descripción de la frecuencia creada es correcta

        }

        [Test]
        public async Task UpdateFrequency()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos una frecuencia a la base de datos
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var frequencyId = createdFrequency.Data!.Id;
            //actualizamos la frecuencia
            var updatedFrequency = await repository.UpdateFrequency(frequencyId, new UpdateFrequencyDto
            {
                Code = "Frecuencia 1 actualizada",
                Description = "Descripción de la frecuencia 1 actualizada",

            });
            //assert
            Assert.IsNotNull(updatedFrequency);
            Assert.That(updatedFrequency.Data!.Code, Is.EqualTo("Frecuencia 1 actualizada")); // Verifica que el código de la frecuencia actualizada es correcto
            Assert.That(updatedFrequency.Data.Description, Is.EqualTo("Descripción de la frecuencia 1 actualizada")); // Verifica que la descripción de la frecuencia actualizada es correcta
        }
        [Test]
        public async Task DeleteFrequency()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos una frecuencia a la base de datos
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var frequencyId = createdFrequency.Data!.Id;
            //eliminamos la frecuencia
            var deleteResult = await repository.DeleteFrequency(frequencyId);
            //assert
            Assert.IsNotNull(deleteResult);
            Assert.That(deleteResult.Success, Is.True); // Verifica que la eliminación fue exitosa
        }

        [Test]
        public async Task GetAllVeredicts()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un veredicto a la base de datos
            await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var allVeredicts = await repository.GetAllVeredicts();
            //assert
            Assert.IsNotNull(allVeredicts);
            Assert.That(allVeredicts.Data!.Count, Is.EqualTo(1)); // Verifica que solo hay un veredicto en la base de datos

        }

        [Test]
        public async Task GetVeredictById()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un veredicto a la base de datos
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var veredictId = createdVeredict.Data!.Id;
            var retrievedVeredict = await repository.GetVeredictById(veredictId);
            //assert
            Assert.IsNotNull(retrievedVeredict);
            Assert.That(retrievedVeredict.Data!.Id, Is.EqualTo(veredictId)); // Verifica que el veredicto recuperado tiene el mismo ID que el creado
        }

        [Test]
        public async Task CreateVeredict()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un veredicto a la base de datos
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            //assert
            Assert.IsNotNull(createdVeredict);
            Assert.That(createdVeredict.Data!.Code, Is.EqualTo("Veredicto 1")); // Verifica que el código del veredicto creado es correcto
            Assert.That(createdVeredict.Data.Description, Is.EqualTo("Descripción del veredicto 1")); // Verifica que la descripción del veredicto creado es correcta
        }

        [Test]
        public async Task UpdateVeredict()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un veredicto a la base de datos
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var veredictId = createdVeredict.Data!.Id;
            //actualizamos el veredicto
            var updatedVeredict = await repository.UpdateVeredict(veredictId, new UpdateVeredictDto
            {
                Code = "Veredicto 1 actualizado",
                Description = "Descripción del veredicto 1 actualizado",
            });
            //assert
            Assert.IsNotNull(updatedVeredict);
            Assert.That(updatedVeredict.Data!.Code, Is.EqualTo("Veredicto 1 actualizado")); // Verifica que el código del veredicto actualizado es correcto
            Assert.That(updatedVeredict.Data.Description, Is.EqualTo("Descripción del veredicto 1 actualizado")); // Verifica que la descripción del veredicto actualizado es correcta
        }

        [Test]
        public async Task DeleteVeredict()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un veredicto a la base de datos
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var veredictId = createdVeredict.Data!.Id;
            //eliminamos el veredicto
            var deleteResult = await repository.DeleteVeredict(veredictId);
            //assert
            Assert.IsNotNull(deleteResult);
            Assert.That(deleteResult.Success, Is.True); // Verifica que la eliminación fue exitosa
        }

        [Test]
        public async Task GetAllRevisionMethods()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un método de revisión a la base de datos
            await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            var allRevisionMethods = await repository.GetAllRevisionMethods();
            //assert
            Assert.IsNotNull(allRevisionMethods);
            Assert.That(allRevisionMethods.Data!.Count, Is.EqualTo(1)); // Verifica que solo hay un método de revisión en la base de datos
        }

        [Test]
        public async Task GetRevisionMethodById()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un método de revisión a la base de datos
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            var revisionMethodId = createdRevisionMethod.Data!.Id;
            var retrievedRevisionMethod = await repository.GetRevisionMethodById(revisionMethodId);
            //assert
            Assert.IsNotNull(retrievedRevisionMethod);
            Assert.That(retrievedRevisionMethod.Data!.Id, Is.EqualTo(revisionMethodId)); // Verifica que el método de revisión recuperado tiene el mismo ID que el creado
        }

        [Test]
        public async Task CreateRevisionMethod()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un método de revisión a la base de datos
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            //assert
            Assert.IsNotNull(createdRevisionMethod);
            Assert.That(createdRevisionMethod.Data!.Code, Is.EqualTo("Método de revisión 1")); // Verifica que el código del método de revisión creado es correcto
            Assert.That(createdRevisionMethod.Data.Description, Is.EqualTo("Descripción del método de revisión 1")); // Verifica que la descripción del método de revisión creado es correcta
        }

        [Test]
        public async Task UpdateRevisionMethod()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un método de revisión a la base de datos
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            var revisionMethodId = createdRevisionMethod.Data!.Id;
            //actualizamos el método de revisión
            var updatedRevisionMethod = await repository.UpdateRevisionMethod(revisionMethodId, new UpdateRevisionMethodDto
            {
                Code = "Método de revisión 1 actualizado",
                Description = "Descripción del método de revisión 1 actualizado",
            });
            //assert
            Assert.IsNotNull(updatedRevisionMethod);
            Assert.That(updatedRevisionMethod.Data!.Code, Is.EqualTo("Método de revisión 1 actualizado")); // Verifica que el código del método de revisión actualizado es correcto
            Assert.That(updatedRevisionMethod.Data.Description, Is.EqualTo("Descripción del método de revisión 1 actualizado")); // Verifica que la descripción del método de revisión actualizado es correcta
        }

        [Test]
        public async Task DeleteRevisionMethod()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //agregamos un método de revisión a la base de datos
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            var revisionMethodId = createdRevisionMethod.Data!.Id;
            //eliminamos el método de revisión
            var deleteResult = await repository.DeleteRevisionMethod(revisionMethodId);
            //assert
            Assert.IsNotNull(deleteResult);
            Assert.That(deleteResult.Success, Is.True); // Verifica que la eliminación fue exitosa

        }
        [Test]
        public async Task GetAllHRIRevisionItems()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var cycleRepository = new HRIRevisionCyclesRepository(context, _mapper, _notificationService);
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //creamos el veredicto, la frecuencia y el método de revisión necesarios para crear un ítem de revisión
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            // creamos el HRI necesario para crear un ítem de revisión
            await context.HRIs.AddAsync(new HRI
            {
                HriId = 1,
                HRILinesId = 1,
                HRIItemId = 1,
                ControlNumber = "Control 1",
                HRIDockId = 1,
                Department = "Department 1",
                SupervisorUserId = 1,
                SSVUserId = 12,
                PlantId = 1,
                AreaId = 1,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            //agregamos un ítem de revisión a la base de datos
            await repository.CreateHRIRevisionItem(new CreateHRIRevisionItemDto
            {
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Punto de revisión 1",
                RevisionMethodId = createdRevisionMethod.Data!.Id,
                VeredictId = createdVeredict.Data!.Id,
                FrequencyId = createdFrequency.Data!.Id,
                IsActive = true
            });
            var allRevisionItems = await repository.GetAllHRIRevisionItems();
            //assert
            Assert.IsNotNull(allRevisionItems);
            Assert.That(allRevisionItems.Data!.Count, Is.EqualTo(1)); // Verifica que solo hay un ítem de revisión en la base de datos
        }
        [Test]
        public async Task GetHRIRevisionItemById()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var cycleRepository = new HRIRevisionCyclesRepository(context, _mapper, _notificationService);
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //creamos el veredicto, la frecuencia y el método de revisión necesarios para crear un ítem de revisión
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            // creamos el HRI necesario para crear un ítem de revisión
            await context.HRIs.AddAsync(new HRI
            {
                HriId = 1,
                HRILinesId = 1,
                HRIItemId = 1,
                ControlNumber = "Control 1",
                HRIDockId = 1,
                Department = "Department 1",
                SupervisorUserId = 1,
                SSVUserId = 12,
                PlantId = 1,
                AreaId = 1,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            //agregamos un ítem de revisión a la base de datos
            var createdRevisionItem = await repository.CreateHRIRevisionItem(new CreateHRIRevisionItemDto
            {
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Punto de revisión 1",
                RevisionMethodId = createdRevisionMethod.Data!.Id,
            });

            //obtenemos el ítem de revisión por su ID
            var revisionItem = await repository.GetHRIRevisionItemById(createdRevisionItem.Data!.ItemId);
            Assert.IsNotNull(revisionItem);
            Assert.That(revisionItem.Data!.ItemId, Is.EqualTo(createdRevisionItem.Data!.ItemId));
        }

        [Test]
        public async Task CreateHRIRevisionItem()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var cycleRepository = new HRIRevisionCyclesRepository(context, _mapper, _notificationService);
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //creamos el veredicto, la frecuencia y el método de revisión necesarios para crear un ítem de revisión
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            // creamos el HRI necesario para crear un ítem de revisión
            await context.HRIs.AddAsync(new HRI
            {
                HriId = 1,
                HRILinesId = 1,
                HRIItemId = 1,
                ControlNumber = "Control 1",
                HRIDockId = 1,
                Department = "Department 1",
                SupervisorUserId = 1,
                SSVUserId = 12,
                PlantId = 1,
                AreaId = 1,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            //agregamos un ítem de revisión a la base de datos
            var createdRevisionItem = await repository.CreateHRIRevisionItem(new CreateHRIRevisionItemDto
            {
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Punto de revisión 1",
                RevisionMethodId = createdRevisionMethod.Data!.Id,
                VeredictId = createdVeredict.Data!.Id,
                FrequencyId = createdFrequency.Data!.Id,
                IsActive = true
            });
            //assert
            Assert.IsNotNull(createdRevisionItem);
            Assert.That(createdRevisionItem.Data!.HriId, Is.EqualTo(1)); // Verifica que el HRI ID del ítem de revisión creado es correcto
            Assert.That(createdRevisionItem.Data.ItemNumber, Is.EqualTo(1)); // Verifica que el número de ítem del ítem de revisión creado es correcto
        }

        [Test]
        public async Task CreateHRIREvisionItemsByHRIId()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var cycleRepository = new HRIRevisionCyclesRepository(context, _mapper, _notificationService);
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //creamos el veredicto, la frecuencia y el método de revisión necesarios para crear un ítem de revisión
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            // creamos el HRI necesario para crear un ítem de revisión
            await context.HRIs.AddAsync(new HRI
            {
                HriId = 1,
                HRILinesId = 1,
                HRIItemId = 1,
                ControlNumber = "Control 1",
                HRIDockId = 1,
                Department = "Department 1",
                SupervisorUserId = 1,
                SSVUserId = 12,
                PlantId = 1,
                AreaId = 1,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            //agregamos un item de revision con el metodo que crea multiples items de revision por HRIId
            var createdRevisionItems = await repository.CreateHRIREvisionItemsByHRIId(1, new List<CreateHRIRevisionItemDto>
            {
                new CreateHRIRevisionItemDto
                {
                    ItemNumber = 1,
                    RevisionPoint = "Punto de revisión 1",
                    RevisionMethodId = createdRevisionMethod.Data!.Id,
                    VeredictId = createdVeredict.Data!.Id,
                    FrequencyId = createdFrequency.Data!.Id,
                    IsActive = true
                }

            }, 1);

            //assert
            Assert.IsNotNull(createdRevisionItems);
            Assert.That(createdRevisionItems.Data!, Is.EqualTo(true));
        }

        [Test]
        public async Task GetAllHRIRevisionItemsByHRIId()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var cycleRepository = new HRIRevisionCyclesRepository(context, _mapper, _notificationService);
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //creamos el veredicto, la frecuencia y el método de revisión necesarios para crear un ítem de revisión
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            // creamos el HRI necesario para crear un ítem de revisión
            await context.HRIs.AddAsync(new HRI
            {
                HriId = 1,
                HRILinesId = 1,
                HRIItemId = 1,
                ControlNumber = "Control 1",
                HRIDockId = 1,
                Department = "Department 1",
                SupervisorUserId = 1,
                SSVUserId = 12,
                PlantId = 1,
                AreaId = 1,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            //agregamos un ítem de revisión a la base de datos
            await repository.CreateHRIRevisionItem(new CreateHRIRevisionItemDto
            {
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Punto de revisión 1",
                RevisionMethodId = createdRevisionMethod.Data!.Id,
                VeredictId = createdVeredict.Data!.Id,
                FrequencyId = createdFrequency.Data!.Id,
                IsActive = true
            });
            //obtenemos los ítems de revisión por el ID del HRI
            var revisionItems = await repository.GetAllHRIRevisionItemsByHRIId(1);
            //assert
            Assert.IsNotNull(revisionItems);
            Assert.That(revisionItems.Data!.Count, Is.EqualTo(1)); // Verifica que solo hay un ítem de revisión para el HRI con ID 1

        }

        [Test]
        public async Task DeleteHRIRevisionItem()
        {
            var context = new GetInMemoryDBContext().GetInMemoryDbContext();
            var _mapper = new MockMapperCreation().GetMockMapper().Object;
            var _service = new Mock<IHRIRevisionCyclesRepository>().Object;
            var _notificationService = new Mock<INotificationService>().Object;
            var cycleRepository = new HRIRevisionCyclesRepository(context, _mapper, _notificationService);
            var repository = new HRIRevisionItemRepository(context, _service, _mapper);
            //creamos el veredicto, la frecuencia y el método de revisión necesarios para crear un ítem de revisión
            var createdVeredict = await repository.CreateVeredict(new CreateVeredictDto
            {
                Code = "Veredicto 1",
                Description = "Descripción del veredicto 1",
                IsActive = true
            });
            var createdFrequency = await repository.CreateFrequency(new CreateFrequencyDto
            {
                Code = "Frecuencia 1",
                Description = "Descripción de la frecuencia 1",
                IsActive = true
            });
            var createdRevisionMethod = await repository.CreateRevisionMethod(new CreateRevisionMethodDto
            {
                Code = "Método de revisión 1",
                Description = "Descripción del método de revisión 1",
                IsActive = true
            });
            // creamos el HRI necesario para crear un ítem de revisión
            await context.HRIs.AddAsync(new HRI
            {
                HriId = 1,
                HRILinesId = 1,
                HRIItemId = 1,
                ControlNumber = "Control 1",
                HRIDockId = 1,
                Department = "Department 1",
                SupervisorUserId = 1,
                SSVUserId = 12,
                PlantId = 1,
                AreaId = 1,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            //agregamos un ítem de revisión a la base de datos
            var createdRevisionItem = await repository.CreateHRIRevisionItem(new CreateHRIRevisionItemDto
            {
                HriId = 1,
                ItemNumber = 1,
                RevisionPoint = "Punto de revisión 1",
                RevisionMethodId = createdRevisionMethod.Data!.Id,
                VeredictId = createdVeredict.Data!.Id,
                FrequencyId = createdFrequency.Data!.Id,
                IsActive = true
            });
            var revisionItemId = createdRevisionItem.Data!.ItemId;
            //eliminamos el ítem de revisión
            var deleteResult = await repository.DeleteHRIRevisionItem(revisionItemId);
            //assert
            Assert.NotNull(deleteResult);
            Assert.That(deleteResult.Success, Is.True); // Verifica que la eliminación fue exitosa
        }

    }
}
