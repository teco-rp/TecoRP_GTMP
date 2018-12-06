//-------------JOB-LEVELS-------------
var skill_menu = API.createMenu("YETENEKLER", "Sahip olduğunuz yetenekler ve seviyeleri", 100, 100, 6);
skill_menu.Visible = false;
skill_menu.ResetKey(menuControl.Back);


API.onServerEventTrigger.connect(function (name, args) {
    if (name == "open_skills") {
        for (var i = 0; i < args[0]; i++) {
            skill_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        skill_menu.Visible = true;
    }
});

//TODO: [Deprecated] Check after test
//API.onUpdate.connect(function () {
//    API.drawMenu(skill_menu);
//});

API.onKeyDown.connect(function (Player, args) {
    if (skill_menu.Visible == true && args.KeyCode == Keys.Escape) {
        skill_menu.Visible = false;
        API.showCursor(false);
        skill_menu.Clear();
    }
    
});
