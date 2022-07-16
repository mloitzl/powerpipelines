import React from "react";

/**
 * Renders information about the user obtained from Microsoft Graph
 */
export const RebelData = (props) => {
    return (
        <div>
            <div id="profile-div">
                <p><strong>Action: </strong> {props.graphData.action}</p>
                <p><strong>Result </strong> {props.graphData.result}</p>
            </div>
        </div>
    );
};