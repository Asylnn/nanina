<script lang="ts">
import { useRoute, RouterLink, RouterView } from 'vue-router'

import NNNHeader from './components/NNNHeader.vue'
import Homepage from './components/Homepage.vue'
import UserPage from './components/UserPage.vue'
import UserOptionPage from './components/UserOptionPage.vue'
import AddMap from './components/AddMap.vue'
import ClaimAndFightPage from './components/ClaimAndFightPage.vue'
import WaifuManagerPage from './components/WaifuManagerPage.vue'
import NotificationMenu from './components/NotificationMenu.vue'
import Notification from './classes/notif'
import PullPage from './components/PullPage.vue'
import WaifuListPage from './components/WaifuListPage.vue'
import LootDisplayComponent from './loot/LootDisplayComponent.vue'
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

import User from './classes/user/user'
import Waifu from './classes/waifu/waifu'
import Page from './classes/page'
import NotificationSeverity from './classes/notification_severity'
import OsuBeatmap from './classes/beatmap'
import ItemManagerPage from './components/ItemManagerPage.vue'
import WaifuDisplayComponent from './components/WaifuDisplayComponent.vue'
import InventoryManagerPage from './components/InventoryManagerPage.vue'
import InventoryPage from './components/InventoryPage.vue'
import DungeonPage from './components/DungeonPage.vue'
import type DungeonTemplate from './classes/dungeons/template_dungeons'
import type Banner from './classes/banner'
import type Item from './classes/item/item'
import type Set from './classes/item/set'
import ActiveDungeon from './classes/dungeons/active_dungeon'
import Chart from './classes/maimai/chart'
import StatsPage from './components/StatsPage.vue'
import type Loot from './classes/loot/loot'
import type Equipment from './classes/item/equipment'

export default {
	name: "La SDA de la mort qui tue",
	data() {
		return {
			page: Page.Homepage,
			logged : false,
			user : new User({}),
			fighting : false,
			link : "",
			beatmap : new OsuBeatmap(),
			xp : 0,
			notifs : Array(),
			dev : true, //Is this dev or prod? IMPORTANT!!
			all_waifus : [] as Waifu[],
			item_db : [] as Item[],
			equipment_db : [] as Equipment[],
			set_db : [] as Set[],
			pulled_waifus : [] as Waifu[],
			banners : [] as Banner[],
			dungeons : [] as DungeonTemplate[],
			active_dungeon : new ActiveDungeon,
			localeSetByUser : false,
			maimai_chart : null as Chart | null,
			loots : [] as Array<Loot[]>,
		}
	},
	components: {
		NNNHeader,
		Homepage,
		UserPage,
		UserOptionPage,
		AddMap,
		ClaimAndFightPage,
		NotificationMenu,
		WaifuManagerPage,
		PullPage,
		WaifuListPage,
		InventoryPage,
		ItemManagerPage,
		WaifuDisplayComponent,
		InventoryManagerPage,
		DungeonPage,
		StatsPage,
		LootDisplayComponent,
	},	
	methods : {
		updateTheme(theme : string) {
			this.user.theme = theme
			if (this.logged) {
				this.SendToServer("update theme", "", this.user.Id)

				
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
			case Page.InventoryPage :
				if (this.logged === true) return 20
				else return 40
			case Page.NotFound :
				return 40
			case Page.WaifuListPage :
				return 50
			case Page.YouSomehowEndedUpThere:
				return 60
			case Page.User:
				return 70
			case Page.UserOption:
				return 80
			case Page.ClaimAndFightPage:
				return 100
			case Page.StatPage:
				return 160
			case Page.DungeonPage:
				return 150
			case Page.AddMap :
				if (this.user.admin == true)	return 90
				else 					return 50
			case Page.WaifuManagerPage :
				if (this.user.admin == true && this.dev == true)	return 110
				else 					return 50
			case Page.PullPage:
				return 120
			case Page.ItemManagerPage:
				return 130
			case Page.InventoryManagerPage:
				return 140
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
		
		const url = new URL(window.location.href);
		const queryParams = new URLSearchParams(url.search);
		if(queryParams.has("code")){
			var code = queryParams.get("code") + ""
			this.SendToServer("connect with discord", queryParams.get("code")!, this.user.Id)
		}
		else 
			this.SendToServer("get session id", "", null)
		
		
		
		const ListenForData = (i: Websocket, ev: MessageEvent) => {
			//console.log(`received message: ${ev.data}`);
			var res : WebSocketReponse = JSON.parse(ev.data)
			switch (res.type) {
				case "session" :
					let session = JSON.parse(res.data)
					$cookies.set("session_id", session.id)
					console.log("session : ")
					console.log(session)
					if(!this.localeSetByUser)
						this.$i18n.locale = session.locale
					break
				case "user" :
					this.logged = true
					console.log("user : ")
					console.log(new User(JSON.parse(res.data)))
					let localFightTimestamp = this.user.localFightTimestamp
					this.user = new User(JSON.parse(res.data))
					this.user.waifus = this.user.waifus.map(waifu => new Waifu(waifu))
					this.user.localFightTimestamp = localFightTimestamp

					this.$i18n.locale = this.user.locale
					this.localeSetByUser = true
					/*if(this.user.admin){
						this.SendToServer("request set db", "",this.user.Id)
						this.SendToServer("request item db", "",this.user.Id)
						this.SendToServer("request waifu db", "",this.user.Id)
					}*/
					break
				case "map link" :
					this.fighting = true
					this.beatmap = JSON.parse(res.data)
					this.link = 'https://osu.ppy.sh/beatmapsets/'+this.beatmap.beatmapset_id+'#'+this.beatmap.mode+'/'+this.beatmap.id
					//this.notifs.push(new Notification("Fight", "You started a fight with the following beatmap! " + this.link, NotificationSeverity.Notification))
					break
				case "maimai link" :
					this.fighting = true
					this.maimai_chart = JSON.parse(res.data)
					break
				case "fighting results" :
					this.fighting = false 
					this.xp = res.data
					break
				case "waifu db" :
					this.all_waifus = JSON.parse(res.data)
					break
				case "item db" :
					this.item_db = JSON.parse(res.data)
					console.log("ITEM DATABASE")
					console.log(this.item_db)
					break
				case "equipment db" :
					this.equipment_db = JSON.parse(res.data)
					console.log("EQUIPMENT DATABASE")
					console.log(this.equipment_db)
					break
				case "set db" :
					this.set_db = JSON.parse(res.data)
					console.log("SET DATABASE")
					console.log(this.item_db)
					break
				case "get banners":
					this.banners = JSON.parse(res.data)
					console.log("Banners data : ")
					console.log(this.banners)
					break
				case "notification":
					let notification = Object.assign(new Notification("","",0), JSON.parse(res.data))
					this.notifs.push(notification)
					break
				case "pull results":
					this.pulled_waifus = JSON.parse(res.data)
					console.log("Pulled Waifus data : ")
					console.log(this.pulled_waifus)
					break
				case "get dungeons":
					this.dungeons = JSON.parse(res.data)
					console.log("Dungeon data : ")
					console.log(this.dungeons)
					
					break
				case "get active dungeon":
					this.active_dungeon = JSON.parse(res.data)
					break
				case "loot":
					this.loots.push(JSON.parse(res.data))
					console.log("Got loot : ")
					console.log(this.loots)
					break
				
			} 
			//i.send(`${ev.data}`);
		};
		//@ts-ignore
		this.ws.addEventListener(WebsocketEvent.message, ListenForData);
		 
	},
}

</script>

<template>
	<div id="main" :class="[user.theme]">
	
		<NNNHeader :dev=dev :logged=logged :user="user" @page-change="updatePage"></NNNHeader>
		<div v-if="loadingPage === 10">
			<Homepage></Homepage>
		</div>
		<div v-else-if="loadingPage === 20">
			<InventoryPage :user="user"></InventoryPage>
			<!--<StatsBlock :objectType="objectType" :stars="stars" :rarity="rarity" :value="value" :owner="owner" :xp="xp" :lvl="lvl" :b_int="b_int" :b_luck="b_luck" :b_exp="b_exp" :o_int="o_int" :o_luck="o_luck"
			:o_exp="o_exp" :u_int="u_int" :u_exp="u_exp" :diffLvlup="diffLvlup"></StatsBlock>-->
		</div>
		<div v-else-if="loadingPage === 40">
			ERREUR 404 AHAHAHAHAH
		</div>
		<div v-else-if="loadingPage === 50">
			<WaifuListPage :user="user"></WaifuListPage>
		</div>
		<div v-else-if="loadingPage === 60">
			How tf did you even up here?
		</div>
		<div v-else-if="loadingPage === 70">
			<UserPage :user="user"></UserPage>
		</div>
		<div v-else-if="loadingPage === 80">
			<UserOptionPage :user=user :theme=user.theme @theme-change="updateTheme"></UserOptionPage>
		</div>
		<div v-else-if="loadingPage === 90">
			<AddMap :id="user.Id"></AddMap>
		</div>
		<div v-else-if="loadingPage === 100">
			<ClaimAndFightPage :maimai_chart="maimai_chart" :xp="xp" :fighting="fighting" :user="user" :beatmap="beatmap"></ClaimAndFightPage>
		</div>
		<div v-else-if="loadingPage === 110">
			<WaifuManagerPage :all_waifus="all_waifus" :id="user.Id"></WaifuManagerPage>
		</div>
		<div v-else-if="loadingPage === 120">
			<PullPage :banners="banners" :pulled_waifus="pulled_waifus" :gacha_currency="user.gacha_currency" :user="user"></PullPage>
		</div>
		<div v-else-if="loadingPage === 130">
			<ItemManagerPage :id="user.Id" :item_db="item_db" :equipment_db="equipment_db" :set_db="set_db"></ItemManagerPage>
		</div>
		<div v-else-if="loadingPage === 140">
			<InventoryManagerPage :user="user" :items="item_db"></InventoryManagerPage>
		</div>
		<div v-else-if="loadingPage === 150">
			<DungeonPage :dungeons="dungeons" :user="user" :active_dungeon="active_dungeon"></DungeonPage>
		</div>
		<div v-else-if="loadingPage === 160">
			<StatsPage :user="user"></StatsPage>
		</div>
		<NotificationMenu :notifs=notifs></NotificationMenu>
		<LootDisplayComponent  :is-new-loot="true"  :loots=loots></LootDisplayComponent>
	</div>
</template>

<style lang="css" scoped>
#main {
	min-height: 100vh;
	position: relative;
}
</style>