<script lang="ts">
import { useRoute, RouterLink, RouterView } from 'vue-router'

import NNNHeader from './components/NNNHeader.vue'
import ImgBlock from './components/ImgBlock.vue'
import StatsBlock from './components/StatsBlock.vue'
import Theme from './components/Theme.vue'
import Homepage from './components/Homepage.vue'
import axios from 'axios'

import {
	ArrayQueue,
	ConstantBackoff,
	Websocket,
	WebsocketBuilder,
	WebsocketEvent,
} from "websocket-ts"

import User from './classes/user'

import Page from './classes/page';

export default {
	name: "La SDA de la mort qui tue",
	data() {
		return {
			page: Page.Inventory,
			logged : false,
			Page,

			id: 1,
			imgURL: "src/assets/waifu-image/GYrXGACboAACxp7.jpg", 
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
		ImgBlock,
		StatsBlock,
    Theme,
    Homepage
	},
	methods : {
		updateTheme(theme : string) {
			this.theme = theme
		},
		updateLogged(logged : boolean) {
			console.log(logged)
			this.logged = logged
		},
    	updatePage(page : Page) {
      		this.page = page
    	}
	},
  computed : {
    loadingPage() {
      this.page;
	  this.logged
      switch (this.page) {
        case Page.Homepage :
          return 10
          break;
        case Page.Inventory :
			if (this.logged === true) {
				return 20;
			}
			else return 50;
		break;
        case Page.Disconnected :
          return 30
          break;
        case Page.NotFound :
          return 40
          break;
		case Page.YouSomehowEndedUpThere:
        default:
          return 50
          break;
      }
    }
  },
	mounted() {
		let user : User = new User("darkmode", "1234", "a username");

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
		
		const url = new URL(window.location.href);
		const queryParams = new URLSearchParams(url.search);
		

		if(queryParams.has("code")){
			var code = queryParams.get("code") + ""
			console.log(code)
			var formData = new URLSearchParams({
				grant_type: 'authorization_code',
				code: code,
				redirect_uri: 'http://localhost:5173',
			})
			axios.post("https://discord.com/api/oauth2/token", formData.toString(),
			{
				headers: {
					"Authorization": `Basic ${btoa(`${"1292571843848568932"}:${"rhZnqlH817wvJ9V13uRNhFQS8h9VLRov"}`)}`,
					'Content-Type': 'application/x-www-form-urlencoded'
				}
			}).then((data) =>  {
				console.log(data)
			}).catch((w) =>  {
				console.log("catched!!!")
				console.log(w)
			});
			
			//ws.send(`{"type":"send code", data =${queryParams.get("code")}}`)*/
		}
		else {
		}
	},
}

</script>

<template>
		<div id="main" :class="[theme]">
    <NNNHeader :logged=logged @connect-change="updateLogged" :theme="theme" @real-theme-change="updateTheme" :page="page" @page-change="updatePage"></NNNHeader>
    <div v-if="loadingPage === 10">
    <Homepage image="src/assets/homepage.png"></Homepage>
    </div>
    <div v-else-if="loadingPage === 20">
		<ImgBlock :imgURL="imgURL" :name="name"></ImgBlock>
		<StatsBlock :objectType="objectType" :stars="stars" :rarity="rarity" :value="value" :owner="owner" :xp="xp" :lvl="lvl" :b_int="b_int" :b_luck="b_luck" :b_exp="b_exp" :o_int="o_int" :o_luck="o_luck"
		:o_exp="o_exp" :u_int="u_int" :u_exp="u_exp" :diffLvlup="diffLvlup"></StatsBlock>
    </div>
    <div v-else-if="loadingPage === 30">
      Déconnexion
    </div>
    <div v-else-if="loadingPage === 40">
      ERREUR 404 AHAHAHAHAH
    </div>
    <div v-else-if="loadingPage === 50">
    How tf did you even up here?
    </div>
	</div>
</template>