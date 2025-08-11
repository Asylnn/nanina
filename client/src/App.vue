<script lang="ts">
import { useRoute, RouterLink, RouterView } from 'vue-router'

import Header from './components/Component/Header.vue'
import Homepage from './components/Page/Homepage.vue'
import UserPage from './components/Page/UserPage.vue'
import UserOptionPage from './components/Page/UserOptionPage.vue'
import AddMap from './components/Admin/AddMap.vue'
import ClaimAndFightPage from './components/Page/FightPage.vue'
import WaifuManagerPage from './components/Admin/Manager/WaifuManagerPage.vue'
import NotificationMenu from './components/Component/NotificationMenu.vue'
import Notification from './classes/notif'
import PullPage from './components/Page/PullPage.vue'
import WaifuListPage from './components/Page/WaifuListPage.vue'
import LootDisplayComponent from './components/Component/LootDisplayComponent.vue'
import FooterComponent from './components/Component/FooterComponent.vue'
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
import ItemManagerPage from './components/Admin/Manager/ItemManagerPage.vue'
import WaifuDisplayComponent from './components/Component/WaifuDisplayComponent.vue'
import InventoryManagerPage from './components/Admin/Manager/InventoryManagerPage.vue'
import InventoryPage from './components/Page/InventoryPage.vue'
import DungeonSelectionPage from './components/Page/DungeonSelectionPage.vue'
import ActiveDungeonPage from './components/Page/ActiveDungeonPage.vue'
import type DungeonTemplate from './classes/dungeons/template_dungeons'
import type Banner from './classes/banner/banner'
import type Item from './classes/item/item'
import type Set from './classes/item/set'
import ActiveDungeon from './classes/dungeons/active_dungeon'
import Chart from './classes/maimai/chart'
import type Loot from './classes/loot/loot'
import Equipment from './classes/item/equipment'
import UserWaifuManagerPage from './components/Admin/Manager/UserWaifuManagerPage.vue'
import PrivacyPage from './components/Page/PrivacyPage.vue'
import ActivitiesPage from './components/Page/ActivitiesPage.vue'
import LootType from './classes/loot/loot_type'
import ResearchNode from './classes/research/research_nodes'
import Craft from './classes/crafting/craft'
import type Dictionary from './classes/dictionary'
import ClientResponseType from './classes/client_response_type'
import ServerResponseType from './classes/server_response_type'

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
			banners : {} as Dictionary<Banner> ,
			dungeons : {} as Dictionary<DungeonTemplate>,
			active_dungeon : new ActiveDungeon,
			localeSetByUser : false,
			maimai_chart : null as Chart | null,
			loots : [] as Array<Loot[]>,
			inside_dungeon : false,
			Page : Page,
			researchNodes : [] as ResearchNode[],
			craftingRecipes : [] as Craft[],
		}
	},
	components: {
		Header,
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
		DungeonSelectionPage,
		ActiveDungeonPage,
		LootDisplayComponent,
		UserWaifuManagerPage,
		FooterComponent,
		PrivacyPage,
		ActivitiesPage,
	},	
	methods : {
		updateTheme(theme : string) {
			this.user.theme = theme
			if (this.logged) {
				this.SendToServer(ClientResponseType.UpdateTheme, theme, this.user.Id)

				
			}
		},
    	updatePage(page : Page) {
      		this.page = page
    	},
	},
	mounted() {
		console.log("mounted app!")
		const $cookies = inject<VueCookies>('$cookies')!; 
		$cookies.config("30d")
		/*set expire times , input string type
			Unit 	full name
			y 	year
			m 	month
			d 	day
			h 	hour
			min 	minute
			s 	second
		*/
		var has_session_id = $cookies.isKey("session_id")
		
		const url = new URL(window.location.href);
		const queryParams = new URLSearchParams(url.search);
		if(queryParams.has("code")){
			var code = queryParams.get("code") + ""
			this.SendToServer(ClientResponseType.ConnectWithDiscord, queryParams.get("code")!, null)
		}
		else 
			this.SendToServer(ClientResponseType.GetSession, "", null)
		
		
		
		const ListenForData = (i: Websocket, ev: MessageEvent) => {
			//console.log(`received message: ${ev.data}`);
			var res : WebSocketReponse = JSON.parse(ev.data)
			switch (res.type) {
				case ServerResponseType.ProvideSession :
					let session = JSON.parse(res.data)
					$cookies.set("session_id", session.id)
					console.log("session : ")
					console.log(session)
					if(!this.localeSetByUser)
						this.$i18n.locale = session.locale
					break
				case ServerResponseType.ProvideUser :
					this.logged = true
					this.user = new User(JSON.parse(res.data))
					console.log("user : ")
					console.log(this.user)
					this.$i18n.locale = this.user.locale
					this.localeSetByUser = true
					/*if(this.user.admin){
						this.SendToServer("request set db", "",this.user.Id)
						this.SendToServer("request item db", "",this.user.Id)
						this.SendToServer("request waifu db", "",this.user.Id)
					}*/
					break
				case ServerResponseType.ProvideMapData :
					this.fighting = true
					this.beatmap = JSON.parse(res.data)
					this.link = 'https://osu.ppy.sh/beatmapsets/'+this.beatmap.beatmapset_id+'#'+this.beatmap.mode+'/'+this.beatmap.id
					//this.notifs.push(new Notification("Fight", "You started a fight with the following beatmap! " + this.link, NotificationSeverity.Notification))
					break
				case ServerResponseType.ProvideMaimaiChartData :
					this.fighting = true
					this.maimai_chart = JSON.parse(res.data)
					break
				case ServerResponseType.ProvideFightResults :
					this.fighting = false 
					this.xp = res.data
					break
				case ServerResponseType.ProvideWaifuDB :
					let waifus = JSON.parse(res.data)
					console.log("WAIFU DATABASE")
					waifus.forEach((waifu:Waifu) => {
						this.all_waifus.push(new Waifu(waifu))
					});
					console.log(this.all_waifus)
					break
				case ServerResponseType.ProvideItemDB :
					this.item_db = JSON.parse(res.data)
					console.log("ITEM DATABASE")
					console.log(this.item_db)
					break
				case ServerResponseType.ProvideEquipmentDB :
					this.equipment_db = JSON.parse(res.data)
					console.log("EQUIPMENT DATABASE")
					console.log(this.equipment_db)
					break
				case ServerResponseType.ProvideSetDB :
					this.set_db = JSON.parse(res.data)
					console.log("SET DATABASE")
					console.log(this.item_db)
					break
				case ServerResponseType.ProvideBanners:
					this.banners = JSON.parse(res.data)
					console.log("Banners data : ")
					console.log(this.banners)
					break
				case ServerResponseType.Notification:
					let notification = Object.assign(new Notification("","",0), JSON.parse(res.data))
					this.notifs.push(notification)
					break
				case ServerResponseType.ProvidePullResults:
					this.pulled_waifus = JSON.parse(res.data)
					console.log("Pulled Waifus data : ")
					console.log(this.pulled_waifus)
					break
				case ServerResponseType.ProvideDungeons:
					this.dungeons = JSON.parse(res.data)
					console.log("Dungeon data : ")
					console.log(this.dungeons)
					break
				case ServerResponseType.ProvideResearchNodes:
					this.researchNodes = JSON.parse(res.data)
					this.researchNodes = this.researchNodes.map(rn => Object.assign(new ResearchNode, rn))
					console.log("research node data : ")
					console.log(this.researchNodes)
					break
				case ServerResponseType.ProvideCraftingRecipes:
					this.craftingRecipes = JSON.parse(res.data)
					this.craftingRecipes = this.craftingRecipes.map(rn => Object.assign(new Craft, rn))
					console.log("crafting recipes data : ")
					console.log(this.craftingRecipes)
					break
				case ServerResponseType.ProvideActiveDungeon:
					this.inside_dungeon = true;
					console.log("ACTIVE DUNGEON")
					console.log(this.active_dungeon)
					this.active_dungeon = JSON.parse(res.data)
					break
				case ServerResponseType.ProvideLoot:
					this.loots.push(JSON.parse(res.data))
					this.loots.forEach(arr => {
						arr.forEach(loot => {
							switch(loot.lootType){
								case LootType.Equipment:
									loot.item = Object.assign(new Equipment, loot.item as Equipment)
									break
							}
						})
					})
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
	
		<Header :dev=dev :logged=logged :user="user" @page-change="updatePage"></Header>
		<div v-if="page == Page.Homepage">
			<Homepage></Homepage>
		</div>
		<div v-else-if="page == Page.Inventory">
			<InventoryPage :user="user"></InventoryPage>
			<!--<StatsBlock :objectType="objectType" :stars="stars" :rarity="rarity" :value="value" :owner="owner" :xp="xp" :lvl="lvl" :b_int="b_int" :b_luck="b_luck" :b_exp="b_exp" :o_int="o_int" :o_luck="o_luck"
			:o_exp="o_exp" :u_int="u_int" :u_exp="u_exp" :diffLvlup="diffLvlup"></StatsBlock>-->
		</div>
		
		<div v-else-if="page == Page.WaifuList">
			<WaifuListPage :user="user"></WaifuListPage>
		</div>
		<div v-else-if="page == Page.YouSomehowEndedUpThere">
			How tf did you even up here?
		</div>
		<div v-else-if="page == Page.User">
			<UserPage :user="user"></UserPage>
		</div>
		<div v-else-if="page == Page.UserOption">
			<UserOptionPage :user=user :theme=user.theme @theme-change="updateTheme"></UserOptionPage>
		</div>
		<div v-else-if="page == Page.AddMap">
			<AddMap :id="user.Id"></AddMap>
		</div>
		<div v-else-if="page == Page.ClaimAndFight">
			<ClaimAndFightPage :maimai_chart="maimai_chart" :xp="xp" :fighting="fighting" :user="user" :beatmap="beatmap"></ClaimAndFightPage>
		</div>
		<div v-else-if="page == Page.WaifuManager">
			<WaifuManagerPage :all_waifus="all_waifus" :id="user.Id"></WaifuManagerPage>
		</div>
		<div v-else-if="page == Page.Pull">
			<PullPage :banners="banners" :pulled_waifus="pulled_waifus" :gacha_currency="user.gacha_currency" :user="user"></PullPage>
		</div>
		<div v-else-if="page == Page.ItemManager">
			<ItemManagerPage :id="user.Id" :item_db="item_db" :equipment_db="equipment_db" :set_db="set_db"></ItemManagerPage>
		</div>
		<div v-else-if="page == Page.InventoryManager">
			<InventoryManagerPage :user="user" :items="item_db"></InventoryManagerPage>
		</div>
		<div v-else-if="page == Page.Dungeon">
			<div v-if="inside_dungeon">
				<ActiveDungeonPage :user="user" :active_dungeon="active_dungeon" @leave-dungeon="inside_dungeon = false"></ActiveDungeonPage>
			</div>
			<div v-else>
				<DungeonSelectionPage :dungeons="dungeons" :user="user"></DungeonSelectionPage>
			</div>
			
		</div>
		<div v-else-if="page == Page.UserWaifuManager">
			<UserWaifuManagerPage  :user="user"></UserWaifuManagerPage>
		</div>
		<div v-else-if="page == Page.Privacy">
			<PrivacyPage ></PrivacyPage>
		</div>
		<div v-else-if="page == Page.Activities">
			<ActivitiesPage :user="user" :research-nodes="researchNodes" :item_db="item_db" :crafting-recipes="craftingRecipes"></ActivitiesPage>
		</div>
		<div v-else>
			ERREUR 404 AHAHAHAHAH
		</div>
		<NotificationMenu :notifs=notifs></NotificationMenu>
		<LootDisplayComponent  :is-new-loot="true"  :loots=loots></LootDisplayComponent>
		<FooterComponent @page-change="updatePage"></FooterComponent>
	</div>
</template>

<style lang="css" scoped>
#main { /*for veils*/ 
	min-height: 100vh;
	position: relative;
}
</style>