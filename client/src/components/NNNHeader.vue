
<script lang="ts">

import type { PropType } from 'vue';
import Page from '../classes/page';
import config from '../../../config.json'

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
        admin : {
            type : Boolean,
            required : true
        },
        dev : {
            type : Boolean,
            required : true
        },
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
            <li @click="onClickChangePage(15)" v-if="admin"><span>Item DB</span></li>   
            <li @click="onClickChangePage(8)" v-if="admin"><span>Waifu DB</span></li>
            <li @click="onClickChangePage(6)" v-if="admin && dev"><span>Add Beatmap</span></li>
            <li @click="onClickChangePage(16)" v-if="admin && dev"><span>InventoryManager</span></li>
        </ul>
        <ul id="buttList">
            <select id="language" v-model="$i18n.locale" @change="onChangeLocale()">
                <option value = "en">🏴󠁧󠁢󠁥󠁮󠁧󠁿</option>
                <option value = "fr">🇫🇷</option>
            </select>
            <li v-if="!logged"><a :href="config.dev ? config.dev_discord_oauth_url : config.prod_discord_oauth_url">Discord</a></li>
            <li v-else><span @click="onClickChangePage(5)"><img height=25px width=25px src="../assets/option_gear_from_google_probably_not_free_of_use.png"></span></li>
        </ul>
    </header>
</template>

<style lang="css" scoped>

#language{
    font-size : 30px;
    align-content: center;
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
    grid-template-columns: 1fr 4fr 0.5fr;
    padding: 0 15vw;
    z-index: 9000;
}
#actiMenu {
    position: absolute;
    z-index: 9999;
}
#actiMenu li {
    padding: 0.727vh 0.5vw;
}
#logo, #pages, #buttList {
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
    grid-template-columns: 1fr 1fr;
    color: greenyellow;
}

li, #logo{
    align-content: center;
}
</style>