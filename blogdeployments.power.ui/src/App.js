import { Box, Button, Collapsible, Heading, Grommet, ResponsiveContext } from 'grommet'
import { Notification } from 'grommet-icons'
import { AppBar } from "./AppBar"
import { useIsAuthenticated, AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react"
import { SignInButton } from "./SignInButton"
import { SignOutButton } from "./SignOutButton"
import { ProfileContent } from "./ProfileContent"
import { RebelContent } from './RebelContent';
import React, { useState } from 'react';

function App() {

  const theme = {
    global: {
      colors: {
        brand: '#228BE6',
      },
      font: {
        family: 'Roboto',
        size: '18px',
        height: '20px',
      },
    },
  };

  const isAuthenticated = useIsAuthenticated();

  const [showSidebar, setShowSidebar] = useState(false);
  return (
    // <FluentCard>
    //   {isAuthenticated ? <SignOutButton /> : <SignInButton />}
    //        <AuthenticatedTemplate>
    //        <ProfileContent />
    //        <RebelContent />
    //      </AuthenticatedTemplate>
    //      <UnauthenticatedTemplate>
    //        <p>You are not signed in! Please sign in.</p>
    //      </UnauthenticatedTemplate>
    //      <p>
    //      <small>NODE_ENV: {process.env.NODE_ENV}</small>
    //      </p>

    // </FluentCard>
    <Grommet theme={theme} full>
      <AppBar>
        <Heading level='3' margin='none'>Power UI</Heading>
        <Button
          icon={<Notification />}
          onClick={() => setShowSidebar(!showSidebar)}
        />
      </AppBar>
      <Box direction='row' flex overflow={{ horizontal: 'hidden' }}>
        <Box flex align='center' justify='center'>
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
        </Box>
        <Collapsible direction="horizontal" open={showSidebar}>
          <Box
            flex
            width='medium'
            background='light-2'
            elevation='small'
            align='center'
            justify='center'
          >
            sidebar
          </Box>
        </Collapsible>
      </Box>
    </Grommet>
  );
}

export default App;
