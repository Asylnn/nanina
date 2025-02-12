<script lang="ts">
import Item from '@/classes/item/item';
import Modifier from '@/classes/modifiers/modifiers';
import ModifierManagerComponent from './ModifierManagerComponent.vue'
import Equipment from '@/classes/item/equipment';
import ItemType from '@/classes/item/item_type';


export default {
    name : "ItemManagerComponent",
    data(){
        return {
            ItemType : ItemType //cringe tbh
        }
    },
    props: {
        item: {
            type: [Item, Equipment],
            required: true
        }
    },
    methods:{
        AddModifier(){
            this.item.modifiers.push(new Modifier())
        },
        DeleteModifier(stat:number){
            this.item.modifiers.splice(this.item.modifiers.findIndex(modf => modf.stat == stat), 1)
        },
        Delete(){
            this.$emit("delete-item", this.item.id)
        },
    },
    components:{
        ModifierManagerComponent
    }
}


</script>

<template>
    <div>
        <div class="row">
            <button @click="Delete">delete</button>
            <span class="attribute">Id  <input class="numberInput" v-model="item.id" type="number"></span>
            <select class="attribute" v-model="item.type">
                <option :value = ItemType.Equipment>Equipment</option>
                <option :value = ItemType.UserConsumable>User Consumable</option>
                <option :value = ItemType.WaifuConsumable>Waifu Consumable</option>
                <option :value = ItemType.Material>Material</option>
            </select>
            <span class="attribute">Rarity  <input class="numberInput"v-model="item.rarity" type="number"></span>
            <span v-if="item.type == ItemType.Equipment" class="attribute">Set Id<input class="numberInput" v-model="item.setId" type="number"></span>
            <select v-if="item.type == ItemType.Equipment" class="attribute" v-model="item.piece">
                <option :value = 0>Weapon</option>
                <option :value = 1>Dress</option>
                <option :value = 2>Accessory</option>
            </select>
            <span class="attribute">Img  <input class="imgImput"v-model="item.imgPATH" type="text"></span>
            <img  class="img" :src="'src/assets/item-image/' + item.imgPATH">
        </div>
        
        
        
        <div class="modifiers">
            <div class="row">Modifiers: <button @click="AddModifier">Add Modifier</button></div>
            <div v-for="modifier in item.modifiers">
                <ModifierManagerComponent class="row" :modifier="modifier" @delete-modifier="DeleteModifier"></ModifierManagerComponent>
            </div>
        </div>
        
    </div>


</template>

<style lang="css" scoped>
.row {
    margin-top: 10px;
}
.modifiers{
    margin-left: 10vw;
}

.InventoryHeader{
    align-content: center;
}

#InventoryPages {
    display:grid;
    cursor: pointer;
    text-align: center;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
}


</style>