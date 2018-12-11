/// <reference path="types-gt-mp/index.d.ts" />

var menu = null;
var player = API.getLocalPlayer();
var currentHead = 0, currentHair = 0, currentHairColor = 0;

API.onServerEventTrigger.connect(function (eventName, args) {


    if (eventName == "ChooseCharacterApperance") {

        menu = API.createMenu("Karakter", "Karakterinizi oluşturun.", 100, 100, 0, true);

        var head = new List(String);

        if (args[0] === "male") {
            head.Add("0");
            head.Add("1");
            head.Add("2");
            head.Add("3");
            head.Add("4");
            head.Add("5");
            head.Add("6");
            head.Add("7");
            head.Add("8");
            head.Add("9");
            head.Add("10");
            head.Add("11");
            head.Add("12");
            head.Add("13");
            head.Add("14");
            head.Add("15");
            head.Add("16");
            head.Add("17");
            head.Add("18");
            head.Add("19");
            head.Add("20");
        }
        else {
            head.Add("21");
            head.Add("22");
            head.Add("23");
            head.Add("24");
            head.Add("25");
            head.Add("26");
            head.Add("27");
            head.Add("28");
            head.Add("29");
            head.Add("30");
            head.Add("31");
            head.Add("32");
            head.Add("33");
            head.Add("34");
            head.Add("35");
            head.Add("36");
            head.Add("37");
            head.Add("38");
            head.Add("39");
            head.Add("30");
            head.Add("40");
        }

        var headOptionsItem = API.createListItem("Ten", "Karakterinizin ten rengini ve ırkını ayarlayabilirsiniz.", head, 0);
        headOptionsItem.Activated.connect(function (sender, selected) { API.sendChatMessage("head options activated triggered"); });
        headOptionsItem.OnListChanged.connect(function (sender, index) {
            currentHead = parseInt(sender.List[index]);
            API.setPlayerClothes(player, 0, currentHead, 0);
            API.sendChatMessage("head options changed");
        });

        var eyes = new List(String);
        eyes.Add("0");
        eyes.Add("1");
        eyes.Add("2");
        eyes.Add("3");
        eyes.Add("4");
        eyes.Add("5");
        eyes.Add("6");
        eyes.Add("7");
        eyes.Add("8");
        eyes.Add("9");
        eyes.Add("10");
        eyes.Add("11");
        eyes.Add("12");
        eyes.Add("13");
        eyes.Add("14");
        eyes.Add("15");
        eyes.Add("16");
        eyes.Add("17");
        eyes.Add("18");
        eyes.Add("19");
        eyes.Add("20");
        eyes.Add("21");
        eyes.Add("22");
        eyes.Add("23");
        eyes.Add("24");
        eyes.Add("25");
        eyes.Add("26");
        eyes.Add("27");
        eyes.Add("28");
        eyes.Add("29");
        eyes.Add("30");
        eyes.Add("31");

        var eyesOptionsItem = API.createListItem("Gözler", "Karakterinizin göz rengini değiştirebilirsiniz.", eyes, 0);
        eyesOptionsItem.OnListChanged.connect(function (sender, index) {
            API.setPlayerEyeColor(player, parseInt(sender.List[index]));
        });

        hairs = new List(String); //36
        if (args[0] === "male") {
            hairs.Add("0");
            hairs.Add("1");
            hairs.Add("2");
            hairs.Add("3");
            hairs.Add("4");
            hairs.Add("5");
            hairs.Add("6");
            hairs.Add("7");
            hairs.Add("8");
            hairs.Add("9");
            hairs.Add("10");
            hairs.Add("11");
            hairs.Add("12");
            hairs.Add("13");
            hairs.Add("14");
            hairs.Add("15");
            hairs.Add("16");
            hairs.Add("17");
            hairs.Add("18");
            hairs.Add("19");
            hairs.Add("20");
            hairs.Add("21");
            hairs.Add("22");
            hairs.Add("23");
            hairs.Add("24");
            hairs.Add("25");
            hairs.Add("26");
            hairs.Add("27");
            hairs.Add("28");
            hairs.Add("29");
            hairs.Add("30");
            hairs.Add("31");
            hairs.Add("32");
            hairs.Add("33");
            hairs.Add("34");
            hairs.Add("35");
            hairs.Add("36");
        }
        else {
            hairs.Add("0");
            hairs.Add("1");
            hairs.Add("2");
            hairs.Add("3");
            hairs.Add("4");
            hairs.Add("5");
            hairs.Add("6");
            hairs.Add("7");
            hairs.Add("8");
            hairs.Add("9");
            hairs.Add("10");
            hairs.Add("11");
            hairs.Add("12");
            hairs.Add("13");
            hairs.Add("14");
            hairs.Add("15");
            hairs.Add("16");
            hairs.Add("17");
            hairs.Add("18");
            hairs.Add("19");
            hairs.Add("20");
            hairs.Add("21");
            hairs.Add("22");
            hairs.Add("23");
            hairs.Add("24");
            hairs.Add("25");
            hairs.Add("26");
            hairs.Add("27");
            hairs.Add("28");
            hairs.Add("29");
            hairs.Add("30");
            hairs.Add("31");
            hairs.Add("32");
            hairs.Add("33");
            hairs.Add("34");
            hairs.Add("35");
            hairs.Add("36");
            hairs.Add("37");
            hairs.Add("38");
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
            hairsColors.Add("0");
            hairsColors.Add("1");
            hairsColors.Add("2");
            hairsColors.Add("3");
            hairsColors.Add("4");
            hairsColors.Add("5");
            hairsColors.Add("6");
        }
        else {
            hairsColors.Add("0");
            hairsColors.Add("1");
            hairsColors.Add("2");
            hairsColors.Add("3");
            hairsColors.Add("4");
            hairsColors.Add("5");
        }

        var hairColorsOptions = API.createListItem("Saç Rengi", "Karakterinizin saç rengini ayarlayabilirsiniz.", hairsColors, 0);
        hairColorsOptions.OnListChanged.connect(function (sender, index) {
            currentHairColor = parseInt(sender.List[index]);
            API.setPlayerClothes(player, 2, currentHair, currentHairColor);
        });

        var completeAction = API.createMenuItem("KAYDET", "");
        completeAction.Activated.connect(function (sender, item) {
            API.triggerServerEvent("SaveCharacterApperance", currentHead, currentHair, currentHairColor);
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

