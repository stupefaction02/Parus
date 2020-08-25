import React from 'react';
import './App.css';
import {BrowserRouter as Router, Switch, Route} from 'react-router-dom';

import Overview from './pages/Overview.js';
import Sidebar from './components/Sidebar.js';
import About from './pages/About.js';
import SingleBroadcast from './pages/SingleBroadcast.js';

import './css/navbar.css';
import './css/common.css';

function App() {
	
  return (
	<div className="App">
		<Router>
			<Sidebar />

			<div className="right_panel">
			<nav className="navbar">
					das
			</nav>
				<div className="page-content">
					<Switch>
						<Route path="/" exact>
							<Overview />
						</Route>
						<Route path="/about" component={About} />
						<Route path="/channel/:id" component={SingleBroadcast} />
					</Switch>
				</div>
			</div>
		</Router>
	</div>
  );
}

export default App;
