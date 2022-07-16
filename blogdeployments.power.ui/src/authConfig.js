export const msalConfig = {
    auth: {
      clientId: "d997bd3e-f115-477b-b985-8aeca80e24e4",
      authority: "https://login.microsoftonline.com/common", // This is a URL (e.g. https://login.microsoftonline.com/{your tenant ID})
      // redirectUri: "https://powerpipelines.z6.web.core.windows.net",
      redirectUri: process.env.REACT_APP_REDIRECT,
    },
    cache: {
      cacheLocation: "sessionStorage", // This configures where your cache will be stored
      storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    }
  };
  
  // Add scopes here for ID token to be used at Microsoft identity platform endpoints.
  export const loginRequest = {
   scopes: ["User.Read"]
  };
  
export const powerApiTokenRequest = {
    scopes: ["api://com.loitzl.test/blogdeployments/Config.Read"]
}

  // Add the endpoints here for Microsoft Graph API services you'd like to use.
  export const graphConfig = {
      graphMeEndpoint: "https://graph.microsoft.com/v1.0/me"
  };