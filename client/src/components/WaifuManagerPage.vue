<script lang="ts">

import WaifuManagerComponent from './WaifuManagerComponent.vue'
import Waifu from '../classes/waifu';
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
            //@ts-ignore
            this.ws.send(JSON.stringify({type:"update waifu db", data:JSON.stringify(this.all_waifus), id: this.id}))
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
            <WaifuManagerComponent :waifu="waifu" @delete="DeleteWaifu"></WaifuManagerComponent>
        </li>
        <button @click="AddNewWaifu">Add new waifu</button><br>
        <button @click="UpdateWaifus">Update</button>
    </div>
</template>
