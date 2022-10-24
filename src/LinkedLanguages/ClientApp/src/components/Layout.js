import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

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
        <ToastContainer position="top-right"
          autoClose={3000}
          hideProgressBar={false}
          newestOnTop={true}
          closeOnClick
          rtl={false}
          pauseOnFocusLoss
          draggable
          pauseOnHover
          theme="light" />
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }
}
