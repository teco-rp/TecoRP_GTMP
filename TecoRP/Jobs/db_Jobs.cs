using GrandTheftMultiplayer.Server.API;
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

namespace TecoRP.Jobs
{
    public static class db_Jobs
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(JobList));
        public static string dataPath = "Data/Jobs.xml";
        public static List<Tuple<Job, Marker, TextLabel>> currentJobsList = new List<Tuple<Job, Marker, TextLabel>>();
        
        static db_Jobs()
        {
            //SpawnAll();
        }

        public static void SpawnAll()
        {
            currentJobsList.Clear();
            foreach (var item in GetAll().Items)
            {
                currentJobsList.Add(new Tuple<Job, Marker, TextLabel>(item,
                    API.shared.createMarker(1, item.Position + new Vector3(0, 0, -1), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 255, 255, 238, 0, item.Dimension),
                    API.shared.createTextLabel(item.Name, item.Position + new Vector3(0, 0, 0.5), 30, 1, false, item.Dimension)
                    ));
            }
        }

        public static Job GetJob(int _Id)
        {
            var _Index = FindIndexById(_Id);
            if (_Index >= 0)
            {
                return currentJobsList[_Index].Item1;
            }

            var job = new Job();
            job.ID = _Id;
            job.JobId = _Id;
            job.Dimension = 0;
            job.Name = "Meslek";
            job.Range = 3;
            return job;
        }

        public static bool EditJob(Job _jobModel)
        {
            var _Index = FindIndexById(_jobModel.ID);
            if (_Index >= 0)
            {
                API.shared.deleteEntity(currentJobsList[_Index].Item2);
                API.shared.deleteEntity(currentJobsList[_Index].Item3);
                currentJobsList.RemoveAt(_Index);
                currentJobsList.Insert(_Index, new Tuple<Job, Marker, TextLabel>(
                    _jobModel,
                    API.shared.createMarker(1, _jobModel.Position + new Vector3(0, 0, -1), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 255, 255, 238, 0, _jobModel.Dimension),
                    API.shared.createTextLabel(_jobModel.Name, _jobModel.Position + new Vector3(0, 0, 0.5), 30, 1, false, _jobModel.Dimension)
                    ));
            }
            else
            {
                _jobModel.ID = _jobModel.JobId;
                _jobModel.JobId = _jobModel.JobId;
                currentJobsList.Add(new Tuple<Job, Marker, TextLabel>(
                    _jobModel,
                    API.shared.createMarker(1, _jobModel.Position + new Vector3(0, 0, -1), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 255, 255, 238, 0, _jobModel.Dimension),
                    API.shared.createTextLabel(_jobModel.Name, _jobModel.Position + new Vector3(0, 0, 0.5), 30, 1, false, _jobModel.Dimension)
                    ));
            }
            SaveChanges();
            return true;
        }
        public static int FindIndexById(int _Id)
        {
            var lst = currentJobsList.Select(s => s.Item1).ToList();
            return lst.IndexOf(lst.FirstOrDefault(x => x.ID == _Id));
        }
        public static bool Remove(int _Id)
        {
            var _Index = FindIndexById(_Id);
            if (_Index>=0)
            {
                currentJobsList.RemoveAt(_Index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static JobList GetAll()
        {
            JobList returnModel = new JobList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(JobList), new XmlRootAttribute("Job_List"));
                    returnModel = (JobList)deserializer.Deserialize(reader);
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
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new JobList { Items = currentJobsList.Select(s => s.Item1).ToList() });
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
    }
}
