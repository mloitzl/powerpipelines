import React from "react";
import { useMsal } from "@azure/msal-react";

function handleLogout(instance) {
    instance.logoutRedirect().catch(e => {
        console.error(e);
    });
}

export const SignOutButton = () => {
    const { instance } = useMsal();

    return (
        <button appearance="accent" onClick={() => handleLogout(instance)}>Sign out</button>
    );
}