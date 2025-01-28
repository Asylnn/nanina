<script lang="ts">
import Equipement from '@/classes/inventory/equipement';
import ModifierComponent from './ModifierComponent.vue';


export default {
    name : "ItemManagerPage",
    props:{
        item : Equipement,
        required : true
    },
    methods:{
    },
    components:{
        ModifierComponent
    }
}


</script>
<template>
    <div>
        <button @click="Delete">delete</button>
                
        <span class="attribute">Id  <input class="numberInput" v-model="item.id" type="text"></span>
        <span class="attribute">Name <input class="name" v-model="item.name" type="text"></span>
        <span class="attribute">Set  <input class="name" v-model="item.imgPATH" type="text"></span>

        <span class="attribute">Desc <input class="descritption" v-model="item.description" type="text"></span>
        <span class="attribute">Img  <input class="imgImput"v-model="item.imgPATH" type="text"></span>
        <button @click="AddModifier">delete</button>
        <div v-for="modifier in item.modifiers">
            <ModifierComponent></ModifierComponent>
        </div>
        <button @click="AddSetModifier">delete</button>
        <div v-for="modifier in item.waifu_modifiers">
            <ModifierComponent></ModifierComponent>
        </div>
    </div>


</template>

<style lang="css" scoped>
.InventoryHeader{
    align-content: center;
}

img {
    width: 100%;
    height: 100%;
}

#InventoryPages {
    display:grid;
    cursor: pointer;
    text-align: center;
    grid-template-columns: 1fr 1fr 1fr 1fr ;
}


</style>