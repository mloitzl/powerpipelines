import * as React from 'react';
import styles from './BlogDeployments.module.scss';
import { IBlogDeploymentsProps } from './IBlogDeploymentsProps';
import { AadHttpClient, HttpClientResponse } from '@microsoft/sp-http';
import { Spinner } from 'office-ui-fabric-react';
import { IBlogDeploymentsState } from './IBlogDeploymentsState';
import { IWeather } from "./IWeather";
import { AadHttpClientFactory } from '@microsoft/sp-http';

export default class BlogDeployments extends React.Component<IBlogDeploymentsProps, IBlogDeploymentsState> {


  aadHttpClientFactory: AadHttpClientFactory;

  constructor(props: IBlogDeploymentsProps, state) {
    super(props);

    this.aadHttpClientFactory = props.aadHttpClientFactory;
    this.state = {
      loading: true,
      weather: []
    };

  }

  public componentDidMount(): void {
    this.loadDeployments();
  }

  loadDeployments() {
    this.aadHttpClientFactory
      .getClient('api://com.loitzl.test/blogdeployments')
      .then((client: AadHttpClient): void => {
        client
          .get('https://localhost:5001/WeatherForecast', AadHttpClient.configurations.v1)
          .then(response => response.json())
          .then(o => {
            this.setState(
              (previousState: IBlogDeploymentsState, curProps: IBlogDeploymentsProps): IBlogDeploymentsState => {
                previousState.loading = false;
                previousState.weather = o;
                return previousState;
              });
          });
      });
  }

  public render(): React.ReactElement<IBlogDeploymentsProps> {
    const loading: JSX.Element = this.state.loading ? <div style={{ margin: '0 auto' }}><Spinner label={'Loading...'} /></div> : <div />;
    
    const weather: JSX.Element[] = this.state.weather.map((w: IWeather, i: number) => {
      return (
        <div>{w.summary} ({new Date(w.date).toLocaleDateString()}) {w.temperatureC}</div>
      );
    });
    
    return (
      <div className={styles.blogDeployments}>
        {loading}
        {weather}
        <div style={{ clear: 'both' }} />
      </div>
    );
  }
}
