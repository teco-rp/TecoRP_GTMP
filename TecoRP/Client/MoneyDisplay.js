var currentMoney = null;
var res_X = API.getScreenResolutionMaintainRatio().Width;

API.onServerEventTrigger.connect(function (name, args) {
    if (name === "update_money_display") {
        currentMoney = args[0];
    }
    if (name === "update_hungerthirsty") {
        currentHunger = args[0];
        currentThirsty = args[1];
    }
    if (name == "display_subtitle") {
        API.displaySubtitle(args[0], args[1]);
    }
});

API.onUpdate.connect(function () {
    if (currentMoney != null) {
        API.drawText("$" + currentMoney, res_X - 15, 50, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
    }
    if (currentHunger != null) {
        API.drawText("" + currentHunger, 375, res_Y - 130, 1, 255, 119, 0, 200, 7, 0, true, false, 0);
        API.drawText("" + currentThirsty, 375, res_Y - 180, 1, 0, 175, 206, 200, 7, 0, true, false, 0);
    }
   // API.dxDrawTexture("logo.png", new Point(240, 240), new Size(400, 400), 0);
});

var currentHunger = null;
var currentThirsty = null;
var res_X = API.getScreenResolutionMaintainRatio().Width;
var res_Y = API.getScreenResolutionMaintainRatio().Height;
