//---------------JOB-SELECTOR-------------
var job_tir_selector_menu = API.createMenu("HEDEF", "Hangi teslimatı yapmak istediğinizi seçin:", 0, 0, 4);
var job_kamyon_selector_menu = API.createMenu("HEDEF", "Hangi teslimatı yapmak istediğinizi seçin:", 0, 0, 4);
job_tir_selector_menu.Visible = false;
job_kamyon_selector_menu.Visible = false;
job_tir_selector_menu.ResetKey(menuControl.Back);
job_kamyon_selector_menu.ResetKey(menuControl.Back);

API.onServerEventTrigger.connect(function (name, args) {
    if (name == "open_tir_selector") {
        for (var i = 0; i < args[0]; i++) {
            job_tir_selector_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        job_tir_selector_menu.Visible = true;
        API.showCursor(true);
    }
    if (name == "open_kamyon_selector") {
        for (var i = 0; i < args[0]; i++) {
            job_kamyon_selector_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        job_kamyon_selector_menu.Visible = true;
        API.showCursor(true);
    }
});

job_tir_selector_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("return_tir_selector", item.Text);
    job_tir_selector_menu.Visible = false;
    job_tir_selector_menu.Clear();
    API.showCursor(false);
});
job_kamyon_selector_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("return_kamyon_selector", item.Text);
    job_kamyon_selector_menu.Visible = false;
    job_kamyon_selector_menu.Clear();
    API.showCursor(false);
});

API.onUpdate.connect(function () {
    API.drawMenu(job_tir_selector_menu);
    API.drawMenu(job_kamyon_selector_menu);
});
//--------------------------------------