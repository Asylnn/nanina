<script lang="ts">

import WaifuManagerComponent from '../WaifuManagerComponent.vue'
import Waifu from '../../classes/waifu/waifu';
import User from '@/classes/user/user';
export default {
    name : "UserWaifuManagerPage.vue",
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
        AddNewWaifu(){
            this.user.waifus.push(new Waifu({}))
        },
        UpdateWaifus(){
            this.SendToServer("update user waifu", JSON.stringify(this.user.waifus), this.user.Id)
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
            <WaifuManagerComponent :editing-existing-waifu="false" :waifu="waifu" @delete="DeleteWaifu"></WaifuManagerComponent>
        </li>
        <button @click="AddNewWaifu">Add new waifu</button><br>
        <button @click="UpdateWaifus">Update</button>
    </div>
</template>