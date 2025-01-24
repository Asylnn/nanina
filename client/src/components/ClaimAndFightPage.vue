<script lang="ts">

import type WebSocketReponse from '../classes/web_socket_response'
import {
	Websocket,
} from "websocket-ts"

export default {
    name : "ClaimAndFightPage",
    data() {},
    props: {
        id : {
            type : String,
            required : true,
        },
        fighting : {
            type : Boolean,
            required : true,
        },
        link : {
            type : String,
            required : true
        },
        xp : {
            type : Number,
            required : true
        }
    },
    emits: ["create-notif"],
    methods :{
        fight(){
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"get map to fight", data:0, id: this.id}))
        },
        getXP(){
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"claim fight", data:0, id: this.id}))
        }
    },
}


</script>


<template>
    <div>
        <button @click="fight">Fight A Map!</button>
        <div v-if="fighting">
            <a :href="link" target="_blank">Fight this map!</a>
            <button @click="getXP">Claim XP!</button>
        </div>
        <div v-else-if="xp != 0">
            <p> You earned {{ xp }}XP! </p>
        </div>
        
    </div>
</template>

<style lang="css" scoped>
img {
    width: 100%;
    height: 100%;
}
</style>