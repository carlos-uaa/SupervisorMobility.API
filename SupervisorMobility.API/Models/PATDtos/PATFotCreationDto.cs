namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATFotCreationDto
    {
        public int PATid { get; set; }

        public int SupervisorId { get; set; }
        public int Status { get; set; }
        public int? SSVresponsibleID { get; set; }


        public int PlantId { get; set; }
        public int AreaId { get; set; }

        //public int DistributionId { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear
        {
            get { return AplicationDate?.Year; }
            set
            {
                if (value != null)
                {
                    int year = value.Value;
                    if (!(year >= 1 && year <= 9999))
                    {
                        // Manejar el caso en el que el año está fuera del rango válido
                        // Puedes lanzar una excepción, asignar un valor predeterminado, etc.
                        // Por ejemplo:
                        throw new ArgumentOutOfRangeException("El año está fuera del rango válido.");
                    }
                }
                else
                {
                    AplicationDate = null;
                }
            }
        }

        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public int? SOSHubId { get; set; }
        public bool IsActive { get; set; }


    }
}
