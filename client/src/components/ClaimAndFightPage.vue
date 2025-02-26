<script lang="ts">

import OsuBeatmapset from '@/classes/beatmapset';
import config from '../../../baseValues.json'

import OsuBeatmap from '@/classes/beatmap';
import User from '@/classes/user/user';
import Waifu from '@/classes/waifu/waifu';
import GridDisplayComponent from './GridDisplayComponent.vue';
import Chart from '@/classes/maimai/chart';
import Game from '@/classes/user/game';



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
            chosen_waifu : null as Waifu | null,
            game : this.user.preferedGame,
            Game: Game,
            
        }
    },
    props: {
        maimai_chart: {
            type: [Chart, null],
            required:true,
        },
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
        
        if(this.user.fight.id != null){
            if(!this.user.fight.completed){
                //this.fighting = true
                this.SendToServer("get map back", this.user.fight.id, this.user.Id)
            }
            if(this.user.fight.timestamp + config.time_for_allowing_another_fight_in_milliseconds >= this.date_milli ){
                this.fight_timing_out = true
                this.user.localFightTimestamp = this.user.fight.timestamp 
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
            this.SendToServer("get map to fight", this.game.toString(), this.user.Id)
        },
        getXP(){
            if(this.chosen_waifu != null){ //This shouldn't happen?
                this.user.claimTimestamp = Date.now()
                this.claim_timing_out = true
                this.updateTimer()
                this.SendToServer("claim fight", JSON.stringify({id:this.chosen_waifu.id, game:this.game}), this.user.Id)
            }
        },
        selectWaifu(waifu : Waifu) {
            this.chosen_waifu = waifu
        },
        resetWaifu() {
            this.chosen_waifu = null
        }
    },
    components: {
        GridDisplayComponent,
    },
    computed:{
        mapURL(){
            return `${this.beatmap.url}#${this.beatmap.mode}/${this.beatmap.id}`
        }
    }
}


</script>


<template>
    
        <div class="flex" id="windowFight">
            <div class="grid" id="gameSelector">
                <span >{{ $t("fight.game_select") }}</span>
                <select v-model="game">
                    <option :value="Game.OsuStandard">{{ $t("games.osu_standard") }}</option>
                    <option :value="Game.MaimaiFinale">{{ $t("games.maimai_finale") }}</option>
                </select>
            </div>
            <p id="welcomeText">
                {{ $t("fight.welcome") }}<br>
                In this section you can gain some juicy xp for your favourite waifus !<br>
                Do you think you have what it takes to succeed ?<br>
                It's time to find out !
            </p>
            <span class="button timer" v-if="fight_timing_out">
                Wait {{ Math.round((user.localFightTimestamp + config.time_for_allowing_another_fight_in_milliseconds - date_milli)/60000*60)  }} seconds
            </span>
            <span class="button" v-else @click="fight">Fight !</span>
            <div class="flex" v-if="game == Game.OsuStandard">
                <div class="flex" v-if="fighting">
                    <a :href="mapURL"> <img id="bgMap" :src="beatmap.beatmapset.covers.cover2x"></a>
                    <p>
                        Mouhahahahhaha !<br>
                        I am the spirit of the map, prove me your worth by :<br>
                        <span class="inRed">Downloading me</span> <br>
                    </p>
                    
                    <span class="button">
                        <a :href="mapURL" target="_blank">
                            Download on the osu! website !
                        </a>
                    </span><br>
                    <p class="inRed" id="claimText">And playing me by submitting a score</p><br>
                </div>
            </div>
            <div v-else-if="game == Game.MaimaiFinale">
                <div v-if="fighting">
                    <p>Play {{ maimai_chart?.title }} / {{ maimai_chart?.difficulty }}</p>
                </div>
            </div>
            <div v-if="fighting">
                <p>If you manage to submit a score, I will gift you XP !<br>
                    Select which waifu are worthy of earning XP</p>
                <GridDisplayComponent class="waifuFightSelector" :no-margin="true" v-if="chosen_waifu == null" @show-element="selectWaifu" :elements="user.waifus" :columns="3"></GridDisplayComponent>
                <div class="flex" v-if="chosen_waifu != null">
                    <div id="selectedWaifu">
                        <GridDisplayComponent :no-margin="true" @show-element="resetWaifu" :elements="[chosen_waifu]" :columns="1"></GridDisplayComponent>
                    </div>
                    <span class="button timer" v-if="claim_timing_out">
                        Wait {{ Math.round((user.claimTimestamp + config.time_for_allowing_another_claim_in_milliseconds - date_milli)/60000*60)  }} seconds
                    </span>
                    <span class="button" v-else @click="getXP">Prove that you are worth getting XP !</span><br>
                </div>
            </div>
            
        </div>
    
</template>

<style lang="css" scoped>

.grid
{
    display:grid;
}

#gameSelector
{
    grid-template-columns: 1fr 0.5fr; 
    margin-bottom: 20px;
}

.flex
{
    display:flex;
    flex-direction: column;

}

#windowFight {
    
    margin: 0 25vw;
    font-size: larger;
}

.button{
    margin: 20px 0;
    color: rgb(203, 165, 221);
    font-size: xx-large;
    border-radius: 25px;
    background-color: rgb(39, 11, 65);
    cursor: pointer;
    padding: 2px 40px;
    text-align: center;
    align-self: center;
}

.button a 
{
    color: rgb(203, 165, 221);
}


/*#fightButton, #timerFight, #timerClaim{
    text-align: center;
    max-width: 20vw;
}*/

.timer {
    cursor:not-allowed;
}
                    
                    
* {
    line-height: 1.5;
}

.inRed {
    color: red;
}
#bgMap {
    width: 50vw;
    cursor:pointer;
}

.waifuFightSelector
{
    margin: 10px 2vw;
}

#selectedWaifu
{
    margin-top: 20px;
    margin-bottom: 20px;
    align-self: center;
}

</style>