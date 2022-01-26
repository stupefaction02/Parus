import React, { Component } from 'react';
import BroadcastPreview from "./BroadcastPreview.js";
import '../css/broadcast-gallery.css';
import $ from 'jquery';

class BroadcastGallery extends Component {

  constructor(props) {
    super(props);

    this.state = {
      items: []
    };
  }

  getBroadcastPreviews() {
    console.log(this.props.items);
	  return this.props.items.map((item) => ( 
      <BroadcastPreview model={item} />
     ));
  }
  
  componentDidMount() {
    $.ajax({
      url: 'https://localhost:5001/api/broadcasts',
      method: 'GET',
      dataType: 'JSON',
      success: data => {
        this.state = {
          items: data
        };
      },
      error: function(msg) {
        console.log("Broadcast Error Response");
      }
    });
  }

  render() {
    return (   
		  <div className="broadcast_gallery">
  			{this.getBroadcastPreviews()}
		  </div>
	  )
  }
}

export default BroadcastGallery;