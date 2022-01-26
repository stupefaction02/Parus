import React, { Component } from 'react';
import BroadcastGallery from "../components/BroadcastGallery.js";
import MainAuthPopup from "../components/Account/MainAuthPopup";
import 'react-router-dom';
import '../css/overview.css';

export default class Overview extends Component {

  constructor(props) {
    super(props);
    
    this.state = {
        broadcasts: null,
        showAuthPopup: true
    };
  }

  getBroadcasts () {
  };
  
  onSignUpButtonClicked() {
    this.toggleAuthPopup();
  };

  onSendInButtonClicked() {
    this.toggleAuthPopup();
  };

  toggleAuthPopup() {
    this.setState({
      showAuthPopup: !this.state.showAuthPopup
    });
  }

  render() {
    return (
      <div className="overview_content">
        <h4>Bow Wow Wow</h4>
  	    
        <div className="auth_buttons">
          <button className="signup_button"
                  onClick={(event) => this.onSignUpButtonClicked(event)}>
            Sign up
          </button>
          <button className="signin_button"
                  onClick={(event) => this.onSendInButtonClicked(event)}>
            Sign in
          </button>
        </div>

        {this.state.showAuthPopup ? 
          <MainAuthPopup closePopup={this.toggleAuthPopup.bind(this)} /> 
          : null
        }
      </div>
    )
  };
}

//<BroadcastGallery items={broadcasts} />