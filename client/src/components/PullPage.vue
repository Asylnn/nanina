<script lang="ts">

import Waifu from '@/classes/waifu';
import Banner from '@/classes/banner';
import WaifuManagerComponent from './WaifuManagerComponent.vue';
import PullBannerHistory from '@/classes/pull_history';
import type Dictionary from '@/classes/dictionary';
import User from '@/classes/user';

export default {
    name : "PullPage",
    data() {
        return {
            selected_banner: new Banner(),
            showing_history : false
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
        WaifuManagerComponent
    },
    mounted(){
        this.selected_banner = this.banners[0]
    },
    methods :{
        showHistory(){
            this.showing_history = !this.showing_history
        },
        pull(pullAmount: number){
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"pull request", data:JSON.stringify({bannerId:this.selected_banner.bannerId, pullAmount:pullAmount}), id: this.user.Id}))
        },
    },
}

</script>


<template>
    <div>
        <select v-for="banner in banners" v-model="selected_banner">
            <option :value="banner" >{{banner.bannerName}}</option>
        </select>
        
        <span Gacha Currency>{{ gacha_currency }}</span><br>
        <button @click="pull(1)">Pull 1!</button>
        <button @click="pull(10)">Pull 10!</button>
        <li v-for="waifu in pulled_waifus">
            <WaifuManagerComponent :waifu="waifu" @delete="pull"></WaifuManagerComponent>
            <img id="imgWaifu" :src="'src/assets/waifu-image/' + waifu.imgPATH">
        </li>
        <button @click="showHistory">Pull History</button>
        <li v-if="showing_history" v-for="waifuId in user.pullBannerHistory[selected_banner.bannerId].pullHistory">
            <span>waifu Id : {{ waifuId }}</span>
        </li>
        <span v-if="showing_history" >pity: {{ user.pullBannerHistory[selected_banner.bannerId].pullBeforePity }}</span>

    </div>
</template>

<style lang="css" scoped>
li {
    display: grid;
    grid-template-columns: 3fr 1fr;
}
img {
    height: 15vh;
}

</style>