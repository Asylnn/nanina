<script lang="ts">

import Waifu from '../classes/waifu';
export default {
    name : "WaifuDisplay",
    data() {
        return {
            style : "width: " + this.waifu.xp/this.waifu.xpToLvlUp * 100 + "%"
        }
    },
    props: {
        waifu : {
            type : Waifu,
            required : true
        },
    }
}

</script>

<template>
    <div>
        <p id="nameWaifu">{{ waifu.name }}</p>
        <div id="backgroundProgressBar">
            <div id="progressBar" :style="style"></div>
        </div>
        <img id="imgWaifu" :src="waifu.imgPATH">
        <div id="centreLesSpans">
            <span>{{ waifu.xp }}/</span>
            <span>{{ waifu.xpToLvlUp }}</span>
        </div>


    </div>
</template>

<style lang="css" scoped>
#imgBlock {
    width: 30%;
    float: left;
    border: 2px solid red;
}

#nameWaifu {
    margin: 0;
    color: salmon;
    font: 33px cursive;
    text-decoration: underline;
    text-align: center;
}

img {
    height: auto;
    width: 25%;
    display: block;
    margin-left: auto;
    margin-right: auto;
}

#backgroundProgressBar {
    height : 15px;
    margin-left:auto;
    margin-right:auto;
    width: 25%;

    background-color: orchid;
}

#progressBar {
    height:100%;
    background-color: turquoise;
}

#centreLesSpans {
    text-align: center;
    margin-left:auto;
    margin-right:auto;
}
</style>