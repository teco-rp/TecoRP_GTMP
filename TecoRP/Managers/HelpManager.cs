using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
namespace TecoRP.Managers
{
    public class HelpManager : Script
    {
        public const string General_Faction_Commands = "/olusumdakiler /olusumadavetet, /olusumdancik, /olusumdanat /rutbeler /rutbever";
        [Command("yardim", "/yardım [Genel/Olusum]", Alias = "help")]
        public void Help(Client sender, string type)
        {
            if ("genel".StartsWith(type.ToLower()))
            {
                API.sendChatMessageToPlayer(sender, "~y~ --OYUN KOMUTLARI--\n" +
                   "/araclarim, /kilit (L), /aracimibul, /park, /bagaj, /kaput, /para, /gir (F), /motor (Y) \n" +
                   "/arac /ev /isyeri /isyerim  /envanter (I) /envanteregerikoy,  /satinal (E), /paraver /isegir \n" +
                   "/ustunuara, /ekemer, /kemerkontrol, /kapikir /karakter, /yetenekler /saglikraporu /bagajabak /torpido \n"+
                   "/uretim /otur /yaslan /dans /evlerim"
                   );
            }
            else
                if ("olusum".StartsWith(type.ToLower()))
            {
                var playerFaction = (int)API.getEntityData(sender, "FactionId");
                switch (playerFaction)
                {
                    case 0:
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Herhangi bir oluşuma dahil değilsiniz.");
                        break;
                    case 1:
                        API.sendChatMessageToPlayer(sender, "~y~ --LSPD KOMUTLARI--\n" +
                   "/kelepcele, /kelepcecikar, /surukle /suruklebirak /ustunuara /kemerkontrol /hapseat ,/polisradyosu \n" +
                   "/rozetolustur, /parmakizi, /parmakizial /polisbilgisayarı (/pb)  /silahruhsati /ihbar /ihbarsil" +
                   General_Faction_Commands
                   );
                        break;
                    case 2:
                        API.sendChatMessageToPlayer(sender, "~y~ --"+FactionManager.ToFactionName(playerFaction)+" KOMUTLARI--\n" +
                 General_Faction_Commands);
                        break;
                    case 4:
                        API.sendChatMessageToPlayer(sender, "~y~ --" + FactionManager.ToFactionName(playerFaction) + " KOMUTLARI--\n" +
                            "/ilkyardim /tasi /aracayukle /yaraliteslimet /cagrikabulet /cagriiptalet /cagrilar \n" +
                            "/mr \n"+
                General_Faction_Commands);
                        break;
                    case 5:
                        API.sendChatMessageToPlayer(sender, "~y~ --" + FactionManager.ToFactionName(playerFaction) + " KOMUTLARI--\n" +
                           "/cy /reklamonayla (/ro) /reklamredded (/rr) \n" +
                           "/wr \n" +
               General_Faction_Commands);
                        break;
                    default:
                        break;
                }
            }
            else
            if ("telefon".StartsWith(type.ToLower()))
            {
                API.sendChatMessageToPlayer(sender, "~y~ --TELEFON KOMUTLARI--\n" +
                   "/ara, /p, /h, /sms"
                   );
            }

        }
        [Command("yardım", "/yardım [Genel/Olusum/Telefon]")]
        public void Yardim(Client sender, string type)
        {
            Help(sender, type);
        }
    }
}
