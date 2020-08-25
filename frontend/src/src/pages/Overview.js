import React, { useState, useEffect } from 'react';
import BroadcastGallery from "../components/BroadcastGallery.js";
import 'react-router-dom';

export default function Overview() {	
  
  useEffect(() => { 
	getBroadcasts();
  }, []);
 
  const [broadcasts, setItems] = useState([]);
 
  const getBroadcasts = async () => {
	
	
    setItems(broadcasts);
  };
  
  return (
	<BroadcastGallery items={broadcasts} />
  );
}