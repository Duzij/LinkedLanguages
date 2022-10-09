import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { ToastContainer } from 'react-toastify';

export class Layout extends Component {
  static displayName = Layout.name;

  constructor(props) {
    super(props);
    this.state = {
      errors: []
    };
  }

  render() {
    return (
      <div>
        <NavMenu />
        <ToastContainer />
        <Container>
            {this.props.children}
        </Container>
      </div>
    );
  }
}
