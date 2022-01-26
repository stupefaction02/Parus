import React, { Component } from 'react';
import $ from 'jquery';
import Hls from 'hls.js';
import CryptoJS from 'crypto-js';

export default class VideoPlayer extends Component {

    macrscomApiUrl = 'http://demo.macroscop.com:8080';

    apiUrl = 'https://localhost:5001';

    constructor(props) {
        super(props);

        this.state = {};

        this.videoStyles = {
            width: 860,
            height: 645,
            marginLeft: 'auto',
            marginRight: 'auto',
            marginTop: '25px',
            display: 'block'
        };
    }

    componentDidMount() {

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
                    var videoSrc = `${hlsServiceUrl}/${playlistUrl}?manifestFile=${manifestFilename}.m3u8`;

                    if (Hls.isSupported()) {

                        console.log('Loading url ... \n' + videoSrc);

                        var hls = new Hls();
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

    __componentDidMount() {
        var localUrl = 'https://localhost:5001/hls/live/segment?channelId=1';

        var videoEdgeBaseUrl = 'https://video-edge-c55ff4.arn03.abs.hls.ttvnw.net/v1/segment'; 
        var segment = 'CuwE5PVwVUdIMpyWBOE72ILJ9qWnquHDFfBgwfSLow6BBkQYnAe7uh9ShvnGxOXMommoVKOdjXOoHjmT5_bCP0IhXPzauFBdx2o12ycdcfwSGi57H_kovdxA9YMwvxBJj0AbYd4W2GEY7_pNuI31s6MapUyGuNRsIZcXnmz3nCBjGqHxE7tT678ITQsQztqNrW2B899uIBUVLFk9m_xa2mAq6VwUu1lJg0JTnnLbPMNlvYmrmCNWqwjwou00_-SG3KfMM9MO9kcEXNnoVs1z8lwCcQoy3Ha7a_tNILgozYURI6sYs7hxLqLFMkH-Kqxib2DtHDKK72WgPLklphpfnr3w4YMgcswW9SCVwFjCcQRpz8622VAihJ70yNADm-69YwExZeKm2jsDIaz4Xq5xvxB2XDeOJ3EWOBDdnakvoUrj4B7zPMuNCASAFigfdFci3s2nNFsNjnnm0TJtE_0J0UsiJSTadnRs4vighXoSsGxw_V88P2Ow8K2UjeR0TgTu9dIltorzQOtEYmCZ9XscBQat_ivRG1JjYbhadJFZmRF4VN5s_E1b2Rfc33475hQiXcyxsPmAtk06b8NBbrA7sD2UimuOyzTpdgCv51AcxzlLmxEQh8hwAMs7czgf1mnffNqOrPUBDr-rlHafFQNmW6hRtBWFsJgIjHJk4vpT6xezdAGQjZeGBpzOAYAk7dfhnpLWtwF_ZkFmyInPPsopjDWRsbqvBk85-5Q7fRgcfDQqnm2X7PnArE8Aclh_gn-sCx7_TSc6yDNy1e2ZfIZC8-zcNDQQgdi6nYskd9_yVrn8clCb5fKqneiICSg4FEISENP15T9HuMnYd-2HmvOWsggaDHnY-gDfO4V9PYy-aQ.ts';

        var testUrl = `${videoEdgeBaseUrl}/${segment}`;

        var stream = fetch(testUrl)
        .then(res => {
            let reader = res.body.getReader();
            
            reader.read().then(function dataHandler({value, done}) {
                debugger
            });
        });

        console.log(testUrl);
    }   

    _componentDidMount() {
        let channelId = '2016897c-8be5-4a80-b1a3-7f79a9ec729c';
        //url: 'http://demo.macroscop.com:8080/mobile?login=root&channelid=2016897c-8be5-4a80-b1a3-7f79a9ec729c&resolutionX=640&resolutionY=480&fps=25',

        //let url = `${this.macrscomApiUrl}/mobile?login=root&channelid=${channelId}&resolutionX=640&resolutionY=480&fps=25`;
        let url = `${this.apiUrl}/channels/1`;

        var stream = fetch(url)
        .then(response => {
            const reader = response.body.getReader();

            return new ReadableStream ({
                start(controller) {
                    return pump();

                    function pump() {
                      return reader.read().then(({ done, value }) => {
                        
                        debugger;

                        if (done) {
                            controller.close();
                            return;
                        }
                        
                        controller.enqueue(value);
                        return pump();
                      });
                    }
                }
            });
        });
        
        /*
        .then(res => {
            let reader = res.body.getReader();

            let parser = new MJPEGParser(function newFrame (frame) { console.log("yoooo!"); });
            
            reader.read().then(function dataHandler({value, done}) {
            
                parser.setData(value);

                parser.start();
            });
        });*/
    }

    render() {
        return (
            <video id="video" style={this.videoStyles} controls>
                <source src="movie.mp4" type="video/mp4" />
            </video>
        )
    };

}