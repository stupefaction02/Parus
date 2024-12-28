import { GetCookie, IsStringEmpty } from "./common.js";
import { CHAT_API_PATH, VIDEO_EDGE_PATH } from "./config.js";
import { HLSPlayer } from "./HlsPlayer/HLSPlayer.js";
import { ApiPostRequest } from "./site.js";
import { SignalRChat } from "./SignalrChat.js";

var play_button = document.getElementById("play_button");
var plays = true;

var hlsServiceUrl = VIDEO_EDGE_PATH;

var video = document.getElementById('video');

//var manifestUrl = `${hlsServiceUrl}/live/123456/master_playlist.m3u8`;
//debugger
var url = VIDEO_EDGE_PATH + "/live/desh02/1_dash.mpd";
var player = dashjs.MediaPlayer().create();

if (player !== null) {
    player.initialize(video, url, true);
}

if (signalR !== null) {
    var chat = new SignalRChat(signalR);
}