using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.Logger;
using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.Entities;
using System.Globalization;

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
        public DbSet<Entities.Group> Groups { get; set; }
        public DbSet<Glosary> Glosary { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Distribution> Distributions { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<SupportDocumentType> SupportDocumentTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AssyChart> AssyCharts { get; set; }
        public DbSet<User> Users { get; set; }
        
        public DbSet<FileUpload> Files { get; set; }
        public DbSet<Guides> Guides { get; set; }
        public DbSet<JobObservationVersion> JobObservationHistory { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Logger> DataLoggs { get; set; }
        public DbSet<LogEvent> LogEvents { get; set; }
        public DbSet<LogSpecificEvent> LogSepecificEvents { get; set; }
        public DbSet<ILULevel> ILULevels { get; set; }
        public DbSet<ILURegister> ILURegisters { get; set; }
        public DbSet<PAT> PATs { get; set; }
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

            //Users
            modelBuilder.Entity<User>()
             .Property(p => p.IsActive)
             .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .UseIdentityColumn();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Areas)
                .WithMany(a => a.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserArea",
                    r => r.HasOne<Area>().WithMany().HasForeignKey("AreaId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    e =>
                    {
                        e.ToTable("UserAreas");
                        e.HasKey("UserId", "AreaId");
                    }
                );


         
            //area
            
            modelBuilder.Entity<Area>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Logger>()
               .Property(D => D.LogId)
               .UseIdentityColumn();

            modelBuilder.Entity<LogEvent>()
              .Property(e => e.LogEventId)
              .UseIdentityColumn();

     

            modelBuilder.Entity<LogSpecificEvent>()
              .Property(e => e.LogSpecificEventId)
              .UseIdentityColumn();

            modelBuilder.Entity<PAT>()
              .Property(e => e.PATid)
              .UseIdentityColumn();

            modelBuilder.Entity<PAT>()
              .Property(p => p.IsActive)
              .HasDefaultValue(true);

            modelBuilder.Entity<PAT>()
               .HasOne(p => p.Area)
               .WithMany()
               .HasForeignKey(p => p.AreaId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PAT>()
                .HasOne(p => p.SSVresponsible)
                .WithMany()
                .HasForeignKey(p => p.SSVresponsibleID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<PAT>()
                .HasOne(p => p.Supervisor)
                .WithMany()
                .HasForeignKey(p => p.SupervisorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PAT>()
                .HasOne(p => p.Distribution)
                .WithMany()
                .HasForeignKey(p => p.DistributionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ILULevel>()
           .Property(e => e.ILULevelId)
           .UseIdentityColumn();

            modelBuilder.Entity<ILURegister>()
           .Property(e => e.ILURegisterid)
           .UseIdentityColumn();

            modelBuilder.Entity<Notification>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<JobObservationVersion>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            //Constraints
            modelBuilder.Entity<ChecklistCategory>()
                .HasCheckConstraint("ck_cc_seq", "[Sequence] > 0");

            modelBuilder.Entity<ChecklistQuestion>()
                .HasCheckConstraint("ck_cq_seq", "[CategorySequence] > 0");


            DateTime startDateFormat;
            DateTime endDateFormat;

            var startDate = DateTime.Now.ToShortDateString() + " 12:00:00";
            var endDate = DateTime.Now.ToShortDateString() + " 13:00:00";

            if (DateTime.TryParseExact(startDate, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out startDateFormat))
            {
                Console.WriteLine(startDateFormat);
            }
            else
                Console.WriteLine("Unable to parse '{0}'", startDate);


            if (DateTime.TryParseExact(endDate, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out endDateFormat))
            {
                Console.WriteLine(endDateFormat);
            }
            else
                Console.WriteLine("Unable to parse '{0}'", endDate);

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
                    StartDate = startDateFormat,
                    EndDate = endDateFormat,
                    PlannedStartDate = startDateFormat,
                    PlannedEndDate = endDateFormat,
                    FinishedDate = null,
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

                });

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
            modelBuilder.Entity<Entities.Group>()
                .HasData(
                new Entities.Group("GA", "Grupo A")
                {
                    GroupId = 1,
                    IsActive = true
                },
                new Entities.Group("GB", "Grupo B")
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
                },
                new Area("T2", "Trim 2")
                {
                    AreaId = 2,
                    IsActive = true,
                    PlantId = 1
                }, new Area("P1", "Paint 1")
                {
                    AreaId = 3,
                    IsActive = true,
                    PlantId = 2
                }, new Area("P1", "Paint 2")
                {
                    AreaId = 4,
                    IsActive = true,
                    PlantId = 2
                });
            modelBuilder.Entity<Distribution>()
                .HasData(
                new Distribution("Dist1", "Distribution 1 Trim 1")
                {
                    DistributionId = 1,
                    IsActive = true,
                    AreaId = 1
                }, new Distribution("Dist2", "Distribution 2 Trim 2")
                {
                    DistributionId = 2,
                    IsActive = true,
                    AreaId = 2
                },
                new Distribution("P1 Dist 3", "Distribution 1 Paint 1")
                {
                    DistributionId = 3,
                    IsActive = true,
                    AreaId = 3
                },
                new Distribution("P2 Dist 4", "Distribution 2 Pint 2")
                {
                    DistributionId = 4,
                    IsActive = true,
                    AreaId = 4
                }
                );



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
                    Description = "Unique list of problems",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 9,
                    Name = "Cycle time",
                    Description = "Operation cycle time by model",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 10,
                    Name = "HOE Time",
                    Description = "Operation cycle time by model",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 11,
                    Name = "Management of the anomaly",
                    Description = "Anomaly tracking",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 12,
                    Name = "Eventual",
                    Description = "Observation of the eventual operation",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 13,
                    Name = "Planeada",
                    Description = "Observation of the planned operation",
                    IsActive = true
                },
                new Glosary()
                {
                    GlosaryWordId = 14,
                    Name = "Assy Chart",
                    Description = "Distribution listing-Operation by stage and plant",
                    IsActive = true
                }
            );

            

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Pedro",
                    Email = "pmunozsinco@compasdcpcs.local",
                    Payroll = null,
                    IsActive = true,
                    UserType = 1
                },
                new User
                {
                    UserId = 2,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Marco",
                    ObjectId = "4f54e317",
                    Payroll = 239935,
                    IsActive = true,
                    UserType = 4

                },
                new User
                {
                    UserId = 3,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    ObjectId = "bb5a1c89-b35e-482a-8e92-931917221add",
                    Name = "Marco Aguayo",
                    Payroll = 0906,
                    IsActive = true,
                    UserType = 1,
                }
                , new User
                {
                    UserId = 4,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "SeniorSupervisor",
                    ObjectId = "4fe317",
                    Payroll = 4,
                    IsActive = true,
                    UserType = 2,
                    SuperiorId = 3
                }, new User
                {
                    UserId = 5,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Supervisor",
                    ObjectId = "4f5317",
                    Payroll = 5,
                    IsActive = true,
                    UserType = 3,
                    SuperiorId = 4,

                }, new User
                {
                    UserId = 6,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Operador 1",
                    ObjectId = "4f54e7",
                    Payroll = 6,
                    IsActive = true,
                    UserType = 4,
                    SuperiorId = 5,
                },
                new User
                {
                    UserId = 7,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Operador 2",
                    ObjectId = "4f54e7",
                    Payroll = 7,
                    IsActive = true,
                    UserType = 4,
                    SuperiorId = 1,
                },
                new User
                {
                    UserId = 8,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Operador 3",
                    ObjectId = "4f54e7",
                    Payroll = 8,
                    IsActive = true,
                    UserType = 4,
                    SuperiorId = 1,
                },
                new User
                {
                    UserId = 9,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Operador 4",
                    ObjectId = "4f54e7",
                    Payroll = 9,
                    IsActive = true,
                    UserType = 4,
                    SuperiorId = 1,
                },
                new User
                {
                    UserId = 10,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "pmunoz@gruposinco.com.mx",
                    ObjectId = "7f223478-bd34-445b-b662-0f64caa9998c",
                    Payroll = 10,
                    IsActive = true,
                    UserType = 1,
                    SuperiorId = 1,
                }
                );;

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
                    },
                    new Notification()
                    {
                        NotificationID = 6,
                        EntryDate = DateTime.Now,
                        IsAccepted = true,
                        IsActive = true,
                        MadeBy = "Marco Aguayo",
                        UserId = 1,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Active and not read"
                    },
                    new Notification()
                    {
                        NotificationID = 7,
                        EntryDate = DateTime.Now,
                        IsAccepted = false,
                        IsActive = true,
                        MadeBy = "Marco Aguayo",
                        UserId = 1,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Active and Read"
                    },
                    new Notification()
                    {
                        NotificationID = 8,
                        EntryDate = DateTime.Now,
                        IsAccepted = true,
                        IsActive = false,
                        MadeBy = "Marco Aguayo",
                        UserId = 1,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Read and delete"
                    },
                    new Notification()
                    {
                        NotificationID = 9,
                        EntryDate = DateTime.Now,
                        IsAccepted = false,
                        IsActive = false,
                        MadeBy = "Marco Aguayo",
                        UserId = 1,
                        NotificationType = "Supervisor",
                        NotificationText = "Example of notify Read and delete"
                    });


            base.OnModelCreating(modelBuilder);
        }
    }
}
