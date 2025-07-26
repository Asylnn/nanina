<script lang="ts">
import type Loot from '@/classes/loot/loot';
import LootType from '@/classes/loot/loot_type';
import ItemComponent from './ItemComponent.vue';
import LootComponent from './LootComponent.vue';

export default {
    name : "LootDisplayComponent",
    data() {
        return {
            LootType: LootType,
            publicPath : import.meta.env.BASE_URL,
            isDisplayingLoot : false,
            displayedLoot : null as Loot | null,
        }
    },
    props:{
        loots:{
            type: Array<Loot[]>,
            required:true,
        },
        isNewLoot:{
            type: Boolean,
            required:true,
        }
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
    
    components:{
        ItemComponent,
        LootComponent
    }

}


</script>
<template>
    <div   v-if="loots.length != 0">
        <div v-if="isDisplayingLoot">
            <div @click="closeLootDisplay" class="veil" id="displaylootveil"></div>
            <div v-if="displayedLoot!.lootType == LootType.Equipment">
                <ItemComponent :is-for-equiping="false" :is-for-loot="true" :item="displayedLoot!.item!"></ItemComponent>
            </div>
        </div>
        <div @click="removeLoot" class="veil" id="lootveil"></div>
        <div id="lootDisplay">
            <LootComponent :play-animation=true @display-loot="displayLoot"  :loots="loots[0]"></LootComponent>
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

#lootDisplay {
    position:fixed;
    z-index: 30;
    top:20vh;
    left:50%;                               
    transform: translateX(-50%);
}



</style>