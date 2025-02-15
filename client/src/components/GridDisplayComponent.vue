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
    },
    mounted() {
        console.log("typeof" + typeof this.elements[0])
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
        showElement(waifu : Waifu | Item) {
            this.$emit("show-element", waifu)
        },
    },
}

</script>

<template>
    <div id="waifuIcons" :style=generateGridTemplateColumns>
        <div v-for="element in elements">
            <div v-if="(element as Waifu).b_dex != null"> <!-- Pretty bad way to test if it's a waifu Object, but it works-->
                <div class="slot">
                    <div class="icon">
                        <img @click="showElement(element as Waifu)" :src="`${publicPath}/waifu-image/${element.imgPATH}`">
                    </div>
                </div>
                <p>{{element.name}} Level {{ (element as Waifu).lvl }}</p>
            </div>
            <div v-else>
                <div class="slot">
                    <div class="icon">
                        <img @click="showElement(element as Item)" :src="`${publicPath}/item-image/${element.imgPATH}`">
                    </div>
                    <p>{{$t(`item.${element.id}.name`)}} ({{ element.count }})</p>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#waifuIcons {
    padding: 0 17.27vw;
    position:relative;
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
}

.slot {
    margin: 2vh 2vw;
    width: 10vw;
}

.icon {
    border-radius: 15px;
    max-width: 10vw;
    max-height: 20vh;
    overflow: hidden;
}

.icon img {
    max-width: 15vw;
    max-height: 35vh;
    cursor: pointer;
}

.slot p {
    text-align: center;
}
</style>