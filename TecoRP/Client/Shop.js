var shop_menu = API.createMenu("Alısveris", "Satışta Olan Eşyalar", 0, 0, 6);
shop_menu.Visible = false;
shop_menu.ResetKey(menuControl.Back);

var shopId = null;
var data = null;

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "shop_open") {

        shopId = args[0];
        data = JSON.parse(args[1]);
        var isAdmin = args[2] > 2;

        for (var i = 0; i < data.length; i++) {
            try {
                shop_menu.AddItem(API.createMenuItem(data[i].GameItem.name + "  $" + data[i].SaleItem.p, data[i].GameItem.des));
            } catch (e) {
                API.sendNotification(e.toString());
            }
        }
        if (isAdmin) {
            shop_menu.AddItem(API.createMenuItem("ÖGE EKLE...", "Mağazadan öge kaldırmak için ~r~X~w~ Fiyatını değiştirmek için ~r~P~w~ tuşunu kullanabilirsiniz."));
        }
        API.showCursor(true);
        shop_menu.Visible = true;


        shop_menu.OnIndexChange.connect(function (sender, index) {

            if (parseInt(data[index].GameItem.type) < 12) {
                var slot = data[index].GameItem.type;
                var drawable = data[index].GameItem.v1;
                var texture = data[index].GameItem.v2;

                API.setPlayerClothes(API.getLocalPlayer(), parseInt(slot), parseInt(drawable), parseInt(texture));
            }
        });

    }
});


shop_menu.OnItemSelect.connect(function (sender, item, index) {
    if (data.length == index) {
        API.sendChatMessage("Eklemek istediğiniz item id aralığını girin. (Örn.: 800-867)");
        API.triggerServerEvent("AddItemToShop", shopId, API.getUserInput("0",10), API.getUserInput("$10",12));
    } else {
        API.triggerServerEvent("shop_item_selected", shopId, index);
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
        API.triggerServerEvent("RemoveItemFromShop", shopId, data[shop_menu.CurrentSelection].GameItem.id);
        //shop_menu.Visible = false;
    }
    if (shop_menu.Visible == true && args.KeyCode == Keys.P) {
        API.triggerServerEvent("UpdatePriceShopItem", shopId, data[shop_menu.CurrentSelection].GameItem.id, API.getUserInput("$" + data[shop_menu.CurrentSelection].SaleItem.p, 12));
        //shop_menu.Visible = false;
    }
});
