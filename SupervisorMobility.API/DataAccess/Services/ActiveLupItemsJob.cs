using AutoMapper;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Services;
using System.Net.Mail;

namespace SupervisorMobility.API.DataAccess.Services
{

    public class ActiveLupItemsJob : IJob
    {
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityService;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public ActiveLupItemsJob(
            IAssyChartService assyChartService,
            ISupervisorMobilityRepository supervisorMobilityService,
            IEmailService emailService,
            IWebHostEnvironment env
            )
        {
            _assyChartService = assyChartService;
            _supervisorMobilityService = supervisorMobilityService;
            _emailService = emailService;
            _env = env;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Job executed !!!!!");
            Console.ResetColor();

            var _allJobObservations = await _supervisorMobilityService.GetAllJobObservationsAsync(includePeople: true, includeLup: true);
            if (_allJobObservations != null)
            {
                var filteredJobObservations = _allJobObservations
                    .Where(j => j.Lup.Any(l => l.IsActive == true && (l.Status == 1 || l.Status == 2)))
                    .ToList();

                var supervisorLupCounts = filteredJobObservations
                 .GroupBy(j => new { j.SupervisorId, j.Supervisor?.Name, j.Supervisor?.SuperiorId, j.Supervisor?.Email })
                 .Select(g => new
                 {
                     SupervisorId = g.Key.SupervisorId,
                     SupervisorName = g.Key.Name,
                     SuperiorId = g.Key.SuperiorId,
                     SupervisorEmail = g.Key.Email,
                     ActiveLupCount = g.Sum(j => j.Lup.Count(l => l.IsActive == true && (l.Status == 1 || l.Status == 2)))
                 })
                 .ToList();

                foreach (var supervisor in supervisorLupCounts)
                {
                    string notificationText = supervisor.ActiveLupCount == 1
                                        ? $"Supervisor {supervisor.SupervisorName} has 1 LUP item active at {DateTime.Now:hh:mm tt}"
                                        : $"Supervisor {supervisor.SupervisorName} has {supervisor.ActiveLupCount} LUP items active at {DateTime.Now:hh:mm tt}";

                    NotificationToCreateDto newnotify = new NotificationToCreateDto
                    {
                        MadeBy = "SM Mobility",
                        UserId = supervisor.SupervisorId.Value,
                        IsAccepted = true,
                        IsActive = true,
                        NotificationText = notificationText,
                        NotificationType = "Active Lup Item"
                    };

                    var response = await _assyChartService.CreateNotificationAsync(newnotify);

                    if (response != null)
                    {
                        try
                        {
                            if (_env.IsDevelopment())
                            {
                                var emailMessageError = _emailService.CreateEmailMessage("pmunoz@gruposinco.com.mx", "Active Lup Item", notificationText);
                                _emailService.Send(emailMessageError);
                            }
                            else
                            {
                                var emailMessageError = _emailService.CreateEmailMessage(supervisor.SupervisorEmail, "Active Lup Item", notificationText);
                                _emailService.Send(emailMessageError);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Notification created for Supervisor {supervisor.SupervisorName}");
                        Console.ResetColor();
                    }

                    if (supervisor.SuperiorId.HasValue)
                    {
                        NotificationToCreateDto newnotifyForSSV = new NotificationToCreateDto
                        {
                            MadeBy = "SM Mobility",
                            UserId = supervisor.SuperiorId.Value,
                            IsAccepted = true,
                            IsActive = true,
                            NotificationText = notificationText,
                            NotificationType = "Active Lup Item"
                        };

                        var responseForSSV = await _assyChartService.CreateNotificationAsync(newnotifyForSSV);

                        if (responseForSSV != null)
                        {

                            User SSV = await _supervisorMobilityService.GetUserAsync(supervisor.SuperiorId.Value);
                            if (SSV != null)
                            {
                                try
                                {
                                    if (_env.IsDevelopment())
                                    {
                                        var emailMessageError = _emailService.CreateEmailMessage("pmunoz@gruposinco.com.mx", "Active Lup Item SSV", notificationText);
                                        _emailService.Send(emailMessageError);
                                    }
                                    else
                                    {
                                        var emailMessageError = _emailService.CreateEmailMessage(SSV.Email, "Active Lup Item", notificationText);
                                        _emailService.Send(emailMessageError);
                                    }
                                }
                                catch
                                {
                                    throw;
                                }
                            }

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Notification created for Senior Supervisor of {supervisor.SupervisorName}");
                            Console.ResetColor();
                        }
                    }
                }
            }
        }
    }

}
