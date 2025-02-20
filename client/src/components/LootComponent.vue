<script lang="ts">
import type Loot from '@/classes/loot/loot';
import LootType from '@/classes/loot/loot_type';
import ItemComponent from './ItemComponent/ItemComponent.vue';

export default {
    name : "Homepage",
    data() {
        return {
            LootType: LootType,
            publicPath : import.meta.env.BASE_URL,
            isDisplayingLoot : false,
            displayedLoot : null as Loot | null
        }
    },
    props:{
        loots:{
            type: Array<Loot[]>,
            required:true,
        },
    },
    methods: {
        removeLoot()
        {
            this.loots.pop()
        },
        displayLoot(loot : Loot)
        {
            this.isDisplayingLoot = true
            this.displayedLoot = loot
        },
        closeLootDisplay()
        {
            this.isDisplayingLoot = false
            this.displayedLoot = null
        }
    },
    mounted()
    {
        console.log(this.loots)
    },
    
    components:{
        ItemComponent
    }

}


</script>
<template>
    <div v-if="loots.length != 0">
        <div v-if="isDisplayingLoot">
            <div @click="closeLootDisplay" class="veil" id="displaylootveil"></div>
            <div v-if="displayedLoot!.lootType == LootType.Equipment">
                <ItemComponent :is-for-equiping="false" :item="displayedLoot!.item!"></ItemComponent>
            </div>
        </div>
        <div @click="removeLoot" class="veil" id="lootveil"></div>
        <div id="loots">
            <div v-for="loot in loots[0]">
                <div v-if="loot.lootType == LootType.WaifuXP" class="loot">
                    <img src="../assets/waifu_xp.svg">
                    <div>{{ loot.amount }}</div>
                </div>
                <div @click="displayLoot(loot)" v-else-if="loot.lootType == LootType.Equipment" class="loot">
                    <img :src="`${publicPath}/item-image/${loot.item!.imgPATH}`">
                </div>
                <div v-else-if="loot.lootType == LootType.Item" class="loot">
                    <img :src="`${publicPath}/item-image/${loot.item!.imgPATH}`">
                    <div class="amount">{{ loot.item!.count }}</div>
                </div>
                <div v-else-if="loot.lootType == LootType.GC" class="loot">
                    <img src="../assets/gc.svg">
                    <div class="amount">{{ loot.amount }}</div>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#lootveil
{
    z-index: 20;
}

#displaylootveil
{
    z-index: 40;
}

img
{
    width: 5vw;
    height: 5vw;
}

#loots {
    z-index: 30;
    padding:10px 20px;
    position:fixed;
    display: flex;
    top:20vh;
    left:50%;                               /*Make the display be at the center of the screen*/
    transform: translateX(-50%);
    border-style: solid;
    border-color: grey;
    border-radius: 10px;
    border-width: 4px;
}

.loot
{
    margin-right:20px;
    text-align: center;
    cursor: pointer;
}


</style>