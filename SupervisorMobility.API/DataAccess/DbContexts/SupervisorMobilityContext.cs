using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.Logger;
using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.Entities;
using System.Globalization;

namespace SupervisorMobility.API.Context
{
    public class SupervisorMobilityContext : DbContext
    {
        #region DbSets
        public DbSet<HeadCount> headCounts { get; set; }
        public DbSet<JobCategoryStructure> JobCategoryStructures { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<ChecklistQuestion> ChecklistQuestions { get; set; }
   
        public DbSet<JobObservation> JobObservations { get; set; }
        public DbSet<Lup> Lup { get; set; }
        public DbSet<ChecklistAnswer> ChecklistAnswers { get; set; }
        public DbSet<Entities.Group> Groups { get; set; }
        public DbSet<Pillar> Pillars { get; set; }
        public DbSet<Glosary> Glosary { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Distribution> Distributions { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<SupportDocumentType> SupportDocumentTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AssyChart> AssyCharts { get; set; }
        public DbSet<SOSCodePath> CodePaths { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserNotFound> UsersNotFound { get; set; }

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

        public DbSet<SOSReviewProgram> SOSReviews { get; set; }
        public DbSet<SOSRegisterJobObservation> SOSRegisters { get; set; }
        public DbSet<SOSRegUserOperation> SOSRegsUserOperation { get; set; }

        #endregion



        public SupervisorMobilityContext(DbContextOptions<SupervisorMobilityContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Default values
            modelBuilder.Entity<JobCategoryStructure>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<SOSReviewProgram>()
               .Property(p => p.IsActive)
               .HasDefaultValue(true);

            modelBuilder.Entity<QuestionType>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<ChecklistQuestion>()
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

            modelBuilder.Entity<Department>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Lup>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<AssyChart>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<SOSCodePath>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Guides>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<UserNotFound>()
                 .Property(p => p.IsActive)
                 .HasDefaultValue(true);
            //Users
            modelBuilder.Entity<User>()
             .Property(p => p.IsActive)
             .HasDefaultValue(true);

            modelBuilder.Entity<User>()
            .Property(u => u.AreaId)
            .IsRequired(false);

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
              .Property(p => p.IsActive)
              .HasDefaultValue(true);

            modelBuilder.Entity<PAT>()
               .HasOne(p => p.Area)
               .WithMany()
               .HasForeignKey(p => p.AreaId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PAT>()
              .HasOne(p => p.Plant)
              .WithMany()
              .HasForeignKey(p => p.PlantId)
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

            modelBuilder.Entity<ILURegister>()
                .Property(p => p.isActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Notification>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);


            modelBuilder.Entity<JobObservationVersion>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Pillar>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            //Constraints
            modelBuilder.Entity<JobCategoryStructure>()
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


            modelBuilder.Entity<JobCategoryStructure>()
                .HasData(
                new JobCategoryStructure("Title", "Job Observation Title Card")
                {
                    JobCategoryStructureId = 1,
                    Sequence = 1,
                    IsActive = true,
                    Type = StructureType.Titular
                },
                new JobCategoryStructure("OPCE", "Observación para el cumplimiento del estándar - Observación de lejos")
                {
                    JobCategoryStructureId = 2,
                    Sequence = 2,
                    IsActive = true,
                    Type = StructureType.Checklist
                },
                new JobCategoryStructure("ATO", "Análisis de tiempo de operación")
                {
                    JobCategoryStructureId = 3,
                    Sequence = 3,
                    IsActive = true,
                    Type = StructureType.Timer
                },
                new JobCategoryStructure("OCE", "Observación para cumplimiento del estándar - Observación de cerca")
                {
                    JobCategoryStructureId = 4,
                    Sequence = 4,
                    IsActive = true,
                    Type = StructureType.Checklist
                },
                new JobCategoryStructure("TOSF", "Trabajo de Observación  - Sumario / Finalización")
                {
                    JobCategoryStructureId = 5,
                    Sequence = 5,
                    IsActive = true,
                    Type = StructureType.Checklist
                },
                 new JobCategoryStructure("OMEFE", "Observación para mejora del estándar de acuerdo al filtro elegido")
                 {
                    JobCategoryStructureId = 6,
                    Sequence = 6,
                    IsActive = true,
                     Type = StructureType.Signature
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
                new Product("N71A", "Infinity N71A")
                {
                    ProductId = 2,
                    IsActive = true
                });

            modelBuilder.Entity<Product>()
                .HasData(
                new Product("X247", "Mercedes X247")
                {
                    ProductId = 3,
                    IsActive = true
                });



            modelBuilder.Entity<Glosary>()
            .HasData(
                new Glosary()
                {
                    GlosaryWordId = 1,
                    Name = "S & E",
                    Description = "Safety & Environment Pillar",
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

            modelBuilder.Entity<Department>()
              .HasData(
                  new Department()
                  {
                      DepartmentId = 1,
                      Code = "MFG",
                      Description = "Manufactura",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 2,
                      Code = "IDE",
                      Description = "Ingeniería de equipos",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 3,
                      Code = "II",
                      Description = "Ingenieria Industrial",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 4,
                      Code = "PROD",
                      Description = "Producción",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 5,
                      Code = "LF",
                      Description = "Linia Final",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 6,
                      Code = "VQA",
                      Description = "VQA",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 7,
                      Code = "PQA",
                      Description = "PQA",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 8,
                      Code = "CDP",
                      Description = "Control de producción",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 9,
                      Code = "MANT",
                      Description = "Mantenimiento",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 10,
                      Code = "PRV",
                      Description = "Procesivo",
                      IsActive = true
                  },
                  new Department()
                  {
                      DepartmentId = 11,
                      Code = "SG",
                      Description = "Servicios generales",
                      IsActive = true
                  }
              );


            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    ObjectId = "marco.aguayo@compasdcpcs.local",
                    Name = "Marco Aguayo",
                    Email = "maguayo@gruposinco.com.mx",
                    IsActive = true,
                    UserType = 1
                },
                new User
                {
                    UserId = 2,
                    ObjectId = "maguayosinco@compasdcpcs.local",
                    Name = "M. Aguayo Sinco",
                    Email = "maguayo@gruposinco.com.mx",
                    IsActive = true,
                    UserType = 1,
                },
                new User
                {
                    UserId = 3,
                    ObjectId = "pmunozsinco@compasdcpcs.local",
                    Name = "Pedro",
                    Email = "pmunoz@gruposinco.com.mx",
                    IsActive = true,
                    UserType = 1
                }
                , new User
                {
                    UserId = 4,
                    ObjectId = "SSV@compasdcpcs.local",
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "SeniorSupervisor",
                    Payroll = 4,
                    IsActive = true,
                    UserType = 2,
                }, new User
                {
                    UserId = 5,
                    ObjectId = "SV@compasdcpcs.local",
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Supervisor",
                    Payroll = 5,
                    IsActive = true,
                    UserType = 3,
                    SuperiorId = 3,

                }, new User
                {
                    UserId = 6,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Operador 1",
                    Payroll = 6,
                    IsActive = true,
                    UserType = 4,
                    SuperiorId = 4,
                },
                new User
                {
                    UserId = 7,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Operador 2",
                    Payroll = 7,
                    IsActive = true,
                    UserType = 4,
                    SuperiorId = 4,
                },
                new User
                {
                    UserId = 8,
                    PlantId = 1,
                    AreaId = 1,
                    DistributionId = null,
                    GroupId = 1,
                    Name = "Operador 3",
                    Payroll = 8,
                    IsActive = true,
                    UserType = 4,
                    SuperiorId = 4,
                }
                ); ;



            modelBuilder.Entity<Notification>()
                .HasData(
                    new Notification()
                    {
                        NotificationID = 1,
                        EntryDate = DateTime.Parse("2023-02-25T12:55:58.303-06:00"),
                        IsAccepted = true,
                        IsActive = true,
                        MadeBy = "Marco Aguayo",
                        UserId = 2,
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
                        UserId = 2,
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
                        UserId = 2,
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
                        UserId = 2,
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

            modelBuilder.Entity<ILULevel>()
                .HasData(
                        new ILULevel()
                        {
                            ILULevelId = 1,
                            ILULevelCode = 'I',
                            ILULevelDescription = "el operador necesita entrenamiento para realizar la operación",
                            isActive = true
                        },
                        new ILULevel()
                        {
                            ILULevelId = 2,
                            ILULevelCode = 'L',
                            ILULevelDescription = "el operador ya la puede realizar por si mismo",
                            isActive = true
                        }, new ILULevel()
                        {
                            ILULevelId = 3,
                            ILULevelCode = 'U',
                            ILULevelDescription = "el operador domina la operación y puede enseñar",
                            isActive = true
                        }
                );

            modelBuilder.Entity<Pillar>()
                .HasData(
                new Pillar("S & E", "Safety & Environment")
                {
                    PillarId = 1,
                    IsActive = true
                },
                new Pillar("Q", "Quality")
                {
                    PillarId = 2,
                    IsActive = true
                },
                new Pillar("D", "Delivery")
                {
                    PillarId = 3,
                    IsActive = true
                },
                new Pillar("C", "Cost")
                {
                    PillarId = 4,
                    IsActive = true
                },
                new Pillar("Other", "Other")
                {
                    PillarId = 5,
                    IsActive = true
                }
            );

            modelBuilder.Entity<ChecklistQuestion>()
                .HasData(
                new ChecklistQuestion()
                {
                    QuestionID = 1,
                    PillarId = 2,
                    Prompt = "Respeta pasos principales y puntos críticos",
                    NotGood = "No Respeta pasos principales y puntos críticos",
                    CategorySequence = 1,
                    IsActive = true,
                    JobCategoryStructureId = 1,

                },
                new ChecklistQuestion()
                {
                    QuestionID = 2,
                    PillarId = 2,
                    Prompt = "Empaque, herramientas, manipuladores están en buenas condiciones y no hay riesgos de calidad",
                    NotGood = "El empaque, herramientas, manipuladores no están en buenas condiciones y hay riesgos de calidad",
                    CategorySequence = 2,
                    IsActive = true,
                    JobCategoryStructureId = 1,

                });

            //modelBuilder.Entity<ChecklistAnswer>()
            //    .HasData(
            //    new ChecklistAnswer()
            //    {
            //        AnswerId = 1,
            //        JobObservationId = 1,
            //        QuestionID = 1,
            //        Prompt = "Respeta pasos principales y puntos críticos",
            //        Answer = "YES",

            //    });

            base.OnModelCreating(modelBuilder);
        }
    }
}
