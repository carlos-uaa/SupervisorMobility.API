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
        public DbSet<JobObservation> JobObservations { get; set; }
        public DbSet<Lup> Lup { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Glosary> Glosary { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Distribution> Distributions { get; set; }

        public DbSet<Operation> Operations { get; set; }
        public DbSet<SupportDocumentType> SupportDocumentTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        //Add AssyCharts db Context
        public DbSet<AssyChart> AssyCharts { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<FileUpload> Files { get; set; }
        public DbSet<Guides> Guides { get; set; }
        public DbSet<JobObservationVersion> JobObservationHistory { get; set; }
        public DbSet<Notification> Notifications { get; set; }

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

            modelBuilder.Entity<JobObservation>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Plant>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Area>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Distribution>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Operation>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<SupportDocumentType>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Product>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Glosary>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Lup>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<AssyChart>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Guides>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Notification>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<JobObservationVersion>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            //Add AssyChartModel



            //Constraints
            modelBuilder.Entity<ChecklistCategory>()
                .HasCheckConstraint("ck_cc_seq", "[Sequence] > 0");

            modelBuilder.Entity<ChecklistQuestion>()
                .HasCheckConstraint("ck_cq_seq", "[CategorySequence] > 0");


            //seeding some data
            modelBuilder.Entity<JobObservation>()
                .HasData(
                new JobObservation()
                {
                    JobObservationId = 1,
                    IsActive = true,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = 1,
                    OperationId = 1,
                    Type = 2,
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now,
                    DateFinalized = DateTime.Now,
                    Status = 1,
                    Option = 1,
                    SupervisorId = 1,
                    OperatorId = 2,
                    Time1HOE = "10 min",
                    Time2HOE = "20 min",
                    Models = "1|1|1|1|1",
                    Cicles = "3000|2500|3000|4000|1500",
                    SsvCommentary = "Senior Supervisor Commentary",
                    OperatorCommentary = "Operator Commentary",
                    SsvSignature = "",
                    OperatorSignature = ""

                }); ; ; ;

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
            modelBuilder.Entity<Distribution>()
                .HasData(
                new Distribution("Dist1", "Distribution 1 Trim 1")
                {
                    DistributionId = 1,
                    IsActive = true,
                    AreaId = 1
                });



            modelBuilder.Entity<Operation>()
                .HasData(
                new Operation("OP1", "Operacion Trim 1")
                {
                    OperationId = 1,
                    IsActive = true,
                    DistributionId = 1
                });


            modelBuilder.Entity<SupportDocumentType>()
                .HasData(
                new SupportDocumentType("GOS", "GOS")
                {
                    SupportDocumentTypeId = 1,
                    IsActive = true
                });
            modelBuilder.Entity<SupportDocumentType>()
                .HasData(
                new SupportDocumentType("HOE", "HOE")
                {
                    SupportDocumentTypeId = 2,
                    IsActive = true
                });

            modelBuilder.Entity<Product>()
                .HasData(
                new Product("P71A", "Infiniti P71A")
                {
                    ProductId = 1,
                    IsActive = true
                });

            modelBuilder.Entity<Product>()
                .HasData(
                new Product("X247", "Mercedes X247")
                {
                    ProductId = 3,
                    IsActive = true
                });

            modelBuilder.Entity<AssyChart>()
                .HasData(
                new AssyChart()
                {
                    AssyChardId = 1,
                    IsActive = true,
                    GOS = "01. PRESS/01. MANUFACTURA/01. X247",
                    CCP = "01. PRESS/01. CCP",
                    HOE = "1§01. PRESS/5§01. CALIDAD",
                    CreationDate = DateTime.Parse("2023-02-25T12:55:58.303-06:00"),
                    ModificationDate = new DateTime(),
                    ProductId = 1,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = 1,
                    OperationId = 1
                });

            modelBuilder.Entity<Glosary>()
            .HasData(
                new Glosary()
                {
                    GlosaryWordId = 1,
                    Name = "S",
                    Description = "Safety Pillar",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 2,
                    Name = "Q",
                    Description = "Quality Pillar",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 3,
                    Name = "D",
                    Description = "Delivery Pillar",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 4,
                    Name = "C",
                    Description = "Cost Pillar",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 5,
                    Name = "Other",
                    Description = "Other",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 6,
                    Name = "SSV",
                    Description = "Senior Supervisor",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 7,
                    Name = "SV",
                    Description = "Supervisor",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 8,
                    Name = "Lup",
                    Description = "Listado único de problemas",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 9,
                    Name = "Tiempo ciclo",
                    Description = "Tiempo ciclo de la operación por modelo",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 10,
                    Name = "Tiempo de la HOE",
                    Description = "Tiempo ciclo de la operación por modelo",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 11,
                    Name = "Manejo de la anomalía",
                    Description = "Seguimiento de anomalías",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 12,
                    Name = "Eventual",
                    Description = "Observación de la operación eventual",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 13,
                    Name = "Planeada",
                    Description = "Observación de la operación planeada",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 14,
                    Name = "Assy Chart",
                    Description = "Listado de distribución-Operación por etapa y planta",
                    IsActive = true
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    PlantId = 1,
                    AreaId = 1,
                    GroupId = 1,
                    Name = "Pedro",
                    ObjectId = "4f54e317-1ab9-45ec-9b72-7d1910a0cc88",
                    Payroll = 12345,
                    IsActive = true,
                    IsAdmin = false,
                    IsOperator = false,
                    IsSupervisor = true,
                },
                new User
                {
                    UserId = 2,
                    PlantId = 1,
                    AreaId = 1,
                    GroupId = 1,
                    Name = "Marco",
                    ObjectId = "4f54e317",
                    Payroll = 239935,
                    IsActive = true,
                    IsAdmin = false,
                    IsOperator = true,
                    IsSupervisor = false,
                },
                new User
                {
                    UserId = 3,
                    PlantId = 1,
                    AreaId = 1,
                    GroupId = 1,
                    ObjectId = "7a184926-2f58-4f9c-872c-97d54d825912",
                    Name = "Marco Aguayo",
                    Payroll = 0906,
                    IsActive = true,
                    IsAdmin = true,
                    IsOperator = true,
                    IsSupervisor = true,
                }
                );

            modelBuilder.Entity<Lup>()
                 .HasData(
                     new Lup()
                     {
                         LupId = 1,
                         JobObservationId = 1,
                         Oportunity = "Operator need a safety helmet",
                         IsActive = true,
                         Observer = "Pedro",
                         Pillar = 1,
                         Q3 = "contramedida inmediata",
                         Q4 = "contramedida definitiva",
                         Status = 1,
                         CreatedDate = DateTime.Now,
                         EndDate = DateTime.Now
                     });

            modelBuilder.Entity<Notification>()
                .HasData(
                    new Notification()
                    {
                        NotificationID = 1,
                        EntryDate = DateTime.Parse("2023-02-25T12:55:58.303-06:00"),
                        IsAccepted = true,
                        IsActive = true,
                        MadeBy = "Marco Aguayo",
                        UserId = 3,
                        NotificationType = "info",
                        NotificationText = "Example of notify"
                    },
                    new Notification()
                    {
                        NotificationID = 2,
                        EntryDate = DateTime.Now,
                        IsAccepted = true,
                        IsActive = true,
                        MadeBy = "Marco Aguayo",
                        UserId = 3,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Active and not read"
                    },
                    new Notification()
                    {
                        NotificationID = 3,
                        EntryDate = DateTime.Now,
                        IsAccepted = false,
                        IsActive = true,
                        MadeBy = "Marco Aguayo",
                        UserId = 3,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Active and Read"
                    },
                    new Notification()
                    {
                        NotificationID = 4,
                        EntryDate = DateTime.Now,
                        IsAccepted = true,
                        IsActive = false,
                        MadeBy = "Marco Aguayo",
                        UserId = 3,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Read and delete"
                    },
                    new Notification()
                    {
                        NotificationID = 5,
                        EntryDate = DateTime.Now,
                        IsAccepted = false,
                        IsActive = false,
                        MadeBy = "Marco Aguayo",
                        UserId = 3,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Read and delete"
                    });


            base.OnModelCreating(modelBuilder);
        }
    }
}
