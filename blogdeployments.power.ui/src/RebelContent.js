import React, { useState } from "react";
import { useMsal } from "@azure/msal-react";
import { powerApiTokenRequest } from "./authConfig";
import { RebelData } from "./RebelData";
import { getRebel } from "./graph";

import { provideFluentDesignSystem, fluentButton } from '@fluentui/web-components';
import { provideReactWrapper } from '@microsoft/fast-react-wrapper';

const { wrap } = provideReactWrapper(React, provideFluentDesignSystem());

export const FluentButton = wrap(fluentButton());


export function RebelContent() {
    const { instance, accounts } = useMsal();
    const [rebelData, setRebelData] = useState(null);

    function RequestRebelData(action) {

        const powerApiTokenRequest2 = {
            ...powerApiTokenRequest,
            account: accounts[0]
        };

        instance.acquireTokenSilent(powerApiTokenRequest2).then((response) => {
            getRebel(
                action,
                response.accessToken
            ).then(
                (response) => {
                    console.log(response);
                    setRebelData(response);
                }
            );
        }).catch((e) => {
            instance.acquireTokenPopup(powerApiTokenRequest2).then((response) => {
                getRebel(
                    action,
                    response.accessToken
                ).then(
                    (response) =>
                        setRebelData(response));
            });
        });
    }

    return (
        <>
            <div>
                { rebelData && <RebelData graphData={rebelData} /> }
                <FluentButton variant="secondary" onClick={() => RequestRebelData("On")}>Power On</FluentButton>
                <FluentButton variant="secondary" onClick={() => RequestRebelData("Off")}>Power Off</FluentButton>
            </div>
        </>
    );
};