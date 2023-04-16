import React, {
    // useState, useEffect,
    Component} from 'react';
import './FetchData.css';

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);

        this.powerOn = this.powerOn.bind(this);
        this.shutDown = this.shutDown.bind(this);
        
        this.state = {
            clusterId: "",
            clusterPowerStatus: 1,
            powerRequestId: "",
            hostsPower: [],
            loading: true,
            powerUpDisabled: false,
            shutDownDisabled: true
        };
    }

    async shutDown(){
        console.log("Shutting down...");

        try {
            const response = await fetch("powerui/shutdown", {
                method: "POST", // or 'PUT'
                headers: {
                    "Content-Type": "application/json",
                },
                body: "",
            });

            const result = await response.json();

            if(result) {
                const oldState = this.state;

                oldState.powerUpDisabled = false;
                oldState.shutDownDisabled = true;

                this.setState(oldState);
            }

            console.log("Success:", result);
        } catch (error) {
            console.error("Error:", error);
        }
        
    }

    async powerOn(){
        console.log("Power up...");

        try {
            const response = await fetch("powerui/poweron", {
                method: "POST", // or 'PUT'
                headers: {
                    "Content-Type": "application/json",
                },
                body: "",
            });

            const result = await response.json();
            
            if(result) {
                const oldState = this.state;

                oldState.powerUpDisabled = true;
                oldState.shutDownDisabled = false;

                this.setState(oldState);
            }
            
            console.log("Success:", result);
        } catch (error) {
            console.error("Error:", error);
        }
    }

    componentDidMount() {
        this.loadPowerStatus();
        this.startSSE();
    }

    static getTrafficLight(state) {
        var color = 'red';
        switch (state) {
            case 2:
                color = "green";
                break;
            case 1:
                color = "grey";
                break;
            case 0:
            default:
                color = "red";
                break;
        }
        return (
            <div id="traffic-signal">
                <div id={color}></div>
            </div>
        );
    }

    static renderForecastsTable(state) {
        return (
            <>
                {/*<pre>{JSON.stringify(state, null, 2)}</pre>*/}
                {/*<span>Cluster: {state.id}</span>|<span>Request: {state.powerRequestId}</span>*/}

                <table className="table table-striped" aria-labelledby="tableLabel">
                    <thead>
                    <tr>
                        <th>device</th>
                        <th>Status</th>
                        <th></th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr key="Cluster">
                        <td>
                            Cluster
                        </td>
                        <td>
                            {state.clusterPowerStatus}
                        </td>
                        <td>
                            {FetchData.getTrafficLight(state.clusterPowerStatus)}
                        </td>
                    </tr>
                    <tr>
                        <td colSpan={3}></td>
                    </tr>
                    {Object.entries(state.hostsPower).map(([key, value]) =>
                        <tr key={key}>
                            <td>
                                {value["hostname"]}
                            </td>
                            <td>
                                {value["status"]}
                            </td>
                            <td>
                                {FetchData.getTrafficLight(value["status"])}
                            </td>
                        </tr>
                    )}
                    </tbody>
                </table>
            </>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : FetchData.renderForecastsTable(this.state);

        return (
            <div>
                <h1 id="tableLabel">Cluster Power Status</h1>
                <div className="btn-group" role="group" aria-label="Basic example">
                    <button type="button" className="btn btn-primary" onClick={async () =>{ await this.powerOn();} }  disabled={this.state.powerUpDisabled}>Power On</button>
                    <button type="button" className="btn btn-primary" onClick={this.shutDown} disabled={this.state.shutDownDisabled}>Shut Down</button>
                </div>
                {contents}
            </div>
        );
    }

    updateClusterStatus = (data) => {
        const payload = JSON.parse(data);

        console.log(payload);
        
        const oldState = this.state;
        oldState["clusterPowerStatus"] = payload["PowerStatus"];
        this.setState(oldState);
    }
    
    updateHostStatus = (data) => {
        const payload = JSON.parse(data);

        console.log(payload);

        const oldState = this.state;
        oldState["hostsPower"][payload["Hostname"]]["status"] = payload["PowerStatus"];
        this.setState(oldState);
    }

    async startSSE() {
        const eventSource = new EventSource("sse");

        eventSource.onmessage = (e) => {
            if (e.lastEventId === "blogdeployments.ui.Handler.SendHostNotification") {
                this.updateHostStatus(e.data);
            }
            if (e.lastEventId === "blogdeployments.ui.Handler.SendClusterNotification") {
                this.updateClusterStatus(e.data);
            }
        };
        eventSource.onopen = () => console.info("opened");
        eventSource.onerror = (ev) => {
            console.error(ev);
        };
    }

    async loadPowerStatus() {
        const response = await fetch('powerui');
        const data = await response.json();
        this.setState({
            clusterId: data.id,
            clusterPowerStatus: 0,
            powerRequestId: data.powerRequestId,
            hostsPower: data.hostsPower,
            loading: false
        });
    }
}
