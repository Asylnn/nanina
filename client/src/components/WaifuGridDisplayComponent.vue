<script lang="ts">
import Waifu from '@/classes/waifu/waifu';


export default {
    name : "WaifuGridDisplayComponent",
    data() {
        return {
        }
    },
    props: {
        waifus: {
            type : Array<Waifu>,
            required : true
        },
        columns: { //grid-template-columns
            type : Number,
            required : true
        },
    },
    computed: { //omg a computed that actually works??????????
        generateGridTemplateColumns() {
            var gridTemplateColumns = "grid-template-columns: ";
            for (let i = 0; i < this.columns; i++) {
                gridTemplateColumns += "1fr ";
            }
            gridTemplateColumns += '; ';
            return gridTemplateColumns
        },
    },
    emits: ["show-waifu"],
    methods: {
        showWaifu(waifu : Waifu) {
            this.$emit("show-waifu", waifu)
        },
    },
}

</script>

<template>
    <div id="waifuIcons" :style=generateGridTemplateColumns>
        <div v-for="waifu in waifus">
            <div class="waifuDisplay">
                <div class="waifuIcon">
                <img @click="showWaifu(waifu)" :src="'src/assets/waifu-image/' + waifu.imgPATH">
                </div>
                <p>{{waifu.name}} Level {{ waifu.lvl }}</p>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>
#waifuIcons {
    padding: 0 17.27vw;
    position:relative;
}
#waifuIcons {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
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
</style>