import React from 'react';
import { Link } from 'react-router-dom';

export default function Navbar() {
	
  return (
	<ul> 
		<Link to="/">
			<li> Overview </li>
		</Link>
		<Link to="/about">
			<li> About </li>
		</Link>
	</ul>
  );
}