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
    }

    async componentDidMount() {
        this.fetchLanguages();
    }

    render() {
        let animated = makeAnimated();

        const handleKnownChange = (selectedKnownLanguages) => {
            var profile = this.state.profile;
            profile.knownLanguages = [selectedKnownLanguages];
            this.fetchStatistics();
        }

        const handleUnknownChange = (selectedUnknownLanguages) => {
            var profile = this.state.profile;
            profile.unknownLanguages = [selectedUnknownLanguages];
            this.fetchStatistics();
        }

        return (
            <div>
                <LoadingSpinner loading={this.state.loading} />
                <form>
                    <div className='form-group col-md-12'>
                        <h1 id="tabelLabel" >User profile</h1>
                        <p>Here you can modify your profile and change known and unknown languages</p>
                        <div className="alert alert-warning" role="alert">
                            For public alpha only one known language and one unknown language is supported.
                        </div>
                        <div className="alert alert-primary d-flex align-items-center" role="alert">
                            <div>
                                <span>Total number of relations:{this.state.predicatesCount}</span>
                                <span className="spinner-border spinner-border-sm" hidden={!this.state.isLoadingStatistics} role="status" aria-hidden="true"></span>
                            </div>
                        </div>
                    </div>
                    <div className="d-flex justify-content-center">
                        <div className='d-flex flex-column m-5 justify-content-end'>
                            <label>Known languages</label>
                            <Select makeAnimated={animated}
                                className="basic-select"
                                value={this.state.profile.knownLanguages}
                                onChange={handleKnownChange}
                                options={this.state.languages}
                                classNamePrefix="select" />
                        </div>
                        <div className='d-flex flex-column m-5 justify-content-start'>
                            <label>Unknown languages</label>
                            <Select makeAnimated={animated}
                                className="basic-select"
                                value={this.state.profile.unknownLanguages}
                                onChange={handleUnknownChange}
                                options={this.state.languages}
                                classNamePrefix="select" />
                        </div>
                    </div>
                    <div className='form-group col-md-12'>
                        <button type="button" className="btn btn-primary" onClick={this.saveLanguages.bind(this)}>Save</button>
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
        fetchPost('languages/statistics',
            this.state.profile,
            (data) => {
                this.setState({ predicatesCount: data, isLoadingStatistics: false });
            })
    }

    async fetchLanguages() {
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
