using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Managers;
using TecoRP.Models;

namespace TecoRP.Users
{
    public class Animation : Script
    {
        public Animation()
        {
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "CTRL_1":
                    Handsup(sender, true);
                    break;
                case "CTRL_2":
                    SitPlayer(sender, 5);
                    break;
                case "CTRL_3":
                    SitPlayer(sender, 6);
                    break;
                case "CTRL_4":
                    LeanPlayer(sender, 1);
                    break;
                case "CTRL_5":
                    LeanPlayer(sender, 2);
                    break;
                default:
                    break;
            }
        }
        

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            CloseMobilePhone(player);
        }

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public static void OpenMobilePhone(Client sender, int phoneObjId)
        {
            CloseMobilePhone(sender);
            var phoneObj = API.shared.createObject(phoneObjId, sender.position, sender.rotation, sender.dimension);

            //X == PARMAKLARININ GÖSTERDİĞİ YÖN
            //Y == ELİNİN SAĞ TARAFI (BAŞ PARMAĞININ GÖSTERDİĞİ EKSEN


            API.shared.attachEntityToEntity(phoneObj, sender, "IK_R_Hand", new Vector3(0.05, 0, 0), new Vector3(110, 150, -20));
            API.shared.setEntityData(sender, "phone", phoneObj);
            Task.Run(async () =>
            {
                API.shared.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "cellphone@", "f_cellphone_text_in");
                await Task.Delay(700);
                API.shared.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@cellphone@in_car@ds", "cellphone_text_read_base");
            });
        }

        //[Command("rt")]
        //public void RotationPhone(Client sender, float x, float y, float z)
        //{
        //    if (API.shared.hasEntityData(sender,"phone"))
        //    {
        //        GrandTheftMultiplayer.Server.Elements.Object _phone = API.shared.getEntityData(sender, "phone");
        //        _phone.rotation = new Vector3(x, y, z);
        //        API.shared.consoleOutput("isNull : "+(_phone == null).ToString());
        //        API.setEntityRotation(_phone, new Vector3(x, y, z));
        //    }
        //}
        /// <summary>
        /// Removes if phone object exist and stops player animation. There is no control if player cuffed or dead etc.
        /// </summary>
        /// <param name="sender"></param>
        public static void CloseMobilePhone(Client sender)
        {
            API.shared.stopPlayerAnimation(sender);
            if (API.shared.hasEntityData(sender, "phone"))
            {
                API.shared.deleteEntity(API.shared.getEntityData(sender, "phone"));
            }
        }

        public static void WearBag(Client player, int bagObjId)
        {
            var bag = API.shared.getEntityData(player, "bag");
            if (bag == null)
                bag = API.shared.createObject(bagObjId, player.position, player.rotation, player.dimension);
            API.shared.attachEntityToEntity(bag, player, "IK_Root", new Vector3(0, -0.15f, 0.35f), new Vector3(0, 0, 180));
        
            API.shared.setEntityData(player, "bag", bag);
        }

        public static void WearWeapon(Client player, int weaponObjId, int weaponType)
        {
            if (weaponType > 4 || weaponType < 1) { return; }
            RemoveObjectIfHasSame(player, weaponType);
            var prop = API.shared.getEntityData(player, $"w_{weaponType}");
            if (prop == null)
                prop = API.shared.createObject(weaponObjId, API.shared.getEntityPosition(player.handle), new Vector3());
            API.shared.setEntityData(player, $"w_{weaponType}", prop); // puts key like "w_1" / "w_2"
            switch (weaponType)
            {
                case 1:
                    API.shared.attachEntityToEntity(prop, player.handle, "SKEL_Pelvis", new Vector3(0, 0, 0.24f), new Vector3(270f, 0, 0));
                    break;
                case 2:
                    API.shared.attachEntityToEntity(prop, player.handle, "SKEL_Pelvis", new Vector3(0, 0, -0.24), new Vector3(90f, 0f, 10f));
                    break;
                case 3:
                    //sırt
                    API.shared.attachEntityToEntity(prop, player.handle, "SKEL_SPINE3", new Vector3(0.25, -0.15, -0.07f), new Vector3(0f, 200f, 10f));
                    break;
                case 4:
                    //sırt ters çapraz
                    API.shared.attachEntityToEntity(prop, player.handle, "SKEL_SPINE3", new Vector3(-0.13f, -0.15, -0.07f), new Vector3(0f, 70f, -10f));
                    break;
                default:
                    break;
            }

        }
        public static void RemovePlayerWeapon(Client player, int weaponType)
        {
            if (API.shared.hasEntityData(player, $"w_{weaponType}"))
            {
                API.shared.deleteEntity(API.shared.getEntityData(player, $"w_{weaponType}"));
                API.shared.resetEntityData(player, $"w_{weaponType}");
            }
        }
        public static void RemoveObjectIfWeapon(Client player, Item _gameItem)
        {
            if (_gameItem.Type == ItemType.Weapon)
            {
                RemovePlayerWeapon(player, Convert.ToInt32(_gameItem.Value_2));
            }
        }
        public static void RemoveObjectIfHasSame(Client player, int weaponType)
        {
            if (API.shared.hasEntityData(player, $"w_{weaponType}"))
            {
                API.shared.deleteEntity(API.shared.getEntityData(player, $"w_{weaponType}"));
                API.shared.resetEntityData(player, $"w_{weaponType}");
            }
        }
        public static void DestroyPlayerWeapons(Client player)
        {
            for (int i = 1; i < 5; i++)
            {
                RemovePlayerWeapon(player, i);
            }
        }
        public static void LoadPlayerWeapons(Client player)
        {
            var _weapons = InventoryManager.GetItemsFromPlayerInventory(player, ItemType.Weapon);
            foreach (var item in _weapons)
            {
                if (!item.Item2.Equipped)
                {
                    try
                    {
                        WearWeapon(player, item.Item1.ObjectId, Convert.ToInt32(item.Item1.Value_2));
                    }
                    catch (Exception ex)
                    {
                        API.shared.consoleOutput(LogCat.Warn, "ÖNEMSİZ HATA: " + ex);
                        continue;
                    }
                }
            }
        }

        [Command("yaslan", "/yaslan [1-2]")]
        public void CMD_Lean(Client sender, int type)
        {
            LeanPlayer(sender, type);
        }
        [Command("dans", "/dans [1-5]")]
        public void CMD_Dance(Client sender, int type)
        {
            DancePlayer(sender, type);
        }
        /// <summary>
        /// Type should be between 1-6
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        public static void SitPlayer(Client sender, int type)
        {
            if (!IsPlayerAvailableForAnim(sender))
                return;

            switch (type)
            {
                case 1:
                    API.shared.playPlayerAnimation(sender, 1, "missheistdocks2aleadinoutlsdh_2a_int", "sitting_loop_floyd");
                    break;
                case 2:
                    API.shared.playPlayerAnimation(sender, 1, "random@robbery", "sit_down_idle_01");
                    break;
                case 3:
                    API.shared.playPlayerAnimation(sender, 1, "rcmjosh3", "sit_stairs_idle");
                    break;
                case 4:
                    API.shared.playPlayerAnimation(sender, 1, "switch@franklin@bye_taxi", "001938_01_fras_v2_7_bye_taxi_exit_girl");
                    break;
                case 5:
                    API.shared.playPlayerAnimation(sender, 1, "switch@michael@sitting", "idle");
                    break;
                case 6:
                    API.shared.playPlayerAnimation(sender, 1, "mp_army_contact", "idle");
                    break;
                default:
                    break;
            }

        }
        public static void Handsup(Client sender, bool walkable)
        {
            if (!IsPlayerAvailableForAnim(sender))
                return;

            if (walkable)
            {
                API.shared.setEntityData(sender, "Handsup", true);
                API.shared.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missminuteman_1ig_2", "handsup_base");
            }
            else
            {
                API.shared.setEntityData(sender, "Handsup", true);
                API.shared.playPlayerAnimation(sender, 1, "missminuteman_1ig_2", "handsup_base");
            }
        }
        public static void LeanPlayer(Client sender, int type)
        {
            if (!IsPlayerAvailableForAnim(sender))
                return;
            switch (type)
            {
                case 1:
                    API.shared.playPlayerAnimation(sender, 1, "rcmminute2lean", "idle_c");
                    break;
                case 2:
                    API.shared.playPlayerAnimation(sender, 1, "mini@strip_club@leaning@base", "base");
                    break;
                default:
                    break;
            }
        }

        public static void DancePlayer(Client sender, int type)
        {
            if (!IsPlayerAvailableForAnim(sender))
                return;
            switch (type)
            {
                case 1:
                    API.shared.playPlayerAnimation(sender, 1, "move_clown@p_m_zero_idles@", "fidget_short_dance");
                    break;
                case 2:
                    API.shared.playPlayerAnimation(sender, 1, "misschinese2_crystalmazemcs1_cs", "dance_loop_tao");
                    break;
                case 3:
                    API.shared.playPlayerAnimation(sender, 1, "missfbi3_sniping", "dance_m_default");
                    break;
                case 4:
                    API.shared.playPlayerAnimation(sender, 1, "missfbi3_sniping", "male_unarmed_a");
                    break;
                case 5:
                    API.shared.playPlayerAnimation(sender, 1, "missfbi3_sniping", "male_unarmed_b");
                    break;
                default:
                    break;
            }
        }

        public static void AnimationStop(Client sender)
        {
            if (!IsPlayerAvailableForAnim(sender, false))
                return;
            API.shared.stopPlayerAnimation(sender);
            CloseMobilePhone(sender);
        }
        public static bool IsPlayerAvailableForAnim(Client sender, bool checkInVehicle = true)
        {
            if (API.shared.getEntityData(sender, "Cuffed") == true)
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kelepçeliyken bunu yapamazsınız.");
                return false;
            }
            if (API.shared.getEntityData(sender, "DeadSeconds") > 0)
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yaralıyken bunu yapamazsınız.");
                return false;
            }
            if (checkInVehicle && sender.isInVehicle)
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araçtayken bunu yapamazsınız.");
                return false;
            }

            return true;
        }


    }
}
