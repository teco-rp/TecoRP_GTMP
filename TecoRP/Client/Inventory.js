//--------------INVENTORY------------
var inventory_menu = API.createMenu("Envanter", "Eşyalarınız", 0, 0, 4);
var player_selection_menu = API.createMenu("Eşya Sat", "Satmak istediğiniz oyuncuyu seçin", 0, 0, 4);
API.setMenuBannerRectangle(inventory_menu, 255, 74, 20, 140);
player_selection_menu.Visible = false;
inventory_menu.Visible = false;
player_selection_menu.ResetKey(menuControl.Back);
inventory_menu.ResetKey(menuControl.Back);

var selectionType = null;
var objectId = null;
var idList = null;
var itemIndex = null;
var invData = null;
var currentFilter = "Hepsi";

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "inventory_open" && inventory_menu.Visible == false) {
        invData = JSON.parse(args[0]);
        callInventoryMenu(invData, args[1]);
    }
    if (name == "inventory_open_selection") {
        selectionType = args[4];
        objectId = args[5];
        API.setMenuSubtitle(inventory_menu, args[3]);
        for (var i = 0; i < args[0]; i++) {
            inventory_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        inventory_menu.Visible = true;
    }
    if (name == "sell_item_player_selection") {
        idList = args[2];
        itemIndex = args[3];
        for (var i = 0; i < args[0]; i++) {
            player_selection_menu.AddItem(API.createMenuItem(args[1][i], ""));
        }
        API.showCursor(true);
        player_selection_menu.Visible = true;

    }
});

function callInventoryMenu(data) {
    inventory_menu.Clear();
    API.setMenuSubtitle(inventory_menu, "Eşyalarım:  " + invData.items.length + "/" + invData.max);
    let filterList = new List(String);
    filterList.Add("Hepsi");
    filterList.Add("Telefonlar");
    filterList.Add("Kıyafetler");
    filterList.Add("İçecekler");
    filterList.Add("Yiyecekler");
    filterList.Add("Uyuşturucu ve Maddeler");
    filterList.Add("Lisanslar");
    filterList.Add("Tamir Parçaları");
    filterList.Add("Normal");
    filterList.Add("Kuşanılabilir");
    filterList.Add("Malzemeler");
    filterList.Add("Silahlar ve Cephane");
    filterList.Add("İlkyardım");

    var filterMenuItem = API.createListItem("Tür", "Eşyalarınızı türlerine göre filtreleyebilirsiniz.", filterList, 0);
    inventory_menu.AddItem(filterMenuItem);
    fillMenuWithType(filterMenuItem.List[0]);
    inventory_menu.Visible = true;

    filterMenuItem.OnListChanged.connect(function (sender, idx) {
        inventory_menu.Clear();
        inventory_menu.AddItem(filterMenuItem);
        fillMenuWithType(sender.List[idx]);
    });
}

function fillMenuWithType(type) {
    currentFilter = type;
    for (let i = 0; i < invData.items.length; i++) {
        if (compare(invData.items[i].type, type)) {

            if (invData.items[i].isEquipped) {
                let menu = API.createColoredItem(invData.items[i].name, invData.items[i].des, "#4A148C", "#F1F1F2");
                menu.SetLeftBadge(BadgeStyle.Star);
                menu.SetRightLabel(invData[i].count);
                inventory_menu.AddItem(menu);
            }
            else {
                let menu = API.createMenuItem(invData.items[i].name, invData.items[i].des)
                menu.SetRightLabel(invData[i].count);
                inventory_menu.AddItem();
            }
        }
    }
}

function compare(type, menuName) {
    switch (menuName) {
        case "Hepsi": return true;
        case "Normal": return type == 0;
        case "Kıyafetler": return type > 0 && type < 12;
        case "içecekler": return type == 12;
        case "Yiyecekler": return type == 13;
        case "İlkyardım": return type == 14;
        case "Silahlar ve Cephane": return type == 15 || type == 18 || type == 19;
        case "Uyuşturucu ve Maddeler": return type == 16;
        case "Lisanslar": return type == 20;
        case "Tamir Parçaları": return type == 22;
        case "Telefonlar": return type == 24;
        case "Malzemeler": return type == 25;
        case "Kuşanılabilir": return type == 27;
    }
    return false;
}

function getItemAtIndex(index) {
    let idx = 0;
    for (let i = 0; i < invData.items.length; i++) {
        if (compare(invData.items[i].type, currentFilter)) {
            if ((index - 1) == idx) {
                return invData.items[i];
            }
            idx++;
        }
    }
    return null;
}

inventory_menu.OnItemSelect.connect(function (sender, item, index) {

    if (selectionType == null) {
  

        try {
            var selectedItem = getItemAtIndex(index);
            API.triggerServerEvent("InventoryItemSelected", selectedItem.id);
            inventory_menu.Visible = false;
            API.showCursor(false);
            inventory_menu.Clear();

        } catch (e) {
            API.sendChatMessage("Error: " + e)
        }
    } else {
        API.triggerServerEvent("inventory_item_selection_selected", selectionType, objectId, index);
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
        selectionType = null;
    }
});
player_selection_menu.OnItemSelect.connect(function (sender, item, index) {
    API.sendChatMessage("Satmak istediğiniz fiyatı giriniz.");
    API.triggerServerEvent("return_sell_item_player_selection", idList[index], itemIndex, API.getUserInput("$", 9));
    player_selection_menu.Visible = false;
    API.showCursor(false);
    player_selection_menu.Clear();
});
API.onKeyDown.connect(function (Player, args) {
    if (inventory_menu.Visible == true && (args.KeyCode == Keys.Escape || args.KeyCode == Keys.Back)) {
        selectionType = null;
        objectId = null;
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    }
    //if (args.KeyCode == Keys.B && inventory_menu.Visible == true) {
    //    API.triggerServerEvent("key_B", inventory_menu.CurrentSelection)
    //    inventory_menu.Visible = false;
    //    API.showCursor(false);
    //    inventory_menu.Clear();
    //}
    if (args.KeyCode == Keys.B && inventory_menu.Visible == true) {
        API.triggerServerEvent("request_sell_item", inventory_menu.CurrentSelection);
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    }
    if (args.KeyCode == Keys.X && inventory_menu.Visible == true) {

        let selected = getItemAtIndex(inventory_menu.CurrentSelection);
        API.triggerServerEvent("Key_X", selected.id);
        if (selected.droppable) {
            invData.items.splice(invData.items.indexOf(selected), 1);
            inventory_menu.RemoveItemAt(inventory_menu.CurrentSelection);
            inventory_menu.RefreshIndex();
        }
        //inventory_menu.Visible = false;
        //API.showCursor(false);
        //inventory_menu.Clear();
    }
    if (args.KeyCode == Keys.N) {
        API.triggerServerEvent("key_N");
    }

});




//----------------GIVE---ITEM-------------------
var players_menu = API.createMenu("Envanter", "Eşyayı vermek istediğiniz kişiyi seçin |", 0, 0, 4);
API.setMenuBannerRectangle(players_menu, 200, 255, 10, 30);
players_menu.Visible = false;
players_menu.ResetKey(menuControl.Back);
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "select_players_list" && players_menu.Visible == false) {
        API.setMenuSubtitle(players_menu, args[3]);
        for (var i = 0; i < args[0]; i++) {
            players_menu.AddItem(API.createMenuItem(args[1][i], args[2]));
        }
        API.showCursor(true);
        players_menu.Visible = true;
    }
});

players_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("retun_players_list", item.Text, item.Description);
    players_menu.Visible = false
    API.showCursor(false);
    players_menu.Clear();
});

API.onKeyDown.connect(function (Player, args) {
    if ((args.KeyCode == Keys.Escape || args.KeyCode == Keys.Back) && players_menu.Visible === true) {
        players_menu.Visible = false;
        API.showCursor(false);
        players_menu.Clear();
    }
});