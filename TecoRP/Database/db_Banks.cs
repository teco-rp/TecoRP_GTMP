using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_Banks
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(BankList));
        public static string dataPath = "Data/Banks.xml";
        public static Tuple<List<Bank>, List<Marker>, List<TextLabel>> CurrentBanks = new Tuple<List<Bank>, List<Marker>, List<TextLabel>>(new List<Bank>(), new List<Marker>(), new List<TextLabel>());

        public static void SpawnAll()
        {
            API.shared.consoleOutput("Bankalar yüklenmeye başladı.");
            foreach (var item in GetAll().Items)
            {
                CurrentBanks.Item1.Add(item);
                CurrentBanks.Item2.Add(API.shared.createMarker(
                    29, item.Position, new Vector3(0, 0, 0), item.Rotation, new Vector3(0.5f, 0.5f, 0.5f), 200, 10, 255, 30, item.Dimension)
                    );
                CurrentBanks.Item3.Add(API.shared.createTextLabel((item.TypeOfBank == BankType.Bank ? "Stokta : "+item.MoneyCountInInside+"$ ((/banka))": "((/atm))"), item.Position + new Vector3(0, 0, 0.5f), 5, 1, false, item.Dimension));

            }
            API.shared.consoleOutput(CurrentBanks.Item1.Count+" adet banka yüklendi.");
        }

        public static void Create(Bank _model)
        {
            _model.BankId = CurrentBanks.Item1.Count > 0 ? CurrentBanks.Item1.LastOrDefault().BankId + 1 : 1;
            CurrentBanks.Item1.Add(_model);
            CurrentBanks.Item2.Add(API.shared.createMarker(
                   29, _model.Position, new Vector3(0, 0, 0), _model.Rotation, new Vector3(0.5f, 0.5f, 0.5f), 200, 10, 255, 30, _model.Dimension)
                   );
            CurrentBanks.Item3.Add(API.shared.createTextLabel((_model.TypeOfBank == BankType.Bank ? "Stokta : " + _model.MoneyCountInInside + "$ ((/banka))" : "((/atm))"), _model.Position + new Vector3(0, 0, 0.5f), 5, 1, false, _model.Dimension));
            SaveChanges();
        }
        public static Bank GetById(int _Id)
        {
            return CurrentBanks.Item1.FirstOrDefault(x => x.BankId == _Id);
        }
        public static bool Update(Bank _model)
        {
            var _Index = FindIndexById(_model.BankId);
            if (_Index>=0)
            {
                try
                {
                    CurrentBanks.Item1[_Index] = _model;
                    CurrentBanks.Item2[_Index].position = _model.Position;
                    CurrentBanks.Item2[_Index].dimension = _model.Dimension;
                    CurrentBanks.Item3[_Index].position = _model.Position;
                    CurrentBanks.Item3[_Index].dimension = _model.Dimension;
                    CurrentBanks.Item3[_Index].text = (_model.TypeOfBank == BankType.Bank ? "Stokta : " + _model.MoneyCountInInside + "$ ((/banka))" : "((/atm))");
                    SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Error,"Bir hata oluştu: " +ex);
                    return false;
                }                
            }else
            {
                return false;
            }
        }

        public static bool Remove(int _Id)
        {
            var _Index = FindIndexById(_Id);
            if (_Index>=0)
            {
                try
                {
                    API.shared.deleteEntity(CurrentBanks.Item2[_Index]);
                    API.shared.deleteEntity(CurrentBanks.Item3[_Index]);
                    CurrentBanks.Item1.RemoveAt(_Index);
                    CurrentBanks.Item2.RemoveAt(_Index);
                    CurrentBanks.Item3.RemoveAt(_Index);
                    SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Error, "Bir hata oluştu: " + ex);
                    return false;
                }
            }else
            {
                return false;
            }
        }
        public static int FindIndexById(int _Id)
        {
            return CurrentBanks.Item1.IndexOf(CurrentBanks.Item1.FirstOrDefault(x => x.BankId == _Id));
        }
        public static BankList GetAll()
        {
            BankList returnModel = new Models.BankList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(BankList), new XmlRootAttribute("Bank_List"));
                    returnModel = (BankList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static void SaveChanges()
        {
            lock (CurrentBanks)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, new BankList { Items = CurrentBanks.Item1 });
                    xWriter.Dispose();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }

    }
}
