<script lang="ts">

import OsuBeatmapset from '@/classes/beatmapset';
import config from '../../../config.json'

import OsuBeatmap from '@/classes/beatmap';
import User from '@/classes/user';
import WaifuGridDisplayComponent from './WaifuGridDisplayComponent.vue';
import Waifu from '@/classes/waifu';



/*
Nom de la map
Lien
Background image de la map

Filters to add : per star rating
per mode (std, taiko, mania, ctb)

*/
export default {
    name : "ClaimAndFightPage",
    data() {
        return {
            date_milli: Date.now(),
            fight_timing_out: false,
            claim_timing_out: false,
            config: config, //You have to do this to access config inside html code
            chosen_waifu : new Waifu({}),
        }
    },
    props: {
        fighting: {
            type : Boolean,
            required : true,
        },
        user : { //user.Id
            type : User,
            required : true,
        },
        xp : {
            type : Number, //0 until you click claim xp
            required : true
        },
        beatmap : { //Beatmap with beatmapset inside
            type : OsuBeatmap,
            required : true
        }
    },
    mounted(){
        
        if(this.user.fights.length != 0){
            if(!this.user.fights[this.user.fights.length - 1].completed){
                //this.fighting = true
                //@ts-ignore
			    this.ws.send(JSON.stringify({type:"get map back", data:this.user.fights[this.user.fights.length - 1].id, id: this.user.Id}))
            }
            if(this.user.fights[this.user.fights.length - 1].timestamp + config.time_for_allowing_another_fight_in_milliseconds >= this.date_milli ){
                this.fight_timing_out = true
                this.user.localFightTimestamp = this.user.fights[this.user.fights.length - 1].timestamp 
            }
        }
        if(this.user.localFightTimestamp + config.time_for_allowing_another_fight_in_milliseconds >= this.date_milli){
            this.fight_timing_out = true
        }

        if( this.user.claimTimestamp + config.time_for_allowing_another_claim_in_milliseconds >= this.date_milli ){
            this.claim_timing_out = true
        }
        setInterval(this.updateTimer, 1000)
    },
    
    methods: {
        updateTimer() {

            this.date_milli = Date.now()
            console.log("local timestamp " + this.user.localFightTimestamp)
            console.log("local config " + config.time_for_allowing_another_fight_in_milliseconds)
            console.log("local date " + this.date_milli)
            if(this.user.localFightTimestamp + config.time_for_allowing_another_fight_in_milliseconds < this.date_milli){
                this.fight_timing_out = false
            }
            if(this.user.claimTimestamp + config.time_for_allowing_another_claim_in_milliseconds < this.date_milli){
                this.claim_timing_out = false
            }
        },
        fight(){
            this.user.localFightTimestamp = Date.now() 
            this.fight_timing_out = true
            this.updateTimer()
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"get map to fight", data:0, id: this.user.Id}))
        },
        getXP(){
            this.user.claimTimestamp = Date.now()
            this.claim_timing_out = true
            this.updateTimer()
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"claim fight", data:0, id: this.user.Id}))
        },
        selectWaifu(waifu : Waifu) {
            this.chosen_waifu = waifu
        },
        resetWaifu() {
            this.chosen_waifu = new Waifu({})
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
        <span id="timerFight" v-if="fight_timing_out">
            Wait {{ Math.round((user.localFightTimestamp + config.time_for_allowing_another_fight_in_milliseconds - date_milli)/60000*60)  }} seconds
        </span>
        <span id="fightButton" v-else @click="fight">Fight !</span>
        <div id="inFight" v-if="fighting">
            <img :href="beatmap.beatmapset.covers.cover2x" :src="beatmap.beatmapset.covers.cover2x">
            Mouhahahahhaha !<br>
            I am the spirit of the map, prove me your worth by :<br>
            <p>Downloading me</p> <br>
            <span id="download">
                <a :href="beatmap.url+'#'+beatmap.mode+'/'+beatmap.id" target="_blank">
                    Download on the osu! website !<br>(not a virus)
                </a>
            </span><br>
            and<br>
            <p>Playing me by submitting a score</p><br>
            If you manage to submit a score, I will gift you XP !<br>
            Select which waifu would recieve rewards if you are worthy !
            <WaifuGridDisplayComponent @show-waifu="selectWaifu" :waifus="user.waifus" :columns="5"></WaifuGridDisplayComponent>
            <div v-if="chosen_waifu != undefined">
                <WaifuGridDisplayComponent @show-waifu="resetWaifu" :waifus="chosen_waifu" :columns="1"></WaifuGridDisplayComponent>
            </div>
            <span id="timerClaim" v-if="claim_timing_out">
                Wait {{ Math.round((user.claimTimestamp + config.time_for_allowing_another_claim_in_milliseconds - date_milli)/60000*60)  }} seconds
            </span>
            <span v-else id="claim" @click="getXP">Prove that you are worth<br>getting XP!</span>
        </div>
        <div v-else-if="xp != 0">
            <p> You earned {{ xp }}XP on {{chosen_waifu.name}}! </p>
        </div>
    </div>
</template>

<style lang="css" scoped>
#windowFight, #inFight {
    display: grid;
}
#windowFight {
    margin: 0 25vw;
    text-align: left;
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
#fightButton, #timerFight, #timerClaim{
    margin: 1vh 15vw;
    text-align: center;
}
#download, #claim {
    margin: 1vh 10vw;
}
#timerFight, #timerClaim {
    cursor:default;
}
#welcomeText {
    line-height: 1.5;
}
#inFight p {
    color: red;
}
</style>