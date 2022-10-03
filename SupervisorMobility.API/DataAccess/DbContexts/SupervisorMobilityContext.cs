using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Context
{
    public class SupervisorMobilityContext : DbContext
    {
        #region DbSets
        public DbSet<ChecklistCategory> ChecklistCategories { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<ChecklistQuestion> ChecklistQuestions { get; set; }
        public DbSet<JobObservationConfig> JobObservationConfigs { get; set; }
        public DbSet<JobObservationType> JobObservationTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Area> Areas { get; set; }
        #endregion

        public SupervisorMobilityContext(DbContextOptions<SupervisorMobilityContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Default values
            modelBuilder.Entity<ChecklistCategory>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<QuestionType>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<ChecklistQuestion>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<JobObservationType>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Area>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            //Constraints
            modelBuilder.Entity<ChecklistCategory>()
                .HasCheckConstraint("ck_cc_seq", "[Sequence] > 0");

            modelBuilder.Entity<ChecklistQuestion>()
                .HasCheckConstraint("ck_cq_seq", "[CategorySequence] > 0");


            //seeding some data
            modelBuilder.Entity<ChecklistCategory>()
                .HasData(
                new ChecklistCategory("PO", "Preparación de la Observación")
                {
                    ChecklistCategoryId = 1,
                    Sequence = 1,
                    IsActive = true
                },
                new ChecklistCategory("OPCE", "Observación para el cumplimiento del estándar - Observación de lejos")
                {
                    ChecklistCategoryId = 2,
                    Sequence = 2,
                    IsActive = true
                },
                new ChecklistCategory("ATO", "Análisis de tiempo de operación")
                {
                    ChecklistCategoryId = 3,
                    Sequence = 3,
                    IsActive = true
                },
                new ChecklistCategory("OCE", "Observación para cumplimiento del estándar - Observación de cerca")
                {
                    ChecklistCategoryId = 4,
                    Sequence = 4,
                    IsActive = true
                },
                new ChecklistCategory("OMEFE", "Observación para mejora del estándar de acuerdo al filtro elegido")
                {
                    ChecklistCategoryId = 5,
                    Sequence = 5,
                    IsActive = true
                },
                new ChecklistCategory("TOSF", "Trabajo de Observación  - Sumario / Finalización")
                {
                    ChecklistCategoryId = 6,
                    Sequence = 6,
                    IsActive = true
                });
            modelBuilder.Entity<QuestionType>()
                .HasData(
                new QuestionType("TXT", "Free text")
                {
                    QuestionTypeId = 1,
                    IsActive = true
                },
                new QuestionType("MC", "Multiple Choice")
                {
                    QuestionTypeId = 2,
                    IsActive = true
                },
                new QuestionType("NMB", "Number")
                {
                    QuestionTypeId = 3,
                    IsActive = true
                },
                new QuestionType("Date", "Date")
                {
                    QuestionTypeId = 4,
                    IsActive = true
                },
                new QuestionType("TM", "Time")
                {
                    QuestionTypeId = 5,
                    IsActive = true
                },
                new QuestionType("TF", "Si/No")
                {
                    QuestionTypeId = 6,
                    IsActive = true
                });
            modelBuilder.Entity<ChecklistQuestion>()
                .HasData(
                new ChecklistQuestion("PO:ECA", "Estandares completos y actualizados", "Los estándares estan completos y actualizados (HOE, Estado de referencia de 5S, etc. Icluyendo la pasada observación de operación  (S/N)")
                {
                    QuestionID = 1,
                    CategorySequence = 1,
                    IsActive = true,
                    ChecklistCategoryId = 1,
                    QuestionTypeId = 6

                },
                new ChecklistQuestion("PO:NIO", "Nivel ILU del operador", "¿Cuál es nivel de ILU del operador?  ¿Está el entrenamiento alineado con el Cuadro de requisitos de Operaicón ? (S/N)")
                {
                    QuestionID = 2,
                    CategorySequence = 2,
                    IsActive = true,
                    ChecklistCategoryId = 1,
                    QuestionTypeId = 6

                });
            modelBuilder.Entity<JobObservationType>()
                .HasData(
                new JobObservationType("JC", "Observación de Operación Cíclica")
                {
                    JobObservationTypeId = 1,
                    IsActive = true
                },
                new JobObservationType("JNC", "Observación de Operación No Cíclica")
                {
                    JobObservationTypeId = 2,
                    IsActive = true
                });
            modelBuilder.Entity<JobObservationConfig>()
                .HasData(
                new JobObservationConfig()
                {
                    JobObservationConfigId = 1,
                    JobObservationTypeId = 1,
                    ChecklistCategoryId = 1
                },
                new JobObservationConfig()
                {
                    JobObservationConfigId = 2,
                    JobObservationTypeId = 1,
                    ChecklistCategoryId = 2
                },
                new JobObservationConfig()
                {
                    JobObservationConfigId = 3,
                    JobObservationTypeId = 1,
                    ChecklistCategoryId = 3
                },
                new JobObservationConfig()
                {
                    JobObservationConfigId = 4,
                    JobObservationTypeId = 1,
                    ChecklistCategoryId = 4
                },
                new JobObservationConfig()
                {
                    JobObservationConfigId = 5,
                    JobObservationTypeId = 1,
                    ChecklistCategoryId = 5
                });
            modelBuilder.Entity<Group>()
                .HasData(
                new Group("GA", "Grupo A")
                {
                    GroupId = 1,
                    IsActive = true
                },
                new Group("GB", "Grupo B")
                {
                    GroupId = 2,
                    IsActive = true
                });
            modelBuilder.Entity<Plant>()
                .HasData(
                new Plant("T&C", "Trim and Chassis")
                {
                    PlantId = 1,
                    IsActive = true
                },
                new Plant("Paint", "Paint")
                {
                    PlantId = 2,
                    IsActive = true
                });
            modelBuilder.Entity<Area>()
                .HasData(
                new Area("T1", "Trim 1")
                {
                    AreaId = 1,
                    IsActive = true,
                    PlantId = 1
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
