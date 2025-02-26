<script lang="ts">
import type Item from '@/classes/item/item';
import Waifu from '@/classes/waifu/waifu';


export default {
    name : "WaifuGridDisplayComponent",
    data() {
        return {
            publicPath : import.meta.env.BASE_URL,
        }
    },
    props: {
        elements: {
            type : [Array<Waifu>, Array<Item>],
            required : true
        },
        columns: { //grid-template-columns
            type : Number,
            required : true
        },
        noMargin: {
            type: Boolean,
            default:false,
            required : false
        }
        
    },
    mounted() {
        console.log("typeof" + typeof this.elements[0])
    },
    computed: { //omg a computed that actually works??????????
        generateGridTemplateColumns() {
            var style = `grid-template-columns:${"1fr ".repeat(this.columns)};`
            if(!this.noMargin)
                style += "margin: 0 17.27vw;"
            return style
        },
    },
    emits: ["show-element"],
    methods: {
        onShowElement(waifu : Waifu | Item) {
            this.$emit("show-element", waifu)
        },
    },
}

</script>

<template>
    <div id="grid" :style="generateGridTemplateColumns">
        <div v-for="element in elements">
            <div v-if="(element as Waifu).b_dex != null"> <!-- Pretty bad way to test if it's a waifu Object, but it works-->
                <div class="slot">
                    <div class="waifuIcon">
                        <img @click="onShowElement(element as Waifu)" :src="`${publicPath}/waifu-image/${element.imgPATH}`">
                    </div>
                </div>
                <p>{{ $t("waifu.level") }} {{ (element as Waifu).lvl }}</p>
            </div>
            <div v-else>
                <div class="slot">
                    <div class="itemIcon">
                        <img @click="onShowElement(element as Item)" :src="`${publicPath}/item-image/${element.imgPATH}`">
                    </div>
                    <p v-if="(element as Item).count != 1">{{ (element as Item).count }}</p>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#grid {
    display:none;
    position:relative;
    display: grid;
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

.itemIcon {
    padding-bottom: 6vh;
    display: table;
    margin: 0 auto;
}

.itemIcon img
{
    width: 64px;
    height: 64px;
    cursor: pointer;
}

.slot p {
    text-align: center;
}
</style>