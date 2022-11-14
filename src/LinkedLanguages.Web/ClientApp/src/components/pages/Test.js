import React, { Component } from 'react';
import LoadingSpinner from '../LoadingSpinner';
import { fetchPost, fetchGet } from './../FetchApi'
import { NavLink } from 'react-router-dom';

export class Test extends Component {
    static displayName = Test.name;

    constructor(props) {
        super(props);
        this.state = {
            word: {},
            submitedWord: undefined,
            loading: true,
            unknownLanguageId: undefined,
            canFetchNext: true,
            errorMessage: undefined,
            unknownLanguageLabel: undefined,
            knownLanguageLabel: undefined,
            statistics: [],
            notLearnedStatistics: []
        };
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.reveal = this.reveal.bind(this);
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
                        unknownLanguageLabel: data.unknownLanguages[0].label,
                        knownLanguageLabel: data.knownLanguages[0].label,
                    },
                        () => {
                            this.fetchTestWordPair();
                        });
                }
            });
    }

    async fetchNotLearnedStatistics() {
        fetchGet(
            "testwordpair/allstatistics",
            (data) => {
                this.setState({ notLearnedStatistics: data })
            });
    }

    async fetchStatistics() {
        fetchGet(
            "testwordpair/statistics",
            (data) => {
                this.setState({ statistics: data })
            });
    }

    async fetchTestWordPair() {
        await fetchGet(
            'testwordpair',
            (data) => {
                this.setInputValidity();
                var input = document.getElementById("knownWord");
                input.value = "";
                this.setState({ word: data, loading: false });
            },
            (error) => {
                if (error.message === '404') {
                    this.setState({
                        word: "",
                        canFetchNext: false,
                        loading: false
                    });
                }
            }
        );
        this.fetchNotLearnedStatistics();
        this.fetchStatistics();
    }

    async reveal() {
        await fetchPost(
            'testwordpair/reveal',
            {
                wordPairId: this.state.word.id,
                submitedWord: this.state.submitedWord
            },
            (data) => {
                var input = document.getElementById("knownWord");
                input.value = data;
                this.setState({ submitedWord: data });
                this.setInputValidity();
            }
        );
    }

    async handleSubmit(event) {
        event.preventDefault();
        await fetchPost(
            'testwordpair',
            {
                wordPairId: this.state.word.id,
                submitedWord: this.state.submitedWord
            },
            (data) => {
                if (data) {
                    this.setInputValidity(true);
                    setTimeout(() => {
                        this.fetchTestWordPair();
                    }, 2000);
                }
            },
            (error) => {
                this.setInputValidity(false)
                if (error.status === 400) {
                    this.setInputValidity(false)
                    return false;
                }
            }
        );
    }

    async handleChange(event) {
        this.setInputValidity();
        this.setState({ submitedWord: event.target.value });
    }

    setInputValidity(valid) {
        var input = document.getElementById("knownWord");

        if (valid === undefined) {
            input.classList.remove("is-invalid");
            input.classList.remove("is-valid");
            return;
        }

        if (valid)
            input.classList.add("is-valid");
        else {
            input.classList.add("is-invalid");
            input.classList.remove("is-valid");
        }
    }

    render() {

        const overallSuccess = this.state.statistics.map((lang) => lang.successRate).reduce((a, b) => a + b, 0) / this.state.statistics.length;

        return (
            <div>
                <LoadingSpinner loading={this.state.loading} />
                <div className="row">
                    <div className='form-group col-md-12'>
                        <h1>Test</h1>
                        <p>Here you can test your knowledge of the words you approved in the Learn section.</p>
                        <p>All word pairs you see here are retrieved based on your current saved known ({this.state.knownLanguageLabel}) and unknown language ({this.state.unknownLanguageLabel}).</p>
                        <div hidden={this.state.errorMessage === undefined} className="alert alert-danger" role="alert">
                            <span>{this.state.errorMessage}</span>
                        </div>
                        <div hidden={this.state.canFetchNext} className="alert alert-success" role="alert">
                            ✔ You exceeded all word pairs for {this.state.unknownLanguageLabel} language. Continue back to <NavLink to="/learn">Learn section</NavLink> to approve more word pairs.
                        </div>
                    </div>
                </div>
                <div className='row d-flex align-items-center justify-content-center'>
                    <div className='col-lg-8 col-md-12 col-sm-12'>
                        <div hidden={this.state.notLearnedStatistics.length === 0} className='mt-3'>
                            <div className="card">
                                <div className="card-header">Statistics of all foreign words</div>
                                <div className="card-body">
                                    <p className="card-text">
                                       Looks like you also approved some words pairs for different language comination. 🤔
                                    </p>
                                    <p className="card-text">
                                    To test the words from other foreign languages, navigate to <NavLink to="/setup">Setup section</NavLink> to change the settings.
                                    </p>
                                    {
                                        this.state.notLearnedStatistics && this.state.notLearnedStatistics.map(
                                            (lang) => <small key={lang.name} className="text-muted">
                                                <p>{lang.name} words not learned: <b>{lang.wordsNotLearnedCount}</b></p>
                                            </small>
                                        )
                                    }
                                </div>
                            </div>
                        </div>
                        <form className='mt-3' hidden={!this.state.canFetchNext || this.state.errorMessage !== undefined} onSubmit={this.handleSubmit}>
                            <div className="card">
                                <div className="card-body">
                                    <div className='row'>
                                        <div className="col-md-6">
                                            <label htmlFor="knownWord">Known word ({this.state.knownLanguageLabel})</label>
                                            <div className="input-group my-3">
                                                <input type="text" className="form-control" value={this.state.value} onChange={this.handleChange} id="knownWord" placeholder="known word" required />
                                                <button className="btn btn-outline-secondary" type="button" onClick={this.reveal}>Reveal</button>
                                            </div>
                                        </div>
                                        <div className="col-md-6">
                                            <label>Unknown word ({this.state.unknownLanguageLabel})</label>
                                            <p className="my-3 form-control-plaintext">{this.state.word.unknownWord}</p>
                                        </div>
                                        <div className="col-12">
                                            <button type="submit" className="btn btn-primary">Submit</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </form>
                        <div className='mt-3'>
                            <div className="card">
                                <div className="card-header">Success rates</div>
                                <div className="card-body">
                                    <h5 className="card-title">How statistics is calculated?</h5>
                                    <p className="card-text">Word pair learning success ratios are calculated as one (1) divided by a total number of failed submissions.</p>
                                    <p className="card-text">If word pair is revealed, success ratio for this particular word pair is set to zero.</p>
                                    <p className="card-text">Language success rate, that you can see below, is an average of all learned word pairs success rates for this language.</p>
                                    {

                                        this.state.statistics && this.state.statistics.map(
                                            (lang) => <small key={lang.name} className="text-muted">
                                                <p>{lang.name}: <b>{lang.successRate}%</b> (words learned {lang.wordsCount})</p>
                                            </small>
                                        )
                                    }
                                    <hr />
                                    <p className="card-text">Overall success rate: <b>{overallSuccess}%</b></p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
