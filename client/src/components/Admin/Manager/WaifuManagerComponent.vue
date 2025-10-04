<script lang="ts">


import Waifu from '../../../classes/waifu/waifu';
export default {
    name : "WaifuManagerComponent",
    props:{
        waifu : {
            type: Waifu,
            required : true,
        },
        editingExistingWaifu : {
            type:Boolean,
            default:true
        }
    },
    methods:{
        Delete(){
            this.$emit("delete", this.waifu.id)
        },
        Reset()
        {
            this.waifu.b_str = this.waifu.o_str + (this.waifu.lvl-1)*this.waifu.u_str
            this.waifu.b_kaw = this.waifu.o_kaw + (this.waifu.lvl-1)*this.waifu.u_kaw
            this.waifu.b_dex = this.waifu.o_dex + (this.waifu.lvl-1)*this.waifu.u_dex
            this.waifu.b_agi = this.waifu.o_agi + (this.waifu.lvl-1)*this.waifu.u_agi
            this.waifu.b_int = this.waifu.o_int + (this.waifu.lvl-1)*this.waifu.u_int
            this.waifu.b_luck = this.waifu.o_luck + (this.waifu.lvl-1)*this.waifu.u_luck
        }
    },
}

</script>

<template>
    <div class="WaifuManagerComponent">
        <button class="wmcButton" @click="Delete">delete</button>
        <button class="wmcButton" v-if="editingExistingWaifu" @click="Reset">snap stats to level</button>
        <span class="attribute">Id  <input class="numberInput" v-model="waifu.id" type="text"></span>
        <span class="attribute">★  <input class="numberInput" v-model="waifu.stars" type="number"></span>


        <span v-if="!editingExistingWaifu">
            <span class="attribute">_STR  <input class="numberInput" v-model="waifu.o_str" type="number"></span>
            <span class="attribute">+STR  <input class="numberInput" v-model="waifu.u_str" type="number"></span>
            <span class="attribute">_KAW  <input class="numberInput" v-model="waifu.o_kaw" type="number"></span>
            <span class="attribute">+KAW  <input class="numberInput" v-model="waifu.u_kaw" type="number"></span>
            <span class="attribute">_INT  <input class="numberInput" v-model="waifu.o_int" type="number"></span>
            <span class="attribute">+INT  <input class="numberInput" v-model="waifu.u_int" type="number"></span>
            <span class="attribute">_AGI  <input class="numberInput" v-model="waifu.o_agi" type="number"></span>
            <span class="attribute">+AGI  <input class="numberInput" v-model="waifu.u_agi" type="number"></span>
            <span class="attribute">_DEX  <input class="numberInput" v-model="waifu.o_dex" type="number"></span>
            <span class="attribute">+DEX  <input class="numberInput" v-model="waifu.u_dex" type="number"></span>
            <span class="attribute">_LUCK  <input class="numberInput" v-model="waifu.o_luck" type="number"></span>
            <span class="attribute">+LUCK  <input class="numberInput" v-model="waifu.u_luck" type="number"></span>
        </span>

        <span v-else>
            <span class="attribute">LVL  <input class="numberInput" v-model="waifu.lvl" type="number"></span>
            <span class="attribute">XP  <input class="numberInput" v-model="waifu.xp" type="number"></span>
            <span class="attribute">STR  <input class="numberInput" v-model="waifu.b_str" type="number"></span>
            <span class="attribute">KAW  <input class="numberInput" v-model="waifu.b_kaw" type="number"></span>
            <span class="attribute">INT  <input class="numberInput" v-model="waifu.b_int" type="number"></span>
            <span class="attribute">AGI  <input class="numberInput" v-model="waifu.b_agi" type="number"></span>
            <span class="attribute">DEX  <input class="numberInput" v-model="waifu.b_dex" type="number"></span>
            <span class="attribute">LUCK  <input class="numberInput" v-model="waifu.b_luck" type="number"></span>
        </span>
        

        

        <span class="attribute">{{ waifu.points }}p</span>
        <span class="attribute">Img  <input class="imgImput" v-model="waifu.imgPATH" type="text"></span>
        <span class="attribute">AuthorURL  <input class="imgImput" v-model="waifu.imgAuthorURL" type="text"></span>
    </div>
</template>


<style>

.WaifuManagerComponent {
    margin-left: 1vw;
    margin-top: 1vh;
    margin-bottom: 1vh;
    margin-right: 1vw;
    /*border: 3px;
    border-style: solid;
    border-color:blueviolet;*/
    /*padding:3px*/

}

.attribute {
    border: 3px;
    border-style: solid;
    border-color:blueviolet;
    padding:2px;
    
}

.numberInput {
    width: 30px;
}

.imgImput {
    width: 350px;
}

.name {
    width: 200px;
}

.wmcButton {
    margin-right: 10px;
}


</style>