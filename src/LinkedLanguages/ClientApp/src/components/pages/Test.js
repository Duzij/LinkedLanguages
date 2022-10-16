import React, { Component } from 'react';
import LoadingSpinner from '../LoadingSpinner';
import { fetchPost, fetchGet } from './../FetchApi'

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
        };
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.reveal = this.reveal.bind(this);
    }

    componentDidMount() {
        this.fetchTestWordPair();
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
                        errorMessage: 'No word found. You can reset the progress or change your current settings to learn some new words. 👀',
                        word: "",
                        canFetchNext: false,
                        loading: false
                    });
                }
            }
        );
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
                    }, 3000);
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
        return (
            <div>
                <LoadingSpinner loading={this.state.loading} />
                <div hidden={this.state.canFetchNext} className="alert alert-danger" role="alert">
                    {this.state.errorMessage}
                </div>
                <div className='form-group col-md-12'>
                    <h1 id="tabelLabel" >User profile</h1>
                    <p>Here you can learn some of the words you approved in Learn section.</p>
                    <div className="alert alert-warning" role="alert">
                        Word pairs languages are gathered from your current settings.
                    </div>
                </div>
                <div className='d-flex justify-content-center'>
                    <form className="row g-3" hidden={!this.state.canFetchNext} onSubmit={this.handleSubmit}>
                        <div className="col-auto">
                            <label htmlFor="knownWord">Known word</label>
                            <input type="text" className="form-control" value={this.state.value} onChange={this.handleChange} id="knownWord" placeholder="known word" required />
                            <div className="invalid-feedback">
                                Provided word is not correct 🤨
                            </div>
                            <div className="valid-feedback">
                                Bravo! Keep it up! 🥳✨
                            </div>
                        </div>
                        <div className="col-auto">
                            <label >Unknown word</label>
                            <p className="form-control-plaintext">{this.state.word.unknownWord}</p>
                        </div>
                        <button type="submit" className="btn btn-primary mb-3">Submit</button>
                    </form>
                </div>
                <div className='d-flex justify-content-center'>
                    <button type="button" className="btn btn-outline-primary mb-3" onClick={this.reveal}>Reveal</button>
                </div>
            </div>
        );
    }


}
