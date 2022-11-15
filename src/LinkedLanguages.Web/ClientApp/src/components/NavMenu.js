import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem } from 'reactstrap';
import { NavLink } from 'react-router-dom';
import { LoginMenu } from './api-authorization/LoginMenu';
import './NavMenu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                    <NavbarBrand href="/">LinkedLanguages</NavbarBrand>
                    <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem>
                                <NavLink className={({isActive}) => isActive ? "active nav-link": "nav-link"} to="/">Home</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink className={({isActive}) => isActive ? "active nav-link": "nav-link"} to="/learn">Learn</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink className={({isActive}) => isActive ? "active nav-link": "nav-link"} to="/test">Test</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink className={({isActive}) => isActive ? "active nav-link": "nav-link"} to="/setup">Setup</NavLink>
                            </NavItem>
                            <LoginMenu>
                            </LoginMenu>
                        </ul>
                    </Collapse>
                </Navbar>
            </header>
        );
    }
}
