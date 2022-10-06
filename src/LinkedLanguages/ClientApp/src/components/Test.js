import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import LoadingSpinner from './LoadingSpinner';

export class Test extends Component {
    static displayName = Test.name;

    constructor(props) {
        super(props);
        this.state = {
            word: {},
            submitedWord : undefined,
            loading: true,
            unknownLanguageId: undefined,
            canFetchNext: true,
            errorMessage: undefined
        };
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
            if (response.status == 401) {
                return authService.signIn();
            }
            if (response.status == 404) {
                throw new Error('No word found. You can reset the progress or change your current settings to learn some new words. 👀');
            }
            else if(response.status != 200){
                console.log(response);
                throw new Error('Something went wrong');
            }
        }).then((data) => {
            this.setState({ word: data, loading: false });
        }).catch((error) => {
            console.log(error);
            this.setState({ errorMessage: error.message, word:"", canFetchNext: false, loading: false })
        });
    }

    async handleChange(event)
    {
        this.setState({submitedWord: event.target.value});
    }

    render() {
        return (
            <div>
                <LoadingSpinner loading={this.state.loading} />
                <div hidden={this.state.canFetchNext} className="alert alert-danger" role="alert">
                    {this.state.errorMessage}
                </div>
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
                            <input type="text" value={this.state.value} onChange={this.handleChange.bind(this)} />
                            <button type="button" value="Submit" onClick={this.submitWord.bind(this)} className="btn btn-outline-success">Submit</button>
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

    async submitWord() {
        const token = await authService.getAccessToken();
        fetch('testwordpair', {
            method: 'POST',
            headers: !token ? {} : {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({wordPairId : this.state.word.id, submitedWord: this.state.submitedWord})
        }).then((response) => {
            if (response.status == 401) {
                return authService.signIn();
            }
            if (response.status == 404) {
                throw new Error('No word found. You can reset the progress or change your current settings to learn some new words. 👀');
            }
            else if(response.status != 200){
                console.log(response);
                throw new Error('Something went wrong');
            }
        }).then((data) => {
            this.fetchTestWordPair();
        }).catch((error) => {
            console.log(error);
            this.setState({ errorMessage: error.message, word:"", canFetchNext: false, loading: false })
        });
    }



}
