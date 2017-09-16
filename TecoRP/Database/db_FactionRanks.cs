using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_FactionRanks
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(FactionRankList));
        public static string dataPath = "Data/FactionRanks.xml";
        public static Models.FactionRankList currentFactionRAnkss = new FactionRankList();

        public db_FactionRanks()
        {

        }
        public static void InitRanks()
        {
            currentFactionRAnkss.Items.Clear();
            GetAll();
            if (currentFactionRAnkss.Items.Count == 0)
            {
                var factionRank = new FactionRank();
                factionRank.FactionId = 1;
                currentFactionRAnkss.Items.Add(factionRank);
                SaveChanges();
            }
        }
        public static FactionRankList GetAll()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(FactionRankList), new XmlRootAttribute("FactionRank_List"));
                    currentFactionRAnkss = (FactionRankList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return currentFactionRAnkss;
        }

        public static Rank GetRank(int factionId, int rankLevel)
        {
            var _FacRank = currentFactionRAnkss.Items.FirstOrDefault(x => x.FactionId == factionId);
            if (_FacRank != null)
            {
                var _rank = _FacRank.Ranks.FirstOrDefault(x => x.RankLevel == rankLevel);
                if (_rank != null)
                {
                    return _rank;
                }
            }
            else
            {
                InitNewFaction(factionId);
                return GetRank(factionId, rankLevel);
            }
            return new Rank { RankName="Yok" };
        }
        public static FactionRank GetFactionRanks(int factionId)
        {
            var _FacRank = currentFactionRAnkss.Items.FirstOrDefault(x => x.FactionId == factionId);
            if (currentFactionRAnkss != null)
            {
                //API.shared.consoleOutput("step 1: " + _FacRank.Ranks.Count);
                return _FacRank;
            }
            else
            {
                //API.shared.consoleOutput("step 2: " + _FacRank.Ranks.Count);

                var _newFacRank = new FactionRank();
                _newFacRank.FactionId = factionId;
                currentFactionRAnkss.Items.Add(_newFacRank);
                SaveChanges();
                //API.shared.consoleOutput("step 3: " + _newFacRank.Ranks.Count);

                return _newFacRank;
            }
        }
        public static bool UpdateFactionRank(FactionRank _model)
        {
            var _editedModel = currentFactionRAnkss.Items.FirstOrDefault(x => x.FactionId == _model.FactionId);
            if (_editedModel != null)
            {
                try
                {
                    _editedModel.Ranks = _model.Ranks;
                    SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    return false;
                }
            }
            else
                return false;

        }
        public static bool AddRankToFaction(int factionId, Rank _model)
        {
            try
            {
                var _faction = currentFactionRAnkss.Items.FirstOrDefault(x => x.FactionId == factionId);
                if (_faction != null)
                {
                    if (_faction.Ranks == null) { _faction.Ranks = new List<Rank>(); }

                    if (_faction.Ranks.Count == 0)
                    {
                        _faction.Ranks = new List<Rank>();
                        _faction.Ranks.Add(new Rank { RankLevel = 1, RankName = _model.RankName });
                    }
                    else
                    _faction.Ranks.Add(new Rank { RankLevel = _faction.Ranks.LastOrDefault().RankLevel + 1, RankName = _model.RankName });

                    SaveChanges();
                    return true;
                }
                else
                {
                    var _FacRank = new FactionRank();
                    _FacRank.FactionId = factionId;
                    _FacRank.Ranks.Add(new Rank { RankLevel = 1, RankName = _model.RankName });
                    currentFactionRAnkss.Items.Add(_FacRank);
                    SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                return false;
            }
        }

        private static void InitNewFaction(int factionId)
        {
            var _FacRank = new FactionRank { FactionId = factionId };
            currentFactionRAnkss.Items.Add(_FacRank);
            SaveChanges();
        }

        public static bool RemoveFactionRank(int factionId, int rankLevel)
        {
            var _factionRank = currentFactionRAnkss.Items.FirstOrDefault(x => x.FactionId == factionId);
            if (_factionRank != null)
            {
                var _Rank = _factionRank.Ranks.FirstOrDefault(x => x.RankLevel == rankLevel);
                if (_Rank != null)
                {
                    _factionRank.Ranks.Remove(_Rank);
                    int i = 1;
                    foreach (var item in _factionRank.Ranks)
                    {
                        item.RankLevel = i;
                        i++;
                    }
                    SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public static void SaveChanges()
        {

            lock (currentFactionRAnkss)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, currentFactionRAnkss);
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
