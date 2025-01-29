<script lang="ts">
import ModifierComponent from './ModifierManagerComponent.vue';
import Material from '@/classes/inventory/material';


export default {
    name : "MaterialManagerComponent",
    props: {
        item: {
            type: Material,
            required: true
        }
    },
    methods:{
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