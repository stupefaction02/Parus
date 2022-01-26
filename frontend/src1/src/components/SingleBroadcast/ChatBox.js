import React, { Component } from 'react';
import '../../css/chatbox.css';
import SignalRClient from '../../services/chat/SignalRClient.js';

export default class ChatBox extends Component {
    constructor(props) {
        super(props);
        
        this.state = {
            currentInput: "",
            client: null
        };
    }

    componentDidMount() {
        const chatServiceUrl = 'https://localhost:3939';

        const client = new SignalRClient();

        this.setState({ client: client });

        client.connect(chatServiceUrl + '/chat');
    }

    chatInputValueChanged(event) {
        this.setState({currentChatBoxText: event.currentTarget.value});
    }

    onSendButtonClicked(event) {
        const inputText = this.state.currentInput;
        const client = this.state.client;

        console.log('Sending message ' + inputText);

        //client.invoke('Send', inputText);

        client.send(inputText);
    }

    render() {
        return (
            <div className="chatbox">
                <h2>ChatBox</h2>

                <div className="chat_body">

                    <div className="chat_message">
                        <p className="chat_message_sender">User1:</p>
                        <p className="chat_message_text">Hello there YohiYo</p>
                    </div>
                    
                </div>

                <div className="chat_send_form">
                    <div>
                        <input className="chat_textbox"
                               onChange={(event) => this.chatInputValueChanged(event)}></input>
                        <button className="chat_send_btn"
                                onClick={(event) => this.onSendButtonClicked(event)}>Send</button>
                    </div>
                </div>
            </div>
        )
    };
}