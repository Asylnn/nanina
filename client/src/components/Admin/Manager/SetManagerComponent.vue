<script lang="ts">
import Modifier from '@/classes/modifiers/modifiers';
import ModifierManagerComponent from './ModifierManagerComponent.vue'
import Set from '@/classes/item/set'

export default {
    name : "SetManagerComponent.vue",
    props: {
        set: {
            type: Set,
            required: true
        }
    },
    methods:{
        AddModifier(){
            this.set.modifiers.push(new Modifier())
        },
        DeleteModifier(stat:number){
            this.set.modifiers.splice(this.set.modifiers.findIndex(modf => modf.stat == stat), 1)
        },
        Delete(){
            this.$emit("delete-item", this.set.id)
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
            <span class="attribute">Id  <input class="numberInput" v-model="set.id" type="number"></span>
        </div>
        <div class="modifiers">
            <div class="row">Modifiers: <button @click="AddModifier">Add Modifier</button></div>
            <div v-for="modifier in set.modifiers">
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