
<script lang="ts">

import Page from '../../classes/page';
import config from '../../../../config.json'
import User from '@/classes/user/user';
import ClientResponseType from '@/classes/client_response_type';
import CherishMenu from './CherishMenu.vue';
import type Item from '@/classes/item/item';

export default {
    name : "Header",
    data() {
        return {
            config: config,
            showEnergyItems: false,
            Page: Page,
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
        },
        itemDb : {
            type : Array<Item>, //Required for CherishMenu
            required: true,
        }
    },
    emits: ["theme-change","page-change"],
    methods : {
        onClickChangePage(page: Page){
            this.$emit("page-change", page)

        },
        onChangeLocale(){
            this.SendToServer(ClientResponseType.UpdateLocale, this.$i18n.locale, null)
        },
        getEnergyColor(){
            if(this.user.energy >= this.user.max_energy)
                return `color: lightgoldenrodyellow`
            return ""
        }
    },
    components:{
        CherishMenu
    }
}

</script>


<template>
    <header>
        <div id="logo">
            <p @click="onClickChangePage(Page.Homepage)">Nanina</p>
        </div>
        <ul id="pages" v-if="logged">
            <li class="clickable" @click="onClickChangePage(19)">
                <span >{{ $t("header.activities") }}</span>
            </li>
            <li class="clickable" @click="onClickChangePage(Page.Inventory)"><span>{{ $t("header.inventory") }}</span></li>
            <li class="clickable" @click="onClickChangePage(Page.WaifuList)"><span>{{ $t("header.waifus") }}</span></li>
            <li class="clickable" @click="onClickChangePage(Page.ClaimAndFight)"><span>{{ $t("header.fighting") }}</span></li>
            <li class="clickable" @click="onClickChangePage(Page.Pull)"><span>{{ $t("header.pull") }}</span></li>
            <li class="clickable" @click="onClickChangePage(Page.Dungeon)"><span>{{ $t("header.dungeon") }}</span></li>
            <li id="db_li" v-if="user.admin">
                <span class="clickable" >Databases</span>
                <ul id="dbMenu">
                    <li class="clickable" @click="onClickChangePage(Page.ItemManager)"><span>Item</span></li>   
                    <li class="clickable" @click="onClickChangePage(Page.WaifuManager)"><span>Waifu</span></li>
                </ul>
            </li>
            <li id="manager_li" v-if="user.admin && dev">
                <span class="clickable" >Manager</span>
                <ul id="managerMenu">
                    <li class="clickable" @click="onClickChangePage(Page.InventoryManager)"><span>Inventory</span></li>
                    <li class="clickable" @click="onClickChangePage(Page.UserWaifuManager)"><span>Waifu</span></li>
                </ul>
            </li>
            <li class="clickable" @click="onClickChangePage(Page.AddMap)" v-if="user.admin && dev"><span>Add Beatmap</span></li>
        </ul>
        <div id="buttList">
            <div class="butitem halo" >
                <select id="language" v-model="$i18n.locale" @change="onChangeLocale()">
                    <option value = "en">🏴󠁧󠁢󠁥󠁮󠁧󠁿</option>
                    <option value = "fr">🇫🇷</option>
                </select>
            </div>
            <div class="butitem halo" v-if="!logged">
                <a :href="config.dev ? config.dev_discord_oauth_url : config.prod_discord_oauth_url"><img id="discordLogin" src="@/assets/discord.png"></a>
            </div>
            <div class="butitem halo" @click="onClickChangePage(Page.UserOption)" v-else>
                <img height=38px width=38px src="@/assets/settings.png">
            </div>
            <div id="user" class="butitem" v-if="logged" @click="onClickChangePage(Page.User)"><img :src="`${user.avatarPATH}?size=40`"></div>
            
        </div>
        <div v-if="logged" id="energy">
            <div><img height=30px width=30px src="@/assets/heart.svg"></div>
            <div id="energyAmount" v-if="logged" :style="getEnergyColor()" ><p >{{ Math.floor(user.energy) }}</p></div>
            <div @click="showEnergyItems = !showEnergyItems"><img id="addEnergy" height=26px width=26px src="@/assets/plus.svg"></div>
            <CherishMenu v-if="showEnergyItems" :user="user" :item-db="itemDb"></CherishMenu>
        </div>
        
    </header>
    
</template>

<style lang="css" scoped>

#discordLogin
{
    width: 40px;
    height: 40px;
}

#language
{
    font-size : 30px;
    width:38px;
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



header, #pages {
    display: grid;
}

header, #dbMenu li, #managerMenu li {
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

#dbMenu, #managerMenu {
    position: absolute;
    z-index: 9999;
    display: none;
}

#db_li:hover #dbMenu, #manager_li:hover #managerMenu{
    display: block;
}

#dbMenu li, #managerMenu li {
    padding: 0.727vh 0.5vw;
}

#logo, .butitem
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

#addEnergy
{
    cursor:pointer;
}

#addEnergy:hover
{
    filter:brightness(1.5)
}

header
{
    margin-bottom: 20px;
}

</style>