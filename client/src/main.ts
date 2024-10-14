import { createApp } from 'vue'
import App from './App.vue'
import User from './classes/user'
//import router from './router'
import {
	ArrayQueue,
	ConstantBackoff,
	Websocket,
	WebsocketBuilder,
	WebsocketEvent,
} from "websocket-ts"

const app = createApp(App)

app.config.globalProperties.ws = new WebsocketBuilder("ws://localhost:4889")
    .withBuffer(new ArrayQueue())           // buffer messages when disconnected
    .withBackoff(new ConstantBackoff(1000)) // retry every 1s
    .build();
//app.use(router)

app.mount('#app')



// Function to output & echo received messages

const ws = app.config.globalProperties.ws
// Add event listeners
ws.addEventListener(WebsocketEvent.open, () => console.log("ws opened!"));
ws.addEventListener(WebsocketEvent.close, () => console.log("ws closed!"));



