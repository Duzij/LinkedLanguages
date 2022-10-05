import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import LoadingSpinner from './LoadingSpinner';

export class Test extends Component {
    static displayName = Test.name;

    constructor(props) {
        super(props);
        this.state = {
            word: {},
            loading: true,
            unknownLanguageId: undefined,
            canFetchNext: true
        };
    }

    componentDidMount() {
        this.fetchTestWordPair();
    }

    async fetchTestWordPair() {
        const token = await authService.getAccessToken();
        fetch(`testwordpair/get/${this.state.unknownLanguageId}`,
            {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            }).then((response) => {
                if (response.ok) {
                    return response.json();
                }
                else {
                    console.log(response);
                    throw new Error('Something went wrong');
                }
            }).then((data) => {
                this.setState({ word: data, loading: false });
            }).catch((error) => {
                console.log(error);
                this.setState({ canFetchNext: false, loading: false })
            });
    }

    render() {
        return (
            <div>
                <LoadingSpinner loading={this.state.loading} />
                <form hidden={!this.state.canFetchNext}>
                    <div className='form-group col-md-12'>
                        <h1 id="tabelLabel" >User profile</h1>
                        <p>Here you can learn some of the words you approved in Learn section.</p>
                        <div className="alert alert-warning" role="alert">
                            Word pairs languages are gathered from your current settings.
                        </div>
                    </div>
                    <div className="d-flex justify-content-center">
                        <div className='d-flex flex-column m-3 justify-content-end'>
                            <p>Known word</p>
                            <p><b>{this.state.word.knownWord}</b></p>
                            <button type="button" className="btn btn-outline-success" onClick={this.approve.bind(this)}>
                                Submit
                            </button>
                        </div>

                        <div className='d-flex flex-column m-3 justify-content-start'>
                            <p>Unknown word</p>
                            <p><b>{this.state.word.unknownWord}</b></p>
                        </div>
                    </div>
                </form>
            </div>
        );
    }

    async sumbitWord() {
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
        this.setState({ languages: data });
    }

    
  
}
