<script lang="ts">


import Waifu from '../classes/waifu';
export default {
    name : "WaifuManagerComponent",
    props:{
        waifu : {
            type: Waifu,
            required : true,
        }
    },
    methods:{
        Delete(){
            this.$emit("delete", this.waifu.id)
        }
    }
}

</script>

<template>
    <div class="WaifuManagerComponent">
            <button @click="Delete">delete</button>
            <span class="attribute">{{waifu.name}}</span>
            <span class="attribute">Id  <input v-model="waifu.id" type="text"></span>
            <span class="attribute">Diff  <input v-model="waifu.diffLvlUp" type="number"></span>
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

input {
    width: 40px;
}

.imgImput {
    width: 350px;
}

button {
    margin-right: 10px;
}


</style>