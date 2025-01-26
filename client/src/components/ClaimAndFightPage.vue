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
    <div id="windowFight">
        <p id="welcomeText">
            Welcome to the Fighting section !<br>
            In this section you can gain some juicy xp for your favourite waifus !<br>
            Do you think you have what it takes to succeed ?<br>
            It's time to find out !
        </p>
        <span id="fightButton" @click="fight">Fight !</span>
        <div id="inFight" v-if="fighting">
            Mouhahahahhaha !<br>
            I am the spirit of the map, prove me your worth by :<br>
            <p>Downloading me</p> <br>
            <span id="download">
                <a :href="'https://osu.ppy.sh/beatmapsets/'+beatmap.beatmapset_id+'#'+beatmap.mode+'/'+beatmap.id" target="_blank">
                    Download on the osu! website !<br>(not a virus)
                </a>
            </span><br>
            and<br>
            <p>Playing me by submitting a score</p><br>
            If you manage to submit a score, I will gift you XP !
            <span id="claim" @click="getXP">Prove that you are worth<br>getting XP!</span>
        </div>
        <div v-else-if="xp != 0">
            <p> You earned {{ xp }}XP! </p>
        </div>
    </div>
</template>

<style lang="css" scoped>
#windowFight, #inFight {
    display: grid;
}
#windowFight {
    margin: 0 25vw;
    text-align: center;
    font-size: larger;
}
span, a {
    color: rgb(0, 78, 33);
    font-size: xx-large;
    cursor: pointer;
    border-radius: 25px;
    background-color: palevioletred;
}
a {
    text-decoration: none;
}
#fightButton {
    margin: 1vh 15vw;
}
#download, #claim {
    margin: 1vh 10vw;
}
#welcomeText {
    line-height: 1.5;
}
#inFight p {
    color: red;
}
</style>