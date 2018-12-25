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
    if (loginBrowser != null) {
        API.destroyCefBrowser(loginBrowser);
        loginBrowser = null;
    }


    var chars_menu = API.createMenu("Karakterler", "Karakter seçimi:", 0, 0, 6);
    API.setMenuBannerRectangle(chars_menu, 255, 49, 27, 146);
    let data = JSON.parse(args[0]);
    let chars = JSON.parse(args[1]);


    for (var i = 0; i < chars.length; i++) {
        chars_menu.AddItem(API.createMenuItem(chars[i].CharacterName, ""));
    }
    chars_menu.OnIndexChange.connect(function (sender, index) {
        if (index < chars.length) {
            API.triggerServerEvent("PreviewCharacter", chars[index].CharacterId);
        }
    });

    chars_menu.OnItemSelect.connect(function (sender, index) {
        if (index < chars.length) {
            API.triggerServerEvent("CharacterSelected", chars[index].CharacterId);
        }
    });

    if (data.MaxCharacters > chars.length) {
        var newCharMenuItem = API.createColoredItem("Karakter Oluştur", "", "#311B92", "#F1F1F2");

        chars_menu.AddItem(newCharMenuItem);
        newCharMenuItem.Activated.connect(function (sender, selected) {
            chars_menu.Clear();
            chars_menu.Visible = false;
            API.triggerServerEvent("CMD_EnableCreator");
        });
    }
    chars_menu.Visible = true;
}

API.onUpdate.connect(function () {
    if (loginBrowser != null) { API.disableAllControlsThisFrame(); }
});