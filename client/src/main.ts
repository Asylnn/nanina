import { createApp } from 'vue'
import App from './App.vue'
//import router from './router'
import User from './classes/user'
const app = createApp(App)

//app.use(router)

app.mount('#app')

import {
ArrayQueue,
ConstantBackoff,
Websocket,
WebsocketBuilder,
WebsocketEvent,
} from "websocket-ts";

let user : User = new User("darkmode", "1234", "a username")

// Initialize WebSocket with buffering and 1s reconnection delay
const ws = new WebsocketBuilder("ws://localhost:4889")
    .withBuffer(new ArrayQueue())           // buffer messages when disconnected
    .withBackoff(new ConstantBackoff(1000)) // retry every 1s
    .build();

// Function to output & echo received messages
const echoOnMessage = (i: Websocket, ev: MessageEvent) => {
    console.log(`received message: ${ev.data}`);
    var obj : User = JSON.parse(ev.data)
    console.log(obj)
    //i.send(`${ev.data}`);
};

// Add event listeners
ws.addEventListener(WebsocketEvent.open, () => console.log("ws opened!"));
ws.addEventListener(WebsocketEvent.close, () => console.log("ws closed!"));
ws.addEventListener(WebsocketEvent.message, echoOnMessage);
ws.send(`{"type":"request user", "data":"727"}`)