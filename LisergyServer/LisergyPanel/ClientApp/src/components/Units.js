import React, { Component } from 'react';

export class Units extends Component {
    displayName = Units.name

  constructor(props) {
    super(props);
    this.state = { units: [], loading: true };
    fetch("api/units/getSpecs")
        .then(response => response.json())
        .then(data => {
            console.log("RECEBI", Object.values(data));
            this.setState({ specs: data, loading: false });
        });
  }

  static renderSpecs(specs) {
      return (
          <div>
              {specs}
          <table className='table'>
            <thead>
              <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
              </tr>
            </thead>
            <tbody>
                      {specs.map(spec =>
                          <tr key={spec}>
                              <td>{spec}</td>
                              <td>{spec}</td>
                              <td>{spec}</td>
                              <td>{spec}</td>
                    </tr>
                )}
            </tbody>
            </table>
        </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
        : Units.renderSpecs(this.state.specs);

    return (
      <div>
        {contents}
      </div>
    );
  }
}
