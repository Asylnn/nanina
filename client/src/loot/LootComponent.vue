<script lang="ts">
import type Loot from '@/classes/loot/loot';
import LootType from '@/classes/loot/loot_type';
import ItemComponent from '@/components/ItemComponent/ItemComponent.vue';

export default {
    name : "Homepage",
    data() {
        return {
            LootType: LootType,
            publicPath : import.meta.env.BASE_URL,
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
        <div v-for="loot in loots">
            <div v-if="loot.lootType == LootType.WaifuXP" class="loot">
                <img src="../assets/waifu_xp.svg">
                <div>{{ loot.amount }}</div>
            </div>
            <div v-if="loot.lootType == LootType.UserXP" class="loot">
                <img src="../assets/user_xp.svg">
                <div>{{ loot.amount }}</div>
            </div>
            <div @click="$emit('display-loot', loot)" v-else-if="loot.lootType == LootType.Equipment" class="loot">
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
</template>

<style lang="css" scoped>

img
{
    width: 5vw;
    height: 5vw;
}

#loots {
    display: flex;
    padding:10px 20px;
    padding-right: 0px; /*Because loot have margin-right: 20px*/ 
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