
<script lang="ts">

import type { PropType } from 'vue';
import Theme from './Theme.vue';
import Page from '../classes/page';

export default {
    name : "NNNHeader",
    data() {
        return {
            _theme: this.theme,
            _page: this.page
        }
    },
    props: {
        logged : {
            type : Boolean,
            required : true
        },
        theme : {
            type : String,
            required : true
        },
        page : {
            type : Number as PropType<Page>,
            required : true
        }
    },
    emits: ["connect-change","real-theme-change","page-change"],
    methods : {
        onClickConnect() {
            this.$emit("connect-change", !this.logged)
        },
        softUpdateTheme(newTheme : string) {
            this._theme = newTheme
            this.$emit("real-theme-change", this._theme)
        },
        onClickHomepage() {
            this.$emit("page-change", Page.Homepage)
        },
        onClickInventory() {
            this.$emit("page-change", Page.Inventory)
        },
        onClickDisconnected() {
            this.$emit("page-change", Page.Disconnected)
        }
    },
    components : {
        Theme
    }
}

</script>


<template>
    <header>
        <div id="logo">
            <p>Nanina</p>
        </div>
        <div id="buttons">
            <ul id="buttList">
                <li><Theme :theme="theme" @theme-change="softUpdateTheme"></Theme></li>
                <li><button id="swapLogged" @click="onClickConnect()">Logged : {{ logged }}</button></li>
            </ul>
        </div>
        <ul id="pages" v-if="logged">
            <li><button @click="onClickHomepage()">Homepage</button></li>
            <li><button @click="onClickInventory()">Inventory</button></li>
            <li><button @click="onClickDisconnected()">Disconnected</button></li>
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
</style>