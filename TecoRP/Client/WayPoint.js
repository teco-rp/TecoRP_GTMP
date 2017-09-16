API.onServerEventTrigger.connect(function (name, args) {
    if (name == "update_waypoint") {
        API.setWaypoint(args[0], args[1]);
    }
});


API.onServerEventTrigger.connect(function (name, args) {
    if (name == "update_marker") {
        API.createMarker(2, new Vector3(args[0], args[1], args[2]), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 155, 155, 155, 100);
    }
});


//-------marker---------
var lastMarker = null;
var lastBlip = null;
var missionBlip = null;
var missionText = null;
var res_Y = API.getScreenResolutionMaintainRatio().Height;
var player = API.getLocalPlayer();
API.onServerEventTrigger.connect(function (name, args) {
    
    if (name == "create_marker") {
        lastMarker = API.createMarker(1, new Vector3(args[0], args[1], args[2]), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(2, 2, 1), 255, 255, 13, 255);
        lastBlip = API.createBlip(new Vector3(args[0], args[1], args[2]));
        API.showBlipRoute(lastBlip, true);
    }
    if (name == "remove_marker") {
        API.deleteEntity(lastMarker);
        API.deleteEntity(lastBlip);
    }
    if (name == "mission_marker_show") {
        if (missionBlip != null) {
            API.deleteEntity(missionBlip);
        }
        missionBlip = API.createBlip(new Vector3(args[0], args[1], args[2]));
        missionText = args[4];
        API.setBlipColor(missionBlip, 11);      
        API.setBlipSprite(missionBlip, args[3]);
        API.showBlipRoute(missionBlip, true);
        API.setBlipRouteColor(missionBlip, 11);

    }
    if (name == "mission_marker_hide") {
        if (missionBlip != null) {
            API.deleteEntity(missionBlip);
        }
        missionText = null;
    }
});
API.onUpdate.connect(function () {
    if (missionText != null) {
        API.drawText(missionText, 15, res_Y / 2, 1, 10, 240, 20, 180, 1, 0, false, false, 0);
    }
});