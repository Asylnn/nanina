<script lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import NNNHeader from './components/NNNHeader.vue'
import ImgBlock from './components/ImgBlock.vue';

export default {
  data() {
    return {
        id: 1,
        imgURL: "src/assets/waifu-image/GYrXGACboAACxp7.jpg", 
        objectType: "Waifu",
        xp: 0,
        lvl: 1,
        b_int: 69,
        b_luck: 17,
        b_exp: 52,
        stars: 4,
        modificators: [],
        equipedItems: [],
        action : {},
        owner : "Le seul l'unique",
        name: "Thighs-sama",
        diffLvlup: 10,
        o_exp: 2.1,
        o_int: 2.2,
        o_luck: 2.3,
        u_exp: 3.1,
        u_int: 3.2,
        rarity: 4,
        value: -1,
        isTradable: false,


        theme: "dark_theme"
    }
  },
  components:{
    NNNHeader,
    ImgBlock,
  }
}

</script>

<template>
  <div id="main" :class="[theme]">
    <NNNHeader></NNNHeader>
    <ImgBlock :imgURL="imgURL" :name="name"></ImgBlock>
    <div id="statsBlock">
        <div>
            <p style="margin-top: 50px">Je suis une {{ objectType }}
                <span v-for="n in stars">&#11088;</span> de rareté {{ rarity }}!</p>
            <p>Ma valeur est de {{ value }} car je suis unique ! &#128525;</p>
            <p>J'ai été recueillie par mon amour, {{ owner }} ! &#128536;</p>
            <p>J'ai actuellement {{ xp }} xp et au level {{ lvl }} &#128546;</p>
            <p>Voici mes stats actuelles :</p>
            <p>{{ b_int }} {{ b_luck }} {{ b_exp }} &#127800;</p>
            <p>J'ai démarré avec ces stats là :</p>
            <p>{{ o_int }} {{ o_luck }} {{ o_exp }} &#127800;</p>
            <p>Si tu joues avec moi je peux gagner ces stats là ! &#128150;</p>
            <p>{{ u_int }} {{ u_exp }} &#127800;</p>
            <p>Mais je ne suis pas une fille facile ! J'ai du mal à lvl up avec ma difficulté de {{ diffLvlup }} ! &#128533;</p>
        </div>
    </div>
    <select v-model="theme">
        <option value = "dark_theme">Dark Theme</option>
        <option value = "white_theme">White Theme</option>
        <option value = "cute_theme">Cute Theme</option>
    </select>
  </div>
</template>

<style scoped>
</style>
