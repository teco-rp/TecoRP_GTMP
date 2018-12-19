using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP.Accounts.Managers.Base
{
    public class BaseManagerScript : Script
    {
        public BaseManagerScript()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
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
            try
            {
                method.Invoke(this, parameters: BindParameters(sender, parameters, arguments).ToArray());
            }
            catch (Exception ex)
            {
                API.consoleOutput(LogCat.Warn, ex.ToString());
                method.Invoke(this, parameters: new object[] { sender, arguments });
            }
        }

        public IEnumerable<object> BindParameters(Client sender, ParameterInfo[] parameters, object[] arguments)
        {
            yield return sender;
            for (int i = 1; i < parameters.Length; i++)
                yield return Convert.ChangeType(arguments[i], parameters[i].ParameterType);

        }
    }
}
