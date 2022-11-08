import React, { Component } from 'react';
import Select from 'react-select';
import makeAnimated from 'react-select/animated';
import LoadingSpinner from './../LoadingSpinner';
import { fetchGet, fetchPost } from '../FetchApi';
import { toast } from 'react-toastify';

export class Setup extends Component {
    static displayName = Setup.name;

    constructor(props) {
        super(props);
        this.state = {
            profile: {},
            languages: [],
            loading: true,
            isLoadingStatistics: true
        };

        this.handleKnownChange = this.handleKnownChange.bind(this);
        this.handleUnknownChange = this.handleUnknownChange.bind(this);
        this.saveLanguages = this.saveLanguages.bind(this);
    }

    async componentDidMount() {
        this.fetchLanguages();
    }

    async handleKnownChange(selectedKnownLanguages) {
        var profile = this.state.profile;
        profile.knownLanguages = [selectedKnownLanguages];
        await this.fetchStatistics();
    }

    async handleUnknownChange(selectedUnknownLanguages) {
        var profile = this.state.profile;
        profile.unknownLanguages = [selectedUnknownLanguages];
        await this.fetchStatistics();
    }

    render() {
        return (
            <div>
                <LoadingSpinner loading={this.state.loading} />
                <form>
                    <h1 id="tabelLabel" >User profile</h1>
                    <p>Here you can modify your profile and change known and unknown languages</p>
                    <div className="alert alert-warning" role="alert">
                        For public alpha only one known language and one unknown language is supported.
                    </div>
                    <div className='row d-flex align-items-center justify-content-center'>
                        <div className='col-lg-6 col-md-8 col-sm-12'>
                            <div class="card">
                                <h5 class="card-header">Select your known and unknown language</h5>
                                <div class="card-body">
                                    <h5 class="card-title">
                                        <span>Number of word pairs found: </span><span hidden={this.state.isLoadingStatistics}>{this.state.predicatesCount}</span>
                                        <span className="spinner-border spinner-border-sm" hidden={!this.state.isLoadingStatistics} role="status" aria-hidden="true"></span>
                                    </h5>
                                    <div className='d-flex flex-column my-3 justify-content-end'>
                                        <label>Known languages</label>
                                        <Select className="basic-select"
                                            value={this.state.profile.knownLanguages}
                                            onChange={this.handleKnownChange}
                                            options={this.state.languages}
                                            classNamePrefix="select" />
                                    </div>
                                    <div className='d-flex flex-column my-3 justify-content-start'>
                                        <label>Unknown languages</label>
                                        <Select className="basic-select"
                                            value={this.state.profile.unknownLanguages}
                                            onChange={this.handleUnknownChange}
                                            options={this.state.languages}
                                            classNamePrefix="select" />
                                    </div>
                                    <button type="button" className="btn btn-primary" onClick={this.saveLanguages}>Save</button>
                                </div>
                            </div>
                        </div>
                    </div>

                </form>

            </div >
        );
    }

    async saveLanguages() {
        fetchPost('profile',
            {
                knownLanguages: this.state.profile.knownLanguages,
                unknownLanguages: this.state.profile.unknownLanguages
            },
            (data) => {
                this.setState({
                    languages: data
                }, () => {
                    toast.success('Profile saved')
                });
            }
        );
    }

    async fetchStatistics() {
        this.setState({ isLoadingStatistics: true })
        await fetchPost('languages/statistics',
            this.state.profile,
            (data) => {
                this.setState({ predicatesCount: data, isLoadingStatistics: false });
            })
    }

    async fetchLanguages() {
        this.setState({ isLoadingStatistics: true })
        fetchGet('languages',
            (data) => {
                this.setState({ languages: data, loading: false }, () => this.fetchProfile());
            })
    }

    async fetchProfile() {
        fetchGet('profile',
            (data) => {
                this.setState(
                    { profile: data },
                    () => this.fetchStatistics()
                );
            })
    }
}
