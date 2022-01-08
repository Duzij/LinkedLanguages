import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import Select from 'react-select';
import makeAnimated from 'react-select/animated';

export class Setup extends Component {
    static displayName = Setup.name;

    constructor(props) {
        super(props);
        this.state = { profile: {}, languages: [], loading: true };
    }

    componentDidMount() {
        this.fetchLanguages();
        this.fetchProfile();
    }

    render() {
        let animated = makeAnimated();

        const handleKnownChange = (selectedKnownLanguages) => {
            var profile = this.state.profile;
            profile.knownLanguages = selectedKnownLanguages;
            this.setState({ profile: profile });
        }

        const handleUnknownChange = (selectedUnknownLanguages) => {
            var profile = this.state.profile;
            profile.unknownLanguages = selectedUnknownLanguages;
            this.setState({ profile: profile });
        }

        return (
            <div>
                <form>
                    <div className='form-group col-md-12'>
                        <h1 id="tabelLabel" >User profile</h1>
                        <p>Here you can modify your profile and change known and unknown languages</p>
                        <div className="alert alert-info" role="alert">
                            For public alpha only one known language and one unknown language are supported.
                        </div>
                    </div>
                    <div className="form-group col-md-6">
                        <label>Known languages</label>
                        <Select
                            makeAnimated={animated}
                            //isMulti uncomment when ready for multilanguage support
                            value={this.state.profile.knownLanguages}
                            onChange={handleKnownChange}
                            options={this.state.languages}
                            className="basic-multi-select"
                            classNamePrefix="select" />

                    </div>
                    <div className="form-group col-md-6">
                        <label>Unknown languages</label>
                        <Select
                            makeAnimated={animated}
                            //isMulti uncomment when ready for multilanguage support
                            value={this.state.profile.unknownLanguages}
                            onChange={handleUnknownChange}
                            options={this.state.languages}
                            className="basic-multi-select"
                            classNamePrefix="select" />
                    </div>

                    <div className='form-group col-md-12'>
                        <button type="button" className="btn btn-primary" onClick={this.saveLanguages.bind(this)}>Save</button>
                    </div>

                </form>

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
            body: JSON.stringify({ knownLanguages: this.state.profile.knownLanguages, unknownLanguages: this.state.profile.unknownLanguages })
        });

        const data = await languagesResponse.json();
        this.setState({ languages: data, loading: false });
    }

    async fetchLanguages() {
        const token = await authService.getAccessToken();
        const languagesResponse = await fetch('languages', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });

        const data = await languagesResponse.json();
        this.setState({ languages: data, loading: false });
    }

    async fetchProfile() {
        const token = await authService.getAccessToken();
        const response = await fetch('profile', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });

        const data = await response.json();
        console.log(data);
        this.setState({ profile: data });
    }
}
