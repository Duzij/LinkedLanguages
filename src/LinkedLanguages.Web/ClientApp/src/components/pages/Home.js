import React, { Component } from 'react';
import { Link } from 'react-router-dom'

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div className='row d-flex align-items-center justify-content-center'>
        <div className='col-lg-4 col-md-12 col-sm-12'>
          <div className='d-flex flex-column align-items-end'>
            <img src="/linked_languages.png" className="rounded float-start home-image my-3" alt="Linked languages, bright colorful abstract art, high definition by DALLE"></img>
            <small className="text-muted me-1">Linked languages, bright colorful abstract art, high definition</small>
            <small className="text-muted me-1">by <a href='https://openai.com/dall-e-2/' className='text-reset' target="_blank">DALL·E</a></small>
          </div>
        </div>
        <div className='col-lg-6 col-md-12 col-sm-12'>
          <div className='align-items-start'>
            <h1>Welcome!</h1>
            <p>LinkedLanguages is a web application, which uses linked linguistic data as a database <a href="https://etytree.toolforge.org" target="_blank">EtyTree</a> for in e-learning purposes.</p>
            <p>Based on evaluation of the web application will try to investigate whether it is possible to learn new words in foreign language based on etymological links between words in foreign language and the word in language user already know.</p>
            <p>You can start in <Link to="/setup">Setup section</Link> where you can setup your known and unknown language.
              Later, in <Link to="/learn">Learn section</Link> LinkedLanguages will serve you some word pairs.</p>
          </div>
        </div>
      </div>
    );
  }
}
