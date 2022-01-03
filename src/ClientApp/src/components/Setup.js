import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import Select from 'react-select';
import makeAnimated from 'react-select/animated';
import Form from 'reactstrap/lib/Form';

export class Setup extends Component {
    static displayName = Setup.name;

    constructor(props) {
        super(props);
        this.state = { profile: null, languages: [], knownLanguages: [], selectedLanguages: [], loading: true };
    }

    componentDidMount() {
        this.fetchLanguages();
        this.fetchProfile();

    }

    render() {
        let animated = makeAnimated();

        const handleChange = (selectedLanguages) => {
            this.setState({ selectedLanguages });
            this.state.knownLanguages = selectedLanguages;
        }

        return (
            <div>
                <h1 id="tabelLabel" >User profile</h1>
                <p>Here you can modify your profile and change known and unknown languages</p>
                <div className="row">
                    <div className="col-md-6">
                        <p>Known languages</p>

                        <Select
                            makeAnimated={animated}
                            isMulti
                            value={this.state.knownLanguages}
                            onChange={handleChange}
                            options={this.state.languages}
                            className="basic-multi-select"
                            classNamePrefix="select" />

                    </div>
                    <div className="col-md-6">
                        <p>Unknown languages</p>
                        {/* {languages} */}
                    </div>
                </div>
                <div className="row">
                    <button className="btn btn-primary" onClick={this.saveLanguages.bind(this)}>Save</button>
                </div>
            </div>
        );
    }

    async saveLanguages() {
        const token = await authService.getAccessToken();
        const languagesResponse = await fetch('profile', {
            method: 'POST',
            headers: !token ? {} : {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ knownLanguages: this.state.selectedLanguages, unknownLanguages: this.state.selectedLanguages })
        });

        const data = await languagesResponse.json();
        console.log(data);
        this.setState({ languages: data, loading: false });
    }

    async fetchLanguages() {
        const token = await authService.getAccessToken();
        const languagesResponse = await fetch('languages', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });

        const data = await languagesResponse.json();

        var d = data.filter(x => !this.state.knownLanguages.includes(x))

        console.log(d);

        this.setState({ languages: d, loading: false });
    }

    async fetchProfile() {
        const token = await authService.getAccessToken();
        const response = await fetch('profile', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });

        const data = await response.json();
        console.log(data);
        if (data.knownLanguages) {
            this.setState({ knownLanguages: data.knownLanguages });
        }
    }
}
