using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;
using System.Net;
using System.Net.Http.Json;


namespace Tests
{
    public class ExportationTest : IDisposable
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _appFactory;
        private HttpClient _customClient;
        private CustomWebApplicationFactory _customFactory;

        [SetUp]
        public void Setup()
        {
            _appFactory = new WebApplicationFactory<Program>();
            _client = _appFactory.CreateClient();
            //customFactory y customClient
            _customFactory = new CustomWebApplicationFactory();
            _customClient = _customFactory.CreateClient();

        }


        [Test,Order(1)]
        public async Task ExportationOfARegularHOESecuence()
        {
            int SecuenceId = 1;
            var response = await _client.GetAsync($"api/Exportation/Excel/Sequence/{SecuenceId}");           
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.IsTrue(bytes.Length > 0);
        }

        [Test, Order(2)]
        public async Task ExportationOfARegularHOECombination()
        {
            int CombinationId = 1;
            var response = await _client.GetAsync($"api/Exportation/Excel/Combination/{CombinationId}");
            var res = await response.Content.ReadAsStringAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.IsTrue(bytes.Length > 0);
        }

        [Test, Order(3)]
        public async Task ExportationOfAHoeCombinationWithMaxOperations()
        {
            //obtenemos la combinacion para crear una nueva combinacion en base a esta y agregarle mas operaciones
            int CombinationId = 1;
            var response = await _client.GetAsync($"api/SOS/Combination/{CombinationId}");
            var res = await response.Content.ReadAsStringAsync();
            var combinationDto = await response.Content.ReadFromJsonAsync<SOSCombinationForCreateDto>();
            //creamos una nueva combinacion en base a la obtenida 
            var newCombinationForCreate = new SOSCombinationForCreateDto
            {
                //copiamos todos los campos y los asignamos a la nueva combinacion
                SOSCombinationId = 0, //nuevo id para crear
                IsActive = combinationDto.IsActive,
                InternalControlNumber = combinationDto.InternalControlNumber,
                OperationName = "Testing",
                ProcessName = "Testing",
                ProductionPlanAndObservations = combinationDto.ProductionPlanAndObservations,
                ReviewerHSId = combinationDto.ReviewerHSId,
                ApplicationMonth = combinationDto.ApplicationMonth,
                ProductionVolumePerShift = combinationDto.ProductionVolumePerShift,
                TackTime = combinationDto.TackTime,
                ControlNumber = combinationDto.ControlNumber,
                SOSHubId = combinationDto.SOSHubId,
                //agragamos 11 operaciones mas a la nueva combinacion
                SOSCombinationOperationSequence = new List<SOSCombinationOperationSequenceForCreateDto>() {


                    new SOSCombinationOperationSequenceForCreateDto {SequenceId=1,SectionId=1,ProcessName = "Op1",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02, IsActive = true },
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId=2,SectionId=2, ProcessName = "Op2",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02,IsActive = true },
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId=3,SectionId=3, ProcessName = "Op3",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02, IsActive = true },
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 4, SectionId = 4, ProcessName = "Op4",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 5, SectionId = 5, ProcessName = "Op5", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 6, SectionId = 6, ProcessName = "Op6", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 7, SectionId = 7, ProcessName = "Op7", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 8, SectionId = 8, ProcessName = "Op8", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 9, SectionId = 9, ProcessName = "Op9", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 10, SectionId = 10, ProcessName = "Op10", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 11, SectionId = 11, ProcessName = "Op11", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true}}
            };

            //agragamos nuestra nueva combinacion a la base virtual
            var createResponse = await _customClient.PostAsJsonAsync($"api/SOS/Combination?SOSHubCollection_Id={newCombinationForCreate.SOSHubId}", newCombinationForCreate);
            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var createdCombination = await createResponse.Content.ReadFromJsonAsync<SOSCombinationDto>();
            //ahora intentamos exportar la nueva combinacion con muchas operaciones
            var exportResponse = await _customClient.GetAsync($"api/Exportation/Excel/Combination/{createdCombination.SOSCombinationId}");
            Assert.That(exportResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(exportResponse.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            var bytes = await exportResponse.Content.ReadAsByteArrayAsync();
            Assert.IsTrue(bytes.Length > 0);
        
        }

        [Test,Order(4)]
        public async Task ExportationOfAHoeCombinationWithExceededOperations()
        {
            //obtenemos la combinacion para crear una nueva combinacion en base a esta y agregarle mas operaciones
            int CombinationId = 1;
            var response = await _client.GetAsync($"api/SOS/Combination/{CombinationId}");
            var res = await response.Content.ReadAsStringAsync();
            var combinationDto = await response.Content.ReadFromJsonAsync<SOSCombinationForCreateDto>();
            //creamos una nueva combinacion en base a la obtenida 
            var newCombinationForCreate = new SOSCombinationForCreateDto
            {
                //copiamos todos los campos y los asignamos a la nueva combinacion
                SOSCombinationId = 0, //nuevo id para crear
                IsActive = combinationDto.IsActive,
                InternalControlNumber = combinationDto.InternalControlNumber,
                OperationName = "Testing",
                ProcessName = "Testing",
                ProductionPlanAndObservations = combinationDto.ProductionPlanAndObservations,
                ReviewerHSId = combinationDto.ReviewerHSId,
                ApplicationMonth = combinationDto.ApplicationMonth,
                ProductionVolumePerShift = combinationDto.ProductionVolumePerShift,
                TackTime = combinationDto.TackTime,
                ControlNumber = combinationDto.ControlNumber,
                SOSHubId = combinationDto.SOSHubId,
                //agragamos 11 operaciones mas a la nueva combinacion
                SOSCombinationOperationSequence = new List<SOSCombinationOperationSequenceForCreateDto>() {


                    new SOSCombinationOperationSequenceForCreateDto {SequenceId=1,SectionId=1,ProcessName = "Op1",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02, IsActive = true },
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId=2,SectionId=2, ProcessName = "Op2",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02,IsActive = true },
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId=3,SectionId=3, ProcessName = "Op3",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02, IsActive = true },
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 4, SectionId = 4, ProcessName = "Op4",PartsPerCycle="1",ManualOperationTime=0.1,ManualOperationTimeWithMachineInAutomatic=0.02,AutomaticMachineOperationTime=0.3,StepsToNextProcess=0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 5, SectionId = 5, ProcessName = "Op5", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 6, SectionId = 6, ProcessName = "Op6", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 7, SectionId = 7, ProcessName = "Op7", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 8, SectionId = 8, ProcessName = "Op8", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 9, SectionId = 9, ProcessName = "Op9", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 10, SectionId = 10, ProcessName = "Op10", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 11, SectionId = 11, ProcessName = "Op11", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 12, SectionId = 12, ProcessName = "Op12", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 13, SectionId = 13, ProcessName = "Op13", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 14, SectionId = 14, ProcessName = "Op14", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true},
                    new SOSCombinationOperationSequenceForCreateDto {SequenceId = 15, SectionId = 15, ProcessName = "Op15", PartsPerCycle = "1", ManualOperationTime = 0.1, ManualOperationTimeWithMachineInAutomatic = 0.02, AutomaticMachineOperationTime = 0.3, StepsToNextProcess = 0.02, IsActive = true}
            }

            };

            //agragamos nuestra nueva combinacion a la base virtual
            var createResponse = await _customClient.PostAsJsonAsync($"api/SOS/Combination?SOSHubCollection_Id={newCombinationForCreate.SOSHubId}", newCombinationForCreate);
            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var createdCombination = await createResponse.Content.ReadFromJsonAsync<SOSCombinationDto>();
            //ahora intentamos exportar la nueva combinacion con muchas operaciones
            var exportResponse = await _customClient.GetAsync($"api/Exportation/Excel/Combination/{createdCombination.SOSCombinationId}");
            Assert.That(exportResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(exportResponse.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            var bytes = await exportResponse.Content.ReadAsByteArrayAsync();
            Assert.IsTrue(bytes.Length > 0);
        }

        [Test, Order(5)]
        public async Task ExportationOfARegularHOEDistribution()
        {
            int distributionId = 1;
            var response = await _client.GetAsync($"api/Exportation/Excel/Distribution/{distributionId}");
            var res = await response.Content.ReadAsStringAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.IsTrue(bytes.Length > 0);
        }


        [OneTimeTearDown]
        public void Dispose()
        {
            _client.Dispose();
            _appFactory.Dispose();
        }
    }
}