import React, { Component } from 'react';
import LoadingSpinner from './../LoadingSpinner';
import authService from './../api-authorization/AuthorizeService'
import { fetchGet } from '../FetchApi';
import { NavLink } from 'react-router-dom';

export class Learn extends Component {
    static displayName = Learn.name;

    constructor(props) {
        super(props);
        this.state = {
            word: {},
            loading: true,
            unknownLanguageId: undefined,
            knownDefinitions: [],
            unknownDefinitions: [],
            canFetchNext: true,
            knownSeeAlsoLink: undefined,
            unknownSeeAlsoLink: undefined,
            errorMessage: undefined
        };
    }

    async componentDidMount() {
        fetchGet(
            "profile",
            (data) => {
                this.setState({ unknownLanguageId: data.unknownLanguages[0].value },
                    () => {
                        this.fetchNextWord()
                    });
            });
    }

    async fetchNextWord() {
        fetchGet(
            `wordpair/get/${this.state.unknownLanguageId}`,
            (data) => {
                this.setState({ word: data },
                    () => {
                        this.fetchDefinitions()
                    });
            },
            () => {
                this.setState({
                    loading: false,
                    canFetchNext: false
                });
            });
    }

    async fetchDefinitions() {
        fetchGet(
            `wordpair/definiton/${this.state.word.id}`,
            (data) => {
                this.setState({
                    knownDefinitions: data.knownDefinitions,
                    unknownDefinitions: data.unknownDefinitions,
                    loading: false
                });
            },
            (error) => {
                if (error.message === '404') {
                    this.setState({
                        errorMessage: 'No definitions found. 😶',
                        loading: false
                    });
                }
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
                            <span>For public alpha only one known language and one unknown language is supported.</span>
                            <span hidden={this.state.canFetchNext}>
                                Congratulations, you managed to learn all word pairs.
                                Continue to the <NavLink to="/test">Test</NavLink> section ✨</span>
                        </div>
                    </div>
                </div>
                <form hidden={!this.state.canFetchNext}>
                    <div className="row">
                        <div className='col align-self-center'>
                            <div className="list-group">
                                <div className="list-group-item">
                                    <div className="d-flex w-100 justify-content-between">
                                        <h5 className="mb-1">{this.state.word.knownWord}</h5>
                                        <small className="text-muted">Known word</small>
                                    </div>
                                    {
                                        this.state.knownDefinitions.map(
                                            (def) => <p key={def.toString()} className="mb-1">{def}</p>
                                        )
                                    }
                                    {
                                        this.state.word.knownSeeAlsoLink !== undefined && this.state.word.knownSeeAlsoLink !== null ?
                                            <small className="text-muted">
                                                <a className='text-reset' href={this.state.word.knownSeeAlsoLink}>See also</a>
                                            </small> : null
                                    }
                                </div>
                            </div>
                        </div>
                        <div className='col align-self-center'>
                            <div className="list-group">
                                <div className="list-group-item">
                                    <div className="d-flex w-100 justify-content-between">
                                        <h5 className="mb-1">{this.state.word.unknownWord}</h5>
                                        <small className="text-muted">Unknown word</small>
                                    </div>
                                    {
                                        this.state.unknownDefinitions.map(
                                            (def) => <p key={def.toString()} className="mb-1">{def}</p>
                                        )
                                    }
                                    {
                                        this.state.word.unknownSeeAlsoLink !== undefined && this.state.word.unknownSeeAlsoLink !== null ?
                                            <small className="text-muted">
                                                <a className='text-reset' href={this.state.word.unknownSeeAlsoLink}>See also</a>
                                            </small> : null
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="d-flex justify-content-center">
                        <div className='d-flex flex-column m-3 justify-content-end'>
                            <button type="button" className="btn btn-outline-danger" onClick={this.reject.bind(this)}>
                                Reject
                            </button>
                        </div>
                        <div className='d-flex flex-column m-3 justify-content-start'>
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
