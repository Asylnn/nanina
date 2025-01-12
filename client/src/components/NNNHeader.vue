
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
    },
    emits: ["connect-change","theme-change","page-change"],
    methods : {
        onClickChangePage(page: Page){
            this.$emit("page-change", page)

        },
        onClickConnect() {
            this.$emit("connect-change", !this.logged)
        },
    },
}

</script>


<template>
    <header>
        <div id="logo">
            <p>Nanina</p>
        </div>
        <div id="buttons">
            <ul id="buttList">
                <li><button id="swapLogged" @click="onClickConnect()">Logged : {{ logged }}</button></li>
                <li v-if="!logged"><a href="https://discord.com/oauth2/authorize?client_id=1292571843848568932&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5173&scope=identify">Discord</a></li>
                <li v-else><button @click="onClickChangePage(7)"><img height=32px width=32px src="../assets/option_gear_from_google_probably_not_free_of_use.png"></button></li>
            </ul>
        </div>
        <ul id="pages" v-if="logged">
            <li><button @click="onClickChangePage(0)">Homepage</button></li>
            <li><button @click="onClickChangePage(1)">Inventory</button></li>
            <li><button @click="onClickChangePage(2)">Disconnected</button></li>
            <li><button @click="onClickChangePage(5)">Waifu</button></li>
            <li v-if="admin"><button @click="onClickChangePage(8)">Add Map</button></li>
        </ul>
    </header>
</template>

<style lang="css" scoped>
header {
    border: 2px solid yellow;
    display: grid;
    grid-template-columns: 0.8fr 0.2fr;
}

#logo {
    color:blueviolet;
    font: italic bold 35px cursive;
    text-align: center;
    border: 2px solid blueviolet;
}

#buttons {
    border: 2px solid sienna;
    
}

#swapLogged {
    padding: 0%;
    width: 100%;
    height: 100%;
    border: 0;
}

li {
    margin: 0%;
    padding: 0%;
    font: bold 15px 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    list-style: none;
    text-align: center;
}

#buttList {
    display: grid;
    grid-template-columns: 1fr 1fr;
}

#buttList, #buttList li, #buttList li button, #buttList li select{
    border: 2px solid rgb(25, 49, 185);
    background-color: rgb(199, 54, 199);
    color: rgb(56, 1, 95);
    padding: 0%;
    margin: 0%;
}

#pages, #pages li, #pages li button {
    background-color: rgb(104, 64, 177);
}

#pages {
    margin: 0;
    padding: 0;
    display:grid;
    grid-template-columns: 1fr 1fr 1fr;
    border: 2px solid aqua;
    color: violet;
}

#pages li, #pages li button{
    border: 2px solid green;
    color: coral;
}

#pages li button {
    width: 100%;
    height: 100%;
    border: 0;
}

select {
    padding: 0%;
    height: 100%;
    border: 0;
}
</style>