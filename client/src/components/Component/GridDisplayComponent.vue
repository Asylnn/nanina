<script lang="ts">
import Item from '@/classes/item/item';
import Waifu from '@/classes/waifu/waifu';


export default {
    name : "GridDisplayComponent",
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
        },
        showBorder: {
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
        onShowElement(element : Waifu | Item) {
            this.$emit("show-element", element)
        },
        getRarityStyle(rarity: number)
        {
            let style = ""
            let color = "yellow;"
            if(rarity == 0)
                color = "black;"
            else if(rarity == 1)
                color = "blue;"
            else if(rarity == 2)
                color = "green;"
            else if(rarity == 3)
                color = "purple;"
            return style + "border-color:" + color
        }
    },
}

</script>

<template>
    <div id="grid" :class="showBorder ? 'border' : ''" :style="generateGridTemplateColumns">
        <div v-for="element in elements">
            <div v-if="(element as Waifu).b_dex != null"> <!-- Pretty bad way to test if it's a waifu Object, but it works-->
                <div class="slot">
                    <div class="waifuIcon">
                        <img @click="onShowElement(element as Waifu)" :src="`${publicPath}/waifu-image/${element.imgPATH}`">
                    </div>
                    <p>{{ $t(`waifu.${(element as Waifu).id}.name`) }} {{ $t("waifu.level") }} {{ (element as Waifu).lvl }}</p>
                </div>
            </div>
            <div v-else>
                <div class="itemSlot">
                    <div class="rarityBorder" :style="getRarityStyle((element as Item).rarity)">
                        <div class="itemIcon">
                            <img @click="onShowElement(element as Item)" :src="`${publicPath}/item-image/${element.imgPATH}`">
                        </div>
                        <p v-if="(element as Item).count != 1">{{ (element as Item).count }}</p>
                    </div>
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


.border {  /*same border as lootcomponent*/ 
    border-style: solid; 
    border-color: grey;
    border-radius: 10px;
    border-width: 4px;
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
    position:relative;
    left:0px;
    bottom: 0px;
    color:blueviolet;
    text-align: left;
}

.slot {
    margin-bottom: 6vh;
}

.itemSlot
{
    display:flex;
    align-items: center;
    justify-items: center;
    align-content: center;
    justify-content: center;
    padding-top: 15px;
    padding-bottom: 15px;
    margin-bottom: 15px;
    margin-left: 15px;
}

.itemSlot p {
    position:relative;
    left:5px;
    bottom: 18px;
    color:blueviolet;
    text-align: right;
}

.rarityBorder
{
    border-width:5px;
    padding: 8px;
    border-radius:10px;
    border-style:outset;
    width:64px;
    height:64px;
}
</style>