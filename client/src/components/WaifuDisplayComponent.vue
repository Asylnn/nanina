<script lang="ts">
import Waifu from '@/classes/waifu/waifu';


export default {
    name : "WaifuDisplayComponent",
    data() {
        return {
        }
    },
    props: {
        waifu: {
            type : Waifu,
            required : true
        },
        count: { //= -1 for single pull in gacha or for a display in WaifuListPage
            type : Number,
            required : true
        }
    },
}

</script>

<template>
    <div id="focusedWaifu" >
        <div id="waifuPic"><img :src="'src/assets/waifu-image/' + waifu.imgPATH"></div>
        <div id="waifuInfos">
            <span v-if="count != -1">Pull number {{ count+1 }}</span><br>
            {{waifu.name}} Level {{ waifu.lvl }} ({{ waifu.xp }} / {{ waifu.xpToLvlUp }})<br>
            id : {{ waifu.id }}<br>
            Difficulty to level up : {{ waifu.diffLvlUp }}
            STR : {{ waifu.b_str }}<br>
            KAW : {{ waifu.b_kaw }}<br>
            INT : {{ waifu.b_int }}<br>
            AGI : {{ waifu.b_agi }}<br>
            DEX : {{ waifu.b_dex }}<br>
            LUCK : {{ waifu.b_luck }}<br>
        </div>
    </div>
</template>

<style lang="css" scoped>

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
#focusedWaifu img {
    max-width: 25vw;
    max-height: 60vh;
}

#waifuPic {
    border-radius: 15px;
    overflow: hidden;
}
#waifuInfos {
    padding: 0 1vw;
}

</style>