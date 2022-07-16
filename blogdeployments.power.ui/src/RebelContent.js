import React, { useState } from "react";
import { useMsal } from "@azure/msal-react";
import { powerApiTokenRequest } from "./authConfig";
import { RebelData } from "./RebelData";
import { getRebel } from "./graph";

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
                <button variant="secondary" onClick={() => RequestRebelData("On")}>Power On</button>
                <button variant="secondary" onClick={() => RequestRebelData("Off")}>Power Off</button>
            </div>
        </>
    );
};