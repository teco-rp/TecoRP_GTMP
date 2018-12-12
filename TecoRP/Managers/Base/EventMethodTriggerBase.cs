using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
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
            var method = this.GetType().GetMethod(eventName);
            if (method == null)
                return;

            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Client))
            {
                method.Invoke(this, parameters: new object[] { sender });
                return;
            }

            method.Invoke(this, parameters: new object[] { sender, arguments });
        }
    }
}
