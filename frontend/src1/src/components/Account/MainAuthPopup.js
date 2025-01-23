import React, { Component } from 'react';
import '../../css/main-auth-popup.css';
import SignalRClient from '../../services/chat/SignalRClient.js';

export default class MainAuthPopup extends Component {
    constructor(props) {
        super(props);
    }

    render() {
         return (
              <div className='auth_popup' onClick={this.props.closePopup}>
                   <div className='auth_popup_content'>
                         <div className='auth_popup_reg_content'>
                              <label>Name
                                   <input type="text" name="username" required />
                              </label> 
                         </div>
                   </div>
              </div>
         )
    };
}