using Hangfire;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SiteChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var con = ConfigurationSettings.AppSettings.Get("HangFireConnectionString");
            GlobalConfiguration.Configuration.UseSqlServerStorage(con);

            using (var server = new BackgroundJobServer())
            {
                RecurringJob.AddOrUpdate(
                    () => CheckIfAlive(),
                    Cron.Minutely);
                Console.ReadKey();
            }
        }

        public static void CheckIfAlive()
        {
            //send get req
            var webClient = new WebClient();
            var site = ConfigurationSettings.AppSettings.Get("SiteToCheck");
            try
            {
                
                var result = webClient.DownloadString($"{site}/api/IsSiteAlive");
            }
            catch
            {
                try
                {

                    var from = ConfigurationSettings.AppSettings.Get("EmailFrom");
                    var password = ConfigurationSettings.AppSettings.Get("EmailPassword");
                    var userName = ConfigurationSettings.AppSettings.Get("EmailUserName");
                    var port = Convert.ToInt32(ConfigurationSettings.AppSettings.Get("EmailPort"));
                    var host = ConfigurationSettings.AppSettings.Get("EmailHost");
                    var to = ConfigurationSettings.AppSettings.Get("EmailAdminAddress");


                    MailMessage mail = new MailMessage(from, to);
                    SmtpClient mailClient = new SmtpClient();
                    mailClient.Port = port;
                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = new System.Net.NetworkCredential(from, password);
                    mailClient.Host = host;
                    mail.Subject = $"Site {site} is down";
                    mail.Body = "In case, site is to responding contact support service";
                    mailClient.Send(mail);
                   
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}
