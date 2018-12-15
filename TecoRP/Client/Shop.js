var shop_menu = API.createMenu("Alısveris", "Satışta Olan Eşyalar", 0, 0, 6);
shop_menu.Visible = false;
shop_menu.ResetKey(menuControl.Back);

var shopId = null;
var data = null;
var isAdmin = false;
var isMale = false;

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "shop_open" && shop_menu.Visible == false) {
        
        shopId = args[0];
        data = JSON.parse(args[1]);
        isAdmin = args[2] > 2;
        isMale = args[3];

        for (var i = 0; i < data.length; i++) {
            try {
                let prefix = "";
                if (parseInt(data[i].GameItem.type) < 12)
                    prefix = ((isMale && data[i].GameItem.v0 == "1") || (isMale == false && data[i].GameItem.v0 == "0")) ? "" : "~c~";
                shop_menu.AddItem(API.createMenuItem(prefix + data[i].GameItem.name + "  $" + data[i].SaleItem.p, data[i].GameItem.des));
            } catch (e) {
                API.sendNotification(e.toString());
            }
        }
        if (isAdmin) {
            shop_menu.AddItem(API.createColoredItem("ÖGE EKLE...", "Mağazadan öge kaldırmak için ~r~X~w~ Fiyatını değiştirmek için ~r~P~w~, Yeniden adlandırmak için ~r~R~w~ tuşunu kullanabilirsiniz.", "#ff7300", "#f1f1f2"));
        }
        shop_menu.Visible = true;


        shop_menu.OnIndexChange.connect(function (sender, index) {

            if (parseInt(data[index].GameItem.type) < 12) {
                if ((isMale && data[index].GameItem.v0 == "1") || (isMale == false && data[index].GameItem.v0 == "0")) {

                    var slot = data[index].GameItem.type;
                    var drawable = data[index].GameItem.v1;
                    var texture = data[index].GameItem.v2;

                    API.setPlayerClothes(API.getLocalPlayer(), parseInt(slot), parseInt(drawable), parseInt(texture));
                }
            }
        });

    }
});


shop_menu.OnItemSelect.connect(function (sender, item, index) {
    if (data.length == index) {
        API.sendChatMessage("Eklemek istediğiniz item id aralığını girin. (Örn.: 800-867)");
        API.triggerServerEvent("AddItemToShop", shopId, API.getUserInput("0", 10), API.getUserInput("$10", 12));
    } else {
        API.triggerServerEvent("BuyItem", shopId, data[index].GameItem.id, index);
        shop_menu.Visible = false;
        API.showCursor(false);
        shop_menu.Clear();
    }
    API.triggerServerEvent("LoadClothes");
});


API.onKeyDown.connect(function (Player, args) {
    if ((args.KeyCode == Keys.Escape || args.KeyCode == Keys.Back) && shop_menu.Visible == true) {
        shop_menu.Visible = false;
        API.showCursor(false);
        shop_menu.Clear();
        API.triggerServerEvent("LoadClothes");
    }

    if (shop_menu.Visible == true && args.KeyCode == Keys.X) {
        if (isAdmin) {
            API.triggerServerEvent("RemoveItemFromShop", shopId, data[shop_menu.CurrentSelection].GameItem.id);
            shop_menu.RemoveItemAt(shop_menu.CurrentSelection);
            data.splice(shop_menu.CurrentSelection, 1);
        }
        //shop_menu.Visible = false;
    }
    if (shop_menu.Visible == true && args.KeyCode == Keys.P) {
        if (isAdmin) {
            var newMoney = API.getUserInput("$" + data[shop_menu.CurrentSelection].SaleItem.p, 12);
            API.triggerServerEvent("UpdatePriceShopItem", shopId, data[shop_menu.CurrentSelection].GameItem.id, newMoney);
            var idx = shop_menu.CurrentSelection;

            shop_menu.Children[idx]
            shop_menu.RemoveItemAt(idx);
            shop_menu.InsertItem(API.createMenuItem(data[idx].GameItem.name + "  $" + newMoney, data[idx].GameItem.des), idx);
            shop_menu.RefreshIndex();
        }
        //shop_menu.Visible = false;
    }

    if (shop_menu.Visible == true && args.KeyCode == Keys.R && isAdmin) {
        API.triggerServerEvent("RenameGameItem", data[shop_menu.CurrentSelection].GameItem.id, API.getUserInput(data[shop_menu.CurrentSelection].GameItem.name,40));
    }
});
