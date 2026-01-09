namespace SupervisorMobility.API.TestingsDtos
{
    public class OperationsDto
    {
       
        public double ManualOperationTime { get; set; }
        public double ManualOperationTimeWithMachineInAutomatic { get; set; }
        public double AutomaticMachineOperationTime { get; set; }
        public double StepsToNextProcess { get; set; }

    }
}
