import React, { Component } from 'react';
import LoadingSpinner from './LoadingSpinner';
import authService from './api-authorization/AuthorizeService'
import axios from 'axios';

export class Learn extends Component {
    static displayName = Learn.name;

    constructor(props) {
        super(props);
        this.state = {
            word: {},
            loading: true,
            unknownLanguageId: undefined,
            canFetchNext: true
        };
    }

    async componentDidMount() {
        const token = await authService.getAccessToken();
        axios.get('profile', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        }).then((response) => {
            const profile = response.data;
            console.log(profile);
            this.setState({ unknownLanguageId: profile.unknownLanguages[0].value }, () => { this.fetchNextWord() });
        }).catch((error) => {
            console.log(error);
        });
    }

    async fetchNextWord() {
        const token = await authService.getAccessToken();
        fetch(`wordpair/get/${this.state.unknownLanguageId}`,
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
                <div className="row">
                    <div className='form-group col-md-12'>
                        <h1>Learn</h1>
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
                    <div className="d-flex justify-content-center">
                        <div className='d-flex flex-column m-3 justify-content-end'>
                            <p>Known word</p>
                            <p><b>{this.state.word.knownWord}</b></p>
                            <button type="button" className="btn btn-outline-danger" onClick={this.reject.bind(this)}>
                                Reject
                            </button>
                        </div>
                        <div className='d-flex flex-column m-3 justify-content-start'>
                            <p>Unknown word</p>
                            <p><b>{this.state.word.unknownWord}</b></p>
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
        this.setState({ loading: true });
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
        this.setState({ loading: true });
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

}
