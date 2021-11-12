import { AadHttpClientFactory } from '@microsoft/sp-http';

export interface IBlogDeploymentsProps {
  description: string;
  aadHttpClientFactory: AadHttpClientFactory;
}
