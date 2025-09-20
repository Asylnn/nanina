<script lang="ts">
import Item from '@/classes/item/item';
import Waifu from '@/classes/waifu/waifu';
import { getRarityStyle } from '@/classes/utils';

export default {
    name : "GridDisplayComponent",
    data() {
        return {
            publicPath : import.meta.env.BASE_URL,
            getRarityStyle:getRarityStyle,
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
        /*noMargin: {
            type: Boolean,
            default:false,
            required : false
        },*/
        showBorder: {
            type: Boolean,
            default:false,
            required : false
        },
        stickyWaifuGrid: {
            type: Boolean,
            default:false,
            required : false
        },
        showItem:{              //Only to set z index above companions
            type:Boolean,
            default : false,
            required : false
        }
        
    },
    mounted() {
        console.log("typeof" + typeof this.elements[0])
    },
    computed: {
        generateGridTemplateColumns() {
            var style = `grid-template-columns:${"1fr ".repeat(this.columns)};`
            /*if(!this.noMargin)
                style += "margin: 0 17.27vw;"*/
            return style
        },
    },
    emits: ["show-element"],
    methods: {
        onShowElement(element : Waifu | Item) {
            this.$emit("show-element", element)
        },
        getGridClasses() : String
        {
            let classes = ""
            classes += this.showBorder ? 'border ' : ''
            classes += this.stickyWaifuGrid ? 'sticky-waifu-grid ' : ''
            classes += this.showItem ? 'sticky-item-grid ' : ''
            return classes
        },
    },
}

</script>

<template>
    <div class="grid" id="grid" :class="getGridClasses()" :style="generateGridTemplateColumns">
        <div v-for="element in elements">
            <div v-if="(element as Waifu).b_dex != null"> <!-- Pretty bad way to test if it's a waifu Object, but it works-->
                <div class="waifu-slot">
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

.border {  /*same border as lootcomponent*/ 
    border-style: solid; 
    border-color: grey;
    border-radius: 10px;
    border-width: 4px;
}

.sticky-waifu-grid {
    z-index: 50;
    position: sticky;
    top : 10vh;
    right: 0px;
    left : 0px;
    padding:0px;
    margin: 0vh 5vw ;
    position:fixed;
    height: 80vh;
    overflow: scroll;
}

.sticky-item-grid {
    z-index: 150;
}

.waifuIcon {
    border-radius: 15px;
    width: 15vw;
    height: 15vw;
    overflow: hidden;
}

.waifuIcon img {
    width: 15vw;
    cursor: pointer;
}

@media only screen and (orientation: landscape) {
    .sticky-waifu-grid {
        margin:5vh 15vw ;
    }
    .waifuIcon {
        width: 12vw;
        height: 12vw;
    }
    .waifuIcon img {
        width: 12vw;
    }
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

.waifu-slot p {
    /*position:relative;
    left:0px;
    bottom: 0px;*/
    color:blueviolet;
    text-align: left;
}

.waifu-slot {
    margin-bottom: 6vh;
    display:flex;
    place-items: center;
    place-content: center;
    flex-direction: column;
}

.itemSlot
{
    display:flex;
    place-items: center;
    place-content: center;
    padding-top: 30px;
    margin-bottom: 15px;
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