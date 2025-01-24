import { createApp } from 'vue'
import App from './App.vue'
//import router from './router'
import {
	ArrayQueue,
	ConstantBackoff,
	WebsocketBuilder,
	WebsocketEvent,
} from "websocket-ts"

import VueCookies from "vue-cookies"

// default options config: { expires: '1d', path: '/', domain: '', secure: '', sameSite: 'Lax' , partitioned: false}

export const app = createApp(App)
app.use(VueCookies)

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



