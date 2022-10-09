import React, { Component } from 'react';
import authService from '../api-authorization/AuthorizeService'
import LoadingSpinner from '../LoadingSpinner';
import { toast } from 'react-toastify';
import {fetchPost} from './../FetchApi'

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
    }

    componentDidCatch(error, errorInfo) {
        toast.error(errorInfo, {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: false,
            progress: undefined,
        });
    }

    componentDidMount() {
        this.fetchTestWordPair();
    }

    async fetchTestWordPair() {
        const token = await authService.getAccessToken();
        fetch('testwordpair', {
            headers: !token ? {} : {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                return response.json();
            }
            if (response.status === 401) {
                authService.signIn();
                this.fetchTestWordPair();
            }
            if (response.status === 404) {
                throw new Error('No word found. You can reset the progress or change your current settings to learn some new words. 👀');
            }
            if (!response.ok) {
                console.log(response);
                throw new Error('Something went wrong');
            }
        }).then((data) => {
            this.setInputValidity();
            var input = document.getElementById("knownWord");
            input.value = "";
            this.setState({ word: data, loading: false });
        }).catch((error) => {
            console.log(error);
            this.setState({ errorMessage: error.message, word: "", canFetchNext: false, loading: false })
        });
    }

    async handleChange(event) {
        this.setInputValidity();
        this.setState({ submitedWord: event.target.value });
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
                            <input type="text" className="form-control" value={this.state.value} onChange={this.handleChange.bind(this)} id="knownWord" placeholder="known word" required />
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
                    <button type="button" className="btn btn-outline-primary mb-3" onClick={this.reveal.bind(this)}>Reveal</button>
                </div>
            </div>
        );
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

}
