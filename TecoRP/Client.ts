/// <reference path="types-gt-mp/index.d.ts" />

//-------------CUSTOM-RETURN-MENU-------------
var custom_menu = API.createMenu("MENU", "AÇIKLAMA", 100, 100, 6);
custom_menu.Visible = false;
custom_menu.ResetKey(menuControl.Back);


var IDList;
var retunName = "";
API.onServerEventTrigger.connect(function (name, args) {
    if (name == "open_custom_menu") {
        IDList = args[3];
        retunName = args[4];
        API.setMenuTitle(custom_menu, args[5]);
        API.setMenuSubtitle(custom_menu, args[6]);
        for (var i = 0; i < args[0]; i++) {
            custom_menu.AddItem(API.createMenuItem(args[1][i], args[2].length == 0 ? "" : args[2][i]));
        }
        API.showCursor(true);
        custom_menu.Visible = true;
    }
});
custom_menu.OnItemSelect.connect(function (sender, item, index)
{
    API.triggerServerEvent(retunName, IDList[index]);
    custom_menu.Visible = false;
    custom_menu.Clear();
    API.showCursor(false);
});


API.onKeyDown.connect(function (Player, args) {
    if (custom_menu.Visible == true && args.KeyCode == Keys.Escape) {
        custom_menu.Visible = false;
        API.showCursor(false);
        custom_menu.Clear();
    }
});