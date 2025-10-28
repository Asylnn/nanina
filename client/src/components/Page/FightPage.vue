<script lang="ts">
import config from '../../../../baseValues.json'
import OsuBeatmap from '@/classes/osu/beatmap';
import User from '@/classes/user/user';
import Waifu from '@/classes/waifu/waifu';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import Chart from '@/classes/maimai/chart';
import Game from '@/classes/user/game';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';
import ClientResponseType from '@/classes/client_response_type';
import { WebsocketEvent, type Websocket } from 'websocket-ts';
import ServerResponseType from '@/classes/server_response_type';
import type WebSocketResponse from '@/classes/web_socket_response';
import type Dictionary from '@/classes/dictionary';


class GameDifficultyInfo
{
    difficultyMin !: number
    difficultyMax !: number
    difficultyStep !: number 
    minDifficultyRange !: number
}

export default {
    name : "ClaimAndFightPage",
    data() {
        return {
            date_milli: Date.now(),
            config: config, //You have to do this to access config inside html code
            chosen_waifu : null as Waifu | null,
            game : this.user.preferedGame,
            Game: Game,
            gameDifficultyInfo : {} as GameDifficultyInfo,
            listener: (() => {}) as (i: Websocket, ev: MessageEvent) => void,
            difficultyMin:2,
            difficultyMax:4,
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
        beatmap : { //Beatmap with beatmapset inside
            type : OsuBeatmap,
            required : true
        }
    },
    methods: {
        fight(){
            this.user.localFightTimestamp = Date.now() 
            User.updateTimer(this.user)
            this.SendToServer(ClientResponseType.StartFight, this.game.toString(), this.user.Id)
        },
        getXP(){
            if(this.chosen_waifu != null){ //This shouldn't happen?
                this.user.claimTimestamp = Date.now()
                User.updateTimer(this.user)
                this.SendToServer(ClientResponseType.ClaimFight, JSON.stringify({waifuId:this.chosen_waifu.id, game:this.game}), this.user.Id)
            }
        },
        selectWaifu(waifu : Waifu) {
            this.chosen_waifu = waifu
        },
        resetWaifu() {
            this.chosen_waifu = null
        },
        checkBounds()
        {
            if(this.difficultyMax > this.gameDifficultyInfo.difficultyMax)
            {
                this.difficultyMax = this.gameDifficultyInfo.difficultyMax
                this.difficultyMin = this.difficultyMax - this.gameDifficultyInfo.minDifficultyRange
            }
            if(this.difficultyMin < this.gameDifficultyInfo.difficultyMin)
            {
                this.difficultyMin = this.gameDifficultyInfo.difficultyMin
                this.difficultyMax = this.difficultyMin + this.gameDifficultyInfo.minDifficultyRange
            }
        },
        checkMax()
        {
            
            if(this.difficultyMin > this.difficultyMax - this.gameDifficultyInfo.minDifficultyRange)
            {
                
                this.difficultyMin = this.difficultyMax - this.gameDifficultyInfo.minDifficultyRange
            }
            this.checkBounds()
        },
        checkMin()
        {
            
            if(this.difficultyMin > this.difficultyMax - this.gameDifficultyInfo.minDifficultyRange)
            {
                
                this.difficultyMax = +this.difficultyMin + this.gameDifficultyInfo.minDifficultyRange
            }
            this.checkBounds()
        },
        changeGame()
        {
            console.log("wwwwwwwooooooooooooooo")

            let gameName = Game[this.game]
            this.gameDifficultyInfo = (config as Dictionary<Object>)[gameName] as GameDifficultyInfo
        },
    },
    components: {
        GridDisplayComponent,
    },
    computed:{
        mapURL(){
            return `${this.beatmap.url}#${this.beatmap.mode}/${this.beatmap.id}`
        },
        fightWaitTime()
        {
            return MillisecondsToHourMinuteSecondFormat(this.user.localFightTimestamp + config.time_for_allowing_another_fight_in_milliseconds - this.date_milli)
        },
        claimWaitTime()
        {
            return MillisecondsToHourMinuteSecondFormat(this.user.claimTimestamp + config.time_for_allowing_another_claim_in_milliseconds - this.date_milli)
        },
        sliderGradient() //https://medium.com/@predragdavidovic10/native-dual-range-slider-html-css-javascript-91e778134816
        {
            let rangeDistance = this.gameDifficultyInfo.difficultyMax - this.gameDifficultyInfo.difficultyMin;
            let fromPosition = this.difficultyMin - this.gameDifficultyInfo.difficultyMin;
            let toPosition = this.difficultyMax - this.gameDifficultyInfo.difficultyMin;
            let sliderColor = "white"
            let rangeColor = "blueviolet"
            
            return `linear-gradient(
            to right,
            ${sliderColor} 0%,
            ${sliderColor} ${(fromPosition)/(rangeDistance)*100}%,
            ${rangeColor} ${((fromPosition)/(rangeDistance))*100}%,
            ${rangeColor} ${(toPosition)/(rangeDistance)*100}%, 
            ${sliderColor} ${(toPosition)/(rangeDistance)*100}%, 
            ${sliderColor} 100%)`
        },
    },
    mounted()
    {
        this.changeGame()

        if(this.user.fight != null){
            if(!this.user.fight.completed){
                if(this.beatmap?.id == undefined)
                    this.SendToServer(ClientResponseType.GetMapData, this.user.fight.id, this.user.Id)
            }
        }
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated

        this.listener = (i: Websocket, ev: MessageEvent) => {
			var res : WebSocketResponse = JSON.parse(ev.data)
			switch (res.type) 
            {
                case ServerResponseType.GiveXPToWaifu:
                    let data : {xp : number, waifuId: string} = JSON.parse(res.data)
                    this.user.waifus[data.waifuId].GiveXP(data.xp)
                    break;
            }
        }
        //@ts-ignore
        this.ws.addEventListener(WebsocketEvent.message, this.listener);
    },
    unmounted()
    {
        //@ts-ignore
        this.ws.removeEventListener(WebsocketEvent.message, this.listener)
    },
}


</script>


<template>
    
        <div class="flex margins">
            <div class="grid" id="gameSelector">
                <span >{{ $t("fight.game_select") }}</span>
                <select v-model="game" v-on:change="changeGame()">
                    <option :value="Game.OsuStandard">{{ $t("games.osu_standard") }}</option>
                    <option :value="Game.MaimaiFinale">{{ $t("games.maimai_finale") }}</option>
                </select>
            </div>
            <p style="text-align: center;"> {{ $t("fight.select_difficulty", {"min":difficultyMin, "max":difficultyMax, minimumFractionDigits: 2}) }}</p>
            <div id="difficulty-input-container">
                <input id="difficulty-input-1" class="difficulty-input" type="range" :step="gameDifficultyInfo.difficultyStep" :min="gameDifficultyInfo.difficultyMin" :max="gameDifficultyInfo.difficultyMax" v-model="difficultyMin" v-on:input="checkMin()"></input>
                <input id="difficulty-input-2" class="difficulty-input" type="range" :step="gameDifficultyInfo.difficultyStep" :min="gameDifficultyInfo.difficultyMin" :max="gameDifficultyInfo.difficultyMax" v-model="difficultyMax" v-on:input="checkMax()"></input>
            </div>
            
            <p id="welcomeText">
                {{ $t("fight.welcome") }}<br>
                {{ $t("fight.explanation") }}<br>
            </p>
            <span class="button nnnbutton timer" v-if="user.fight_timing_out">
                {{ $t("fight.play_wait" , {wait_time: fightWaitTime})}}
            </span>
            <span class="button nnnbutton" v-else @click="fight">{{ $t("fight.play") }}</span>
            <div class="flex" v-if="game == Game.OsuStandard">
                <div class="flex" v-if="fighting">
                    
                    <p> {{$t("fight.play_osustd" , {title: beatmap.beatmapset.title, version: beatmap.version})}}</p>
                    <a :href="mapURL"> <img id="bgMap" :src="beatmap.beatmapset.covers.cover2x"></a>
                    <span class="button nnnbutton">
                        <a :href="mapURL" target="_blank">
                            {{ $t("fight.download_osu_website") }}
                        </a>
                    </span><br>
                    <p class="inRed" id="claimText">{{ $t("fight.submit_score") }}</p><br>
                </div>
            </div>
            <div v-else-if="game == Game.MaimaiFinale">
                <div v-if="fighting">
                    <p>{{$t("fight.play_maimai" , {title: maimai_chart?.title, difficulty: maimai_chart?.difficulty})}}</p>
                </div>
            </div>
            <div v-if="fighting">
                <p>
                    {{ $t("fight.submit_score_explanation") }}<br>
                    {{ $t("fight.select_waifu") }}
                </p>
                <GridDisplayComponent class="waifuFightSelector" :no-margin="true" v-if="chosen_waifu == null" @show-element="selectWaifu" :elements="Object.values(user.waifus)" :columns="3"></GridDisplayComponent>
                <div class="flex" v-if="chosen_waifu != null">
                    <div id="selectedWaifu">
                        <GridDisplayComponent :no-margin="true" @show-element="resetWaifu" :elements="[chosen_waifu]" :columns="1"></GridDisplayComponent>
                    </div>
                    <span class="button nnnbutton timer" v-if="user.claim_timing_out">
                        {{ $t("fight.play_wait" , {wait_time: claimWaitTime})}}
                    </span>
                    <span class="button nnnbutton" v-else @click="getXP">{{ $t("fight.claim_xp") }}</span><br>
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
    max-height: 40vh;
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




/*
#difficulty-input-container p:nth-of-type(1)
{
    margin-left: -1.5%;
}

#difficulty-input-container p
{
    position: absolute;
}
*/

/* For that fucking double range input */

#difficulty-input-container
{
    margin-left: 10%;
    margin-bottom: 40px;
}

.difficulty-input
{
    pointer-events: none;
    position:absolute;
    width: 40%;
    border: none;
    z-index: 0;
}

/* firefox */
.difficulty-input::-moz-range-thumb
{
    border :none;
    pointer-events: auto;
    cursor: pointer;
    background: white;
    width:var(--thumb-width);
    height:var(--thumb-height);
    border:4px blueviolet solid;
    border-radius: 50%;
}

#difficulty-input-1::-moz-range-track
{
    height: var(--track-height);
    background-image: v-bind(sliderGradient);
    border:0px none none;
    border-radius: 100px;
}

/* chromium */
.difficulty-input::-webkit-slider-thumb
{
    appearance: none;
    pointer-events: auto;
    cursor: pointer;
    background-color: #303030;
    border:3px blueviolet solid;
    border-radius: 50%;
    width: var(--thumb-width);
    height: var(--thumb-height);
    margin-top: calc((var(--track-height) / 2) - (var(--thumb-height) / 2));
}

#difficulty-input-1::-webkit-slider-runnable-track
{
    height: var(--track-height);
    background-color: blueviolet;
    border:0px none none;
    border-radius: 100px;
}





</style>