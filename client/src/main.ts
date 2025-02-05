import {createApp} from 'vue'
import App from './App.vue'
import {createI18n} from 'vue-i18n'
import fr from './assets/translations/fr.json'
import en from './assets/translations/en.json'
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



import VueCookies from "vue-cookies"




let i18n = createI18n({	
	locale:"fr",
	messages:messages,
})

// default options config: { expires: '1d', path: '/', domain: '', secure: '', sameSite: 'Lax' , partitioned: false}

export const app = createApp(App)
app.use(VueCookies)
app.use(i18n)
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



//Weird workaround, make $t no longer show errors, though it isn't typesafe. 
declare module "@vue/runtime-core" {
	export interface ComponentCustomProperties {
		$t: (key: string, ...args: any[]) => string;
		ws: WebSocket
	}
}