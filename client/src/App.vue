<script lang="ts">
import { useRoute, RouterLink, RouterView } from 'vue-router'

import NNNHeader from './components/NNNHeader.vue'
import WaifuDisplay from './components/WaifuDisplay.vue'
import StatsBlock from './components/StatsBlock.vue'
import Homepage from './components/Homepage.vue'
import UserPage from './components/UserPage.vue'
import UserOptionPage from './components/UserOptionPage.vue'
import AddMap from './components/AddMap.vue'
import ClaimAndFightPage from './components/ClaimAndFightPage.vue'
import WaifuManagerPage from './components/WaifuManagerPage.vue'
import NotificationMenu from './components/NotificationMenu.vue'
import Notification from './classes/notif'
import PullPage from './components/PullPage.vue'
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
import NotificationSeverity from './classes/notification_severity'

export default {
	name: "La SDA de la mort qui tue",
	data() {
		return {
			page: Page.WaifuDisplay,
			logged : false,
			user : new User({}),
			fighting : false,
			link : "",
			xp : 0,
			notifs : Array(),
			dev : true, //Is this dev or prod? IMPORTANT!!
			all_waifus : [],
			pulled_waifus : [],
			banners : [],
		}
	},
	components:{
		NNNHeader,
		WaifuDisplay,
		StatsBlock,
    	Homepage,
		UserPage,
		UserOptionPage,
		AddMap,
		ClaimAndFightPage,
		NotificationMenu,
		WaifuManagerPage,
		PullPage,
	},
	
	methods : {
		updateTheme(theme : string) {
			this.user.theme = theme
			if (this.logged) {
				//@ts-ignore
				this.ws.send(JSON.stringify({type:"update theme", data:theme, id: this.user.Id}))

				
			}
		},
    	updatePage(page : Page) {
			console.log(page)
      		this.page = page
    	},
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
			case Page.ClaimAndFightPage:
				return 100
			case Page.AddMap :
				if (this.user.admin == true)	return 90
				else 					return 50
			case Page.WaifuManagerPage :
				if (this.user.admin == true && this.dev == true)	return 110
				else 					return 50
			case Page.PullPage:
				return 120
			default:
				return 40
		}
    }
  },
	mounted() {
		console.log("mounted app!")
		const $cookies = inject<VueCookies>('$cookies')!; 
		$cookies.config("30m") //ATENTION !!!!!
		var has_session_id = $cookies.isKey("session_id")
		if (!has_session_id) {
			//@ts-ignore
			this.ws.send(`{"type":"get session id", "data":""}`)
		}
		else {
			
			console.log("Sent request for user using session Id, waiting for response...")
			//@ts-ignore
			this.ws.send(JSON.stringify({type:"request user with session id", data:$cookies.get("session_id"), id:""}))
		}
		const ListenForData = (i: Websocket, ev: MessageEvent) => {
			console.log(`received message: ${ev.data}`);
			var res : WebSocketReponse = JSON.parse(ev.data)
			switch (res.type) {
				case "session_id" :
					$cookies.set("session_id", res.data)
					break
				case "user" :
					this.logged = true
					console.log(new User(JSON.parse(res.data)))
					this.user = new User(JSON.parse(res.data))
					if(this.user.admin){
						//@ts-ignore
						this.ws.send(JSON.stringify({type:"request waifu db", data:"", id:this.user.Id}))
					}
					break
				case "map link" :
					this.fighting = true 
					this.link = res.data
					this.notifs.push(new Notification("Fight", "You started with the following beatmap! " + this.link, NotificationSeverity.Notification))
					break
				case "fighting results" :
					this.fighting = false 
					this.xp = res.data
					break
				case "waifu db" :
					this.all_waifus = JSON.parse(res.data)
					break
				case "get banners":
					this.banners = JSON.parse(res.data)
					break
				case "notification":
					let notification = Object.assign(new Notification("","",0), JSON.parse(res.data))
					this.notifs.push(notification)
					break
				case "pull results":
					console.log(res.data)
					this.pulled_waifus = JSON.parse(res.data)
					break
				
			} 
			//i.send(`${ev.data}`);
		};
		//@ts-ignore
		this.ws.addEventListener(WebsocketEvent.message, ListenForData);
		 
		const url = new URL(window.location.href);
		const queryParams = new URLSearchParams(url.search);
		if(queryParams.has("code")){
			var code = queryParams.get("code") + ""
			//@ts-ignore
			this.ws.send(JSON.stringify({type:"connect with discord", data:queryParams.get("code"), id:""}))
		}
		else {
		}
	},
}

</script>

<template>
	<div id="main" :class="[user.theme]">
		<NNNHeader :dev=dev :logged=logged :admin=user.admin @page-change="updatePage"></NNNHeader>
		<div v-if="loadingPage === 10">
		<Homepage image="src/assets/homepage.png"></Homepage>
		</div>
		<div v-else-if="loadingPage === 20">
			Inventory
			<!--<StatsBlock :objectType="objectType" :stars="stars" :rarity="rarity" :value="value" :owner="owner" :xp="xp" :lvl="lvl" :b_int="b_int" :b_luck="b_luck" :b_exp="b_exp" :o_int="o_int" :o_luck="o_luck"
			:o_exp="o_exp" :u_int="u_int" :u_exp="u_exp" :diffLvlup="diffLvlup"></StatsBlock>-->
		</div>
		<div v-else-if="loadingPage === 40">
			ERREUR 404 AHAHAHAHAH
		</div>
		<div v-else-if="loadingPage === 50">
			<WaifuDisplay :waifu="user.waifus[0]"></WaifuDisplay>
		</div>
		<div v-else-if="loadingPage === 60">
			How tf did you even up here?
		</div>
		<div v-else-if="loadingPage === 70">
			<p>Hello User!</p>
			<UserPage></UserPage>
		</div>
		<div v-else-if="loadingPage === 80">
			<UserOptionPage :id=user.Id :theme=user.theme @theme-change="updateTheme"></UserOptionPage>
		</div>
		<div v-else-if="loadingPage === 90">
			<AddMap :id="user.Id"></AddMap>
		</div>
		<div v-else-if="loadingPage === 100">
			<ClaimAndFightPage :xp="xp" :fighting="fighting" :id="user.Id" :link="link"></ClaimAndFightPage>
		</div>
		<div v-else-if="loadingPage === 110">
			<WaifuManagerPage :all_waifus="all_waifus" :id="user.Id"></WaifuManagerPage>
		</div>
		<div v-else-if="loadingPage === 120">
			<PullPage :banners="banners" :pulled_waifus="pulled_waifus" :gacha_currency="user.gacha_currency" :id="user.Id"></PullPage>
		</div>
		<NotificationMenu :notifs=notifs></NotificationMenu>
	</div>
</template>

<style lang="css" scoped>
#main {
	min-height: 100vh;
}
</style>