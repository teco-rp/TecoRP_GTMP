/// <reference path="../../types-gt-mp/Declarations.d.ts" />

var loginBrowser = null; //we set it to null because the browser is not yet created, we cannot have a var that has empty value, so we use null


API.onResourceStart.connect(function () {
    try {
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

function login(email, password) {
    API.sendChatMessage("Login triggered with params: " + email + " | " + password);
}

function register(email, password) {
    API.register("Register triggered with params: " + email + " | " + password);
    API.destroyCefBrowser(loginBrowser);
}