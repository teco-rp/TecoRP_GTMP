///// <reference path="types-gt-mp/index.d.ts" />

var browser = null;
var help_main_menu = null;

API.onKeyDown.connect(function (sender, args) {

    switch (args.KeyCode) {
        case Keys.F2:
        callHelpMenu();
            break;
        case Keys.F3:
        openCefTest();
            break;
        case Keys.F4:
        closeCefTest();
            break;
        case Keys.F5:
            break;
        case Keys.F6:
            break;
    }   
});

function openCefTest() {
    var screen = API.getScreenResolution();
    browser = API.createCefBrowser(screen.Width, screen.height, false);
    API.waitUntilCefBrowserInit(browser);
    API.setCefBrowserPosition(browser, 0, 0);
    API.setCefBrowserHeadless(browser, false);

    API.loadPageCefBrowser(browser, "https://enisnecipoglu.com");
}

function closeCefTest() {
    if (browser !== null) {
        API.destroyCefBrowser(browser);
    }
}

function callHelpMenu() {
    if (help_main_menu == null) {
        help_main_menu = API.createMenu("YARDIM", 100, 0, 3);
        help_main_menu.AddItem(API.createMenuItem("Kısayollar", "Oyun içerisinde kullanabileceğiniz kısayol tuşlarıdır."));
        help_main_menu.AddItem(API.createMenuItem("Araçlar", "Araçlar hakkında bilmeniz gerekenler."));
        help_main_menu.AddItem(API.createMenuItem("Evler", "Evler hakkında bilgiler."));
        help_main_menu.AddItem(API.createMenuItem("Envanter", "Evler hakkında bilgiler."));
    }

    help_main_menu.Visible = true;
    API.showCursor(false);    
}

function callShortcutsMenu() {

}