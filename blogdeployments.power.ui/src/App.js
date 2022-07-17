import './App.css';
import { useIsAuthenticated, AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react"
import { SignInButton } from "./SignInButton"
import { SignOutButton } from "./SignOutButton"
import { ProfileContent } from "./ProfileContent"
import { RebelContent } from './RebelContent';
import { provideFluentDesignSystem, fluentCard, fluentButton } from '@fluentui/web-components';
import { provideReactWrapper } from '@microsoft/fast-react-wrapper';
import React from 'react';

const { wrap } = provideReactWrapper(React, provideFluentDesignSystem());

export const FluentCard = wrap(fluentCard());
export const FluentButton = wrap(fluentButton());


function App() {
  const isAuthenticated = useIsAuthenticated();
  return (
    <FluentCard>
      {isAuthenticated ? <SignOutButton /> : <SignInButton />}
           <AuthenticatedTemplate>
           <ProfileContent />
           <RebelContent />
         </AuthenticatedTemplate>
         <UnauthenticatedTemplate>
           <p>You are not signed in! Please sign in.</p>
         </UnauthenticatedTemplate>
         <p>
         <small>NODE_ENV: {process.env.NODE_ENV}</small>
         </p>

    </FluentCard>
    // <div className="App">
    //   <header className="App-header">
    //     {isAuthenticated ? <SignOutButton /> : <SignInButton />}
    //     <AuthenticatedTemplate>
    //       <ProfileContent />
    //       <RebelContent />
    //     </AuthenticatedTemplate>
    //     <UnauthenticatedTemplate>
    //       <p>You are not signed in! Please sign in.</p>
    //     </UnauthenticatedTemplate>
    //     <p>
    //     <small>NODE_ENV: {process.env.NODE_ENV}</small>
    //     </p>

    //   </header>
    // </div>
  );
}

export default App;
