import React from "react";
import { useMsal } from "@azure/msal-react";
import { provideFluentDesignSystem, fluentButton } from '@fluentui/web-components';
import { provideReactWrapper } from '@microsoft/fast-react-wrapper';

const { wrap } = provideReactWrapper(React, provideFluentDesignSystem());

export const FluentButton = wrap(fluentButton());

function handleLogout(instance) {
    instance.logoutRedirect().catch(e => {
        console.error(e);
    });
}

export const SignOutButton = () => {
    const { instance } = useMsal();

    return (
        <FluentButton appearance="accent" onClick={() => handleLogout(instance)}>Sign out</FluentButton>
    );
}