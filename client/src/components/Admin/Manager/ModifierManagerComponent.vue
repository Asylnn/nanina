<script lang="ts">
import Modifier from '@/classes/modifiers/modifiers';
import OperationType from '@/classes/modifiers/operation_type';


export default {
    name : "ModifierManagerComponent",
    data() {
        return {
            OperationType : OperationType,
        }
    },
    props: {
        modifier: {
            type: Modifier, 
            required: true,
        },
    },
    methods:{

        Delete(){
            this.$emit("delete-modifier", this.modifier.stat)
        },
    },
}


</script>
<template>
    <div>
        <button @click="Delete">delete</button>
        <span class="attribute">Stat  <input class="numberInput" v-model="modifier.stat" type="number"></span>
        <span class="attribute">Timeout  <input class="numberInput" v-model="modifier.timeout" type="number"></span>
        <span class="attribute">Amount <input class="numberInput" v-model="modifier.amount" type="number"></span>
         <select class="attribute" v-model="modifier.operationType">
            <option :value = OperationType.Additive>Additive</option>
            <option :value = OperationType.Multiplicative>Multiplicative</option>
        </select>
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