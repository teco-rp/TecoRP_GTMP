
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;

namespace TecoRP.Managers
{
    public class BusinessManager : Script
    {


        [Command("isyeri", "/isyeri [fiyat/satinal]")]
        public void BusinessBaseCommand(Client sender, string commandParam)
        {
            foreach (var itemBusiness in db_Businesses.currentBusiness.Values)
            {
                if (Vector3.Distance(sender.position, itemBusiness.Position) < 2)
                {
                    if ("fiyat".StartsWith(commandParam.ToLower()))
                    {
                        if (itemBusiness.IsSelling == true)
                        {
                            API.sendChatMessageToPlayer(sender, "Bu işyerinin fiyatı: ~g~" + itemBusiness.Price + "$");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu işyeri satılık değil.");
                        }

                    }
                    else
                    if ("satinal".StartsWith(commandParam.ToLower()))
                    {
                        if (itemBusiness.IsSelling)
                        {
                            if (String.IsNullOrEmpty(itemBusiness.OwnerSocialClubName))
                            {
                                int money = API.getEntityData(sender, "Money");
                                if (money >= itemBusiness.Price)
                                {
                                    money -= itemBusiness.Price;
                                    itemBusiness.OwnerSocialClubName = sender.socialClubName;
                                    itemBusiness.IsSelling = false;
                                    itemBusiness.IsClosed = false;
                                    db_Businesses.Update(itemBusiness);
                                    return;
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Paranız yetersiz.");
                                }

                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~İkinci el satış henüz yapılmadı.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu işyeri satılık değil.");
                        }
                    }
                    break;
                }
            }


        }

        [Command("isyerim", "/isyerim [kasa/paracek/parayatir/ac/kapa]", GreedyArg = true)]
        public void MyBusiness(Client sender, string commandParam)
        {
            foreach (var itemBusiness in db_Businesses.currentBusiness.Values)
            {
                if (Vector3.Distance(sender.position, itemBusiness.Position) < 2)
                {
                    string[] splittedParam = commandParam.Split(' ');
                    if (splittedParam.Length == 1) {
                        API.sendChatMessageToPlayer(sender, "_____" + itemBusiness.BusinessName + "_____\n" +
                            "Kasa: " + (itemBusiness.VaultMoney < itemBusiness.MaxVaultMoney * 0.1f ? "~g~" : "~r~") + itemBusiness.VaultMoney + "~s~/" + itemBusiness.MaxVaultMoney + "\n" +
                            "Durum: " + (itemBusiness.IsClosed ? "~r~KAPALI" : "~g~AÇIK")+"\n"
                            //"Saatlik Gelir: "+itemBusiness.MoneyIncomePerHour
                            );
                        return; }
                    //------------------------------------------------------------------------

                    if ("kasa".StartsWith(splittedParam[0].ToLower()))
                    {
                        API.sendChatMessageToPlayer(sender, "~b~" + itemBusiness.BusinessName + " ~s~(" + itemBusiness.VaultMoney + "~g~$ ~s~/" + itemBusiness.MaxVaultMoney + " )");
                        return;
                    }
                    else
                     if ("paracek".StartsWith(splittedParam[0].ToLower()))
                    {
                        #region Paracek
                        if (splittedParam.Length > 1)
                        {
                            int money = API.getEntityData(sender, "Money");
                            int value = Convert.ToInt32(splittedParam[1]);

                            if (value <= itemBusiness.VaultMoney)
                            {
                                money += value;
                                itemBusiness.VaultMoney -= value;
                                db_Businesses.Update(itemBusiness);
                                API.setEntityData(sender, "Money", money);
                                API.triggerClientEvent(sender, "update_money_display", money);
                                API.sendNotificationToPlayer(sender, "~g~+" + value + "$");
                                API.sendNotificationToPlayer(sender, itemBusiness.BusinessName + " adlı işyerinizin kasasında güncelleme: \n ~r~-" + value + "$");
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kasada bu kadar para bulunmuyor.");

                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "/isyerim paracek [Miktar]");
                        }
                        #endregion
                        return;
                    }
                    else
                     if ("parayatir".StartsWith(splittedParam[0].ToLower()))
                    {
                        #region Paracek
                        if (splittedParam.Length > 1)
                        {
                            int money = API.getEntityData(sender, "Money");
                            int value = Convert.ToInt32(splittedParam[1]);

                            if (money >= value)
                            {
                                if (itemBusiness.VaultMoney + value < itemBusiness.MaxVaultMoney)
                                {
                                    money -= value;
                                    itemBusiness.VaultMoney += value;
                                    db_Businesses.Update(itemBusiness);
                                    API.setEntityData(sender, "Money", money);
                                    API.triggerClientEvent(sender, "update_money_display", money);
                                    API.sendNotificationToPlayer(sender, "~y~-" + value + "$");
                                    API.sendNotificationToPlayer(sender, itemBusiness.BusinessName + " adlı işyerinizin kasasında güncelleme: \n ~g~+" + value + "$");
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~İşyerinizin kasası bu kadar alamıyor.");
                                }
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Üzerinizde bu kadar para bulunmuyor.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "/isyerim paracek [Miktar]");
                        }
                        #endregion
                        return;
                    }
                    else
                     if ("ac".StartsWith(splittedParam[0].ToLower()) && "aç".StartsWith(splittedParam[0].ToLower()))
                    {
                        if (itemBusiness.IsClosed)
                        {
                            itemBusiness.IsClosed = false;
                            API.sendChatMessageToPlayer(sender, "~b~İçyeriniz kapatıldı. Artık (varsa)satış yapamayacak ve gelir sağlamayacak.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~İşyeriniz zaten şu anda açık");
                        }
                    }
                    else
                     if ("kapa".StartsWith(splittedParam[0].ToLower()))
                    {
                        if (!itemBusiness.IsClosed)
                        {
                            itemBusiness.IsClosed = true;
                            API.sendChatMessageToPlayer(sender, "~b~İçyeriniz kapatıldı. Artık (varsa)satış yapacak ve gelir sağlayacak.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~İşyeriniz zaten şu anda kapalı.");
                        }
                    }
                    break;
                }
            }
            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~İşyerinizin yakınlarında değilsiniz.");

        }


    }
}
