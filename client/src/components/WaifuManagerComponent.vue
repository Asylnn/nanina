<script lang="ts">


import Waifu from '../classes/waifu';
export default {
    name : "WaifuManagerComponent",
    /*data () {
        return {
            str: 0,
        }
    },*/
    props:{
        waifu : {
            type: Waifu,
            required : true,
        },
    },
    methods:{
        Delete(){
            this.$emit("delete", this.waifu.id)
        }
    },
    computed() {
        str:{
            get() {
                return this.waifu.o_str
            }
        }
    }
}

</script>

<template>
    <div class="WaifuManagerComponent">
        <button @click="Delete">delete</button>
        <span class="attribute"><input class="name" v-model="waifu.name" type="text"></span>
        <span class="attribute">Id  <input class="numberInput" v-model="waifu.id" type="text"></span>
        <span class="attribute">Diff  <input class="numberInput" v-model="waifu.diffLvlUp" type="number"></span>
        <span class="attribute">★  <input class="numberInput" v-model="waifu.stars" type="number"></span>
        
        <span class="attribute">_STR  <input class="numberInput" v-model="waifu.o_str" type="number"></span>
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
        <span class="attribute">+LUCK  <input class="numberInput" v-model="waifu.u_luck" type="number"></span>

        <span class="attribute">Img  <input class="imgImput"v-model="waifu.imgPATH" type="text"></span>
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

button {
    margin-right: 10px;
}


</style>