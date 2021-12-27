import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'

export class Setup extends Component {
  static displayName = Setup.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderProfile(languages) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Name</th>
            <th>Code</th>
          </tr>
        </thead>
        <tbody>
          {languages.map(lang =>
            <tr key={lang.id}>
              <td>{lang.languageName}</td>
              <td>{lang.code}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Setup.renderProfile(this.state.languages);

    return (
      <div>
        <h1 id="tabelLabel" >User profile</h1>
        <p>Here you can modify your profile and change known and unknown languages</p>
        <div className="row">
            <div className="col-md-6">
            <p>Known languages</p>
             {contents}
            </div>
            <div className="col-md-6">
             <p>Unknown languages</p>
             {contents}
            </div>
        </div>
      </div>
    );
  }

  async populateWeatherData() {
    const token = await authService.getAccessToken();
    const response = await fetch('languages', {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });

    const data = await response.json();
    console.log(data);
    this.setState({ languages: data, loading: false });
  }
}
