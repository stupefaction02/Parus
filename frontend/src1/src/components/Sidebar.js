import React from 'react';
import { Link } from 'react-router-dom';
import '../css/sidebar.css';

export default function Sidebar() {

    return (
        <div className="sidebar-container">
            <div className="sidebar-logo">
                Subscriptions
            </div>
            <ul className="sidebar-navigation">
                <li>
                    <Link to="/">
                        Overview
                    </Link>
                </li>
                <li>
                    <Link to='/account'>
                        Account
                    </Link>
                </li>
                <li>
                    <Link to='/friends'>
                        Friends
                    </Link>
                </li>
                <li>
                    <Link to='/settings'>
                        Settings
                    </Link>
                </li>
                <li>
                    <Link to='/About'>
                        About
                    </Link>
                </li>
            </ul>
        </div>
  );
}