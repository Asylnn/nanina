<script lang="ts">

import WaifuManagerComponent from './WaifuManagerComponent.vue'
import Waifu from '../classes/waifu/waifu';
import User from '@/classes/user/user';

export default {
    name : "UserWaifuManagerPage",
    props: {
        user : {
            type : User,
            required : true
        },
    },
    components: {
        WaifuManagerComponent
    },

    methods:{
        UpdateWaifus(){
            this.SendToServer("update user waifus", JSON.stringify(this.user.waifus), this.user.Id)
        },
        DeleteWaifu(id : string){
            this.user.waifus.splice(this.user.waifus.findIndex(waifu => waifu.id == id), 1)
        }
    }
}

</script>

<template>
    <div>
        <li v-for="waifu in user.waifus">
            <WaifuManagerComponent :waifu="waifu" @delete="DeleteWaifu"></WaifuManagerComponent>
        </li>
        <button @click="UpdateWaifus">Update</button>
    </div>
</template>