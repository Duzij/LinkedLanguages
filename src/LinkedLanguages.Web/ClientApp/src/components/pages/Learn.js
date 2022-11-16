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
            knownLinks: [],
            unknownLinks: [],
            knownLanguageLabel: undefined,
            unknownLanguageLabel: undefined,
            errorMessage: undefined
        };
    }

    async componentDidMount() {
        fetchGet(
            "profile",
            (data) => {
                if (data.knownLanguages.length === 0 || data.unknownLanguages.length === 0) {
                    this.setState({
                        loading: false,
                        errorMessage: "Known and unknown languages are not set, please navigate to Setup section first."
                    });
                }
                else {
                    this.setState({
                        unknownLanguageId: data.unknownLanguages[0].value,
                        unknownLanguageLabel: data.unknownLanguages[0].label,
                        knownLanguageLabel: data.knownLanguages[0].label,
                    },
                        () => {
                            this.fetchNextWord()
                        });
                }
            });
    }

    async fetchNextWord() {
        fetchGet(
            `wordpair/get/${this.state.unknownLanguageId}`,
            (data) => {
                this.setState({ word: data, loading: false },
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

    async fetchLinks() {
        fetchGet(
            `wordpair/link/${this.state.word.id}`,
            (data) => {
                this.setState({
                    knownLinks: data.knownLinks,
                    unknownLinks: data.unknownLinks
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


    async fetchDefinitions() {
        fetchGet(
            `wordpair/definiton/${this.state.word.id}`,
            (data) => {
                this.setState({
                    knownDefinitions: data.knownDefinitions,
                    unknownDefinitions: data.unknownDefinitions
                }, () => {
                    this.fetchLinks()
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
                            <span>For public alpha only one known language and one unknown language is supported. </span>
                        </div>
                        <p><span class="text-danger"><b>Reject</b></span> the word pair if you don't like it 😖</p>
                        <p><span class="text-success"><b>Approve</b></span> the word pair if it does make sense and you want to learn it 👀</p>
                        <div hidden={this.state.errorMessage === undefined} className="alert alert-danger" role="alert">
                            <span>{this.state.errorMessage}</span>
                        </div>
                        <div hidden={this.state.canFetchNext} className="alert alert-success" role="alert">
                            ✨ Congratulations, you learned all available word pairs for current known and unknown language. Continue to the <NavLink to="/test">Test</NavLink> section
                        </div>
                    </div>
                </div>
                <form hidden={!this.state.canFetchNext || this.state.errorMessage !== undefined}>
                    <div className="row row-cols-1 row-cols-md-2 g-2 mb-3">
                        <div className='col d-flex justify-content-center justify-content-lg-end'>
                            <button type="button" className="align-self-md-center btn btn-lg btn-outline-danger m-sm-1" onClick={this.reject.bind(this)}>
                                Reject
                            </button>
                        </div>
                        <div className='col d-flex justify-content-center justify-content-lg-start'>
                            <button type="button" className="align-self-md-center btn btn-lg btn-outline-success m-sm-1" onClick={this.approve.bind(this)}>
                                Approve
                            </button>
                        </div>
                    </div>
                    <div className="row row-cols-1 row-cols-md-2 g-2">
                        <div className='col'>
                            <div className="card">
                                <div className="card-header">Known word ({this.state.knownLanguageLabel})</div>
                                <div className="card-body">
                                    <h3 className="card-title">{this.state.word.knownWord}</h3>
                                    {this.state.knownDefinitions && this.state.knownDefinitions.map((def) => <p className="card-text">{def}</p>)}
                                    <p hidden={this.state.knownLinks === null} className="card-text">
                                        <small className="text-muted me-1">
                                            See also:
                                        </small>
                                        {
                                            this.state.knownLinks && this.state.knownLinks.map(
                                                (link) => <small key={link.uri} className="text-muted">
                                                    <a className='text-reset me-1' target="_blank" rel="noreferrer" href={link.uri}>{link.name}</a>
                                                </small>
                                            )
                                        }
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div className='col'>
                            <div className="card">
                                <div className="card-header">Unknown word ({this.state.unknownLanguageLabel})</div>
                                <div className="card-body">
                                    <h3 className="card-title">{this.state.word.unknownWord}</h3>
                                    {this.state.unknownDefinitions && this.state.unknownDefinitions.map((def) => <p className="card-text">{def}</p>)}
                                    <p hidden={this.state.unknownLinks === null} className="card-text">
                                        <small className="text-muted me-1">
                                            <span >See also:</span>
                                        </small>
                                        {
                                            this.state.unknownLinks && this.state.unknownLinks.map(
                                                (link) => <small key={link.uri} className="text-muted">
                                                    <a className='text-reset me-1' target="_blank" rel="noreferrer" href={link.uri}>{link.name}</a>
                                                </small>
                                            )
                                        }
                                    </p>
                                </div>
                            </div>
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
