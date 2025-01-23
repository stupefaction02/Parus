import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import './App.css';
import Sidebar from './components/Sidebar.js';
import './css/common.css';
import './css/navbar.css';
import About from './pages/About.js';
import Overview from './pages/Overview.js';
import SingleBroadcast from './pages/SingleBroadcast.js';

function App() {
    return (
        <div className="App">
            <Router>
                <div className="page-content">
                    <Switch>
                        <Route path="/" exact>
                            <Overview />
                        </Route>
                        <Route path="/about" component={About} />
                        <Route path="/channel/:id" component={SingleBroadcast} />
                    </Switch>
                </div>
            </Router>
        </div>
    );
}

export default App;