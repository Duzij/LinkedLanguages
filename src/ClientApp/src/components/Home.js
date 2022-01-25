import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
            <h1>Welcome!</h1>
            <p>LinkedLanguages is a web application, which uses linked linguistic data as a database <a href="https://etytree.toolforge.org">EtyTree</a> for in e-learning purposes.</p>
            <p>Based on evaluation of the web application will try to investigate whether it is possible to learn new words in foreign language based on etymological links between words in foreign language and the word in language user already know.</p>
            <p>You can start in <Link to="/setup">Setup section</Link> where you can setup your known and unknown language.
                Later, in <Link to="/learn">Learn section</Link> LinkedLanguages will serve you some word pairs.</p>
      </div>
    );
  }
}
