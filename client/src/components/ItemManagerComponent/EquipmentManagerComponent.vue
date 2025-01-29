<script lang="ts">
import Equipement from '@/classes/inventory/equipment';
import ModifierComponent from './ModifierManagerComponent.vue';
import WaifuModifier from '@/classes/inventory/modifiers/waifu_modifier';


export default {
    name : "EquipmentManagerComponent",
    props: {
        item: {
            type: Equipement,
            required: true
        }
    },
    methods:{
        AddModifier(){
            this.item.modifiers.push(new WaifuModifier())
        },
        AddSetModifier(){
            this.item.setModifiers.push(new WaifuModifier())
        },
        DeleteModifier(id:number){
            this.item.modifiers.splice(this.item.modifiers.findIndex(modf => modf.id == id), 1)
        },
        DeleteSetModifier(id:number){
            this.item.setModifiers.splice(this.item.setModifiers.findIndex(modf => modf.id == id), 1)
        },
        Delete(){
            this.$emit("delete-item", this.item.id)
        },
    },
    components:{
        ModifierComponent
    }
}


</script>
<template>
    <div>
        <div class="row">
            <button @click="Delete">delete</button>
                
            <span class="attribute">Id  <input class="numberInput" v-model="item.id" type="number"></span>
            <span class="attribute">Set Id<input class="numberInput" v-model="item.set_id" type="number"></span>

            <span class="attribute">Name <input class="name" v-model="item.name" type="text"></span>
            <span class="attribute">Set  <input class="name" v-model="item.set_name" type="text"></span>
            <span class="attribute">Desc <input class="description" v-model="item.description" type="text"></span>
            <span class="attribute">Img  <input class="imgImput"v-model="item.imgPATH" type="text"></span>
            <img  class="img" :src="'src/assets/item-image/' + item.imgPATH">
            <select class="attribute" v-model="item.piece">
                <option :value = 0>Weapon</option>
                <option :value = 1>Dress</option>
                <option :value = 2>Accessory</option>
            </select>
        </div>
        
        
        
        <div class="modifiers">
            <div class="row">Modifiers: <button @click="AddModifier">Add Modifier</button></div>
            <div v-for="modifier in item.modifiers">
                <ModifierComponent class="row" :modifier="modifier" @delete-modifier="DeleteModifier"></ModifierComponent>
            </div>
            <div class="row">Set Modifiers: <button @click="AddSetModifier">Add Set Modifier</button></div>
            <div v-for="modifier in item.setModifiers">
                <ModifierComponent class="row" :modifier="modifier" @delete-modifier="DeleteSetModifier"></ModifierComponent>
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
    grid-template-columns: 1fr 1fr 1fr 1fr ;
}


</style>