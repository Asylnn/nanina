<script lang="ts">
import Modifier from '@/classes/modifiers/modifiers';


export default {
    name : "ModifierManagerComponent",
    props: {
        modifier: {
            type: Modifier, 
            required: true
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
        <span class="attribute ">Amount <input class="numberInput" v-model="modifier.amount" type="number"></span>
    </div>
    <!--<span class="attribute">_STR  <input class="numberInput" v-model="waifu.o_str" type="number"></span>
    <span class="attribute">+STR  <input class="numberInput" v-model="waifu.u_str" type="number"></span>
    <span class="attribute">_AGI  <input class="numberInput" v-model="waifu.o_agi" type="number"></span>
    <span class="attribute">+AGI  <input class="numberInput" v-model="waifu.u_agi" type="number"></span>
    <span class="attribute">_KAW  <input class="numberInput" v-model="waifu.o_kaw" type="number"></span>
    <span class="attribute">+KAW  <input class="numberInput" v-model="waifu.u_kaw" type="number"></span>
    <span class="attribute">_DEX  <input class="numberInput" v-model="waifu.o_dex" type="number"></span>
    <span class="attribute">+DEX  <input class="numberInput" v-model="waifu.u_dex" type="number"></span>
    <span class="attribute">_INT  <input class="numberInput" v-model="waifu.o_int" type="number"></span>
    <span class="attribute">+INT  <input class="numberInput" v-model="waifu.u_int" type="number"></span>
    <span class="attribute">_LUCK  <input class="numberInput" v-model="waifu.o_luck" type="number"></span>
    <span class="attribute">+LUCK  <input class="numberInput" v-model="waifu.u_luck" type="number"></span>-->

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