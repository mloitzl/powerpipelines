import React, {useState, useEffect} from "react";

function ServerSentEvents() {

    const [events, setEvents] = useState([]);
    
    const updateEvents = (data) =>{
        console.log(data);
        setEvents( (events) =>{
            return [data];
        } );
    }
    
     useEffect(
         () => {
             const eventSource = new EventSource("sse");
             eventSource.onmessage = (e) => {
                 updateEvents(
                     { 
                         data: e.data,
                         time: new Date(e.timeStamp)
                     }
                 );
             };
             eventSource.onopen =  () => console.info("opened");
             eventSource.onerror =  (ev) => {
                 console.error(ev);
             };
             
             return () => {
                 eventSource.close();
             };
         }, []
     );
    
    return (
        <pre>{JSON.stringify(events, null, 2)}</pre>
    );
}

export default ServerSentEvents