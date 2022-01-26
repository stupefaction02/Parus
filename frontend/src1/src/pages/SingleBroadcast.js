import React from 'react';
import VideoPlayer from '../components/SingleBroadcast/VideoPlayer.js';
import ChatBox from '../components/SingleBroadcast/ChatBox.js';
import '../css/single-broadcast.css';

export default function SingleBroadcast({ match }) {
  return (
      <div className="single_broadcast_page_content">
          <div className="video_player_container container">
          </div>

          <div className="chat_box_container container">
            <ChatBox />
          </div>
      </div>
  );
}

// <VideoPlayer broadcasts Id={match.id} />