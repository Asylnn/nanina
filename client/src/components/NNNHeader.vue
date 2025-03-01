
<script lang="ts">

import type { PropType } from 'vue';
import Page from '../classes/page';
import config from '../../../config.json'
import User from '@/classes/user/user';

export default {
    name : "NNNHeader",
    data() {
        return {
            actiMenu : false,
            config: config
        }
    },
    props: {
        logged : {
            type : Boolean,
            required : true
        },
        dev : {
            type : Boolean,
            required : true
        },
        user : {
            type : User,
            required : true
        }
    },
    emits: ["theme-change","page-change"],
    methods : {
        onClickChangePage(page: Page){
            this.$emit("page-change", page)

        },
        updateActiviesMenu() {
            this.actiMenu = !this.actiMenu
        },
        onChangeLocale(){
            this.SendToServer("change locale", this.$i18n.locale, null)
        },
        getEnergyColor(){
            if(this.user.energy >= this.user.max_energy)
                return `color: lightgoldenrodyellow`
            return ""
        }
    },
}

</script>


<template>
    <header>
        <div id="logo">
            <p @click="onClickChangePage(0)">Nanina</p>
        </div>
        <ul id="pages" v-if="logged">
            <li @click="updateActiviesMenu">
                <span >{{ $t("header.activities") }}</span><img width=30px src="../assets/fleche-vers-le-bas.png">
                <ul id="actiMenu" v-if="actiMenu">
                    <li>{{ $t("header.maidCafé") }}</li>
                    <li>{{ $t("header.mineralMining") }}</li>
                    <li>{{ $t("header.technoTree") }}</li>
                    <li>{{ $t("header.gadgetAnalyse") }}</li>
                    <li>{{ $t("header.exploration") }}</li>
                </ul>
            </li>
            <li @click="onClickChangePage(1)"><span>{{ $t("header.inventory") }}</span></li>
            <li @click="onClickChangePage(14)"><span>{{ $t("header.waifus") }}</span></li>
            <li @click="onClickChangePage(7)"><span>{{ $t("header.fighting") }}</span></li>
            <li @click="onClickChangePage(9)"><span>{{ $t("header.pull") }}</span></li>
            <li @click="onClickChangePage(10)"><span>{{ $t("header.dungeon") }}</span></li>
            <li @click="onClickChangePage(15)" v-if="user.admin"><span>Item DB</span></li>   
            <li @click="onClickChangePage(8)" v-if="user.admin"><span>Waifu DB</span></li>
            <li @click="onClickChangePage(6)" v-if="user.admin && dev"><span>Add Beatmap</span></li>
            <li @click="onClickChangePage(16)" v-if="user.admin && dev"><span>InventoryManager</span></li>
        </ul>
        <div id="buttList">
            <div class="butitem halo" >
                <select id="language" v-model="$i18n.locale" @change="onChangeLocale()">
                    <option value = "en">🏴󠁧󠁢󠁥󠁮󠁧󠁿</option>
                    <option value = "fr">🇫🇷</option>
                </select>
            </div>
            <div class="butitem halo" v-if="!logged">
                <a :href="config.dev ? config.dev_discord_oauth_url : config.prod_discord_oauth_url"><img id="discordLogin" src="../assets/discord.png"></a>
            </div>
            <div class="butitem halo" @click="onClickChangePage(5)" v-else>
                <img height=30px width=30px src="../assets/option_gear_from_google_probably_not_free_of_use.png">
            </div>
            <div id="user" class="butitem" v-if="logged" @click="onClickChangePage(4)"><img :src="`${user.avatarPATH}?size=40`"></div>
            
        </div>
        <div id="energy">
            <div><img height=30px width=30px src="../assets/heart.svg"></div>
            <div id="energyAmount" v-if="logged" :style="getEnergyColor()" ><p >{{ Math.floor(user.energy) }}</p></div>
            <div><img height=26px width=26px src="../assets/plus.svg"></div>
        </div>
    </header>
</template>

<style lang="css" scoped>

#discordLogin
{
    width: 40px;
    height: 40px;
}

#language{
    font-size : 30px;
    width:32px;
    cursor:pointer;
    /*display: flex;
    justify-content: center;
    align-content: center;*/
}

select {
    border:0;
    background:none;
    -webkit-appearance: none;
    appearance: none;
}



header, #pages, #buttList {
    display: grid;
}
header, #actiMenu li {
    text-align: center;
    background-color: rgb(39, 11, 65);
}
header {
    position:sticky;
    top:0;
    min-height: 60px;
    height:6vh;
    grid-template-columns: 1fr 4fr 0fr 0.2fr 0.2fr 0.2fr;
    padding: 0 8vw;
    z-index: 9000;
}
#actiMenu {
    position: absolute;
    z-index: 9999;
}
#actiMenu li {
    padding: 0.727vh 0.5vw;
}
#logo, #pages, .butitem
{
    cursor: pointer;
}

#logo {
    color:blueviolet;
    font: italic bold 4.5vh cursive;
}

#pages {
    color: rgb(203, 165, 221);
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
}

#buttList {
    width:15vw;
    display:flex;
    flex-direction: row;
    align-items: center;
    justify-content: flex-start;
}

.butitem
{
    margin-right: 20px;
    
}

.halo
{
    width:45px;
    height:45px;
    border-radius: 40px;
    text-align: center;
    display: flex;
    justify-content: center;
    align-items: center;
}

.halo:hover
{
    background-color: blueviolet;
}

#user
{
    width:40px;
    height:40px;
    border-radius: 40px;
    border-style:hidden;
    border-color: blueviolet;
    overflow: hidden;
}

#user img
{
    width:40px;
    height:40px;
}

#user:hover
{
    border-style:solid;
    transform: translateX(-3px);
}

#energy
{
    width:5vw;
    display: flex;
    align-items: center;
}

li, #logo{
    align-content: center;
}

#energy div img
{
    margin-top: 5px;
}

#energyAmount
{
    margin-right: 20px;
}

</style>