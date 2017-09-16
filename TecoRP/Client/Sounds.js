API.onServerEventTrigger.connect(function (name, args) {
    if (name == "start_audio") {
        API.startMusic(args[0], false);
    }
});