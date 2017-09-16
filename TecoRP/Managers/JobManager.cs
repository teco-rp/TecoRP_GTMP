using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Jobs;

namespace TecoRP.Managers
{
    public class JobManager : Script
    {
        [Command("isegir")]
        public void JoinJob(Client sender)
        {
            foreach (var item in db_Jobs.currentJobsList.Select(s => s.Item1))
            {
                if (item.Dimension == sender.dimension && Vector3.Distance(sender.position, item.Position) <= item.Range)
                {
                    if ((API.getEntityData(sender, "JobId") != item.JobId))
                    {
                        API.setEntityData(sender, "JobId", item.JobId);
                        API.sendNotificationToPlayer(sender, "Yeni bir işe girdiniz.\n~g~Hayırlı olsun~s~!");
                        switch (item.JobId)
                        {
                            case 1:
                                API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutları ~y~/otobus ~s~ yazarak görebilirsiniz.");
                                break;
                            case 2:
                                API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutları ~y~/tir ~s~ yazarak görebilirsiniz.");
                                break;
                            case 3:
                                API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutları ~y~/kamyon ~s~ yazarak görebilirsiniz.");
                                break;
                            case 4:
                                API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutları ~y~/benzinci ~s~ yazarak görebilirsiniz.");
                                break;
                            case 5:
                                API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutları ~y~/bankaci ~s~ yazarak görebilirsiniz.");
                                break;
                            case 11:
                                API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutlara ~y~/tamirci ~s~ve ~y~/tamir ~s~yazarak ulaşabilirsiniz.");
                                break;
                            case 12:
                                API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutları ~y~/taksi ~s~ yazarak görebilirsiniz.");
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "Zaten ~r~bu iştesiniz~s~.");
                    }


                }
            }
            //var _playerJob = Convert.ToInt32(API.getEntityData(sender, "JobId"));
            //if (Vector3.Distance(sender.position, db_BusJob.busJob_pos) < 3)
            //{
            //    if (!_playerJob == db_BusJob.busJob_jobId)
            //    {
            //        API.setEntityData(sender, "JobId", db_BusJob.busJob_jobId);
            //        API.sendNotificationToPlayer(sender, "Yeni bir işe girdiniz.\n~g~Hayırlı olsun~s~!");
            //        API.sendChatMessageToPlayer(sender, " * Yeni işinizle ilgili komutları ~y~/otobus ~s~ yazarak görebilirsini.");
            //    }
            //}
            //else
            //    API.sendNotificationToPlayer(sender, "Girebileceğiniz bir işin ~r~yakınında~s~ değilsiniz.");
        }

        [Command("yetenekler")]
        public void Skills(Client sender)
        {
            var skills = GetPlayerSkills(sender);
            List<string> names = new List<string>();
            List<string> descs = new List<string>();
            foreach (var itemSkill in skills)
            {
                switch (itemSkill.JobLevel)
                {
                    case 1:
                        names.Add("~m~" + itemSkill.GetJobName());
                        break;
                    case 2:
                        names.Add("~c~" + itemSkill.GetJobName());
                        break;
                    case 3:
                        names.Add("~o~" + itemSkill.GetJobName());
                        break;
                    case 4:
                        names.Add("~y~" + itemSkill.GetJobName());
                        break;
                    case 5:
                        names.Add("~g~" + itemSkill.GetJobName());
                        break;
                    default:
                        names.Add("~s~" + itemSkill.GetJobName());
                        break;
                }
                descs.Add($"Seviye: ~y~{itemSkill.JobLevel}~s~ | Tamamlanan: ~y~{itemSkill.JobsCompleted}");
            }
            Clients.ClientManager.ShowPlayerMenu(sender, names, descs);
        }
        public static List<Models.Jobs> GetPlayerSkills(Client player)
        {
            return (List<Models.Jobs>)API.shared.getEntityData(player, "JobAbilities");
        }

        public static string ToJobName(int _Id)
        {
            switch (_Id)
            {
                case 0:
                    return "Yok";
                case 1:
                    return "Otobüs Şoförü";
                case 2:
                    return "Tır Şoförü";
                case 3:
                    return "Kamyon Şoförü";
                case 4:
                    return "Benzin Taşımacı";
                case 5:
                    return "Banka Hizmeti";
                case 11:
                    return "Tamirci";
                case 12:
                    return "Taksici";
                case 21:
                    return "Hırsız";
                case 22:
                    return "Silah Kaçakçısı";
                case 25:
                    return "Aşçı";
                default:
                    return "";
            }
        }
        public static int GetSkillLevel(Client player, int jobId)
        {
            var skill = GetPlayerSkills(player).FirstOrDefault(x => x.JobID == jobId);
            if (skill == null) { return 0; }
            return skill.JobLevel;
        }
        public static void PlayerJobComplete(Client sender, int jobId)
        {
            List<Models.Jobs> playerJobs = API.shared.getEntityData(sender, "JobAbilities");
            if (playerJobs == null) { playerJobs = new List<Models.Jobs>(); }
            var _job = playerJobs.FirstOrDefault(x => x.JobID == jobId);
            if (_job == null)
            {
                playerJobs.Add(new Models.Jobs
                {
                    JobID = jobId,
                    JobLevel = 1,
                    JobsCompleted = 1
                });
                _job = playerJobs.LastOrDefault();
            }
            else
                _job.JobsCompleted++;

            if ( _job.JobLevel < 5 && _job.JobsCompleted % (50 * _job.JobLevel ) == 0)
            {
                _job.JobLevel++;
            }

            API.shared.setEntityData(sender, "JobAbilities", playerJobs);

        }
    }
}
