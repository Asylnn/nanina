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
            ItemType : ItemType, //cringe tbh
            publicPath : import.meta.env.BASE_URL
        }
    },
    props: {
        item: {
            type: [Item, Equipment],
            required: true
        },
        forInventoryManager:{
            type: Boolean,
            required: false,
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
            let id = this.item.type == ItemType.Equipment ? this.item.inventoryId : this.item.id
            this.$emit("delete-item", id)
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
            <button class="smallbutton nnnbutton" @click="Delete">delete</button>
            <span class="attribute">id  <input class="numberInput" v-model="item.id" type="number"></span>
            <span class="attribute">count  <input class="numberInput" v-model="item.count" type="number"></span>
            <select class="attribute" v-model="item.type">
                <option :value = ItemType.Equipment>equipment</option>
                <option :value = ItemType.UserConsumable>user consumable</option>
                <option :value = ItemType.WaifuConsumable>waifu consumable</option>
                <option :value = ItemType.Material>material</option>
            </select>
            <span class="attribute">rarity  <input class="numberInput"v-model="item.rarity" type="number"></span>
            <span v-if="item.type == ItemType.Equipment" class="attribute">set id<input class="numberInput" v-model="(item as Equipment).setId" type="number"></span>
            <select v-if="item.type == ItemType.Equipment" class="attribute" v-model="(item as Equipment).piece">
                <option :value = 0>weapon</option>
                <option :value = 1>dress</option>
                <option :value = 2>accessory</option>
            </select>
            <span class="attribute">Img  <input class="imgImput"v-model="item.imgPATH" type="text"></span>
            <img  class="img" :src="`${publicPath}/item-image/${item.imgPATH}`">
        </div>
        
        
        
        <div class="modifiers">
            <div class="row">Modifiers: <button class="smallbutton nnnbutton" @click="AddModifier">Add Modifier</button></div>
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