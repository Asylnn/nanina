<script lang="ts">
import { useRoute, RouterLink, RouterView } from 'vue-router'

import NNNHeader from './components/NNNHeader.vue'
import WaifuDisplay from './components/WaifuDisplay.vue'
import StatsBlock from './components/StatsBlock.vue'
import Homepage from './components/Homepage.vue'
import UserPage from './components/UserPage.vue'
import UserOptionPage from './components/UserOptionPage.vue'
import type WebSocketReponse from './classes/web_socket_response'
import {inject} from 'vue'
import type {VueCookies} from 'vue-cookies'

import {
	ArrayQueue,
	ConstantBackoff,
	Websocket,
	WebsocketBuilder,
	WebsocketEvent,
} from "websocket-ts"

import User from './classes/user'
import Waifu from './classes/waifu'
import Page from './classes/page'

export default {
	name: "La SDA de la mort qui tue",
	data() {
		return {
			page: Page.WaifuDisplay,
			waifu : new Waifu,
			logged : false,

			id: 1,
			objectType: "Waifu",
			xp: 0,
			lvl: 1,
			b_int: 69,
			b_luck: 17,
			b_exp: 52,
			stars: 4,
			modificators: [],
			equipedItems: [],
			action : {},
			owner : "Le seul l'unique",
			name: "Thighs-sama",
			diffLvlup: 10,
			o_exp: 2.1,
			o_int: 2.2,
			o_luck: 2.3,
			u_exp: 3.1,
			u_int: 3.2,
			rarity: 4,
			value: -1,
			isTradable: false,
			theme: "dark_theme"
		}
	},
	components:{
		NNNHeader,
		WaifuDisplay,
		StatsBlock,
    	Homepage,
		UserPage,
		UserOptionPage
	},
	methods : {
		updateTheme(theme : string) {
			this.theme = theme
		},
		updateLogged(logged : boolean) {
			this.logged = logged
		},
    	updatePage(page : Page) {
			console.log(page)
      		this.page = page
    	}
	},
  computed : {
    loadingPage() {
      this.page;
	  this.logged;
      switch (this.page) {
        case Page.Homepage :
          return 10
        case Page.Inventory :
			if (this.logged === true) return 20
			else return 50
        case Page.Disconnected :
          return 30
        case Page.NotFound :
          return 40
		case Page.WaifuDisplay :
			return 50
		case Page.YouSomehowEndedUpThere:
			return 60
		case Page.User:
			return 70
		case Page.UserOption:
			return 80
        default:
        	return 40
      }
    }
  },
  	created(){
		

	},
	mounted() {
		console.log("mounted app!")
		const $cookies = inject<VueCookies>('$cookies')!; 
		$cookies.config("5min")
		var has_session_id = $cookies.isKey("session_id")
		if (!has_session_id) {
			//@ts-ignore
			this.ws.send(`{"type":"get session id", "data":""}`)
		}
		else {
			
			console.log("Sent request for user using session Id, waiting for response...")
			//@ts-ignore
			this.ws.send(JSON.stringify({type:"request user with session id", data:$cookies.get("session_id")}))
		}
		const ListenForData = (i: Websocket, ev: MessageEvent) => {
			console.log(`received message: ${ev.data}`);
			var res : WebSocketReponse = JSON.parse(ev.data)
			if(res.type == "session_id"){
				$cookies.set("session_id", res.data)
			}
			else if (res.type == "user"){
				this.logged = true
			}

			//i.send(`${ev.data}`);
		};
		//@ts-ignore
		this.ws.addEventListener(WebsocketEvent.message, ListenForData);
		//@ts-ignore
		
		const url = new URL(window.location.href);
		const queryParams = new URLSearchParams(url.search);
		if(queryParams.has("code")){
			var code = queryParams.get("code") + ""
			//@ts-ignore
			this.ws.send(`{"type":"connect with discord", "data":"${queryParams.get("code")}"}`)
		}
		else {
		}
	},
}

</script>

<template>
	<div id="main" :class="[theme]">
		<NNNHeader :logged=logged @connect-change="updateLogged" @page-change="updatePage"></NNNHeader>
		<div v-if="loadingPage === 10">
		<Homepage image="src/assets/homepage.png"></Homepage>
		</div>
		<div v-else-if="loadingPage === 20">
			Inventory
			<!--<StatsBlock :objectType="objectType" :stars="stars" :rarity="rarity" :value="value" :owner="owner" :xp="xp" :lvl="lvl" :b_int="b_int" :b_luck="b_luck" :b_exp="b_exp" :o_int="o_int" :o_luck="o_luck"
			:o_exp="o_exp" :u_int="u_int" :u_exp="u_exp" :diffLvlup="diffLvlup"></StatsBlock>-->
		</div>
		<div v-else-if="loadingPage === 30">
			Déconnexion
		</div>
		<div v-else-if="loadingPage === 40">
			ERREUR 404 AHAHAHAHAH
		</div>
		<div v-else-if="loadingPage === 50">
			<WaifuDisplay :waifu="waifu"></WaifuDisplay>
		</div>
		<div v-else-if="loadingPage === 60">
			How tf did you even up here?
		</div>
		<div v-else-if="loadingPage === 70">
			<p>Hello User!</p>
			<UserPage></UserPage>
		</div>
		<div v-else-if="loadingPage === 80">
			<UserOptionPage :theme=theme @theme-change="updateTheme"></UserOptionPage>
		</div>
	</div>
</template>