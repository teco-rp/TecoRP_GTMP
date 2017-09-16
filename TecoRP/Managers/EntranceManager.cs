using GrandTheftMultiplayer.Server.API;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;
using System.Threading.Tasks;

namespace TecoRP.Managers
{
    public class EntranceManager : Script
    {
        public static List<GrandTheftMultiplayer.Server.Elements.Marker> MarkersOnMap = new List<GrandTheftMultiplayer.Server.Elements.Marker>();
        public EntranceManager()
        {
            db_Entrances dbEntrances = new db_Entrances();
            MarkersOnMap.Clear();

            foreach (var item in dbEntrances.GetAll().Items)
            {
                MarkersOnMap.Add(API.createMarker(item.MarkerType, item.EntrancePosition, item.Direction, item.Rotation,new Vector3(item.Scale,item.Scale,item.Scale) , item.Color.Alpha, item.Color.Red, item.Color.Green, item.Color.Blue, 0));
                MarkersOnMap.LastOrDefault().dimension = item.EntranceDimension;


                //if (!String.IsNullOrEmpty(item.Name))
                //{
                //    var _label = API.createTextLabel(item.Name, MarkersOnMap.LastOrDefault().position + new Vector3(0, 0, 1), 10, 0.5f, false, 0);
                //    //API.attachEntityToEntity(_label, MarkersOnMap.LastOrDefault(), "SKEL_ROOT", new Vector3(1, 1, 1), new Vector3(1, 1, 1));
                //}
            }
        }
        
    }
}
