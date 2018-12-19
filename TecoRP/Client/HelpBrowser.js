//CEF-BROWSER HELP


var res_X = API.getScreenResolutionMaintainRatio().Width;
var res_Y = API.getScreenResolutionMaintainRatio().Height;
var isBrowserOpened = false;
var browser = null;

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.F1 && !API.isChatOpen() && isBrowserOpened == false) {
        browser = API.createCefBrowser(1000, 1000, false);
        //browser = API.createCefBrowser(res_X * 0.80, res_Y * 0.80, false);
        API.setCefBrowserHeadless(browser, true);
        API.setCefBrowserPosition(browser, 0, 0);
        //API.setCefBrowserPosition(browser, res_X * 0.1, res_Y * 0.1);
        API.waitUntilCefBrowserInit(browser);
        API.loadPageCefBrowser(browser, "Client/yardim/index.html", false);

        isBrowserOpened = true;
    }
    if ((args.KeyCode == Keys.Escape || args.KeyCode == Keys.F1) && isBrowserOpened == true) {
        API.destroyCefBrowser(browser);
        isBrowserOpened = false;
    }
}
);