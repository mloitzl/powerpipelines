import logo from './logo.svg';
import './App.css';
import { useIsAuthenticated, AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react"
import { SignInButton } from "./SignInButton"
import { SignOutButton } from "./SignOutButton"
import { ProfileContent } from "./ProfileContent"

function App() {
  const isAuthenticated = useIsAuthenticated();
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        {isAuthenticated ? <SignOutButton /> : <SignInButton />}
        <AuthenticatedTemplate>
          <ProfileContent />
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
          <p>You are not signed in! Please sign in.</p>
        </UnauthenticatedTemplate>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
