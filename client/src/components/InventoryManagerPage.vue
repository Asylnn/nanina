<script lang="ts">
import type Equipment from '@/classes/inventory/equipment';
import type Material from '@/classes/inventory/material';
import type UserConsumable from '@/classes/inventory/user_consumable';
import type WaifuConsumable from '@/classes/inventory/waifu_consumable';

//Selectionner un user pour regarder son inventaire et pouvoir le modifier ##PLUS TARD

//



export default {
    name : "InventoryManagerPage",
    data() {
        return {
            page: 1,
            equipment: new Array<Equipment>(),
            user_consumable: new Array<UserConsumable>(),
            waifu_consumable: new Array<WaifuConsumable>(),
            material: new Array<Material>(),
        }
    },
}


</script>
<template>
    Bienvenue a l'inventaire manager
</template>

<style lang="css" scoped>


</style>