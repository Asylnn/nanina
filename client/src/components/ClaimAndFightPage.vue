<script lang="ts">

import OsuBeatmapset from '@/classes/beatmapset';
import type WebSocketReponse from '../classes/web_socket_response'
import {
	Websocket,
} from "websocket-ts"
import OsuBeatmap from '@/classes/beatmap';



/*
Nom de la map
Lien
Background image de la map

Filters to add : per star rating
per mode (std, taiko, mania, ctb)

*/
export default {
    name : "ClaimAndFightPage",
    data() {},
    props: {
        id : { //user.Id
            type : String,
            required : true,
        },
        fighting : { //Is in fight?
            type : Boolean,
            required : true,
        },
        xp : {
            type : Number, //0 until you click claim xp
            required : true
        },
        beatmapset : {
            type : OsuBeatmapset,
            required : false
        },
        beatmap : {
            type : OsuBeatmap,
            required : true
        }
    },
    methods: {
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
            <a :href="'https://osu.ppy.sh/beatmapsets/'+beatmap.beatmapset_id+'#'+beatmap.mode+'/'+beatmap.id" target="_blank">Fight this map!</a>
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