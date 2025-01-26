<script lang="ts">

import Waifu from '@/classes/waifu';

export default {
    name : "WaifuListPage",
    data() {
        return {
            focusedView : false,
            waifuToDisplay : new Waifu({}),
            gridTemplateColumns : "grid-template-columns: 1fr 1fr 1fr 1fr",
            selected : "4",
            lol : "",
        }
    },
    props: {
        waifus : {
            type : Array<Waifu>,
            required : true
        },
    },
    methods: {
        openWaifuDisplay(waifu : Waifu) {
            this.focusedView = !this.focusedView
            this.waifuToDisplay = waifu
        },
        closeWaifuDisplay() {
            this.focusedView = !this.focusedView
            this.waifuToDisplay = new Waifu({})
        },
        generateGridTemplateColumns() {
            this.selected;
            this.gridTemplateColumns;
            this.gridTemplateColumns = "grid-template-columns: ";
            for (let i = 0; i < Number(this.selected); i++) {
                this.gridTemplateColumns += "1fr ";
            }
            this.gridTemplateColumns += '; ';
        }
    },
}


</script>

<template>
    <div id="filters">
        <div id="rowFilter">
            <label>Number per row : </label>
            <select v-model="selected">
                <option @click="generateGridTemplateColumns()">4</option>
                <option @click="generateGridTemplateColumns()">3</option>
                <option @click="generateGridTemplateColumns()">2</option>
                <option @click="generateGridTemplateColumns()">5</option>
            </select>
        </div>
        <div id="idFilter">
            <label>Filter by name (doesn't work) : </label>
            <select v-model="lol">
                <option>R</option>
                <option>A</option>
                <option>M</option>
            </select>
        </div>
    </div>
    <div id="waifuIcons" :style=gridTemplateColumns>
        <div v-for="waifu in waifus">
            <div class="waifuDisplay">
                <div class="waifuIcon">
                    <img @click="openWaifuDisplay(waifu)" :src="'src/assets/waifu-image/' + waifu.imgPATH">
                </div>
                <p>{{waifu.name}} Level {{ waifu.lvl }}</p>
            </div>
        </div>
    </div>
    <div v-if="focusedView">
        <div @click="closeWaifuDisplay()" id="veil"></div>
        <div id="focusedWaifu">
            <div id="waifuPic"><img :src="'src/assets/waifu-image/' + waifuToDisplay.imgPATH"></div>
            <div id="waifuInfos">
                {{waifuToDisplay.name}} Level {{ waifuToDisplay.lvl }} ({{ waifuToDisplay.xp }} / {{ waifuToDisplay.xpToLvlUp }})<br>
                id : {{ waifuToDisplay.id }}<br>
                Difficulty to level up : {{ waifuToDisplay.diffLvlUp }}
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#filters, #waifuIcons {
    padding: 0 17.27vw;
    position:relative;
}
#filters {
    grid-template-columns: 1fr 1fr;
}
#filters, #waifuIcons {
    display: grid;
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
#veil {
    position: absolute;
    top: 0;
    left: 0;
    height: 100%;
    width: 100%;
    background-color: rgba(0,0,0,0.8);
    z-index: 726;
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
</style>