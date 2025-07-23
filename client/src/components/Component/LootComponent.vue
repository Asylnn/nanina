<script lang="ts">
import type Loot from '@/classes/loot/loot';
import LootType from '@/classes/loot/loot_type';
import ItemComponent from './ItemComponent.vue';
import { getRarityStyle } from '@/classes/utils';

export default {
    name : "Homepage",
    data() {
        return {
            LootType: LootType,
            publicPath : import.meta.env.BASE_URL,
            getRarityStyle:getRarityStyle,
            /*isDisplayingLoot : false,
            displayedLoot : null as Loot | null*/
        }
    },
    props:{
        loots:{
            type: Array<Loot>,
            required:true,
        },
    },
    emits:["display-loot"],
    components:{
        ItemComponent
    }

}


</script>
<template>
    <div id="loots">
        <div v-for="loot in loots" class="loot">
            <div v-if="loot.lootType == LootType.WaifuXP">
                <img src="@/assets/waifu_xp.svg">
                <div>{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.UserXP">
                <img src="@/assets/user_xp.svg">
                <div>{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.Equipment"  @click="$emit('display-loot', loot)" class="rarityBorder" :style="getRarityStyle(loot.item!.rarity)">
                <img :src="`${publicPath}/item-image/${loot.item!.imgPATH}`">
            </div>
            <div v-else-if="loot.lootType == LootType.Item" c>
                <img :src="`${publicPath}/item-image/${loot.item!.imgPATH}`">
                <div class="amount">{{ loot.item!.count }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.GC" >
                <img src="@/assets/gc.svg">
                <div class="amount">{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.Money">
                <img src="@/assets/ugly_coin.svg">
                <div class="amount">{{ loot.amount }}</div>
            </div>
        </div>
        
    </div>
    <div id="square">

        </div>
</template>

<style lang="css" scoped>

img
{
    width: 5vw;
    height: 5vw;
}


@property --progress {
    syntax: '<percentage>';
    inherits: false;
    initial-value: 100%;
}

@property --length {
    syntax: '<percentage>';
    inherits: false;
    initial-value: 0%;
}

@keyframes anim {
    /*0% {border-image:linear-gradient(100deg, transparent);}
    25% {border-image:linear-gradient(100deg, blue, white 20%, red 20% 40%,transparent 40%);}
    50% {border-image:linear-gradient(100deg, blue, white 40%, red 40% 60%,transparent 60%);}
    75% {border-image:linear-gradient(100deg, blue, white 60%, red 60% 80%,transparent 80%);}
    100% {border-image:linear-gradient(100deg, blue, white 80%, red 80% 100%);}*/
    0% {
        --progress:0%;
        --length:0%;
        animation-timing-function:linear;
    }
    20% {
        --progress:20%;
        --length:10%;
        animation-timing-function:linear;
    }
    80% {
        --progress:85%;
        --length:10%;
        animation-timing-function:ease-out;
    }
    100% {
        --progress:100%;
        --length:0%;
        
    }

}

#loots {
    display: flex;
    padding:10px 20px;
    padding-right: 0px; /*Because loot have margin-right: 20px*/ 
    border-color: grey;
    border-style: outset;
    border-image: linear-gradient(110deg, gray var(--progress), purple var(--progress) calc(var(--progress) + var(--length)),transparent calc(var(--progress) + var(--length)));
    border-image-slice: 1;
    background-position: 200px;
    animation-name: anim;
    animation-duration: 1.5s;
    animation-iteration-count: 1;
}

.loot
{
    margin-right:20px;
    text-align: center;
    cursor: pointer;
}

.rarityBorder
{
    border-width:5px;
    padding: 8px;
    border-radius:15px;
    border-style:outset;
    width:5vw;
    height:5vw;
}

</style>