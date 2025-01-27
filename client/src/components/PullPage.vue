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
            showing_history : false,
            end_screen : false,
            count : 0,
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
            this.focusedView = true
            this.count = 0
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
        <div id="gachaPull" v-if="focusedView">
            <div @click="incrementCount" id="veil"></div>
            <div id="focusedWaifu">
                <div id="waifuPic"><img :src="'src/assets/waifu-image/' + pulled_waifus[count].imgPATH"></div>
                <div id="waifuInfos">
                    Pull number {{ count+1 }}<br>
                    {{pulled_waifus[count].name}} Level {{ pulled_waifus[count].lvl }} ({{ pulled_waifus[count].xp }} / {{ pulled_waifus[count].xpToLvlUp }})<br>
                    id : {{ pulled_waifus[count].id }}<br>
                    Difficulty to level up : {{ pulled_waifus[count].diffLvlUp }}
                </div>
            </div>
        </div>
        <div id="gridPull" v-else>
            <div id="waifuIcons">
                <div v-for="waifu in pulled_waifus">
                    <div class="waifuDisplay">
                        <div class="waifuIcon">
                        <img :src="'src/assets/waifu-image/' + waifu.imgPATH">
                        </div>
                        <p>{{waifu.name}} Level {{ waifu.lvl }}</p>
                    </div>
                </div>
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
#focusedWaifu {
    position: fixed;
    width: 40vw;
    height: 50vh;
    border-radius: 15px;
    display: grid;
    grid-template-columns: 1fr 1fr;
    padding: 2vh 2vh;
    z-index: 727;
    top: 25vh; 
    left: 30vw;
    background-color: rgb(6, 16, 26);
}
#waifuPic {
    border-radius: 15px;
    overflow: hidden;
}
#waifuInfos {
    padding: 0 1vw;
}
#focusedWaifu img {
    max-width: 25vw;
    max-height: 60vh;
}
#gridPull {
    display: grid;
    
}
#waifuIcons {
    padding: 0 17.27vw;
    position:relative;
}
#waifuIcons {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
}
.waifuDisplay {
    margin: 2vh 2vw;
    width: 10vw;
}
.waifuIcon {
    border-radius: 15px;
    max-width: 10vw;
    max-height: 20vh;
    overflow: hidden;
}
.waifuIcon img {
    max-width: 15vw;
    max-height: 35vh;
    cursor: pointer;
}
.waifuDisplay p {
    text-align: center;
}
</style>