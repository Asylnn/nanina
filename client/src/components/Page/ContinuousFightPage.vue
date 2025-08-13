<script lang="ts">
import User from '@/classes/user/user';
import ClientResponseType from '@/classes/client_response_type';
import { WebsocketEvent, type Websocket } from 'websocket-ts';
import type WebSocketResponse from '@/classes/web_socket_response';
import ServerResponseType from '@/classes/server_response_type';
import type ScoreDTO from '@/classes/osu/scoreDTO';
import FightResultComponent from '../Component/FightResultComponent.vue';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';
import config from '../../../../baseValues.json'

export default {
    name : "ContinuousFightPage",
    data(){
        return {
            //ActivityType: ActivityType,
            //publicPath : import.meta.env.BASE_URL,
            date_milli:0,
            scores : [] as ScoreDTO[],
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
            this.SendToServer(ClientResponseType.CheckContinuousFight, "", this.user.Id)
        },
        fightWaitTime()
        {
            return MillisecondsToHourMinuteSecondFormat(this.user.lastContinuousFightTimestamp + config.time_for_allowing_another_continuous_fight_in_milliseconds - this.date_milli)
        },

    },
    components:{
        FightResultComponent,
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
				case ServerResponseType.ProvideContinuousFightResults:
                    this.user.lastContinuousFightTimestamp = Date.now()
                    this.scores = JSON.parse(res.data)
                    let activeActivities = this.user.activities.filter(acitivity => !acitivity.finished);
                    let totalTimeSave = this.scores.reduce((timesave, score) => timesave + score.timesave!, 0) * 1000;
                    console.log("1", totalTimeSave)
                    totalTimeSave /= activeActivities.length;
                    for(let activity of activeActivities)
                    {
                        console.log("2", totalTimeSave)
                        activity.timeout -= totalTimeSave;
                    }
                        
                    
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
    
    <div>
        <p>{{ $t("activities.fight.explanation") }}</p>
        <button v-if="timeout" class="nnnbutton smallbutton" @click="CheckContinuousFight()">{{ $t("activities.fight_timeout")}}  {{ fightWaitTime() }}</button>
        <button v-else class="nnnbutton smallbutton" @click="CheckContinuousFight()">{{ $t("activities.fight") }}</button>
        <div v-for="score in scores">
            <FightResultComponent :score="score"></FightResultComponent>
        </div>
    </div>

</template>

<style lang="css" scoped>

</style>