import {createApp} from 'vue'
import App from './App.vue'
import {createI18n} from 'vue-i18n'
import fr from './assets/translations/fr.json'
import en from './assets/translations/en.json'
import config from '../../config.json'

//import router from './router'
import {
	ArrayQueue,
	ConstantBackoff,
	WebsocketBuilder,
	WebsocketEvent,
} from "websocket-ts"

let messages = {
	"fr":fr,
	"en":en,
} 

declare module "@vue/runtime-core" {
	export interface ComponentCustomProperties {
		$t: (key: string, ...args: any[]) => string;
		ws: WebSocket
		$i18n : any
		SendToServer : (type : string, data: string, userId: string | null) => void;
	}
}

import VueCookies from "vue-cookies"




let i18n = createI18n({	
	locale:"fr",
	messages:messages,
})

// default options config: { expires: '1d', path: '/', domain: '', secure: '', sameSite: 'Lax' , partitioned: false}

export const app = createApp(App)
app.use(VueCookies)
app.use(i18n)

let url = (config.dev ? config.ws_dev_server_url : config.ws_prod_server_url) 
let port = (config.dev ? config.ws_port : config.prod_ws_port) 
url += ":" + port + "/ws"
console.log("listening on url: " + url)
app.config.globalProperties.ws = new WebsocketBuilder(url)
    .withBuffer(new ArrayQueue())           // buffer messages when disconnected
    .withBackoff(new ConstantBackoff(1000)) // retry every 1s
    .build();


const ws = app.config.globalProperties.ws
	// Add event listeners
	ws.addEventListener(WebsocketEvent.open, () => console.log("ws opened!"));
	ws.addEventListener(WebsocketEvent.close, () => console.log("ws closed!"));

app.config.globalProperties.SendToServer = (type : string, data: string, userId: string) => {
	ws.send(JSON.stringify({
		type: type,
		data: data,
		userId : userId,
		sessionId : app.config.globalProperties.$cookies.get("session_id")
	}))
}

//app.use(router)
app.mount('#app')





// Function to output & echo received messages






//Weird workaround, make $t no longer show errors, though it isn't typesafe. 


