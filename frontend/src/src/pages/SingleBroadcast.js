import React from 'react';
import VideoPlayer from '../components/SingleBroadcast/VideoPlayer.js';
import ChatBox from '../components/SingleBroadcast/ChatBox.js';
import '../css/single-broadcast.css';

export default function SingleBroadcast({ match }) {
  return (
      <div className="single_broadcast_page_content">
          <div className="video_player_container">
        <VideoPlayer broadcastsId={match.id} />
    </div>
   </div>
  );
}