<script lang="ts">

import Waifu from '@/classes/waifu';
import WaifuDisplayComponent from './WaifuDisplayComponent.vue';
import WaifuGridDisplayComponent from './WaifuGridDisplayComponent.vue';

export default {
    name : "WaifuListPage",
    data() {
        return {
            focusedView : false,
            waifuDisplayed : new Waifu({}),
            columns : 4,
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
            this.waifuDisplayed = waifu
            this.focusedView = !this.focusedView
        },
        closeWaifuDisplay() {
            this.focusedView = !this.focusedView
            this.waifuDisplayed = new Waifu({})
        },
        updateColumns(num : number){
            this.columns = num
        },
        waifuToDisplay(){
            return this.waifuDisplayed
        },
        columnsToSend(){
            return this.columns
        }
    },
    components: {
        WaifuDisplayComponent,
        WaifuGridDisplayComponent,
    }
}


</script>

<template>
    <div id="filters">
        <div id="rowFilter">
            <label>Number per row : </label>
            <select value="4">
                <option @click="updateColumns(5)" value="5">5</option>
                <option @click="updateColumns(4)" value="4">4</option>
                <option @click="updateColumns(3)" value="3">3</option>
                <option @click="updateColumns(2)" value="2">2</option>
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
    <WaifuGridDisplayComponent :waifus=waifus :columns=columnsToSend()></WaifuGridDisplayComponent>
    <div v-if="focusedView">
        <div @click="closeWaifuDisplay()" id="veil"></div>
        <WaifuDisplayComponent :waifu="waifuToDisplay()" :count=-1></WaifuDisplayComponent>
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
#veil {
    position: absolute;
    top: 0;
    left: 0;
    height: 100%;
    width: 100%;
    background-color: rgba(0,0,0,0.8);
    z-index: 726;
}
</style>