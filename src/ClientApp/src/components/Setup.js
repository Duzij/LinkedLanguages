import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import Select from 'react-select';
import makeAnimated from 'react-select/animated';
import Form from 'reactstrap/lib/Form';

export class Setup extends Component {
    static displayName = Setup.name;

    constructor(props) {
        super(props);
        this.state = { profile: null, languages: null, knownLanguages: [], selectedLanguages: [], loading: true };
    }

    componentDidMount() {
        this.fetchLanguages();
        this.fetchProfile();
    }

    // static renderLanguages(savedLanguages, languages) {
    //     const animatedComponents = makeAnimated();

    //     return (

    //     );
    // }

    render() {

        // let languages = this.state.loading
        //     ? <p><em>Loading...</em></p>
        //     : Setup.renderLanguages(, , this.state.selectedLanguages);

        return (
            <div>
                <h1 id="tabelLabel" >User profile</h1>
                <p>Here you can modify your profile and change known and unknown languages</p>
                <div className="row">
                    <div className="col-md-6">
                        <p>Known languages</p>

                        <Select
                            //animatedComponents={animatedComponents}
                            isMulti
                            defaultValue={this.state.knownLanguages}
                            onChange={(selectedLanguages) => this.setState({ selectedLanguages })}
                            name="colors"
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
                    <button className="btn btn-primary" onClick={this.incrementCounter.bind(this)}>Save</button>
                </div>
            </div>
        );
    }

    incrementCounter() {
        console.log('saveLanguages' + this.state.selectedLanguages);
    }

    saveLanguages = () => {

        console.log('saveLanguages' + this.state.selectedLanguages);

        // const token = await authService.getAccessToken();
        // const languagesResponse = await fetch('profile', {
        //     method: 'POST',
        //     headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
        //     body: JSON.stringify({ knownLanguages: this.state.knownLanguages })
        // });

        // const data = await languagesResponse.json();
        // console.log(data);
        // this.setState({ languages: data, loading: false });
    }

    async fetchLanguages() {
        const token = await authService.getAccessToken();
        const languagesResponse = await fetch('languages', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });

        const data = await languagesResponse.json();
        console.log(data);
        const languages = data.map((item) => ({ value: item.id, label: item.languageName }));
        this.setState({ languages: languages, loading: false });
    }

    async fetchProfile() {
        const token = await authService.getAccessToken();
        const response = await fetch('profile', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });

        const data = await response.json();
        console.log(data);
        if (data.knownLanguages) {
            const knownLanguages = data.knownLanguages.map((item) => ({ value: item.id, label: item.languageName }));
            this.setState({ knownLanguages: knownLanguages });
        }
    }
}
