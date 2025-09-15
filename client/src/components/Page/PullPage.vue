<script lang="ts">

import Waifu from '@/classes/waifu/waifu';
import Banner from '@/classes/banner/banner';
import WaifuDisplayComponent from '../Component/WaifuDisplayComponent.vue';
import type Dictionary from '@/classes/dictionary';
import User from '@/classes/user/user';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import ClientResponseType from '@/classes/client_response_type';
import { WebsocketEvent, type Websocket } from 'websocket-ts';
import ServerResponseType from '@/classes/server_response_type';
import type WebSocketResponse from '@/classes/web_socket_response';
import type Item from '@/classes/item/item';
import type Loot from '@/classes/loot/loot';
import LootType from '@/classes/loot/loot_type';

export default {
    name : "PullPage",
    data() {
        return {
            tempLoot: [] as Loot[],
            selected_banner: new Banner(),
            pulled_banner: new Banner(),
            pulled_waifus : [] as Waifu[],
            showing_history : false,
            count : 2,
            focusedView : false,
            listener : (() => {}) as (i: Websocket, ev: MessageEvent) => void,
        }
    },
    props: {
        user : {
            type : User,
            required : true,
        },
        banners: {
            type : Object as () => Dictionary<Banner>, //ugly but it works
            required : true
        },
    },
    components: {
        WaifuDisplayComponent,
        GridDisplayComponent,
    },
    mounted(){
        this.selected_banner = Object.values(this.banners)[0]
        this.listener = (i: Websocket, ev: MessageEvent) => {
			var res : WebSocketResponse = JSON.parse(ev.data)
			switch (res.type) {
				case ServerResponseType.ProvidePullResults:
                    let data = JSON.parse(res.data) as {waifus:Waifu[], items:Item[]}
                    data.waifus = JSON.parse(data.waifus.toString()) //It's actually a string even before calling the toString method, but the linter tell me otherwise /shrug
                    data.items = JSON.parse(data.items.toString())
                    this.user.gacha_currency -= this.pulled_banner.pullCost;
					this.pulled_waifus = data.waifus
                    for(let item of data.items)
                    {
                        console.log(data)
                        console.log(data.items)
                        this.user.inventory.AddItem(item)
                        this.tempLoot.push({
                            lootType: LootType.Item,
                            item:item,
                            amount:1
                        })
                    }
                        
					console.log("Pulled Waifus data : ")
					console.log(this.pulled_waifus)

                    this.pulled_waifus.forEach(waifu => 
                    {
                        if(!this.user.waifus[waifu.id])
                            this.user.waifus[waifu.id] = waifu
                    })
                    
                    break;
            }
        }
        //@ts-ignore
		this.ws.addEventListener(WebsocketEvent.message, this.listener);
    },
    unmounted()
    {
        //@ts-ignore
        this.ws.removeEventListener(WebsocketEvent.message, this.listener)
    },
    methods :{
        showHistory(){
            this.showing_history = !this.showing_history
        },
        pull(pullAmount: number){
            this.pulled_banner = this.selected_banner
            this.SendToServer(ClientResponseType.GetPullResults, JSON.stringify({bannerId:this.selected_banner.id, pullAmount:pullAmount}), this.user.Id)
            console.log("J'ai pull")
            this.count = 0
            this.focusedView = true
        },
        incrementCount(){
            if (this.pulled_waifus.length > this.count + 1) {
                this.count++
            }
            else {
                this.focusedView = false
                this.$emit("pull-finished-show-loot", this.tempLoot)
                this.tempLoot = []
            }
        },
        waifuToSend(){
            return this.pulled_waifus[this.count]
        },
        waifusToSend(){
            return this.pulled_waifus
        },
        countToSend(){
            return this.count
        },
    },
    emits:["pull-finished-show-loot"],
}

</script>


<template>
    <div id="gachaWindow">
        <div id="bannerInfo" class="margins">
            <select v-for="banner in banners" v-model="selected_banner">
                <option :value="banner" >{{banner.id}}</option>
            </select>
            <button class="nnnbutton" @click="pull(1)">{{ $t("gacha.pull", {pullAmount:1}) }}</button>
            <button class="nnnbutton" @click="pull(10)">{{ $t("gacha.pull", {pullAmount:10}) }}</button>
            <div id="gc-display">
                <span>{{ Math.floor(user.gacha_currency) }}</span>
                <img src="@/assets/gc.svg"></img>
            </div>
        </div>
        <div v-if="pulled_waifus[0] != undefined">
            <div id="gachaPull" v-if=focusedView>
                <div class="veil" id="backveil"></div>
                <div class="veil" id="frontveil" @click="incrementCount"></div>
                <WaifuDisplayComponent :user="user" :for-dungeon="false" :for-pull="true" :waifu="waifuToSend()" :count="countToSend()"></WaifuDisplayComponent>
            </div>
            <div class="grid" v-else>
                <GridDisplayComponent :elements="waifusToSend()" :columns=5></GridDisplayComponent>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>
#bannerInfo {
    display: grid;
    grid-template-columns: 1fr 2fr 2fr 1fr;
    align-items: center;
    margin-bottom: 40px;
}
#bannerInfo select {
    font-size: large;
    cursor: pointer;
    height:30px;
}

#bannerInfo button {
    margin: 0px 40px;
    font-size: larger;
    cursor: pointer;
}

#backveil {
    z-index: 50;
}

#frontveil
{
    background-color: rgba(0,0,0,0);
    z-index:9999;
    cursor: pointer;
}

#gc-display
{
    display: flex;
    place-items: center
}

#gc-display span
{
    margin-right: 10px;
}


</style>