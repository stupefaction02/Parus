import sendPost from "../network.js";

var play_button = document.getElementById("play_button");
var plays = true;


function switch_play() {
    if (plays) {
        plays = false;

        pause();
    } else {
        plays = true;

        play();
    }
}

function play() {

}

function pause() {

}

function ParseManifest() {
    var hlsServiceUrl = "https://localhost:2020";
    var playlistUrl = "hls/live/playlist";

    var serverUrl = "https://localhost:3939";
    var serverBroadcastsUrl = "api/broadcasts/sessions";

    // Channel we got previously when we loaded channel, so
    var channelId = 1;
    var localManifestUrl = `${serverUrl}/${serverBroadcastsUrl}?channelId=` + channelId;

    fetch(localManifestUrl).then(response => {

        response.json().then(function (data) {

            console.log(`Got a channel from server: ${data.channelSessionKey}`);

            debugger

            var manifestFilename = CryptoJS.MD5(data.channelSessionKey + 'master_manifest').toString();

            if (manifestFilename != "") {

                var video = document.getElementById('video');
                //var videoSrc = `${hlsServiceUrl}/${playlistUrl}?manifestFile=${manifestFilename}.m3u8`;
                var videoSrc = `${hlsServiceUrl}/${playlistUrl}?manifestFile=1.m3u8`;

                if (Hls.isSupported()) {

                    console.log('Loading url ... \n' + videoSrc);

                    var hls = new Hls();
                    // loading manifest
                    hls.loadSource(videoSrc);
                    hls.attachMedia(video);
                    hls.on(Hls.Events.MANIFEST_PARSED, function () {
                        debugger
                        video.play();
                    });

                    hls.on(Hls.Events.ERROR, function (event, data) {
                        var errorType = data.type;
                        var errorDetails = data.details;
                        var errorFatal = data.fatal;

                        debugger
                    });
                }
            }
            else {
                console.log("Can't get channel key from a server");
            }
        });
    });
}

play_button.onclick = function () {
    switch_play();
}


