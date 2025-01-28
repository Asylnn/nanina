
<script lang="ts">

import type { PropType } from 'vue';
import Page from '../classes/page';

export default {
    name : "NNNHeader",
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
    },
}

</script>


<template>
    <header>
        <div id="logo">
            <p @click="onClickChangePage(0)">Nanina</p>
        </div>
        <ul id="pages" v-if="logged">
            <li @click="onClickChangePage(1)"><span>Inventory</span></li>
            <li @click="onClickChangePage(14)"><span>Waifus</span></li>
            <li @click="onClickChangePage(15)" v-if="admin"><span>Item DB</span></li>
            <li @click="onClickChangePage(7)"><span>Fighting</span></li>
            <li @click="onClickChangePage(9)"><span>Pull!!</span></li>
            <li @click="onClickChangePage(8)" v-if="admin"><span>Waifu DB</span></li>
            <li @click="onClickChangePage(6)" v-if="admin && dev"><span>Add Beatmap</span></li>
        </ul>
        <ul id="buttList">
            <li v-if="!logged"><a href="https://discord.com/oauth2/authorize?client_id=1292571843848568932&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5173&scope=identify">Discord</a></li>
            <li v-else><span @click="onClickChangePage(5)"><img height=25px width=25px src="../assets/option_gear_from_google_probably_not_free_of_use.png"></span></li>
        </ul>
    </header>
</template>

<style lang="css" scoped>

header, #pages, #buttList {
    display: grid;
}

header {
    position:sticky;
    top:0;
    height:6vh;
    text-align: center;
    background-color: rgb(39, 11, 65);
    grid-template-columns: 1fr 4fr 0.5fr;
    padding: 0 15vw;
    z-index: 9999;
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