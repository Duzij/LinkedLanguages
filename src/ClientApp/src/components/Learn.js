import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import axios from 'axios';

export class Learn extends Component {
    static displayName = Learn.name;

    constructor(props) {
        super(props);
        this.state = {
            word: {},
            loading: true,
            unknownLanguageId: {},
            canFetchNext: true
        };
    }

    async componentDidMount() {
        const token = await authService.getAccessToken();

        axios.get('profile', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then(res => {
            const profile = res.data;
            console.log(profile);
            this.setState({ unknownLanguageId: profile.unknownLanguages[0].value });

            this.fetchNextWord();
        });
    }

    render() {

        return (
            <div>
                <div className="row">
                    <div className='form-group col-md-12'>
                        <h1 id="tabelLabel" >Learn</h1>
                        <p>In this section you can learn some new words</p>
                        <div className="alert alert-primary" role="alert">
                            For public alpha only one known language and one unknown language is supported.
                            <span hidden={this.state.canFetchNext}>
                                 Congratulations, you managed to learn all word pairs.
                                Continue to the test section ✨</span>
                        </div>
                    </div>
                </div>
                <form hidden={!this.state.canFetchNext}>
                    <div className="row">
                        <div className="form-group col-md-6 text-right">
                            <div>
                                <p>Known word</p>
                                <b>{this.state.word.knownWord}</b>
                            </div>
                        </div>
                        <div className="form-group col-md-6">
                            <div>
                                <i className="bi bi-arrow-right"></i>
                                <p>Unknown word</p>
                                <b>{this.state.word.unknownWord}</b>
                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="form-group col-md-6 text-right">
                            <button type="button" className="btn btn-outline-danger" onClick={this.reject.bind(this)}>
                                Reject
                            </button>
                        </div>
                        <div className="form-group col-md-6">
                            <button type="button" className="btn btn-outline-success" onClick={this.approve.bind(this)}>
                                Approve
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        );
    }

    async approve() {
        const token = await authService.getAccessToken();
        const approveResponse = await fetch(`wordpair/approve/${this.state.word.id}`, {
            method: 'POST',
            headers: !token ? {} : {
                'Authorization': `Bearer ${token}`
            },
        });

        console.log(approveResponse);
        if (approveResponse.status === 200) {
            this.fetchNextWord();
        }
    }

    async reject() {
        const token = await authService.getAccessToken();
        const rejectResponse = await fetch(`wordpair/reject/${this.state.word.id}`, {
            method: 'POST',
            headers: !token ? {} : {
                'Authorization': `Bearer ${token}`
            },
        });

        console.log(rejectResponse);
        if (rejectResponse.status === 200) {
            this.fetchNextWord();
        }
    }

    async fetchNextWord() {
        if (this.state.canFetchNext) {
            const token = await authService.getAccessToken();

            fetch(`wordpair/get/${this.state.unknownLanguageId}`, {
                method: 'GET',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            }).then((response) => {
                if (response.ok) {
                    return response.json();
                }
                else{
                    console.log(response);
                    throw new Error('Something went wrong');
                }
            }).then((data) => {
                this.setState({ word: data });
            }).catch((error) => {
                console.log(error);
                this.setState({ canFetchNext: false })
            });
        }
    }
}
