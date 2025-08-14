<script lang="ts">

import WaifuManagerComponent from './WaifuManagerComponent.vue'
import Waifu from '../../../classes/waifu/waifu';
import ClientResponseType from '@/classes/client_response_type';
import type Dictionary from '@/classes/dictionary';
import type { PropType } from 'vue';

export default {
    name : "WaifuManagerPage",
    props: {
        id : {
            type : String,
            required : true
        },
        all_waifus : {
            type : Object as PropType<Dictionary<Waifu>>,
            required : true
        },
    },
    components: {
        WaifuManagerComponent
    },
    methods:{
        AddNewWaifu(){
            Array<Waifu>
            let waifu = new Waifu({})
            let keys = Object.keys(this.all_waifus).sort((a, b) => +a - +b)
            waifu.id = (+keys.slice(-1)[0] +1).toString() || "1" //Don't worry too much about it
            this.all_waifus[waifu.id] = waifu
        },
        UpdateWaifus(){
            for(let waifu of Object.values(this.all_waifus)){
                waifu.b_agi = waifu.o_agi
                waifu.b_luck = waifu.o_luck
                waifu.b_str = waifu.o_str
                waifu.b_int = waifu.o_int
                waifu.b_kaw = waifu.o_kaw
                waifu.b_dex = waifu.o_dex
            }
            this.SendToServer(ClientResponseType.UpdateWaifuDB, JSON.stringify(this.all_waifus), this.id)
        },
        DeleteWaifu(id : string){
            delete this.all_waifus[id]
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
