import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import '../css/broadcast-gallery.css';

export default class BroadcastPreview extends Component {
	
  constructor(props) {
	  super(props);
	  
	  this.state = {
		  viewers: 13
	  };
  }
  
  render() {
	const {model} = this.props;
	return (
		<div className="broadcast_item">
			<h1>Name: {model.name}</h1>
			<Link to={`/channel/${model.id}`}>
			</Link>
		</div>
	);
  }
}
