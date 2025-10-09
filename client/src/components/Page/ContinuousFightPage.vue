<script lang="ts">
import User from '@/classes/user/user';
import ClientResponseType from '@/classes/client_response_type';
import { WebsocketEvent, type Websocket } from 'websocket-ts';
import type WebSocketResponse from '@/classes/web_socket_response';
import ServerResponseType from '@/classes/server_response_type';
import type MaimaiScoreDTO  from '@/classes/maimai/scoreDTO';
import type OsuScoreDTO  from '@/classes/osu/scoreDTO';
import FightResultComponent from '../Component/FightResultComponent.vue';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';
import config from '../../../../baseValues.json'
import Game from '@/classes/user/game';
import MaimaiFightResultComponent from '../Component/MaimaiFightResultComponent.vue';
import OsuFightResultComponent from '../Component/OsuFightResultComponent.vue';

export default {
    name : "ContinuousFightPage",
    data(){
        return {
            //ActivityType: ActivityType,
            //publicPath : import.meta.env.BASE_URL,
            date_milli:0,
            game : this.user.preferedGame,
            Game : Game,
            scores : [] as OsuScoreDTO[] | MaimaiScoreDTO[],
            listener : (() => {}) as (i: Websocket, ev: MessageEvent) => void,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        },
    },
    methods:{
        CheckContinuousFight()
        {
            this.SendToServer(ClientResponseType.CheckContinuousFight, this.game.toString(), this.user.Id)
        },
        fightWaitTime()
        {
            return MillisecondsToHourMinuteSecondFormat(this.user.lastContinuousFightTimestamp + config.time_for_allowing_another_continuous_fight_in_milliseconds - this.date_milli)
        },
        updateActivities()
        {
            let activeActivities = this.user.activities.filter(acitivity => !acitivity.finished);
            let totalTimeSave = this.scores.reduce((timesave, score) => timesave + score.timesave!, 0) * 1000;
            totalTimeSave /= activeActivities.length;
            for(let activity of activeActivities)
            {
                activity.timeout -= totalTimeSave;
            }
        }
    },
    components:{
        MaimaiFightResultComponent,
        OsuFightResultComponent,
    },
    computed:{
        timeout()
        {
            return this.user.lastContinuousFightTimestamp + config.time_for_allowing_another_continuous_fight_in_milliseconds >= this.date_milli
        }
    },
    mounted(){
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated
        this.listener = (i: Websocket, ev: MessageEvent) => {
			var res : WebSocketResponse = JSON.parse(ev.data)
            console.log(res)
			switch (res.type) {
				case ServerResponseType.ProvideContinuousFightResultsOsu:
                case ServerResponseType.ProvideContinuousFightResultsMaimai:
                    /*switch(res.type)
                    {
                        case ServerResponseType.ProvideContinuousFightResultsOsu:
                            this.game = Game.OsuStandard
                            break
                        case ServerResponseType.ProvideContinuousFightResultsOsu:
                            this.game = Game.MaimaiFinale
                            break
                    }*/
                    this.user.lastContinuousFightTimestamp = Date.now()
                    this.scores = JSON.parse(res.data)
                    this.updateActivities()
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
    }
}


</script>
<template>
    
    <div class="margins">
        <p>{{ $t("activities.fight.overview") }}</p>
        <div class="grid" id="gameSelector">
            <div class = "line">
                <span >{{ $t("fight.game_select") }}</span>
                <select v-model="game">
                    <option :value="Game.OsuStandard">{{ $t("games.osu_standard") }}</option>
                    <option :value="Game.MaimaiFinale">{{ $t("games.maimai_finale") }}</option>
                </select>
            </div>
            
        </div>
        <button v-if="timeout" class="nnnbutton smallbutton" @click="CheckContinuousFight()">{{ $t("activities.fight.timeout")}}  {{ fightWaitTime() }}</button>
        <button v-else class="nnnbutton smallbutton" @click="CheckContinuousFight()">{{ $t("activities.fight.button") }}</button>
        <div v-for="score in scores">
            <MaimaiFightResultComponent v-if="game == Game.MaimaiFinale" :score="score as MaimaiScoreDTO"></MaimaiFightResultComponent>
            <OsuFightResultComponent v-if="game == Game.OsuStandard" :score="score as OsuScoreDTO"></OsuFightResultComponent>
        </div>
    </div>

</template>

<style lang="css" scoped>


.line
{
    display: grid;
    grid-template-columns: 1fr 0.5fr;
}
</style>