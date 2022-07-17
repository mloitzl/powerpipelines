import React from "react";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "./authConfig";

import { provideFluentDesignSystem, fluentButton } from '@fluentui/web-components';
import { provideReactWrapper } from '@microsoft/fast-react-wrapper';

const { wrap } = provideReactWrapper(React, provideFluentDesignSystem());

export const FluentButton = wrap(fluentButton());


function handleLogin(instance) {
    instance.loginRedirect(loginRequest).catch(e => {
        console.error(e);
    });
}

export const SignInButton = () => {
    const { instance } = useMsal();

    return (
        <FluentButton appearance="accent" onClick={() => handleLogin(instance)}>Sign in</FluentButton>
    );
}