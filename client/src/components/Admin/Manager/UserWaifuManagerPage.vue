<script lang="ts">

import WaifuManagerComponent from './WaifuManagerComponent.vue'
import User from '@/classes/user/user';
import ClientResponseType from '@/classes/client_response_type';

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
            this.SendToServer(ClientResponseType.UpdateUserWaifu, JSON.stringify(this.user.waifus), this.user.Id)
        },
        DeleteWaifu(id : string){
            delete this.user.waifus[id]
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