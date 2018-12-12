using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP.Managers.Base
{
    public class EventMethodTriggerBase : Script
    {
        public EventMethodTriggerBase()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }
        
        private void API_onClientEventTrigger(GrandTheftMultiplayer.Server.Elements.Client sender, string eventName, params object[] arguments)
        {
            this.GetType().GetMethod(eventName)?.Invoke(this, parameters: new object[] { sender, arguments });
        }
    }
}
