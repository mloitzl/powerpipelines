import React from "react";
import { useMsal } from "@azure/msal-react";

function handleLogout(instance) {
    instance.logoutPopup().catch(e => {
        console.error(e);
    });
}

export const SignOutButton = () => {
    const { instance } = useMsal();

    return (
        <button variant="secondary" className="ml-auto" onClick={() => handleLogout(instance)}>Sign out using Popup</button>
    );
}