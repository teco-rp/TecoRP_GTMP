/// <reference path="types-gt-mp/index.d.ts" />

var menu = null;
var currentHead = 0,currentEyes = 0, currentHair = 0, currentHairColor = 0;

API.onServerEventTrigger.connect(function (eventName, args) {


    if (eventName == "ChooseCharacterApperance") {

        var player = API.getLocalPlayer();
        menu = API.createMenu("Karakter", "Karakterinizi oluşturun.", 100, 100, 0, true);

        var head = new List(String);

        if (args[0] === "male") {
            for (let i = 0; i < 21; i++) {
                head.Add(i.toString());
            }
        }
        else {
            for (let i = 21; i < 41; i++) {
                head.Add(i.toString());
            }
        }

        var headOptionsItem = API.createListItem("Ten", "Karakterinizin ten rengini ve ırkını ayarlayabilirsiniz.", head, 0);
        headOptionsItem.OnListChanged.connect(function (sender, index) {
            currentHead = parseInt(sender.List[index]);
            API.setPlayerClothes(player, 0, currentHead, 0);
        });

        var eyes = new List(String);
        for (let i = 0; i < 32; i++) {
            eyes.Add(i.toString());
        }

        var eyesOptionsItem = API.createListItem("Gözler", "Karakterinizin göz rengini değiştirebilirsiniz.", eyes, 0);
        eyesOptionsItem.OnListChanged.connect(function (sender, index) {
            currentEyes = parseInt(sender.List[index]);
            API.setPlayerEyeColor(player, currentEyes);
        });

        hairs = new List(String); //36
        if (args[0] === "male") {
            for (var i = 0; i < 37; i++) {
                hairs.Add(i.toString());
            }
        }
        else {
            for (let i = 0; i < 39; i++) {
                hairs.Add(i.toString());
            }
            hairs.Add("76");
            hairs.Add("77");
        }

        var hairsOptions = API.createListItem("Saç", "Karakterinizin saç şeklini seçebilirsiniz.", hairs, 0);
        hairsOptions.OnListChanged.connect(function (sender, index) {
            currentHair = parseInt(sender.List[index]);
            API.setPlayerClothes(player, 2, currentHair, currentHairColor);
        });

        hairsColors = new List(String);
        if (args[0] === "male") {
            for (let i = 0; i < 7; i++) {
                hairsColors.Add(i.toString());
            }       
        }
        else {
            for (let i = 0; i < 6; i++) {
                hairsColors.Add(i.toString());
            }  
        }

        var hairColorsOptions = API.createListItem("Saç Rengi", "Karakterinizin saç rengini ayarlayabilirsiniz.", hairsColors, 0);
        hairColorsOptions.OnListChanged.connect(function (sender, index) {
            currentHairColor = parseInt(sender.List[index]);
            API.setPlayerClothes(player, 2, currentHair, currentHairColor);
        });

        var completeAction = API.createMenuItem("KAYDET", "");
        completeAction.Activated.connect(function (sender, item) {
            API.triggerServerEvent("SaveCharacterApperance", currentHead, currentEyes, currentHair, currentHairColor);
            menu.Visible = false;
            menu = null;
        });

        menu.AddItem(headOptionsItem);
        menu.AddItem(eyesOptionsItem);
        menu.AddItem(hairsOptions);
        menu.AddItem(hairColorsOptions);
        menu.AddItem(completeAction);
        menu.Visible = true;
    }
});

