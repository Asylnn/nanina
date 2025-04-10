<script lang="ts">

import WaifuManagerComponent from './WaifuManagerComponent.vue'
import Waifu from '../classes/waifu/waifu';
export default {
    name : "WaifuManagerPage",
    props: {
        id : {
            type : String,
            required : true
        },
        all_waifus : {
            type : Array<Waifu>,
            required : true
        },
    },
    components: {
        WaifuManagerComponent
    },
    methods:{
        AddNewWaifu(){
            let waifu = new Waifu({})
            waifu.id = this.all_waifus.length == 0 ? waifu.id = "1" : (+this.all_waifus[this.all_waifus.length - 1].id +1).toString() //Don't worry too much about it
            this.all_waifus.push(waifu)
        },
        UpdateWaifus(){
            
            let updated_waifus = this.all_waifus.map(waifu => {
                waifu.b_agi = waifu.o_agi
                waifu.b_luck = waifu.o_luck
                waifu.b_str = waifu.o_str
                waifu.b_int = waifu.o_int
                waifu.b_kaw = waifu.o_kaw
                waifu.b_dex = waifu.o_dex
                return waifu;
            })
            this.SendToServer("update waifu db", JSON.stringify(updated_waifus), this.id)
        },
        DeleteWaifu(id : string){
            this.all_waifus.splice(this.all_waifus.findIndex(waifu => waifu.id == id), 1)
        }
    }
    
}

</script>

<template>
    <div>
        <li v-for="waifu in all_waifus">
            <WaifuManagerComponent :editing-existing-waifu="false" :waifu="waifu" @delete="DeleteWaifu"></WaifuManagerComponent>
        </li>
        <button @click="AddNewWaifu">Add new waifu</button><br>
        <button @click="UpdateWaifus">Update</button>
    </div>
</template>
