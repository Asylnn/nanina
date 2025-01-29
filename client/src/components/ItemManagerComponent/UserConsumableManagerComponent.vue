<script lang="ts">
import UserModifier from '@/classes/inventory/modifiers/user_modifier';
import ModifierComponent from './ModifierManagerComponent.vue';
import WaifuModifier from '@/classes/inventory/modifiers/waifu_modifier';
import UserConsumable from '@/classes/inventory/user_consumable';


export default {
    name : "UserConsumableManagerComponent",
    props: {
        item: {
            type: UserConsumable,
            required: true
        }
    },
    methods:{
        AddModifier(){
            this.item.modifiers.push(new UserModifier())
        },
        DeleteModifier(id:number){
            this.item.modifiers.splice(this.item.modifiers.findIndex(modf => modf.id == id), 1)
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
            <span class="attribute">Name <input class="name" v-model="item.name" type="text"></span>
            <span class="attribute">Desc <input class="description" v-model="item.description" type="text"></span>
            <span class="attribute">Img  <input class="imgImput"v-model="item.imgPATH" type="text"></span>
            <img  class="img" :src="'src/assets/item-image/' + item.imgPATH">
        </div>
        
        
        
        <div class="modifiers">
            <div class="row">Modifiers: <button @click="AddModifier">Add Modifier</button></div>
            <div v-for="modifier in item.modifiers">
                <ModifierComponent class="row" :modifier="modifier" @delete-modifier="DeleteModifier"></ModifierComponent>
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