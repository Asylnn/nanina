<script lang="ts">
import type Loot from '@/classes/loot/loot';
import LootType from '@/classes/loot/loot_type';


export default {
    name : "Homepage",
    data() {
        return {
            LootType: LootType,
            publicPath : import.meta.env.BASE_URL,
        }
    },
    props:{
        loots:{
            type: Array<Loot[]>,
            required:true,
        }
    },
    methods: {
        removeLoot()
        {
            this.loots.pop()
        }
    }

}


</script>
<template>
    <div v-if="loots.length != 0">
        <div @click="removeLoot" class="veil" id="lootveil"></div>
        <div id="loots">
            <div v-for="loot in loots[0]">
                <div v-if="loot.lootType == LootType.WaifuXP" class="loot">
                    <img src="../assets/waifu_xp.svg">
                    <div>{{ loot.amount }}</div>
                </div>
                <div v-else-if="loot.lootType == LootType.Equipment" class="loot">
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

}

img
{
    width: 5vw;
    height: 5vw;
}

#loots {
    padding:10px 20px;
    position:fixed;
    display: flex;
    top:20vh;
    /*right:20vw;
    left:20vw;*/
    left:50%;
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
}


</style>