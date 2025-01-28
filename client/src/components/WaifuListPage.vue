<script lang="ts">

import Waifu from '@/classes/waifu';

export default {
    name : "WaifuListPage",
    data() {
        return {
            focusedView : false,
            waifuToDisplay : new Waifu({}),
            gridTemplateColumns : "grid-template-columns: 1fr 1fr 1fr 1fr",
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
        updateSorting(filter : number) {
            switch (filter) {
                case 0 :
                    this.waifus.sort((a, b) => a.lvl-b.lvl)
                    break
                case 1 :
                    this.waifus.sort((a, b) => b.lvl-a.lvl)
                    break
                case 2 :
                    this.waifus.sort((a, b) => ('' + a.name).localeCompare(b.name))
                    break
                case 3 :
                    this.waifus.sort((a, b) => ('' + b.name).localeCompare(a.name))
                    break
                default :
                    this.waifus.sort((a, b) => a.lvl-b.lvl)
                    break
            }
        },
        openWaifuDisplay(waifu : Waifu) {
            this.focusedView = !this.focusedView
            this.waifuToDisplay = waifu
        },
        closeWaifuDisplay() {
            this.focusedView = !this.focusedView
            this.waifuToDisplay = new Waifu({})
        },
        generateGridTemplateColumns(select : number) {
            this.gridTemplateColumns;
            this.gridTemplateColumns = "grid-template-columns: ";
            for (let i = 0; i < Number(select); i++) {
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
            <select value="4">
                <option @click="generateGridTemplateColumns(5)" value="5">5</option>
                <option @click="generateGridTemplateColumns(4)" value="4">4</option>
                <option @click="generateGridTemplateColumns(3)" value="3">3</option>
                <option @click="generateGridTemplateColumns(2)" value="2">2</option>
            </select>
        </div>
        <div id="idFilter">
            <label>Filter : </label>
            <select value="LD">
                <option @click="updateSorting(0)" value="LA">Level (Ascendant)</option>
                <option @click="updateSorting(1)" value="LD">Level (Descendant)</option>
                <option @click="updateSorting(2)" value="NA">Name (Ascendant)</option>
                <option @click="updateSorting(3)" value="BD">Name (Descendant)</option>
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
                Difficulty to level up : {{ waifuToDisplay.diffLvlUp }}<br>
                STR : {{ waifuToDisplay.b_str }}<br>
                KAW : {{ waifuToDisplay.b_kaw }}<br>
                INT : {{ waifuToDisplay.b_int }}<br>
                AGI : {{ waifuToDisplay.b_agi }}<br>
                DEX : {{ waifuToDisplay.b_dex }}<br>
                LUCK : {{ waifuToDisplay.b_luck }}<br>
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
    padding-top: 1vh;
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