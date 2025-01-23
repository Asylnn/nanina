<script lang="ts">

import WaifuManagerComponent from './WaifuManagerComponent.vue'
import Waifu from '../classes/waifu';
export default {
    name : "WaifuManagerPage",
    data() {
    },
    props: {
        all_waifus : {
            type : Array<Waifu>,
            required : true
        },
    },
    components: {
        WaifuManagerComponent
    }
    
}

</script>

<template>
    <div>
        <WaifuManagerComponent></WaifuManagerComponent>
        

    </div>
</template>

<style lang="css" scoped>
* {
    text-align: center;
}

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