<script lang="ts">

import Waifu from '@/classes/waifu/waifu';
import WaifuDisplayComponent from '../Component/WaifuDisplayComponent.vue';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import User from '@/classes/user/user';

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
        user : {
            type : User,
            required : true
        },
    },
    methods: {
        updateSorting(filter : number) {
            switch (filter) {
                case 0 :
                    this.user.waifus.sort((a, b) => a.lvl-b.lvl)
                    break
                case 1 :
                    this.user.waifus.sort((a, b) => b.lvl-a.lvl)
                    break
                case 2 :
                    this.user.waifus.sort((a, b) => ('' + this.$t(`waifu.${a.id}.name`)).localeCompare( this.$t(`waifu.${b.id}.name`)))
                    break
                case 3 :
                    this.user.waifus.sort((a, b) => ('' + this.$t(`waifu.${b.id}.name`)).localeCompare(this.$t(`waifu.${a.id}.name`)))
                    break
                default :
                    this.user.waifus.sort((a, b) => a.lvl-b.lvl)
                    break
            }
        },
        openWaifuDisplay(waifu : Waifu) {
            this.waifuDisplayed = waifu
            this.focusedView = !this.focusedView
        },
        closeWaifuDisplay() {
            this.focusedView = false
            this.waifuDisplayed = new Waifu({})
        },
        updateColumns(num : number){
            this.columns = num
        },
        onEscape(){
            this.closeWaifuDisplay()
        }
    },
    components: {
        WaifuDisplayComponent,
        GridDisplayComponent,
    }
}


</script>

<template>
    <div id="filters" >
        <div id="rowFilter">
            <label>{{ $t("waifulist.nbPerRow") }}</label>
            <select value="4">waifus
                <option @click="updateColumns(5)" value="5">5</option>
                <option @click="updateColumns(4)" value="4">4</option>
                <option @click="updateColumns(3)" value="3">3</option>
                <option @click="updateColumns(2)" value="2">2</option>
            </select>
        </div>
        <div id="idFilter">
            <label>{{ $t("waifulist.filter") }}</label>
            <select value="LD">
                <option @click="updateSorting(0)" value="LA">{{ $t("waifulist.levelAscendant") }}</option>
                <option @click="updateSorting(1)" value="LD">{{ $t("waifulist.levelDescendant") }}</option>
                <option @click="updateSorting(2)" value="NA">{{ $t("waifulist.nameAscendant") }}</option>
                <option @click="updateSorting(3)" value="ND">{{ $t("waifulist.nameDescendant") }}</option>
            </select>
        </div>
    </div>
    <!-- tabindex is weird... truly html moment-->
    <GridDisplayComponent @show-element="openWaifuDisplay" tabindex="0" @keydown.esc="closeWaifuDisplay" :elements=user.waifus :columns=columns></GridDisplayComponent>
    <div v-if="focusedView">
        <div @click="closeWaifuDisplay" class="veil" ></div>
        <WaifuDisplayComponent :for-dungeon="false" :for-pull="false" @input="onEscape" :waifu="waifuDisplayed" :user="user" tabindex="0" @keydown.esc="closeWaifuDisplay" :count=-1></WaifuDisplayComponent>
    </div>
</template>

<style lang="css" scoped>

#filters {
    padding: 0 17.27vw;
    position:relative;
}
#filters {
    grid-template-columns: 1fr 1fr;
    padding-top: 1vh;
}
#filters {
    display: grid;
}

</style>