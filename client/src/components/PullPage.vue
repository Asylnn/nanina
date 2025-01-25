<script lang="ts">

import Waifu from '@/classes/waifu';
import Banner from '@/classes/banner';
import WaifuManagerComponent from './WaifuManagerComponent.vue';

export default {
    name : "PullPage",
    data() {
        return {
            selected_banner: new Banner()
        }
    },
    props: {
        id : {
            type : String,
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
        }
    },
    components: {
        WaifuManagerComponent
    },
    methods :{
        pull(pullAmount: number){
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"pull request", data:JSON.stringify({bannerId:this.selected_banner.bannerId, pullAmount:pullAmount}), id: this.id}))
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