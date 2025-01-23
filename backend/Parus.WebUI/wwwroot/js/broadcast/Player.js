import HlsPlayer from "../HlsPlayer/HLSPlayer.js"
console.log(exports);
var play_button = document.getElementById("play_button");
var plays = true;

var hlsServiceUrl = "https://localhost:2020";

var video = document.getElementById('video');

var manifestUrl = `${hlsServiceUrl}/live/123456/master_playlist.m3u8`;

var hlsPlayer = new HlsPlayer(video, manifestUrl);

setTimeout(() =>
{
    hlsPlayer.Play();
}, 1000);

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

//fetch("https://localhost:2020/b.mp4")
//    .then(function (response) {
//        return response.blob()
//    })
//    .then(function (blob) {
//        //document.getElementById("video").src = URL.createObjectURL(blob);
//    });

//var observer = new MutationObserver(function (mutations) {
//    mutations.forEach(function (mutation) {
//        if (mutation.type === "attributes") {
//            console.log("Src attribute changed");
//        }
//    });
//});

//observer.observe(document.getElementById("video"), {
//    attributes: true //configure it to listen to attribute changes
//});

//function ParseManifest() {
//    var hlsServiceUrl = "https://localhost:2020";
//    var playlistUrl = "";

//    var serverUrl = "https://localhost:3939";
//    var serverBroadcastsUrl = "api/broadcasts/sessions";

//    // Channel we got previously when we loaded channel, so
//    var channelId = 1;
//    var localManifestUrl = `${serverUrl}/${serverBroadcastsUrl}?channelId=` + channelId;

//    var video = document.getElementById('video');
   
//    var videoSrc = `${hlsServiceUrl}/live/123456/master_playlist.m3u8`;

//    var config = {
//        debug: true,
//        enableWorker: false,
//        lowLatencyMode: false,
//        backBufferLength: 290,
//        startPosition: 0,
//        capLevelToPlayerSize: false
//    };

//    var hls = new Hls(config);

//    console.log("Hls.isSupported() = " + Hls.isSupported());

//    // loading manifest
//    hls.loadSource(videoSrc);
//    hls.attachMedia(video);
//    hls.on(Hls.Events.MANIFEST_PARSED, function (e, data) {
//        console.log("Manifest has parsed!"); /*debugger*/
//        //video.play();
//    });

//    hls.on(Hls.Events.FRAG_LOADED, function (e, data) {
//       /* debugger*/
//    })

//    hls.on(Hls.Events.FRAG_PARSED, function (e, data) {
//        /*debugger*/
//    })

//    hls.on(Hls.Events.ERROR, function (event, data) {
//        console.log("Manifest loading error!" + " type: " + data.type + " details: " + data.details + ", message: " + data.error.message);

//        //debugger
//    });

//    //fetch(localManifestUrl).then(response => {

//    //    response.json().then(function (data) {

//    //        console.log(`Got a channel from server: ${data.channelSessionKey}`);

//    //        debugger

//    //        var manifestFilename = CryptoJS.MD5(data.channelSessionKey + 'master_manifest').toString();

//    //        if (manifestFilename != "") {

//    //            var video = document.getElementById('video');
//    //            //var videoSrc = `${hlsServiceUrl}/${playlistUrl}?manifestFile=${manifestFilename}.m3u8`;
//    //            var videoSrc = `${hlsServiceUrl}/${playlistUrl}?manifestFile=1.m3u8`;

//    //            if (Hls.isSupported()) {

//    //                console.log('Loading url ... \n' + videoSrc);

//    //                var hls = new Hls();
//    //                // loading manifest
//    //                hls.loadSource(videoSrc);
//    //                hls.attachMedia(video);
//    //                hls.on(Hls.Events.MANIFEST_PARSED, function () {
//    //                    debugger
//    //                    video.play();
//    //                });

//    //                hls.on(Hls.Events.ERROR, function (event, data) {
//    //                    var errorType = data.type;
//    //                    var errorDetails = data.details;
//    //                    var errorFatal = data.fatal;

//    //                    debugger
//    //                });
//    //            }
//    //        }
//    //        else {
//    //            console.log("Can't get channel key from a server");
//    //        }
//    //    });
//    //});
//}

play_button.onclick = function () {
    switch_play();
}

ParseManifest();

//var arrayOfBlobs = [];
//setInterval(function () {
//    arrayOfBlobs.append(nextChunk());
//    // NEW: Try to flush our queue of video data to the video element
//    appendToSourceBuffer();
//}, 1000);

//// 1. Create a `MediaSource`
//var mediaSource = new MediaSource();

//// 2. Create an object URL from the `MediaSource`
//var url = URL.createObjectURL(mediaSource);

//// 3. Set the video's `src` to the object URL
//var video = document.getElementById("video");
//video.src = url;

//// 4. On the `sourceopen` event, create a `SourceBuffer`
//var sourceBuffer = null;
//mediaSource.addEventListener("sourceopen", function () {
//    // NOTE: Browsers are VERY picky about the codec being EXACTLY
//    // right here. Make sure you know which codecs you're using!
//    sourceBuffer = mediaSource.addSourceBuffer("video/webm; codecs=\"opus,vp8\"");

//    // If we requested any video data prior to setting up the SourceBuffer,
//    // we want to make sure we only append one blob at a time
//    sourceBuffer.addEventListener("updateend", appendToSourceBuffer);
//});

//// 5. Use `SourceBuffer.appendBuffer()` to add all of your chunks to the video
//function appendToSourceBuffer() {
//    if (
//        mediaSource.readyState === "open" &&
//        sourceBuffer &&
//        sourceBuffer.updating === false
//    ) {
//        sourceBuffer.appendBuffer(arrayOfBlobs.shift());
//    }

//    // Limit the total buffer size to 20 minutes
//    // This way we don't run out of RAM
//    if (
//        video.buffered.length &&
//        video.buffered.end(0) - video.buffered.start(0) > 1200
//    ) {
//        sourceBuffer.remove(0, video.buffered.end(0) - 1200)
//    }
//}

