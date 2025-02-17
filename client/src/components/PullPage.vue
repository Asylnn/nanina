<script lang="ts">

import Waifu from '@/classes/waifu/waifu';
import Banner from '@/classes/banner';
import WaifuDisplayComponent from './WaifuDisplayComponent.vue';
import PullBannerHistory from '@/classes/user/pull_history';
import type Dictionary from '@/classes/dictionary';
import User from '@/classes/user/user';
import GridDisplayComponent from './GridDisplayComponent.vue';

export default {
    name : "PullPage",
    data() {
        return {
            selected_banner: new Banner(),
            showing_history : false,
            count : 2,
            focusedView : false,
        }
    },
    props: {
        user : {
            type : User,
            required : true,
        },
        gacha_currency : {
            type : Number,
            required: true,
        },
        pulled_waifus : {
            type : Array<Waifu>,
            required : true
        },
        banners: {
            type : Array<Banner>,
            required : true
        },
    },
    components: {
        WaifuDisplayComponent,
        GridDisplayComponent,
    },
    mounted(){
        this.selected_banner = this.banners[0]
    },
    methods :{
        showHistory(){
            this.showing_history = !this.showing_history
        },
        pull(pullAmount: number){
            this.SendToServer("pull request", JSON.stringify({bannerId:this.selected_banner.id, pullAmount:pullAmount}), this.user.Id)
            console.log("J'ai pull")
            this.count = 0
            this.focusedView = true
            console.log(this.pulled_waifus[this.count])
        },
        incrementCount(){
            if (this.pulled_waifus.length > 1) {
                if (this.count == 9) {
                    this.focusedView = false
                }
                else {
                    this.count += 1
                }
            }
            else {
                this.focusedView = false
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
}

</script>


<template>
    <div id="gachaWindow">
        <div id="bannerInfo">
            <select v-for="banner in banners" v-model="selected_banner">
                <option :value="banner" >{{banner.bannerName}}</option>
            </select>
            <button @click="pull(1)">Pull 1</button>
            <button @click="pull(10)">Pull 10</button>
            <span>Gacha Currency : {{ gacha_currency }}</span>
        </div>
        <div v-if="pulled_waifus[0] != undefined">
            <div id="gachaPull" v-if=focusedView>
                <div @click="incrementCount" id="veil"></div>
                <WaifuDisplayComponent :user="user" :for-pull="true" :waifu="waifuToSend()" :count="countToSend()"></WaifuDisplayComponent>
            </div>
            <div id="gridPull" v-else>
                <GridDisplayComponent :elements="waifusToSend()" :columns=5></GridDisplayComponent>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>
#bannerInfo {
    display: grid;
    grid-template-columns: 1fr 3fr 3fr 1fr;
}
#bannerInfo select {
    font-size: large;
    cursor: pointer;
}
#bannerInfo span {
    color: rgb(185, 83, 185);
}
#bannerInfo button {
    width: 20vw;
    margin-left: 10vw;
    font-size: larger;
    cursor: pointer;
}
#history {
    margin-top: 1vh;
    cursor: pointer;
    font-size:medium;
}

#veil {
    position: absolute;
    top: 0;
    left: 0;
    height: 100%;
    width: 100%;
    background-color: rgba(0,0,0,0.8);
    z-index: 726;
    cursor: pointer;
}
#gridPull {
    display: grid;
    
}
</style>