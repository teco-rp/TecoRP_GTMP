/// <reference path="../../types-gt-mp/Declarations.d.ts" />

var loginBrowser = null; 

API.onResourceStart.connect(function () {
    try {
        API.setEntityTransparency(API.getLocalPlayer(),0);
        var res = API.getScreenResolution(); //this gets the client's screen resoulution
        loginBrowser = API.createCefBrowser(res.Width, res.Height); //we're initializing the browser here. This will be the full size of the user's screen.
        API.waitUntilCefBrowserInit(loginBrowser); //this stops the script from getting ahead of itself, it essentially pauses until the browser is initialized
        API.setCefBrowserPosition(loginBrowser, 0, 0); //The orientation (top left) corner in relation to the user's screen.  This is useful if you do not want a full page browser.  0,0 is will lock the top left corner of the browser to the top left of the screen.
        API.loadPageCefBrowser(loginBrowser, "Client/Accounts/login.html"); //This loads the HTML file of your choice.      .    API.setCefBrowserHeadless(myBrowser, true); //this will remove the scroll bars from the bottom/right side
        API.showCursor(true); //This will show the mouse cursor
        API.setCanOpenChat(false);  //This disables the chat, so the user can type in a form without opening the chat and causing issues.
    
    } catch (e) {
        API.sendChatMessage("Error Occoured: " + e);
        API.sendNotification("Error Occoured:");
        API.sendNotification(":" + e);
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "register_result") {
        if (args[0] == true) {
            API.loadPageCefBrowser(loginBrowser, "Client/Accounts/login.html");
        }
        else {
            API.loadPageCefBrowser(loginBrowser, "Client/Accounts/login.html?message="+args[1]);
        }
    }
    if (eventName == "login_result") {
        if (args[0] == true) {
            API.destroyCefBrowser(loginBrowser);
        }
        else {
            API.loadPageCefBrowser(loginBrowser, "Client/Accounts/login.html?message="+args[1]);
        }
    }

    if (eventName == "go_character_selection") {
        callCharacterSelectionMenu(args);
    }
});

function login(email, password) {
    API.triggerServerEvent("Login",email, password);
}

function registerFromCef(email, password) {
    API.triggerServerEvent("Register", email, password);
}

function callCharacterSelectionMenu(args) {
    if (loginBrowser != null)
        API.destroyCefBrowser(loginBrowser);


    var chars_menu = API.createMenu("Karakterler", "Karakter seçimi:", 0, 0, 6);
    API.setMenuBannerRectangle(chars_menu, 255, 49, 27, 146);
    API.sendChatMessage("args length: " + args.Length);

    let data = JSON.parse(args[0]);

    for (var i = 0; i < data.Characters.length; i++) {
        chars_menu.AddItem(API.createMenuItem(data.Characters[i], ""));
    }

    var newCharMenuItem = API.createColoredItem("Karakter Oluştur", "", "#311B92", "#F1F1F2");
    chars_menu.AddItem(newCharMenuItem);
    newCharMenuItem.Activated.connect(function (sender, selected) {
        chars_menu.Clear();
        chars_menu.Visible = false;
        API.triggerServerEvent("CMD_EnableCreator");
    });
    chars_menu.Visible = true;
}